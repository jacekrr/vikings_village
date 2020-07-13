// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.

using UnityEngine;
using System.Collections.Generic;
using System;
using ClientAbstract;

namespace EJROrbEngine.ActiveObjects
{
    public class AOInfo
    {
        public int UniqueID { get; set; }
        public float LastPosX { get; set; }
        public float LastPosY { get; set; }
        public float LastPosZ { get; set; }
        public string PrefabName { get; set; }
        public GameObject CreatedObject { get; set; }
    }
    public class ActiveObjectsManager: MonoBehaviour
    {
        public static ActiveObjectsManager Instance { get; private set; }
        public const float RECALC_TIME = 0.44f;                       //jak często przetwarzać tablicę obiektów [s.]
        public const float ACTIVEOBJECTS_MAXIMUM_VISIBILITY = 60;      //z jakiej odleglosci obiekty mają być widoczne [m.]
        private int _nextUniqueID = 0;
        private Dictionary<int, AOInfo> _activeObjects;             //klucz jest unikalnym identyfikatorem a wartość strukturą informacyjną
        private List<int> _toShow, _toHide;
        private float _timer;

        public void LoadGame(IGameState gameState)
        {
            _nextUniqueID = gameState.GetIntKey("sm_ukey");
            int liczbaRekordow = gameState.GetIntKey("sm_recs");
            for (int i = 0; i < liczbaRekordow; i++)
            {
                int id = gameState.GetIntKey("sm_uid_" + i);
                string aPrefab = gameState.GetStringKey("sm_pr_" + i);
                float posX = gameState.GetFloatKey("sm_px_" + i);
                float posY = gameState.GetFloatKey("sm_py_" + i);
                float posZ = gameState.GetFloatKey("sm_pz_" + i);
                AOInfo ai = new AOInfo() { UniqueID = id, PrefabName = aPrefab, LastPosX = posX, LastPosY = posY, LastPosZ = posZ, CreatedObject = null};
                if(!_activeObjects.ContainsKey(id))
                    _activeObjects.Add(id, ai);
            }
        }
        public void SaveGame(IGameState gameState)
        {
            int currentSaveIndex = 0;
            Vector3 currCorr = EJRConsts.Instance["useEndlessMap"] == "true" ? EndlessWorld.EndlessWorldModuleManager.Instance.GetRootCorrection() : Vector3.zero;
            foreach (int id in _activeObjects.Keys)
            {
                AOInfo info = _activeObjects[id];
                gameState.SetKey("sm_uid_" + currentSaveIndex, info.UniqueID);
                gameState.SetKey("sm_pr_" + currentSaveIndex, info.PrefabName);
                if(info.CreatedObject != null)
                {
                    gameState.SetKey("sm_px_" + currentSaveIndex, info.CreatedObject.transform.position.x - currCorr.x);
                    gameState.SetKey("sm_py_" + currentSaveIndex, info.CreatedObject.transform.position.y - currCorr.y);
                    gameState.SetKey("sm_pz_" + currentSaveIndex, info.CreatedObject.transform.position.z - currCorr.z);
                } else
                {
                    gameState.SetKey("sm_px_" + currentSaveIndex, info.LastPosX - currCorr.x);
                    gameState.SetKey("sm_py_" + currentSaveIndex, info.LastPosY - currCorr.y);
                    gameState.SetKey("sm_pz_" + currentSaveIndex, info.LastPosZ - currCorr.z);
                }
                currentSaveIndex++;
            }
            gameState.SetKey("sm_recs", currentSaveIndex);
            gameState.SetKey("sm_ukey", _nextUniqueID);
        }

        //użyj tej funkcji gdy aktywny przedmiot jest pierwszy raz tworzony, np. przez generator
        public GameObject CreateAvtiveObject(string prefabName, Vector3 newPosition)
        {
            GameObject createdGO = InternalSpawn(prefabName, newPosition);
            ActiveObject createdAO = createdGO.GetComponent<ActiveObject>();
            if (createdAO == null)
                Debug.LogError(prefabName + "nie posiada komponentu AktywnyObiekt");
            else
            {
                createdAO.UniqueID = GetNextUniqueID();
                AOInfo aoi = new AOInfo { UniqueID = createdAO.UniqueID, PrefabName = prefabName, LastPosX = 0, LastPosY = 0, LastPosZ = 0, CreatedObject = createdGO };
                _activeObjects.Add(createdAO.UniqueID, aoi);
                createdGO.SetActive(true);
                return createdGO;
            }
            return null;
        }     
        //odrejestrowanie, jest równoważne ze śmiercią obiektu, przestanie być śledzony przez menedżera
        public void RemoveActiveObject(ActiveObject ao)
        {
            if (ao != null && _activeObjects.ContainsKey(ao.UniqueID))
                _activeObjects.Remove(ao.UniqueID);
        }
        //rejestruje przedmiot utworzony przez inną klasę
        public void AddActiveObject(ActiveObject ao)
        {
            ao.UniqueID = GetNextUniqueID();
            AOInfo aoi = new AOInfo { UniqueID = ao.UniqueID, PrefabName = ao.name, LastPosX = ao.transform.position.x, LastPosY = ao.transform.position.y, LastPosZ = ao.transform.position.z, CreatedObject = ao.gameObject };
            _activeObjects.Add(ao.UniqueID, aoi);
        }
        //zwraca informację mówiącą czy na listach menedżera istnieje aktywny przedmiot o podanym identyfikatorze
        public bool ActiveObjectExists(int id)
        {
            return _activeObjects.ContainsKey(id);
        }
        //when root correction is aplied to all objects on the scene, it must be also aplied to invisible object or their saved positions could be invalid
        public void ChangedRootCorrection(Vector3 deltaCor)
        {
            foreach (AOInfo aoi in _activeObjects.Values)
                if (aoi.CreatedObject == null)
                {
                    aoi.LastPosX += deltaCor.x;
                    aoi.LastPosY += deltaCor.y;
                    aoi.LastPosZ += deltaCor.z;
                }
        }

        private void RefreshVisibility()
        {
            _toShow.Clear();
            _toHide.Clear();
            foreach (AOInfo aoi in _activeObjects.Values)
            {
                float objX = aoi.LastPosX;
                float objZ = aoi.LastPosZ;
                if (aoi.CreatedObject != null)
                {
                    objX = aoi.CreatedObject.transform.position.x;
                    objZ = aoi.CreatedObject.transform.position.z;
                }
                float playerWorldX = Camera.main.transform.position.x;
                float playerWorldZ = Camera.main.transform.position.z;
                bool czyPoiwnienBycWidoczny = objX > playerWorldX - ACTIVEOBJECTS_MAXIMUM_VISIBILITY && objX < playerWorldX + ACTIVEOBJECTS_MAXIMUM_VISIBILITY && objZ > playerWorldZ - ACTIVEOBJECTS_MAXIMUM_VISIBILITY && objZ < playerWorldZ + ACTIVEOBJECTS_MAXIMUM_VISIBILITY ;
                if (czyPoiwnienBycWidoczny && aoi.CreatedObject == null)
                   _toShow.Add(aoi.UniqueID);
                else if (!czyPoiwnienBycWidoczny && aoi.CreatedObject != null)
                    _toHide.Add(aoi.UniqueID);
            }
            foreach (int uid in _toShow)
                ShowActiveObject(_activeObjects[uid]);
            foreach (int uid in _toHide)
                HideActiveObject(_activeObjects[uid]);
        }

        private void HideActiveObject(AOInfo aoi)
        {
            aoi.LastPosX = aoi.CreatedObject.transform.position.x;
            aoi.LastPosY = aoi.CreatedObject.transform.position.y;
            aoi.LastPosZ = aoi.CreatedObject.transform.position.z;
            PrefabPool.Instance.ReleasePrefab( aoi.CreatedObject);
            aoi.CreatedObject = null;
        }
        private void ShowActiveObject(AOInfo aoi)
        {
            Vector3 newPosition = new Vector3(aoi.LastPosX, aoi.LastPosY, aoi.LastPosZ);
            if (aoi.PrefabName == null || aoi.PrefabName == "")
                Debug.LogError("aoi.prefabName is null");
            else
            {
                GameObject createdObject = InternalSpawn(aoi.PrefabName, newPosition);
                ActiveObject ao = createdObject.GetComponent<ActiveObject>();
                if (ao == null)
                    Debug.LogError(aoi.PrefabName + " nie posiada AktywnyObiekt");
                else
                {
                    ao.UniqueID = aoi.UniqueID;
                    aoi.CreatedObject = createdObject;
                }
            }
        }
        private int GetNextUniqueID()
        {
            _nextUniqueID++;
//            MenedzerGry.InstancjaMenedzeraGry.ZapiszZmienna("sm_ukey", _nextUniqueID);
            return _nextUniqueID;
        }
        private GameObject InternalSpawn(string prefabName, Vector3 newPosition)
        {
            GameObject createdObject = PrefabPool.Instance.GetPrefab(prefabName);
            if (createdObject.GetComponent<ActiveObject>() == null)
                createdObject.AddComponent<ActiveObject>();
            
            createdObject.transform.parent = transform;
            createdObject.transform.position = newPosition;
            return createdObject;
        }
        private void Start()
        {
            _activeObjects = new Dictionary<int, AOInfo>();
            _toHide = new List<int>();
            _toShow = new List<int>();
            _timer = RECALC_TIME;
        }
        private void Update()
        {
            _timer -= Time.deltaTime;
            if (_timer < 0)
            {
                RefreshVisibility();
                _timer = RECALC_TIME;
            }
        }
        private void Awake()
        {
            if (Instance != null && Instance != this)
                throw new Exception("Niedozwolone tworzenie kolejnej kopii klasy ActiveObjectsManager");
            Instance = this;

        }
    }
}