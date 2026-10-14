using UnityEngine;

public class DamageReceiverChild : AbstractMonoBehaviour
{
	[SerializeField]
	private DamageReceiver receiver;

	public DamageReceiver Receiver
	{
		get
		{
			return receiver;
		}
	}

	private void Start()
	{
		base.tag = receiver.tag;
	}
}
