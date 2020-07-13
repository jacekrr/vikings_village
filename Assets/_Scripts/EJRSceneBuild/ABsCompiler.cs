// **** Plik jest czescia projektu VikingsVillageIdle 
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of VikingsVillageIdle project
// **** Copyrights: EJR Sp. z o.o.

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace EJRSceneBuild
{
    

    public class ABsCompiler : MonoBehaviour
    {

        public const BuildTarget OSBuildTarget = BuildTarget.Android;

        //Compile logo scene bundle
        public static void CompileLogoScene()
        {
            AssetDatabase.DeleteAsset("Assets/AssetBundles/logoscene.manifest");
            AssetDatabase.DeleteAsset("Assets/StreamingAssets/logoscene");
            AssetBundleBuild[] buildMap = new AssetBundleBuild[1];
            buildMap[0].assetBundleName = "logoscene";
            string[] assets = new string[1];
            assets[0] = "Assets/Prefabs/logoscene";
            buildMap[0].assetNames = assets;
            AssetBundleManifest abm = BuildPipeline.BuildAssetBundles("Assets/AssetBundles", buildMap, BuildAssetBundleOptions.UncompressedAssetBundle, OSBuildTarget);
            AssetDatabase.ImportAsset("Assets/AssetBundles/logoscene");
            AssetDatabase.Refresh();
            AssetDatabase.MoveAsset("Assets/AssetBundles/logoscene", "Assets/StreamingAssets/logoscene");
        }

        //Compile VV map and prefabs bundle
        public static void CompileVVMainScene()
        {
            AssetDatabase.DeleteAsset("Assets/AssetBundles/VVMain.manifest");
            AssetDatabase.DeleteAsset("Assets/StreamingAssets/VVMain");
            AssetBundleBuild[] buildMap = new AssetBundleBuild[1];
            buildMap[0].assetBundleName = "VVMain";
            string[] assets = new string[1];
            assets[0] = "Assets/Prefabs/VVMain";
            buildMap[0].assetNames = assets;
            AssetBundleManifest abm = BuildPipeline.BuildAssetBundles("Assets/AssetBundles", buildMap, BuildAssetBundleOptions.UncompressedAssetBundle, OSBuildTarget);
            AssetDatabase.ImportAsset("Assets/AssetBundles/VVMain");
            AssetDatabase.Refresh();
            AssetDatabase.MoveAsset("Assets/AssetBundles/VVMain", "Assets/StreamingAssets/VVMain");
        }


    }

}
#endif