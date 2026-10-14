using System;
using System.Diagnostics;
using System.Threading;

namespace DialoguerCore
{
	public class DialoguerEventManager
	{
		public delegate void StartedHandler();

		public delegate void EndedHandler();

		public delegate void SuddenlyEndedHandler();

		public delegate void TextPhaseHandler(DialoguerTextData data);

		public delegate void WindowCloseHandler();

		public delegate void WaitStartHandler();

		public delegate void WaitCompleteHandler();

		public delegate void MessageEventHandler(string message, string metadata);

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static StartedHandler onStarted__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static EndedHandler onEnded__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static SuddenlyEndedHandler onSuddenlyEnded__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static TextPhaseHandler onTextPhase__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static WindowCloseHandler onWindowClose__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static WaitStartHandler onWaitStart__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static WaitCompleteHandler onWaitComplete__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static MessageEventHandler onMessageEvent__BackingField;

		public static event StartedHandler onStarted
		{
			add
			{
				StartedHandler startedHandler = onStarted__BackingField;
				StartedHandler startedHandler2;
				do
				{
					startedHandler2 = startedHandler;
					startedHandler = Interlocked.CompareExchange(ref onStarted__BackingField, (StartedHandler)Delegate.Combine(startedHandler2, value), startedHandler);
				}
				while ((object)startedHandler != startedHandler2);
			}
			remove
			{
				StartedHandler startedHandler = onStarted__BackingField;
				StartedHandler startedHandler2;
				do
				{
					startedHandler2 = startedHandler;
					startedHandler = Interlocked.CompareExchange(ref onStarted__BackingField, (StartedHandler)Delegate.Remove(startedHandler2, value), startedHandler);
				}
				while ((object)startedHandler != startedHandler2);
			}
		}

		public static event EndedHandler onEnded
		{
			add
			{
				EndedHandler endedHandler = onEnded__BackingField;
				EndedHandler endedHandler2;
				do
				{
					endedHandler2 = endedHandler;
					endedHandler = Interlocked.CompareExchange(ref onEnded__BackingField, (EndedHandler)Delegate.Combine(endedHandler2, value), endedHandler);
				}
				while ((object)endedHandler != endedHandler2);
			}
			remove
			{
				EndedHandler endedHandler = onEnded__BackingField;
				EndedHandler endedHandler2;
				do
				{
					endedHandler2 = endedHandler;
					endedHandler = Interlocked.CompareExchange(ref onEnded__BackingField, (EndedHandler)Delegate.Remove(endedHandler2, value), endedHandler);
				}
				while ((object)endedHandler != endedHandler2);
			}
		}

		public static event SuddenlyEndedHandler onSuddenlyEnded
		{
			add
			{
				SuddenlyEndedHandler suddenlyEndedHandler = onSuddenlyEnded__BackingField;
				SuddenlyEndedHandler suddenlyEndedHandler2;
				do
				{
					suddenlyEndedHandler2 = suddenlyEndedHandler;
					suddenlyEndedHandler = Interlocked.CompareExchange(ref onSuddenlyEnded__BackingField, (SuddenlyEndedHandler)Delegate.Combine(suddenlyEndedHandler2, value), suddenlyEndedHandler);
				}
				while ((object)suddenlyEndedHandler != suddenlyEndedHandler2);
			}
			remove
			{
				SuddenlyEndedHandler suddenlyEndedHandler = onSuddenlyEnded__BackingField;
				SuddenlyEndedHandler suddenlyEndedHandler2;
				do
				{
					suddenlyEndedHandler2 = suddenlyEndedHandler;
					suddenlyEndedHandler = Interlocked.CompareExchange(ref onSuddenlyEnded__BackingField, (SuddenlyEndedHandler)Delegate.Remove(suddenlyEndedHandler2, value), suddenlyEndedHandler);
				}
				while ((object)suddenlyEndedHandler != suddenlyEndedHandler2);
			}
		}

		public static event TextPhaseHandler onTextPhase
		{
			add
			{
				TextPhaseHandler textPhaseHandler = onTextPhase__BackingField;
				TextPhaseHandler textPhaseHandler2;
				do
				{
					textPhaseHandler2 = textPhaseHandler;
					textPhaseHandler = Interlocked.CompareExchange(ref onTextPhase__BackingField, (TextPhaseHandler)Delegate.Combine(textPhaseHandler2, value), textPhaseHandler);
				}
				while ((object)textPhaseHandler != textPhaseHandler2);
			}
			remove
			{
				TextPhaseHandler textPhaseHandler = onTextPhase__BackingField;
				TextPhaseHandler textPhaseHandler2;
				do
				{
					textPhaseHandler2 = textPhaseHandler;
					textPhaseHandler = Interlocked.CompareExchange(ref onTextPhase__BackingField, (TextPhaseHandler)Delegate.Remove(textPhaseHandler2, value), textPhaseHandler);
				}
				while ((object)textPhaseHandler != textPhaseHandler2);
			}
		}

		public static event WindowCloseHandler onWindowClose
		{
			add
			{
				WindowCloseHandler windowCloseHandler = onWindowClose__BackingField;
				WindowCloseHandler windowCloseHandler2;
				do
				{
					windowCloseHandler2 = windowCloseHandler;
					windowCloseHandler = Interlocked.CompareExchange(ref onWindowClose__BackingField, (WindowCloseHandler)Delegate.Combine(windowCloseHandler2, value), windowCloseHandler);
				}
				while ((object)windowCloseHandler != windowCloseHandler2);
			}
			remove
			{
				WindowCloseHandler windowCloseHandler = onWindowClose__BackingField;
				WindowCloseHandler windowCloseHandler2;
				do
				{
					windowCloseHandler2 = windowCloseHandler;
					windowCloseHandler = Interlocked.CompareExchange(ref onWindowClose__BackingField, (WindowCloseHandler)Delegate.Remove(windowCloseHandler2, value), windowCloseHandler);
				}
				while ((object)windowCloseHandler != windowCloseHandler2);
			}
		}

		public static event WaitStartHandler onWaitStart
		{
			add
			{
				WaitStartHandler waitStartHandler = onWaitStart__BackingField;
				WaitStartHandler waitStartHandler2;
				do
				{
					waitStartHandler2 = waitStartHandler;
					waitStartHandler = Interlocked.CompareExchange(ref onWaitStart__BackingField, (WaitStartHandler)Delegate.Combine(waitStartHandler2, value), waitStartHandler);
				}
				while ((object)waitStartHandler != waitStartHandler2);
			}
			remove
			{
				WaitStartHandler waitStartHandler = onWaitStart__BackingField;
				WaitStartHandler waitStartHandler2;
				do
				{
					waitStartHandler2 = waitStartHandler;
					waitStartHandler = Interlocked.CompareExchange(ref onWaitStart__BackingField, (WaitStartHandler)Delegate.Remove(waitStartHandler2, value), waitStartHandler);
				}
				while ((object)waitStartHandler != waitStartHandler2);
			}
		}

		public static event WaitCompleteHandler onWaitComplete
		{
			add
			{
				WaitCompleteHandler waitCompleteHandler = onWaitComplete__BackingField;
				WaitCompleteHandler waitCompleteHandler2;
				do
				{
					waitCompleteHandler2 = waitCompleteHandler;
					waitCompleteHandler = Interlocked.CompareExchange(ref onWaitComplete__BackingField, (WaitCompleteHandler)Delegate.Combine(waitCompleteHandler2, value), waitCompleteHandler);
				}
				while ((object)waitCompleteHandler != waitCompleteHandler2);
			}
			remove
			{
				WaitCompleteHandler waitCompleteHandler = onWaitComplete__BackingField;
				WaitCompleteHandler waitCompleteHandler2;
				do
				{
					waitCompleteHandler2 = waitCompleteHandler;
					waitCompleteHandler = Interlocked.CompareExchange(ref onWaitComplete__BackingField, (WaitCompleteHandler)Delegate.Remove(waitCompleteHandler2, value), waitCompleteHandler);
				}
				while ((object)waitCompleteHandler != waitCompleteHandler2);
			}
		}

		public static event MessageEventHandler onMessageEvent
		{
			add
			{
				MessageEventHandler messageEventHandler = onMessageEvent__BackingField;
				MessageEventHandler messageEventHandler2;
				do
				{
					messageEventHandler2 = messageEventHandler;
					messageEventHandler = Interlocked.CompareExchange(ref onMessageEvent__BackingField, (MessageEventHandler)Delegate.Combine(messageEventHandler2, value), messageEventHandler);
				}
				while ((object)messageEventHandler != messageEventHandler2);
			}
			remove
			{
				MessageEventHandler messageEventHandler = onMessageEvent__BackingField;
				MessageEventHandler messageEventHandler2;
				do
				{
					messageEventHandler2 = messageEventHandler;
					messageEventHandler = Interlocked.CompareExchange(ref onMessageEvent__BackingField, (MessageEventHandler)Delegate.Remove(messageEventHandler2, value), messageEventHandler);
				}
				while ((object)messageEventHandler != messageEventHandler2);
			}
		}

		public static void dispatchOnStarted()
		{
			if (onStarted__BackingField != null)
			{
				onStarted__BackingField();
			}
		}

		public static void dispatchOnEnded()
		{
			if (onEnded__BackingField != null)
			{
				onEnded__BackingField();
			}
		}

		public static void dispatchOnSuddenlyEnded()
		{
			if (onSuddenlyEnded__BackingField != null)
			{
				onSuddenlyEnded__BackingField();
			}
		}

		public static void dispatchOnTextPhase(DialoguerTextData data)
		{
			if (onTextPhase__BackingField != null)
			{
				onTextPhase__BackingField(data);
			}
		}

		public static void dispatchOnWindowClose()
		{
			if (onWindowClose__BackingField != null)
			{
				onWindowClose__BackingField();
			}
		}

		public static void dispatchOnWaitStart()
		{
			if (onWaitStart__BackingField != null)
			{
				onWaitStart__BackingField();
			}
		}

		public static void dispatchOnWaitComplete()
		{
			if (onWaitComplete__BackingField != null)
			{
				onWaitComplete__BackingField();
			}
		}

		public static void dispatchOnMessageEvent(string message, string metadata)
		{
			if (onMessageEvent__BackingField != null)
			{
				onMessageEvent__BackingField(message, metadata);
			}
		}
	}
}
