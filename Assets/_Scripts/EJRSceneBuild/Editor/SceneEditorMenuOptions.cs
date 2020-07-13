// **** Plik jest czescia projektu VikingsVillageIdle 
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of VikingsVillageIdle project
// **** Copyrights: EJR Sp. z o.o.



using UnityEditor;
using UnityEngine;

namespace EJRSceneBuild
{

    //definition of editor menu items
    public class SceneEditorMenuOptions : MonoBehaviour
    {

     
#if UNITY_EDITOR
        // Compile logo scene asset bundles
        [MenuItem("EJR/Compile Logo AB", false, 0)]
        static void CompileLogoAB()
        {
            ABsCompiler.CompileLogoScene();
            Debug.Log("CompileLogoScene finished with success");
        }

        // Compile client asset bundles
        [MenuItem("EJR/Compile VV Main AB", false, 0)]
        static void CompileVVMainAB()
        {
            ABsCompiler.CompileVVMainScene();
            Debug.Log("CompileVVMainScene finished with success");
        }
        // Compile all asset bundles
        [MenuItem("EJR/Compile ALL ABs", false, 13)]
        static void CompileAllABs()
        {
            CompileLogoAB();
            CompileVVMainAB();


            Debug.Log("CompileAllABs finished with success");
        }

#endif
    }
}
