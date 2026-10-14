using UnityEngine;

public class GenericTextHandler : AbstractPausableComponent
{
	[SerializeField]
	private GameObject textChosen;

	[SerializeField]
	private GameObject[] otherText;

	private void Start()
	{
		GameObject[] array = otherText;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(false);
		}
	}

	private void ShowText()
	{
		textChosen.SetActive(true);
	}
}
