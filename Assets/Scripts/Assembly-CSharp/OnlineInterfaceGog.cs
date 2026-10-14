using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class OnlineInterfaceGog : OnlineInterface
{
	private string SavePath
	{
		get
		{
			if (Application.platform == RuntimePlatform.WebGLPlayer)
			{
				return Path.Combine(Application.persistentDataPath, "Cuphead/");
			}
			if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
			{
				return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Cuphead\\");
			}
			if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
			{
				return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Library/Application Support/unity.Studio MDHR.Cuphead/Cuphead/");
			}
			if (Application.platform == RuntimePlatform.LinuxEditor || Application.platform == RuntimePlatform.LinuxPlayer)
			{
				return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Cuphead/");
			}
			return Path.Combine(Application.persistentDataPath, "Cuphead/");
		}
	}

	public OnlineUser MainUser
	{
		get
		{
			return null;
		}
	}

	public OnlineUser SecondaryUser
	{
		get
		{
			return null;
		}
	}

	public bool CloudStorageInitialized
	{
		get
		{
			return true;
		}
	}

	public bool SupportsMultipleUsers
	{
		get
		{
			return false;
		}
	}

	public bool SupportsUserSignIn
	{
		get
		{
			return false;
		}
	}

	public event SignInEventHandler OnUserSignedIn;

	public event SignOutEventHandler OnUserSignedOut;

	public void Init()
	{
		Debug.Log("[OnlineInterface] Initialized Web/Offline mode.");
	}

	public void Reset()
	{
	}

	public void SignInUser(bool silent, PlayerId player, ulong controllerId)
	{
		if (OnUserSignedIn != null)
		{
			OnUserSignedIn(null);
		}
	}

	public void SwitchUser(PlayerId player, ulong controllerId)
	{
	}

	public OnlineUser GetUserForController(ulong id)
	{
		return null;
	}

	public List<ulong> GetControllersForUser(PlayerId player)
	{
		return null;
	}

	public bool IsUserSignedIn(PlayerId player)
	{
		return false;
	}

	public OnlineUser GetUser(PlayerId player)
	{
		return null;
	}

	public void SetUser(PlayerId player, OnlineUser user)
	{
	}

	public Texture2D GetProfilePic(PlayerId player)
	{
		return null;
	}

	public void GetAchievement(PlayerId player, string id, AchievementEventHandler achievementRetrievedHandler)
	{
	}

	public void UnlockAchievement(PlayerId player, string id)
	{
		Debug.Log("[OnlineInterface] Unlocked Achievement: " + id);
	}

	public void SyncAchievementsAndStats()
	{
	}

	public void SetStat(PlayerId player, string id, int value)
	{
	}

	public void SetStat(PlayerId player, string id, float value)
	{
	}

	public void SetStat(PlayerId player, string id, string value)
	{
	}

	public void IncrementStat(PlayerId player, string id, int value)
	{
	}

	public void SetRichPresence(PlayerId player, string id, bool active)
	{
	}

	public void SetRichPresenceActive(PlayerId player, bool active)
	{
	}

	public void InitializeCloudStorage(PlayerId player, InitializeCloudStoreHandler handler)
	{
		if (handler != null)
		{
			handler(true);
		}
	}

	public void UninitializeCloudStorage()
	{
	}

	public void SaveCloudData(IDictionary<string, string> data, SaveCloudDataHandler handler)
	{
		string savePath = SavePath;
		if (!Directory.Exists(savePath))
		{
			Directory.CreateDirectory(savePath);
		}
		foreach (string key in data.Keys)
		{
			try
			{
				using (StreamWriter streamWriter = new StreamWriter(Path.Combine(savePath, key + ".sav")))
				{
					streamWriter.Write(data[key]);
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning("[OnlineInterface] Save failed for " + key + ": " + ex.Message);
				if (Cuphead.Current != null)
				{
					Cuphead.Current.StartCoroutine(saveFailed_cr(handler));
				}
				else if (handler != null)
				{
					handler(false);
				}
				return;
			}
		}
		if (handler != null)
		{
			handler(true);
		}
	}

	private IEnumerator saveFailed_cr(SaveCloudDataHandler handler)
	{
		yield return new WaitForSeconds(0.25f);
		if (handler != null)
		{
			handler(false);
		}
	}

	public void LoadCloudData(string[] keys, LoadCloudDataHandler handler)
	{
		string[] array = new string[keys.Length];
		string savePath = SavePath;
		for (int i = 0; i < array.Length; i++)
		{
			string filePath = Path.Combine(savePath, keys[i] + ".sav");
			if (File.Exists(filePath))
			{
				try
				{
					using (TextReader textReader = new StreamReader(filePath))
					{
						array[i] = textReader.ReadToEnd();
					}
				}
				catch (Exception ex)
				{
					Debug.LogWarning("[OnlineInterface] Load failed for " + keys[i] + ": " + ex.Message);
					if (handler != null)
					{
						handler(array, CloudLoadResult.Failed);
					}
					return;
				}
				continue;
			}
			if (handler != null)
			{
				handler(array, CloudLoadResult.NoData);
			}
			return;
		}
		if (handler != null)
		{
			handler(array, CloudLoadResult.Success);
		}
	}

	public void UpdateControllerMapping()
	{
	}

	public bool ControllerMappingChanged()
	{
		return false;
	}
}
