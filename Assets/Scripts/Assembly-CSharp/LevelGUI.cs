using System;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public class LevelGUI : AbstractMonoBehaviour
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action DebugOnDisableGuiEvent__BackingField;

	[SerializeField]
	private Canvas canvas;

	[SerializeField]
	private LevelPauseGUI pause;

	[SerializeField]
	private LevelGameOverGUI gameOver;

	[SerializeField]
	private OptionsGUI optionsPrefab;

	[SerializeField]
	private RectTransform optionsRoot;

	[SerializeField]
	private RestartTowerConfirmGUI restartTowerConfirmPrefab;

	[SerializeField]
	private RectTransform restartTowerConfirmRoot;

	[SerializeField]
	private AchievementsGUI achievementsPrefab;

	[SerializeField]
	private RectTransform achievementsRoot;

	private OptionsGUI options;

	private AchievementsGUI achievements;

	private RestartTowerConfirmGUI restartTowerConfirm;

	[Space(10f)]
	[SerializeField]
	private CupheadUICamera uiCameraPrefab;

	private CupheadUICamera uiCamera;

	public static LevelGUI Current { get; private set; }

	public Canvas Canvas
	{
		get
		{
			return canvas;
		}
	}

	public static event Action DebugOnDisableGuiEvent
	{
		add
		{
			Action action = DebugOnDisableGuiEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref DebugOnDisableGuiEvent__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = DebugOnDisableGuiEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref DebugOnDisableGuiEvent__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static void DebugDisableGUI()
	{
		if (DebugOnDisableGuiEvent__BackingField != null)
		{
			DebugOnDisableGuiEvent__BackingField();
		}
	}

	protected override void Awake()
	{
		base.Awake();
		Current = this;
	}

	private void Start()
	{
		uiCamera = UnityEngine.Object.Instantiate(uiCameraPrefab);
		uiCamera.transform.SetParent(base.transform);
		uiCamera.transform.ResetLocalTransforms();
		canvas.worldCamera = uiCamera.camera;
	}

	private void OnDestroy()
	{
		pause = null;
		options = null;
		restartTowerConfirm = null;
		if (Current == this)
		{
			Current = null;
		}
	}

	public void LevelInit()
	{
		options = optionsPrefab.InstantiatePrefab<OptionsGUI>();
		options.rectTransform.SetParent(optionsRoot, false);
		if (PlatformHelper.ShowAchievements)
		{
			achievements = achievementsPrefab.InstantiatePrefab<AchievementsGUI>();
			achievements.rectTransform.SetParent(achievementsRoot, false);
		}
		if (Level.IsTowerOfPower)
		{
			restartTowerConfirm = restartTowerConfirmPrefab.InstantiatePrefab<RestartTowerConfirmGUI>();
			restartTowerConfirm.rectTransform.SetParent(restartTowerConfirmRoot, false);
		}
		pause.Init(true, options, achievements, restartTowerConfirm);
	}
}
