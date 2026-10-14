using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.PostProcessing;

public class TowerOfPowerScorecard : AbstractMonoBehaviour
{
	private static readonly int[] TryRegularCurveValues = new int[12]
	{
		0, 62, 48, 57, 65, 74, 58, 36, 54, 72,
		83, 27
	};

	private static readonly int[] TryRegularCurveOffsets = new int[12]
	{
		0, 32, 18, 27, 35, 45, 28, 7, 24, 42,
		50, -3
	};

	private const float BOB_FRAME_TIME = 1f / 24f;

	[Header("Delays")]
	[SerializeField]
	private float introDelay = 10f;

	[SerializeField]
	private float talliesDelay = 0.5f;

	[SerializeField]
	private float gradeDelay = 0.7f;

	[SerializeField]
	private float advanceDelay = 10f;

	[SerializeField]
	private WinScreenTicker timeTicker;

	[SerializeField]
	private WinScreenTicker hitsTicker;

	[SerializeField]
	private WinScreenTicker parriesTicker;

	[SerializeField]
	private WinScreenTicker superMeterTicker;

	[SerializeField]
	private WinScreenTicker difficultyTicker;

	[SerializeField]
	private LocalizationHelper spiritStockLabelLocalizationHelper;

	[SerializeField]
	private WinScreenGradeDisplay gradeDisplay;

	[SerializeField]
	private GameObject continuePrompt;

	[Header("UI Scoring")]
	[SerializeField]
	private GameObject scoring;

	[SerializeField]
	private TextMeshProUGUI gradeLabel;

	[Header("Try Text")]
	[SerializeField]
	private GameObject tryRegular;

	[SerializeField]
	private TMP_Text tryRegularText;

	[SerializeField]
	private SpriteRenderer tryRegularEnglishBackground;

	[Header("Glow effect")]
	[SerializeField]
	private GameObject glowingText;

	[SerializeField]
	private GlowText glowScript;

	[SerializeField]
	private PostProcessingBehaviour postProcessingScript;

	[SerializeField]
	public PostProcessingProfile asianProfile;

	[SerializeField]
	private GameObject tryExpert;

	[SerializeField]
	private TMP_Text tryExpertText;

	[Space(10f)]
	[SerializeField]
	private SpriteRenderer[] studioMHDRSubtitles;

	[SerializeField]
	private Transform[] resultsTitles;

	[SerializeField]
	private Vector3 playerOneOffCenterTitleRoot;

	[SerializeField]
	private Vector3 japaneseTitleRoot;

	[SerializeField]
	private Vector3 koreanTitleRoot;

	[SerializeField]
	private Vector3 chineseTitleRoot;

	[Space(10f)]
	[SerializeField]
	private Canvas results;

	[Header("BannerCurve")]
	[SerializeField]
	private MinMax textWidthRange;

	[SerializeField]
	private MinMax curveScaleRange;

	[SerializeField]
	private float yOffsetDelta;

	private CupheadInput.AnyPlayerInput input;

	private const float BG_NORMAL_SPEED = 50f;

	private const float BG_FAST_SPEED = 150f;

	public bool done;

	protected override void Awake()
	{
		base.Awake();
		if (Localization.language != Localization.Languages.English)
		{
			DisableEnglishMDHRSubtitles();
		}
		if (Localization.language == Localization.Languages.Japanese)
		{
			CenterResultTitles(japaneseTitleRoot);
		}
		else if (Localization.language == Localization.Languages.Korean)
		{
			CenterResultTitles(koreanTitleRoot);
		}
		else if (Localization.language == Localization.Languages.SimplifiedChinese || Localization.language == Localization.Languages.German || Localization.language == Localization.Languages.SpanishSpain || Localization.language == Localization.Languages.SpanishAmerica || Localization.language == Localization.Languages.Russian || Localization.language == Localization.Languages.PortugueseBrazil)
		{
			CenterResultTitles(chineseTitleRoot);
		}
		Cuphead.Init();
		if (PlayerManager.Multiplayer)
		{
			if (Localization.language == Localization.Languages.Polish || Localization.language == Localization.Languages.Italian || Localization.language == Localization.Languages.French)
			{
				CenterResultTitles(japaneseTitleRoot);
			}
		}
		else if (Localization.language == Localization.Languages.Polish || Localization.language == Localization.Languages.Italian || Localization.language == Localization.Languages.French || Localization.language == Localization.Languages.SimplifiedChinese || Localization.language == Localization.Languages.Japanese)
		{
			CenterResultTitles(playerOneOffCenterTitleRoot);
		}
		StartCoroutine(main_cr());
		continuePrompt.SetActive(false);
		input = new CupheadInput.AnyPlayerInput();
	}

	private void DisableEnglishMDHRSubtitles()
	{
		SpriteRenderer[] array = studioMHDRSubtitles;
		foreach (SpriteRenderer spriteRenderer in array)
		{
			spriteRenderer.enabled = false;
		}
	}

	private void CenterResultTitles(Vector3 rootPosition)
	{
		Transform[] array = resultsTitles;
		foreach (Transform transform in array)
		{
			transform.localPosition = rootPosition;
		}
	}

	private IEnumerator main_cr()
	{
		LevelScoringData data = Level.ScoringData;
		done = false;
		if (Localization.language == Localization.Languages.Korean)
		{
			TextMeshProUGUI[] componentsInChildren = scoring.GetComponentsInChildren<TextMeshProUGUI>();
			foreach (TextMeshProUGUI textMeshProUGUI in componentsInChildren)
			{
				textMeshProUGUI.fontStyle = FontStyles.Bold;
			}
			gradeLabel.fontStyle = FontStyles.Bold;
		}
		if (!Level.IsTowerOfPowerMain && data.difficulty == Level.Mode.Easy && Level.PreviousDifficulty == Level.Mode.Easy && Level.PreviousLevelType == Level.Type.Battle && !Level.IsDicePalace && !Level.IsDicePalaceMain && Level.PreviousLevel != Levels.Devil)
		{
			Localization.Translation translation = Localization.Translate("ResultsTryRegular");
			tryRegular.SetActive(true);
			if (translation.image == null && !translation.hasSpriteAtlasImage)
			{
				tryRegular.GetComponent<SpriteRenderer>().enabled = false;
				tryRegularEnglishBackground.enabled = false;
				tryRegularText.text = translation.text;
				tryRegularText.font = translation.fonts.fontAsset;
				tryRegularText.fontSize = ((translation.fonts.fontAssetSize != 0f) ? translation.fonts.fontAssetSize : tryRegularText.fontSize);
				tryRegularText.outlineWidth = ((Localization.language != Localization.Languages.Korean) ? tryRegularText.outlineWidth : 0.07f);
				AlignBannerText(tryRegularText.gameObject);
				if (Localization.language == Localization.Languages.Korean || Localization.language == Localization.Languages.Japanese)
				{
					postProcessingScript.profile = asianProfile;
				}
				AlignBannerText(glowingText);
				glowScript.InitTMPText(tryRegularText);
				if (Localization.language != Localization.Languages.English)
				{
					glowScript.BeginGlow();
				}
			}
			else
			{
				tryRegularText.enabled = false;
			}
		}
		timeTicker.TargetValue = (int)data.time;
		timeTicker.MaxValue = (int)data.goalTime;
		hitsTicker.TargetValue = ((data.numTimesHit < 3) ? (3 - data.numTimesHit) : 0);
		hitsTicker.MaxValue = 3;
		parriesTicker.TargetValue = Mathf.Min(data.numParries, (int)Cuphead.Current.ScoringProperties.parriesForHighestGrade);
		parriesTicker.MaxValue = (int)Cuphead.Current.ScoringProperties.parriesForHighestGrade;
		superMeterTicker.TargetValue = Mathf.Min(data.superMeterUsed, (int)Cuphead.Current.ScoringProperties.superMeterUsageForHighestGrade);
		superMeterTicker.MaxValue = (int)Cuphead.Current.ScoringProperties.superMeterUsageForHighestGrade;
		if (data.useCoinsInsteadOfSuperMeter)
		{
			superMeterTicker.TargetValue = data.coinsCollected;
			superMeterTicker.MaxValue = 5;
			spiritStockLabelLocalizationHelper.currentID = Localization.Find("ResultsMenuCoins").id;
		}
		difficultyTicker.TargetValue = ((data.difficulty != Level.Mode.Easy) ? ((data.difficulty == Level.Mode.Normal) ? 1 : 2) : 0);
		gradeDisplay.Grade = Level.Grade;
		gradeDisplay.Difficulty = data.difficulty;
		yield return new WaitForSeconds(introDelay);
		WinScreenTicker[] tickers = new WinScreenTicker[5] { timeTicker, hitsTicker, parriesTicker, superMeterTicker, difficultyTicker };
		WinScreenTicker[] array = tickers;
		foreach (WinScreenTicker ticker in array)
		{
			ticker.StartCounting();
			while (!ticker.FinishedCounting)
			{
				yield return null;
			}
			if (ticker.TargetValue != 0)
			{
				yield return new WaitForSeconds(talliesDelay);
			}
		}
		InterruptingPrompt.SetCanInterrupt(true);
		float timer = 0f;
		while (timer < gradeDelay && !input.GetAnyButtonDown())
		{
			if (!InterruptingPrompt.IsInterrupting())
			{
				timer += Time.deltaTime;
			}
			yield return null;
		}
		gradeDisplay.Show();
		while (!gradeDisplay.FinishedGrading)
		{
			yield return null;
		}
		timer = 0f;
		continuePrompt.SetActive(true);
		while (timer < advanceDelay && !input.GetActionButtonDown())
		{
			if (!InterruptingPrompt.IsInterrupting())
			{
				timer += Time.deltaTime;
			}
			yield return null;
		}
		done = true;
	}

	private void AlignBannerText(GameObject bannerText)
	{
		bannerText.GetComponent<TextMeshCurveAndJitter>().CurveScale = TryRegularCurveValues[(int)Localization.language];
		Vector3 localPosition = bannerText.transform.localPosition;
		localPosition.y = TryRegularCurveOffsets[(int)Localization.language];
		bannerText.transform.localPosition = localPosition;
	}

	private string GetTriggerName(Localization.Languages language)
	{
		switch (language)
		{
		case Localization.Languages.French:
			return "useFrench";
		case Localization.Languages.Italian:
			return "useItalian";
		case Localization.Languages.German:
			return "useGerman";
		case Localization.Languages.Polish:
			return "usePolish";
		case Localization.Languages.PortugueseBrazil:
			return "usePortuguese";
		case Localization.Languages.Russian:
			return "useRussian";
		case Localization.Languages.SpanishSpain:
			return "useSpanishSpain";
		case Localization.Languages.SpanishAmerica:
			return "useSpanishAmerica";
		case Localization.Languages.Japanese:
			return "useJapanese";
		case Localization.Languages.Korean:
			return "useKorean";
		case Localization.Languages.SimplifiedChinese:
			return "useChinese";
		default:
			return "useEnglish";
		}
	}
}
