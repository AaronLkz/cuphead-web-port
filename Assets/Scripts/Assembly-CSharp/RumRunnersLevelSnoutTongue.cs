using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public class RumRunnersLevelSnoutTongue : ParrySwitch
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private CollisionChild.OnCollisionHandler OnPlayerCollision__BackingField;

	private CollisionChild collisionChild;

	public event CollisionChild.OnCollisionHandler OnPlayerCollision
	{
		add
		{
			CollisionChild.OnCollisionHandler onCollisionHandler = OnPlayerCollision__BackingField;
			CollisionChild.OnCollisionHandler onCollisionHandler2;
			do
			{
				onCollisionHandler2 = onCollisionHandler;
				onCollisionHandler = Interlocked.CompareExchange(ref OnPlayerCollision__BackingField, (CollisionChild.OnCollisionHandler)Delegate.Combine(onCollisionHandler2, value), onCollisionHandler);
			}
			while ((object)onCollisionHandler != onCollisionHandler2);
		}
		remove
		{
			CollisionChild.OnCollisionHandler onCollisionHandler = OnPlayerCollision__BackingField;
			CollisionChild.OnCollisionHandler onCollisionHandler2;
			do
			{
				onCollisionHandler2 = onCollisionHandler;
				onCollisionHandler = Interlocked.CompareExchange(ref OnPlayerCollision__BackingField, (CollisionChild.OnCollisionHandler)Delegate.Remove(onCollisionHandler2, value), onCollisionHandler);
			}
			while ((object)onCollisionHandler != onCollisionHandler2);
		}
	}

	private void OnEnable()
	{
		GetComponent<CollisionChild>().OnPlayerCollision += onPlayerCollision;
	}

	private void OnDisable()
	{
		GetComponent<CollisionChild>().OnPlayerCollision -= onPlayerCollision;
	}

	protected override void Awake()
	{
		base.Awake();
		base.gameObject.tag = "Enemy";
		collisionChild = GetComponent<CollisionChild>();
	}

	public override void OnParryPrePause(AbstractPlayerController player)
	{
		if ((bool)parrySpark)
		{
			parrySpark.Create(player.transform.position);
		}
		FirePrePauseEvent();
		player.stats.ParryOneQuarter();
	}

	public override void OnParryPostPause(AbstractPlayerController player)
	{
		base.OnParryPostPause(player);
		base.IsParryable = false;
		collisionChild.enabled = false;
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
		base.IsParryable = true;
		collisionChild.enabled = true;
	}

	private void onPlayerCollision(GameObject hit, CollisionPhase phase)
	{
		if (base.IsParryable && OnPlayerCollision__BackingField != null)
		{
			OnPlayerCollision__BackingField(hit, phase);
		}
	}
}
