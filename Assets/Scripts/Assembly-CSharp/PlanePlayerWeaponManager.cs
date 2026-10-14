using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public class PlanePlayerWeaponManager : AbstractPlanePlayerComponent
{
	public enum State
	{
		Inactive = 0,
		Ready = 1,
		Busy = 2
	}

	public delegate void OnWeaponChangeHandler(Weapon weapon);

	[Serializable]
	public class Weapons
	{
		public AbstractPlaneWeapon peashot;

		public AbstractPlaneWeapon bomb;

		public AbstractPlaneWeapon chalice3Way;

		public AbstractPlaneWeapon chaliceBomb;

		public void Init(PlanePlayerWeaponManager manager)
		{
			peashot = UnityEngine.Object.Instantiate(peashot);
			peashot.Initialize(manager, 0);
			peashot.transform.SetParent(manager.transform);
			peashot.transform.ResetLocalTransforms();
			bomb = UnityEngine.Object.Instantiate(bomb);
			bomb.Initialize(manager, 2);
			bomb.transform.SetParent(manager.transform);
			bomb.transform.ResetLocalTransforms();
			chalice3Way = UnityEngine.Object.Instantiate(chalice3Way);
			chalice3Way.Initialize(manager, 3);
			chalice3Way.transform.SetParent(manager.transform);
			chalice3Way.transform.ResetLocalTransforms();
			chaliceBomb = UnityEngine.Object.Instantiate(chaliceBomb);
			chaliceBomb.Initialize(manager, 4);
			chaliceBomb.transform.SetParent(manager.transform);
			chaliceBomb.transform.ResetLocalTransforms();
		}

		public AbstractPlaneWeapon GetWeapon(Weapon weapon)
		{
			switch (weapon)
			{
			case Weapon.plane_weapon_peashot:
				return peashot;
			case Weapon.plane_weapon_bomb:
				return bomb;
			case Weapon.plane_chalice_weapon_3way:
				return chalice3Way;
			case Weapon.plane_chalice_weapon_bomb:
				return chaliceBomb;
			default:
				return null;
			}
		}

		public void OnDestroy()
		{
			peashot = null;
			bomb = null;
		}
	}

	public class States
	{
		public enum Basic
		{
			Ready = 0
		}

		public enum Ex
		{
			Ready = 0,
			Intro = 1,
			Fire = 2,
			Ending = 3
		}

		public enum Super
		{
			Ready = 0,
			Intro = 1,
			Countdown = 2,
			Ending = 3
		}

		public Super super;

		public Basic basic { get; internal set; }

		public Ex ex { get; internal set; }

		public States()
		{
			basic = Basic.Ready;
			ex = Ex.Ready;
			super = Super.Ready;
		}
	}

	[SerializeField]
	private Weapons weapons;

	[SerializeField]
	private AbstractPlaneSuper super;

	[SerializeField]
	private AbstractPlaneSuper chaliceSuper;

	[Space(10f)]
	[SerializeField]
	private Transform bulletRoot;

	[NonSerialized]
	public bool IsShooting;

	private Weapon currentWeapon = Weapon.plane_weapon_peashot;

	private Weapon unshrunkWeapon = Weapon.None;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private OnWeaponChangeHandler OnWeaponChangeEvent__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action OnExStartEvent__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action OnExFireEvent__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action OnSuperStartEvent__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action OnSuperCountdownEvent__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action OnSuperFireEvent__BackingField;

	public State state { get; protected set; }

	public AbstractPlaneWeapon CurrentWeapon
	{
		get
		{
			return weapons.GetWeapon(currentWeapon);
		}
	}

	public States states { get; private set; }

	public bool CanInterupt { get; private set; }

	public event OnWeaponChangeHandler OnWeaponChangeEvent
	{
		add
		{
			OnWeaponChangeHandler onWeaponChangeHandler = OnWeaponChangeEvent__BackingField;
			OnWeaponChangeHandler onWeaponChangeHandler2;
			do
			{
				onWeaponChangeHandler2 = onWeaponChangeHandler;
				onWeaponChangeHandler = Interlocked.CompareExchange(ref OnWeaponChangeEvent__BackingField, (OnWeaponChangeHandler)Delegate.Combine(onWeaponChangeHandler2, value), onWeaponChangeHandler);
			}
			while ((object)onWeaponChangeHandler != onWeaponChangeHandler2);
		}
		remove
		{
			OnWeaponChangeHandler onWeaponChangeHandler = OnWeaponChangeEvent__BackingField;
			OnWeaponChangeHandler onWeaponChangeHandler2;
			do
			{
				onWeaponChangeHandler2 = onWeaponChangeHandler;
				onWeaponChangeHandler = Interlocked.CompareExchange(ref OnWeaponChangeEvent__BackingField, (OnWeaponChangeHandler)Delegate.Remove(onWeaponChangeHandler2, value), onWeaponChangeHandler);
			}
			while ((object)onWeaponChangeHandler != onWeaponChangeHandler2);
		}
	}

	public event Action OnExStartEvent
	{
		add
		{
			Action action = OnExStartEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnExStartEvent__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnExStartEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnExStartEvent__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public event Action OnExFireEvent
	{
		add
		{
			Action action = OnExFireEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnExFireEvent__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnExFireEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnExFireEvent__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public event Action OnSuperStartEvent
	{
		add
		{
			Action action = OnSuperStartEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnSuperStartEvent__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnSuperStartEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnSuperStartEvent__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public event Action OnSuperCountdownEvent
	{
		add
		{
			Action action = OnSuperCountdownEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnSuperCountdownEvent__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnSuperCountdownEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnSuperCountdownEvent__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public event Action OnSuperFireEvent
	{
		add
		{
			Action action = OnSuperFireEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnSuperFireEvent__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnSuperFireEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnSuperFireEvent__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	private void Start()
	{
		weapons.Init(this);
		states = new States();
		base.player.animationController.OnExFireAnimEvent += OnExAnimFire;
		base.player.OnReviveEvent += OnRevive;
		base.player.stats.OnPlayerDeathEvent += StopSound;
		CanInterupt = true;
	}

	private void FixedUpdate()
	{
		if (state != State.Inactive && state != State.Busy)
		{
			CheckBasic();
			CheckEx();
			HandleWeaponSwitch();
		}
	}

	public override void OnLevelStart()
	{
		base.OnLevelStart();
		if (base.player.stats.isChalice)
		{
			currentWeapon = Weapon.plane_chalice_weapon_3way;
		}
		else if (base.player.stats.Loadout.charm == Charm.charm_curse && base.player.stats.CurseCharmLevel >= 0)
		{
			int[] availableShmupWeaponIDs = WeaponProperties.CharmCurse.availableShmupWeaponIDs;
			currentWeapon = (Weapon)availableShmupWeaponIDs[UnityEngine.Random.Range(0, availableShmupWeaponIDs.Length)];
		}
		if (!(base.player.stats.StoneTime > 0f))
		{
			state = State.Ready;
			if (base.player.input.actions.GetButton(3))
			{
				StartBasic();
			}
		}
	}

	public override void OnLevelEnd()
	{
		base.OnLevelEnd();
		EndBasic();
	}

	public void SwitchToWeapon(Weapon weapon)
	{
		if (weapon != Weapon.None)
		{
			weapons.GetWeapon(currentWeapon).EndBasic();
			weapons.GetWeapon(currentWeapon).EndEx();
			if (OnWeaponChangeEvent__BackingField != null)
			{
				OnWeaponChangeEvent__BackingField(weapon);
			}
			currentWeapon = weapon;
			if (base.player.input.actions.GetButton(3))
			{
				StartBasic();
			}
		}
	}

	private void OnRevive(Vector3 pos)
	{
		IsShooting = false;
		state = State.Ready;
		states.basic = States.Basic.Ready;
		states.ex = States.Ex.Ready;
		CanInterupt = true;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		weapons.OnDestroy();
		super = null;
	}

	private void CheckBasic()
	{
		if (base.player.stats.Loadout.charm == Charm.charm_EX)
		{
			return;
		}
		if ((base.player.input.actions.GetButtonDown(3) || (base.player.input.actions.GetButtonTimePressed(3) > 0f && !IsShooting)) && base.player.stats.StoneTime <= 0f)
		{
			if (base.player.stats.Loadout.charm == Charm.charm_curse && base.player.stats.CurseCharmLevel >= 0 && !base.player.Shrunk)
			{
				int[] availableShmupWeaponIDs = WeaponProperties.CharmCurse.availableShmupWeaponIDs;
				int num;
				for (num = (int)currentWeapon; num == (int)currentWeapon; num = availableShmupWeaponIDs[UnityEngine.Random.Range(0, availableShmupWeaponIDs.Length)])
				{
				}
				SwitchWeapon((Weapon)num);
			}
			else
			{
				StartBasic();
			}
		}
		else if (base.player.input.actions.GetButtonUp(3) || (IsShooting && base.player.stats.StoneTime > 0f))
		{
			EndBasic();
		}
		else if (!base.player.input.actions.GetButton(3) && IsShooting)
		{
			EndBasic();
		}
		else if ((!base.player.Shrunk && unshrunkWeapon != Weapon.None) || (IsShooting && base.player.Shrunk && currentWeapon != Weapon.plane_weapon_peashot))
		{
			EndBasic();
			if (base.player.input.actions.GetButton(3))
			{
				StartBasic();
			}
		}
	}

	private void StartBasic()
	{
		if ((currentWeapon == Weapon.plane_weapon_bomb || currentWeapon == Weapon.plane_chalice_weapon_3way || currentWeapon == Weapon.plane_chalice_weapon_bomb) && base.player.Shrunk)
		{
			unshrunkWeapon = currentWeapon;
			currentWeapon = Weapon.plane_weapon_peashot;
		}
		weapons.GetWeapon(currentWeapon).BeginBasic();
	}

	private void EndBasic()
	{
		weapons.GetWeapon(currentWeapon).EndBasic();
		if (unshrunkWeapon != Weapon.None)
		{
			currentWeapon = unshrunkWeapon;
			unshrunkWeapon = Weapon.None;
		}
		StopSound(base.player.id);
	}

	private void StopSound(PlayerId id)
	{
		if ((id == PlayerId.PlayerOne && !PlayerManager.player1IsMugman) || (id == PlayerId.PlayerTwo && PlayerManager.player1IsMugman))
		{
			if (AudioManager.CheckIfPlaying("player_plane_weapon_fire_loop_cuphead"))
			{
				AudioManager.Stop("player_plane_weapon_fire_loop_cuphead");
				AudioManager.Play("player_plane_weapon_fire_loop_end_cuphead");
				emitAudioFromObject.Add("player_plane_weapon_fire_loop_end_cuphead");
			}
		}
		else if (AudioManager.CheckIfPlaying("player_plane_weapon_fire_loop_mugman"))
		{
			AudioManager.Stop("player_plane_weapon_fire_loop_mugman");
			AudioManager.Play("player_plane_weapon_fire_loop_end_mugman");
			emitAudioFromObject.Add("player_plane_weapon_fire_loop_end_mugman");
		}
	}

	private void CheckEx()
	{
		if (base.player.stats.CanUseEx && !base.player.Parrying && !base.player.Shrunk && !(base.player.stats.StoneTime > 0f) && (base.player.input.actions.GetButtonDown(4) || base.player.motor.HasBufferedInput(PlanePlayerMotor.BufferedInput.Super) || (base.player.stats.Loadout.charm == Charm.charm_EX && base.player.input.actions.GetButton(3))))
		{
			if (base.player.stats.SuperMeter >= base.player.stats.SuperMeterMax && base.player.stats.Loadout.charm != Charm.charm_EX)
			{
				StartSuper();
			}
			else
			{
				StartEx();
			}
			base.player.motor.ClearBufferedInput();
		}
	}

	private void StartEx()
	{
		StartCoroutine(ex_cr());
	}

	private void OnExAnimFire()
	{
		states.ex = States.Ex.Fire;
	}

	private IEnumerator ex_cr()
	{
		AudioManager.Play("player_plane_weapon_special_fire");
		state = State.Inactive;
		states.ex = States.Ex.Intro;
		CanInterupt = false;
		EndBasic();
		base.player.stats.OnEx();
		if (OnExStartEvent__BackingField != null)
		{
			OnExStartEvent__BackingField();
		}
		while (states.ex != States.Ex.Fire)
		{
			if (base.player.stats.StoneTime > 0f)
			{
				CancelEX();
				yield return null;
			}
			yield return null;
		}
		weapons.GetWeapon(currentWeapon).BeginEx();
		if (OnExFireEvent__BackingField != null)
		{
			OnExFireEvent__BackingField();
		}
		AudioManager.Play("player_plane_up_ex_end");
		states.ex = States.Ex.Ending;
		while (base.animator.GetCurrentAnimatorStateInfo(0).IsName("Ex_End"))
		{
			if (base.player.stats.StoneTime > 0f)
			{
				CancelEX();
				yield return null;
			}
			yield return null;
		}
		state = State.Ready;
		states.ex = States.Ex.Ready;
		if (base.player.input.actions.GetButtonDown(3))
		{
			StartBasic();
		}
		CanInterupt = true;
	}

	private void CancelEX()
	{
		StopCoroutine(ex_cr());
		if (base.player.input.actions.GetButtonDown(3))
		{
			StartBasic();
		}
		CanInterupt = true;
	}

	public void StartSuper()
	{
		StartCoroutine(super_cr());
	}

	private IEnumerator super_cr()
	{
		state = State.Inactive;
		states.super = States.Super.Ready;
		CanInterupt = false;
		EndBasic();
		base.player.stats.OnSuper();
		AbstractPlaneSuper s = ((!base.player.stats.isChalice) ? super.Create(base.player) : chaliceSuper.Create(base.player));
		if (OnSuperStartEvent__BackingField != null)
		{
			OnSuperStartEvent__BackingField();
		}
		while (states.super != States.Super.Ending && states.super != States.Super.Countdown)
		{
			states.super = s.State;
			yield return null;
		}
		if (OnSuperCountdownEvent__BackingField != null)
		{
			OnSuperCountdownEvent__BackingField();
		}
		while (states.super != States.Super.Ending)
		{
			states.super = s.State;
			yield return null;
		}
		if (OnSuperFireEvent__BackingField != null)
		{
			OnSuperFireEvent__BackingField();
		}
		base.player.stats.OnSuperEnd();
		state = State.Ready;
		states.super = States.Super.Ready;
		if (base.player.input.actions.GetButtonDown(3))
		{
			StartBasic();
		}
		CanInterupt = true;
	}

	public Vector2 GetBulletPosition()
	{
		return bulletRoot.position;
	}

	private void HandleWeaponSwitch()
	{
		if (!base.player.input.actions.GetButtonDown(5) || (base.player.stats.Loadout.charm == Charm.charm_curse && base.player.stats.CurseCharmLevel >= 0) || (!PlayerData.Data.IsUnlocked(base.player.id, Weapon.plane_weapon_bomb) && !Level.IsTowerOfPower))
		{
			return;
		}
		if (base.player.stats.isChalice)
		{
			if (currentWeapon == Weapon.plane_chalice_weapon_3way)
			{
				SwitchWeapon(Weapon.plane_chalice_weapon_bomb);
			}
			else
			{
				SwitchWeapon(Weapon.plane_chalice_weapon_3way);
			}
		}
		else if (currentWeapon == Weapon.plane_weapon_peashot)
		{
			SwitchWeapon(Weapon.plane_weapon_bomb);
		}
		else
		{
			SwitchWeapon(Weapon.plane_weapon_peashot);
		}
	}

	private void SwitchWeapon(Weapon weapon)
	{
		EndBasic();
		currentWeapon = weapon;
		if (OnWeaponChangeEvent__BackingField != null)
		{
			OnWeaponChangeEvent__BackingField(weapon);
		}
		if (base.player.input.actions.GetButton(3))
		{
			StartBasic();
		}
	}
}
