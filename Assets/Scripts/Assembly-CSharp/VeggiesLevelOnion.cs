using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public class VeggiesLevelOnion : LevelProperties.Veggies.Entity
{
	public enum Side
	{
		Left = -1,
		Right = 1
	}

	public enum State
	{
		Idle = 0,
		Crying = 1,
		Complete = 2
	}

	public delegate void OnDamageTakenHandler(float damage);

	private const float START_SHOOTING_TIME = 0.6f;

	[SerializeField]
	private Transform leftRoot;

	[SerializeField]
	private Transform rightRoot;

	[SerializeField]
	private Transform radishRootRight;

	[SerializeField]
	private Transform radishRootLeft;

	[SerializeField]
	private VeggiesLevelOnionTearsStream tearStreamPrefab;

	[SerializeField]
	private VeggiesLevelOnionTearProjectile projectilePrefab;

	[SerializeField]
	private VeggiesLevelOnionTearProjectile pinkProjectilePrefab;

	[SerializeField]
	private VeggiesLevelOnionHomingHeart homingHeartPrefab;

	private new LevelProperties.Veggies.Onion properties;

	private CircleCollider2D circleCollider;

	private float hp;

	private DamageDealer damageDealer;

	private VeggiesLevelOnionTearsStream rightStream;

	private VeggiesLevelOnionTearsStream leftStream;

	private int currentCryLoop;

	private int targetCryLoops = 8;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private OnDamageTakenHandler OnDamageTakenEvent__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action OnHappyLeave__BackingField;

	private bool noSecret;

	private IEnumerator rightTearsCoroutine;

	private IEnumerator leftTearsCoroutine;

	public State state { get; private set; }

	public bool HappyLeave { get; private set; }

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

	public event Action OnHappyLeave
	{
		add
		{
			Action action = OnHappyLeave__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnHappyLeave__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnHappyLeave__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnHappyLeave__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	private void Start()
	{
		if (properties == null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		noSecret = true;
		circleCollider = GetComponent<CircleCollider2D>();
		state = State.Idle;
		StartCoroutine(happyTimer_cr());
		SfxGround();
	}

	private void SfxGround()
	{
		AudioManager.Play("level_veggies_onion_rise");
	}

	private void Update()
	{
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
	}

	public override void LevelInitWithGroup(AbstractLevelPropertyGroup propertyGroup)
	{
		base.LevelInitWithGroup(propertyGroup);
		properties = propertyGroup as LevelProperties.Veggies.Onion;
		hp = properties.hp;
		GetComponent<DamageReceiver>().OnDamageTaken += OnDamageTaken;
		damageDealer = new DamageDealer(1f, 0.2f, true, false, false);
		damageDealer.SetDirection(DamageDealer.Direction.Neutral, base.transform);
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		if (state == State.Idle)
		{
			state = State.Crying;
			noSecret = false;
			base.animator.SetTrigger("SadStart");
			return;
		}
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

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		if (phase != CollisionPhase.Exit)
		{
			damageDealer.DealDamage(hit);
		}
	}

	private void OnDeathAnimComplete()
	{
		state = State.Complete;
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void Die()
	{
		StopCrying();
		StopAllCoroutines();
		StartCoroutine(die_cr());
	}

	private void StartExplosions()
	{
		GetComponent<LevelBossDeathExploder>().StartExplosion();
	}

	private void StopExplosions()
	{
		GetComponent<LevelBossDeathExploder>().StopExplosions();
	}

	private void StartCrying()
	{
		AudioManager.Play("level_veggies_onion_crying");
		rightStream = tearStreamPrefab.Create(rightRoot.position, 1);
		leftStream = tearStreamPrefab.Create(leftRoot.position, -1);
		StartTearCoroutines();
	}

	private void StopCrying()
	{
		AudioManager.Stop("level_veggies_onion_crying");
		currentCryLoop = 0;
		targetCryLoops = properties.cryLoops.RandomInt();
		base.animator.SetBool("ContinueCrying", true);
		if (rightStream != null)
		{
			rightStream.End();
			rightStream = null;
		}
		if (leftStream != null)
		{
			leftStream.End();
			leftStream = null;
		}
		StopTearCoroutines();
	}

	private void CryLoop()
	{
		currentCryLoop++;
		if (currentCryLoop >= targetCryLoops)
		{
			base.animator.SetBool("ContinueCrying", false);
		}
	}

	private void BashfulAnimComplete()
	{
		AbstractPlayerController next = PlayerManager.GetNext();
		Vector2 pos;
		bool onLeft;
		if (next.transform.position.x > base.transform.position.x)
		{
			pos = radishRootRight.position;
			onLeft = false;
		}
		else
		{
			pos = radishRootLeft.position;
			onLeft = true;
		}
		homingHeartPrefab.CreateRadish(pos, properties.heartMaxSpeed, properties.heartAcceleration, properties.heartHP, onLeft);
		state = State.Complete;
	}

	private IEnumerator happyTimer_cr()
	{
		float t = 0f;
		while (t < properties.happyTime)
		{
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		if (noSecret)
		{
			if (OnHappyLeave__BackingField != null)
			{
				OnHappyLeave__BackingField();
			}
			if (state == State.Idle)
			{
				HappyLeave = true;
				base.animator.SetTrigger("HappyExit");
				StartCoroutine(handle_dirt_cr());
			}
		}
	}

	public void StopTearCoroutines()
	{
		if (rightTearsCoroutine != null)
		{
			StopCoroutine(rightTearsCoroutine);
		}
		if (leftTearsCoroutine != null)
		{
			StopCoroutine(leftTearsCoroutine);
		}
		rightTearsCoroutine = null;
		leftTearsCoroutine = null;
	}

	public void StartTearCoroutines()
	{
		StopTearCoroutines();
		string pattern = properties.tearPatterns.GetRandom().ToUpper();
		rightTearsCoroutine = tears_cr(Side.Right, pattern);
		leftTearsCoroutine = tears_cr(Side.Left, pattern);
		StartCoroutine(rightTearsCoroutine);
		StartCoroutine(leftTearsCoroutine);
	}

	private IEnumerator tears_cr(Side side, string pattern)
	{
		float tearDelay = UnityEngine.Random.Range(0.3f, 0.7f);
		yield return CupheadTime.WaitForSeconds(this, properties.tearAnticipate);
		string[] patterns = pattern.Split(',');
		int currentPattern = 0;
		int numUntilPink = properties.pinkTearRange.RandomInt();
		while (true)
		{
			if (patterns[currentPattern][0] == 'D')
			{
				float wait = 0f;
				if (Parser.FloatTryParse(patterns[currentPattern].Replace("D", string.Empty), out wait))
				{
					yield return CupheadTime.WaitForSeconds(this, wait);
				}
			}
			else
			{
				string[] destinations = patterns[currentPattern].Split('-');
				for (int i = 0; i < destinations.Length; i++)
				{
					float x = 0f;
					if (Parser.FloatTryParse(destinations[i], out x))
					{
						numUntilPink--;
						if (numUntilPink <= 0)
						{
							yield return CupheadTime.WaitForSeconds(this, tearDelay);
							numUntilPink = properties.pinkTearRange.RandomInt();
							pinkProjectilePrefab.Create(properties.tearTime, (side != Side.Right) ? (0f - x) : x);
						}
						else
						{
							yield return CupheadTime.WaitForSeconds(this, tearDelay);
							projectilePrefab.Create(properties.tearTime, (side != Side.Right) ? (0f - x) : x);
						}
						tearDelay = UnityEngine.Random.Range(0.3f, 0.7f);
					}
				}
			}
			currentPattern = (int)Mathf.Repeat(currentPattern + 1, patterns.Length);
			yield return CupheadTime.WaitForSeconds(this, properties.tearCommaDelay);
		}
	}

	private IEnumerator die_cr()
	{
		circleCollider.enabled = false;
		AudioManager.Play("level_veggies_onion_die");
		base.animator.Play("Sad_Die");
		StartExplosions();
		yield return CupheadTime.WaitForSeconds(this, 2f);
		StopExplosions();
		yield return null;
	}

	private IEnumerator handle_dirt_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 2f);
		base.animator.SetTrigger("FadeDirt");
	}

	private void OnionVoiceExisBashfulSFX()
	{
		AudioManager.Play("level_veggies_onion_exit_bashful");
		emitAudioFromObject.Add("level_veggies_onion_exit_bashful");
	}
}
