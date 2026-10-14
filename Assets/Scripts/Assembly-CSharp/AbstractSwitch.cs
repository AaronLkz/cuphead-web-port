using System;
using System.Diagnostics;
using System.Threading;

public abstract class AbstractSwitch : AbstractCollidableObject
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action OnActivate__BackingField;

	public event Action OnActivate
	{
		add
		{
			Action action = OnActivate__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnActivate__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnActivate__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnActivate__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	protected void DispatchEvent()
	{
		if (OnActivate__BackingField != null)
		{
			OnActivate__BackingField();
		}
	}
}
