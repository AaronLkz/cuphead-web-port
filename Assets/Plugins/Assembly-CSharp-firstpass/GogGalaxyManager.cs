using System.IO;
using Galaxy.Api;
using UnityEngine;

[DisallowMultipleComponent]
public class GogGalaxyManager : MonoBehaviour
{
	public string clientID = "50555459027037412";

	public string clientSecret = "ce3ce00794309c249161c1175035225816fc21d423ca3de0dcc098685b685d70";

	private static GogGalaxyManager singleton;

	private bool isInitialized;

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
		return singleton != null && singleton.isInitialized;
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
		try
		{
			InitParams initpParams = new InitParams(clientID, clientSecret, GetGalaxyPeerDLLPath());
			GalaxyInstance.Init(initpParams);
		}
		catch (GalaxyInstance.Error error)
		{
			Debug.LogError("Failed to initialize GOG Galaxy: Error = " + error.ToString(), this);
			return;
		}
		Debug.Log("Galaxy SDK was initialized", this);
		isInitialized = true;
	}

	private void OnDestroy()
	{
		if (!(singleton != this))
		{
			singleton = null;
			if (isInitialized)
			{
				GalaxyInstance.Shutdown();
			}
		}
	}

	private void Update()
	{
		if (isInitialized)
		{
			GalaxyInstance.ProcessData();
		}
	}
}
