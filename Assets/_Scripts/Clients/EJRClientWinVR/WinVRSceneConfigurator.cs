// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia projektu Gułag obejmującego serię gier o tematyce ucieczki z Gułagu, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of Gulag project including a series of games about escape from Gulag, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.


using ClientAbstract;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;


namespace ClientWinVR
{
    public  class WinVRSceneConfigurator : ISceneConfigurator
    {
        public void CreateSceneGameObjects( GameObject glownyObiektSceny)
        {
        /*    AssetBundle pakiet = MenedzerPakietowZasobow.Instancja.PodajPakiet("klientwindows");
            if (pakiet == null)
                Debug.LogError("Brak pakietu z zasobami dla wersji Windows! ");
            TlumaczCiagow.DodajNakladke("WinVR");
            if (glownyObiektSceny.GetComponent<MenedzerGry>() != null)
            {
                MenedzerGry menedzerGry = glownyObiektSceny.GetComponent<MenedzerGry>();
                GameObject szablonSteamVR = pakiet.LoadAsset<GameObject>("[SteamVR]");
                GameObject.Instantiate(szablonSteamVR, null);
                GameObject szablonCameraRig = pakiet.LoadAsset<GameObject>("[CameraRig]");
                GameObject obiektCameraRig = GameObject.Instantiate(szablonCameraRig, null);
                BazowyKontrolerGracza kontrolerGracza = obiektCameraRig.transform.GetComponentInChildren<BazowyKontrolerGracza>();
                menedzerGry.KontrolerGracza = kontrolerGracza.gameObject;
                menedzerGry.ObiektPlecaka = kontrolerGracza.GetComponentInChildren<Plecak>();
                MenedzerUI menedzerUI = menedzerGry.GetComponent<MenedzerUI>();
                menedzerUI.KanwaOpisu = GameObject.Find("KanwaOpisu").GetComponent<Canvas>();
                menedzerUI.KanwaMalegoInfo = GameObject.Find("KanwaMalegoInfo").GetComponent<Canvas>();
                menedzerUI.KanwaPomocy = GameObject.Find("KanwaPomocy").GetComponent<Canvas>();
                menedzerUI.TekstZlota = GameObject.Find("TekstZloto").GetComponent<Text>();
                menedzerUI.KropkiSzybkosciCzasu = GameObject.Find("ObrazekKropki").GetComponent<Image>();
                menedzerGry.gameObject.AddComponent<MenedzerWejsciaWinVR>();
                menedzerGry.gameObject.GetComponent<MenedzerWejsciaWinVR>().KontrolerGracza = kontrolerGracza.gameObject;
                menedzerGry.ObiektPlecaka.Rozszerzony = true;
            } else if(glownyObiektSceny.GetComponent<ScenaStartowa>() != null)
            {
                ScenaStartowa scenaStartowa = glownyObiektSceny.GetComponent<ScenaStartowa>();
                GameObject szablonSteamVR = pakiet.LoadAsset<GameObject>("[SteamVR]");
                GameObject.Instantiate(szablonSteamVR, null);
                GameObject szablonCameraRig = pakiet.LoadAsset<GameObject>("[CameraRig]StartScene");
                GameObject obiektCameraRig = GameObject.Instantiate(szablonCameraRig, null);
                GameObject sztucznyGracz = GameObject.Find("SztucznyGracz");
                obiektCameraRig.transform.parent = sztucznyGracz.transform;
                obiektCameraRig.transform.localPosition = Vector3.zero;
                scenaStartowa.TekstKontynuuj = GameObject.Find("TekstKontynuuj").GetComponent<Text>();
                scenaStartowa.TekstNowy = GameObject.Find("TekstNowy").GetComponent<Text>();
                scenaStartowa.TekstPoziomTrudnosci = GameObject.Find("TekstPoziomTrudnosci").GetComponent<Text>();
                scenaStartowa.TekstPrzegrana = GameObject.Find("TekstPrzegrana").GetComponent<Text>();
                scenaStartowa.TekstZamknij = GameObject.Find("TekstZamknij").GetComponent<Text>();
                scenaStartowa.TekstPoziomJakosci = GameObject.Find("TekstPoziomJakosci").GetComponent<Text>();
                scenaStartowa.TekstKontynuuj.GetComponent<Button>().onClick.AddListener(scenaStartowa.KontyuujGre);
                scenaStartowa.TekstNowy.GetComponent<Button>().onClick.AddListener(scenaStartowa.NowaGra);
                scenaStartowa.TekstPoziomTrudnosci.GetComponent<Button>().onClick.AddListener(scenaStartowa.ZmienPoziomTrudnosci);
                scenaStartowa.TekstPoziomJakosci.GetComponent<Button>().onClick.AddListener(scenaStartowa.ZmienPoziomJakosci);
                scenaStartowa.TekstZamknij.GetComponent<Button>().onClick.AddListener(scenaStartowa.Zamknij);
                scenaStartowa.gameObject.AddComponent<MenedzerWejsciaWinVR>();

            }
            XRSettings.enabled = true;
            glownyObiektSceny.AddComponent<SystemDniaNocyPogody>();
            if (GameObject.Find("Slonce") != null)
                glownyObiektSceny.GetComponent<SystemDniaNocyPogody>().Slonce = GameObject.Find("Slonce").GetComponent<Light>();
                */
        }

       
    }
}