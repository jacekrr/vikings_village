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
using EJROrbEngine.ActiveObjects;
using System.Collections;
using EJROrbEngine.SceneObjects;

namespace EJROrbEngine.EndlessWorld
{
   
    public class MapObjectsManager: MonoBehaviour
    {
        public const float MINIMUM_VISIBILITY_CHECK_DIST = 8;       //this meters number should player move before next visibility check
        public static MapObjectsManager Instance { get; private set; }
        public bool IsLoading { get { return _showObjects.Count > 0 || _hideObjects.Count > 0; } }
        
        //for all visibiliy group key is string created from PosX/KEY_SIZE and PosZ/KEY_SIZE where KEY_SIZE is 16 for LOD0, 32 for LOD1 etc
        private Dictionary<string, VisibilityGroupInfo> _visGroups16;       //256 groups per biom with diameter 16m for LOD0
        private Dictionary<string, VisibilityGroupInfo> _visGroups32;       //64 groups per biom with diameter 32m for LOD1
        private Dictionary<string, VisibilityGroupInfo> _visGroups64;       //16 groups per biom with diameter 64m for LOD2
        private Dictionary<string, VisibilityGroupInfo> _visGroups128;      //4 groups per biom with diameter 128m for LOD3
        private Dictionary<string, VisibilityGroupInfo> _visGroups256;      //1 group per biom with diameter 256m for LOD4
        private Dictionary<string, VisibilityGroupInfo> _visGroups512;      //biom independent group with diameter 512m for LOD5
        private VisibilityGroupInfo _visGroupsInfinite;                     //biom independent group with diameter larger than 512m for LOD6 (alias NOLOD)
        private float[] _timerLOD;
        private Vector3[] _lastPlayerPosForLODCalc;
        private List<MapObjectInfo> _showObjects;
        private List<MapObjectInfo> _hideObjects;
       
        public void AddObjectsToVisibilityGroup(Biom forBiom)
        {
            List<MapObjectInfo> objs = EndlessWorldModuleManager.Instance.TheMapManager.GetBiomMapObjectsData(forBiom.BiomX, forBiom.BiomZ);
            forBiom.AddObjectsToTrack(objs);
            Vector3 biomPos = new Vector3(forBiom.BiomX * Biom.BIOM_SIZE, 0, forBiom.BiomZ * Biom.BIOM_SIZE);
            foreach (MapObjectInfo obj in objs)
            {
                obj.TheBiom = forBiom;
                if (obj.MaxLOD == 6)
                {
                    _visGroupsInfinite.ObjectList.Add(obj);
                    InstantiateMapObject(obj);
                }
                else
                {
                    int size_key = 256;
                    Dictionary<string, VisibilityGroupInfo> list = _visGroups256;
                    CalcKeySizeAndListForLOD(obj.MaxLOD, out size_key, out list);
                    string gkey = ((int)(obj.ObjectPosition.x + biomPos.x) / size_key).ToString() + "_" + ((int)(obj.ObjectPosition.z + biomPos.z) / size_key).ToString();
                    if (!list.ContainsKey(gkey))
                        list.Add(gkey, new VisibilityGroupInfo() { ObjectList = new List<MapObjectInfo>() });
                    list[gkey].ObjectList.Add(obj);
                    obj.MyGroup = list[gkey];
                }
            }

        }

        public void CheckVisibility()
        {
            for (int lod = 0; lod < 6; lod++)
            {
                _timerLOD[lod] -= Time.deltaTime;
                if (_timerLOD[lod] <= 0 && _showObjects.Count == 0 && _hideObjects.Count == 0 )
                {
                    if ((_lastPlayerPosForLODCalc[lod] - GameManager.Instance.ThePlayerController.GetPlayerPosition()).magnitude > MINIMUM_VISIBILITY_CHECK_DIST)
                    {
                        _timerLOD[lod] = SettingsManager.LOD_DELTATIME[lod];
                        if (_lastPlayerPosForLODCalc[lod] == Vector3.zero)
                            _lastPlayerPosForLODCalc[lod] = GameManager.Instance.ThePlayerController.GetPlayerPosition();
                        List<string> gkeysShow = new List<string>();
                        List<string> gkeysHide = new List<string>();
                        int size_key = 256;
                        Dictionary<string, VisibilityGroupInfo> list = _visGroups256;
                        CalcKeySizeAndListForLOD(lod, out size_key, out list);
                        BuildVisGroupsList(size_key, lod, _lastPlayerPosForLODCalc[lod] - EndlessWorldModuleManager.Instance.GetRootCorrection(), false, gkeysHide);
                        BuildVisGroupsList(size_key, lod, GameManager.Instance.ThePlayerController.GetPlayerPosition() - EndlessWorldModuleManager.Instance.GetRootCorrection(), true, gkeysShow);
                        foreach (string key in gkeysShow)
                            if (gkeysHide.Contains(key))
                                gkeysHide.Remove(key);
                        foreach (string key in gkeysHide)
                            ShowOrHideObjects(key, list, false);
                        foreach (string key in gkeysShow)
                            ShowOrHideObjects(key, list, true);
                        _lastPlayerPosForLODCalc[lod] = GameManager.Instance.ThePlayerController.GetPlayerPosition();
                    }
                }

            }
          
        }

        //main maintenance thread - load, change or add object from maintenance lists
        private IEnumerator ObjectsLoader()
        {
            while (true)
            {
                long ticks = DateTime.Now.Ticks;
                long ticksElapsed = 0;
                bool doneSth = true;
                for (int i = 0; i < 512 && ticksElapsed < 11000 && doneSth; i++)
                {
                     doneSth = LoaderOneAction();
                    ticksElapsed = DateTime.Now.Ticks - ticks;
                }
                yield return new WaitForSeconds(0.012f);
            }
        }
        public void LoadAllObjectsOnce()
        {
            if (_showObjects.Count > 0 || _hideObjects.Count > 0)
            {
                foreach (MapObjectInfo obj in _hideObjects)
                {
                    if (/*!_showObjects.Contains(obj) && */obj.SceneObject != null)
                    {
                        PrefabPool.Instance.ReleasePrefab(obj.SceneObject);
                        obj.SceneObject = null;
                    }
                }
                foreach (MapObjectInfo obj in _showObjects)
                    InstantiateMapObject(obj);
                _hideObjects.Clear();
                _showObjects.Clear();
            }
        }
        //single action - showo r hide object
        private bool LoaderOneAction()
        {
            bool doneSth = false;
            //1. something to hide?
            if(_hideObjects.Count > 0)
            {
                MapObjectInfo obj = _hideObjects[0];
                if (obj.SceneObject != null)
                {
                    PrefabPool.Instance.ReleasePrefab(obj.SceneObject);
                    obj.SceneObject = null;
                }
                _hideObjects.RemoveAt(0);
                doneSth = true;
            }
            else if (_showObjects.Count > 0)
            {
                MapObjectInfo obj = _showObjects[0];
                InstantiateMapObject(obj);
                _showObjects.RemoveAt(0);
                doneSth = true;
            }
            return doneSth;
        }
        private void BuildVisGroupsList(int size_key, int lod, Vector3 thePosition, bool show, List<string> vgsKeysToCheck)
        {
            int key_x = (int)thePosition.x / size_key;
            int key_z = (int)thePosition.z / size_key;
            vgsKeysToCheck.Add(key_x.ToString() + "_" + key_z.ToString());
            vgsKeysToCheck.Add((key_x - 1).ToString() + "_" + key_z.ToString());
            vgsKeysToCheck.Add(key_x.ToString() + "_" + (key_z - 1).ToString());
            vgsKeysToCheck.Add((key_x + 1).ToString() + "_" + key_z.ToString());
            vgsKeysToCheck.Add(key_x.ToString() + "_" + (key_z + 1).ToString());
            vgsKeysToCheck.Add((key_x - 1).ToString() + "_" + (key_z - 1).ToString());
            vgsKeysToCheck.Add((key_x - 1).ToString() + "_" + (key_z + 1).ToString());
            vgsKeysToCheck.Add((key_x + 1).ToString() + "_" + (key_z - 1).ToString());
            vgsKeysToCheck.Add((key_x + 1).ToString() + "_" + (key_z + 1).ToString());
        }
        private void ShowOrHideObjects(string gkey, Dictionary<string, VisibilityGroupInfo> list, bool show)
        {
            if (list.ContainsKey(gkey))
            {
                if (show)
                    _showObjects.AddRange(list[gkey].ObjectList);
                else
                    _hideObjects.AddRange(list[gkey].ObjectList);
            }
        }
        private void InstantiateMapObject(MapObjectInfo moi)
        {
            //Debug.Log("InstantiateMapObject " + moi.ObjectName);
            if (moi.SceneObject == null)
            {
                PrefabTemplate templ = PrefabPool.Instance.ShowTemplate(moi.ObjectName).GetComponent<PrefabTemplate>();
                if(templ != null && templ.isActive)
                {
                    if(ShouldSpawnActiveObject(moi))
                    {
                        GameObject createdObject = ActiveObjectsManager.Instance.CreateAvtiveObject(moi.ObjectName, moi.ObjectPosition + EndlessWorldModuleManager.Instance.GetBiomPosition(moi.TheBiom));
                        createdObject.SetActive(true);
                    }
                    moi.MyGroup.ObjectList.Remove(moi);
                }
                else
                {
                    moi.SceneObject = PrefabPool.Instance.GetPrefab(moi.ObjectName);
                    moi.SceneObject.transform.parent = moi.TheBiom.TheMainGO.transform;
                    moi.SceneObject.transform.localPosition = moi.ObjectPosition;
                    moi.SceneObject.transform.localRotation = moi.ObjectRotation;
                    moi.SceneObject.transform.localScale = moi.ObjectScale;
                    moi.SceneObject.SetActive(true);
                }
            }
        }
        //returns true if ao was never spawned yet (during whole game) and thus it should be spawned or returns false if it was spawned and shouldn't be spawned again
        private bool ShouldSpawnActiveObject(MapObjectInfo moi)
        {
            string unique_id = moi.ObjectName + "_" + moi.TheBiom.BiomX + "_" + moi.TheBiom.BiomZ + "_" + moi.ObjectPosition.x.ToString() + "_" + moi.ObjectPosition.z.ToString();
            if (!GameManager.Instance.TheGameState.KeyExists("genid_" + unique_id))
            {
                GameManager.Instance.TheGameState.SetKey("genid_" + unique_id, true);
                return true;
            }
            return false;
        }

        private void CalcKeySizeAndListForLOD(int lod, out int keySize, out Dictionary<string, VisibilityGroupInfo> list)
        {
            keySize = 512;
            list = _visGroups512;
            if (lod == 0)
            {
                keySize = 16;
                list = _visGroups16;
            }
            else if (lod == 1)
            {
                keySize = 32;
                list = _visGroups32;
            }
            else if (lod == 2)
            {
                keySize = 64;
                list = _visGroups64;
            }
            else if (lod == 3)
            {
                keySize = 128;
                list = _visGroups128;
            }
            else if (lod == 4)
            {
                keySize = 256;
                list = _visGroups256;
            }

        }


        private void Start()
        {

            _visGroups16 = new Dictionary<string, VisibilityGroupInfo>();
            _visGroups32 = new Dictionary<string, VisibilityGroupInfo>();
            _visGroups64 = new Dictionary<string, VisibilityGroupInfo>();
            _visGroups128 = new Dictionary<string, VisibilityGroupInfo>();
            _visGroups256 = new Dictionary<string, VisibilityGroupInfo>();
            _visGroups512 = new Dictionary<string, VisibilityGroupInfo>();
            _visGroupsInfinite = new VisibilityGroupInfo() { ObjectList = new List<MapObjectInfo>() };
            _timerLOD = new float[7];
            _lastPlayerPosForLODCalc = new Vector3[7];
            for (int i = 0; i < 7; i++)
            {
                _timerLOD[i] = 0;
                _lastPlayerPosForLODCalc[i] = Vector3.zero;
            }

            _showObjects = new List<MapObjectInfo>();
            _hideObjects = new List<MapObjectInfo>();
            StartCoroutine("ObjectsLoader");
        }
        private void Awake()
        {
            if (Instance != null && Instance != this)
                throw new Exception("Niedozwolone tworzenie kolejnej kopii klasy MapObjectsManager");
            Instance = this;

        }
    }
}