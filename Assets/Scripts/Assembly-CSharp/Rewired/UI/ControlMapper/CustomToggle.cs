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
	public class CustomToggle : Toggle, ICustomSelectable, ICancelHandler, IEventSystemHandler
	{
		[SerializeField]
		private Image checkImage;

		[SerializeField]
		private Image checkBoxImage;

		[SerializeField]
		private Color[] checkOverrideColor;

		[SerializeField]
		private Color[] checkBoxOverrideColor;

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

		private bool isHighlightDisabled;

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

		protected override void Start()
		{
			base.Start();
			ControlMapper.OnPlayerChange += UpdateColors;
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			StartCoroutine(update_text_color_on_enable_cr());
		}

		protected override void OnDestroy()
		{
			ControlMapper.OnPlayerChange -= UpdateColors;
		}

		private IEnumerator update_text_color_on_enable_cr()
		{
			yield return new WaitForEndOfFrame();
			UpdateAssociatedImageColors(base.currentSelectionState);
		}

		private void UpdateColors()
		{
			UpdateAssociatedImageColors(base.currentSelectionState);
		}

		public override void OnPointerClick(PointerEventData eventData)
		{
			base.OnPointerClick(eventData);
			UpdateColors();
		}

		public override void OnSubmit(BaseEventData eventData)
		{
			base.OnSubmit(eventData);
			UpdateColors();
		}

		protected void UpdateAssociatedImageColors(SelectionState state)
		{
			if (state == SelectionState.Highlighted)
			{
				checkImage.color = checkOverrideColor[(!base.isOn) ? 3 : 2];
			}
			else
			{
				checkImage.color = checkOverrideColor[(!base.isOn) ? 3 : ControlMapper.CurrentPlayer()];
			}
			checkBoxImage.color = checkBoxOverrideColor[(int)state];
		}

		protected override void DoStateTransition(SelectionState state, bool instant)
		{
			UpdateAssociatedImageColors(state);
			ColorBlock colorBlock = base.colors;
			colorBlock.normalColor = new Color(base.colors.normalColor.r, base.colors.normalColor.g, base.colors.normalColor.b, 0f);
			base.colors = colorBlock;
			if (isHighlightDisabled)
			{
				Color color = _disabledHighlightedColor;
				Sprite newSprite = _disabledHighlightedSprite;
				string triggername = _disabledHighlightedTrigger;
				if (base.gameObject.activeInHierarchy)
				{
					switch (base.transition)
					{
					case Transition.ColorTint:
						StartColorTween(color * base.colors.colorMultiplier, instant);
						break;
					case Transition.SpriteSwap:
						DoSpriteSwap(newSprite);
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
			EvaluateHightlightDisabled(true);
		}

		public override void OnDeselect(BaseEventData eventData)
		{
			base.OnDeselect(eventData);
			EvaluateHightlightDisabled(false);
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
