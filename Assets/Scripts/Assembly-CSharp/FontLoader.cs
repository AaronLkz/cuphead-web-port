using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public static class FontLoader
{
	public enum FontType
	{
		None = 0,
		CupheadFelix_Regular_merged = 1,
		CupheadHenriette_A_merged = 2,
		CupheadMemphis_Medium_merged = 3,
		CupheadPoster_Regular66Cyr_Lat_English99 = 4,
		CupheadVogue_Bold_merged = 5,
		CupheadVogue_ExtraBold_merged = 6,
		DFBrushRDStd_W7 = 7,
		DFBrushSQStd_W5 = 8,
		DSRefinedLetterB = 9,
		FBBlue = 10,
		hyk2gjm = 11,
		jpchw00u = 12,
		MComicPRC_Medium = 13,
		YoonBackjaeM = 14,
		rounded_mgenplus_1c_medium = 15,
		hisikusa_A = 16,
		FGPotego__2 = 17,
		FGPotegoBold__2 = 18,
		FGNewRetro = 19,
		ElegantHeiseiMinchoMono_9W = 20
	}

	public enum TMPFontType
	{
		None = 0,
		CupheadFelix_Regular_merged__SDF = 1,
		CupheadHenriette_A_merged__SDF = 2,
		CupheadMemphis_Medium_merged__SDF = 3,
		CupheadPoster_Regular66Cyr_Lat_English99__SDF = 4,
		CupheadVogue_Bold_merged__SDF = 5,
		CupheadVogue_ExtraBold_merged__outline__SDF = 6,
		CupheadVogue_ExtraBold_merged__SDF = 7,
		CupheadVogue_ExtraBold_merged__shadow__SDF = 8,
		DFBrushRDStd_W7__outline__SDF = 9,
		DFBrushRDStd_W7__SDF = 10,
		DFBrushRDStd_W7__shadow__SDF = 11,
		DFBrushSQStd_W5__SDF = 12,
		DSRefinedLetterB__SDF = 13,
		FBBlue__SDF = 14,
		hyk2gjm__outline__SDF = 15,
		hyk2gjm__SDF = 16,
		hyk2gjm__shadow__SDF = 17,
		jpchw00u__SDF = 18,
		MComicPRC_Medium__SDF = 19,
		YoonBackjaeM__outline__SDF = 20,
		YoonBackjaeM__SDF = 21,
		YoonBackjaeM__shadow__SDF = 22,
		YoonBackjaeM__bold__SDF = 23,
		rounded_mgenplus_1c_meduim__SDF = 24,
		hisikusa_A__SDF = 25,
		FGPotego__2__SDF = 26,
		FGPotegoBold__2__SDF = 27,
		FGNewRetro__SDF = 28,
		ElegantHeiseiMinchoMono_9W__SDF = 29,
		FGPotegoBold__2__outline__SDF = 30
	}

	private static Dictionary<FontType, string> FontTypeMapping = new Dictionary<FontType, string>
	{
		{
			FontType.CupheadFelix_Regular_merged,
			"CupheadFelix-Regular-merged"
		},
		{
			FontType.CupheadHenriette_A_merged,
			"CupheadHenriette-A-merged"
		},
		{
			FontType.CupheadMemphis_Medium_merged,
			"CupheadMemphis-Medium-merged"
		},
		{
			FontType.CupheadPoster_Regular66Cyr_Lat_English99,
			"CupheadPoster-Regular(Cyr_Lat_English)"
		},
		{
			FontType.CupheadVogue_Bold_merged,
			"CupheadVogue-Bold-merged"
		},
		{
			FontType.CupheadVogue_ExtraBold_merged,
			"CupheadVogue-ExtraBold-merged"
		},
		{
			FontType.DFBrushRDStd_W7,
			"DFBrushRDStd-W7"
		},
		{
			FontType.DFBrushSQStd_W5,
			"DFBrushSQStd-W5"
		},
		{
			FontType.DSRefinedLetterB,
			"DSRefinedLetterB"
		},
		{
			FontType.FBBlue,
			"FBBlue"
		},
		{
			FontType.hyk2gjm,
			"hyk2gjm"
		},
		{
			FontType.jpchw00u,
			"jpchw00u"
		},
		{
			FontType.MComicPRC_Medium,
			"MComicPRC-Medium"
		},
		{
			FontType.YoonBackjaeM,
			"YoonBackjaeM"
		},
		{
			FontType.rounded_mgenplus_1c_medium,
			"rounded-mgenplus-1c-medium"
		},
		{
			FontType.hisikusa_A,
			"hisikusa-A"
		},
		{
			FontType.FGPotego__2,
			"FGPotego 2"
		},
		{
			FontType.FGPotegoBold__2,
			"FGPotegoBold 2"
		},
		{
			FontType.FGNewRetro,
			"FGNewRetro"
		},
		{
			FontType.ElegantHeiseiMinchoMono_9W,
			"ElegantHeiseiMinchoMono-9W"
		}
	};

	private static Dictionary<TMPFontType, string> TMPFontTypeMapping = new Dictionary<TMPFontType, string>
	{
		{
			TMPFontType.CupheadFelix_Regular_merged__SDF,
			"CupheadFelix-Regular-merged SDF"
		},
		{
			TMPFontType.CupheadHenriette_A_merged__SDF,
			"CupheadHenriette-A-merged SDF"
		},
		{
			TMPFontType.CupheadMemphis_Medium_merged__SDF,
			"CupheadMemphis-Medium-merged SDF"
		},
		{
			TMPFontType.CupheadPoster_Regular66Cyr_Lat_English99__SDF,
			"CupheadPoster-Regular(Cyr_Lat_English) SDF"
		},
		{
			TMPFontType.CupheadVogue_Bold_merged__SDF,
			"CupheadVogue-Bold-merged SDF"
		},
		{
			TMPFontType.CupheadVogue_ExtraBold_merged__outline__SDF,
			"CupheadVogue-ExtraBold-merged outline SDF"
		},
		{
			TMPFontType.CupheadVogue_ExtraBold_merged__SDF,
			"CupheadVogue-ExtraBold-merged SDF"
		},
		{
			TMPFontType.CupheadVogue_ExtraBold_merged__shadow__SDF,
			"CupheadVogue-ExtraBold-merged shadow SDF"
		},
		{
			TMPFontType.DFBrushRDStd_W7__outline__SDF,
			"DFBrushRDStd-W7 outline SDF"
		},
		{
			TMPFontType.DFBrushRDStd_W7__SDF,
			"DFBrushRDStd-W7 SDF"
		},
		{
			TMPFontType.DFBrushRDStd_W7__shadow__SDF,
			"DFBrushRDStd-W7 shadow SDF"
		},
		{
			TMPFontType.DFBrushSQStd_W5__SDF,
			"DFBrushSQStd-W5 SDF"
		},
		{
			TMPFontType.DSRefinedLetterB__SDF,
			"DSRefinedLetterB SDF"
		},
		{
			TMPFontType.FBBlue__SDF,
			"FBBlue SDF"
		},
		{
			TMPFontType.hyk2gjm__outline__SDF,
			"hyk2gjm outline SDF"
		},
		{
			TMPFontType.hyk2gjm__SDF,
			"hyk2gjm SDF"
		},
		{
			TMPFontType.hyk2gjm__shadow__SDF,
			"hyk2gjm shadow SDF"
		},
		{
			TMPFontType.jpchw00u__SDF,
			"jpchw00u SDF"
		},
		{
			TMPFontType.MComicPRC_Medium__SDF,
			"MComicPRC-Medium SDF"
		},
		{
			TMPFontType.YoonBackjaeM__outline__SDF,
			"YoonBackjaeM outline SDF"
		},
		{
			TMPFontType.YoonBackjaeM__SDF,
			"YoonBackjaeM SDF"
		},
		{
			TMPFontType.YoonBackjaeM__shadow__SDF,
			"YoonBackjaeM shadow SDF"
		},
		{
			TMPFontType.YoonBackjaeM__bold__SDF,
			"YoonBackjaeM bold SDF"
		},
		{
			TMPFontType.rounded_mgenplus_1c_meduim__SDF,
			"rounded-mgenplus-1c-meduim SDF"
		},
		{
			TMPFontType.hisikusa_A__SDF,
			"hisikusa-A SDF"
		},
		{
			TMPFontType.FGPotego__2__SDF,
			"FGPotego 2 SDF"
		},
		{
			TMPFontType.FGPotegoBold__2__SDF,
			"FGPotegoBold 2 SDF"
		},
		{
			TMPFontType.FGNewRetro__SDF,
			"FGNewRetro SDF"
		},
		{
			TMPFontType.ElegantHeiseiMinchoMono_9W__SDF,
			"ElegantHeiseiMinchoMono-9W SDF"
		},
		{
			TMPFontType.FGPotegoBold__2__outline__SDF,
			"FGPotegoBold 2 outline SDF"
		}
	};

	private static Dictionary<FontType, Font> FontCache = new Dictionary<FontType, Font>();

	private static Dictionary<TMPFontType, TMP_FontAsset> TMPFontCache = new Dictionary<TMPFontType, TMP_FontAsset>();

	private static Dictionary<string, Material> TMPMaterialCache = new Dictionary<string, Material>();

	public static Coroutine[] Initialize()
	{
		Array values = Enum.GetValues(typeof(FontType));
		Array values2 = Enum.GetValues(typeof(TMPFontType));
		Coroutine[] array = new Coroutine[values.Length + values2.Length - 2];
		for (int i = 1; i < values.Length; i++)
		{
			FontType fontType = (FontType)values.GetValue(i);
			string bundleName = fontType.ToString();
			Action<Font> completionHandler = (Font font) =>
			{
				FontType key = fontType;
				if (font != null)
				{
					FontCache[key] = font;
				}
			};
			array[i - 1] = AssetBundleLoader.LoadFont(bundleName, GetFilename(fontType), completionHandler);
		}
		for (int num = 1; num < values2.Length; num++)
		{
			TMPFontType fontType2 = (TMPFontType)values2.GetValue(num);
			string bundleName2 = fontType2.ToString();
			Action<UnityEngine.Object[]> completionHandler2 = (UnityEngine.Object[] objects) =>
			{
				if (objects == null)
				{
					return;
				}
				foreach (UnityEngine.Object obj in objects)
				{
					if (obj == null)
					{
						continue;
					}
					if (obj is TMP_FontAsset)
					{
						TMPFontType key = fontType2;
						TMPFontCache[key] = (TMP_FontAsset)obj;
					}
					else if (obj is Material)
					{
						TMPMaterialCache[obj.name] = (Material)obj;
					}
				}
			};
			array[num - 1 + (values.Length - 1)] = AssetBundleLoader.LoadTMPFont(bundleName2, completionHandler2);
		}
		return array;
	}

	public static Font GetFont(FontType fontType)
	{
		if (fontType == FontType.None)
		{
			return null;
		}
		Font font;
		if (FontCache.TryGetValue(fontType, out font))
		{
			return font;
		}
		return null;
	}

	public static TMP_FontAsset GetTMPFont(TMPFontType fontType)
	{
		if (fontType == TMPFontType.None)
		{
			return null;
		}
		TMP_FontAsset fontAsset;
		if (TMPFontCache.TryGetValue(fontType, out fontAsset))
		{
			return fontAsset;
		}
		return null;
	}

	public static Material GetTMPMaterial(string materialName)
	{
		if (string.IsNullOrEmpty(materialName))
		{
			return null;
		}
		Material material;
		if (TMPMaterialCache.TryGetValue(materialName, out material))
		{
			return material;
		}
		return null;
	}

	public static string ConvertAssetNameToEnumName(string assetName)
	{
		assetName = assetName.Replace("-", "_");
		assetName = assetName.Replace(" ", "__");
		assetName = assetName.Replace("(", "66");
		assetName = assetName.Replace(")", "99");
		return assetName;
	}

	public static string GetFilename(FontType fontType)
	{
		return FontTypeMapping[fontType];
	}

	public static string GetFilename(TMPFontType fontType)
	{
		return TMPFontTypeMapping[fontType];
	}

	public static FontType ConvertFontToFontType(Font font)
	{
		return ConvertAssetNameToFontType(font.name);
	}

	private static FontType ConvertAssetNameToFontType(string assetName)
	{
		string value = ConvertAssetNameToEnumName(assetName);
		return (FontType)Enum.Parse(typeof(FontType), value);
	}

	public static TMPFontType ConvertTMPFontAssetToTMPFontType(TMP_FontAsset fontAsset)
	{
		return ConvertAssetNameToTMPFontType(fontAsset.name);
	}

	private static TMPFontType ConvertAssetNameToTMPFontType(string assetName)
	{
		string text = ConvertAssetNameToEnumName(assetName);
		if (text == "rounded_mgenplus_1c_medium__SDF")
		{
			text = "rounded_mgenplus_1c_meduim__SDF";
		}
		return (TMPFontType)Enum.Parse(typeof(TMPFontType), text);
	}
}
