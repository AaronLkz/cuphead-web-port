using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.U2D;

public class AssetBundleLoader : MonoBehaviour
{
	private enum AssetBundleLocation
	{
		StreamingAssets = 0,
		DLC = 1
	}

	private class AssetBundleContainer
	{
		public AssetBundle assetBundle;

		public AssetBundleLocation location;

		public AssetBundleContainer(AssetBundle assetBundle, AssetBundleLocation location)
		{
			this.assetBundle = assetBundle;
			this.location = location;
		}
	}

	public static readonly string AssetBundlePrefixSpriteAtlas = "atlas_";

	public static readonly string AssetBundlePrefixMusic = "music_";

	public static readonly string AssetBundlePrefixFont = "font_";

	public static readonly string AssetBundlePrefixTMPFont = "tmpfont_";

	public static readonly string AssetBundlePrefixTexture = "tex_";

	private static AssetBundleLoader Instance;

	[SerializeField]
	private AssetLocationDatabase atlasLocationDatabase;

	[SerializeField]
	private AssetLocationDatabase musicLocationDatabase;

	private Dictionary<string, AssetBundleContainer> loadedBundles = new Dictionary<string, AssetBundleContainer>();

	public static int loadCounter { get; private set; }

	private void Awake()
	{
		if (Instance != null)
		{
			throw new Exception("Should only be one instance");
		}
		Instance = this;
	}

	public static void UnloadAssetBundles()
	{
		foreach (KeyValuePair<string, AssetBundleContainer> loadedBundle in Instance.loadedBundles)
		{
			loadedBundle.Value.assetBundle.Unload(false);
		}
		Instance.loadedBundles.Clear();
	}

	public static Coroutine LoadSpriteAtlas(string atlasName, Action<SpriteAtlas> completionHandler)
	{
		AssetBundleLocation location = AssetBundleLocation.StreamingAssets;
		if (Instance.atlasLocationDatabase.dlcAssets.Contains(atlasName))
		{
			location = AssetBundleLocation.DLC;
		}
		string spriteAtlasBundleName = GetSpriteAtlasBundleName(atlasName);
		return Instance.StartCoroutine(Instance.loadAsset(spriteAtlasBundleName, location, atlasName, completionHandler));
	}

	public static Coroutine LoadMusic(string audioClipName, Action<AudioClip> completionHandler)
	{
		AssetBundleLocation location = AssetBundleLocation.StreamingAssets;
		if (Instance.musicLocationDatabase.dlcAssets.Contains(audioClipName))
		{
			location = AssetBundleLocation.DLC;
		}
		string musicBundleName = GetMusicBundleName(audioClipName);
		return Instance.StartCoroutine(Instance.loadAsset(musicBundleName, location, audioClipName, completionHandler));
	}

	public static Coroutine LoadFont(string bundleName, string assetName, Action<Font> completionHandler)
	{
		AssetBundleLocation location = AssetBundleLocation.StreamingAssets;
		bundleName = AssetBundlePrefixFont + bundleName.ToLowerInvariant();
		return Instance.StartCoroutine(Instance.loadAsset(bundleName, location, assetName, completionHandler));
	}

	public static Coroutine LoadTMPFont(string bundleName, Action<UnityEngine.Object[]> completionHandler)
	{
		AssetBundleLocation location = AssetBundleLocation.StreamingAssets;
		bundleName = AssetBundlePrefixTMPFont + bundleName.ToLowerInvariant();
		return Instance.StartCoroutine(Instance.loadAllAssets(bundleName, location, completionHandler));
	}

	public static Coroutine LoadTextures(string bundleName, Action<Texture2D[]> completionHandler)
	{
		AssetBundleLocation location = AssetBundleLocation.StreamingAssets;
		bundleName = AssetBundlePrefixTexture + bundleName.ToLowerInvariant();
		return Instance.StartCoroutine(Instance.loadAllAssets(bundleName, location, completionHandler));
	}

	public static Texture2D[] LoadTexturesSynchronous(string bundleName)
	{
		AssetBundleLocation location = AssetBundleLocation.StreamingAssets;
		bundleName = AssetBundlePrefixTexture + bundleName.ToLowerInvariant();
		return Instance.loadAllAssetsSynchronous<Texture2D>(bundleName, location);
	}

	private IEnumerator loadAssetBundle(string assetBundleName, AssetBundleLocation location)
	{
		loadCounter++;
		string path = getBasePath(location);
		path = Path.Combine(path, "AssetBundles");
		path = Path.Combine(path, assetBundleName).Replace('\\', '/');
		AssetBundle assetBundle = null;
#if UNITY_WEBGL
		using (UnityEngine.Networking.UnityWebRequest uwr = UnityEngine.Networking.UnityWebRequestAssetBundle.GetAssetBundle(path))
		{
			yield return uwr.SendWebRequest();
			if (uwr.isNetworkError || uwr.isHttpError)
			{
				Debug.LogError("[AssetBundleLoader WebGL] Error loading bundle: " + uwr.error + " at " + path);
			}
			else
			{
				assetBundle = UnityEngine.Networking.DownloadHandlerAssetBundle.GetContent(uwr);
			}
		}
#else
		if (location == AssetBundleLocation.DLC && DLCManager.UsesAlternateBundleLoadingMechanism())
		{
			DLCManager.AssetBundleLoadWaitInstruction waitInstruction = DLCManager.LoadAssetBundle(path);
			yield return waitInstruction;
			assetBundle = waitInstruction.assetBundle;
		}
		else
		{
			AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(path);
			yield return request;
			assetBundle = request.assetBundle;
		}
#endif
		if (assetBundle != null)
		{
			loadedBundles.Add(assetBundleName, new AssetBundleContainer(assetBundle, location));
		}
		loadCounter--;
	}

	private AssetBundleContainer loadAssetBundleSynchronous(string assetBundleName, AssetBundleLocation location)
	{
		string basePath = getBasePath(location);
		basePath = Path.Combine(basePath, "AssetBundles");
		basePath = Path.Combine(basePath, assetBundleName).Replace('\\', '/');
		AssetBundle assetBundle = null;
#if !UNITY_WEBGL
		assetBundle = AssetBundle.LoadFromFile(basePath);
#else
		Debug.LogWarning("[AssetBundleLoader WebGL] Synchronous asset bundle loading called in WebGL for: " + assetBundleName);
#endif
		AssetBundleContainer assetBundleContainer = new AssetBundleContainer(assetBundle, location);
		loadedBundles.Add(assetBundleName, assetBundleContainer);
		return assetBundleContainer;
	}

	private IEnumerator loadAsset<T>(string assetBundleName, AssetBundleLocation location, string assetName, Action<T> completionHandler) where T : UnityEngine.Object
	{
		loadCounter++;
		AssetBundleContainer assetBundleContainer;
		if (!loadedBundles.TryGetValue(assetBundleName, out assetBundleContainer))
		{
			yield return StartCoroutine(loadAssetBundle(assetBundleName, location));
			if (!loadedBundles.TryGetValue(assetBundleName, out assetBundleContainer) || assetBundleContainer == null || assetBundleContainer.assetBundle == null)
			{
				Debug.LogError("[AssetBundleLoader] Bundle failed to load: " + assetBundleName);
				if (completionHandler != null)
				{
					completionHandler(null);
				}
				loadCounter--;
				yield break;
			}
		}
		AssetBundleRequest assetRequest = assetBundleContainer.assetBundle.LoadAssetAsync<T>(assetName);
		yield return assetRequest;
		completionHandler(assetRequest.asset as T);
		if (assetBundleContainer.location == AssetBundleLocation.DLC && DLCManager.UnloadBundlesImmediately() && typeof(T) == typeof(SpriteAtlas))
		{
			loadedBundles.Remove(assetBundleContainer.assetBundle.name);
			assetBundleContainer.assetBundle.Unload(false);
		}
		loadCounter--;
	}

	private IEnumerator loadAllAssets<T>(string assetBundleName, AssetBundleLocation location, Action<T[]> completionHandler) where T : UnityEngine.Object
	{
		loadCounter++;
		AssetBundleContainer assetBundleContainer;
		if (!loadedBundles.TryGetValue(assetBundleName, out assetBundleContainer))
		{
			yield return StartCoroutine(loadAssetBundle(assetBundleName, location));
			if (!loadedBundles.TryGetValue(assetBundleName, out assetBundleContainer) || assetBundleContainer == null || assetBundleContainer.assetBundle == null)
			{
				Debug.LogError("[AssetBundleLoader] Bundle failed to load: " + assetBundleName);
				if (completionHandler != null)
				{
					completionHandler(new T[0]);
				}
				loadCounter--;
				yield break;
			}
		}
		AssetBundleRequest assetRequest = assetBundleContainer.assetBundle.LoadAllAssetsAsync<T>();
		yield return assetRequest;
		UnityEngine.Object[] allAssets = assetRequest.allAssets;
		T[] castAssets = new T[allAssets.Length];
		for (int i = 0; i < allAssets.Length; i++)
		{
			castAssets[i] = (T)allAssets[i];
		}
		completionHandler(castAssets);
		loadCounter--;
	}

	private T[] loadAllAssetsSynchronous<T>(string assetBundleName, AssetBundleLocation location) where T : UnityEngine.Object
	{
		AssetBundleContainer value;
		if (!loadedBundles.TryGetValue(assetBundleName, out value))
		{
			value = loadAssetBundleSynchronous(assetBundleName, location);
		}
		if (value == null || value.assetBundle == null)
		{
			return new T[0];
		}
		return value.assetBundle.LoadAllAssets<T>();
	}

	private static string getBasePath(AssetBundleLocation location)
	{
		if (location == AssetBundleLocation.DLC)
		{
			return DLCManager.AssetBundlePath();
		}
		return Application.streamingAssetsPath;
	}

	public static string GetSpriteAtlasBundleName(string atlasName)
	{
		return AssetBundlePrefixSpriteAtlas + atlasName.ToLowerInvariant();
	}

	public static string GetMusicBundleName(string audioClipName)
	{
		return AssetBundlePrefixMusic + audioClipName.ToLowerInvariant();
	}

	public static List<string> DEBUG_LoadedAssetBundles()
	{
		return new List<string>(Instance.loadedBundles.Keys);
	}
}
