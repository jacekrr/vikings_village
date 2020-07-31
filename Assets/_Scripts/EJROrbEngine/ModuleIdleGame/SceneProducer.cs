// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.


using ClientAbstract;
using EJROrbEngine.ActiveObjects;
using EJROrbEngine.SceneObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EJROrbEngine.IdleGame
{
 

    public class SceneProducer : BaseSceneBuilding
    {
        private float _productionTimer;
        private float _outputInterval;
        private string _outputPrefabName;
        private CIProductionPlace []_externalProductionPlaces;

        public override void SaveGame(IGameState gameState)
        {
            base.SaveGame(gameState);
         
        }
        public override void LoadGame(IGameState gameState)
        {
            base.LoadGame(gameState);
         
        }
        public override string ProductionInfo()
        {
            return String.Format(StringsTranslator.GetString("building_current_prod"), Level.ToString(), ProductionToString(Level), _outputInterval);
        }

        public override void OnConfigure()
        {
            TheData = GetComponent<PrefabTemplate>().DataObjects.GetDataAddon("idle_producers");
            base.OnConfigure();
            _outputInterval = (float)TheData["outputInterval"];
            _outputPrefabName = (string)TheData["outputPrefab"];
        }
        protected override void OnAwake()
        {
            base.OnAwake();            
         
            _externalProductionPlaces = gameObject.GetComponentsInChildren<CIProductionPlace>();
            if (_externalProductionPlaces == null)
                Debug.LogError("No production place in " + gameObject.name);
        }
        protected override void OnStart()
        {
            base.OnStart();
            _productionTimer = _outputInterval;    
        }
        protected override void OnUpdate()
        {
            base.OnUpdate();
            _productionTimer -= Time.deltaTime;
            if (_productionTimer <= 0)
            {
                _productionTimer = _outputInterval;
                OnProduce();
            }
        }

        private void OnProduce()
        {
            // produce
            List<ResourceData> production = GetProductionOnLevel(Level);
            foreach (ResourceData rd in production)
                rd.CurrentValue = rd.MaximumValue = rd.CurrentValue * (int)_outputInterval;
            //find free production place to store the production (separately for every resource produced), if there's not enough of free space for producted resopurces some of them will be canceled
            foreach (ResourceData producedRes in production)
            {
                CIProductionPlace freeProductionPlace = null;
                foreach(CIProductionPlace pp in _externalProductionPlaces)
                    if (pp.GetComponentInChildren<SceneResStack>() == null)
                        freeProductionPlace = pp;
                if(freeProductionPlace != null)  //there;s free place for this resource
                {
                    string resPrefabName = _outputPrefabName.Replace("[RES]", producedRes.Type);
                    GameObject newStack = ActiveObjectsManager.Instance.CreateAvtiveObject(resPrefabName, freeProductionPlace.transform.position);
                    //GameObject newStack = PrefabPool.Instance.GetPrefab(resPrefabName);
                    newStack.transform.parent = freeProductionPlace.transform;
                    newStack.transform.position = freeProductionPlace.transform.position;
                    newStack.SetActive(true);
                    newStack.GetComponent<PrefabTemplate>().Configure();
                    newStack.GetComponent<SceneResStack>().SetProduction(producedRes);
                }
            }
        }
    }

}