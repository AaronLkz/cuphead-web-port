using System;
using System.Collections.Generic;

public class EventManager
{
	public delegate void EventDelegate<T>(T e) where T : GameEvent;

	private delegate void EventDelegate(GameEvent e);

	private static EventManager _instance;

	private Dictionary<Type, EventDelegate> delegates = new Dictionary<Type, EventDelegate>();

	private Dictionary<Delegate, EventDelegate> delegateLookup = new Dictionary<Delegate, EventDelegate>();

	public static EventManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new EventManager();
			}
			return _instance;
		}
	}

	public void AddListener<T>(EventDelegate<T> del) where T : GameEvent
	{
		if (!delegateLookup.ContainsKey(del))
		{
			EventDelegate eventDelegate = (GameEvent e) =>
			{
				del((T)e);
			};
			delegateLookup[del] = eventDelegate;
			EventDelegate value;
			if (delegates.TryGetValue(typeof(T), out value))
			{
				value = (delegates[typeof(T)] = (EventDelegate)Delegate.Combine(value, eventDelegate));
			}
			else
			{
				delegates[typeof(T)] = eventDelegate;
			}
		}
	}

	public void RemoveListener<T>(EventDelegate<T> del) where T : GameEvent
	{
		EventDelegate value;
		if (!delegateLookup.TryGetValue(del, out value))
		{
			return;
		}
		EventDelegate value2;
		if (delegates.TryGetValue(typeof(T), out value2))
		{
			value2 = (EventDelegate)Delegate.Remove(value2, value);
			if (value2 == null)
			{
				delegates.Remove(typeof(T));
			}
			else
			{
				delegates[typeof(T)] = value2;
			}
		}
		delegateLookup.Remove(del);
	}

	public void Raise(GameEvent e)
	{
		EventDelegate value;
		if (delegates.TryGetValue(e.GetType(), out value))
		{
			value(e);
		}
	}
}
