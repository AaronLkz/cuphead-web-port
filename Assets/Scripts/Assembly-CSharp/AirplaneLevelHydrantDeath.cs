using System.Collections;
using UnityEngine;

public class AirplaneLevelHydrantDeath : MonoBehaviour
{
	[SerializeField]
	private Animator anim;

	[SerializeField]
	private GameObject pieces;

	private void Start()
	{
		if ((bool)pieces)
		{
			StartCoroutine(recede_cr());
		}
	}

	private IEnumerator recede_cr()
	{
		Vector3 startPos = base.transform.position;
		Vector3 endPos = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y - 100f, base.transform.position.z);
		endPos = Vector3.Lerp(startPos, endPos, 0.35f);
		while (true)
		{
			float t = anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
			pieces.transform.position = Vector3.Lerp(startPos, endPos, EaseUtils.EaseInSine(0f, 1f, t));
			yield return null;
		}
	}
}
