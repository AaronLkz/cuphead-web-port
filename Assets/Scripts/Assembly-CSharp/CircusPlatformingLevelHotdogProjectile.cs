using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public class CircusPlatformingLevelHotdogProjectile : BasicProjectile
{
	private const string KetchupState = "Ketchup";

	private const string MustardState = "Mustard";

	private const string RelishState = "Relish";

	private const string Ketchup = "K";

	private const string Mustard = "M";

	private const string Relish = "R";

	[SerializeField]
	private float scaleFactor;

	[SerializeField]
	private SpriteRenderer[] renderers;

	[SerializeField]
	private Effect spark;

	private Collider2D collider2d;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action<CircusPlatformingLevelHotdogProjectile> OnDestroyCallback__BackingField;

	protected override float DestroyLifetime
	{
		get
		{
			return 20f;
		}
	}

	public event Action<CircusPlatformingLevelHotdogProjectile> OnDestroyCallback
	{
		add
		{
			Action<CircusPlatformingLevelHotdogProjectile> action = OnDestroyCallback__BackingField;
			Action<CircusPlatformingLevelHotdogProjectile> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnDestroyCallback__BackingField, (Action<CircusPlatformingLevelHotdogProjectile>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<CircusPlatformingLevelHotdogProjectile> action = OnDestroyCallback__BackingField;
			Action<CircusPlatformingLevelHotdogProjectile> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnDestroyCallback__BackingField, (Action<CircusPlatformingLevelHotdogProjectile>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	protected override void Awake()
	{
		base.Awake();
		collider2d = GetComponent<Collider2D>();
	}

	protected override void Start()
	{
		base.Start();
		base.transform.localScale = new Vector3(0f, 1f, 1f);
		spark.Create(base.transform.position - new Vector3(10f, 0f, 0f));
		StartCoroutine(scaleOnStart_cr());
	}

	private IEnumerator scaleOnStart_cr()
	{
		while (base.transform.localScale.x < 1f)
		{
			base.transform.AddScale(scaleFactor * (float)CupheadTime.Delta);
			yield return null;
		}
		base.transform.SetScale(1f, 1f, 1f);
	}

	public void Side(bool isRight)
	{
		if (isRight)
		{
			for (int i = 0; i < renderers.Length; i++)
			{
				renderers[i].sortingOrder += 3;
			}
		}
	}

	public void SetCondiment(string type)
	{
		switch (type)
		{
		case "K":
			base.animator.Play("Ketchup");
			break;
		case "M":
			base.animator.Play("Mustard");
			break;
		case "R":
			base.animator.Play("Relish");
			break;
		}
	}

	public void EnableCollider(bool enable)
	{
		if (collider2d != null)
		{
			collider2d.enabled = enable;
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (OnDestroyCallback__BackingField != null)
		{
			OnDestroyCallback__BackingField(this);
		}
		spark = null;
	}
}
