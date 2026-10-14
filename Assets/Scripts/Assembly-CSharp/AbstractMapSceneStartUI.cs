using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using Rewired;
using UnityEngine;

public class AbstractMapSceneStartUI : AbstractMonoBehaviour
{
	public enum State
	{
		Inactive = 0,
		Animating = 1,
		Active = 2,
		Loading = 3
	}

	[HideInInspector]
	public string level;

	private CanvasGroup canvasGroup;

	private Player player;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action OnLoadLevelEvent__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action OnBackEvent__BackingField;

	public State CurrentState { get; protected set; }

	protected bool Able
	{
		get
		{
			if (CurrentState != State.Active)
			{
				return false;
			}
			if (AbstractEquipUI.Current.CurrentState != AbstractEquipUI.ActiveState.Inactive)
			{
				return false;
			}
			if (Map.Current.CurrentState != Map.State.Ready)
			{
				return false;
			}
			if (InterruptingPrompt.IsInterrupting())
			{
				return false;
			}
			if (Map.Current != null && Map.Current.CurrentState == Map.State.Graveyard)
			{
				return false;
			}
			return true;
		}
	}

	public event Action OnLoadLevelEvent
	{
		add
		{
			Action action = OnLoadLevelEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnLoadLevelEvent__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnLoadLevelEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnLoadLevelEvent__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public event Action OnBackEvent
	{
		add
		{
			Action action = OnBackEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnBackEvent__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnBackEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnBackEvent__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	protected override void Awake()
	{
		base.Awake();
		timeLayer = CupheadTime.Layer.UI;
		ignoreGlobalTime = true;
		canvasGroup = GetComponent<CanvasGroup>();
		SetAlpha(0f);
	}

	protected virtual void Start()
	{
		PlayerManager.OnPlayerJoinedEvent += OnPlayerJoined;
		PlayerManager.OnPlayerJoinedEvent += OnPlayerLeft;
	}

	private void OnDestroy()
	{
		PlayerManager.OnPlayerJoinedEvent -= OnPlayerJoined;
		PlayerManager.OnPlayerJoinedEvent -= OnPlayerLeft;
	}

	protected bool GetButtonDown(CupheadButton button)
	{
		if (!Able || InterruptingPrompt.IsInterrupting())
		{
			return false;
		}
		if (player != null && player.GetButtonDown((int)button))
		{
			return true;
		}
		return false;
	}

	private void OnPlayerJoined(PlayerId playerId)
	{
	}

	private void OnPlayerLeft(PlayerId playerId)
	{
	}

	protected void LoadLevel()
	{
		CurrentState = State.Loading;
		if (OnLoadLevelEvent__BackingField != null)
		{
			OnLoadLevelEvent__BackingField();
		}
		OnLoadLevelEvent__BackingField = null;
	}

	public void In(MapPlayerController playerController)
	{
		player = playerController.input.actions;
		StartCoroutine(fade_cr(1f, State.Active));
	}

	public void Out()
	{
		StartCoroutine(fade_cr(0f, State.Inactive));
		if (OnBackEvent__BackingField != null)
		{
			OnBackEvent__BackingField();
		}
		OnBackEvent__BackingField = null;
	}

	protected void SetAlpha(float alpha)
	{
		canvasGroup.alpha = alpha;
	}

	private IEnumerator fade_cr(float end, State endState)
	{
		float t = 0f;
		float start = canvasGroup.alpha;
		CurrentState = State.Animating;
		while (t < 0.2f)
		{
			float val = t / 0.2f;
			SetAlpha(Mathf.Lerp(start, end, val));
			t += Time.deltaTime;
			yield return null;
		}
		SetAlpha(end);
		CurrentState = endState;
	}
}
