using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public class ParrySwitch : AbstractSwitch
{
	[SerializeField]
	protected Effect parrySpark;

	[SerializeField]
	protected float coolDown = 0.4f;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action OnPrePauseActivate__BackingField;

	public bool IsParryable { get; protected set; }

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

	protected void FirePrePauseEvent()
	{
		if (OnPrePauseActivate__BackingField != null)
		{
			OnPrePauseActivate__BackingField();
		}
	}

	protected override void Awake()
	{
		base.Awake();
		base.tag = "ParrySwitch";
		IsParryable = true;
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
		FirePrePauseEvent();
	}

	public virtual void OnParryPostPause(AbstractPlayerController player)
	{
		DispatchEvent();
	}

	public void ActivateFromOtherSource()
	{
		if ((bool)parrySpark)
		{
			parrySpark.Create(base.transform.position);
		}
		DispatchEvent();
	}

	public void StartParryCooldown()
	{
		GetComponent<Collider2D>().enabled = false;
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
		yield return null;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		parrySpark = null;
	}
}
