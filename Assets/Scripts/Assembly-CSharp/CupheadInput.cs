using Rewired;

public class CupheadInput
{
	public enum InputDevice
	{
		Keyboard = 0,
		Controller_1 = 1,
		Controller_2 = 2
	}

	public enum InputSymbols
	{
		XBOX_NONE = 0,
		XBOX_A = 1,
		XBOX_B = 2,
		XBOX_X = 3,
		XBOX_Y = 4,
		XBOX_RB = 5,
		XBOX_LB = 6
	}

	public class AnyPlayerInput
	{
		private Player[] players;

		public bool checkIfDead;

		public AnyPlayerInput(bool checkIfDead = false)
		{
			this.checkIfDead = checkIfDead;
			players = new Player[2]
			{
				PlayerManager.GetPlayerInput(PlayerId.PlayerOne),
				PlayerManager.GetPlayerInput(PlayerId.PlayerTwo)
			};
		}

		public bool GetButton(CupheadButton button)
		{
			Player[] array = players;
			foreach (Player player in array)
			{
				if (player.GetButton((int)button) && (!checkIfDead || !IsDead(player)))
				{
					return true;
				}
			}
			return false;
		}

		public bool GetButtonDown(CupheadButton button)
		{
			if (InterruptingPrompt.IsInterrupting())
			{
				return false;
			}
			Player[] array = players;
			foreach (Player player in array)
			{
				if (player.GetButtonDown((int)button) && (!checkIfDead || !IsDead(player)))
				{
					return true;
				}
			}
			return false;
		}

		public bool GetActionButtonDown()
		{
			if (InterruptingPrompt.IsInterrupting())
			{
				return false;
			}
			Player[] array = players;
			foreach (Player player in array)
			{
				if ((player.GetButtonDown(13) || player.GetButtonDown(14) || player.GetButtonDown(7) || player.GetButtonDown(15) || player.GetButtonDown(2) || player.GetButtonDown(6) || player.GetButtonDown(8) || player.GetButtonDown(3) || player.GetButtonDown(4) || player.GetButtonDown(5)) && (!checkIfDead || !IsDead(player)))
				{
					return true;
				}
			}
			return false;
		}

		public bool GetAnyButtonDown()
		{
			if (InterruptingPrompt.IsInterrupting())
			{
				return false;
			}
			Player[] array = players;
			foreach (Player player in array)
			{
				foreach (Controller controller in player.controllers.Controllers)
				{
					if (controller.GetAnyButtonDown() && (!checkIfDead || !IsDead(player)))
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool GetAnyButtonHeld()
		{
			if (InterruptingPrompt.IsInterrupting())
			{
				return false;
			}
			Player[] array = players;
			foreach (Player player in array)
			{
				foreach (Controller controller in player.controllers.Controllers)
				{
					if (controller.GetAnyButton() && (!checkIfDead || !IsDead(player)))
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool GetButtonUp(CupheadButton button)
		{
			Player[] array = players;
			foreach (Player player in array)
			{
				if (player.GetButtonUp((int)button) && (!checkIfDead || !IsDead(player)))
				{
					return true;
				}
			}
			return false;
		}

		private bool IsDead(Player player)
		{
			PlayerId id = ((player != players[0]) ? PlayerId.PlayerTwo : PlayerId.PlayerOne);
			AbstractPlayerController player2 = PlayerManager.GetPlayer(id);
			return player2 == null || player2.IsDead;
		}
	}

	public class Pair
	{
		public readonly InputSymbols symbol;

		public readonly string first;

		public readonly string second;

		public Pair(InputSymbols symbol, string first, string second)
		{
			this.symbol = symbol;
			this.first = first;
			this.second = second;
		}
	}

	public static readonly Pair[] pairs = new Pair[4]
	{
		new Pair(InputSymbols.XBOX_A, "<sprite=0>", "<sprite=1>"),
		new Pair(InputSymbols.XBOX_B, "<sprite=2>", "<sprite=3>"),
		new Pair(InputSymbols.XBOX_X, "<sprite=4>", "<sprite=5>"),
		new Pair(InputSymbols.XBOX_Y, "<sprite=6>", "<sprite=7>")
	};

	public static Localization.Translation InputDisplayForButton(CupheadButton button, int rewiredPlayerId = 0)
	{
		ActionElementMap actionElementMap = null;
		Player.ControllerHelper controllers = ReInput.players.GetPlayer(rewiredPlayerId).controllers;
		if (controllers != null && controllers.joystickCount > 0)
		{
			ControllerType controllerType = ControllerType.Joystick;
			Controller lastActiveController = ReInput.players.GetPlayer(rewiredPlayerId).controllers.GetLastActiveController();
			if (lastActiveController != null)
			{
				controllerType = lastActiveController.type;
			}
			actionElementMap = controllers.maps.GetFirstElementMapWithAction(controllerType, (int)button, true);
		}
		else
		{
			if (PlatformHelper.IsConsole)
			{
				return default(Localization.Translation);
			}
			actionElementMap = ReInput.players.GetPlayer(rewiredPlayerId).controllers.maps.GetFirstElementMapWithAction((int)button, true);
		}
		if (actionElementMap == null)
		{
			return new Localization.Translation
			{
				text = string.Empty
			};
		}
		string text = actionElementMap.elementIdentifierName;
		if (button == CupheadButton.EquipMenu && text.Contains("Shift"))
		{
			text = "Shift";
		}
		string text2 = handleCustomGlyphs(text, rewiredPlayerId);
		Localization.Translation result = Localization.Translate(text);
		if (text2 == null)
		{
			if (!string.IsNullOrEmpty(result.text))
			{
				text = result.text;
			}
		}
		else
		{
			text = text2;
		}
		text = text.ToUpper();
		text = text.Replace(" SHOULDER", "B");
		text = text.Replace(" BUMPER", "B");
		text = text.Replace(" TRIGGER", "T");
		text = text.Replace("LEFT", "L");
		text = text.Replace("RIGHT", "R");
		text = text.Replace("R SHIFT", "SHIFT");
		text = text.Replace("L SHIFT", "SHIFT");
		text = text.Replace(" +", string.Empty);
		text = text.Replace(" -", string.Empty);
		result.text = text;
		return result;
	}

	private static string handleCustomGlyphs(string input, int rewiredPlayerId)
	{
		return null;
	}

	public static InputSymbols InputSymbolForButton(CupheadButton button)
	{
		InputSymbols inputSymbols = InputSymbols.XBOX_NONE;
		switch (button)
		{
		default:
			return InputSymbols.XBOX_NONE;
		case CupheadButton.Accept:
			return InputSymbols.XBOX_A;
		case CupheadButton.Jump:
			return InputSymbols.XBOX_A;
		case CupheadButton.Cancel:
			return InputSymbols.XBOX_B;
		case CupheadButton.Super:
			return InputSymbols.XBOX_B;
		case CupheadButton.Shoot:
			return InputSymbols.XBOX_X;
		case CupheadButton.Dash:
			return InputSymbols.XBOX_Y;
		case CupheadButton.Lock:
			return InputSymbols.XBOX_RB;
		case CupheadButton.SwitchWeapon:
			return InputSymbols.XBOX_LB;
		}
	}

	public static string DialogueStringFromButton(CupheadButton button)
	{
		return string.Concat(" {", button, "} ");
	}

	public static Joystick CheckForUnconnectedControllerPress()
	{
		foreach (Joystick joystick in ReInput.controllers.Joysticks)
		{
			if (ReInput.controllers.IsJoystickAssigned(joystick) || !joystick.GetAnyButtonDown())
			{
				continue;
			}
			return joystick;
		}
		return null;
	}

	public static Joystick CheckForControllerPress(long systemID)
	{
		foreach (Joystick joystick in ReInput.controllers.Joysticks)
		{
			long? systemId = joystick.systemId;
			if (systemId.GetValueOrDefault() == systemID && systemId.HasValue && joystick.GetAnyButtonDown())
			{
				return joystick;
			}
		}
		return null;
	}

	public static bool AutoAssignController(int rewiredPlayerId)
	{
		foreach (Joystick joystick in ReInput.controllers.Joysticks)
		{
			if (!ReInput.controllers.IsJoystickAssigned(joystick))
			{
				Player player = ReInput.players.GetPlayer(rewiredPlayerId);
				if (player != null && player.controllers.joystickCount <= 0)
				{
					player.controllers.AddController(joystick, true);
					return true;
				}
			}
		}
		return false;
	}
}
