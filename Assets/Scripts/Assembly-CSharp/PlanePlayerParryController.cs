using System;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public class PlanePlayerParryController : AbstractPlanePlayerComponent, IParryAttack
{
	public enum ParryState
	{
		Init = 0,
		Ready = 1,
		Cooldown = 2,
		Parrying = 3,
		Disabled = 4
	}

	public const float COOLDOWN_DURATION = 0.3f;

	public const float CHALICE_PARRY_SIZE_MODIFIER = 1.3f;

	private ParryState state;

	[SerializeField]
	private PlanePlayerParryEffect effect;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action OnParryStartEvent__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action OnParrySuccessEvent__BackingField;

	private PlanePlayerParryEffect effectInstance;

	private float timeSinceParry;

	public ParryState State
	{
		get
		{
			return state;
		}
		set
		{
			state = value;
		}
	}

	public bool AttackParryUsed { get; set; }

	public bool HasHitEnemy { get; set; }

	public event Action OnParryStartEvent
	{
		add
		{
			Action action = OnParryStartEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnParryStartEvent__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnParryStartEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnParryStartEvent__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public event Action OnParrySuccessEvent
	{
		add
		{
			Action action = OnParrySuccessEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnParrySuccessEvent__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnParrySuccessEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnParrySuccessEvent__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	private void Start()
	{
		base.player.OnReviveEvent += OnRevive;
		base.player.stats.OnStoned += OnStoned;
	}

	private void FixedUpdate()
	{
		switch (state)
		{
		case ParryState.Ready:
			UpdateReady();
			break;
		case ParryState.Cooldown:
			UpdateCooldown();
			break;
		}
	}

	private void UpdateReady()
	{
		if (!base.player.Shrunk && !base.player.WeaponBusy && !(base.player.stats.StoneTime > 0f) && state == ParryState.Ready && (base.player.input.actions.GetButtonDown(2) || base.player.motor.HasBufferedInput(PlanePlayerMotor.BufferedInput.Jump)))
		{
			base.player.motor.ClearBufferedInput();
			StartParry();
		}
	}

	private void UpdateCooldown()
	{
		timeSinceParry += CupheadTime.FixedDelta;
		if (timeSinceParry > 0.3f)
		{
			state = ParryState.Ready;
			AttackParryUsed = false;
		}
	}

	public override void OnLevelStart()
	{
		base.OnLevelStart();
		state = ParryState.Ready;
	}

	private void StartParry()
	{
		if (state == ParryState.Ready)
		{
			state = ParryState.Parrying;
			if (OnParryStartEvent__BackingField != null)
			{
				OnParryStartEvent__BackingField();
			}
			effectInstance = effect.Create(base.player) as PlanePlayerParryEffect;
			if (base.player.stats.isChalice)
			{
				effectInstance.GetComponent<CircleCollider2D>().radius *= 1.3f;
			}
		}
	}

	public void OnParrySuccess()
	{
		if (OnParrySuccessEvent__BackingField != null)
		{
			OnParrySuccessEvent__BackingField();
		}
		state = ParryState.Cooldown;
		timeSinceParry = 0f;
		if (effectInstance != null)
		{
			UnityEngine.Object.Destroy(effectInstance.gameObject);
		}
	}

	private void OnParryEnd()
	{
		state = ParryState.Cooldown;
		timeSinceParry = 0f;
		HasHitEnemy = false;
		if (effectInstance != null)
		{
			UnityEngine.Object.Destroy(effectInstance.gameObject);
		}
	}

	private void OnRevive(Vector3 pos)
	{
		state = ParryState.Ready;
	}

	private void OnStoned()
	{
		state = ParryState.Ready;
	}

	private void SoundPlaneParry()
	{
		AudioManager.Play("player_plane_parry");
		emitAudioFromObject.Add("player_plane_parry");
	}
}
