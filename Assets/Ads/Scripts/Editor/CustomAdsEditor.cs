using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ManagerAds))]
public class CustomAdsEditor : Editor
{
    private ManagerAds managerAds;
    private void OnEnable()
    {
        managerAds = (ManagerAds)target;
    }
    public override void OnInspectorGUI()
    {
        GUILayout.Space(10);
        EditorGUILayout.BeginVertical();
        managerAds.isUseAdmob = EditorGUILayout.Toggle("Use Admob", managerAds.isUseAdmob);

        if (managerAds.isUseAdmob)
        {
            managerAds.admob = (AdmobAds)EditorGUILayout.ObjectField(managerAds.admob, typeof(AdmobAds), true);
        }

        GUILayout.Space(10);
        managerAds.isUseUnityAds = EditorGUILayout.Toggle("Use Unity Ads", managerAds.isUseUnityAds);

        if (managerAds.isUseUnityAds)
        {
            managerAds.unityAds = (UnityAds)EditorGUILayout.ObjectField(managerAds.unityAds, typeof(UnityAds), true);
        }

        //managerAds.isUseAppLovinAds = EditorGUILayout.Toggle("Use AppLovin Ads", managerAds.isUseAppLovinAds);

        //if (managerAds.isUseAppLovinAds)
        //{
        //    managerAds.appLovinAds = (AppLovinAds)EditorGUILayout.ObjectField(managerAds.appLovinAds, typeof(AppLovinAds), true);
        //}

        GUILayout.Space(10);
        managerAds.isStartBanner = EditorGUILayout.Toggle("Banner In Start", managerAds.isStartBanner);
        GUILayout.Space(10);
        managerAds.ID_MORE = EditorGUILayout.TextField("ID MORE", managerAds.ID_MORE);
        GUILayout.Space(10);
        managerAds.isUseFireBase = EditorGUILayout.Toggle("Use Firebase", managerAds.isUseFireBase);
        GUILayout.Space(10);
        if (GUILayout.Button("Save"))
        {

            SetUpDefineSymbolsForGroup(managerAds.USE_ADMOB, managerAds.isUseAdmob);
            SetUpDefineSymbolsForGroup(managerAds.USE_UNITY_ADS, managerAds.isUseUnityAds);
            SetUpDefineSymbolsForGroup(managerAds.USE_FIREBASE, managerAds.isUseFireBase);
            SetUpDefineSymbolsForGroup(managerAds.USE_APPLOVIN_ADS, managerAds.isUseAppLovinAds);
            AssetDatabase.SaveAssets();
            EditorUtility.SetDirty(managerAds);
        }


        EditorGUILayout.EndVertical();
    }

    private void SetUpDefineSymbolsForGroup(string key, bool enable)
    {
        Debug.Log(enable);
        BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
        string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
        // Only if not defined already.
        if (defines.Contains(key))
        {
            if (enable)
            {
                Debug.LogWarning("Selected build target (" + EditorUserBuildSettings.activeBuildTarget.ToString() + ") already contains <b>" + key + "</b> <i>Scripting Define Symbol</i>.");
                return;
            }
            else
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, (defines.Replace(key, "")));

                return;
            }
        }
        else
        {
            // Append
            if (enable)
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, (defines + ";" + key));
        }


    }
}
