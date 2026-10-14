using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public static class LocalAchievementsManager
{
	public enum Achievement
	{
		DefeatBoss = 0,
		ParryApprentice = 1,
		ParryMaster = 2,
		ExWin = 3,
		SuperWin = 4,
		ParryChain = 5,
		NoHitsTaken = 6,
		ARankWorld1 = 7,
		ARankWorld2 = 8,
		ARankWorld3 = 9,
		CompleteWorld1 = 10,
		CompleteWorld2 = 11,
		CompleteWorld3 = 12,
		UnlockedAllSupers = 13,
		FoundAllLevelMoney = 14,
		BoughtAllItems = 15,
		CompleteDicePalace = 16,
		ARankWorld4 = 17,
		GoodEnding = 18,
		SRank = 19,
		NewGamePlus = 20,
		FoundSecretPassage = 21,
		SmallPlaneOnlyWin = 22,
		FoundAllMoney = 23,
		PacifistRun = 24,
		NoHitsTakenDicePalace = 25,
		BadEnding = 26,
		CompleteDevil = 27,
		CompleteWorldDLC = 28,
		ARankWorldDLC = 29,
		DefeatBossAsChalice = 30,
		DefeatXBossesAsChalice = 31,
		ChaliceSuperWin = 32,
		DefeatBossDLCWeapon = 33,
		DefeatAllKOG = 34,
		DefeatKOGGauntlet = 35,
		DefeatSaltbaker = 36,
		SRankAnyDLC = 37,
		DefeatBossNoMinions = 38,
		HP9 = 39,
		DefeatDevilPhase2 = 40,
		Paladin = 41
	}

	[Serializable]
	private class AchievementData
	{
		public List<Achievement> unlockedAchievements = new List<Achievement>();

		public int parryCount;
	}

	private static readonly string CloudKey = "cuphead_ach";

	public static readonly Achievement[] DLCAchievements = new Achievement[14]
	{
		Achievement.CompleteWorldDLC,
		Achievement.ARankWorldDLC,
		Achievement.DefeatBossAsChalice,
		Achievement.DefeatXBossesAsChalice,
		Achievement.ChaliceSuperWin,
		Achievement.DefeatBossDLCWeapon,
		Achievement.DefeatAllKOG,
		Achievement.DefeatKOGGauntlet,
		Achievement.DefeatSaltbaker,
		Achievement.SRankAnyDLC,
		Achievement.DefeatBossNoMinions,
		Achievement.HP9,
		Achievement.DefeatDevilPhase2,
		Achievement.Paladin
	};

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<Achievement> AchievementUnlockedEvent__BackingField;

	private static bool initialized;

	private static AchievementData achievementData;

	public static event Action<Achievement> AchievementUnlockedEvent
	{
		add
		{
			Action<Achievement> action = AchievementUnlockedEvent__BackingField;
			Action<Achievement> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref AchievementUnlockedEvent__BackingField, (Action<Achievement>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<Achievement> action = AchievementUnlockedEvent__BackingField;
			Action<Achievement> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref AchievementUnlockedEvent__BackingField, (Action<Achievement>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static void Initialize()
	{
		if (!initialized)
		{
			initialized = true;
			loadFromCloud();
		}
	}

	public static void UnlockAchievement(PlayerId playerId, string achievementName)
	{
		Achievement achievement = (Achievement)Enum.Parse(typeof(Achievement), achievementName);
		if (!IsAchievementUnlocked(achievement))
		{
			achievementData.unlockedAchievements.Add(achievement);
			saveToCloud();
			if (AchievementUnlockedEvent__BackingField != null)
			{
				AchievementUnlockedEvent__BackingField(achievement);
			}
		}
	}

	public static void IncrementStat(PlayerId player, string id, int value)
	{
		if (id == "Parries" && achievementData.parryCount < 100)
		{
			achievementData.parryCount += value;
			bool flag = true;
			if (achievementData.parryCount >= 20)
			{
				UnlockAchievement(PlayerId.Any, "ParryApprentice");
				flag = false;
			}
			if (achievementData.parryCount >= 100)
			{
				UnlockAchievement(PlayerId.Any, "ParryMaster");
				flag = false;
			}
			if (flag)
			{
				saveToCloud();
			}
		}
	}

	public static IList<Achievement> GetUnlockedAchievements()
	{
		return achievementData.unlockedAchievements;
	}

	public static bool IsAchievementUnlocked(Achievement achievement)
	{
		return achievementData.unlockedAchievements.Contains(achievement);
	}

	public static bool IsHiddenAchievement(Achievement achievement)
	{
		return achievement == Achievement.FoundSecretPassage || achievement == Achievement.SmallPlaneOnlyWin || achievement == Achievement.FoundAllMoney || achievement == Achievement.PacifistRun || achievement == Achievement.NoHitsTakenDicePalace || achievement == Achievement.BadEnding || achievement == Achievement.CompleteDevil || achievement == Achievement.DefeatDevilPhase2 || achievement == Achievement.Paladin;
	}

	private static void saveToCloud()
	{
		if (OnlineManager.Instance.Interface.CloudStorageInitialized)
		{
			string value = JsonUtility.ToJson(achievementData);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary[CloudKey] = value;
			OnlineManager.Instance.Interface.SaveCloudData(dictionary, onSavedCloudData);
		}
	}

	private static void onSavedCloudData(bool success)
	{
	}

	private static void loadFromCloud()
	{
		if (OnlineManager.Instance.Interface.CloudStorageInitialized)
		{
			OnlineManager.Instance.Interface.LoadCloudData(new string[1] { CloudKey }, onLoadedCloudData);
		}
	}

	private static void onLoadedCloudData(string[] data, CloudLoadResult result)
	{
		if (result == CloudLoadResult.Failed)
		{
			loadFromCloud();
			return;
		}
		try
		{
			if (result == CloudLoadResult.NoData)
			{
				achievementData = new AchievementData();
				saveToCloud();
			}
			else
			{
				achievementData = JsonUtility.FromJson<AchievementData>(data[0]);
			}
		}
		catch (ArgumentException)
		{
			achievementData = new AchievementData();
		}
	}
}
