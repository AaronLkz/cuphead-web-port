using System;
using System.Diagnostics;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Rewired.UI.ControlMapper
{
	[AddComponentMenu("")]
	public abstract class UIElementInfo : MonoBehaviour, ISelectHandler, IEventSystemHandler
	{
		public string identifier;

		public int intData;

		public Text text;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<GameObject> OnSelectedEvent__BackingField;

		public event Action<GameObject> OnSelectedEvent
		{
			add
			{
				Action<GameObject> action = OnSelectedEvent__BackingField;
				Action<GameObject> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnSelectedEvent__BackingField, (Action<GameObject>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<GameObject> action = OnSelectedEvent__BackingField;
				Action<GameObject> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnSelectedEvent__BackingField, (Action<GameObject>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public void OnSelect(BaseEventData eventData)
		{
			if (OnSelectedEvent__BackingField != null)
			{
				OnSelectedEvent__BackingField(base.gameObject);
			}
		}
	}
}
