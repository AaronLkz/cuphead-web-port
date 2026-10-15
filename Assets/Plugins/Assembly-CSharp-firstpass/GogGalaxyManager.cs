using System.IO;
using UnityEngine;

[DisallowMultipleComponent]
public class GogGalaxyManager : MonoBehaviour
{
	public string clientID = "50555459027037412";

	public string clientSecret = "ce3ce00794309c249161c1175035225816fc21d423ca3de0dcc098685b685d70";

	private static GogGalaxyManager singleton;

	public static GogGalaxyManager Instance
	{
		get
		{
			if (singleton == null)
			{
				return new GameObject("GogGalaxyManager").AddComponent<GogGalaxyManager>();
			}
			return singleton;
		}
	}

	public static bool IsInitialized()
	{
		return false;
	}

	private string GetGalaxyPeerDLLPath()
	{
		return Path.Combine(Application.dataPath, "Plugins");
	}

	private void Awake()
	{
		if (singleton != null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		singleton = this;
		Object.DontDestroyOnLoad(base.gameObject);
		Debug.Log("[GogGalaxyManager] Running in offline/WebGL stub mode.");
	}

	private void OnDestroy()
	{
		if (!(singleton != this))
		{
			singleton = null;
		}
	}

	private void Update()
	{
	}
}
