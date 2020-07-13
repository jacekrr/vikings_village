// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.


using ClientAbstract;
using EJROrbEngine.SceneObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EJROrbEngine.IdleGame
{
    //base class for all objects that has a cost, level and can provide some production, storage or other service
    public class BaseSceneBuilding : BaseSceneObject
    {
        public BaseDataAddon TheData { get; protected set; }
        public int Level { get; private set; }                  // current level of building (stored in savegame)
        public float QCost { get; private set; }                // coefficient of cost increasing on levels
        public float QProd { get; private set; }                // coefficient of production increasing on levels

        private List<ResourceData> _costLevel1;      // cost of level 1 building in various resources (taken from the config data addon)
        private List<ResourceData> _prodLevel1;      // production of services of level 1 building in various resources (taken from the config data addon)
        

        public override void SaveGame(IGameState gameState)
        {
            base.SaveGame(gameState);
            gameState.SetKey("_bsb" + Type + "_l", Level);
        }
        public override void LoadGame(IGameState gameState)
        {
            base.LoadGame(gameState);
            Level = gameState.GetIntKey("_bsb" + Type + "_l");
        }

        protected override void OnAwake()
        {
            
        }

        protected override void OnStart()
        {
            _costLevel1 = DecodeResConfigString((string)TheData["costL1"]);
            _prodLevel1 = DecodeResConfigString((string)TheData["prodL1"]);
            QCost = (float)TheData["qCost"];
            QProd = (float)TheData["qProd"];

        }
        protected override void OnUpdate()
        {
           
        }

        public List<ResourceData> GetCostOfLevel(int level)
        {
            List<ResourceData> retList = new List<ResourceData>();
            foreach(ResourceData rd in _costLevel1)
            {
                ResourceData costRD = new ResourceData(rd.Type);
                float cost = rd.CurrentValue * Mathf.Pow(QCost, level);
                costRD.MaximumValue = cost;
                costRD.CurrentValue = cost;
                retList.Add(costRD);
            }
            return retList;
        }
        public List<ResourceData> GetProductionOnLevel(int level)
        {
            List<ResourceData> retList = new List<ResourceData>();
            foreach (ResourceData rd in _prodLevel1)
            {
                ResourceData prodRD = new ResourceData(rd.Type);
                float cost = rd.CurrentValue * Mathf.Pow(QProd, level);
                prodRD.MaximumValue = cost;
                prodRD.CurrentValue = cost;
                retList.Add(prodRD);
            }
            return retList;
        }

        //decode xml confguration resource strings like "Gold=25;Vikings=25;Food=25" to resource data list
        public static List<ResourceData>DecodeResConfigString(string confStr)
        {
            List<ResourceData> retList = new List<ResourceData>();
            string[] resTokens = confStr.Split(';');
            foreach(string oneResToken in resTokens)
            {
                string[] innerTokens = oneResToken.Split('=');
                if (innerTokens.Length == 2)
                {
                    try
                    {
                        ResourceData rd = new ResourceData(innerTokens[0]);
                        rd.MaximumValue = int.Parse(innerTokens[1]);
                        rd.CurrentValue = rd.MaximumValue;
                        retList.Add(rd);
                    }
                    catch (System.Exception)
                    {
                        Debug.LogError("Invalid resource token: " + oneResToken);
                    }
                }
            }
            return retList;
        }

        private void OnSecond()
        {

        }
    }

}