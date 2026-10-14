using System;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public class CircusPlatformingLevelMagicianBullet : BasicProjectile
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action OnProjectileDeath__BackingField;

	[SerializeField]
	private SpriteRenderer puffs;

	protected override float DestroyLifetime
	{
		get
		{
			return 0f;
		}
	}

	public event Action OnProjectileDeath
	{
		add
		{
			Action action = OnProjectileDeath__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnProjectileDeath__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnProjectileDeath__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnProjectileDeath__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	protected override void Start()
	{
		base.Start();
		AudioManager.PlayLoop("circus_magician_magic_loop");
		emitAudioFromObject.Add("circus_magician_magic_loop");
		puffs.flipX = Rand.Bool();
		puffs.flipY = Rand.Bool();
		DestroyDistance = 0f;
	}

	protected override void OnDestroy()
	{
		AudioManager.Stop("circus_magician_magic_loop");
		if (OnProjectileDeath__BackingField != null)
		{
			OnProjectileDeath__BackingField();
		}
		base.OnDestroy();
	}
}
