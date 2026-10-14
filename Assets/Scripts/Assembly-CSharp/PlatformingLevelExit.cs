using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public class PlatformingLevelExit : AbstractCollidableObject
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action OnWinStartEvent__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action OnWinCompleteEvent__BackingField;

	[SerializeField]
	[Range(200f, 1500f)]
	private float _exitDistance = 500f;

	[SerializeField]
	private float onCompleteWaitTime = 2f;

	private bool _activated;

	private bool _exited;

	public static event Action OnWinStartEvent
	{
		add
		{
			Action action = OnWinStartEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnWinStartEvent__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnWinStartEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnWinStartEvent__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action OnWinCompleteEvent
	{
		add
		{
			Action action = OnWinCompleteEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnWinCompleteEvent__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnWinCompleteEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnWinCompleteEvent__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	private void FixedUpdate()
	{
		if (_activated)
		{
			if (_exited)
			{
				return;
			}
			for (int i = 0; i < 2; i++)
			{
				AbstractPlayerController player = PlayerManager.GetPlayer((i != 0) ? PlayerId.PlayerTwo : PlayerId.PlayerOne);
				if (!(player == null) && player.center.x > base.transform.position.x + _exitDistance)
				{
					_exited = true;
					if (OnWinCompleteEvent__BackingField != null)
					{
						OnWinCompleteEvent__BackingField();
						OnWinCompleteEvent__BackingField = null;
					}
					break;
				}
			}
			return;
		}
		for (int j = 0; j < 2; j++)
		{
			AbstractPlayerController player2 = PlayerManager.GetPlayer((j != 0) ? PlayerId.PlayerTwo : PlayerId.PlayerOne);
			if (!(player2 == null) && !player2.IsDead && player2.center.x > base.transform.position.x)
			{
				_activated = true;
				if (OnWinStartEvent__BackingField != null)
				{
					OnWinStartEvent__BackingField();
					OnWinStartEvent__BackingField = null;
				}
				PlatformingLevelEnd.Win();
				StartCoroutine(on_win_complete_cr());
				break;
			}
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		OnWinStartEvent__BackingField = null;
		OnWinCompleteEvent__BackingField = null;
	}

	private IEnumerator on_win_complete_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, onCompleteWaitTime);
		if (OnWinCompleteEvent__BackingField != null)
		{
			OnWinCompleteEvent__BackingField();
			OnWinCompleteEvent__BackingField = null;
		}
	}

	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		DrawGizmos(0.5f);
	}

	protected override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
		DrawGizmos(1f);
	}

	private void DrawGizmos(float a)
	{
		Gizmos.color = new Color(0f, 1f, 0f, a);
		Vector3 vector = base.baseTransform.position + new Vector3(_exitDistance, 0f, 0f);
		Gizmos.DrawLine(base.baseTransform.position, vector);
		Gizmos.DrawLine(vector + new Vector3(0f, -5000f, 0f), vector + new Vector3(0f, 5000f, 0f));
		Gizmos.color = Color.white;
	}
}
