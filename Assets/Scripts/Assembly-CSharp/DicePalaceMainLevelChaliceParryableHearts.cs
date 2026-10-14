using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public class DicePalaceMainLevelChaliceParryableHearts : AbstractProjectile
{
	[SerializeField]
	private Effect parrySpark;

	[SerializeField]
	protected float coolDown = 0.4f;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action OnPrePauseActivate__BackingField;

	public bool IsParryable { get; protected set; }

	public override float ParryMeterMultiplier
	{
		get
		{
			return 0f;
		}
	}

	public event Action OnPrePauseActivate
	{
		add
		{
			Action action = OnPrePauseActivate__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnPrePauseActivate__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnPrePauseActivate__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnPrePauseActivate__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	protected override void Awake()
	{
		base.Awake();
		SetParryable(true);
		if (!(GetComponent<Collider2D>() == null))
		{
		}
	}

	public virtual void OnParryPrePause(AbstractPlayerController player)
	{
		if ((bool)parrySpark)
		{
			parrySpark.Create(base.transform.position);
		}
	}

	public override void OnParry(AbstractPlayerController player)
	{
		SetParryable(false);
		StartCoroutine(parryCooldown_cr());
	}

	private IEnumerator parryCooldown_cr()
	{
		float t = 0f;
		while (t < coolDown)
		{
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		Collider2D collider = GetComponent<Collider2D>();
		collider.enabled = true;
		SetParryable(true);
		yield return null;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		parrySpark = null;
	}
}
