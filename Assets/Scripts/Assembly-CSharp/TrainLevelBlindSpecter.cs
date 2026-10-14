using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public class TrainLevelBlindSpecter : LevelProperties.Train.Entity
{
	public delegate void OnDamageTakenHandler(float damage);

	[SerializeField]
	private Transform eyeRoot;

	[SerializeField]
	private TrainLevelBlindSpecterEyeProjectile eyePrefab;

	private SpriteRenderer spriteRenderer;

	private LevelProperties.Train.BlindSpecter blindSpecterProperties;

	private int shots;

	private DamageDealer damageDealer;

	private DamageReceiver damageReceiver;

	private float health;

	private bool dead;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private OnDamageTakenHandler OnDamageTakenEvent__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action OnDeathEvent__BackingField;

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

	protected override void Awake()
	{
		base.Awake();
		spriteRenderer = GetComponent<SpriteRenderer>();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		damageDealer = DamageDealer.NewEnemy();
		base.animator.enabled = false;
		spriteRenderer.enabled = false;
	}

	private void Start()
	{
		Level.Current.OnIntroEvent += OnIntro;
	}

	private void Update()
	{
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		if (damageDealer != null && phase != CollisionPhase.Exit)
		{
			damageDealer.DealDamage(hit);
		}
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		if (!dead)
		{
			if (OnDamageTakenEvent__BackingField != null)
			{
				OnDamageTakenEvent__BackingField(info.damage);
			}
			health -= info.damage;
			if (health <= 0f)
			{
				Die();
			}
		}
	}

	public override void LevelInit(LevelProperties.Train properties)
	{
		base.LevelInit(properties);
		health = properties.CurrentState.blindSpecter.health;
	}

	private void OnIntro()
	{
		StartCoroutine(loop_cr());
	}

	private void Die()
	{
		if (!dead)
		{
			GetComponent<LevelBossDeathExploder>().StartExplosion();
			dead = true;
			damageReceiver.enabled = false;
			StopAllCoroutines();
			base.animator.Play("Death");
			AudioManager.Play("train_blindspector_death");
			emitAudioFromObject.Add("train_blindspector_death");
		}
	}

	private void OnDeathAnimComplete()
	{
		if (OnDeathEvent__BackingField != null)
		{
			OnDeathEvent__BackingField();
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void SfxIntro()
	{
		AudioManager.Play("level_train_blindspecter_intro");
	}

	private void FireEyeball()
	{
		AudioManager.Play("train_blindspector_attack");
		emitAudioFromObject.Add("train_blindspector_attack");
		float value = UnityEngine.Random.value;
		Vector2 time = new Vector2(blindSpecterProperties.timeX.RandomFloat(), blindSpecterProperties.timeY.GetFloatAt(value));
		eyePrefab.Create(eyeRoot.position, time, blindSpecterProperties.heightMax.GetFloatAt(value), shots % 2 > 0, blindSpecterProperties.eyeHealth);
		shots++;
	}

	private IEnumerator loop_cr()
	{
		base.animator.enabled = true;
		spriteRenderer.enabled = true;
		base.animator.Play("Intro");
		blindSpecterProperties = base.properties.CurrentState.blindSpecter;
		yield return base.animator.WaitForAnimationToEnd(this, "Intro");
		yield return CupheadTime.WaitForSeconds(this, 2f);
		while (true)
		{
			shots = 0;
			base.animator.Play("Attack_Start");
			while (shots < blindSpecterProperties.attackLoops * 2)
			{
				yield return null;
			}
			base.animator.SetTrigger("Continue");
			yield return base.animator.WaitForAnimationToEnd(this, "Attack_End");
			yield return CupheadTime.WaitForSeconds(this, blindSpecterProperties.hesitate);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		eyePrefab = null;
	}
}
