using System;
using System.Diagnostics;
using System.Threading;

public class DummyPlmInterface : PlmInterface
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private OnSuspendHandler OnSuspend__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private OnResumeHandler OnResume__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private OnConstrainedHandler OnConstrained__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private OnUnconstrainedHandler OnUnconstrained__BackingField;

	public event OnSuspendHandler OnSuspend
	{
		add
		{
			OnSuspendHandler onSuspendHandler = OnSuspend__BackingField;
			OnSuspendHandler onSuspendHandler2;
			do
			{
				onSuspendHandler2 = onSuspendHandler;
				onSuspendHandler = Interlocked.CompareExchange(ref OnSuspend__BackingField, (OnSuspendHandler)Delegate.Combine(onSuspendHandler2, value), onSuspendHandler);
			}
			while ((object)onSuspendHandler != onSuspendHandler2);
		}
		remove
		{
			OnSuspendHandler onSuspendHandler = OnSuspend__BackingField;
			OnSuspendHandler onSuspendHandler2;
			do
			{
				onSuspendHandler2 = onSuspendHandler;
				onSuspendHandler = Interlocked.CompareExchange(ref OnSuspend__BackingField, (OnSuspendHandler)Delegate.Remove(onSuspendHandler2, value), onSuspendHandler);
			}
			while ((object)onSuspendHandler != onSuspendHandler2);
		}
	}

	public event OnResumeHandler OnResume
	{
		add
		{
			OnResumeHandler onResumeHandler = OnResume__BackingField;
			OnResumeHandler onResumeHandler2;
			do
			{
				onResumeHandler2 = onResumeHandler;
				onResumeHandler = Interlocked.CompareExchange(ref OnResume__BackingField, (OnResumeHandler)Delegate.Combine(onResumeHandler2, value), onResumeHandler);
			}
			while ((object)onResumeHandler != onResumeHandler2);
		}
		remove
		{
			OnResumeHandler onResumeHandler = OnResume__BackingField;
			OnResumeHandler onResumeHandler2;
			do
			{
				onResumeHandler2 = onResumeHandler;
				onResumeHandler = Interlocked.CompareExchange(ref OnResume__BackingField, (OnResumeHandler)Delegate.Remove(onResumeHandler2, value), onResumeHandler);
			}
			while ((object)onResumeHandler != onResumeHandler2);
		}
	}

	public event OnConstrainedHandler OnConstrained
	{
		add
		{
			OnConstrainedHandler onConstrainedHandler = OnConstrained__BackingField;
			OnConstrainedHandler onConstrainedHandler2;
			do
			{
				onConstrainedHandler2 = onConstrainedHandler;
				onConstrainedHandler = Interlocked.CompareExchange(ref OnConstrained__BackingField, (OnConstrainedHandler)Delegate.Combine(onConstrainedHandler2, value), onConstrainedHandler);
			}
			while ((object)onConstrainedHandler != onConstrainedHandler2);
		}
		remove
		{
			OnConstrainedHandler onConstrainedHandler = OnConstrained__BackingField;
			OnConstrainedHandler onConstrainedHandler2;
			do
			{
				onConstrainedHandler2 = onConstrainedHandler;
				onConstrainedHandler = Interlocked.CompareExchange(ref OnConstrained__BackingField, (OnConstrainedHandler)Delegate.Remove(onConstrainedHandler2, value), onConstrainedHandler);
			}
			while ((object)onConstrainedHandler != onConstrainedHandler2);
		}
	}

	public event OnUnconstrainedHandler OnUnconstrained
	{
		add
		{
			OnUnconstrainedHandler onUnconstrainedHandler = OnUnconstrained__BackingField;
			OnUnconstrainedHandler onUnconstrainedHandler2;
			do
			{
				onUnconstrainedHandler2 = onUnconstrainedHandler;
				onUnconstrainedHandler = Interlocked.CompareExchange(ref OnUnconstrained__BackingField, (OnUnconstrainedHandler)Delegate.Combine(onUnconstrainedHandler2, value), onUnconstrainedHandler);
			}
			while ((object)onUnconstrainedHandler != onUnconstrainedHandler2);
		}
		remove
		{
			OnUnconstrainedHandler onUnconstrainedHandler = OnUnconstrained__BackingField;
			OnUnconstrainedHandler onUnconstrainedHandler2;
			do
			{
				onUnconstrainedHandler2 = onUnconstrainedHandler;
				onUnconstrainedHandler = Interlocked.CompareExchange(ref OnUnconstrained__BackingField, (OnUnconstrainedHandler)Delegate.Remove(onUnconstrainedHandler2, value), onUnconstrainedHandler);
			}
			while ((object)onUnconstrainedHandler != onUnconstrainedHandler2);
		}
	}

	public void Init()
	{
	}

	public bool IsConstrained()
	{
		return false;
	}
}
