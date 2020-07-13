// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.

using ClientAbstract;
using EJROrbEngine.SceneObjects;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

namespace EJROrbEngine.IdleGame
{
   
    public sealed class IdleGameModuleManager : MonoBehaviour, IEngineModule
    {

        private Dictionary<string, BaseDataAddon> _entities;      // wzorce danych obiektow gry

        public static IdleGameModuleManager Instance { get; private set; }
        private void Awake()
        {
            if (Instance != null && Instance != this)
                throw new System.Exception("Niedozwolone tworzenie kolejnej kopii klasy IdleGameModuleManager");
            Instance = this;
            _entities = new Dictionary<string, BaseDataAddon>();



        }

        public void OnLoad(IGameState gameState)
        {
            // do nothing
        }
        public void OnSave(IGameState gameState)
        {
            // do nothing
        }
        public void OnNewGame()
        {
            // do nothing
        }
        public void OnConfigure()
        {
            if (_entities.Count == 0)
            {
                try
                {
                    if (EJRConsts.Instance["useIdleEngine"] == "true")
                    {
                        //_entities
                        XmlDataInfo entInfo = Utils.LoadXmlAssetFile("data/entities", "entities");
                        if (_entities != null)
                        {
                            foreach (XElement element in entInfo.MainNodeElements)
                            {
                                BaseDataAddon newItem = new BaseDataAddon();
                                newItem.LoadData(entInfo, element);
                                _entities.Add(newItem.Type, newItem);
                            }
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
            BaseDataAddon entData = FindEntity(ao.Type);
            if (entData != null)
            {
                ao.DataObjects.AddDataAddon("entities", entData);
              //  ao.gameObject.AddComponent<SceneEntity>();
            }

        }

        //próbuje dopasować wzorzec przedmiotu do podanego typu i jeśli mu się uda - zwraca go
        public BaseDataAddon FindEntity(string type)
        {
            if (_entities.ContainsKey(type))
                return _entities[type];
            return null;
        }
    }
}

