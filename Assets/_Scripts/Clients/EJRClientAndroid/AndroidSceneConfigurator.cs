// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.

using ClientAbstract;
using EJROrbEngine;
using EJROrbEngine.UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

namespace ClientAndroid
{
    public  class AndroidSceneConfigurator : ISceneConfigurator
    {
        public void CreateSceneGameObjects( GameObject glownyObiektSceny)
        {
            //main object
            glownyObiektSceny.AddComponent<GameManager>();
            glownyObiektSceny.AddComponent<AndroidInputManager>();
            glownyObiektSceny.AddComponent<PrefabPool>();
            glownyObiektSceny.AddComponent<EJROrbEngine.SettingsManager>();
            glownyObiektSceny.AddComponent<EJROrbEngine.ActiveObjects.ActiveObjectsManager>();
            if (EJRConsts.Instance.ModulesToLoad.Contains("EndlessMap"))
                glownyObiektSceny.AddComponent<EJROrbEngine.EndlessWorld.MapObjectsManager>();
             
            /*    TlumaczCiagow.DodajNakladke("Android");
                AssetBundle pakiet = MenedzerPakietowZasobow.Instancja.PodajPakiet("klientAndroid");
                if (pakiet == null)
                    Debug.LogError("Brak pakietu z zasobami dla wersji Android! ");
                if (glownyObiektSceny.GetComponent<MenedzerGry>() != null)
                {
                    MenedzerGry menedzerGry = glownyObiektSceny.GetComponent<MenedzerGry>();
                    GameObject kontroler1PSzablon = pakiet.LoadAsset<GameObject>("KontrolerAndroid");
                    GameObject kontroler1P = GameObject.Instantiate(kontroler1PSzablon, null);
                    menedzerGry.KontrolerGracza = kontroler1P;
                    menedzerGry.ObiektPlecaka = kontroler1P.GetComponentInChildren<Plecak>();
                    MenedzerUI menedzerUI = menedzerGry.GetComponent<MenedzerUI>();
                    menedzerUI.KanwaOpisu = GameObject.Find("KanwaOpisu").GetComponent<Canvas>();
                    menedzerUI.KanwaMalegoInfo = GameObject.Find("KanwaMalegoInfo").GetComponent<Canvas>();
                    menedzerUI.KanwaPomocy = GameObject.Find("KanwaPomocy").GetComponent<Canvas>();
                    menedzerUI.TekstZlota = GameObject.Find("TekstZloto").GetComponent<Text>();
                    menedzerUI.KropkiSzybkosciCzasu = GameObject.Find("ObrazekKropki").GetComponent<Image>();
                    menedzerUI.ObiektPlecaka = GameObject.Find("Plecak").GetComponent<Plecak>();
                    menedzerGry.gameObject.AddComponent<MenedzerWejsciaAndroid>();
                    GameObject.Find("TekstPanelPoruszania").GetComponent<Text>().text = TlumaczCiagow.PodajCiag("TekstPanelPoruszania");
                    GameObject.Find("TekstPanelRozgladania").GetComponent<Text>().text = TlumaczCiagow.PodajCiag("TekstPanelRozgladania");
                    GameObject.Find("TekstSkok").GetComponent<Text>().text = TlumaczCiagow.PodajCiag("TekstSkok");
                    GameObject.Find("TekstPlecak").GetComponent<Text>().text = TlumaczCiagow.PodajCiag("TekstPlecak");
                    GameObject.Find("TekstRzemieslnictwo").GetComponent<Text>().text = TlumaczCiagow.PodajCiag("TekstRzemieslnictwo");
                    GameObject.Find("TekstPomoc").GetComponent<Text>().text = TlumaczCiagow.PodajCiag("TekstPomoc");
                    GameObject.Find("TekstMenu").GetComponent<Text>().text = TlumaczCiagow.PodajCiag("TekstMenu");
                    GameObject.Find("TekstWyrzuc").GetComponent<Text>().text = TlumaczCiagow.PodajCiag("TekstWyrzuc");
                    GameObject.Find("ObrazekKropki").GetComponent<Button>().onClick.AddListener( menedzerGry.gameObject.GetComponent<MenedzerWejsciaAndroid>().ZmianaSzybkosciCzasu);
                    GameObject.Find("ObrazekZegar").GetComponent<Button>().onClick.AddListener(menedzerGry.gameObject.GetComponent<MenedzerWejsciaAndroid>().ZmianaSzybkosciCzasu);
                }
                else if(glownyObiektSceny.GetComponent<ScenaStartowa>() != null)
                {
                    glownyObiektSceny.AddComponent<MenedzerWejsciaAndroid>();
                    ScenaStartowa scenaStartowa = glownyObiektSceny.GetComponent<ScenaStartowa>();
                    GameObject kontroler1PSzablon = pakiet.LoadAsset<GameObject>("KontrolerAndroidScenaStartowa");
                    GameObject kontroler1P = GameObject.Instantiate(kontroler1PSzablon, null);
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
                }
                XRSettings.enabled = false;

                glownyObiektSceny.AddComponent<SystemDniaNocyPogody>();
                if (GameObject.Find("Slonce") != null)
                    glownyObiektSceny.GetComponent<SystemDniaNocyPogody>().Slonce = GameObject.Find("Slonce").GetComponent<Light>();
                    */
        }


    }
}