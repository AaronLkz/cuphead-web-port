using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public class VeggiesLevelCarrot : LevelProperties.Veggies.Entity
{
	public enum Direction
	{
		Down = 0,
		DownLeft = 1,
		DownRight = 2
	}

	public enum State
	{
		Start = 0,
		Complete = 1
	}

	public delegate void OnAttackHandler(Direction direction);

	public delegate void OnDamageTakenHandler(float damage);

	[SerializeField]
	private AudioSource mindLoopPrefab;

	private AudioSource mindLoop;

	[SerializeField]
	private Transform homingRoot;

	[SerializeField]
	private Transform straightRoot;

	[SerializeField]
	private VeggiesLevelCarrotHomingProjectile homingPrefab;

	[SerializeField]
	private VeggiesLevelCarrotRegularProjectile straightPrefab;

	[SerializeField]
	private BasicProjectile ringPrefab;

	[SerializeField]
	private Effect ringEffectPrefab;

	[SerializeField]
	private VeggiesLevelCarrotBgCarrot bgPrefab;

	[SerializeField]
	private Effect spark;

	private LevelProperties.Veggies.Carrot carrot;

	private Transform[] homingRoots;

	private bool dead;

	private float hp;

	private IEnumerator floatingCoroutine;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action OnDeathEvent__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private OnDamageTakenHandler OnDamageTakenEvent__BackingField;

	public State state { get; private set; }

	public event Action OnDeathEvent
	{
		add
		{
			Action action = OnDeathEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnDeathEvent__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnDeathEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnDeathEvent__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public event OnDamageTakenHandler OnDamageTakenEvent
	{
		add
		{
			OnDamageTakenHandler onDamageTakenHandler = OnDamageTakenEvent__BackingField;
			OnDamageTakenHandler onDamageTakenHandler2;
			do
			{
				onDamageTakenHandler2 = onDamageTakenHandler;
				onDamageTakenHandler = Interlocked.CompareExchange(ref OnDamageTakenEvent__BackingField, (OnDamageTakenHandler)Delegate.Combine(onDamageTakenHandler2, value), onDamageTakenHandler);
			}
			while ((object)onDamageTakenHandler != onDamageTakenHandler2);
		}
		remove
		{
			OnDamageTakenHandler onDamageTakenHandler = OnDamageTakenEvent__BackingField;
			OnDamageTakenHandler onDamageTakenHandler2;
			do
			{
				onDamageTakenHandler2 = onDamageTakenHandler;
				onDamageTakenHandler = Interlocked.CompareExchange(ref OnDamageTakenEvent__BackingField, (OnDamageTakenHandler)Delegate.Remove(onDamageTakenHandler2, value), onDamageTakenHandler);
			}
			while ((object)onDamageTakenHandler != onDamageTakenHandler2);
		}
	}

	private void Start()
	{
		GetComponent<Collider2D>().enabled = false;
		mindLoop = UnityEngine.Object.Instantiate(mindLoopPrefab);
		List<Transform> list = new List<Transform>(homingRoot.GetComponentsInChildren<Transform>());
		list.Remove(homingRoot);
		homingRoots = list.ToArray();
		SfxGround();
	}

	private void Update()
	{
		if (PauseManager.state == PauseManager.State.Paused)
		{
			mindLoop.Pause();
		}
		else
		{
			mindLoop.UnPause();
		}
	}

	public override void OnLevelEnd()
	{
		base.OnLevelEnd();
		mindLoop.Stop();
	}

	public override void LevelInit(LevelProperties.Veggies properties)
	{
		base.LevelInit(properties);
		carrot = base.properties.CurrentState.carrot;
		hp = carrot.hp;
		GetComponent<DamageReceiver>().OnDamageTaken += OnDamageTaken;
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		if (!dead)
		{
			if (OnDamageTakenEvent__BackingField != null)
			{
				OnDamageTakenEvent__BackingField(info.damage);
			}
			hp -= info.damage;
			if (hp <= 0f)
			{
				Die();
			}
		}
	}

	private void OnInAnimComplete()
	{
		base.transform.GetComponent<Collider2D>().enabled = true;
		StartCoroutine(rings_cr());
	}

	private void Die()
	{
		dead = true;
		StopAllCoroutines();
		StartCoroutine(die_cr());
	}

	private void SfxGround()
	{
		AudioManager.Play("level_veggies_carrot_rise");
	}

	private void ShootRegular()
	{
		spark.Create(straightRoot.position);
		straightRoot.LookAt2D(PlayerManager.GetNext().center);
		straightPrefab.Create(this, straightRoot.position, carrot.bulletSpeed, straightRoot.eulerAngles.z);
	}

	public void ShootHoming()
	{
		homingPrefab.Create(PlayerManager.GetNext(), this, GetHomingRoot(), carrot.homingSpeed, carrot.homingRotation, carrot.homingHP);
	}

	private Vector2 GetHomingRoot()
	{
		Vector3 position = homingRoots[UnityEngine.Random.Range(0, homingRoots.Length)].position;
		homingRoot.SetScale(homingRoot.localScale.x * -1f);
		return position;
	}

	private IEnumerator rings_cr()
	{
		while (true)
		{
			StartCoroutine(carrot_cr());
			yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.carrot.idleRange.RandomFloat());
			int count = 0;
			base.animator.SetTrigger("AttackStart");
			yield return base.animator.WaitForAnimationToEnd(this, "Attack_Start");
			for (; count < carrot.bulletCount; count++)
			{
				yield return CupheadTime.WaitForSeconds(this, carrot.bulletDelay * 0.5f);
				ringEffectPrefab.Create(straightRoot.position);
				yield return CupheadTime.WaitForSeconds(this, carrot.bulletDelay * 0.5f);
				straightRoot.LookAt2D(PlayerManager.GetNext().center);
				for (int i = 0; i < 5; i++)
				{
					AudioManager.Play("level_veggies_carrot_beam");
					ringPrefab.Create(straightRoot.position, straightRoot.eulerAngles.z, carrot.bulletSpeed);
					yield return CupheadTime.WaitForSeconds(this, 0.1f);
				}
			}
			base.animator.SetTrigger("AttackEnd");
		}
	}

	private IEnumerator carrot_cr()
	{
		int bgCount = 0;
		LevelProperties.Veggies.Carrot p = base.properties.CurrentState.carrot;
		AudioManager.Play("level_veggies_mindmeld_start");
		mindLoop.Play();
		yield return CupheadTime.WaitForSeconds(this, carrot.startIdleTime);
		bool side = false;
		bgCount = 0;
		int numOfCarrots = p.homingNumOfCarrots.RandomInt();
		while (bgCount < numOfCarrots)
		{
			bgPrefab.Create(side ? 1 : (-1), carrot.homingBgSpeed, this);
			side = !side;
			bgCount++;
			yield return CupheadTime.WaitForSeconds(this, carrot.homingDelay);
		}
	}

	private IEnumerator die_cr()
	{
		mindLoop.Stop();
		AudioManager.Play("level_veggies_carrot_die");
		if (OnDeathEvent__BackingField != null)
		{
			OnDeathEvent__BackingField();
		}
		GetComponent<Collider2D>().enabled = false;
		base.animator.SetTrigger("Dead");
		yield return null;
		base.properties.WinInstantly();
	}
}
