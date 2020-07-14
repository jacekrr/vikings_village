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
        private Dictionary<string, BaseDataAddon> _resStacks;            // resource stacks definitions
        private Dictionary<string, ResourceData> _cumulatedResources;   // all resource storages cumulated here (no stacks of resources located on map)

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
            _resStacks = new Dictionary<string, BaseDataAddon>();
            _cumulatedResources = new Dictionary<string, ResourceData>();
            _idleResUI = gameObject.AddComponent<IdleResUI>();
            _idleUI = gameObject.AddComponent<IdleUIManager>();

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
                            BaseDataAddon newItem = new BaseDataAddon();
                            newItem.LoadData(prodInfo, element);
                            _producers.Add(newItem.Type, newItem);
                        }
                    }
                    //_storages
                    XmlDataInfo storInfo = Utils.LoadXmlAssetFile("data/idle_res_storages", "idle_res_storages");
                    if (storInfo != null)
                    {
                        foreach (XElement element in storInfo.MainNodeElements)
                        {
                            BaseDataAddon newItem = new BaseDataAddon();
                            newItem.LoadData(storInfo, element);
                            _storages.Add(newItem.Type, newItem);
                        }
                    }

                    //_resStacks
                    XmlDataInfo resInfo = Utils.LoadXmlAssetFile("data/idle_res_stacks", "idle_res_stacks");
                    if (resInfo != null)
                    {
                        foreach (XElement element in resInfo.MainNodeElements)
                        {
                            BaseDataAddon newItem = new BaseDataAddon();
                            newItem.LoadData(resInfo, element);
                            _resStacks.Add(newItem.Type, newItem);
                        }
                    }



                }
                catch (System.Exception wyjatek)
                {
                    Debug.LogError("Ogólny wyjątek: " + wyjatek.Message);
                }
            }
        }
        public void OnConfigureObjectRequest(SceneObjects.PrefabTemplate ao)
        {
            BaseDataAddon prodData = FindProducer(ao.Type);
            if (prodData != null)
            {
                ao.DataObjects.AddDataAddon("idle_producers", prodData);
                ao.gameObject.AddComponent<SceneProducer>();
            }
            BaseDataAddon stoData = FindStorage(ao.Type);
            if (stoData != null)
            {
                ao.DataObjects.AddDataAddon("idle_res_storages", stoData);
                ao.gameObject.AddComponent<SceneStorage>();
            }
            BaseDataAddon resData = FindResStack(ao.Type);
            if (resData != null)
            {
                ao.DataObjects.AddDataAddon("idle_res_stacks", resData);
                ao.gameObject.AddComponent<SceneResStack>();
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

        //refresh information about present storages - it will create a storage space for every resource
        public void RefreshStorage()
        {
            SceneStorage[] allStorages = FindObjectsOfType<SceneStorage>();
            Dictionary<string, ResourceData> newCumulatedStorage = new Dictionary<string, ResourceData>();
            foreach (SceneStorage store in allStorages)
            {
                List<ResourceData> storeProduction = store.GetProductionOnLevel(store.Level);
                foreach (ResourceData rd in storeProduction)
                {
                    if (!newCumulatedStorage.ContainsKey(rd.Type))
                        newCumulatedStorage.Add(rd.Type, new ResourceData(rd.Type));
                    newCumulatedStorage[rd.Type].MaximumValue += rd.MaximumValue;
                }
            }
            foreach (ResourceData newData in newCumulatedStorage.Values)
                if (_cumulatedResources.ContainsKey(newData.Type))
                    newData.CurrentValue = _cumulatedResources[newData.Type].CurrentValue;
            _cumulatedResources.Clear();
            foreach (string type in newCumulatedStorage.Keys)
                _cumulatedResources.Add(type, newCumulatedStorage[type]);

            RefreshResUI();
            
        }

        //refreshes values of resources on its UI
        public void RefreshResUI()
        {
            foreach(ResourceData rd in _cumulatedResources.Values)
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
                    rest.CurrentValue = val.CurrentValue - _cumulatedResources[val.Type].FreeValue;
                _cumulatedResources[val.Type] += val;
            }
            RefreshResUI();
            return rest;
        }
    }
}

