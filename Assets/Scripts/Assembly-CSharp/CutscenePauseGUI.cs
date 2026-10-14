using System;
using System.Diagnostics;
using System.Threading;

public class CutscenePauseGUI : AbstractPauseGUI
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action OnPauseEvent__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action OnUnpauseEvent__BackingField;

	public bool pauseAllowed = true;

	protected override bool CanPause
	{
		get
		{
			return PauseManager.state != PauseManager.State.Paused && pauseAllowed;
		}
	}

	public static event Action OnPauseEvent
	{
		add
		{
			Action action = OnPauseEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnPauseEvent__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnPauseEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnPauseEvent__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action OnUnpauseEvent
	{
		add
		{
			Action action = OnUnpauseEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnUnpauseEvent__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnUnpauseEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnUnpauseEvent__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	protected override void OnPause()
	{
		base.OnPause();
		CupheadCutsceneCamera.Current.StartBlur();
		if (OnPauseEvent__BackingField != null)
		{
			OnPauseEvent__BackingField();
		}
	}

	protected override void OnUnpause()
	{
		base.OnUnpause();
		CupheadCutsceneCamera.Current.EndBlur();
		if (OnUnpauseEvent__BackingField != null)
		{
			OnUnpauseEvent__BackingField();
		}
	}

	private void OnDestroy()
	{
		PauseManager.Unpause();
	}

	protected override void Update()
	{
		base.Update();
		if (base.state == State.Paused)
		{
			if (GetButtonDown(CupheadButton.Pause))
			{
				Unpause();
			}
			else if (GetButtonDown(CupheadButton.Cancel))
			{
				Unpause();
			}
			else if (GetButtonDown(CupheadButton.Accept))
			{
				Cutscene.Current.Skip();
			}
		}
	}

	private void Restart()
	{
		base.state = State.Animating;
		SceneLoader.ReloadLevel();
	}

	private void StartNewGame()
	{
		base.state = State.Animating;
		PlayerManager.ResetPlayers();
		SceneLoader.LoadScene(Scenes.scene_title, SceneLoader.Transition.Iris, SceneLoader.Transition.Iris);
	}

	protected override void InAnimation(float i)
	{
	}

	protected override void OutAnimation(float i)
	{
	}
}
