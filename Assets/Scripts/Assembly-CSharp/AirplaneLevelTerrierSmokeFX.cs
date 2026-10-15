using UnityEngine;

public class AirplaneLevelTerrierSmokeFX : Effect
{
	public SpriteRenderer rend;

	public Vector3 vel;

	public bool dead;

	public Transform myTransform;

	public AirplaneLevelTerrierSmokeFX()
	{
		inUse = true;
	}

	public void Step(float t)
	{
		myTransform.position += vel * t;
	}

	protected override void OnEffectComplete()
	{
		if (dead)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			inUse = false;
		}
	}
}
