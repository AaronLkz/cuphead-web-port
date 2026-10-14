using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public class TrainLevelLollipopGhoulsManager : LevelProperties.Train.Entity
{
	public delegate void OnDamageTakenHandler(float damage);

	[SerializeField]
	private TrainLevelLollipopGhoul ghoulLeft;

	[SerializeField]
	private TrainLevelLollipopGhoul ghoulRight;

	[Space(10f)]
	[SerializeField]
	private TrainLevelGhostCannons cannons;

	[Space(10f)]
	[SerializeField]
	private TrainLevelPassengerCar[] cars;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private OnDamageTakenHandler OnDamageTakenEvent__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action OnDeathEvent__BackingField;

	private int deadCount;

	private int current;

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

	public void Setup()
	{
		cars[1].Explode(2);
	}

	public override void LevelInit(LevelProperties.Train properties)
	{
		base.LevelInit(properties);
		ghoulLeft.LevelInit(properties);
		ghoulRight.LevelInit(properties);
		cannons.LevelInit(properties);
		ghoulLeft.OnDamageTakenEvent += OnDamageTaken;
		ghoulLeft.OnDeathEvent += OnDeath;
		ghoulRight.OnDamageTakenEvent += OnDamageTaken;
		ghoulRight.OnDeathEvent += OnDeath;
	}

	private void OnDeath()
	{
		deadCount++;
		if (deadCount > 1)
		{
			EndGhouls();
		}
	}

	private void OnDamageTaken(float damage)
	{
		if (OnDamageTakenEvent__BackingField != null)
		{
			OnDamageTakenEvent__BackingField(damage);
		}
	}

	private IEnumerator start_cr()
	{
		AudioManager.Play("level_train_top_explode");
		cars[0].Explode(0);
		cars[2].Explode(1);
		yield return null;
		ghoulLeft.AnimateIn();
		ghoulRight.AnimateIn();
		AudioManager.Play("train_lollipop_ghoul_intro");
		emitAudioFromObject.Add("train_lollipop_ghoul_intro");
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.lollipopGhouls.initDelay);
		StartCoroutine(ghouls_cr());
		StartCoroutine(cannons_cr());
	}

	public void StartGhouls()
	{
		StartCoroutine(start_cr());
	}

	private void EndGhouls()
	{
		StopAllCoroutines();
		cannons.End();
		if (OnDeathEvent__BackingField != null)
		{
			OnDeathEvent__BackingField();
		}
	}

	private TrainLevelLollipopGhoul NextGhoul()
	{
		if (deadCount > 1)
		{
			return null;
		}
		if (ghoulRight.state == TrainLevelLollipopGhoul.State.Dead || ghoulRight.transform == null)
		{
			return ghoulLeft;
		}
		if (ghoulLeft.state == TrainLevelLollipopGhoul.State.Dead || ghoulLeft.transform == null)
		{
			return ghoulRight;
		}
		current = (int)Mathf.Repeat(current + 1, 2f);
		int num = current;
		if (num == 0 || num != 1)
		{
			return ghoulLeft;
		}
		return ghoulRight;
	}

	private IEnumerator ghouls_cr()
	{
		current = UnityEngine.Random.Range(0, 2);
		while (true)
		{
			TrainLevelLollipopGhoul ghoul = NextGhoul();
			yield return null;
			if (ghoul != null)
			{
				ghoul.Attack();
				while (ghoul.state == TrainLevelLollipopGhoul.State.Attacking)
				{
					yield return null;
				}
				yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.lollipopGhouls.mainDelay);
			}
		}
	}

	private IEnumerator cannons_cr()
	{
		int cannon = UnityEngine.Random.Range(0, 3);
		int direction = 1;
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.lollipopGhouls.cannonDelay);
			cannons.Shoot(cannon);
			yield return CupheadTime.WaitForSeconds(this, 1f);
			cannon += direction;
			if (cannon >= 3)
			{
				direction = -1;
				cannon = 1;
			}
			else if (cannon < 0)
			{
				cannon = 1;
				direction = 1;
			}
		}
	}
}
