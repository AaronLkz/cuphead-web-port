using UnityEngine;

public class DevilLevelSplitDevil : LevelProperties.Devil.Entity
{
	public enum State
	{
		Idle = 0,
		Shoot = 1,
		summon = 2
	}

	public State state;

	[SerializeField]
	private Animator headsControler;

	public bool DevilLeft = true;

	public bool SplitDevilAnimationDone;

	[SerializeField]
	private DevilLevelSplitDevilProjectile projectilePrefab;

	private DevilLevelSplitDevilProjectile AngelprojectilePrefab;

	[SerializeField]
	private Transform projectileRootLeft;

	[SerializeField]
	private Transform projectileRootRight;

	private int patternIndex;

	private LevelProperties.Devil.Pattern pattern;

	protected override void Awake()
	{
		base.Awake();
		base.animator.Play("Idle");
		state = State.Idle;
	}

	private void LateUpdate()
	{
		LevelPlayerController levelPlayerController = PlayerManager.GetPlayer(PlayerId.PlayerOne) as LevelPlayerController;
		LevelPlayerController levelPlayerController2 = PlayerManager.GetPlayer(PlayerId.PlayerTwo) as LevelPlayerController;
		bool value = levelPlayerController == null || levelPlayerController.transform.localScale.x > 0f;
		headsControler.SetBool("LookRight", value);
		base.animator.SetBool("LookRight", value);
		headsControler.SetBool("DevilLeft", DevilLeft);
	}

	public void OnIdleLeftEnd()
	{
		bool flag = base.animator.GetBool("Shoot");
		bool flag2 = base.animator.GetBool("LookRight");
		string stateName = "DevilLeftShootTransition_2_3_4";
		string stateName2 = "DevilLeftIdleBody_2_3_4";
		if (flag)
		{
			if (flag2)
			{
				stateName = "DevilToAngel_Transition_2_3_4";
				stateName2 = "DevilToAngelIdleBody_2_3_4";
			}
			headsControler.enabled = true;
			headsControler.SetBool("Shoot", true);
			headsControler.Play(stateName, -1, 1f);
			base.animator.Play(stateName2, -1, 1f);
		}
	}

	public void OnIdleRightEnd()
	{
		bool flag = base.animator.GetBool("Shoot");
		bool flag2 = base.animator.GetBool("LookRight");
		string stateName = "DevilRightShootTransition_2_3_4";
		string stateName2 = "DevilRightIdleBody_2_3_4";
		if (flag)
		{
			if (!flag2)
			{
				stateName = "AngelToDevil_transition_2_3_4";
				stateName2 = "AngelToDevilIdleBody_2_3_4";
			}
			headsControler.enabled = true;
			headsControler.SetBool("Shoot", true);
			headsControler.Play(stateName, -1, 1f);
			base.animator.Play(stateName2, -1, 1f);
		}
	}

	public void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		base.properties.DealDamage(info.damage);
	}

	public void StartTransform()
	{
		base.animator.SetTrigger("IsDead");
	}

	public void OnDeadAnimationDone()
	{
		SplitDevilAnimationDone = true;
		base.gameObject.SetActive(false);
	}
}
