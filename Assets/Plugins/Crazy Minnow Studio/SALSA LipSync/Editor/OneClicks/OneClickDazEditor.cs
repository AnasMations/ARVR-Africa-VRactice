using UnityEditor;
using UnityEngine;

namespace CrazyMinnow.SALSA.OneClicks
{
	/// <summary>
	/// RELEASE NOTES:
	///		2.5.3 (2021-06-30):
	///			+ Support for DAZ Genesis 8.1 model configurations.
	///			+ Ability to use multiple menu options mapped to different setup scripts.
	///			~ Tweaks to SALSA and EmoteR settings, inline with Tuning video recommendations.
	///			~ Tweaks to legacy DAZ generation configurations to conform with new 8.1 configuration dynamics.
	///			! Fixes for legacy DAZ generations where shapes were not found or one-sided shapes chosen over universal shapes.
	/// 	2.5.2 (2020-12-06):
	/// 		+ Support for Daz2Unity bridge format blendshape names. Blendshape
	/// 			searches modified to use regex searches instead of absolute names.
	/// 	2.5.0.1 (2020-10-03):
	/// 		! typos missing decimal causing overdrive or incorrect frac values.
	/// 	2.5.0 (2020-08-20):
	/// 		REQUIRES: SALSA LipSync Suite v2.5.0+, does NOT work with prior versions.
	/// 		REQUIRES: OneClickBase v2.5.0+
	/// 		+ Support for Eyes module v2.5.0+
	/// 		+ Adds Advanced Dynamics Silence Analyzer to character.
	/// 		~ Tweaks to SALSA settings.
	/// 	2.3.1 (2020-02-04):
	/// 		! removed missing Eyes.eyelidTracking field reference.
	///		2.3.0 (2020-02-02):
	/// 		~ updated to operate with SALSA Suite v2.3.0+
	/// 		NOTE: Does not work with prior versions of SALSA Suite (before v2.3.0)
	/// 	2.1.3 (2019-07-23):
	/// 		! corrected check for prefab code implementation.
	/// 	2.1.2 (2019-07-03):
	/// 		- confirmed operation with Base 2.1.2
	/// 	2.1.1 (2019-06-28):
	/// 		+ 2018.4+ check for prefab and warn > then unpack or cancel.
	/// 	2.1.0:
	/// 		~ convert from editor code to full engine code and move to Plugins.
	/// 	2.0.1-BETA:
	/// 		+ support for Genesis (1) models.
	/// 		+ support for Emotiguy model.
	///		2.0.0-BETA : Initial release.
	/// ==========================================================================
	/// PURPOSE: This script provides simple, simulated lip-sync input to the
	///		Salsa component from text/string values. For the latest information
	///		visit crazyminnowstudio.com.
	/// ==========================================================================
	/// DISCLAIMER: While every attempt has been made to ensure the safe content
	///		and operation of these files, they are provided as-is, without
	///		warranty or guarantee of any kind. By downloading and using these
	///		files you are accepting any and all risks associated and release
	///		Crazy Minnow Studio, LLC of any and all liability.
	/// ==========================================================================
	/// </summary>
	public class OneClickDazEditor : Editor
	{
		public delegate void SalsaOneClickChoice(GameObject gameObject, AudioClip audioClip);
		public static SalsaOneClickChoice SalsaOneClickSetup = OneClickDAZ.Setup;

		public delegate void EyesOneClickChoice(GameObject gameObject);
		public static EyesOneClickChoice EyesOneClickSetup = OneClickDAZEyes.Setup;

		[MenuItem("GameObject/Crazy Minnow Studio/SALSA LipSync/One-Clicks/DAZ/Gen 8.1")]
		public static void OneClickSetup_Gen81()
		{
			SalsaOneClickSetup = OneClickDAZGen81.Setup;
			EyesOneClickSetup = OneClickDAZEyes.Setup;

			OneClickSetup();
		}

		[MenuItem("GameObject/Crazy Minnow Studio/SALSA LipSync/One-Clicks/DAZ/Gen 1,2,3,8")]
		public static void OneClickSetup_GenLegacy()
		{
			SalsaOneClickSetup = OneClickDAZ.Setup;
			EyesOneClickSetup = OneClickDAZEyes.Setup;

			OneClickSetup();
		}

		public static void OneClickSetup()
		{
			GameObject go = Selection.activeGameObject;
			if (go == null)
			{
				Debug.LogWarning(
					"NO OBJECT SELECTED: You must select an object in the scene to apply the OneClick to.");
				return;
			}

#if UNITY_2018_3_OR_NEWER
				if (PrefabUtility.IsPartOfAnyPrefab(go))
				{
					if (EditorUtility.DisplayDialog(
													OneClickBase.PREFAB_ALERT_TITLE,
													OneClickBase.PREFAB_ALERT_MSG,
													"YES", "NO"))
					{
						PrefabUtility.UnpackPrefabInstance(go, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);
						ApplyOneClick(go);
					}
				}
				else
				{
					ApplyOneClick(go);
				}
#else
			ApplyOneClick(go);
#endif
		}

		private static void ApplyOneClick(GameObject go)
		{
			SalsaOneClickSetup(go, AssetDatabase.LoadAssetAtPath<AudioClip>(OneClickBase.RESOURCE_CLIP));
			EyesOneClickSetup(Selection.activeGameObject);
		}
	}
}