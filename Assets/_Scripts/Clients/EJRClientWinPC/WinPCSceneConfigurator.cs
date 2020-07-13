// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia projektu Gułag obejmującego serię gier o tematyce ucieczki z Gułagu, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of Gulag project including a series of games about escape from Gulag, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.

using ClientAbstract;
using UnityEngine;
using EJROrbEngine;
using UnityEngine.XR;
using EJROrbEngine.FPPGame.UI;

namespace ClientWinPC
{
    public  class WinPCSceneConfigurator : ISceneConfigurator
    {
        public void CreateSceneGameObjects(GameObject glownyObiektSceny)
        {
            //main object
            glownyObiektSceny.AddComponent<GameManager>();
     //       glownyObiektSceny.AddComponent<EJROrbEngine.EndlessWorld.WorldManager>();
            glownyObiektSceny.AddComponent<PrefabPool>();
            glownyObiektSceny.AddComponent<WinPCInputManager>();
//            GameObject prefabPool = new GameObject();
//            prefabPool.name = "_DynamicPrefabPool";
//            prefabPool.transform.parent = glownyObiektSceny.transform;
            glownyObiektSceny.AddComponent<EJROrbEngine.SettingsManager>();
            glownyObiektSceny.AddComponent<EJROrbEngine.ActiveObjects.ActiveObjectsManager>();
            if (EJRConsts.Instance.ModulesToLoad.Contains("EndlessWorld") )
                glownyObiektSceny.AddComponent<EJROrbEngine.EndlessWorld.MapObjectsManager>();


            AssetBundle pakiet = ABsManager.Instance.GetBundle("klientwindows");
            if (pakiet == null)
                 Debug.LogError("Brak pakietu z zasobami dla wersji Windows! ");            
           
            //FPP controller
            //    GameObject kontroler1PSzablon = pakiet.LoadAsset<GameObject>("FPSController");
       /*     if (MainConsts.FppType == FPPTypes.UCC_OPSIVE)
            {

                GameObject kontroler1PSzablon = pakiet.LoadAsset<GameObject>("UCC_Character");
                GameObject kontroler1P = GameObject.Instantiate(kontroler1PSzablon, null);
                kontroler1P.transform.parent = glownyObiektSceny.transform;
                kontroler1P.AddComponent<WinPCPlayerControllerUCC>();
                glownyObiektSceny.GetComponent<GameManager>().ThePlayerController = kontroler1P.GetComponent<BasePlayerController>();

                GameObject CameraSzablon = pakiet.LoadAsset<GameObject>("MainCamera");
                GameObject CameraGO = GameObject.Instantiate(CameraSzablon, null);
                CameraGO.transform.parent = glownyObiektSceny.transform;
                if(GameStarter.StartManager.Instance.UseEnviroSky)
                    CameraGO.GetComponentInChildren<EnviroSkyRendering>().enabled = true;

                GameObject GameSzablon = pakiet.LoadAsset<GameObject>("UCC_Game");
                GameObject GameGO = GameObject.Instantiate(GameSzablon, null);
                GameGO.transform.parent = glownyObiektSceny.transform;
            }
            else if (MainConsts.FppType == FPPTypes.FPP_EJR)
            {
            */
                GameObject kontroler1PSzablon = pakiet.LoadAsset<GameObject>("FPSController");
                GameObject kontroler1P = GameObject.Instantiate(kontroler1PSzablon, null);
                kontroler1P.transform.parent = glownyObiektSceny.transform;
                kontroler1P.AddComponent<WinPCPlayerController>();
                glownyObiektSceny.GetComponent<GameManager>().ThePlayerController = kontroler1P.GetComponent<BasePlayerController>();
                if (EJRConsts.Instance.ModulesToLoad.Contains("EnviroTimeAndWeather"))
                    kontroler1P.GetComponentInChildren<EnviroSkyRendering>().enabled = true;
                GameObject fpp = GameObject.Find("FirstPersonCharacter");
                //Te linie poniżej powinny być niepotrzebne. Jednak w edytorze Unity obiekt z główną kamera spawnowany z AB, czasami traci tag, co powoduje że liczne wywołania Camera.main przestają potem działać
                if (fpp.tag == null || fpp.tag =="Untagged")
                    fpp.tag = "MainCamera";
                if (kontroler1PSzablon.tag == null || kontroler1PSzablon.tag == "Untagged")
                    kontroler1PSzablon.tag = "Player";
        //    }
        /*
            if (GameStarter.StartManager.Instance.UseEnviroSky)
            {
                //EnviroSky
                GameObject enviroSzablon = pakiet.LoadAsset<GameObject>("EnviroSky");
                GameObject.Instantiate(enviroSzablon, null);
                glownyObiektSceny.AddComponent<EJROrbEngine.TimeAndWeather.EnviroDayNightWeather>();
            } else
            {
                GameObject sun = new GameObject();
                sun.name = "Sun";
                sun.AddComponent<Light>().type = LightType.Directional;
                sun.GetComponent<Light>().intensity = 2;
                sun.GetComponent<Light>().shadows = LightShadows.Hard;
                glownyObiektSceny.AddComponent<EJROrbEngine.TimeAndWeather.BaseDayNightWeather>().Sun = sun.GetComponent<Light>();                            
            }
            */
            //UI
            FPPUIManager menedzerUI = glownyObiektSceny.AddComponent<FPPUIManager>();
            menedzerUI.WskaznikWlaczony = Resources.Load<Sprite>("Wskaznik");
            menedzerUI.WskaznikWylaczony = Resources.Load<Sprite>("WskaznikOff");
            
            menedzerUI.WskaznikStrzalCenter = GameObject.Find("WskaznikStrzalCenter").GetComponent<SpriteRenderer>();
            menedzerUI.WskaznikStrzalE = GameObject.Find("WskaznikStrzalE").GetComponent<SpriteRenderer>();
            menedzerUI.WskaznikStrzalN = GameObject.Find("WskaznikStrzalN").GetComponent<SpriteRenderer>();
            menedzerUI.WskaznikStrzalS = GameObject.Find("WskaznikStrzalS").GetComponent<SpriteRenderer>();
            menedzerUI.WskaznikStrzalW = GameObject.Find("WskaznikStrzalW").GetComponent<SpriteRenderer>();
            menedzerUI.KanwaMalegoInfo = GameObject.Find("KanwaMalegoInfo").GetComponent<Canvas>();
            menedzerUI.KanwaOpisu = GameObject.Find("KanwaOpisu").GetComponent<Canvas>();
            /* 
              menedzerUI.KanwaPomocy = GameObject.Find("KanwaPomocy").GetComponent<Canvas>();
              menedzerUI.TekstZlota = GameObject.Find("TekstZloto").GetComponent<Text>();
              menedzerUI.KropkiSzybkosciCzasu = GameObject.Find("ObrazekKropki").GetComponent<Image>();*/
            menedzerUI.InventoryObject = GameObject.Find("TheInventoryObject").GetComponent<InventoryUI>();
            menedzerUI.HealthText = GameObject.Find("HealthText").GetComponent<TextMesh>();
            menedzerUI.FoodText = GameObject.Find("FoodText").GetComponent<TextMesh>();
            menedzerUI.ThirstText = GameObject.Find("ThirstText").GetComponent<TextMesh>();
            menedzerUI.DiseasesText = GameObject.Find("DiseasesText").GetComponent<TextMesh>();
        //    if (MainConsts.FppType == FPPTypes.FPP_EJR)
          //  {
                menedzerUI.HandsController = GameObject.Find("AnimatedHands").GetComponent<FPPHandsController>();
            //}
            //various
            XRSettings.enabled = false;            
        }

       
    }
}