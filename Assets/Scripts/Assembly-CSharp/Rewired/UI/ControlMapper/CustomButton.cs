using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Rewired.UI.ControlMapper
{
	[AddComponentMenu("")]
	public class CustomButton : Button, ICustomSelectable, ICancelHandler, IEventSystemHandler
	{
		[SerializeField]
		protected Text associatedText;

		[SerializeField]
		private Color[] textOverrideColor;

		[SerializeField]
		private Sprite _disabledHighlightedSprite;

		[SerializeField]
		private Color _disabledHighlightedColor;

		[SerializeField]
		private string _disabledHighlightedTrigger;

		[SerializeField]
		private bool _autoNavUp = true;

		[SerializeField]
		private bool _autoNavDown = true;

		[SerializeField]
		private bool _autoNavLeft = true;

		[SerializeField]
		private bool _autoNavRight = true;

		protected bool isHighlightDisabled;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private UnityAction _CancelEvent__BackingField;

		public Sprite disabledHighlightedSprite
		{
			get
			{
				return _disabledHighlightedSprite;
			}
			set
			{
				_disabledHighlightedSprite = value;
			}
		}

		public Color disabledHighlightedColor
		{
			get
			{
				return _disabledHighlightedColor;
			}
			set
			{
				_disabledHighlightedColor = value;
			}
		}

		public string disabledHighlightedTrigger
		{
			get
			{
				return _disabledHighlightedTrigger;
			}
			set
			{
				_disabledHighlightedTrigger = value;
			}
		}

		public bool autoNavUp
		{
			get
			{
				return _autoNavUp;
			}
			set
			{
				_autoNavUp = value;
			}
		}

		public bool autoNavDown
		{
			get
			{
				return _autoNavDown;
			}
			set
			{
				_autoNavDown = value;
			}
		}

		public bool autoNavLeft
		{
			get
			{
				return _autoNavLeft;
			}
			set
			{
				_autoNavLeft = value;
			}
		}

		public bool autoNavRight
		{
			get
			{
				return _autoNavRight;
			}
			set
			{
				_autoNavRight = value;
			}
		}

		private bool isDisabled
		{
			get
			{
				return !IsInteractable();
			}
		}

		private event UnityAction _CancelEvent
		{
			add
			{
				UnityAction unityAction = _CancelEvent__BackingField;
				UnityAction unityAction2;
				do
				{
					unityAction2 = unityAction;
					unityAction = Interlocked.CompareExchange(ref _CancelEvent__BackingField, (UnityAction)Delegate.Combine(unityAction2, value), unityAction);
				}
				while ((object)unityAction != unityAction2);
			}
			remove
			{
				UnityAction unityAction = _CancelEvent__BackingField;
				UnityAction unityAction2;
				do
				{
					unityAction2 = unityAction;
					unityAction = Interlocked.CompareExchange(ref _CancelEvent__BackingField, (UnityAction)Delegate.Remove(unityAction2, value), unityAction);
				}
				while ((object)unityAction != unityAction2);
			}
		}

		public event UnityAction CancelEvent
		{
			add
			{
				_CancelEvent += value;
			}
			remove
			{
				_CancelEvent -= value;
			}
		}

		public override Selectable FindSelectableOnLeft()
		{
			if ((base.navigation.mode & Navigation.Mode.Horizontal) != Navigation.Mode.None || _autoNavLeft)
			{
				return UISelectionUtility.FindNextSelectable(this, base.transform, Selectable.allSelectables, base.transform.rotation * Vector3.left);
			}
			return base.FindSelectableOnLeft();
		}

		public override Selectable FindSelectableOnRight()
		{
			if ((base.navigation.mode & Navigation.Mode.Horizontal) != Navigation.Mode.None || _autoNavRight)
			{
				return UISelectionUtility.FindNextSelectable(this, base.transform, Selectable.allSelectables, base.transform.rotation * Vector3.right);
			}
			return base.FindSelectableOnRight();
		}

		public override Selectable FindSelectableOnUp()
		{
			if ((base.navigation.mode & Navigation.Mode.Vertical) != Navigation.Mode.None || _autoNavUp)
			{
				return UISelectionUtility.FindNextSelectable(this, base.transform, Selectable.allSelectables, base.transform.rotation * Vector3.up);
			}
			return base.FindSelectableOnUp();
		}

		public override Selectable FindSelectableOnDown()
		{
			if ((base.navigation.mode & Navigation.Mode.Vertical) != Navigation.Mode.None || _autoNavDown)
			{
				return UISelectionUtility.FindNextSelectable(this, base.transform, Selectable.allSelectables, base.transform.rotation * Vector3.down);
			}
			return base.FindSelectableOnDown();
		}

		protected override void OnCanvasGroupChanged()
		{
			base.OnCanvasGroupChanged();
			if (!(EventSystem.current == null))
			{
				EvaluateHightlightDisabled(EventSystem.current.currentSelectedGameObject == base.gameObject);
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			StartCoroutine(update_text_color_on_enable_cr());
		}

		private IEnumerator update_text_color_on_enable_cr()
		{
			yield return new WaitForEndOfFrame();
			UpdateAssociatedTextColor(base.currentSelectionState);
		}

		protected void UpdateAssociatedTextColor(SelectionState state)
		{
			if (!associatedText)
			{
				return;
			}
			if (textOverrideColor.Length == 4)
			{
				associatedText.color = textOverrideColor[(int)state];
				return;
			}
			switch ((int)state)
			{
			case 0:
				associatedText.color = base.colors.normalColor;
				break;
			case 1:
				associatedText.color = base.colors.highlightedColor;
				break;
			case 2:
				associatedText.color = base.colors.pressedColor;
				break;
			case 3:
				associatedText.color = base.colors.disabledColor;
				break;
			}
		}

		protected override void DoStateTransition(SelectionState state, bool instant)
		{
			UpdateAssociatedTextColor(state);
			if (isHighlightDisabled)
			{
				Color color = _disabledHighlightedColor;
				Sprite sprite = _disabledHighlightedSprite;
				string triggername = _disabledHighlightedTrigger;
				if (base.gameObject.activeInHierarchy)
				{
					switch (base.transition)
					{
					case Transition.ColorTint:
						StartColorTween(color * base.colors.colorMultiplier, instant);
						break;
					case Transition.SpriteSwap:
						DoSpriteSwap((state != SelectionState.Highlighted) ? sprite : base.spriteState.highlightedSprite);
						break;
					case Transition.Animation:
						TriggerAnimation(triggername);
						break;
					}
				}
			}
			else
			{
				base.DoStateTransition(state, instant);
			}
		}

		private void StartColorTween(Color targetColor, bool instant)
		{
			if (!(base.targetGraphic == null))
			{
				base.targetGraphic.CrossFadeColor(targetColor, (!instant) ? base.colors.fadeDuration : 0f, true, true);
			}
		}

		private void DoSpriteSwap(Sprite newSprite)
		{
			if (!(base.image == null))
			{
				base.image.overrideSprite = newSprite;
			}
		}

		private void TriggerAnimation(string triggername)
		{
			if (!(base.animator == null) && base.animator.enabled && base.animator.isActiveAndEnabled && !(base.animator.runtimeAnimatorController == null) && !string.IsNullOrEmpty(triggername))
			{
				base.animator.ResetTrigger(_disabledHighlightedTrigger);
				base.animator.SetTrigger(triggername);
			}
		}

		public override void OnSelect(BaseEventData eventData)
		{
			base.OnSelect(eventData);
			AudioManager.Play("level_menu_move");
			EvaluateHightlightDisabled(true);
		}

		public override void OnDeselect(BaseEventData eventData)
		{
			base.OnDeselect(eventData);
			EvaluateHightlightDisabled(false);
		}

		public void SetNavOnToggle(bool setting)
		{
			base.navigation = new Navigation
			{
				mode = (setting ? Navigation.Mode.Automatic : Navigation.Mode.None)
			};
		}

		public void Press()
		{
			if (IsActive() && IsInteractable())
			{
				base.onClick.Invoke();
			}
		}

		public override void OnPointerClick(PointerEventData eventData)
		{
			if (IsActive() && IsInteractable() && eventData.button == PointerEventData.InputButton.Left)
			{
				Press();
				if (!IsActive() || !IsInteractable())
				{
					isHighlightDisabled = true;
					DoStateTransition(SelectionState.Disabled, false);
				}
			}
		}

		public override void OnSubmit(BaseEventData eventData)
		{
			Press();
			if (!IsActive() || !IsInteractable())
			{
				isHighlightDisabled = true;
				DoStateTransition(SelectionState.Disabled, false);
			}
			else
			{
				DoStateTransition(SelectionState.Pressed, false);
				StartCoroutine(OnFinishSubmit());
			}
		}

		private IEnumerator OnFinishSubmit()
		{
			float fadeTime = base.colors.fadeDuration;
			float elapsedTime = 0f;
			while (elapsedTime < fadeTime)
			{
				elapsedTime += Time.unscaledDeltaTime;
				yield return null;
			}
			DoStateTransition(base.currentSelectionState, false);
		}

		private void EvaluateHightlightDisabled(bool isSelected)
		{
			if (!isSelected)
			{
				if (isHighlightDisabled)
				{
					isHighlightDisabled = false;
					SelectionState state = ((!isDisabled) ? base.currentSelectionState : SelectionState.Disabled);
					DoStateTransition(state, false);
				}
			}
			else if (isDisabled)
			{
				isHighlightDisabled = true;
				DoStateTransition(SelectionState.Disabled, false);
			}
		}

		public void OnCancel(BaseEventData eventData)
		{
			if (_CancelEvent__BackingField != null)
			{
				_CancelEvent__BackingField();
			}
		}
	}
}
