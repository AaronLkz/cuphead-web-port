using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using GCFreeUtils;
using UnityEngine;
using UnityEngine.Audio;

public static class AudioManager
{
	public enum Channel
	{
		Default = 0,
		Level = 1
	}

	public enum Property
	{
		MasterVolume = 0,
		Options_BGMVolume = 1,
		Options_SFXVolume = 2
	}

	public enum Snapshots
	{
		Cutscene = 0,
		FrontEnd = 1,
		Unpaused = 2,
		Unpaused_Clean = 3,
		Unpaused_1920s = 4,
		Loadscreen = 5,
		Paused = 6,
		Super = 7,
		SuperStart = 8,
		Death = 9,
		EquipMenu = 10,
		RumRunners_RedBeam = 11,
		RumRunners_GreenBeam = 12,
		RumRunners_YellowBeam = 13
	}

	public delegate bool OnCheckIfPlaying(string key);

	public delegate void OnSfxHandler(string key);

	public delegate void OnTransformHandler(string key, Transform transform);

	public delegate void OnAttenuationHandler(string key, bool attenuation, float endVolume);

	public delegate void OnChangeBGMHandler(float end, float time);

	public delegate void OnChangeBGMVolumeHandler(float end, float time, bool fadeOut);

	public delegate void OnStartBGMAlternateHandler(int index);

	public delegate void OnChangeSFXHandler(string key, float end, float time);

	public delegate void OnChangeSFXStartEndHandler(string key, float start, float end, float time);

	public delegate void OnWarbleBGMPitchHandler(int warbles, float[] minValue, float[] maxValue, float[] warbleTime, float[] playTime);

	public delegate void OnSnapshotHandler(string[] names, float[] weight, float time);

	public delegate void OnPanEventHandler(string key, float value);

	public delegate void OnBGMPlayListManualHandler(bool loopPlayListAfter);

	private const float VOLUME_MAX = 0f;

	private const float VOLUME_MIN = -80f;

	public static GCFreePredicateList<string> OnCheckEvent = new GCFreePredicateList<string>(10, true);

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static OnAttenuationHandler OnAttenuation__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static OnTransformHandler OnFollowObject__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static OnPanEventHandler OnPanEvent__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static OnSnapshotHandler OnSnapshotEvent__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static OnChangeBGMHandler OnBGMSlowdown__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static OnChangeBGMVolumeHandler OnBGMFadeVolume__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static OnStartBGMAlternateHandler OnStartBGMAlternate__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static OnChangeSFXHandler OnSFXSlowDown__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static OnChangeSFXStartEndHandler OnSFXFadeVolumeLinear__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static OnChangeSFXStartEndHandler OnSFXFadeVolume__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static OnWarbleBGMPitchHandler OnBGMPitchWarble__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static OnBGMPlayListManualHandler OnPlayManualBGM__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static OnSfxHandler OnPlayEvent__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static OnSfxHandler OnPlayLoopEvent__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static OnSfxHandler OnStopEvent__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static OnSfxHandler OnPauseEvent__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static OnSfxHandler OnUnpauseEvent__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action OnStopAllEvent__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action OnStopBGMEvent__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action OnPlayBGMEvent__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action OnPlayBGMPlaylistEvent__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action OnPauseAllSFXEvent__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action OnUnpauseAllSFXEvent__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action OnStopManualBGMTrackEvent__BackingField;

	private static AudioMixer _mixer;

	private static bool checkIfPlaying;

	private static AudioMixer mixer
	{
		get
		{
			if (_mixer == null)
			{
				_mixer = AudioManagerMixer.GetMixer();
			}
			return _mixer;
		}
	}

	public static float sfxOptionsVolume
	{
		get
		{
			float value;
			mixer.GetFloat(Property.Options_SFXVolume.ToString(), out value);
			return value;
		}
		set
		{
			mixer.SetFloat(Property.Options_SFXVolume.ToString(), value);
		}
	}

	public static float bgmOptionsVolume
	{
		get
		{
			float value;
			mixer.GetFloat(Property.Options_BGMVolume.ToString(), out value);
			return value;
		}
		set
		{
			mixer.SetFloat(Property.Options_BGMVolume.ToString(), value);
		}
	}

	public static float masterVolume
	{
		get
		{
			float value;
			mixer.GetFloat(Property.MasterVolume.ToString(), out value);
			return value;
		}
		set
		{
			mixer.SetFloat(Property.MasterVolume.ToString(), value);
		}
	}

	public static event OnAttenuationHandler OnAttenuation
	{
		add
		{
			OnAttenuationHandler onAttenuationHandler = OnAttenuation__BackingField;
			OnAttenuationHandler onAttenuationHandler2;
			do
			{
				onAttenuationHandler2 = onAttenuationHandler;
				onAttenuationHandler = Interlocked.CompareExchange(ref OnAttenuation__BackingField, (OnAttenuationHandler)Delegate.Combine(onAttenuationHandler2, value), onAttenuationHandler);
			}
			while ((object)onAttenuationHandler != onAttenuationHandler2);
		}
		remove
		{
			OnAttenuationHandler onAttenuationHandler = OnAttenuation__BackingField;
			OnAttenuationHandler onAttenuationHandler2;
			do
			{
				onAttenuationHandler2 = onAttenuationHandler;
				onAttenuationHandler = Interlocked.CompareExchange(ref OnAttenuation__BackingField, (OnAttenuationHandler)Delegate.Remove(onAttenuationHandler2, value), onAttenuationHandler);
			}
			while ((object)onAttenuationHandler != onAttenuationHandler2);
		}
	}

	public static event OnTransformHandler OnFollowObject
	{
		add
		{
			OnTransformHandler onTransformHandler = OnFollowObject__BackingField;
			OnTransformHandler onTransformHandler2;
			do
			{
				onTransformHandler2 = onTransformHandler;
				onTransformHandler = Interlocked.CompareExchange(ref OnFollowObject__BackingField, (OnTransformHandler)Delegate.Combine(onTransformHandler2, value), onTransformHandler);
			}
			while ((object)onTransformHandler != onTransformHandler2);
		}
		remove
		{
			OnTransformHandler onTransformHandler = OnFollowObject__BackingField;
			OnTransformHandler onTransformHandler2;
			do
			{
				onTransformHandler2 = onTransformHandler;
				onTransformHandler = Interlocked.CompareExchange(ref OnFollowObject__BackingField, (OnTransformHandler)Delegate.Remove(onTransformHandler2, value), onTransformHandler);
			}
			while ((object)onTransformHandler != onTransformHandler2);
		}
	}

	public static event OnPanEventHandler OnPanEvent
	{
		add
		{
			OnPanEventHandler onPanEventHandler = OnPanEvent__BackingField;
			OnPanEventHandler onPanEventHandler2;
			do
			{
				onPanEventHandler2 = onPanEventHandler;
				onPanEventHandler = Interlocked.CompareExchange(ref OnPanEvent__BackingField, (OnPanEventHandler)Delegate.Combine(onPanEventHandler2, value), onPanEventHandler);
			}
			while ((object)onPanEventHandler != onPanEventHandler2);
		}
		remove
		{
			OnPanEventHandler onPanEventHandler = OnPanEvent__BackingField;
			OnPanEventHandler onPanEventHandler2;
			do
			{
				onPanEventHandler2 = onPanEventHandler;
				onPanEventHandler = Interlocked.CompareExchange(ref OnPanEvent__BackingField, (OnPanEventHandler)Delegate.Remove(onPanEventHandler2, value), onPanEventHandler);
			}
			while ((object)onPanEventHandler != onPanEventHandler2);
		}
	}

	public static event OnSnapshotHandler OnSnapshotEvent
	{
		add
		{
			OnSnapshotHandler onSnapshotHandler = OnSnapshotEvent__BackingField;
			OnSnapshotHandler onSnapshotHandler2;
			do
			{
				onSnapshotHandler2 = onSnapshotHandler;
				onSnapshotHandler = Interlocked.CompareExchange(ref OnSnapshotEvent__BackingField, (OnSnapshotHandler)Delegate.Combine(onSnapshotHandler2, value), onSnapshotHandler);
			}
			while ((object)onSnapshotHandler != onSnapshotHandler2);
		}
		remove
		{
			OnSnapshotHandler onSnapshotHandler = OnSnapshotEvent__BackingField;
			OnSnapshotHandler onSnapshotHandler2;
			do
			{
				onSnapshotHandler2 = onSnapshotHandler;
				onSnapshotHandler = Interlocked.CompareExchange(ref OnSnapshotEvent__BackingField, (OnSnapshotHandler)Delegate.Remove(onSnapshotHandler2, value), onSnapshotHandler);
			}
			while ((object)onSnapshotHandler != onSnapshotHandler2);
		}
	}

	public static event OnChangeBGMHandler OnBGMSlowdown
	{
		add
		{
			OnChangeBGMHandler onChangeBGMHandler = OnBGMSlowdown__BackingField;
			OnChangeBGMHandler onChangeBGMHandler2;
			do
			{
				onChangeBGMHandler2 = onChangeBGMHandler;
				onChangeBGMHandler = Interlocked.CompareExchange(ref OnBGMSlowdown__BackingField, (OnChangeBGMHandler)Delegate.Combine(onChangeBGMHandler2, value), onChangeBGMHandler);
			}
			while ((object)onChangeBGMHandler != onChangeBGMHandler2);
		}
		remove
		{
			OnChangeBGMHandler onChangeBGMHandler = OnBGMSlowdown__BackingField;
			OnChangeBGMHandler onChangeBGMHandler2;
			do
			{
				onChangeBGMHandler2 = onChangeBGMHandler;
				onChangeBGMHandler = Interlocked.CompareExchange(ref OnBGMSlowdown__BackingField, (OnChangeBGMHandler)Delegate.Remove(onChangeBGMHandler2, value), onChangeBGMHandler);
			}
			while ((object)onChangeBGMHandler != onChangeBGMHandler2);
		}
	}

	public static event OnChangeBGMVolumeHandler OnBGMFadeVolume
	{
		add
		{
			OnChangeBGMVolumeHandler onChangeBGMVolumeHandler = OnBGMFadeVolume__BackingField;
			OnChangeBGMVolumeHandler onChangeBGMVolumeHandler2;
			do
			{
				onChangeBGMVolumeHandler2 = onChangeBGMVolumeHandler;
				onChangeBGMVolumeHandler = Interlocked.CompareExchange(ref OnBGMFadeVolume__BackingField, (OnChangeBGMVolumeHandler)Delegate.Combine(onChangeBGMVolumeHandler2, value), onChangeBGMVolumeHandler);
			}
			while ((object)onChangeBGMVolumeHandler != onChangeBGMVolumeHandler2);
		}
		remove
		{
			OnChangeBGMVolumeHandler onChangeBGMVolumeHandler = OnBGMFadeVolume__BackingField;
			OnChangeBGMVolumeHandler onChangeBGMVolumeHandler2;
			do
			{
				onChangeBGMVolumeHandler2 = onChangeBGMVolumeHandler;
				onChangeBGMVolumeHandler = Interlocked.CompareExchange(ref OnBGMFadeVolume__BackingField, (OnChangeBGMVolumeHandler)Delegate.Remove(onChangeBGMVolumeHandler2, value), onChangeBGMVolumeHandler);
			}
			while ((object)onChangeBGMVolumeHandler != onChangeBGMVolumeHandler2);
		}
	}

	public static event OnStartBGMAlternateHandler OnStartBGMAlternate
	{
		add
		{
			OnStartBGMAlternateHandler onStartBGMAlternateHandler = OnStartBGMAlternate__BackingField;
			OnStartBGMAlternateHandler onStartBGMAlternateHandler2;
			do
			{
				onStartBGMAlternateHandler2 = onStartBGMAlternateHandler;
				onStartBGMAlternateHandler = Interlocked.CompareExchange(ref OnStartBGMAlternate__BackingField, (OnStartBGMAlternateHandler)Delegate.Combine(onStartBGMAlternateHandler2, value), onStartBGMAlternateHandler);
			}
			while ((object)onStartBGMAlternateHandler != onStartBGMAlternateHandler2);
		}
		remove
		{
			OnStartBGMAlternateHandler onStartBGMAlternateHandler = OnStartBGMAlternate__BackingField;
			OnStartBGMAlternateHandler onStartBGMAlternateHandler2;
			do
			{
				onStartBGMAlternateHandler2 = onStartBGMAlternateHandler;
				onStartBGMAlternateHandler = Interlocked.CompareExchange(ref OnStartBGMAlternate__BackingField, (OnStartBGMAlternateHandler)Delegate.Remove(onStartBGMAlternateHandler2, value), onStartBGMAlternateHandler);
			}
			while ((object)onStartBGMAlternateHandler != onStartBGMAlternateHandler2);
		}
	}

	public static event OnChangeSFXHandler OnSFXSlowDown
	{
		add
		{
			OnChangeSFXHandler onChangeSFXHandler = OnSFXSlowDown__BackingField;
			OnChangeSFXHandler onChangeSFXHandler2;
			do
			{
				onChangeSFXHandler2 = onChangeSFXHandler;
				onChangeSFXHandler = Interlocked.CompareExchange(ref OnSFXSlowDown__BackingField, (OnChangeSFXHandler)Delegate.Combine(onChangeSFXHandler2, value), onChangeSFXHandler);
			}
			while ((object)onChangeSFXHandler != onChangeSFXHandler2);
		}
		remove
		{
			OnChangeSFXHandler onChangeSFXHandler = OnSFXSlowDown__BackingField;
			OnChangeSFXHandler onChangeSFXHandler2;
			do
			{
				onChangeSFXHandler2 = onChangeSFXHandler;
				onChangeSFXHandler = Interlocked.CompareExchange(ref OnSFXSlowDown__BackingField, (OnChangeSFXHandler)Delegate.Remove(onChangeSFXHandler2, value), onChangeSFXHandler);
			}
			while ((object)onChangeSFXHandler != onChangeSFXHandler2);
		}
	}

	public static event OnChangeSFXStartEndHandler OnSFXFadeVolume
	{
		add
		{
			OnChangeSFXStartEndHandler onChangeSFXStartEndHandler = OnSFXFadeVolume__BackingField;
			OnChangeSFXStartEndHandler onChangeSFXStartEndHandler2;
			do
			{
				onChangeSFXStartEndHandler2 = onChangeSFXStartEndHandler;
				onChangeSFXStartEndHandler = Interlocked.CompareExchange(ref OnSFXFadeVolume__BackingField, (OnChangeSFXStartEndHandler)Delegate.Combine(onChangeSFXStartEndHandler2, value), onChangeSFXStartEndHandler);
			}
			while ((object)onChangeSFXStartEndHandler != onChangeSFXStartEndHandler2);
		}
		remove
		{
			OnChangeSFXStartEndHandler onChangeSFXStartEndHandler = OnSFXFadeVolume__BackingField;
			OnChangeSFXStartEndHandler onChangeSFXStartEndHandler2;
			do
			{
				onChangeSFXStartEndHandler2 = onChangeSFXStartEndHandler;
				onChangeSFXStartEndHandler = Interlocked.CompareExchange(ref OnSFXFadeVolume__BackingField, (OnChangeSFXStartEndHandler)Delegate.Remove(onChangeSFXStartEndHandler2, value), onChangeSFXStartEndHandler);
			}
			while ((object)onChangeSFXStartEndHandler != onChangeSFXStartEndHandler2);
		}
	}

	public static event OnChangeSFXStartEndHandler OnSFXFadeVolumeLinear
	{
		add
		{
			OnChangeSFXStartEndHandler onChangeSFXStartEndHandler = OnSFXFadeVolumeLinear__BackingField;
			OnChangeSFXStartEndHandler onChangeSFXStartEndHandler2;
			do
			{
				onChangeSFXStartEndHandler2 = onChangeSFXStartEndHandler;
				onChangeSFXStartEndHandler = Interlocked.CompareExchange(ref OnSFXFadeVolumeLinear__BackingField, (OnChangeSFXStartEndHandler)Delegate.Combine(onChangeSFXStartEndHandler2, value), onChangeSFXStartEndHandler);
			}
			while ((object)onChangeSFXStartEndHandler != onChangeSFXStartEndHandler2);
		}
		remove
		{
			OnChangeSFXStartEndHandler onChangeSFXStartEndHandler = OnSFXFadeVolumeLinear__BackingField;
			OnChangeSFXStartEndHandler onChangeSFXStartEndHandler2;
			do
			{
				onChangeSFXStartEndHandler2 = onChangeSFXStartEndHandler;
				onChangeSFXStartEndHandler = Interlocked.CompareExchange(ref OnSFXFadeVolumeLinear__BackingField, (OnChangeSFXStartEndHandler)Delegate.Remove(onChangeSFXStartEndHandler2, value), onChangeSFXStartEndHandler);
			}
			while ((object)onChangeSFXStartEndHandler != onChangeSFXStartEndHandler2);
		}
	}

	public static event OnWarbleBGMPitchHandler OnBGMPitchWarble
	{
		add
		{
			OnWarbleBGMPitchHandler onWarbleBGMPitchHandler = OnBGMPitchWarble__BackingField;
			OnWarbleBGMPitchHandler onWarbleBGMPitchHandler2;
			do
			{
				onWarbleBGMPitchHandler2 = onWarbleBGMPitchHandler;
				onWarbleBGMPitchHandler = Interlocked.CompareExchange(ref OnBGMPitchWarble__BackingField, (OnWarbleBGMPitchHandler)Delegate.Combine(onWarbleBGMPitchHandler2, value), onWarbleBGMPitchHandler);
			}
			while ((object)onWarbleBGMPitchHandler != onWarbleBGMPitchHandler2);
		}
		remove
		{
			OnWarbleBGMPitchHandler onWarbleBGMPitchHandler = OnBGMPitchWarble__BackingField;
			OnWarbleBGMPitchHandler onWarbleBGMPitchHandler2;
			do
			{
				onWarbleBGMPitchHandler2 = onWarbleBGMPitchHandler;
				onWarbleBGMPitchHandler = Interlocked.CompareExchange(ref OnBGMPitchWarble__BackingField, (OnWarbleBGMPitchHandler)Delegate.Remove(onWarbleBGMPitchHandler2, value), onWarbleBGMPitchHandler);
			}
			while ((object)onWarbleBGMPitchHandler != onWarbleBGMPitchHandler2);
		}
	}

	public static event OnBGMPlayListManualHandler OnPlayManualBGM
	{
		add
		{
			OnBGMPlayListManualHandler onBGMPlayListManualHandler = OnPlayManualBGM__BackingField;
			OnBGMPlayListManualHandler onBGMPlayListManualHandler2;
			do
			{
				onBGMPlayListManualHandler2 = onBGMPlayListManualHandler;
				onBGMPlayListManualHandler = Interlocked.CompareExchange(ref OnPlayManualBGM__BackingField, (OnBGMPlayListManualHandler)Delegate.Combine(onBGMPlayListManualHandler2, value), onBGMPlayListManualHandler);
			}
			while ((object)onBGMPlayListManualHandler != onBGMPlayListManualHandler2);
		}
		remove
		{
			OnBGMPlayListManualHandler onBGMPlayListManualHandler = OnPlayManualBGM__BackingField;
			OnBGMPlayListManualHandler onBGMPlayListManualHandler2;
			do
			{
				onBGMPlayListManualHandler2 = onBGMPlayListManualHandler;
				onBGMPlayListManualHandler = Interlocked.CompareExchange(ref OnPlayManualBGM__BackingField, (OnBGMPlayListManualHandler)Delegate.Remove(onBGMPlayListManualHandler2, value), onBGMPlayListManualHandler);
			}
			while ((object)onBGMPlayListManualHandler != onBGMPlayListManualHandler2);
		}
	}

	public static event OnSfxHandler OnPlayEvent
	{
		add
		{
			OnSfxHandler onSfxHandler = OnPlayEvent__BackingField;
			OnSfxHandler onSfxHandler2;
			do
			{
				onSfxHandler2 = onSfxHandler;
				onSfxHandler = Interlocked.CompareExchange(ref OnPlayEvent__BackingField, (OnSfxHandler)Delegate.Combine(onSfxHandler2, value), onSfxHandler);
			}
			while ((object)onSfxHandler != onSfxHandler2);
		}
		remove
		{
			OnSfxHandler onSfxHandler = OnPlayEvent__BackingField;
			OnSfxHandler onSfxHandler2;
			do
			{
				onSfxHandler2 = onSfxHandler;
				onSfxHandler = Interlocked.CompareExchange(ref OnPlayEvent__BackingField, (OnSfxHandler)Delegate.Remove(onSfxHandler2, value), onSfxHandler);
			}
			while ((object)onSfxHandler != onSfxHandler2);
		}
	}

	public static event OnSfxHandler OnPlayLoopEvent
	{
		add
		{
			OnSfxHandler onSfxHandler = OnPlayLoopEvent__BackingField;
			OnSfxHandler onSfxHandler2;
			do
			{
				onSfxHandler2 = onSfxHandler;
				onSfxHandler = Interlocked.CompareExchange(ref OnPlayLoopEvent__BackingField, (OnSfxHandler)Delegate.Combine(onSfxHandler2, value), onSfxHandler);
			}
			while ((object)onSfxHandler != onSfxHandler2);
		}
		remove
		{
			OnSfxHandler onSfxHandler = OnPlayLoopEvent__BackingField;
			OnSfxHandler onSfxHandler2;
			do
			{
				onSfxHandler2 = onSfxHandler;
				onSfxHandler = Interlocked.CompareExchange(ref OnPlayLoopEvent__BackingField, (OnSfxHandler)Delegate.Remove(onSfxHandler2, value), onSfxHandler);
			}
			while ((object)onSfxHandler != onSfxHandler2);
		}
	}

	public static event OnSfxHandler OnStopEvent
	{
		add
		{
			OnSfxHandler onSfxHandler = OnStopEvent__BackingField;
			OnSfxHandler onSfxHandler2;
			do
			{
				onSfxHandler2 = onSfxHandler;
				onSfxHandler = Interlocked.CompareExchange(ref OnStopEvent__BackingField, (OnSfxHandler)Delegate.Combine(onSfxHandler2, value), onSfxHandler);
			}
			while ((object)onSfxHandler != onSfxHandler2);
		}
		remove
		{
			OnSfxHandler onSfxHandler = OnStopEvent__BackingField;
			OnSfxHandler onSfxHandler2;
			do
			{
				onSfxHandler2 = onSfxHandler;
				onSfxHandler = Interlocked.CompareExchange(ref OnStopEvent__BackingField, (OnSfxHandler)Delegate.Remove(onSfxHandler2, value), onSfxHandler);
			}
			while ((object)onSfxHandler != onSfxHandler2);
		}
	}

	public static event OnSfxHandler OnPauseEvent
	{
		add
		{
			OnSfxHandler onSfxHandler = OnPauseEvent__BackingField;
			OnSfxHandler onSfxHandler2;
			do
			{
				onSfxHandler2 = onSfxHandler;
				onSfxHandler = Interlocked.CompareExchange(ref OnPauseEvent__BackingField, (OnSfxHandler)Delegate.Combine(onSfxHandler2, value), onSfxHandler);
			}
			while ((object)onSfxHandler != onSfxHandler2);
		}
		remove
		{
			OnSfxHandler onSfxHandler = OnPauseEvent__BackingField;
			OnSfxHandler onSfxHandler2;
			do
			{
				onSfxHandler2 = onSfxHandler;
				onSfxHandler = Interlocked.CompareExchange(ref OnPauseEvent__BackingField, (OnSfxHandler)Delegate.Remove(onSfxHandler2, value), onSfxHandler);
			}
			while ((object)onSfxHandler != onSfxHandler2);
		}
	}

	public static event OnSfxHandler OnUnpauseEvent
	{
		add
		{
			OnSfxHandler onSfxHandler = OnUnpauseEvent__BackingField;
			OnSfxHandler onSfxHandler2;
			do
			{
				onSfxHandler2 = onSfxHandler;
				onSfxHandler = Interlocked.CompareExchange(ref OnUnpauseEvent__BackingField, (OnSfxHandler)Delegate.Combine(onSfxHandler2, value), onSfxHandler);
			}
			while ((object)onSfxHandler != onSfxHandler2);
		}
		remove
		{
			OnSfxHandler onSfxHandler = OnUnpauseEvent__BackingField;
			OnSfxHandler onSfxHandler2;
			do
			{
				onSfxHandler2 = onSfxHandler;
				onSfxHandler = Interlocked.CompareExchange(ref OnUnpauseEvent__BackingField, (OnSfxHandler)Delegate.Remove(onSfxHandler2, value), onSfxHandler);
			}
			while ((object)onSfxHandler != onSfxHandler2);
		}
	}

	public static event Action OnStopAllEvent
	{
		add
		{
			Action action = OnStopAllEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnStopAllEvent__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnStopAllEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnStopAllEvent__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action OnStopBGMEvent
	{
		add
		{
			Action action = OnStopBGMEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnStopBGMEvent__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnStopBGMEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnStopBGMEvent__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action OnPlayBGMEvent
	{
		add
		{
			Action action = OnPlayBGMEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnPlayBGMEvent__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnPlayBGMEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnPlayBGMEvent__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action OnPlayBGMPlaylistEvent
	{
		add
		{
			Action action = OnPlayBGMPlaylistEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnPlayBGMPlaylistEvent__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnPlayBGMPlaylistEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnPlayBGMPlaylistEvent__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action OnPauseAllSFXEvent
	{
		add
		{
			Action action = OnPauseAllSFXEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnPauseAllSFXEvent__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnPauseAllSFXEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnPauseAllSFXEvent__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action OnUnpauseAllSFXEvent
	{
		add
		{
			Action action = OnUnpauseAllSFXEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnUnpauseAllSFXEvent__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnUnpauseAllSFXEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnUnpauseAllSFXEvent__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action OnStopManualBGMTrackEvent
	{
		add
		{
			Action action = OnStopManualBGMTrackEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnStopManualBGMTrackEvent__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnStopManualBGMTrackEvent__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnStopManualBGMTrackEvent__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static bool CheckIfPlaying(string key)
	{
		checkIfPlaying = false;
		if (OnCheckEvent != null)
		{
			key = key.ToLowerIfNecessary();
			checkIfPlaying = OnCheckEvent.CallAnyTrue(key);
			return checkIfPlaying;
		}
		return false;
	}

	public static void PlayBGMPlaylistManually(bool goThroughPlaylistAfter)
	{
		if (OnPlayManualBGM__BackingField != null)
		{
			OnPlayManualBGM__BackingField(goThroughPlaylistAfter);
		}
	}

	public static void StopBGMPlaylistManually()
	{
		if (OnStopManualBGMTrackEvent__BackingField != null)
		{
			OnStopManualBGMTrackEvent__BackingField();
		}
	}

	public static void ChangeSFXPitch(string key, float endPitch, float time)
	{
		if (OnSFXSlowDown__BackingField != null)
		{
			OnSFXSlowDown__BackingField(key, endPitch, time);
		}
	}

	public static void ChangeBGMPitch(float endPitch, float time)
	{
		if (OnBGMSlowdown__BackingField != null)
		{
			OnBGMSlowdown__BackingField(endPitch, time);
		}
	}

	public static void FadeBGMVolume(float endVolume, float time, bool fadeOut)
	{
		if (OnBGMFadeVolume__BackingField != null)
		{
			OnBGMFadeVolume__BackingField(endVolume, time, fadeOut);
		}
	}

	public static void WarbleBGMPitch(int warbles, float[] minValue, float[] maxValue, float[] incrementTime, float[] playTime)
	{
		if (OnBGMPitchWarble__BackingField != null)
		{
			OnBGMPitchWarble__BackingField(warbles, minValue, maxValue, incrementTime, playTime);
		}
	}

	public static void StartBGMAlternate(int index)
	{
		if (OnStartBGMAlternate__BackingField != null)
		{
			OnStartBGMAlternate__BackingField(index);
		}
	}

	public static void Attenuation(string key, bool attenuation, float endVolume)
	{
		if (OnAttenuation__BackingField != null)
		{
			OnAttenuation__BackingField(key, attenuation, endVolume);
		}
	}

	public static void Play(string key)
	{
		key = key.ToLowerIfNecessary();
		if (OnPlayEvent__BackingField != null)
		{
			OnPlayEvent__BackingField(key);
		}
	}

	public static void Stop(string key)
	{
		key = key.ToLowerIfNecessary();
		if (OnStopEvent__BackingField != null)
		{
			OnStopEvent__BackingField(key);
		}
	}

	public static void PlayLoop(string key)
	{
		key = key.ToLowerIfNecessary();
		if (OnPlayLoopEvent__BackingField != null)
		{
			OnPlayLoopEvent__BackingField(key);
		}
	}

	public static void Pause(string key)
	{
		key = key.ToLowerIfNecessary();
		if (OnPauseEvent__BackingField != null)
		{
			OnPauseEvent__BackingField(key);
		}
	}

	public static void Unpaused(string key)
	{
		key = key.ToLowerIfNecessary();
		if (OnUnpauseEvent__BackingField != null)
		{
			OnUnpauseEvent__BackingField(key);
		}
	}

	public static void Pan(string key, float value)
	{
		key = key.ToLowerIfNecessary();
		if (OnPanEvent__BackingField != null)
		{
			OnPanEvent__BackingField(key, value);
		}
	}

	public static void FadeSFXVolume(string key, float endVolume, float time)
	{
		FadeSFXVolume(key, -1f, endVolume, time);
	}

	public static void FadeSFXVolume(string key, float startVolume, float endVolume, float time)
	{
		if (OnSFXFadeVolume__BackingField != null)
		{
			OnSFXFadeVolume__BackingField(key, startVolume, endVolume, time);
		}
	}

	public static void FadeSFXVolumeLinear(string key, float endVolume, float time)
	{
		FadeSFXVolumeLinear(key, -1f, endVolume, time);
	}

	public static void FadeSFXVolumeLinear(string key, float startVolume, float endVolume, float time)
	{
		if (OnSFXFadeVolumeLinear__BackingField != null)
		{
			OnSFXFadeVolumeLinear__BackingField(key, startVolume, endVolume, time);
		}
	}

	public static void FollowObject(IEnumerable<string> keys, Transform transform)
	{
		foreach (string key in keys)
		{
			FollowObject(keys, transform);
		}
	}

	public static void FollowObject(string key, Transform transform)
	{
		key.ToLowerIfNecessary();
		if (OnFollowObject__BackingField != null)
		{
			OnFollowObject__BackingField(key, transform);
		}
	}

	[Obsolete("Use Play(string key) instead")]
	public static void Play(Sfx sfx)
	{
		Play(sfx.ToString());
	}

	[Obsolete("Use Stop(string key) instead")]
	public static void Stop(Sfx sfx)
	{
		Stop(sfx.ToString());
	}

	public static void StopAll()
	{
		if (OnStopAllEvent__BackingField != null)
		{
			OnStopAllEvent__BackingField();
		}
	}

	public static void StopBGM()
	{
		if (OnStopBGMEvent__BackingField != null)
		{
			OnStopBGMEvent__BackingField();
		}
	}

	public static void PlayBGM()
	{
		if (OnPlayBGMEvent__BackingField != null)
		{
			OnPlayBGMEvent__BackingField();
		}
	}

	public static void PlaylistBGM()
	{
		if (OnPlayBGMPlaylistEvent__BackingField != null)
		{
			OnPlayBGMPlaylistEvent__BackingField();
		}
	}

	public static void PauseAllSFX()
	{
		if (OnPauseAllSFXEvent__BackingField != null)
		{
			OnPauseAllSFXEvent__BackingField();
		}
	}

	public static void UnpauseAllSFX()
	{
		if (OnUnpauseAllSFXEvent__BackingField != null)
		{
			OnUnpauseAllSFXEvent__BackingField();
		}
	}

	public static void SnapshotTransition(string[] snapshotNames, float[] weights, float time)
	{
		if (OnSnapshotEvent__BackingField != null)
		{
			OnSnapshotEvent__BackingField(snapshotNames, weights, time);
		}
	}

	public static void HandleSnapshot(string snapshot, float time)
	{
		string[] array = new string[14]
		{
			Snapshots.Cutscene.ToString(),
			Snapshots.FrontEnd.ToString(),
			Snapshots.Unpaused.ToString(),
			Snapshots.Unpaused_Clean.ToString(),
			Snapshots.Unpaused_1920s.ToString(),
			Snapshots.Loadscreen.ToString(),
			Snapshots.Paused.ToString(),
			Snapshots.Super.ToString(),
			Snapshots.SuperStart.ToString(),
			Snapshots.Death.ToString(),
			Snapshots.EquipMenu.ToString(),
			Snapshots.RumRunners_RedBeam.ToString(),
			Snapshots.RumRunners_GreenBeam.ToString(),
			Snapshots.RumRunners_YellowBeam.ToString()
		};
		float[] array2 = new float[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array2[i] = ((!(array[i] == snapshot)) ? 0f : 1f);
		}
		SnapshotTransition(array, array2, time);
	}

	public static void SnapshotReset(string sceneName, float time)
	{
		string[] array = new string[14]
		{
			Snapshots.Cutscene.ToString(),
			Snapshots.FrontEnd.ToString(),
			Snapshots.Unpaused.ToString(),
			Snapshots.Unpaused_Clean.ToString(),
			Snapshots.Unpaused_1920s.ToString(),
			Snapshots.Loadscreen.ToString(),
			Snapshots.Paused.ToString(),
			Snapshots.Super.ToString(),
			Snapshots.SuperStart.ToString(),
			Snapshots.Death.ToString(),
			Snapshots.EquipMenu.ToString(),
			Snapshots.RumRunners_RedBeam.ToString(),
			Snapshots.RumRunners_GreenBeam.ToString(),
			Snapshots.RumRunners_YellowBeam.ToString()
		};
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			if (SettingsData.Data.vintageAudioEnabled)
			{
				if (array[i] == Snapshots.Unpaused_1920s.ToString())
				{
					num = i;
				}
			}
			else if (sceneName == Scenes.scene_level_retro_arcade.ToString())
			{
				if (array[i] == Snapshots.Unpaused_Clean.ToString())
				{
					num = i;
				}
			}
			else if (array[i] == Snapshots.Unpaused.ToString())
			{
				num = i;
			}
		}
		float[] array2 = new float[array.Length];
		for (int j = 0; j < array.Length; j++)
		{
			array2[j] = ((j != num) ? 0f : 1f);
		}
		SnapshotTransition(array, array2, time);
	}
}
