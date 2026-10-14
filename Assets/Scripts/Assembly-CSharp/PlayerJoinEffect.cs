using System;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public class PlayerJoinEffect : AbstractMonoBehaviour
{
	public const string NAME = "Player_Join";

	[SerializeField]
	private SpriteRenderer cuphead;

	[SerializeField]
	private SpriteRenderer mugman;

	[SerializeField]
	private SpriteRenderer chalice;

	private PlayerId playerId;

	private SpriteRenderer spriteRenderer;

	private PlayerMode playerMode;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private AbstractPlayerController.OnReviveHandler OnPreReviveEvent__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private AbstractPlayerController.OnReviveHandler OnReviveEvent__BackingField;

	public event AbstractPlayerController.OnReviveHandler OnPreReviveEvent
	{
		add
		{
			AbstractPlayerController.OnReviveHandler onReviveHandler = OnPreReviveEvent__BackingField;
			AbstractPlayerController.OnReviveHandler onReviveHandler2;
			do
			{
				onReviveHandler2 = onReviveHandler;
				onReviveHandler = Interlocked.CompareExchange(ref OnPreReviveEvent__BackingField, (AbstractPlayerController.OnReviveHandler)Delegate.Combine(onReviveHandler2, value), onReviveHandler);
			}
			while ((object)onReviveHandler != onReviveHandler2);
		}
		remove
		{
			AbstractPlayerController.OnReviveHandler onReviveHandler = OnPreReviveEvent__BackingField;
			AbstractPlayerController.OnReviveHandler onReviveHandler2;
			do
			{
				onReviveHandler2 = onReviveHandler;
				onReviveHandler = Interlocked.CompareExchange(ref OnPreReviveEvent__BackingField, (AbstractPlayerController.OnReviveHandler)Delegate.Remove(onReviveHandler2, value), onReviveHandler);
			}
			while ((object)onReviveHandler != onReviveHandler2);
		}
	}

	public event AbstractPlayerController.OnReviveHandler OnReviveEvent
	{
		add
		{
			AbstractPlayerController.OnReviveHandler onReviveHandler = OnReviveEvent__BackingField;
			AbstractPlayerController.OnReviveHandler onReviveHandler2;
			do
			{
				onReviveHandler2 = onReviveHandler;
				onReviveHandler = Interlocked.CompareExchange(ref OnReviveEvent__BackingField, (AbstractPlayerController.OnReviveHandler)Delegate.Combine(onReviveHandler2, value), onReviveHandler);
			}
			while ((object)onReviveHandler != onReviveHandler2);
		}
		remove
		{
			AbstractPlayerController.OnReviveHandler onReviveHandler = OnReviveEvent__BackingField;
			AbstractPlayerController.OnReviveHandler onReviveHandler2;
			do
			{
				onReviveHandler2 = onReviveHandler;
				onReviveHandler = Interlocked.CompareExchange(ref OnReviveEvent__BackingField, (AbstractPlayerController.OnReviveHandler)Delegate.Remove(onReviveHandler2, value), onReviveHandler);
			}
			while ((object)onReviveHandler != onReviveHandler2);
		}
	}

	public static PlayerJoinEffect Create(PlayerId playerId, Vector2 pos, PlayerMode mode, bool isChalice)
	{
		PlayerJoinEffect playerJoinEffect = UnityEngine.Object.Instantiate(Level.Current.LevelResources.joinEffect);
		playerJoinEffect.name = playerJoinEffect.name.Replace("(Clone)", string.Empty);
		playerJoinEffect.Init(playerId, pos, mode, isChalice);
		return playerJoinEffect;
	}

	protected override void Awake()
	{
		base.Awake();
		PlayerManager.OnPlayerLeaveEvent += OnPlayerLeave;
	}

	private void Init(PlayerId playerId, Vector2 pos, PlayerMode mode, bool isChalice)
	{
		this.playerId = playerId;
		playerMode = mode;
		base.animator.SetInteger("Mode", (int)playerMode);
		if (playerId == PlayerId.PlayerOne || playerId != PlayerId.PlayerTwo)
		{
			spriteRenderer = cuphead;
		}
		else
		{
			spriteRenderer = mugman;
		}
		if (isChalice)
		{
			spriteRenderer = chalice;
		}
		cuphead.gameObject.SetActive(false);
		mugman.gameObject.SetActive(false);
		chalice.gameObject.SetActive(false);
		spriteRenderer.gameObject.SetActive(true);
		base.transform.position = pos;
	}

	public void GameOverUnpause()
	{
		base.animator.enabled = true;
		AnimationHelper component = GetComponent<AnimationHelper>();
		component.IgnoreGlobal = true;
		ignoreGlobalTime = true;
	}

	private void OnReviveStealAnimComplete()
	{
		if (OnReviveEvent__BackingField != null)
		{
			OnReviveEvent__BackingField(base.transform.position);
		}
		OnReviveEvent__BackingField = null;
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void OnPlayerLeave(PlayerId id)
	{
		if (playerId == id)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void OnDestroy()
	{
		PlayerManager.OnPlayerLeaveEvent -= OnPlayerLeave;
	}
}
