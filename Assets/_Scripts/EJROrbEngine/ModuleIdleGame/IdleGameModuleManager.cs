// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.

using ClientAbstract;
using EJROrbEngine.IdleGame.UI;
using EJROrbEngine.SceneObjects;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

namespace EJROrbEngine.IdleGame
{
   
    public sealed class IdleGameModuleManager : MonoBehaviour, IEngineModule
    {
        private Dictionary<string, BaseDataAddon> _producers;            // producers definitions
        private Dictionary<string, BaseDataAddon> _storages;            // resource storages defninitions
        private Dictionary<string, BaseDataAddon> _stubs;               // buildings stubs defninitions
        private Dictionary<string, BaseDataAddon> _resStacks;            // resource stacks definitions
        private Dictionary<string, ResourceData> _cumulatedResources;   // all resource storages cumulated here (no stacks of resources located on map)
        private List<BaseSceneBuilding> _buildingsBuilt;                //all buildings built (on scene)

        private IdleResUI _idleResUI;
        private IdleUIManager _idleUI;

        public static IdleGameModuleManager Instance { get; private set; }
        private void Awake()
        {
            if (Instance != null && Instance != this)
                throw new System.Exception("Niedozwolone tworzenie kolejnej kopii klasy IdleGameModuleManager");
            Instance = this;
            _producers = new Dictionary<string, BaseDataAddon>();
            _storages = new Dictionary<string, BaseDataAddon>();
            _stubs = new Dictionary<string, BaseDataAddon>();
            _resStacks = new Dictionary<string, BaseDataAddon>();
            _cumulatedResources = new Dictionary<string, ResourceData>();
            _idleResUI = gameObject.AddComponent<IdleResUI>();
            _idleUI = gameObject.AddComponent<IdleUIManager>();
            _buildingsBuilt = new List<BaseSceneBuilding>();
            Camera.main.gameObject.AddComponent<IdleCameraController>();
        }

        public void OnLoad(IGameState gameState)
        {
            int resNumber = gameState.GetIntKey("_mstor_numb");
            for(int i = 0; i < resNumber; i++)
            {
                string resName = gameState.GetStringKey("_mstor_nam_r" + i);
                string resStr = gameState.GetStringKey("_mstor_str_r" + i);
                ResourceData rd = new ResourceData(resName);
                rd.FromSaveGameValue(resStr);
                _cumulatedResources.Add(resName, rd);
            }
        }
        public void OnSave(IGameState gameState)
        {
            int index = 0;
            foreach(ResourceData rd in _cumulatedResources.Values)
            {
                gameState.SetKey("_mstor_str_r" + index, rd.ToSaveGameValue());
                gameState.SetKey("_mstor_nam_r" + index, rd.Type);
                index++;
            }
            gameState.SetKey("_mstor_numb", _cumulatedResources.Count);
        }

        public void CleanupBeforeSave()
        {
    
        }
        public void OnNewGame()
        {
            // do nothing
        }
        public void OnConfigure()
        {
            if (_producers.Count == 0)
            {
                try
                {
                    //_producers
                    XmlDataInfo prodInfo = Utils.LoadXmlAssetFile("data/idle_producers", "idle_producers");
                    if (prodInfo != null)
                    {
                        foreach (XElement element in prodInfo.MainNodeElements)
                        {
                            BaseDataAddon newProd = new BaseDataAddon();
                            newProd.LoadData(prodInfo, element);
                            _producers.Add(newProd.Type, newProd);
                        }
                    }
                    //_storages
                    XmlDataInfo storInfo = Utils.LoadXmlAssetFile("data/idle_res_storages", "idle_res_storages");
                    if (storInfo != null)
                    {
                        foreach (XElement element in storInfo.MainNodeElements)
                        {
                            BaseDataAddon newStor = new BaseDataAddon();
                            newStor.LoadData(storInfo, element);
                            _storages.Add(newStor.Type, newStor);
                        }
                    }
                    //_resStacks
                    XmlDataInfo resInfo = Utils.LoadXmlAssetFile("data/idle_res_stacks", "idle_res_stacks");
                    if (resInfo != null)
                    {
                        foreach (XElement element in resInfo.MainNodeElements)
                        {
                            BaseDataAddon newRes = new BaseDataAddon();
                            newRes.LoadData(resInfo, element);
                            _resStacks.Add(newRes.Type, newRes);
                        }
                    }
                    //_stubs
                    XmlDataInfo stubInfo = Utils.LoadXmlAssetFile("data/idle_stubs", "idle_stubs");
                    if (stubInfo != null)
                    {
                        foreach (XElement element in stubInfo.MainNodeElements)
                        {
                            BaseDataAddon newStub = new BaseDataAddon();
                            newStub.LoadData(stubInfo, element);
                            _stubs.Add(newStub.Type, newStub);
                        }
                    }
                }
                catch (System.Exception wyjatek)
                {
                    Debug.LogError("Ogólny wyjątek: " + wyjatek.Message);
                }
                _buildingsBuilt.Clear();             
            }
        }
        public void OnConfigureObjectRequest(SceneObjects.PrefabTemplate ao)
        {
            BaseDataAddon prodData = FindProducer(ao.Type);
            if (prodData != null)
            {
                ao.DataObjects.AddDataAddon("idle_producers", prodData);
                ao.gameObject.AddComponent<SceneProducer>();
                ao.GetComponent<SceneProducer>().OnConfigure();
                _buildingsBuilt.Add(ao.GetComponent<BaseSceneBuilding>());
            }
            BaseDataAddon stoData = FindStorage(ao.Type);
            if (stoData != null)
            {
                ao.DataObjects.AddDataAddon("idle_res_storages", stoData);
                ao.gameObject.AddComponent<SceneStorage>();
                ao.GetComponent<SceneStorage>().OnConfigure();
                _buildingsBuilt.Add(ao.GetComponent<BaseSceneBuilding>());
            }
            BaseDataAddon resData = FindResStack(ao.Type);
            if (resData != null)
            {
                ao.DataObjects.AddDataAddon("idle_res_stacks", resData);
                ao.gameObject.AddComponent<SceneResStack>();
                ao.GetComponent<SceneResStack>().OnConfigure();
            }
            BaseDataAddon stubData = FindStub(ao.Type);
            if (stubData != null)
            {
                ao.DataObjects.AddDataAddon("idle_stubs", stubData);
                ao.gameObject.AddComponent<SceneStub>();
                ao.GetComponent<SceneStub>().OnConfigure();
            }
        }

        //próbuje dopasować wzorzec producenta do podanego typu i jeśli mu się uda - zwraca go
        public BaseDataAddon FindProducer(string type)
        {
            if (_producers.ContainsKey(type))
                return _producers[type];
            return null;
        }
        //próbuje dopasować wzorzec magazynu do podanego typu i jeśli mu się uda - zwraca go
        public BaseDataAddon FindStorage(string type)
        {
            if (_storages.ContainsKey(type))
                return _storages[type];
            return null;
        }
        //próbuje dopasować wzorzec kupki zasobow do podanego typu i jeśli mu się uda - zwraca go
        public BaseDataAddon FindResStack(string type)
        {
            if (_resStacks.ContainsKey(type))
                return _resStacks[type];
            return null;
        }
        //próbuje dopasować wzorzec stuba budynku do podanego typu i jeśli mu się uda - zwraca go
        public BaseDataAddon FindStub(string type)
        {
            if (_stubs.ContainsKey(type))
                return _stubs[type];
            return null;
        }
        //próbuje dopasować wzorzec dowolnego budynku do podanego typu i jeśli mu się uda - zwraca go
        public BaseDataAddon FindAnyBuilding(string type)
        {
            if (_producers.ContainsKey(type))
                return _producers[type];
            if (_storages.ContainsKey(type))
                return _storages[type];
            return null;
        }

        //adds new storage space to cumulated resources, oldstorage is storage before and newstorage is storage now
        public void AddNewStorage(List<ResourceData> oldStorageList, List<ResourceData> newStorageList)
        {
            foreach (ResourceData newStorage in newStorageList)
            {
                ResourceData oldStorage = new ResourceData("0 0");
                if(oldStorageList != null)
                    foreach (ResourceData stor in oldStorageList)
                        if (stor.Type == newStorage.Type)
                            oldStorage = stor;
                if (!_cumulatedResources.ContainsKey(newStorage.Type))
                {
                    ResourceData rd = new ResourceData(newStorage.Type);
                    rd.MaximumValue = newStorage.MaximumValue;
                    _cumulatedResources.Add(newStorage.Type, rd);
                }
                else
                {
                    _cumulatedResources[newStorage.Type].MaximumValue += newStorage.MaximumValue;
                    _cumulatedResources[newStorage.Type].MaximumValue -= oldStorage.MaximumValue;
                }
            }
            RefreshResUI();
        }
       

        //refreshes values of resources on its UI
        public void RefreshResUI()
        {
            RefreshStubsLabels();
            foreach (ResourceData rd in _cumulatedResources.Values)
            {
                _idleResUI.RefreshResData(rd);
            }
        }

        //adds a ResourceData stock of resource to storage, returns rest - ammount of not added resources
        public ResourceData AddResourcesToStorage(ResourceData val)
        {
            ResourceData rest = new ResourceData(val);
            if (_cumulatedResources.ContainsKey(val.Type))
            {
                if (_cumulatedResources[val.Type].CurrentValue + val.CurrentValue > _cumulatedResources[val.Type].MaximumValue)
                    rest.CurrentValue = _cumulatedResources[val.Type].FreeValue;
                _cumulatedResources[val.Type] += val;
            }
            else
                rest.CurrentValue = BigInteger.Zero;
            RefreshResUI();
            return rest;
        }
        //how much levels could we buy for all storaged amount of resources
        public int LevelsForResources(BaseSceneBuilding building)
        {
            List<ResourceData> resList = new List<ResourceData>();
            foreach (ResourceData rd in _cumulatedResources.Values)
                resList.Add(rd);
            return building.LevelsForResources(resList);
        }
        //checks if all resources are sufficient to the costs
        public bool FindIfEnoughResources(List<ResourceData> cost)
        {
            foreach (ResourceData rd in cost)
            {
                if (!_cumulatedResources.ContainsKey(rd.Type))
                    return false;
                if (_cumulatedResources[rd.Type].CurrentValue < rd.CurrentValue)
                    return false;
            }
            return true;
        }
        //if possible - substract given resources from all resources. Returns true if substraction was possible and done)
        public bool SubstractResourceIfPossible(List<ResourceData> cost)
        {
            //first check if all resources are sufficient before start any substraction
            bool enough = FindIfEnoughResources(cost);
            if (!enough)
                return false;
            foreach (ResourceData rd in cost)
            {
                _cumulatedResources[rd.Type].CurrentValue -= rd.CurrentValue;
            }
            RefreshResUI();
            return true;
        }
        //finds if there is a building built on the scene with given type
        public bool IsBuiltBuildingType(string type)
        {
            foreach (BaseSceneBuilding b in _buildingsBuilt)
                if (b.Type == type)
                    return true;
            return false;
        }
        //refreshes stubs labels after built buildings list might be changed (for example after a building is produced)
        public void RefreshStubsLabels()
        {
            SceneStub[] stList = FindObjectsOfType<SceneStub>();
            foreach (SceneStub scst in stList)
                scst.RefreshLabel();
        }

        //produce given building from the stub, remove stub
        public void BuildBuilding(SceneStub stub)
        {            
            GameObject newBuilding = ActiveObjects.ActiveObjectsManager.Instance.CreateAvtiveObject(stub.TheTargetData.Type, stub.transform.position);
            stub.GetComponent<ActiveObjects.ActiveObject>().RemoveFromGame();
            RefreshStubsLabels();
            newBuilding.GetComponent<PrefabTemplate>().Configure();
            if (newBuilding.GetComponent<SceneStorage>() != null)
            {
                AddNewStorage(null, newBuilding.GetComponent<SceneStorage>().GetProductionOnLevel(1));
            }
        }
    }
}

