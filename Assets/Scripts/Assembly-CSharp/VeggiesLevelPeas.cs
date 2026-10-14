using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public class VeggiesLevelPeas : LevelProperties.Veggies.Entity
{
	public enum State
	{
		Start = 0,
		Complete = 1
	}

	public delegate void OnDamageTakenHandler(float damage);

	[SerializeField]
	private VeggiesLevelOnionTearProjectile projectilePrefab;

	private new LevelProperties.Veggies.Peas properties;

	private float hp;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private OnDamageTakenHandler OnDamageTakenEvent__BackingField;

	public State state { get; private set; }

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
	}

	public override void LevelInitWithGroup(AbstractLevelPropertyGroup propertyGroup)
	{
		base.LevelInitWithGroup(propertyGroup);
		properties = propertyGroup as LevelProperties.Veggies.Peas;
		hp = properties.hp;
		GetComponent<DamageReceiver>().OnDamageTaken += OnDamageTaken;
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
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

	private void OnInAnimComplete()
	{
		GetComponent<Collider2D>().enabled = true;
		StartCoroutine(peas_cr());
	}

	private void OnDeathAnimComplete()
	{
		state = State.Complete;
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void Die()
	{
		StopAllCoroutines();
		StartCoroutine(die_cr());
	}

	private IEnumerator peas_cr()
	{
		yield return null;
	}

	private IEnumerator die_cr()
	{
		GetComponent<Collider2D>().enabled = false;
		base.animator.SetTrigger("Idle");
		yield return StartCoroutine(dieFlash_cr());
		base.animator.SetTrigger("Dead");
	}
}
