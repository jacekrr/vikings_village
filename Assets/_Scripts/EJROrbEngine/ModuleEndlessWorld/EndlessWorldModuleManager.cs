// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.

using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using ClientAbstract;

namespace EJROrbEngine.EndlessWorld
{
    //Manages spawning of dynamic world: biom objects with terrains 
    public sealed class EndlessWorldModuleManager : MonoBehaviour, IEngineModule
    {
        public static EndlessWorldModuleManager Instance { get; private set; }
        public MapManager TheMapManager { get; private set; }
        public MapConfig TheMapConfig { get; private set; }         //loaded map configuration
        public Biom CurrentBiom { get; private set; }               //The biom on which player is currently located. It's refreshed often and should be the right way to check on what terrain player currently is.
        public GameObject BiomsRootObject;                          //root gameobject of dynamic bioms objects
        public GameObject AllRootObject;         //The root of whole scene, including bioms, terrains and player. The position of it is being corrected to avoid big scale numbers in objects positions (this is called root correction
        public bool IsLoading { get { return loadingOnce; } }

        private bool firstUpdate = true;
        private Dictionary<int, Biom> _biomsHashed; //all bioms where key is sX + sZ * 65536 which is also calculated in Biom.HashKey and static Biom.BiomHashKey
        private Vector3 _lastManageBiomsPosition;
        private float calc_timeleft;
        //      private bool movementConstraintsDelayed;        //we freeze movement of player's rigidbodu up to moment when first sectors are loaded, it prevents collisions with suddenly appearing terrain
        private Vector3 rootPositionCorrection;         //correction vector for root to prevent large coords of objects

        //lists for sector loader
        private List<Biom> _biomsJustAdded;
        private List<Biom> _biomsJustChangedLOD;
        private List<Biom> _biomsForDelete;
        private bool _wasLoaded;
        private bool loadingOnce;

        private void Awake()
        {
            if (Instance != null && Instance != this)
                throw new System.Exception("Niedozwolone tworzenie kolejnej kopii klasy GameManager");
            Instance = this;
            _wasLoaded = false;
            TheMapManager = new MapManager(EJRConsts.Instance["mapName"]);
        }

        //load saved game from autosave file
        public void OnLoad(IGameState gameState)
        {
            //TheMapManager.LoadMap(false); 
            _wasLoaded = true;
            checkRootCorrection(true);
            reinitAll(false, false);
         //   RefreshCurrentBiom();
        }
        public void CleanupBeforeSave()
        {
            // do nothing
        }
        public void OnSave(IGameState gameState)
        {
            // do nothing
        }
        public void OnNewGame()
        {
            reinitAll(false, false);
        }
        public void OnConfigure()
        {
            // do nothing
        }
        public void OnConfigureObjectRequest(SceneObjects.PrefabTemplate ao)
        {
            // do nothing
        }
        public void checkRootCorrection(bool isShort)
        {
            float correctDistance = isShort ? 250 : 5000;
            Vector3 playerPos = GameManager.Instance.ThePlayerController.GetPlayerPosition();
            if (playerPos.x > correctDistance || playerPos.x < -correctDistance || playerPos.z > correctDistance || playerPos.z < -correctDistance || playerPos.y > correctDistance || playerPos.y < -correctDistance)
            {
                Vector3 correction = new Vector3(-playerPos.x, 0, -playerPos.z);
                AllRootObject.transform.position = new Vector3(AllRootObject.transform.position.x + correction.x, AllRootObject.transform.position.y + correction.y, AllRootObject.transform.position.z + correction.z);
                //GameManager.TheInstance.TheSerializationManager.ChangedRootCorrection(correction);
                rootPositionCorrection += correction;
                if (!isShort)
                    reinitAll(false, false);
                ActiveObjects.ActiveObjectsManager.Instance.ChangedRootCorrection(correction);
            }
        }
        public Vector3 GetRootCorrection()
        {
            return rootPositionCorrection;
        }
        public Vector3 GetBiomPosition(Biom bm)
        {
            return new Vector3(bm.BiomX * Biom.BIOM_SIZE + rootPositionCorrection.x, 0, bm.BiomZ * Biom.BIOM_SIZE + rootPositionCorrection.z);
        }
        public float GetCurrentWorldX()
        {
            return GameManager.Instance.ThePlayerController.GetPlayerPosition().x - rootPositionCorrection.x;
        }
        public float GetCurrentWorldZ()
        {
            return GameManager.Instance.ThePlayerController.GetPlayerPosition().z - rootPositionCorrection.z;
        }
        public float GetCurrentWorldY()
        {
            return GameManager.Instance.ThePlayerController.GetPlayerPosition().y - rootPositionCorrection.y;
        }
        public float GetObjectWorldX(GameObject go)
        {
            return go.transform.position.x - rootPositionCorrection.x;
        }
        public float GetObjectWorldZ(GameObject go)
        {
            return go.transform.position.z - rootPositionCorrection.z;
        }


        void Start()
        {
            firstUpdate = true;
            loadingOnce = false;
            _wasLoaded = false;
            _biomsJustAdded = new List<Biom>();
            _biomsJustChangedLOD = new List<Biom>();
            _biomsForDelete = new List<Biom>();
            XmlDataInfo mapConfigInfo = Utils.LoadXmlAssetFile("prefabs/Maps/" + GameManager.Instance.MapName + "/config/MapConfig.xml", "Configs");
            TheMapConfig = new MapConfig(mapConfigInfo.MainNodeElements);
            calc_timeleft = 0.1f;
            rootPositionCorrection = new Vector3();
            _biomsHashed = new Dictionary<int, Biom>();
            BiomsRootObject = new GameObject();
            BiomsRootObject.name = "_DynamicBioms";
            BiomsRootObject.transform.parent = transform;
            AllRootObject = transform.gameObject;

        }

        void Update()
        {
            if (firstUpdate)
            {
                firstUpdate = false;
                _lastManageBiomsPosition.x = -100;
                _lastManageBiomsPosition.z = -100;
                ReloadGlobalObject();
                StartCoroutine("BiomsLoader");
            }

//            RefreshCurrentBiom();
          
            calc_timeleft -= Time.deltaTime;
            if (calc_timeleft < 0)
            {

                if (_wasLoaded && (Math.Abs(_lastManageBiomsPosition.x - GameManager.Instance.ThePlayerController.GetPlayerPosition().x) > SettingsManager.BIOMSMANAGEMENT_MOVEMENT ||
                   Math.Abs(_lastManageBiomsPosition.z - GameManager.Instance.ThePlayerController.GetPlayerPosition().z) > SettingsManager.BIOMSMANAGEMENT_MOVEMENT))
                {   //sector management starts when player moves at least SECTORMANAGEMENT_MOVEMENT meters and it changed the sector in which the player is
                    MaintenanceBioms(false);
                    checkRootCorrection(false);
                }
                //              if (_sceneGlobalObject != null)
                //                    _sceneGlobalObject.transform.position = new Vector3(_sceneGlobalObject.transform.position.x, 0, _sceneGlobalObject.transform.position.z);
                calc_timeleft = 0.15f;
            }
            MapObjectsManager.Instance.CheckVisibility();
        }
        private void ReloadGlobalObject()
        {
            /*   if(_sceneGlobalObject == null)
                   _sceneGlobalObject = GameObject.FindGameObjectWithTag("GlobalObject");
               if (_sceneGlobalObject != null)
                   GameObject.Destroy(_sceneGlobalObject);
               if (GameManager.TheInstance.TheSceneConfig.GlobalObject != null && GameManager.TheInstance.TheSceneConfig.GlobalObject != "")
               {
                   _sceneGlobalObject = GameManager.TheInstance.ThePrefabPool.GetPrefab(GameManager.TheInstance.TheSceneConfig.GlobalObject);
                   if (_sceneGlobalObject != null)
                   {
                       _sceneGlobalObject.SetActive(true);
                       _sceneGlobalObject.transform.parent = GameManager.TheInstance.TheControlManager.ThePlayer.transform;
                       _sceneGlobalObject.transform.localPosition = Vector3.zero;
                       _sceneGlobalObject.transform.rotation = Quaternion.identity;                   
                   }
               }*/
        }
        //check which bioms should be changed after player moved and other maintenance operations
        private void MaintenanceBioms(bool fullLoad)
        {
            _lastManageBiomsPosition = GameManager.Instance.ThePlayerController.GetPlayerPosition();
            ManageBioms();
            if (_biomsJustAdded.Count > _biomsHashed.Count / 2)
                LoadAllAtOnce(fullLoad);
        }

        private void RefreshCurrentBiom()
        {
            int cBiomX = GetCurrentBiomX();
            int cBiomZ = GetCurrentBiomZ();
            RefreshOneBiom(cBiomX, cBiomZ, cBiomX, cBiomZ);
        }
        private int GetCurrentBiomX()
        {
            return (int)Math.Floor((GameManager.Instance.ThePlayerController.GetPlayerPosition().x - rootPositionCorrection.x) / (1.0f * Biom.BIOM_SIZE));
        }
        private int GetCurrentBiomZ()
        {
            return (int)Math.Floor((GameManager.Instance.ThePlayerController.GetPlayerPosition().z - rootPositionCorrection.z) / (1.0f * Biom.BIOM_SIZE));
        }

        //Find biom given world unrelative coords
        /*   public Biom GetBiomAtWorldPosition(float x, float z)
           {
               int biomX = (int)Math.Floor((x - rootPositionCorrection.x) / (1.0f * Biom.BIOM_SIZE));
               int biomZ = (int)Math.Floor((z - rootPositionCorrection.z) / (1.0f * Biom.BIOM_SIZE));

               return findBiom(biomX, biomZ);
           }
           //get relative position inside a biom, given world unrelative coords
           public Vector3 GetInBiomPositionFromWorldPosition(float x, float z)
           {
               float corrX = x - rootPositionCorrection.x;
               float corrZ = z - rootPositionCorrection.z;
               int sectX = (int)Math.Floor((corrX) / (1.0f * Biom.BIOM_SIZE));
               int sectZ = (int)Math.Floor((corrZ) / (1.0f * Biom.BIOM_SIZE));
               Vector3 ret = new Vector3(corrX - sectX * (1.0f * Biom.BIOM_SIZE), 0, corrZ - sectZ * (1.0f * Biom.BIOM_SIZE));
               return ret;
           }*/

        //after settings change - force reinit all sectors
        //also used when player move more than 5km in one play
        public void reinitAll(bool clearSuperBioms, bool reinitConfig)
        {
            /*           if (reinitConfig)
                       {
                           worldGenerator.InitConfiguration(_gameManager.TheSceneConfig);
                           ReloadGlobalObject();
                       }*/
            foreach (Biom sc in _biomsHashed.Values)
                sc.ReleaseObjects();
            foreach (Biom sc in _biomsForDelete)
                sc.ReleaseObjects();
            foreach (Biom sc in _biomsJustAdded)
                sc.ReleaseObjects();
            foreach (Biom sc in _biomsJustChangedLOD)
                sc.ReleaseObjects();
            _biomsForDelete.Clear();
            _biomsJustAdded.Clear();
            _biomsJustChangedLOD.Clear();
            _biomsHashed.Clear();
            CurrentBiom = null;

            MaintenanceBioms(true);

        }
        //maintenance of one biom, it will be added to maintenance list if needed
        private void RefreshOneBiom(int currentBiomX, int currentBiomZ, int iX, int iZ)
        {
            Biom calcBiom = findBiom(iX, iZ);
            if (calcBiom == null)
            {
                calcBiom = new Biom(TheMapConfig, PrefabPool.Instance, iX, iZ);
                if (currentBiomX == iX && currentBiomZ == iZ)
                {
                    CurrentBiom = calcBiom;
                    InitBiom(calcBiom);
                }
                else
                    _biomsJustAdded.Add(calcBiom);
                _biomsHashed.Add(calcBiom.HashKey, calcBiom);
            }
            else
            {
                if (currentBiomX == iX && currentBiomZ == iZ)
                    CurrentBiom = calcBiom;
                if (calcBiom.CurrentLODLevel != GetBiomsCurrentLOD(calcBiom))
                    _biomsJustChangedLOD.Add(calcBiom);
            }
            //            calcBiom.SetLODLevel((currentBiomX - iX) * (currentBiomX - iX) + (currentBiomZ - iZ) * (currentBiomZ - iZ) < SettingsManager.LOD_DIST_BIOMS ? 0 : 1);
        }

        private int GetBiomsCurrentLOD(Biom bm)
        {
            if (CurrentBiom == null)
                return 1;
            return (CurrentBiom.BiomX - bm.BiomX) * (CurrentBiom.BiomX - bm.BiomX) + (CurrentBiom.BiomZ - bm.BiomZ) * (CurrentBiom.BiomZ - bm.BiomZ) < SettingsManager.LOD_DIST_BIOMS ? 0 : 1;
        }

        //check which biom should be changed after player moved
        private void ManageBioms()
        {
            int cBiomX = GetCurrentBiomX();
            int cBiomZ = GetCurrentBiomZ();
            //check bioms to be added
            for (int iX = cBiomX - SettingsManager.MAX_BIOMS; iX <= cBiomX + SettingsManager.MAX_BIOMS; iX++)
                for (int iZ = cBiomZ - SettingsManager.MAX_BIOMS; iZ <= cBiomZ + SettingsManager.MAX_BIOMS; iZ++)
                    if (iX >= 0 && iX < TheMapConfig.BiomsCountX && iZ >= 0 && iZ < TheMapConfig.BiomsCountZ)
                        RefreshOneBiom(cBiomX, cBiomZ, iX, iZ);
            //check bioms to be removed
            foreach (Biom abiom in _biomsHashed.Values)
                if (abiom.BiomX < cBiomX - SettingsManager.MAX_BIOMS || abiom.BiomX > cBiomX + SettingsManager.MAX_BIOMS || abiom.BiomZ < cBiomZ - SettingsManager.MAX_BIOMS || abiom.BiomZ > cBiomZ + SettingsManager.MAX_BIOMS)
                    if (!_biomsForDelete.Contains(abiom))
                        _biomsForDelete.Add(abiom);
            foreach (Biom abiom in _biomsForDelete)
                if (_biomsHashed.ContainsValue(abiom))
                    _biomsHashed.Remove(abiom.HashKey);
        }
        //find a biom at coords 
        public Biom findBiom(int iX, int iZ)
        {
            if (_biomsHashed == null)
                return null;
            if (_biomsHashed.ContainsKey(Biom.BiomHashKey(iX, iZ)))
                return _biomsHashed[Biom.BiomHashKey(iX, iZ)];
            return null;
        }

        //main maintenance thread - load, change or add sectors from maintenance list
        private IEnumerator BiomsLoader()
        {
            while (true)
            {
                long ticks = DateTime.Now.Ticks;
                long ticksElapsed = 0;
                bool doneSth = true;
                for (int i = 0; i < 8 && ticksElapsed < 15000 && doneSth; i++)
                {
                    doneSth = LoaderOneAction();
                    ticksElapsed = DateTime.Now.Ticks - ticks;
                    //      if (doneSth)
                    //        sceneGenerator.RefreshNavLinks();
                }
                yield return new WaitForSeconds(0.016f);
            }
        }
        //maintenance of bioms at once
        private IEnumerator BiomsLoaderAtOnce()
        {
            loadingOnce = true;
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            while (_biomsJustAdded.Count > 0 || _biomsForDelete.Count > 0 || _biomsJustChangedLOD.Count > 0)
                LoaderOneAction();
            //     sceneGenerator.RefreshNavLinks();
            StopCoroutine("BiomsLoaderAtOnce");
            StartCoroutine("BiomsLoader");

            //      GameManager.TheInstance.TheEngineGUIManager.SetLoadingInfo(false, false);
            /*
            //after some reloads player might be placed at wrong height (for example after changing seed in such way that he will be moved from flat to hill place), so we need do a correction
            float terrainHeight = currentSector.GetBiom().GetTerrain().SampleHeight(new Vector3(controller.transform.position.x, controller.transform.position.y, controller.transform.position.z));
            if(controller.transform.position.y < terrainHeight + 0.01f)  //lift up only if player is lower than terrain, dont do lift down, or he will be forced down to the ground level for example in a house
                controller.transform.position = new Vector3(controller.transform.position.x, terrainHeight + 0.01f, controller.transform.position.z);
                */
            MapObjectsManager.Instance.LoadAllObjectsOnce();
            EJRConsts.Instance.DebugLog("Bioms loaded at once!");
            loadingOnce = false;
        }

        private void LoadAllAtOnce(bool fullLoad)
        {
            if (!loadingOnce)
            {
                //    GameManager.TheInstance.TheEngineGUIManager.SetLoadingInfo(true, fullLoad);
                StopCoroutine("BiomsLoader");
                StartCoroutine("BiomsLoaderAtOnce");
            }
        }
        //single action of SectorsLoader or SectorsLoaderAtOnce, create, change or delete one sector
        private bool LoaderOneAction()
        {
            bool doneSth = false;
            //1. something was added?
            if (_biomsJustAdded.Count > 0)
            {
                InitBiom(_biomsJustAdded[0]);
                _biomsJustAdded.RemoveAt(0);
                doneSth = true;
            }
            //2. something changed LOD ?
            else if (_biomsJustChangedLOD.Count > 0)
            {
                Biom biomCh = _biomsJustChangedLOD[0];
                _biomsJustChangedLOD.RemoveAt(0);
                biomCh.SetLODLevel(GetBiomsCurrentLOD(biomCh));
            }
            //3. something was deleted?
            else if (_biomsForDelete.Count > 0)
            {
                while (_biomsForDelete.Count > 0)
                { //this should be fast, if this will become a bottleneck any in the future, process less objects
                    DestroySector(_biomsForDelete[0]);
                    _biomsForDelete.RemoveAt(0);
                    doneSth = true;
                }
            }

            return doneSth;
        }
        //operations performed on sector when its to be deleted
        private void DestroySector(Biom bm)
        {
            bm.ReleaseObjects();
        }
        //operations performed on sector that is recently added on lists
        private void InitBiom(Biom aNewBiom)
        {
            /*   int sectorSuperBiomX = (int)Math.Floor(aNewBiom.sectorX / (1.0f * SuperBiom.SUPERBIOM_SECTORS_LENGTH));   //coords of superbiom in superbioms area
               int sectorSuperBiomZ = (int)Math.Floor(aNewSeaNewBiomctor.sectorZ / (1.0f * SuperBiom.SUPERBIOM_SECTORS_LENGTH));
               SuperBiom theSuperBiom = GetOrCreateSuperBiom(sectorSuperBiomX, sectorSuperBiomZ);
               aNewBiom.InitSector(theSuperBiom.GetSectorName(aNewSector.sectorX % SuperBiom.SUPERBIOM_SECTORS_LENGTH, aNewSector.sectorZ % SuperBiom.SUPERBIOM_SECTORS_LENGTH),   sceneGenerator);
               aNewBiom.GenerateSceneObjects(_gameManager.TheSceneConfig, this, allRoot, worldGenerator, sceneGenerator);
               aNewBiom.Owner = theSuperBiom;*/
            aNewBiom.InitBiom(BiomsRootObject, this, GetBiomsCurrentLOD(aNewBiom));
            MapObjectsManager.Instance.AddObjectsToVisibilityGroup(aNewBiom);
        }


    }
}

