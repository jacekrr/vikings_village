// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.


using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;


namespace EJROrbEngine
{
    public class ABsManager : MonoBehaviour
    {
        //bundle is being loaded
        public bool IsLoading { get { return _czyLaduje || _pakietyDoZaladowania.Count > 0; } }
        public static ABsManager Instance { get; private set; }

        private Dictionary<string, AssetBundle> _zaladowanePakiety;
        private Queue<string> _pakietyDoZaladowania;
        private Dictionary<string, List<string>> _nazwyZasobow;       //Nazwy wszystkich załadowanych zasobów, jedna list zawiera zasoby z jednego pakietu, a klucz słownika jest nazwą pakietu
        private bool _czyLaduje;

        //make request to load an asset bundle
        public void AddBundleToQueue(string nazwaPakietu)
        {
            if (!IsBundleLoaded(nazwaPakietu) && !_pakietyDoZaladowania.Contains(nazwaPakietu) && !_zaladowanePakiety.ContainsKey(nazwaPakietu))
                _pakietyDoZaladowania.Enqueue(nazwaPakietu);
        }
        //check if bundle with given name is loaded
        public bool IsBundleLoaded(string nazwaPakietu)
        {
            return _zaladowanePakiety.ContainsKey(nazwaPakietu);
        }
        //release uneeded bundle (and it's assets)
        public void ReleaseBundle(string nazwaPakietu)
        {
            if (IsBundleLoaded(nazwaPakietu))
            {
                AssetBundle ab = _zaladowanePakiety[nazwaPakietu];
                _zaladowanePakiety.Remove(nazwaPakietu);
                ab.Unload(true);
                _nazwyZasobow.Remove(nazwaPakietu);
            }
        }
        //get a bundle with given name (returns null if it's not loaded)
        public AssetBundle GetBundle(string nazwa)
        {
            if (IsBundleLoaded(nazwa.ToLower()))
                return _zaladowanePakiety[nazwa.ToLower()];
            return null;
        }

        //get asset bundle that contain an asset with given name (if asset with this name is loaded by more than one bundle, it will return first that will be found that match the name)
        public AssetBundle FindBundleWithAsset(string nazwaZasobu)
        {
            nazwaZasobu = nazwaZasobu.ToLower();
            foreach (string bundName in _nazwyZasobow.Keys)
                if (_nazwyZasobow[bundName].Contains(nazwaZasobu))
                    return _zaladowanePakiety[bundName];
            return null;
        }
        private IEnumerator StartBundleLoading(string nazwaPakietu)
        {
            string uri = Application.streamingAssetsPath + "/" + nazwaPakietu;
            Debug.Log("Start loading AB " + uri);
            UnityWebRequest zlecenie = UnityWebRequestAssetBundle.GetAssetBundle(uri);
            //   zlecenie.chunkedTransfer = false;
            yield return zlecenie.SendWebRequest();
            if (zlecenie.isNetworkError || zlecenie.isHttpError)
            {
                Debug.LogError("Błąd ładowania pakietu " + uri + " błąd: " + zlecenie.error);
            }
            else
            {
                _zaladowanePakiety.Add(nazwaPakietu, DownloadHandlerAssetBundle.GetContent(zlecenie));
                ZaladujZawartoscPakietu(nazwaPakietu);
                Debug.Log("AB " + uri + " has been loaded");
            }
            _czyLaduje = false;
        }


        private void Awake()
        {
            if (Instance != null && Instance != this)
                throw new Exception("Niedozwolone tworzenie kolejnej kopii klasy ABsManager");
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            Init();
        }
        private void Update()
        {
            if (!_czyLaduje && _pakietyDoZaladowania.Count > 0)
            {
                string toLoadName = _pakietyDoZaladowania.Dequeue();
                _czyLaduje = true;
                StartCoroutine(StartBundleLoading(toLoadName));
            }
        }
        private void Init()
        {
            _zaladowanePakiety = new Dictionary<string, AssetBundle>();
            _pakietyDoZaladowania = new Queue<string>();
            _nazwyZasobow = new Dictionary<string, List<string>>();
            _czyLaduje = false;
            foreach (AssetBundle ab in AssetBundle.GetAllLoadedAssetBundles())
            {
                _zaladowanePakiety.Add(ab.name, ab);
                ZaladujZawartoscPakietu(ab.name);
            }
        }


        private void ZaladujZawartoscPakietu(string nazwaPakietu)
        {
            string[] wszystkieNazwy = _zaladowanePakiety[nazwaPakietu].GetAllAssetNames();
            for (int i = 0; i < wszystkieNazwy.Length; i++)
            {
                wszystkieNazwy[i] = wszystkieNazwy[i].Substring(wszystkieNazwy[i].LastIndexOf('/') + 1);
            }
            List<String> listaNazw = new List<string>();
            listaNazw.AddRange(wszystkieNazwy);
            _nazwyZasobow.Add(nazwaPakietu, listaNazw);
        }
    }

}