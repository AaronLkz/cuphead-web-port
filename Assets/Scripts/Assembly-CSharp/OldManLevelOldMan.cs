using System.Collections;
using UnityEngine;

public class OldManLevelOldMan : LevelProperties.OldMan.Entity
{
	public enum State
	{
		Intro = 0,
		Idle = 1,
		Spit = 2,
		Goose = 3,
		Bear = 4
	}

	private static readonly int IsSpitAttackParameterID = Animator.StringToHash("IsSpitAttack");

	private static readonly int IsSpitAttackEyeLoopParameterID = Animator.StringToHash("IsSpitAttackEyeLoop");

	private static readonly int IsGooseAttackParameterID = Animator.StringToHash("IsGooseAttack");

	private static readonly int ContinueParameterID = Animator.StringToHash("Continue");

	private static readonly int IsBearAttackParameterID = Animator.StringToHash("IsBearAttack");

	private const float DUCK_MOVE_END = -165f;

	private const float BEAR_START_X = -1300f;

	private const float BEAR_Y = 100f;

	[SerializeField]
	private OldManLevelGoose goosePrefab;

	[SerializeField]
	private OldManLevelBear bearBeam;

	[SerializeField]
	private Transform spitRoot;

	[SerializeField]
	private Transform spitEndArc;

	[SerializeField]
	private OldManLevelSpitProjectile spitProjectile;

	[SerializeField]
	private OldManLevelSpitProjectile spitProjectilePink;

	[SerializeField]
	private OldManLevelPlatformManager platformManager;

	[SerializeField]
	private OldManLevelSockPuppetHandler sockPuppets;

	[SerializeField]
	private SpriteRenderer eyeRenderer;

	[SerializeField]
	private GameObject cauldron;

	[SerializeField]
	private GameObject cauldronEyes;

	[SerializeField]
	private Animator gooseFXAnimator;

	[SerializeField]
	private GameObject rightWall;

	private SpriteRenderer sprite;

	private DamageReceiver damageReceiver;

	private DamageDealer damageDealer;

	private int spitStringMainIndex;

	private bool shouldIdleBlink;

	private bool endPhaseOne;

	private PatternString gooseSpawnString;

	public State state { get; private set; }

	private void Start()
	{
		sprite = GetComponent<SpriteRenderer>();
		damageDealer = DamageDealer.NewEnemy();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
	}

	private void Update()
	{
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		base.properties.DealDamage(info.damage);
	}

	public override void LevelInit(LevelProperties.OldMan properties)
	{
		base.LevelInit(properties);
		LevelProperties.OldMan.SpitAttack spitAttack = properties.CurrentState.spitAttack;
		spitStringMainIndex = Random.Range(0, spitAttack.spitString.Length);
		gooseSpawnString = new PatternString(properties.CurrentState.gooseAttack.gooseSpawnString, ':');
		StartCoroutine(intro_cr());
	}

	private IEnumerator intro_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 3f);
		state = State.Idle;
	}

	private void AniEvent_CameraRumble()
	{
		CupheadLevelCamera.Current.Shake(5f, 0.66f);
	}

	private void AniEvent_CameraShake()
	{
		CupheadLevelCamera.Current.Shake(30f, 1.2f);
	}

	private void ChangeColor(Color color)
	{
		sprite.color = color;
	}

	public void Spit()
	{
		StartCoroutine(spit_cr());
	}

	private IEnumerator spit_cr()
	{
		AudioManager.FadeSFXVolume("sfx_dlc_omm_mouthcauldron_stirring_loop_start", 0f, 0.0001f);
		state = State.Spit;
		base.animator.SetBool(IsSpitAttackParameterID, true);
		LevelProperties.OldMan.SpitAttack p = base.properties.CurrentState.spitAttack;
		string[] spitString = p.spitString[spitStringMainIndex].Split(',');
		PatternString spitParryString = new PatternString(p.spitParryString);
		yield return base.animator.WaitForAnimationToEnd(this, "Spit_Intro_Continued");
		float height = p.spitApexHeight;
		float apexTime2 = p.spitApexTime * p.spitApexTime;
		float g = -2f * height / apexTime2;
		float viX = 2f * height / p.spitApexTime;
		float viY2 = viX * viX;
		float endPosX = spitEndArc.position.x;
		float endPosY = 0f;
		Vector3 startPosition = Vector3.zero;
		Vector3 endPosition = Vector3.zero;
		for (int i = 0; i < spitString.Length; i++)
		{
			if (endPhaseOne)
			{
				break;
			}
			Parser.FloatTryParse(spitString[i], out endPosY);
			startPosition = spitRoot.transform.position + Vector3.right * Random.Range(-15, 15);
			endPosition = new Vector3(endPosX, endPosY);
			float x = endPosition.x - startPosition.x;
			float y = endPosition.y - startPosition.y;
			float sqrtRooted = viY2 + 2f * g * x;
			float tEnd1 = (0f - viX + Mathf.Sqrt(sqrtRooted)) / g;
			float tEnd2 = (0f - viX - Mathf.Sqrt(sqrtRooted)) / g;
			float tEnd3 = Mathf.Max(tEnd1, tEnd2);
			float velocityY = y / tEnd3;
			OldManLevelSpitProjectile projectile = ((spitParryString.PopLetter() != 'P') ? (spitProjectile.Create() as OldManLevelSpitProjectile) : (spitProjectilePink.Create() as OldManLevelSpitProjectile));
			projectile.Move(startPosition, viX, velocityY, spitEndArc.position.x, g, p.spitApexTime, i % 4);
			if (!endPhaseOne)
			{
				yield return CupheadTime.WaitForSeconds(this, p.spitDelay);
			}
		}
		base.animator.SetBool(IsSpitAttackParameterID, false);
		spitStringMainIndex = (spitStringMainIndex + 1) % p.spitString.Length;
		yield return base.animator.WaitForAnimationToEnd(this, "Spit_Outro");
		float t = 0f;
		while (t < p.attackCooldown && !endPhaseOne)
		{
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		state = State.Idle;
		yield return null;
	}

	public void Goose()
	{
		StartCoroutine(Goose_cr());
	}

	private IEnumerator Goose_cr()
	{
		LevelProperties.OldMan.GooseAttack p = base.properties.CurrentState.gooseAttack;
		state = State.Goose;
		YieldInstruction wait = new WaitForFixedUpdate();
		base.animator.SetBool(IsGooseAttackParameterID, true);
		int targetAnimHash = Animator.StringToHash(base.animator.GetLayerName(0) + ".Goose_Atk_Anti");
		int idleOneAnimHash = Animator.StringToHash(base.animator.GetLayerName(0) + ".Idle_Part_1");
		int idleTwoAnimHash = Animator.StringToHash(base.animator.GetLayerName(0) + ".Idle_Part_2");
		while (base.animator.GetCurrentAnimatorStateInfo(0).fullPathHash != targetAnimHash && !endPhaseOne)
		{
			yield return null;
		}
		if (endPhaseOne && (base.animator.GetCurrentAnimatorStateInfo(0).fullPathHash == idleOneAnimHash || base.animator.GetCurrentAnimatorStateInfo(0).fullPathHash == idleTwoAnimHash))
		{
			base.animator.SetBool(IsGooseAttackParameterID, false);
			yield break;
		}
		float t = 0f;
		while (t < p.goosePreAntic && !endPhaseOne)
		{
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		base.animator.SetTrigger(ContinueParameterID);
		t = 0f;
		while (t < p.gooseWarning && !endPhaseOne)
		{
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		bool spawningGeese = true;
		float geeseDurationTimer = 0f;
		float geeseDelayTimer = 0f;
		float geeseDelayMaxTime = gooseSpawnString.GetSubsubstringFloat(0);
		float xPos = 840f;
		float speed = 0f;
		while (spawningGeese && !endPhaseOne)
		{
			geeseDurationTimer += CupheadTime.FixedDelta;
			geeseDelayTimer += CupheadTime.FixedDelta;
			if (geeseDelayTimer >= geeseDelayMaxTime)
			{
				OldManLevelGoose oldManLevelGoose = goosePrefab.Create() as OldManLevelGoose;
				bool hasCollision = false;
				string sortingLayer = "Default";
				int sortingOrder = 100;
				float whiten = 0f;
				switch (gooseSpawnString.GetSubsubstringLetter(1))
				{
				case 'B':
					oldManLevelGoose.transform.localScale = new Vector3(0.655f, 0.655f);
					speed = p.gooseBSpeed;
					sortingLayer = "Background";
					sortingOrder = 1000;
					whiten = 0.15f;
					break;
				case 'M':
					oldManLevelGoose.transform.localScale = new Vector3(0.848f, 0.848f);
					speed = p.gooseMSpeed;
					hasCollision = true;
					sortingOrder = -100;
					whiten = 0.05f;
					break;
				case 'C':
					speed = p.gooseCSpeed;
					hasCollision = true;
					break;
				case 'F':
					oldManLevelGoose.transform.localScale = new Vector3(1.4414f, 1.4414f);
					speed = p.gooseFSpeed;
					sortingLayer = "Foreground";
					whiten = 0.85f;
					break;
				}
				Vector3 vector = new Vector3(xPos, gooseSpawnString.GetSubsubstringFloat(2));
				oldManLevelGoose.Init(vector, speed, p, hasCollision, sortingLayer, sortingOrder, whiten);
				geeseDelayTimer = 0f;
				geeseDelayMaxTime = gooseSpawnString.GetSubsubstringFloat(0);
				gooseSpawnString.IncrementString();
			}
			if (geeseDurationTimer >= p.gooseDuration)
			{
				spawningGeese = false;
			}
			yield return wait;
		}
		base.animator.SetBool(IsGooseAttackParameterID, false);
		yield return base.animator.WaitForAnimationToEnd(this, "Goose_Atk_End");
		t = 0f;
		while (t < p.gooseCooldown && !endPhaseOne)
		{
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		state = State.Idle;
		yield return null;
	}

	public void Bear()
	{
		StartCoroutine(bear_cr());
	}

	private IEnumerator bear_cr()
	{
		LevelProperties.OldMan.CamelAttack p = base.properties.CurrentState.camelAttack;
		state = State.Bear;
		base.animator.SetBool(IsBearAttackParameterID, true);
		int targetAnimHash = Animator.StringToHash(base.animator.GetLayerName(0) + ".Bear_Atk_Start");
		int targetAnimHashAlt = Animator.StringToHash(base.animator.GetLayerName(0) + ".Bear_Atk_Start_F10");
		int idleOneAnimHash = Animator.StringToHash(base.animator.GetLayerName(0) + ".Idle_Part_1");
		int idleTwoAnimHash = Animator.StringToHash(base.animator.GetLayerName(0) + ".Idle_Part_2");
		while (base.animator.GetCurrentAnimatorStateInfo(0).fullPathHash != targetAnimHash && base.animator.GetCurrentAnimatorStateInfo(0).fullPathHash != targetAnimHashAlt && !endPhaseOne)
		{
			yield return null;
		}
		if (endPhaseOne && (base.animator.GetCurrentAnimatorStateInfo(0).fullPathHash == idleOneAnimHash || base.animator.GetCurrentAnimatorStateInfo(0).fullPathHash == idleTwoAnimHash))
		{
			base.animator.SetBool(IsBearAttackParameterID, false);
			yield break;
		}
		yield return base.animator.WaitForAnimationToStart(this, "Bear_Atk_Anti");
		Animator bearAni = bearBeam.GetComponent<Animator>();
		bearBeam.gameObject.SetActive(true);
		bearBeam.transform.position = new Vector3(-1300f, 100f);
		bearBeam.thrown = false;
		float t = 0f;
		while (t < p.camelAttackWarning && !endPhaseOne)
		{
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		base.animator.SetTrigger(ContinueParameterID);
		t = 0f;
		yield return base.animator.WaitForNormalizedTime(this, 1.2916666f, "Bear_Atk_Cont");
		yield return CupheadTime.WaitForSeconds(this, (!endPhaseOne) ? p.camelOffScreenTime : 0.5f);
		float endPoint = ((!endPhaseOne) ? p.endingPoint : (-990f));
		bool exiting = false;
		YieldInstruction wait = new WaitForFixedUpdate();
		bearAni.Play("Idle");
		while (bearBeam.transform.position.x < endPoint && !bearBeam.thrown)
		{
			if ((endPhaseOne || bearBeam.transform.position.x > p.boredomPoint) && !exiting)
			{
				exiting = true;
				StartCoroutine(exit_bear_cr());
			}
			bearBeam.transform.position += Vector3.right * p.camelAttackSpeed * CupheadTime.FixedDelta;
			yield return wait;
		}
		if (!exiting)
		{
			yield return StartCoroutine(exit_bear_cr());
		}
		t = 0f;
		while (t < p.camelAttackCooldown && !endPhaseOne)
		{
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		state = State.Idle;
		yield return null;
	}

	private IEnumerator exit_bear_cr()
	{
		Animator bearAni = bearBeam.GetComponent<Animator>();
		base.animator.SetBool(IsBearAttackParameterID, false);
		yield return base.animator.WaitForAnimationToStart(this, "Bear_Atk_End_1");
		bearAni.SetTrigger("OnExit");
		yield return bearAni.WaitForAnimationToEnd(this, "End");
		bearBeam.StartCoroutine(bearBeam.fall_cr());
		base.animator.SetTrigger(ContinueParameterID);
		yield return base.animator.WaitForAnimationToEnd(this, "Bear_Atk_End", false, false);
	}

	public void EndPhase1()
	{
		base.animator.SetBool("Phase2", true);
		endPhaseOne = true;
	}

	private void AniEvent_EndPhase1BeardBoil()
	{
		rightWall.SetActive(false);
		base.animator.Play("None", 1);
	}

	private void AniEvent_ActivatePhase2Beard()
	{
		((OldManLevel)Level.Current).ActivatePhase2Beard();
		GetComponent<SpriteRenderer>().sortingOrder = -1;
	}

	public void OnPhase2()
	{
		sockPuppets.StartPhase2();
		Object.Destroy(base.gameObject);
	}

	private void animationEvent_PlayGooseFX()
	{
		gooseFXAnimator.Play("FX");
	}

	private void animationEvent_IdleBlinkStart()
	{
		if (shouldIdleBlink)
		{
			eyeRenderer.enabled = true;
		}
		shouldIdleBlink = !shouldIdleBlink;
	}

	private void animationEvent_IdleBlinkEnd()
	{
		eyeRenderer.enabled = false;
	}

	private void animationEvent_BeginCauldron()
	{
		cauldron.SetActive(true);
	}

	private void animationEvent_EndCauldron()
	{
		cauldron.SetActive(false);
	}

	private void animationEvent_BeginSpitEyes()
	{
		cauldronEyes.SetActive(true);
	}

	private void animationEvent_EndSpitEyes()
	{
		cauldronEyes.SetActive(false);
	}

	private void AnimationEvent_SFX_OMM_Intro()
	{
		AudioManager.Play("sfx_dlc_omm_intro");
		emitAudioFromObject.Add("sfx_dlc_omm_intro");
	}

	private void AnimationEvent_SFX_OMM_IntroPickaxe()
	{
		AudioManager.Play("sfx_dlc_omm_intropickaxe");
		emitAudioFromObject.Add("sfx_dlc_omm_intropickaxe");
	}

	private void AnimationEvent_SFX_OMM_GooseStormIntro()
	{
		AudioManager.Play("sfx_dlc_omm_goosestorm");
		emitAudioFromObject.Add("sfx_dlc_omm_goosestorm");
	}

	private void AnimationEvent_SFX_OMM_GooseStormLoop()
	{
		StartCoroutine(SFX_OMM_GooseStormLoop_cr());
	}

	private IEnumerator SFX_OMM_GooseStormLoop_cr()
	{
		yield return new WaitForEndOfFrame();
		AudioManager.PlayLoop("sfx_dlc_omm_goosestorm_loop");
		emitAudioFromObject.Add("sfx_dlc_omm_goosestorm_loop");
	}

	private void AnimationEvent_SFX_OMM_GooseStormLoopEnd()
	{
		AudioManager.Stop("sfx_dlc_omm_goosestorm_loop");
		AudioManager.Play("sfx_dlc_omm_goosestorm_loop_end");
		emitAudioFromObject.Add("sfx_dlc_omm_goosestorm_loop_end");
	}

	private void AnimationEvent_SFX_OMM_MouthCauldron_MouthClose()
	{
		AudioManager.Play("sfx_dlc_omm_mouthcauldron_mouthclose");
		emitAudioFromObject.Add("sfx_dlc_omm_mouthcauldron_mouthclose");
		AudioManager.Stop("sfx_dlc_omm_mouthcauldron_stirring_loop");
	}

	private void AnimationEvent_SFX_OMM_MouthCauldron_MouthOpen()
	{
		AudioManager.Play("sfx_dlc_omm_mouthcauldron_mouthopen");
		emitAudioFromObject.Add("sfx_dlc_omm_mouthcauldron_mouthopen");
		AudioManager.FadeSFXVolume("sfx_dlc_omm_mouthcauldron_stirring_loop_start", 1f, 0.1f);
		AudioManager.Play("sfx_dlc_omm_mouthcauldron_stirring_loop_start");
		emitAudioFromObject.Add("sfx_dlc_omm_mouthcauldron_stirring_loop_start");
		AudioManager.PlayLoop("sfx_dlc_omm_mouthcauldron_stirring_loop");
		emitAudioFromObject.Add("sfx_dlc_omm_mouthcauldron_stirring_loop");
	}

	private void AnimationEvent_SFX_OMM_BearAttackOMMStartVocal()
	{
		AudioManager.Play("sfx_dlc_omm_bearattack_ommstartvocal");
		emitAudioFromObject.Add("sfx_dlc_omm_bearattack_ommstartvocal");
	}

	private void AnimationEvent_SFX_OMM_BearAttackStart()
	{
		AudioManager.Play("sfx_dlc_omm_bearattack_start");
		emitAudioFromObject.Add("sfx_dlc_omm_bearattack_start");
	}

	private void AnimationEvent_SFX_OMM_P2_OMMVocalFrustrated()
	{
		AudioManager.Play("sfx_dlc_omm_p2_end_ommvocalfrustrated");
		emitAudioFromObject.Add("sfx_dlc_omm_p2_end_ommvocalfrustrated");
	}

	private void AnimationEvent_SFX_OMM_P2_TransitionBeardPull()
	{
		AudioManager.Play("sfx_dlc_omm_p2_transition_pullbeardoff");
		emitAudioFromObject.Add("sfx_dlc_omm_p2_transition_pullbeardoff");
	}
}
