using System;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public class CollisionChild : AbstractCollidableObject
{
	public delegate void OnCollisionHandler(GameObject hit, CollisionPhase phase);

	[SerializeField]
	[Tooltip("OPTIONAL: Drag collision parent to this slot to register all collision events to this child. If null, no collisions are registered.")]
	private AbstractCollidableObject collisionParent;

	[SerializeField]
	private bool forwardParry;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private OnCollisionHandler OnAnyCollision__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private OnCollisionHandler OnWallCollision__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private OnCollisionHandler OnGroundCollision__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private OnCollisionHandler OnCeilingCollision__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private OnCollisionHandler OnPlayerCollision__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private OnCollisionHandler OnPlayerProjectileCollision__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private OnCollisionHandler OnEnemyCollision__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private OnCollisionHandler OnEnemyProjectileCollision__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private OnCollisionHandler OnOtherCollision__BackingField;

	public event OnCollisionHandler OnAnyCollision
	{
		add
		{
			OnCollisionHandler onCollisionHandler = OnAnyCollision__BackingField;
			OnCollisionHandler onCollisionHandler2;
			do
			{
				onCollisionHandler2 = onCollisionHandler;
				onCollisionHandler = Interlocked.CompareExchange(ref OnAnyCollision__BackingField, (OnCollisionHandler)Delegate.Combine(onCollisionHandler2, value), onCollisionHandler);
			}
			while ((object)onCollisionHandler != onCollisionHandler2);
		}
		remove
		{
			OnCollisionHandler onCollisionHandler = OnAnyCollision__BackingField;
			OnCollisionHandler onCollisionHandler2;
			do
			{
				onCollisionHandler2 = onCollisionHandler;
				onCollisionHandler = Interlocked.CompareExchange(ref OnAnyCollision__BackingField, (OnCollisionHandler)Delegate.Remove(onCollisionHandler2, value), onCollisionHandler);
			}
			while ((object)onCollisionHandler != onCollisionHandler2);
		}
	}

	public event OnCollisionHandler OnWallCollision
	{
		add
		{
			OnCollisionHandler onCollisionHandler = OnWallCollision__BackingField;
			OnCollisionHandler onCollisionHandler2;
			do
			{
				onCollisionHandler2 = onCollisionHandler;
				onCollisionHandler = Interlocked.CompareExchange(ref OnWallCollision__BackingField, (OnCollisionHandler)Delegate.Combine(onCollisionHandler2, value), onCollisionHandler);
			}
			while ((object)onCollisionHandler != onCollisionHandler2);
		}
		remove
		{
			OnCollisionHandler onCollisionHandler = OnWallCollision__BackingField;
			OnCollisionHandler onCollisionHandler2;
			do
			{
				onCollisionHandler2 = onCollisionHandler;
				onCollisionHandler = Interlocked.CompareExchange(ref OnWallCollision__BackingField, (OnCollisionHandler)Delegate.Remove(onCollisionHandler2, value), onCollisionHandler);
			}
			while ((object)onCollisionHandler != onCollisionHandler2);
		}
	}

	public event OnCollisionHandler OnGroundCollision
	{
		add
		{
			OnCollisionHandler onCollisionHandler = OnGroundCollision__BackingField;
			OnCollisionHandler onCollisionHandler2;
			do
			{
				onCollisionHandler2 = onCollisionHandler;
				onCollisionHandler = Interlocked.CompareExchange(ref OnGroundCollision__BackingField, (OnCollisionHandler)Delegate.Combine(onCollisionHandler2, value), onCollisionHandler);
			}
			while ((object)onCollisionHandler != onCollisionHandler2);
		}
		remove
		{
			OnCollisionHandler onCollisionHandler = OnGroundCollision__BackingField;
			OnCollisionHandler onCollisionHandler2;
			do
			{
				onCollisionHandler2 = onCollisionHandler;
				onCollisionHandler = Interlocked.CompareExchange(ref OnGroundCollision__BackingField, (OnCollisionHandler)Delegate.Remove(onCollisionHandler2, value), onCollisionHandler);
			}
			while ((object)onCollisionHandler != onCollisionHandler2);
		}
	}

	public event OnCollisionHandler OnCeilingCollision
	{
		add
		{
			OnCollisionHandler onCollisionHandler = OnCeilingCollision__BackingField;
			OnCollisionHandler onCollisionHandler2;
			do
			{
				onCollisionHandler2 = onCollisionHandler;
				onCollisionHandler = Interlocked.CompareExchange(ref OnCeilingCollision__BackingField, (OnCollisionHandler)Delegate.Combine(onCollisionHandler2, value), onCollisionHandler);
			}
			while ((object)onCollisionHandler != onCollisionHandler2);
		}
		remove
		{
			OnCollisionHandler onCollisionHandler = OnCeilingCollision__BackingField;
			OnCollisionHandler onCollisionHandler2;
			do
			{
				onCollisionHandler2 = onCollisionHandler;
				onCollisionHandler = Interlocked.CompareExchange(ref OnCeilingCollision__BackingField, (OnCollisionHandler)Delegate.Remove(onCollisionHandler2, value), onCollisionHandler);
			}
			while ((object)onCollisionHandler != onCollisionHandler2);
		}
	}

	public event OnCollisionHandler OnPlayerCollision
	{
		add
		{
			OnCollisionHandler onCollisionHandler = OnPlayerCollision__BackingField;
			OnCollisionHandler onCollisionHandler2;
			do
			{
				onCollisionHandler2 = onCollisionHandler;
				onCollisionHandler = Interlocked.CompareExchange(ref OnPlayerCollision__BackingField, (OnCollisionHandler)Delegate.Combine(onCollisionHandler2, value), onCollisionHandler);
			}
			while ((object)onCollisionHandler != onCollisionHandler2);
		}
		remove
		{
			OnCollisionHandler onCollisionHandler = OnPlayerCollision__BackingField;
			OnCollisionHandler onCollisionHandler2;
			do
			{
				onCollisionHandler2 = onCollisionHandler;
				onCollisionHandler = Interlocked.CompareExchange(ref OnPlayerCollision__BackingField, (OnCollisionHandler)Delegate.Remove(onCollisionHandler2, value), onCollisionHandler);
			}
			while ((object)onCollisionHandler != onCollisionHandler2);
		}
	}

	public event OnCollisionHandler OnPlayerProjectileCollision
	{
		add
		{
			OnCollisionHandler onCollisionHandler = OnPlayerProjectileCollision__BackingField;
			OnCollisionHandler onCollisionHandler2;
			do
			{
				onCollisionHandler2 = onCollisionHandler;
				onCollisionHandler = Interlocked.CompareExchange(ref OnPlayerProjectileCollision__BackingField, (OnCollisionHandler)Delegate.Combine(onCollisionHandler2, value), onCollisionHandler);
			}
			while ((object)onCollisionHandler != onCollisionHandler2);
		}
		remove
		{
			OnCollisionHandler onCollisionHandler = OnPlayerProjectileCollision__BackingField;
			OnCollisionHandler onCollisionHandler2;
			do
			{
				onCollisionHandler2 = onCollisionHandler;
				onCollisionHandler = Interlocked.CompareExchange(ref OnPlayerProjectileCollision__BackingField, (OnCollisionHandler)Delegate.Remove(onCollisionHandler2, value), onCollisionHandler);
			}
			while ((object)onCollisionHandler != onCollisionHandler2);
		}
	}

	public event OnCollisionHandler OnEnemyCollision
	{
		add
		{
			OnCollisionHandler onCollisionHandler = OnEnemyCollision__BackingField;
			OnCollisionHandler onCollisionHandler2;
			do
			{
				onCollisionHandler2 = onCollisionHandler;
				onCollisionHandler = Interlocked.CompareExchange(ref OnEnemyCollision__BackingField, (OnCollisionHandler)Delegate.Combine(onCollisionHandler2, value), onCollisionHandler);
			}
			while ((object)onCollisionHandler != onCollisionHandler2);
		}
		remove
		{
			OnCollisionHandler onCollisionHandler = OnEnemyCollision__BackingField;
			OnCollisionHandler onCollisionHandler2;
			do
			{
				onCollisionHandler2 = onCollisionHandler;
				onCollisionHandler = Interlocked.CompareExchange(ref OnEnemyCollision__BackingField, (OnCollisionHandler)Delegate.Remove(onCollisionHandler2, value), onCollisionHandler);
			}
			while ((object)onCollisionHandler != onCollisionHandler2);
		}
	}

	public event OnCollisionHandler OnEnemyProjectileCollision
	{
		add
		{
			OnCollisionHandler onCollisionHandler = OnEnemyProjectileCollision__BackingField;
			OnCollisionHandler onCollisionHandler2;
			do
			{
				onCollisionHandler2 = onCollisionHandler;
				onCollisionHandler = Interlocked.CompareExchange(ref OnEnemyProjectileCollision__BackingField, (OnCollisionHandler)Delegate.Combine(onCollisionHandler2, value), onCollisionHandler);
			}
			while ((object)onCollisionHandler != onCollisionHandler2);
		}
		remove
		{
			OnCollisionHandler onCollisionHandler = OnEnemyProjectileCollision__BackingField;
			OnCollisionHandler onCollisionHandler2;
			do
			{
				onCollisionHandler2 = onCollisionHandler;
				onCollisionHandler = Interlocked.CompareExchange(ref OnEnemyProjectileCollision__BackingField, (OnCollisionHandler)Delegate.Remove(onCollisionHandler2, value), onCollisionHandler);
			}
			while ((object)onCollisionHandler != onCollisionHandler2);
		}
	}

	public event OnCollisionHandler OnOtherCollision
	{
		add
		{
			OnCollisionHandler onCollisionHandler = OnOtherCollision__BackingField;
			OnCollisionHandler onCollisionHandler2;
			do
			{
				onCollisionHandler2 = onCollisionHandler;
				onCollisionHandler = Interlocked.CompareExchange(ref OnOtherCollision__BackingField, (OnCollisionHandler)Delegate.Combine(onCollisionHandler2, value), onCollisionHandler);
			}
			while ((object)onCollisionHandler != onCollisionHandler2);
		}
		remove
		{
			OnCollisionHandler onCollisionHandler = OnOtherCollision__BackingField;
			OnCollisionHandler onCollisionHandler2;
			do
			{
				onCollisionHandler2 = onCollisionHandler;
				onCollisionHandler = Interlocked.CompareExchange(ref OnOtherCollision__BackingField, (OnCollisionHandler)Delegate.Remove(onCollisionHandler2, value), onCollisionHandler);
			}
			while ((object)onCollisionHandler != onCollisionHandler2);
		}
	}

	public bool ForwardParry(out AbstractCollidableObject collisionParent)
	{
		collisionParent = this.collisionParent;
		return forwardParry;
	}

	private void Start()
	{
		if (collisionParent != null)
		{
			collisionParent.RegisterCollisionChild(this);
		}
	}

	protected override void OnCollision(GameObject hit, CollisionPhase phase)
	{
		if (OnAnyCollision__BackingField != null)
		{
			OnAnyCollision__BackingField(hit, phase);
		}
	}

	protected override void OnCollisionWalls(GameObject hit, CollisionPhase phase)
	{
		if (OnWallCollision__BackingField != null)
		{
			OnWallCollision__BackingField(hit, phase);
		}
	}

	protected override void OnCollisionGround(GameObject hit, CollisionPhase phase)
	{
		if (OnGroundCollision__BackingField != null)
		{
			OnGroundCollision__BackingField(hit, phase);
		}
	}

	protected override void OnCollisionCeiling(GameObject hit, CollisionPhase phase)
	{
		if (OnCeilingCollision__BackingField != null)
		{
			OnCeilingCollision__BackingField(hit, phase);
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		if (OnPlayerCollision__BackingField != null)
		{
			OnPlayerCollision__BackingField(hit, phase);
		}
	}

	protected override void OnCollisionPlayerProjectile(GameObject hit, CollisionPhase phase)
	{
		if (OnPlayerProjectileCollision__BackingField != null)
		{
			OnPlayerProjectileCollision__BackingField(hit, phase);
		}
	}

	protected override void OnCollisionEnemy(GameObject hit, CollisionPhase phase)
	{
		if (OnEnemyCollision__BackingField != null)
		{
			OnEnemyCollision__BackingField(hit, phase);
		}
	}

	protected override void OnCollisionEnemyProjectile(GameObject hit, CollisionPhase phase)
	{
		if (OnEnemyProjectileCollision__BackingField != null)
		{
			OnEnemyProjectileCollision__BackingField(hit, phase);
		}
	}

	protected override void OnCollisionOther(GameObject hit, CollisionPhase phase)
	{
		if (OnOtherCollision__BackingField != null)
		{
			OnOtherCollision__BackingField(hit, phase);
		}
	}
}
