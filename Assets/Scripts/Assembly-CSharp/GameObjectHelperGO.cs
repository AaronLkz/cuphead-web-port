using System;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public class GameObjectHelperGO : MonoBehaviour
{
	public delegate void OnUpdateHandler();

	public delegate void OnFixedUpdateHandler();

	public delegate void OnLateUpdateHandler();

	public delegate void OnDrawGizmosHandler();

	public delegate void OnDestroyHandler();

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private OnUpdateHandler onUpdate__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private OnFixedUpdateHandler onFixedUpdate__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private OnLateUpdateHandler onLateUpdate__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private OnDrawGizmosHandler onDrawGizmos__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private OnDestroyHandler onDestroy__BackingField;

	public event OnUpdateHandler onUpdate
	{
		add
		{
			OnUpdateHandler onUpdateHandler = onUpdate__BackingField;
			OnUpdateHandler onUpdateHandler2;
			do
			{
				onUpdateHandler2 = onUpdateHandler;
				onUpdateHandler = Interlocked.CompareExchange(ref onUpdate__BackingField, (OnUpdateHandler)Delegate.Combine(onUpdateHandler2, value), onUpdateHandler);
			}
			while ((object)onUpdateHandler != onUpdateHandler2);
		}
		remove
		{
			OnUpdateHandler onUpdateHandler = onUpdate__BackingField;
			OnUpdateHandler onUpdateHandler2;
			do
			{
				onUpdateHandler2 = onUpdateHandler;
				onUpdateHandler = Interlocked.CompareExchange(ref onUpdate__BackingField, (OnUpdateHandler)Delegate.Remove(onUpdateHandler2, value), onUpdateHandler);
			}
			while ((object)onUpdateHandler != onUpdateHandler2);
		}
	}

	public event OnFixedUpdateHandler onFixedUpdate
	{
		add
		{
			OnFixedUpdateHandler onFixedUpdateHandler = onFixedUpdate__BackingField;
			OnFixedUpdateHandler onFixedUpdateHandler2;
			do
			{
				onFixedUpdateHandler2 = onFixedUpdateHandler;
				onFixedUpdateHandler = Interlocked.CompareExchange(ref onFixedUpdate__BackingField, (OnFixedUpdateHandler)Delegate.Combine(onFixedUpdateHandler2, value), onFixedUpdateHandler);
			}
			while ((object)onFixedUpdateHandler != onFixedUpdateHandler2);
		}
		remove
		{
			OnFixedUpdateHandler onFixedUpdateHandler = onFixedUpdate__BackingField;
			OnFixedUpdateHandler onFixedUpdateHandler2;
			do
			{
				onFixedUpdateHandler2 = onFixedUpdateHandler;
				onFixedUpdateHandler = Interlocked.CompareExchange(ref onFixedUpdate__BackingField, (OnFixedUpdateHandler)Delegate.Remove(onFixedUpdateHandler2, value), onFixedUpdateHandler);
			}
			while ((object)onFixedUpdateHandler != onFixedUpdateHandler2);
		}
	}

	public event OnLateUpdateHandler onLateUpdate
	{
		add
		{
			OnLateUpdateHandler onLateUpdateHandler = onLateUpdate__BackingField;
			OnLateUpdateHandler onLateUpdateHandler2;
			do
			{
				onLateUpdateHandler2 = onLateUpdateHandler;
				onLateUpdateHandler = Interlocked.CompareExchange(ref onLateUpdate__BackingField, (OnLateUpdateHandler)Delegate.Combine(onLateUpdateHandler2, value), onLateUpdateHandler);
			}
			while ((object)onLateUpdateHandler != onLateUpdateHandler2);
		}
		remove
		{
			OnLateUpdateHandler onLateUpdateHandler = onLateUpdate__BackingField;
			OnLateUpdateHandler onLateUpdateHandler2;
			do
			{
				onLateUpdateHandler2 = onLateUpdateHandler;
				onLateUpdateHandler = Interlocked.CompareExchange(ref onLateUpdate__BackingField, (OnLateUpdateHandler)Delegate.Remove(onLateUpdateHandler2, value), onLateUpdateHandler);
			}
			while ((object)onLateUpdateHandler != onLateUpdateHandler2);
		}
	}

	public event OnDrawGizmosHandler onDrawGizmos
	{
		add
		{
			OnDrawGizmosHandler onDrawGizmosHandler = onDrawGizmos__BackingField;
			OnDrawGizmosHandler onDrawGizmosHandler2;
			do
			{
				onDrawGizmosHandler2 = onDrawGizmosHandler;
				onDrawGizmosHandler = Interlocked.CompareExchange(ref onDrawGizmos__BackingField, (OnDrawGizmosHandler)Delegate.Combine(onDrawGizmosHandler2, value), onDrawGizmosHandler);
			}
			while ((object)onDrawGizmosHandler != onDrawGizmosHandler2);
		}
		remove
		{
			OnDrawGizmosHandler onDrawGizmosHandler = onDrawGizmos__BackingField;
			OnDrawGizmosHandler onDrawGizmosHandler2;
			do
			{
				onDrawGizmosHandler2 = onDrawGizmosHandler;
				onDrawGizmosHandler = Interlocked.CompareExchange(ref onDrawGizmos__BackingField, (OnDrawGizmosHandler)Delegate.Remove(onDrawGizmosHandler2, value), onDrawGizmosHandler);
			}
			while ((object)onDrawGizmosHandler != onDrawGizmosHandler2);
		}
	}

	public event OnDestroyHandler onDestroy
	{
		add
		{
			OnDestroyHandler onDestroyHandler = onDestroy__BackingField;
			OnDestroyHandler onDestroyHandler2;
			do
			{
				onDestroyHandler2 = onDestroyHandler;
				onDestroyHandler = Interlocked.CompareExchange(ref onDestroy__BackingField, (OnDestroyHandler)Delegate.Combine(onDestroyHandler2, value), onDestroyHandler);
			}
			while ((object)onDestroyHandler != onDestroyHandler2);
		}
		remove
		{
			OnDestroyHandler onDestroyHandler = onDestroy__BackingField;
			OnDestroyHandler onDestroyHandler2;
			do
			{
				onDestroyHandler2 = onDestroyHandler;
				onDestroyHandler = Interlocked.CompareExchange(ref onDestroy__BackingField, (OnDestroyHandler)Delegate.Remove(onDestroyHandler2, value), onDestroyHandler);
			}
			while ((object)onDestroyHandler != onDestroyHandler2);
		}
	}

	protected void Update()
	{
		if (onUpdate__BackingField != null)
		{
			onUpdate__BackingField();
		}
	}

	protected void FixedUpdate()
	{
		if (onFixedUpdate__BackingField != null)
		{
			onFixedUpdate__BackingField();
		}
	}

	protected void LateUpdate()
	{
		if (onLateUpdate__BackingField != null)
		{
			onLateUpdate__BackingField();
		}
	}

	protected void OnDrawGizmos()
	{
		if (onDrawGizmos__BackingField != null)
		{
			onDrawGizmos__BackingField();
		}
	}

	protected void OnDestroy()
	{
		if (onDestroy__BackingField != null)
		{
			onDestroy__BackingField();
		}
		clear();
	}

	private void clear()
	{
		onUpdate__BackingField = null;
		onFixedUpdate__BackingField = null;
		onLateUpdate__BackingField = null;
		onDrawGizmos__BackingField = null;
	}
}
