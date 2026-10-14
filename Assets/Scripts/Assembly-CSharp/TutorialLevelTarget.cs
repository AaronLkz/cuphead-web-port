using System;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public class TutorialLevelTarget : AbstractCollidableObject
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action OnShotEvent__BackingField;

	public event Action OnShotEvent
	{
		add
		{
			Action action = OnShotEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnShotEvent__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnShotEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnShotEvent__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	protected override void OnCollisionPlayerProjectile(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayerProjectile(hit, phase);
		GetComponent<Collider2D>().enabled = false;
		if (OnShotEvent__BackingField != null)
		{
			OnShotEvent__BackingField();
		}
	}
}
