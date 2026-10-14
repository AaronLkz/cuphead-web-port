using System;
using System.Collections;
using UnityEngine;
using UnityEngine.U2D;

public class DLCManager
{
	public class AssetBundleLoadWaitInstruction : IEnumerator
	{
		protected bool complete;

		public object Current
		{
			get
			{
				return null;
			}
		}

		public AssetBundle assetBundle { get; protected set; }

		public bool MoveNext()
		{
			return !complete;
		}

		public void Reset()
		{
		}
	}

	private static bool availabilityPromptTriggered;

	private static readonly ulong GOGDLCProductID;

	private static bool dlcAvailable;

	public static bool persistentAssetsLoaded { get; set; }

	public static bool showAvailabilityPrompt { get; private set; }

	public static void RefreshDLC()
	{
		refreshDLC();
	}

	public static void CheckInstallationStatusChanged()
	{
		checkInstallationStatusChanged();
	}

	public static bool DLCEnabled()
	{
		bool enabled = false;
		dlcEnabled(ref enabled);
		return enabled;
	}

	public static string AssetBundlePath()
	{
		string path = null;
		assetBundlePath(ref path);
		return path;
	}

	public static bool UsesAlternateBundleLoadingMechanism()
	{
		bool usesAlternate = false;
		usesAlternateBundleLoadingMechanism(ref usesAlternate);
		return usesAlternate;
	}

	public static AssetBundleLoadWaitInstruction LoadAssetBundle(string path)
	{
		AssetBundleLoadWaitInstruction waitInstruction = null;
		loadAssetBundle(path, ref waitInstruction);
		return waitInstruction;
	}

	public static bool UnloadBundlesImmediately()
	{
		bool unloadImmediately = false;
		unloadBundlesImmediately(ref unloadImmediately);
		return unloadImmediately;
	}

	public static bool CanRedirectToStore()
	{
		return false;
	}

	public static void LaunchStore()
	{
	}

	public static Coroutine[] LoadPersistentAssets()
	{
		if (persistentAssetsLoaded || !DLCEnabled())
		{
			return null;
		}
		persistentAssetsLoaded = true;
		return new Coroutine[1] { AssetLoader<SpriteAtlas>.LoadPersistentAssetsDLC() };
	}

	public static void ResetAvailabilityPrompt()
	{
		availabilityPromptTriggered = true;
		showAvailabilityPrompt = false;
	}

	private static void refreshDLC()
	{
		dlcAvailable = true;
	}

	private static void checkInstallationStatusChanged()
	{
	}

	private static void dlcEnabled(ref bool enabled)
	{
		enabled = dlcAvailable;
	}

	private static void assetBundlePath(ref string path)
	{
		path = Application.streamingAssetsPath;
	}

	private static void usesAlternateBundleLoadingMechanism(ref bool usesAlternate)
	{
		usesAlternate = false;
	}

	private static void unloadBundlesImmediately(ref bool unloadImmediately)
	{
		unloadImmediately = false;
	}

	private static void loadAssetBundle(string path, ref AssetBundleLoadWaitInstruction waitInstruction)
	{
		throw new NotImplementedException();
	}

	static DLCManager()
	{
		GOGDLCProductID = 2048763853uL;
	}
}
