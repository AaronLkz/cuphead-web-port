using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public abstract class Level : AbstractPausableComponent
{
	public enum Type
	{
		Battle = 0,
		Tutorial = 1,
		Platforming = 2
	}

	public enum Mode
	{
		Easy = 0,
		Normal = 1,
		Hard = 2
	}

	[Serializable]
	public class Bounds
	{
		public enum Side
		{
			Left = 0,
			Right = 1,
			Top = 2,
			Bottom = 3
		}

		public int left;

		public int right;

		public int top;

		public int bottom;

		public bool topEnabled = true;

		public bool bottomEnabled = true;

		public bool leftEnabled = true;

		public bool rightEnabled = true;

		public Dictionary<Side, BoxCollider2D> colliders = new Dictionary<Side, BoxCollider2D>();

		public int Width
		{
			get
			{
				return left + right;
			}
		}

		public int Height
		{
			get
			{
				return top + bottom;
			}
		}

		public Vector2 Center
		{
			get
			{
				return new Vector2(right - left, top - bottom) / 2f;
			}
		}

		public Bounds()
		{
			left = 0;
			right = 0;
			top = 0;
			bottom = 0;
		}

		public Bounds(int left, int right, int top, int bottom)
		{
			this.left = left;
			this.right = right;
			this.top = top;
			this.bottom = bottom;
		}

		public void SetColliderPositions()
		{
			Rect rect = new Rect
			{
				xMin = -left,
				xMax = right,
				yMin = -bottom,
				yMax = top
			};
			if (colliders.ContainsKey(Side.Left) && colliders[Side.Left] != null)
			{
				colliders[Side.Left].transform.position = new Vector2(-left - 200, rect.center.y);
			}
			if (colliders.ContainsKey(Side.Right) && colliders[Side.Right] != null)
			{
				colliders[Side.Right].transform.position = new Vector2(right + 200, rect.center.y);
			}
			if (colliders.ContainsKey(Side.Top) && colliders[Side.Top] != null)
			{
				colliders[Side.Top].transform.position = new Vector2(rect.center.x, top + 200);
			}
			if (colliders.ContainsKey(Side.Bottom) && colliders[Side.Bottom] != null)
			{
				colliders[Side.Bottom].transform.position = new Vector2(rect.center.x, -bottom - 200);
			}
		}

		public int GetValue(Side side)
		{
			switch (side)
			{
			default:
				return bottom;
			case Side.Top:
				return top;
			case Side.Left:
				return left;
			case Side.Right:
				return right;
			}
		}

		public void SetValue(Side side, int value)
		{
			switch (side)
			{
			default:
				bottom = value;
				break;
			case Side.Top:
				top = value;
				break;
			case Side.Left:
				left = value;
				break;
			case Side.Right:
				right = value;
				break;
			}
		}

		public bool GetEnabled(Side side)
		{
			switch (side)
			{
			default:
				return bottomEnabled;
			case Side.Top:
				return topEnabled;
			case Side.Left:
				return leftEnabled;
			case Side.Right:
				return rightEnabled;
			}
		}

		public void SetEnabled(Side side, bool value)
		{
			switch (side)
			{
			default:
				bottomEnabled = value;
				break;
			case Side.Top:
				topEnabled = value;
				break;
			case Side.Left:
				leftEnabled = value;
				break;
			case Side.Right:
				rightEnabled = value;
				break;
			}
		}

		public Bounds Copy()
		{
			return MemberwiseClone() as Bounds;
		}
	}

	[Serializable]
	public class Spawns
	{
		public Vector2 playerOne = new Vector2(-460f, 0f);

		public Vector2 playerTwo = new Vector2(-580f, 0f);

		public Vector2 playerOneSingle = new Vector2(-520f, 0f);

		public Vector2 this[int i]
		{
			get
			{
				switch (i)
				{
				case 0:
					return playerOne;
				case 1:
					return playerTwo;
				case 2:
					return playerOneSingle;
				default:
					Debug.LogError("Spawn index '" + i + "' not in range");
					return Vector2.zero;
				}
			}
		}
	}

	[Serializable]
	public class Camera
	{
		public CupheadLevelCamera.Mode mode = CupheadLevelCamera.Mode.Relative;

		[Space(10f)]
		[Range(0.5f, 2f)]
		public float zoom = 1f;

		[Space(10f)]
		public bool moveX;

		public bool moveY;

		public bool stabilizeY;

		public float stabilizePaddingTop = 50f;

		public float stabilizePaddingBottom = 100f;

		[Space(10f)]
		public bool colliders;

		[Space(10f)]
		public Bounds bounds;

		[HideInInspector]
		public VectorPath path;

		public bool pathMovesOnlyForward;

		public Camera(CupheadLevelCamera.Mode mode, int left, int right, int top, int bottom)
		{
			this.mode = mode;
			bounds = new Bounds(left, right, top, bottom);
		}
	}

	public class GoalTimes
	{
		public readonly float easy;

		public readonly float normal;

		public readonly float hard;

		public GoalTimes(float easy, float normal, float hard)
		{
			this.easy = easy;
			this.normal = normal;
			this.hard = hard;
		}
	}

	[Serializable]
	public class IntroProperties
	{
		[NonSerialized]
		public bool introComplete;

		[NonSerialized]
		public bool readyComplete;

		public void OnIntroAnimComplete()
		{
			introComplete = true;
		}

		public void OnReadyAnimComplete()
		{
			readyComplete = true;
		}
	}

	public class Timeline
	{
		public class Event
		{
			public string name { get; private set; }

			public float percentage { get; private set; }

			public Event(string name, float percentage)
			{
				this.name = name;
				this.percentage = percentage;
			}
		}

		public float health;

		public float damage { get; private set; }

		public List<Event> events { get; private set; }

		public float cuphead { get; private set; }

		public float mugman { get; private set; }

		public Timeline()
		{
			health = 0f;
			damage = 0f;
			cuphead = -1f;
			mugman = -1f;
			events = new List<Event>();
		}

		public int GetHealthOfLastEvent()
		{
			int num = 0;
			float num2 = 1f;
			for (int i = 0; i < events.Count; i++)
			{
				if (events[i].percentage < num2)
				{
					num2 = events[i].percentage;
				}
			}
			return (int)(health * (1f - num2));
		}

		public void DealDamage(float damage)
		{
			this.damage += damage;
		}

		public void OnPlayerDeath(PlayerId playerId)
		{
			if (playerId == PlayerId.PlayerOne || playerId != PlayerId.PlayerTwo)
			{
				if (PlayerManager.player1IsMugman)
				{
					mugman = damage;
				}
				else
				{
					cuphead = damage;
				}
			}
			else if (PlayerManager.player1IsMugman)
			{
				cuphead = damage;
			}
			else
			{
				mugman = damage;
			}
		}

		public void OnPlayerRevive(PlayerId playerId)
		{
			if (playerId == PlayerId.PlayerOne || playerId != PlayerId.PlayerTwo)
			{
				if (PlayerManager.player1IsMugman)
				{
					mugman = -1f;
				}
				else
				{
					cuphead = -1f;
				}
			}
			else if (PlayerManager.player1IsMugman)
			{
				cuphead = -1f;
			}
			else
			{
				mugman = -1f;
			}
		}

		public void SetPlayerDamage(PlayerId playerId, float value)
		{
			if (playerId == PlayerId.PlayerOne || playerId != PlayerId.PlayerTwo)
			{
				if (PlayerManager.player1IsMugman)
				{
					mugman = value;
				}
				else
				{
					cuphead = value;
				}
			}
			else if (PlayerManager.player1IsMugman)
			{
				cuphead = value;
			}
			else
			{
				mugman = value;
			}
		}

		public void AddEvent(Event e)
		{
			events.Add(e);
		}

		public void AddEventAtHealth(string eventName, int targetHealth)
		{
			float percentage = 1f - (float)targetHealth / health;
			AddEvent(new Event(eventName, percentage));
		}
	}

	private const int BOUND_COLLIDER_SIZE = 400;

	private const float IRIS_NO_INTRO_DELAY = 0.4f;

	private const float IRIS_OPEN_DELAY = 1f;

	private const int PLAYER_DEATH_DELAY = 5;

	public const string GENERIC_STATE_NAME = "Generic";

	public static readonly Levels[] world1BossLevels;

	public static readonly Levels[] world2BossLevels;

	public static readonly Levels[] world3BossLevels;

	public static readonly Levels[] world4BossLevels;

	public static readonly Levels[] world4MiniBossLevels;

	public static readonly Levels[] worldDLCBossLevels;

	public static readonly Levels[] worldDLCBossLevelsWithSaltbaker;

	public static readonly Levels[] platformingLevels;

	public static readonly Levels[] kingOfGamesLevels;

	public static readonly Levels[] kingOfGamesLevelsWithCastle;

	public static readonly Levels[] chaliceLevels;

	public LevelResources LevelResources;

	[SerializeField]
	protected Type type;

	[SerializeField]
	public PlayerMode playerMode;

	[SerializeField]
	protected bool allowMultiplayer = true;

	[SerializeField]
	public bool blockChalice;

	[SerializeField]
	protected IntroProperties intro;

	[SerializeField]
	protected Spawns spawns;

	[SerializeField]
	protected Bounds bounds = new Bounds(640, 640, 360, 200);

	public int playerShadowSortingOrder;

	[SerializeField]
	protected Camera camera = new Camera(CupheadLevelCamera.Mode.Lerp, 640, 640, 360, 360);

	protected LevelGUI gui;

	protected LevelHUD hud;

	protected AbstractPlayerController[] players;

	protected Transform collidersRoot;

	protected GoalTimes goalTimes;

	protected bool waitingForPlayerJoin;

	protected bool isMausoleum;

	protected bool isDevil;

	protected bool isTowerOfPower;

	protected bool secretTriggered;

	public int BGMPlaylistCurrent;

	private readonly Vector3 player1PlaneSpawnPos = new Vector3(-550f, 74.3f);

	private readonly Vector3 player2PlaneSpawnPos = new Vector3(-450f, -79.8f);

	private int playerDeathDelayFrames;

	private bool playerIsDead;

	private bool player1HeldJump;

	private bool player2HeldJump;

	private bool player1HeldSuper;

	private bool player2HeldSuper;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action OnLevelStartEvent__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action OnLevelEndEvent__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action OnPlatformingLevelAwakeEvent__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action OnStateChangedEvent__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action OnWinEvent__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action OnPreWinEvent__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action OnLoseEvent__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action OnPreLoseEvent__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action OnTransitionInCompleteEvent__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action OnIntroEvent__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action OnBossDeathExplosionsEvent__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action OnBossDeathExplosionsEndEvent__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action OnBossDeathExplosionsFalloffEvent__BackingField;

	public static Level Current { get; private set; }

	public static Mode CurrentMode { get; private set; }

	public static bool PreviouslyWon { get; private set; }

	public static bool Won { get; private set; }

	public static LevelScoringData.Grade Grade { get; private set; }

	public static LevelScoringData.Grade PreviousGrade { get; private set; }

	public static Mode Difficulty { get; private set; }

	public static Mode PreviousDifficulty { get; private set; }

	public static LevelScoringData ScoringData { get; private set; }

	public static Levels PreviousLevel { get; private set; }

	public static Type PreviousLevelType { get; private set; }

	public static bool IsDicePalace { get; protected set; }

	public static bool IsDicePalaceMain { get; protected set; }

	public static bool SuperUnlocked { get; protected set; }

	public static bool OverrideDifficulty { get; protected set; }

	public static bool IsChessBoss { get; protected set; }

	public static bool IsTowerOfPower { get; protected set; }

	public static bool IsTowerOfPowerMain { get; protected set; }

	public static bool IsGraveyard { get; set; }

	public Mode mode { get; protected set; }

	public bool defeatedMinion { get; set; }

	public bool PlayersCreated { get; private set; }

	public bool Initialized { get; private set; }

	public bool Started { get; private set; }

	public bool[] BlockChaliceCharm { get; private set; }

	public float LevelTime { get; private set; }

	public int Ground
	{
		get
		{
			return -bounds.bottom;
		}
	}

	public int Ceiling
	{
		get
		{
			return bounds.top;
		}
	}

	public int Left
	{
		get
		{
			return -bounds.left;
		}
	}

	public int Right
	{
		get
		{
			return bounds.right;
		}
	}

	public int Width
	{
		get
		{
			return bounds.left + bounds.right;
		}
	}

	public int Height
	{
		get
		{
			return bounds.top + bounds.bottom;
		}
	}

	public Type LevelType
	{
		get
		{
			return type;
		}
	}

	public bool CameraRotates { get; protected set; }

	public bool IntroComplete
	{
		get
		{
			return intro.introComplete;
		}
	}

	public Timeline timeline { get; protected set; }

	public abstract Levels CurrentLevel { get; }

	public abstract Scenes CurrentScene { get; }

	public Camera CameraSettings
	{
		get
		{
			return camera;
		}
	}

	public abstract Sprite BossPortrait { get; }

	public abstract string BossQuote { get; }

	public bool Ending { get; protected set; }

	public static bool IsInBossesHub
	{
		get
		{
			return IsDicePalace || IsDicePalaceMain || IsTowerOfPower;
		}
	}

	protected virtual float LevelIntroTime
	{
		get
		{
			return 1f;
		}
	}

	protected virtual float BossDeathTime
	{
		get
		{
			return 2f;
		}
	}

	public event Action OnLevelStartEvent
	{
		add
		{
			Action action = OnLevelStartEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnLevelStartEvent__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnLevelStartEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnLevelStartEvent__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public event Action OnLevelEndEvent
	{
		add
		{
			Action action = OnLevelEndEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnLevelEndEvent__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnLevelEndEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnLevelEndEvent__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public event Action OnPlatformingLevelAwakeEvent
	{
		add
		{
			Action action = OnPlatformingLevelAwakeEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnPlatformingLevelAwakeEvent__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnPlatformingLevelAwakeEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnPlatformingLevelAwakeEvent__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public event Action OnStateChangedEvent
	{
		add
		{
			Action action = OnStateChangedEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnStateChangedEvent__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnStateChangedEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnStateChangedEvent__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public event Action OnWinEvent
	{
		add
		{
			Action action = OnWinEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnWinEvent__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnWinEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnWinEvent__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public event Action OnPreWinEvent
	{
		add
		{
			Action action = OnPreWinEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnPreWinEvent__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnPreWinEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnPreWinEvent__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public event Action OnLoseEvent
	{
		add
		{
			Action action = OnLoseEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnLoseEvent__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnLoseEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnLoseEvent__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public event Action OnPreLoseEvent
	{
		add
		{
			Action action = OnPreLoseEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnPreLoseEvent__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnPreLoseEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnPreLoseEvent__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public event Action OnTransitionInCompleteEvent
	{
		add
		{
			Action action = OnTransitionInCompleteEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnTransitionInCompleteEvent__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnTransitionInCompleteEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnTransitionInCompleteEvent__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public event Action OnIntroEvent
	{
		add
		{
			Action action = OnIntroEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnIntroEvent__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnIntroEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnIntroEvent__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public event Action OnBossDeathExplosionsEvent
	{
		add
		{
			Action action = OnBossDeathExplosionsEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnBossDeathExplosionsEvent__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnBossDeathExplosionsEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnBossDeathExplosionsEvent__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public event Action OnBossDeathExplosionsEndEvent
	{
		add
		{
			Action action = OnBossDeathExplosionsEndEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnBossDeathExplosionsEndEvent__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnBossDeathExplosionsEndEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnBossDeathExplosionsEndEvent__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public event Action OnBossDeathExplosionsFalloffEvent
	{
		add
		{
			Action action = OnBossDeathExplosionsFalloffEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnBossDeathExplosionsFalloffEvent__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnBossDeathExplosionsFalloffEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnBossDeathExplosionsFalloffEvent__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	static Level()
	{
		world1BossLevels = new Levels[5]
		{
			Levels.Veggies,
			Levels.Slime,
			Levels.FlyingBlimp,
			Levels.Flower,
			Levels.Frogs
		};
		world2BossLevels = new Levels[5]
		{
			Levels.Baroness,
			Levels.Clown,
			Levels.FlyingGenie,
			Levels.Dragon,
			Levels.FlyingBird
		};
		world3BossLevels = new Levels[7]
		{
			Levels.Bee,
			Levels.Pirate,
			Levels.SallyStagePlay,
			Levels.Mouse,
			Levels.Robot,
			Levels.FlyingMermaid,
			Levels.Train
		};
		world4BossLevels = new Levels[2]
		{
			Levels.DicePalaceMain,
			Levels.Devil
		};
		world4MiniBossLevels = new Levels[9]
		{
			Levels.DicePalaceBooze,
			Levels.DicePalaceChips,
			Levels.DicePalaceCigar,
			Levels.DicePalaceDomino,
			Levels.DicePalaceEightBall,
			Levels.DicePalaceFlyingHorse,
			Levels.DicePalaceFlyingMemory,
			Levels.DicePalaceRabbit,
			Levels.DicePalaceRoulette
		};
		worldDLCBossLevels = new Levels[5]
		{
			Levels.Airplane,
			Levels.FlyingCowboy,
			Levels.OldMan,
			Levels.RumRunners,
			Levels.SnowCult
		};
		worldDLCBossLevelsWithSaltbaker = new Levels[6]
		{
			Levels.Airplane,
			Levels.FlyingCowboy,
			Levels.OldMan,
			Levels.RumRunners,
			Levels.SnowCult,
			Levels.Saltbaker
		};
		platformingLevels = new Levels[6]
		{
			Levels.Platforming_Level_1_1,
			Levels.Platforming_Level_1_2,
			Levels.Platforming_Level_2_1,
			Levels.Platforming_Level_2_2,
			Levels.Platforming_Level_3_1,
			Levels.Platforming_Level_3_2
		};
		kingOfGamesLevels = new Levels[5]
		{
			Levels.ChessPawn,
			Levels.ChessKnight,
			Levels.ChessBishop,
			Levels.ChessRook,
			Levels.ChessQueen
		};
		kingOfGamesLevelsWithCastle = new Levels[6]
		{
			Levels.ChessPawn,
			Levels.ChessKnight,
			Levels.ChessBishop,
			Levels.ChessRook,
			Levels.ChessQueen,
			Levels.ChessCastle
		};
		chaliceLevels = new Levels[25]
		{
			Levels.Veggies,
			Levels.Slime,
			Levels.FlyingBlimp,
			Levels.Flower,
			Levels.Frogs,
			Levels.Baroness,
			Levels.Clown,
			Levels.FlyingGenie,
			Levels.Dragon,
			Levels.FlyingBird,
			Levels.Bee,
			Levels.Pirate,
			Levels.SallyStagePlay,
			Levels.Mouse,
			Levels.Robot,
			Levels.FlyingMermaid,
			Levels.Train,
			Levels.DicePalaceMain,
			Levels.Devil,
			Levels.Airplane,
			Levels.FlyingCowboy,
			Levels.OldMan,
			Levels.RumRunners,
			Levels.SnowCult,
			Levels.Saltbaker
		};
		CurrentMode = Mode.Normal;
	}

	public static void SetCurrentMode(Mode mode)
	{
		CurrentMode = mode;
	}

	public static void ResetPreviousLevelInfo()
	{
		Won = false;
		SuperUnlocked = false;
		PlayerManager.playerWasChalice[0] = false;
		PlayerManager.playerWasChalice[1] = false;
	}

	public static Levels GetEnumByName(string levelName)
	{
		return (Levels)Enum.Parse(typeof(Levels), levelName);
	}

	public static string GetLevelName(Levels level)
	{
		return Localization.Translate(level.ToString()).text;
	}

	public static PlayersStatsBossesHub GetPlayerStats(PlayerId playerId)
	{
		if (IsTowerOfPower)
		{
			return TowerOfPowerLevelGameInfo.PLAYER_STATS[(int)playerId];
		}
		if (IsDicePalace || IsDicePalaceMain)
		{
			return (playerId != PlayerId.PlayerOne) ? DicePalaceMainLevelGameInfo.PLAYER_TWO_STATS : DicePalaceMainLevelGameInfo.PLAYER_ONE_STATS;
		}
		return null;
	}

	protected virtual void OnEnable()
	{
		EventManager.Instance.AddListener<PlayerStatsManager.DeathEvent>(OnPlayerDeath);
		EventManager.Instance.AddListener<PlayerStatsManager.ReviveEvent>(OnPlayerRevive);
	}

	protected virtual void OnDisable()
	{
		EventManager.Instance.RemoveListener<PlayerStatsManager.DeathEvent>(OnPlayerDeath);
		EventManager.Instance.RemoveListener<PlayerStatsManager.ReviveEvent>(OnPlayerRevive);
	}

	protected override void Awake()
	{
		base.Awake();
		CheckIfInABossesHub();
		Cuphead.Init();
		PlayerManager.OnPlayerJoinedEvent += OnPlayerJoined;
		PlayerManager.OnPlayerLeaveEvent += OnPlayerLeave;
		DamageDealer.didDamageWithNonSmallPlaneWeapon = false;
		switch (CurrentLevel)
		{
		case Levels.Platforming_Level_1_1:
		case Levels.Platforming_Level_1_2:
		case Levels.Platforming_Level_3_1:
		case Levels.Platforming_Level_3_2:
		case Levels.Platforming_Level_2_2:
		case Levels.Platforming_Level_2_1:
			mode = Mode.Normal;
			break;
		default:
			mode = CurrentMode;
			break;
		}
		Current = this;
		PlayerData.PlayerLevelDataObject levelData = PlayerData.Data.GetLevelData(CurrentLevel);
		Won = false;
		BGMPlaylistCurrent = levelData.bgmPlayListCurrent;
		PreviousLevel = CurrentLevel;
		PreviousLevelType = type;
		PreviouslyWon = levelData.completed;
		PreviousGrade = levelData.grade;
		PreviousDifficulty = levelData.difficultyBeaten;
		SuperUnlocked = false;
		IsChessBoss = false;
		IsGraveyard = false;
		Ending = false;
		PartialInit();
		Application.targetFrameRate = 60;
		CreateUI();
		CreateHUD();
		LevelCoin.OnLevelStart();
		SceneLoader.SetCurrentLevel(CurrentLevel);
	}

	public virtual bool AllowDjimmi()
	{
		return !isMausoleum && type != Type.Tutorial && !(SceneLoader.CurrentContext is GauntletContext);
	}

	protected virtual void CheckIfInABossesHub()
	{
		IsDicePalace = false;
		IsDicePalaceMain = false;
	}

	public static void ResetBossesHub()
	{
		IsDicePalace = false;
		IsDicePalaceMain = false;
		if (IsTowerOfPower)
		{
			TowerOfPowerLevelGameInfo.GameInfo.CleanUp();
			IsTowerOfPower = false;
			IsTowerOfPowerMain = false;
		}
	}

	protected virtual void PartialInit()
	{
		if (ScoringData == null || ((type == Type.Battle || type == Type.Platforming) && (!IsDicePalace || (IsDicePalaceMain && DicePalaceMainLevelGameInfo.TURN_COUNTER == 0)) && (!IsTowerOfPowerMain || TowerOfPowerLevelGameInfo.TURN_COUNTER == 0)))
		{
			ScoringData = new LevelScoringData();
			ScoringData.goalTime = ((mode == Mode.Easy) ? goalTimes.easy : ((mode != Mode.Normal) ? goalTimes.hard : goalTimes.normal));
		}
		if ((IsDicePalace && !IsDicePalaceMain) || (IsTowerOfPower && !IsTowerOfPowerMain))
		{
			ScoringData.goalTime += ((mode == Mode.Easy) ? goalTimes.easy : ((mode != Mode.Normal) ? goalTimes.hard : goalTimes.normal));
		}
		ScoringData.difficulty = mode;
	}

	protected virtual void Start()
	{
		CupheadTime.SetAll(1f);
		switch (type)
		{
		default:
			StartCoroutine(startBattle_cr());
			break;
		case Type.Tutorial:
			StartCoroutine(startNonBattle_cr());
			break;
		case Type.Platforming:
			StartCoroutine(startPlatforming_cr());
			break;
		}
		CreateAudio();
		CreateColliders();
		CreatePlayers();
		CreateCamera();
		gui.LevelInit();
		hud.LevelInit();
		SetRichPresence();
		Initialized = true;
		if (playerMode != PlayerMode.Plane && CurrentLevel != Levels.Devil && CurrentLevel != Levels.Saltbaker && type != Type.Platforming && type != Type.Tutorial)
		{
			StartCoroutine(check_intros_cr());
		}
		if (CurrentLevel == Levels.Devil || CurrentLevel == Levels.Saltbaker)
		{
			CheckIntros();
		}
	}

	protected virtual void Update()
	{
		if (!Started)
		{
			CheckPlayerHoldingButtons();
		}
		LevelTime += CupheadTime.Delta;
		if (!playerIsDead)
		{
			return;
		}
		playerDeathDelayFrames++;
		if (playerDeathDelayFrames >= 5)
		{
			return;
		}
		if (PlayerManager.Multiplayer)
		{
			if (players[0].IsDead && players[1].IsDead)
			{
				_OnLose();
			}
		}
		else
		{
			_OnLose();
		}
		playerIsDead = false;
		playerDeathDelayFrames = 0;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		PlayerManager.ClearPlayers();
		Current = null;
		PlayerManager.OnPlayerJoinedEvent -= OnPlayerJoined;
		PlayerManager.OnPlayerLeaveEvent -= OnPlayerLeave;
		LevelResources = null;
		players = null;
	}

	public void SetBounds(int? left, int? right, int? top, int? bottom)
	{
		if (left.HasValue)
		{
			bounds.left = left.Value;
		}
		if (right.HasValue)
		{
			bounds.right = right.Value;
		}
		if (top.HasValue)
		{
			bounds.top = top.Value;
		}
		if (bottom.HasValue)
		{
			bounds.bottom = bottom.Value;
		}
		bounds.SetColliderPositions();
	}

	protected void CleanUpScore()
	{
		ScoringData = null;
	}

	public void RegisterMinionKilled()
	{
		if (!defeatedMinion)
		{
		}
		defeatedMinion = true;
	}

	private void CreateAudio()
	{
		LevelAudio.Create();
	}

	protected virtual void CreatePlayers()
	{
		PlayersCreated = true;
		AbstractPlayerController[] array = UnityEngine.Object.FindObjectsOfType<AbstractPlayerController>();
		foreach (AbstractPlayerController abstractPlayerController in array)
		{
			UnityEngine.Object.Destroy(abstractPlayerController.gameObject);
		}
		players = new AbstractPlayerController[2];
		BlockChaliceCharm = new bool[2];
		BlockChaliceCharm[0] = blockChalice;
		BlockChaliceCharm[1] = blockChalice;
		if (playerMode == PlayerMode.Custom)
		{
			return;
		}
		if (PlayerManager.Multiplayer && allowMultiplayer)
		{
			if (PlayerData.Data.Loadouts.GetPlayerLoadout(PlayerId.PlayerOne).charm == Charm.charm_chalice && PlayerData.Data.Loadouts.GetPlayerLoadout(PlayerId.PlayerTwo).charm == Charm.charm_chalice)
			{
				BlockChaliceCharm[(!Rand.Bool()) ? 1u : 0u] = true;
			}
			if (isMausoleum)
			{
				BlockChaliceCharm[0] = true;
				BlockChaliceCharm[1] = true;
			}
			Vector3 vector = ((playerMode != PlayerMode.Plane) ? ((Vector3)spawns.playerOne) : player1PlaneSpawnPos);
			players[0] = AbstractPlayerController.Create(PlayerId.PlayerOne, vector, playerMode);
			Vector3 vector2 = ((playerMode != PlayerMode.Plane) ? ((Vector3)spawns.playerTwo) : player2PlaneSpawnPos);
			players[1] = AbstractPlayerController.Create(PlayerId.PlayerTwo, vector2, playerMode);
		}
		else
		{
			Vector3 vector3 = ((playerMode != PlayerMode.Plane) ? ((Vector3)spawns.playerOneSingle) : player1PlaneSpawnPos);
			players[0] = AbstractPlayerController.Create(PlayerId.PlayerOne, vector3, playerMode);
		}
	}

	private void CheckPlayerCharacters()
	{
		ScoringData.player1IsChalice = players[0].stats.isChalice;
		if (PlayerManager.Multiplayer && allowMultiplayer)
		{
			ScoringData.player2IsChalice = players[1].stats.isChalice;
		}
	}

	private void CheckIntros()
	{
		LevelPlayerAnimationController component = players[0].GetComponent<LevelPlayerAnimationController>();
		if (component != null)
		{
			if (players[0].stats.Loadout.charm == Charm.charm_chalice)
			{
				if (players[1] != null && players[1].stats.isChalice && CurrentLevel != Levels.Devil && CurrentLevel != Levels.Saltbaker && (!IsDicePalace || DicePalaceMainLevelGameInfo.IS_FIRST_ENTRY))
				{
					component.CookieFail();
				}
				if (players[0].stats.isChalice && (CurrentLevel == Levels.Devil || CurrentLevel == Levels.Saltbaker))
				{
					component.ScaredChalice(CurrentLevel == Levels.Devil);
				}
			}
			else if (CurrentLevel != Levels.Devil && CurrentLevel != Levels.Saltbaker)
			{
				if (player1HeldJump && !player1HeldSuper)
				{
					component.IsIntroB();
				}
				else if (!player1HeldJump && !player1HeldSuper && Rand.Bool())
				{
					component.IsIntroB();
				}
			}
		}
		if (players.Length < 2 || !(players[1] != null))
		{
			return;
		}
		LevelPlayerAnimationController component2 = players[1].GetComponent<LevelPlayerAnimationController>();
		if (!(component2 != null))
		{
			return;
		}
		if (players[1].stats.Loadout.charm == Charm.charm_chalice)
		{
			if (players[0].stats.isChalice && CurrentLevel != Levels.Devil && CurrentLevel != Levels.Saltbaker && (!IsDicePalace || DicePalaceMainLevelGameInfo.IS_FIRST_ENTRY))
			{
				component2.CookieFail();
			}
			if (players[1].stats.isChalice && (CurrentLevel == Levels.Devil || CurrentLevel == Levels.Saltbaker))
			{
				component2.ScaredChalice(CurrentLevel == Levels.Devil);
			}
		}
		else if (PlayerManager.Multiplayer && CurrentLevel != Levels.Devil && CurrentLevel != Levels.Saltbaker)
		{
			if (player2HeldJump && !player2HeldSuper)
			{
				component2.IsIntroB();
			}
			else if (!player2HeldJump && !player2HeldSuper && Rand.Bool())
			{
				component2.IsIntroB();
			}
		}
	}

	protected virtual void CreatePlayerTwoOnJoin()
	{
		if (PlayerManager.Multiplayer && allowMultiplayer)
		{
			if (players[0].stats.isChalice || blockChalice)
			{
				BlockChaliceCharm[1] = true;
			}
			players[1] = AbstractPlayerController.Create(PlayerId.PlayerTwo, players[0].center, playerMode);
			players[1].LevelJoin(players[0].center);
		}
	}

	private void CreateCamera()
	{
		if (players == null)
		{
			Debug.LogError("Level.CreateCamera() must be called AFTER Level.CreatePlayers()");
		}
		CupheadLevelCamera cupheadLevelCamera = UnityEngine.Object.FindObjectOfType<CupheadLevelCamera>();
		cupheadLevelCamera.Init(camera);
	}

	private void CreateUI()
	{
		gui = UnityEngine.Object.FindObjectOfType<LevelGUI>();
		if (gui == null)
		{
			gui = LevelResources.levelGUI.InstantiatePrefab<LevelGUI>();
		}
	}

	private void CreateHUD()
	{
		hud = UnityEngine.Object.FindObjectOfType<LevelHUD>();
		if (hud == null)
		{
			hud = LevelResources.levelHUD.InstantiatePrefab<LevelHUD>();
		}
	}

	private void CreateColliders()
	{
		if (playerMode != PlayerMode.Plane)
		{
			collidersRoot = new GameObject("Colliders").transform;
			collidersRoot.parent = base.transform;
			collidersRoot.ResetLocalTransforms();
			SetupCollider(Bounds.Side.Left);
			SetupCollider(Bounds.Side.Right);
			SetupCollider(Bounds.Side.Top);
			SetupCollider(Bounds.Side.Bottom);
		}
	}

	private Transform SetupCollider(Bounds.Side side)
	{
		string text = string.Empty;
		string text2 = string.Empty;
		int layer = 0;
		int num = 0;
		Vector2 zero = Vector2.zero;
		switch (side)
		{
		case Bounds.Side.Left:
			text = "Level_Wall_Left";
			text2 = "Wall";
			layer = LayerMask.NameToLayer(Layers.Bounds_Walls.ToString());
			num = 90;
			break;
		case Bounds.Side.Right:
			text = "Level_Wall_Right";
			text2 = "Wall";
			layer = LayerMask.NameToLayer(Layers.Bounds_Walls.ToString());
			num = -90;
			break;
		case Bounds.Side.Top:
			text = "Level_Ceiling";
			text2 = "Ceiling";
			layer = LayerMask.NameToLayer(Layers.Bounds_Ceiling.ToString());
			break;
		case Bounds.Side.Bottom:
			text = "Level_Ground";
			text2 = "Ground";
			layer = LayerMask.NameToLayer(Layers.Bounds_Ground.ToString());
			num = 180;
			break;
		}
		GameObject gameObject = new GameObject(text);
		gameObject.tag = text2;
		gameObject.layer = layer;
		gameObject.transform.ResetLocalTransforms();
		gameObject.transform.SetPosition(zero.x, zero.y);
		gameObject.transform.SetEulerAngles(null, null, num);
		gameObject.transform.parent = collidersRoot;
		BoxCollider2D boxCollider2D = gameObject.AddComponent<BoxCollider2D>();
		boxCollider2D.isTrigger = true;
		boxCollider2D.size = new Vector2(10000f, 400f);
		Rigidbody2D rigidbody2D = gameObject.AddComponent<Rigidbody2D>();
		rigidbody2D.gravityScale = 0f;
		rigidbody2D.drag = 0f;
		rigidbody2D.angularDrag = 0f;
		rigidbody2D.isKinematic = true;
		bounds.colliders.Add(side, boxCollider2D);
		bounds.SetColliderPositions();
		gameObject.SetActive(bounds.GetEnabled(side));
		return gameObject.transform;
	}

	protected override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
		Gizmos.color = Color.white;
		Gizmos.DrawSphere(spawns.playerOne, 20f);
		Gizmos.DrawSphere(spawns.playerTwo, 20f);
		Gizmos.DrawSphere(spawns.playerOneSingle, 30f);
		Gizmos.color = Color.red;
		Gizmos.DrawCube(spawns.playerOneSingle, new Vector3(20f, 20f, 20f));
		Gizmos.DrawWireSphere(spawns.playerOne, 20f);
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(spawns.playerTwo, 20f);
		Gizmos.color = Color.white;
		if (camera.bounds.topEnabled)
		{
			Gizmos.DrawLine(new Vector3(camera.bounds.right, camera.bounds.top, 0f), new Vector3(-camera.bounds.left, camera.bounds.top, 0f));
		}
		if (camera.bounds.bottomEnabled)
		{
			Gizmos.DrawLine(new Vector3(camera.bounds.right, -camera.bounds.bottom, 0f), new Vector3(-camera.bounds.left, -camera.bounds.bottom, 0f));
		}
		if (camera.bounds.leftEnabled)
		{
			Gizmos.DrawLine(new Vector3(-camera.bounds.left, camera.bounds.top, 0f), new Vector3(-camera.bounds.left, -camera.bounds.bottom, 0f));
		}
		if (camera.bounds.rightEnabled)
		{
			Gizmos.DrawLine(new Vector3(camera.bounds.right, camera.bounds.top, 0f), new Vector3(camera.bounds.right, -camera.bounds.bottom, 0f));
		}
		if (bounds.topEnabled)
		{
			Gizmos.color = Color.blue;
		}
		else
		{
			Gizmos.color = Color.black;
		}
		Gizmos.DrawLine(new Vector3(bounds.right, bounds.top, 0f), new Vector3(-bounds.left, bounds.top, 0f));
		if (bounds.bottomEnabled)
		{
			Gizmos.color = Color.green;
		}
		else
		{
			Gizmos.color = Color.black;
		}
		Gizmos.DrawLine(new Vector3(bounds.right, -bounds.bottom, 0f), new Vector3(-bounds.left, -bounds.bottom, 0f));
		if (bounds.leftEnabled)
		{
			Gizmos.color = Color.red;
		}
		else
		{
			Gizmos.color = Color.black;
		}
		Gizmos.DrawLine(new Vector3(-bounds.left, bounds.top, 0f), new Vector3(-bounds.left, -bounds.bottom, 0f));
		if (bounds.rightEnabled)
		{
			Gizmos.color = Color.red;
		}
		else
		{
			Gizmos.color = Color.black;
		}
		Gizmos.DrawLine(new Vector3(bounds.right, bounds.top, 0f), new Vector3(bounds.right, -bounds.bottom, 0f));
	}

	protected virtual void OnPlayerJoined(PlayerId playerId)
	{
		LevelNewPlayerGUI.Current.Init();
		CreatePlayerTwoOnJoin();
		SetRichPresence();
	}

	private void OnPlayerLeave(PlayerId playerId)
	{
		if (playerId == PlayerId.PlayerTwo)
		{
			AbstractPlayerController player = PlayerManager.GetPlayer(playerId);
			if (player != null)
			{
				player.OnLeave(playerId);
			}
			if (PlayerManager.GetPlayer(PlayerId.PlayerOne).IsDead)
			{
				_OnLose();
			}
		}
	}

	private void SetRichPresence()
	{
		if (CurrentLevel == Levels.Mausoleum)
		{
			OnlineManager.Instance.Interface.SetRichPresence(PlayerId.Any, "Mausoleum", true);
			return;
		}
		if (CurrentLevel == Levels.Tutorial || CurrentLevel == Levels.House)
		{
			OnlineManager.Instance.Interface.SetRichPresence(PlayerId.Any, "Tutorial", true);
			return;
		}
		if (CurrentLevel == Levels.ShmupTutorial)
		{
			OnlineManager.Instance.Interface.SetRichPresence(PlayerId.Any, "Blueprint", true);
			return;
		}
		switch (type)
		{
		case Type.Battle:
			OnlineManager.Instance.Interface.SetStat(PlayerId.Any, "Boss", SceneLoader.SceneName);
			OnlineManager.Instance.Interface.SetRichPresence(PlayerId.Any, "Fighting", true);
			break;
		case Type.Platforming:
			OnlineManager.Instance.Interface.SetStat(PlayerId.Any, "PlatformingLevel", SceneLoader.SceneName);
			OnlineManager.Instance.Interface.SetRichPresence(PlayerId.Any, "Playing", true);
			break;
		}
	}

	private void OnPlayerDeath(PlayerStatsManager.DeathEvent e)
	{
		if (timeline != null && LevelType != Type.Platforming)
		{
			timeline.OnPlayerDeath(e.playerId);
		}
		playerIsDead = true;
	}

	private void OnPlayerRevive(PlayerStatsManager.ReviveEvent e)
	{
		timeline.OnPlayerRevive(e.playerId);
	}

	private void CheckPlayerHoldingButtons()
	{
		if (PlayerManager.GetPlayerInput(PlayerId.PlayerOne).GetButton(2) && !player1HeldJump)
		{
			player1HeldJump = true;
		}
		if (PlayerManager.GetPlayerInput(PlayerId.PlayerOne).GetButton(4) && !player1HeldSuper)
		{
			player1HeldSuper = true;
		}
		if (PlayerManager.Multiplayer)
		{
			if (PlayerManager.GetPlayerInput(PlayerId.PlayerTwo).GetButton(2) && !player2HeldJump)
			{
				player2HeldJump = true;
			}
			if (PlayerManager.GetPlayerInput(PlayerId.PlayerTwo).GetButton(4) && !player2HeldSuper)
			{
				player2HeldSuper = true;
			}
		}
	}

	private void _OnLevelStart()
	{
		OnLevelStart();
		if (OnLevelStartEvent__BackingField != null)
		{
			OnLevelStartEvent__BackingField();
		}
		PlayerManager.SetPlayerCanJoin(PlayerId.PlayerTwo, true, true);
		InterruptingPrompt.SetCanInterrupt(true);
		PlayerData.PlayerLevelDataObject levelData = PlayerData.Data.GetLevelData(CurrentLevel);
		if (levelData != null && !IsTowerOfPower)
		{
			levelData.played = true;
		}
	}

	private void _OnLevelEnd()
	{
		Ending = true;
		OnLevelEnd();
		if (OnLevelEndEvent__BackingField != null)
		{
			OnLevelEndEvent__BackingField();
		}
		PlayerManager.SetPlayerCanJoin(PlayerId.PlayerTwo, false, false);
		PlayerManager.ClearJoinPrompt();
	}

	protected void zHack_OnStateChanged()
	{
		OnStateChanged();
		if (OnStateChangedEvent__BackingField != null)
		{
			OnStateChangedEvent__BackingField();
		}
	}

	protected void zHack_OnWin()
	{
		PlayerManager.playerWasChalice[0] = PlayerManager.GetPlayer(PlayerId.PlayerOne).stats.isChalice;
		PlayerManager.playerWasChalice[1] = PlayerManager.GetPlayer(PlayerId.PlayerTwo) != null && PlayerManager.GetPlayer(PlayerId.PlayerTwo).stats.isChalice;
		CheckPlayerCharacters();
		Won = true;
		Difficulty = mode;
		PlayerData.PlayerLevelDataObject levelData = PlayerData.Data.GetLevelData(CurrentLevel);
		ScoringData.finalHP = PlayerManager.GetPlayer(PlayerId.PlayerOne).stats.Health;
		if (PlayerManager.GetPlayer(PlayerId.PlayerTwo) != null)
		{
			ScoringData.finalHP = Mathf.Max(ScoringData.finalHP, PlayerManager.GetPlayer(PlayerId.PlayerTwo).stats.Health);
		}
		ScoringData.finalHP = Mathf.Min(ScoringData.finalHP, (int)Cuphead.Current.ScoringProperties.hitsForNoScore);
		ScoringData.usedDjimmi = PlayerData.Data.DjimmiActivatedCurrentRegion() && AllowDjimmi() && mode != Mode.Hard;
		if (ScoringData.usedDjimmi && (!IsDicePalace || IsDicePalaceMain))
		{
			PlayerData.Data.DeactivateDjimmi();
		}
		if (!IsTowerOfPower)
		{
			levelData.completed = true;
			if (PlayerData.Data.Loadouts.GetPlayerLoadout(PlayerId.PlayerOne).charm == Charm.charm_chalice)
			{
				levelData.completedAsChaliceP1 = true;
			}
			if (PlayerData.Data.Loadouts.GetPlayerLoadout(PlayerId.PlayerTwo).charm == Charm.charm_chalice)
			{
				levelData.completedAsChaliceP2 = true;
			}
		}
		ScoringData.time += LevelTime;
		if ((type == Type.Battle || type == Type.Platforming) && (!IsDicePalace || IsDicePalaceMain))
		{
			Grade = ScoringData.CalculateGrade();
			float time = ScoringData.time;
			if (!IsTowerOfPower)
			{
				if (Difficulty > PreviousDifficulty || !PreviouslyWon)
				{
					levelData.difficultyBeaten = Difficulty;
				}
				if (Grade > PreviousGrade || !PreviouslyWon)
				{
					levelData.grade = Grade;
					levelData.bestTime = time;
				}
				else if (Grade == PreviousGrade && time < levelData.bestTime)
				{
					levelData.bestTime = time;
				}
				if (CurrentLevel == Levels.Devil)
				{
					PlayerData.Data.IsHardModeAvailable = true;
				}
				if (CurrentLevel == Levels.Saltbaker)
				{
					PlayerData.Data.IsHardModeAvailableDLC = true;
				}
			}
		}
		if (IsChessBoss)
		{
			if (PlayerData.Data.currentChessBossZone != MapCastleZones.Zone.None)
			{
				MapCastleZones.Zone currentChessBossZone = PlayerData.Data.currentChessBossZone;
				PlayerData.Data.currentChessBossZone = MapCastleZones.Zone.None;
				List<MapCastleZones.Zone> usedChessBossZones = PlayerData.Data.usedChessBossZones;
				if (!usedChessBossZones.Contains(currentChessBossZone))
				{
					usedChessBossZones.Add(currentChessBossZone);
				}
			}
			string[] value;
			if (ChessCastleLevel.Coins.TryGetValue(CurrentLevel, out value))
			{
				string[] array = value;
				foreach (string coinID in array)
				{
					if (!PlayerData.Data.coinManager.GetCoinCollected(coinID))
					{
						PlayerData.Data.coinManager.SetCoinValue(coinID, true, PlayerId.Any);
						PlayerData.Data.AddCurrency(PlayerId.PlayerOne, 1);
						PlayerData.Data.AddCurrency(PlayerId.PlayerTwo, 1);
					}
				}
			}
		}
		AbstractPlayerController player = PlayerManager.GetPlayer(PlayerId.PlayerOne);
		if (Difficulty != Mode.Easy && player != null && player.stats.Loadout.charm == Charm.charm_curse && CharmCurse.CalculateLevel(PlayerId.PlayerOne) >= 0)
		{
			levelData.curseCharmP1 = true;
		}
		AbstractPlayerController player2 = PlayerManager.GetPlayer(PlayerId.PlayerTwo);
		if (Difficulty != Mode.Easy && player2 != null && player2.stats.Loadout.charm == Charm.charm_curse && CharmCurse.CalculateLevel(PlayerId.PlayerTwo) >= 0)
		{
			levelData.curseCharmP2 = true;
		}
		_OnLevelEnd();
		_OnPreWin();
		if (LevelType == Type.Battle)
		{
			StartCoroutine(bossDeath_cr());
		}
		OnWin();
		if (OnWinEvent__BackingField != null)
		{
			OnWinEvent__BackingField();
		}
		if (!IsTowerOfPower)
		{
			PlayerData.SaveCurrentFile();
		}
		if (!IsTowerOfPower)
		{
			Levels[] levels = null;
			Levels[] levels2 = null;
			string text = null;
			Scenes currentMap = PlayerData.Data.CurrentMap;
			bool flag = Array.Exists(kingOfGamesLevels, (Levels level) => CurrentLevel == level);
			switch (currentMap)
			{
			case Scenes.scene_map_world_1:
				levels = (levels2 = world1BossLevels);
				text = "World1";
				break;
			case Scenes.scene_map_world_2:
				levels = (levels2 = world2BossLevels);
				text = "World2";
				break;
			case Scenes.scene_map_world_3:
				levels = (levels2 = world3BossLevels);
				text = "World3";
				break;
			case Scenes.scene_map_world_4:
				levels = (levels2 = world4BossLevels);
				text = "World4";
				break;
			case Scenes.scene_map_world_DLC:
				levels = worldDLCBossLevels;
				levels2 = worldDLCBossLevelsWithSaltbaker;
				text = "WorldDLC";
				break;
			}
			if (currentMap == Scenes.scene_map_world_4)
			{
				if (CurrentLevel == Levels.DicePalaceMain)
				{
					OnlineManager.Instance.Interface.UnlockAchievement(PlayerId.Any, "CompleteDicePalace");
				}
				else if (CurrentLevel == Levels.Devil)
				{
					OnlineManager.Instance.Interface.UnlockAchievement(PlayerId.Any, "CompleteDevil");
				}
			}
			else if (type == Type.Battle && PlayerData.Data.CheckLevelsCompleted(levels))
			{
				OnlineManager.Instance.Interface.UnlockAchievement(PlayerId.Any, "Complete" + text);
			}
			if (CurrentLevel == Levels.Saltbaker)
			{
				OnlineManager.Instance.Interface.UnlockAchievement(PlayerId.Any, OnlineAchievementData.DLC.DefeatSaltbaker);
			}
			if (type == Type.Battle && Difficulty == Mode.Hard && PlayerData.Data.CheckLevelsHaveMinDifficulty(world1BossLevels, Mode.Hard) && PlayerData.Data.CheckLevelsHaveMinDifficulty(world2BossLevels, Mode.Hard) && PlayerData.Data.CheckLevelsHaveMinDifficulty(world3BossLevels, Mode.Hard) && PlayerData.Data.CheckLevelsHaveMinDifficulty(world4BossLevels, Mode.Hard))
			{
				OnlineManager.Instance.Interface.UnlockAchievement(PlayerId.Any, "NewGamePlus");
			}
			if (type == Type.Battle && Grade >= LevelScoringData.Grade.AMinus && PlayerData.Data.CheckLevelsHaveMinGrade(levels2, LevelScoringData.Grade.AMinus))
			{
				OnlineManager.Instance.Interface.UnlockAchievement(PlayerId.Any, "ARank" + text);
			}
			if (type == Type.Platforming && !isMausoleum && ScoringData.pacifistRun && PlayerData.Data.CheckLevelsHaveMinGrade(platformingLevels, LevelScoringData.Grade.P))
			{
				OnlineManager.Instance.Interface.UnlockAchievement(PlayerId.Any, "PacifistRun");
			}
			if ((type == Type.Battle || type == Type.Platforming) && (!IsDicePalace || IsDicePalaceMain) && !isMausoleum && !flag && ScoringData.numTimesHit == 0)
			{
				if (IsDicePalaceMain)
				{
					OnlineManager.Instance.Interface.UnlockAchievement(PlayerId.Any, "NoHitsTakenDicePalace");
				}
				OnlineManager.Instance.Interface.UnlockAchievement(PlayerId.Any, "NoHitsTaken");
			}
			if (type == Type.Battle)
			{
				if (DamageDealer.lastPlayerDamageSource == DamageDealer.DamageSource.Super)
				{
					OnlineManager.Instance.Interface.UnlockAchievement(DamageDealer.lastPlayer, "SuperWin");
					AbstractPlayerController abstractPlayerController = ((DamageDealer.lastPlayer != PlayerId.PlayerOne) ? player2 : player);
					if (abstractPlayerController != null && abstractPlayerController.stats.isChalice)
					{
						OnlineManager.Instance.Interface.UnlockAchievement(DamageDealer.lastPlayer, OnlineAchievementData.DLC.ChaliceSuperWin);
					}
				}
				if (DamageDealer.lastPlayerDamageSource == DamageDealer.DamageSource.Ex)
				{
					OnlineManager.Instance.Interface.UnlockAchievement(DamageDealer.lastPlayer, "ExWin");
				}
				if (playerMode == PlayerMode.Plane && !DamageDealer.didDamageWithNonSmallPlaneWeapon)
				{
					OnlineManager.Instance.Interface.UnlockAchievement(PlayerId.Any, "SmallPlaneOnlyWin");
				}
				if (DamageDealer.lastDamageWasDLCWeapon)
				{
					OnlineManager.Instance.Interface.UnlockAchievement(DamageDealer.lastPlayer, OnlineAchievementData.DLC.DefeatBossDLCWeapon);
				}
				if (player != null && player.stats.Loadout.charm == Charm.charm_curse && CharmCurse.IsMaxLevel(PlayerId.PlayerOne))
				{
					OnlineManager.Instance.Interface.UnlockAchievement(PlayerId.PlayerOne, OnlineAchievementData.DLC.Paladin);
				}
				if (player2 != null && player2.stats.Loadout.charm == Charm.charm_curse && CharmCurse.IsMaxLevel(PlayerId.PlayerTwo))
				{
					OnlineManager.Instance.Interface.UnlockAchievement(PlayerId.PlayerTwo, OnlineAchievementData.DLC.Paladin);
				}
			}
			int num = 0;
			int num2 = 0;
			List<Levels> list = new List<Levels>(world1BossLevels);
			list.AddRange(world2BossLevels);
			list.AddRange(world3BossLevels);
			foreach (Levels item in list)
			{
				PlayerData.PlayerLevelDataObject levelData2 = PlayerData.Data.GetLevelData(item);
				if (levelData2.completed && levelData2.difficultyBeaten >= Mode.Normal)
				{
					num2++;
				}
			}
			List<Levels> list2 = new List<Levels>(world1BossLevels);
			list2.AddRange(world2BossLevels);
			list2.AddRange(world3BossLevels);
			list2.AddRange(world4BossLevels);
			list2.AddRange(platformingLevels);
			foreach (Levels item2 in list2)
			{
				PlayerData.PlayerLevelDataObject levelData3 = PlayerData.Data.GetLevelData(item2);
				if (levelData3.completed && levelData3.grade >= LevelScoringData.Grade.AMinus)
				{
					num++;
				}
			}
			if (type == Type.Battle && CurrentLevel != Levels.Mausoleum)
			{
				OnlineManager.Instance.Interface.UnlockAchievement(PlayerId.Any, "DefeatBoss");
			}
			if (type == Type.Battle && Array.Exists(chaliceLevels, (Levels level) => CurrentLevel == level))
			{
				bool flag2 = false;
				if (player != null && player.stats.isChalice)
				{
					flag2 = true;
					OnlineManager.Instance.Interface.UnlockAchievement(PlayerId.PlayerOne, OnlineAchievementData.DLC.DefeatBossAsChalice);
				}
				if (player2 != null && player2.stats.isChalice)
				{
					flag2 = true;
					OnlineManager.Instance.Interface.UnlockAchievement(PlayerId.PlayerTwo, OnlineAchievementData.DLC.DefeatBossAsChalice);
				}
				if (flag2)
				{
					if (PlayerData.Data.CountLevelsChaliceCompleted(chaliceLevels, PlayerId.PlayerOne) >= OnlineAchievementData.DLC.Triggers.DefeatXBossesAsChaliceTrigger)
					{
						OnlineManager.Instance.Interface.UnlockAchievement(PlayerId.PlayerOne, OnlineAchievementData.DLC.DefeatXBossesAsChalice);
					}
					if (PlayerData.Data.CountLevelsChaliceCompleted(chaliceLevels, PlayerId.PlayerTwo) >= OnlineAchievementData.DLC.Triggers.DefeatXBossesAsChaliceTrigger)
					{
						OnlineManager.Instance.Interface.UnlockAchievement(PlayerId.PlayerTwo, OnlineAchievementData.DLC.DefeatXBossesAsChalice);
					}
				}
			}
			if (type == Type.Battle && CurrentLevel == Levels.Graveyard)
			{
				OnlineManager.Instance.Interface.UnlockAchievement(PlayerId.Any, OnlineAchievementData.DLC.DefeatDevilPhase2);
			}
			if (Grade == LevelScoringData.Grade.S && CurrentLevel != Levels.Mausoleum && !flag)
			{
				OnlineManager.Instance.Interface.UnlockAchievement(PlayerId.Any, "SRank");
				if (Array.Exists(worldDLCBossLevelsWithSaltbaker, (Levels level) => CurrentLevel == level))
				{
					OnlineManager.Instance.Interface.UnlockAchievement(PlayerId.Any, OnlineAchievementData.DLC.SRankAnyDLC);
				}
			}
			if (Array.Exists(worldDLCBossLevels, (Levels level) => CurrentLevel == level) && !defeatedMinion)
			{
				OnlineManager.Instance.Interface.UnlockAchievement(PlayerId.Any, OnlineAchievementData.DLC.DefeatBossNoMinions);
			}
			if (flag && PlayerData.Data.CheckLevelsCompleted(kingOfGamesLevels))
			{
				OnlineManager.Instance.Interface.UnlockAchievement(PlayerId.Any, OnlineAchievementData.DLC.DefeatAllKOG);
			}
			OnlineManager.Instance.Interface.SetStat(PlayerId.Any, "ARanks", num);
			OnlineManager.Instance.Interface.SetStat(PlayerId.Any, "BossesDefeatedNormal", num2);
			OnlineManager.Instance.Interface.SyncAchievementsAndStats();
		}
		if (!isMausoleum)
		{
			InterruptingPrompt.SetCanInterrupt(false);
		}
	}

	private void _OnPreWin()
	{
		OnPreWin();
		if (OnPreWinEvent__BackingField != null)
		{
			OnPreWinEvent__BackingField();
		}
	}

	protected void _OnLose()
	{
		_OnLevelEnd();
		_OnPreLose();
		OnLose();
		if (OnLoseEvent__BackingField != null)
		{
			OnLoseEvent__BackingField();
		}
		PlayerManager.SetPlayerCanJoin(PlayerId.PlayerTwo, false, false);
		LevelEnd.Lose(isMausoleum, secretTriggered);
		if (!IsTowerOfPower)
		{
			PlayerData.SaveCurrentFile();
		}
	}

	private void _OnPreLose()
	{
		OnPreLose();
		if (OnPreLoseEvent__BackingField != null)
		{
			OnPreLoseEvent__BackingField();
		}
	}

	private void _OnTransitionInComplete()
	{
		OnTransitionInComplete();
		if (OnTransitionInCompleteEvent__BackingField != null)
		{
			OnTransitionInCompleteEvent__BackingField();
		}
	}

	private void OnStartExplosions()
	{
		if (OnBossDeathExplosionsEvent__BackingField != null)
		{
			OnBossDeathExplosionsEvent__BackingField();
		}
	}

	private void OnEndExplosions()
	{
		if (OnBossDeathExplosionsEndEvent__BackingField != null)
		{
			OnBossDeathExplosionsEndEvent__BackingField();
		}
	}

	private void OnFalloffExplosions()
	{
		if (OnBossDeathExplosionsFalloffEvent__BackingField != null)
		{
			OnBossDeathExplosionsFalloffEvent__BackingField();
		}
	}

	protected virtual void PlayAnnouncerReady()
	{
		if (!isMausoleum)
		{
			AudioManager.Play("level_announcer_ready");
		}
		else
		{
			AudioManager.Play("level_announcer_opening_line");
		}
	}

	protected virtual void PlayAnnouncerBegin()
	{
		AudioManager.Play("level_announcer_begin");
	}

	protected virtual LevelIntroAnimation CreateLevelIntro(Action callback)
	{
		return LevelIntroAnimation.Create(callback);
	}

	protected virtual void OnLevelStart()
	{
	}

	protected virtual void OnStateChanged()
	{
	}

	protected virtual void OnWin()
	{
	}

	protected virtual void OnPreWin()
	{
	}

	protected virtual void OnLose()
	{
	}

	protected virtual void OnPreLose()
	{
	}

	protected virtual void OnTransitionInComplete()
	{
	}

	protected virtual IEnumerator knockoutSFX_cr()
	{
		if (!isMausoleum)
		{
			AudioManager.Play("level_announcer_knockout_bell");
			AudioManager.Play("level_announcer_knockout");
			yield return CupheadTime.WaitForSeconds(this, 1.4f);
			if (!IsChessBoss && CurrentLevel != Levels.Saltbaker && CurrentLevel != Levels.Graveyard)
			{
				AudioManager.Play("level_boss_defeat_sting");
			}
		}
		else
		{
			AudioManager.Play("level_announcer_victory");
		}
	}

	protected virtual void OnBossDeath()
	{
	}

	private IEnumerator check_intros_cr()
	{
		yield return new WaitForSeconds(0.25f);
		CheckIntros();
		yield return null;
	}

	protected virtual IEnumerator startBattle_cr()
	{
		LevelIntroAnimation introAnim = CreateLevelIntro(intro.OnReadyAnimComplete);
		yield return new WaitForSeconds(0.4f + SceneLoader.EndTransitionDelay);
		if (!IsDicePalaceMain && !IsTowerOfPowerMain)
		{
			PlayAnnouncerReady();
			AudioManager.Play("level_bell_intro");
		}
		yield return new WaitForSeconds(0.25f);
		if (players[0] != null)
		{
			players[0].PlayIntro();
		}
		if (players[1] != null)
		{
			if (!players[1].stats.isChalice)
			{
				yield return CupheadTime.WaitForSeconds(this, 0.7f);
			}
			players[1].PlayIntro();
		}
		yield return new WaitForSeconds(0.25f);
		_OnTransitionInComplete();
		if (OnIntroEvent__BackingField != null)
		{
			OnIntroEvent__BackingField();
		}
		OnIntroEvent__BackingField = null;
		yield return new WaitForSeconds(LevelIntroTime);
		if (!IsDicePalaceMain && !IsTowerOfPowerMain)
		{
			introAnim.Play();
			while (!intro.readyComplete)
			{
				yield return null;
			}
			PlayAnnouncerBegin();
		}
		else if (!IsTowerOfPowerMain)
		{
			yield return CupheadTime.WaitForSeconds(this, 1.5f);
		}
		AbstractPlayerController[] array = players;
		foreach (AbstractPlayerController abstractPlayerController in array)
		{
			if (!(abstractPlayerController == null))
			{
				abstractPlayerController.LevelStart();
			}
		}
		Started = true;
		_OnLevelStart();
	}

	protected virtual IEnumerator startPlatforming_cr()
	{
		PlatformingLevelIntroAnimation introAnim = PlatformingLevelIntroAnimation.Create(intro.OnReadyAnimComplete);
		yield return new WaitForEndOfFrame();
		if (players[0] != null)
		{
			players[0].OnPlatformingLevelAwake();
		}
		if (players[1] != null)
		{
			players[1].OnPlatformingLevelAwake();
		}
		yield return new WaitForSeconds(0.4f + SceneLoader.EndTransitionDelay);
		_OnTransitionInComplete();
		if (OnIntroEvent__BackingField != null)
		{
			OnIntroEvent__BackingField();
		}
		OnIntroEvent__BackingField = null;
		introAnim.Play();
		AudioManager.Play("level_announcer_begin");
		while (!intro.readyComplete)
		{
			yield return null;
		}
		AbstractPlayerController[] array = players;
		foreach (AbstractPlayerController abstractPlayerController in array)
		{
			if (!(abstractPlayerController == null))
			{
				abstractPlayerController.LevelStart();
			}
		}
		Started = true;
		_OnLevelStart();
	}

	protected virtual IEnumerator startNonBattle_cr()
	{
		yield return new WaitForSeconds(0.4f + SceneLoader.EndTransitionDelay - 0.25f);
		if (playerMode == PlayerMode.Plane)
		{
			yield return new WaitForSeconds(0.5f);
			if (players[0] != null)
			{
				players[0].PlayIntro();
			}
			if (players[1] != null)
			{
				yield return CupheadTime.WaitForSeconds(this, 0.7f);
				players[1].PlayIntro();
			}
			yield return new WaitForSeconds(0.25f);
		}
		_OnTransitionInComplete();
		if (OnIntroEvent__BackingField != null)
		{
			OnIntroEvent__BackingField();
		}
		OnIntroEvent__BackingField = null;
		if (playerMode == PlayerMode.Plane)
		{
			yield return new WaitForSeconds(1.25f);
		}
		AbstractPlayerController[] array = players;
		foreach (AbstractPlayerController abstractPlayerController in array)
		{
			if (!(abstractPlayerController == null))
			{
				abstractPlayerController.LevelStart();
			}
		}
		Started = true;
		_OnLevelStart();
	}

	protected virtual IEnumerator bossDeath_cr()
	{
		LevelEnd.Win(knockoutSFX_cr(), OnBossDeath, OnStartExplosions, OnFalloffExplosions, OnEndExplosions, players, BossDeathTime, (type == Type.Battle || type == Type.Platforming) && (!IsDicePalace || IsDicePalaceMain) && !IsTowerOfPower && !isMausoleum && !IsGraveyard && !IsChessBoss, isMausoleum, isDevil, isTowerOfPower);
		yield return null;
	}
}
