using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AssetLoader<T> : MonoBehaviour where T : class
{
	private class AssetContainer<U>
	{
		public U asset;

		public AssetLoaderOption assetOption;

		public AssetContainer(U asset, AssetLoaderOption assetOption)
		{
			this.asset = asset;
			this.assetOption = assetOption;
		}
	}

	private class LoadOperation
	{
		public Coroutine coroutine;

		public List<Action<T>> completionHandlers = new List<Action<T>>();
	}

	protected static AssetLoader<T> Instance;

	[SerializeField]
	private RuntimeSceneAssetDatabase sceneAssetDatabase;

	[SerializeField]
	private AssetLocationDatabase assetLocationDatabase;

	private Dictionary<string, LoadOperation> loadOperations = new Dictionary<string, LoadOperation>();

	private Dictionary<string, AssetContainer<T>> loadedAssets = new Dictionary<string, AssetContainer<T>>();

	private bool _persistentAssetsLoaded;

	public static bool persistentAssetsLoaded
	{
		get
		{
			return Instance._persistentAssetsLoaded;
		}
		private set
		{
			Instance._persistentAssetsLoaded = value;
		}
	}

	private void Awake()
	{
		if (Instance != null)
		{
			throw new Exception("More than one instance found");
		}
		Instance = this;
	}

	private void Start()
	{
		StartCoroutine(loadPersistentAssets());
	}

	private IEnumerator loadPersistentAssets()
	{
		if (sceneAssetDatabase != null)
		{
			foreach (string assetName in sceneAssetDatabase.persistentAssets)
			{
				yield return loadAssetFromAssetBundle(assetName, AssetLoaderOption.PersistInCache(), null);
			}
		}
		persistentAssetsLoaded = true;
	}

	public static string[] GetPreloadAssetNames(string sceneName)
	{
		string[] value;
		if (!Instance.sceneAssetDatabase.sceneAssetMappings.TryGetValue(sceneName, out value))
		{
			return new string[0];
		}
		return value;
	}

	public static Coroutine LoadAsset(string assetName, AssetLoaderOption option)
	{
		return Instance.loadAssetFromAssetBundle(assetName, option, null);
	}

	public static T LoadAssetSynchronous(string assetName, AssetLoaderOption option)
	{
		return Instance.loadAssetFromAssetBundleSynchronous(assetName, option);
	}

	protected abstract Coroutine loadAsset(string assetName, Action<T> completionHandler);

	protected abstract T loadAssetSynchronous(string assetName);

	public static Coroutine LoadPersistentAssetsDLC()
	{
		return Instance.loadPersistentAssetsDLC();
	}

	private Coroutine loadPersistentAssetsDLC()
	{
		return StartCoroutine(loadPersistentAssetsDLC_cr());
	}

	private IEnumerator loadPersistentAssetsDLC_cr()
	{
		foreach (string assetName in sceneAssetDatabase.persistentAssetsDLC)
		{
			yield return loadAssetFromAssetBundle(assetName, AssetLoaderOption.PersistInCache(), null);
		}
	}

	public static T GetCachedAsset(string assetName)
	{
		T asset;
		if (Instance != null && Instance.tryGetAsset(assetName, out asset))
		{
			return asset;
		}
		Debug.LogWarning("[AssetLoader] Asset not cached: " + assetName);
		return null;
	}

	public static void UnloadAssets(params string[] persistentTagsToUnload)
	{
		List<string> list = new List<string>(Instance.loadedAssets.Keys);
		for (int num = list.Count - 1; num >= 0; num--)
		{
			AssetContainer<T> assetContainer = Instance.loadedAssets[list[num]];
			if ((assetContainer.assetOption.type & AssetLoaderOption.Type.PersistInCache) != AssetLoaderOption.Type.None)
			{
				list.RemoveAt(num);
			}
			else if ((assetContainer.assetOption.type & AssetLoaderOption.Type.PersistInCacheTagged) != AssetLoaderOption.Type.None && Array.IndexOf(persistentTagsToUnload, (string)assetContainer.assetOption.context) < 0)
			{
				list.RemoveAt(num);
			}
			else if ((assetContainer.assetOption.type & AssetLoaderOption.Type.DontDestroyOnUnload) == 0)
			{
				Instance.destroyAsset(assetContainer.asset);
			}
		}
		foreach (string item in list)
		{
			Instance.loadedAssets.Remove(item);
		}
	}

	protected abstract void destroyAsset(T asset);

	private void cacheAsset(string assetName, AssetContainer<T> container)
	{
		loadedAssets.Add(assetName, container);
	}

	protected bool tryGetAsset(string assetName, out T asset)
	{
		asset = (T)null;
		AssetContainer<T> value;
		if (loadedAssets.TryGetValue(assetName, out value))
		{
			asset = value.asset;
			return true;
		}
		return false;
	}

	protected Coroutine loadAssetFromAssetBundle(string assetName, AssetLoaderOption option, Action<T> completionAction)
	{
		T asset;
		if (tryGetAsset(assetName, out asset))
		{
			if (completionAction != null)
			{
				completionAction(asset);
			}
			return null;
		}
		if (!DLCManager.DLCEnabled() && assetLocationDatabase.dlcAssets.Contains(assetName))
		{
			if (completionAction != null)
			{
				completionAction((T)null);
			}
			return null;
		}
		LoadOperation value;
		if (!loadOperations.TryGetValue(assetName, out value))
		{
			value = new LoadOperation();
			loadOperations.Add(assetName, value);
			value.coroutine = loadAsset(assetName, (T val) =>
			{
				cacheAsset(assetName, new AssetContainer<T>(val, option));
				LoadOperation loadOperation = loadOperations[assetName];
				foreach (Action<T> completionHandler in loadOperation.completionHandlers)
				{
					if (completionHandler != null)
					{
						completionHandler(val);
					}
				}
				loadOperations.Remove(assetName);
			});
		}
		value.completionHandlers.Add(completionAction);
		return value.coroutine;
	}

	protected T loadAssetFromAssetBundleSynchronous(string assetName, AssetLoaderOption option)
	{
		T asset;
		if (tryGetAsset(assetName, out asset))
		{
			return asset;
		}
		T val = loadAssetSynchronous(assetName);
		cacheAsset(assetName, new AssetContainer<T>(val, option));
		return val;
	}

	public static bool IsDLCAsset(string assetName)
	{
		return Instance.assetLocationDatabase.dlcAssets.Contains(assetName);
	}

	public static List<string> DEBUG_GetLoadedAssets()
	{
		List<string> list = new List<string>(Instance.loadedAssets.Count);
		foreach (KeyValuePair<string, AssetContainer<T>> loadedAsset in Instance.loadedAssets)
		{
			AssetContainer<T> value = loadedAsset.Value;
			string text = string.Format("{0} ({1})", loadedAsset.Key, value.assetOption.type.ToString());
			if ((value.assetOption.type & AssetLoaderOption.Type.PersistInCacheTagged) != AssetLoaderOption.Type.None)
			{
				text += string.Format(" [Tag={0}]", value.assetOption.context);
			}
			list.Add(text);
		}
		return list;
	}
}
