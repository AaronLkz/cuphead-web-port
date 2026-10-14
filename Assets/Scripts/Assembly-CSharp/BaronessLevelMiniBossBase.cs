using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public class BaronessLevelMiniBossBase : AbstractCollidableObject
{
	public delegate void OnDamageTakenHandler(float damage);

	public bool isDying;

	public bool startInvisible;

	public int layerSwitch = 4;

	public BaronessLevelCastle.BossPossibility bossId;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private OnDamageTakenHandler OnDamageTakenEvent__BackingField;

	protected float fadeTime = 0.5f;

	protected Color endColor;

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

	protected virtual void Start()
	{
		endColor = GetComponent<SpriteRenderer>().color;
		GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f, 1f);
		base.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = SpriteLayer.Background.ToString();
		base.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 150;
		StartCoroutine(switchLayer_cr(layerSwitch));
	}

	protected virtual void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		if (OnDamageTakenEvent__BackingField != null)
		{
			OnDamageTakenEvent__BackingField(info.damage);
		}
	}

	protected virtual IEnumerator switchLayer_cr(int layerswitch)
	{
		StartCoroutine(fade_color_cr());
		yield return CupheadTime.WaitForSeconds(this, 3f);
		base.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = SpriteLayer.Enemies.ToString();
		base.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 260;
	}

	protected virtual IEnumerator fade_color_cr()
	{
		float t = 0f;
		Color start = new Color(0f, 0f, 0f, 1f);
		while (t < fadeTime)
		{
			GetComponent<SpriteRenderer>().color = Color.Lerp(start, endColor, t / fadeTime);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		GetComponent<SpriteRenderer>().color = endColor;
		yield return null;
	}

	protected virtual float hitPauseCoefficient()
	{
		return (!GetComponent<DamageReceiver>().IsHitPaused) ? 1f : 0f;
	}

	protected virtual void StartExplosions()
	{
		if (GetComponent<LevelBossDeathExploder>() != null)
		{
			GetComponent<LevelBossDeathExploder>().StartExplosion();
		}
	}

	protected virtual void EndExplosions()
	{
		if (GetComponent<LevelBossDeathExploder>() != null)
		{
			GetComponent<LevelBossDeathExploder>().StopExplosions();
		}
	}

	protected virtual void Die()
	{
		EndExplosions();
		StopAllCoroutines();
		if (GetComponent<Collider2D>() != null)
		{
			GetComponent<Collider2D>().enabled = false;
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
