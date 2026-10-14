using System;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public abstract class AbstractPlayerSuper : AbstractCollidableObject
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action OnEndedEvent__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action OnStartedEvent__BackingField;

	[SerializeField]
	[Header("Player Sprites")]
	private SpriteRenderer cuphead;

	[SerializeField]
	private SpriteRenderer mugman;

	[SerializeField]
	private bool isChaliceSuper;

	protected SpriteRenderer spriteRenderer;

	protected LevelPlayerController player;

	protected DamageDealer damageDealer;

	protected AnimationHelper animHelper;

	protected bool interrupted;

	protected override bool allowCollisionPlayer
	{
		get
		{
			return false;
		}
	}

	public event Action OnEndedEvent
	{
		add
		{
			Action action = OnEndedEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnEndedEvent__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnEndedEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnEndedEvent__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public event Action OnStartedEvent
	{
		add
		{
			Action action = OnStartedEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnStartedEvent__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnStartedEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnStartedEvent__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	protected override void Awake()
	{
		base.tag = "PlayerProjectile";
		base.Awake();
	}

	protected virtual void Start()
	{
		animHelper = GetComponent<AnimationHelper>();
		base.transform.position = player.transform.position;
	}

	protected virtual void Update()
	{
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
	}

	protected override void OnCollisionEnemy(GameObject hit, CollisionPhase phase)
	{
		if (damageDealer != null)
		{
			damageDealer.DealDamage(hit);
		}
		base.OnCollisionEnemy(hit, phase);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (player != null)
		{
			player.weaponManager.OnSuperInterrupt -= Interrupt;
		}
	}

	public AbstractPlayerSuper Create(LevelPlayerController player)
	{
		AbstractPlayerSuper abstractPlayerSuper = InstantiatePrefab<AbstractPlayerSuper>();
		abstractPlayerSuper.player = player;
		PlayerId id = player.id;
		if (id == PlayerId.PlayerOne || id != PlayerId.PlayerTwo)
		{
			if (!isChaliceSuper)
			{
				abstractPlayerSuper.spriteRenderer = ((!PlayerManager.player1IsMugman) ? cuphead : mugman);
				abstractPlayerSuper.cuphead.gameObject.SetActive(!PlayerManager.player1IsMugman);
				abstractPlayerSuper.mugman.gameObject.SetActive(PlayerManager.player1IsMugman);
			}
		}
		else if (!isChaliceSuper)
		{
			abstractPlayerSuper.spriteRenderer = ((!PlayerManager.player1IsMugman) ? mugman : cuphead);
			abstractPlayerSuper.cuphead.gameObject.SetActive(PlayerManager.player1IsMugman);
			abstractPlayerSuper.mugman.gameObject.SetActive(!PlayerManager.player1IsMugman);
		}
		interrupted = false;
		player.weaponManager.OnSuperInterrupt += abstractPlayerSuper.Interrupt;
		abstractPlayerSuper.StartSuper();
		return abstractPlayerSuper;
	}

	public virtual void Interrupt()
	{
		interrupted = true;
	}

	protected virtual void StartSuper()
	{
		AnimationHelper component = GetComponent<AnimationHelper>();
		component.IgnoreGlobal = true;
		PauseManager.Pause();
		AudioManager.HandleSnapshot(AudioManager.Snapshots.SuperStart.ToString(), 0.2f);
		AudioManager.ChangeBGMPitch(1.3f, 1.5f);
		base.transform.SetScale(player.transform.localScale.x, player.transform.localScale.y, 1f);
		base.transform.position = player.transform.position;
		if (OnStartedEvent__BackingField != null)
		{
			OnStartedEvent__BackingField();
		}
		OnStartedEvent__BackingField = null;
	}

	protected virtual void Fire()
	{
		PauseManager.Unpause();
		AudioManager.HandleSnapshot(AudioManager.Snapshots.Super.ToString(), 0.2f);
		if (player == null)
		{
			Interrupt();
		}
		else
		{
			player.PauseAll();
		}
		AnimationHelper component = GetComponent<AnimationHelper>();
		component.IgnoreGlobal = false;
	}

	protected virtual void EndSuper(bool changePitch = true)
	{
		AudioManager.SnapshotReset(SceneLoader.SceneName, 1f);
		if (changePitch)
		{
			AudioManager.ChangeBGMPitch(1f, 2f);
		}
		if (player != null)
		{
			player.UnpauseAll();
		}
		if (OnEndedEvent__BackingField != null)
		{
			OnEndedEvent__BackingField();
		}
		OnEndedEvent__BackingField = null;
	}
}
