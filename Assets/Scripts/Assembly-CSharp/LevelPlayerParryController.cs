using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public class LevelPlayerParryController : AbstractLevelPlayerComponent, IParryAttack
{
	public enum ParryState
	{
		Init = 0,
		Ready = 1,
		Parrying = 2
	}

	public const float DURATION = 0.2f;

	private ParryState state;

	[SerializeField]
	private LevelPlayerParryEffect effect;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action OnParryStartEvent__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action OnParryEndEvent__BackingField;

	public ParryState State
	{
		get
		{
			return state;
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

	public event Action OnParryEndEvent
	{
		add
		{
			Action action = OnParryEndEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnParryEndEvent__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnParryEndEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnParryEndEvent__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	private void Start()
	{
		base.player.motor.OnParryEvent += StartParry;
		base.player.motor.OnGroundedEvent += OnGround;
	}

	private void OnGround()
	{
		AttackParryUsed = false;
	}

	public override void OnLevelStart()
	{
		base.OnLevelStart();
		state = ParryState.Ready;
	}

	private void StartParry()
	{
		state = ParryState.Parrying;
		if (OnParryStartEvent__BackingField != null)
		{
			OnParryStartEvent__BackingField();
		}
		StartCoroutine(parry_cr());
	}

	private IEnumerator parry_cr()
	{
		effect.Create(base.player);
		yield return CupheadTime.WaitForSeconds(this, 0.2f);
		state = ParryState.Ready;
		if (OnParryEndEvent__BackingField != null)
		{
			OnParryEndEvent__BackingField();
		}
		HasHitEnemy = false;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		effect = null;
	}
}
