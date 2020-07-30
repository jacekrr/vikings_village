// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.


using ClientAbstract;
using EJROrbEngine.SceneObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EJROrbEngine.IdleGame
{
    //base class for all objects that has a cost, level and can provide some production, storage or other service
    public class BaseSceneBuilding : BaseSceneObject
    {
        public UI.SceneBuildingLabel TheLabel;
        public BaseDataAddon TheData { get; protected set; }
        public int Level { get; private set; }                  // current level of building (stored in savegame)
        public float QCost { get; private set; }                // coefficient of cost increasing on levels
        public float QProd { get; private set; }                // coefficient of production increasing on levels
        public int BigUpgrade { get; private set; }             // every this levels, production DOUBLES, 0 means there is no big upgrade at all
        private List<ResourceData> _costLevel1;      // cost of level 1 building in various resources (taken from the config data addon)
        private List<ResourceData> _prodLevel1;      // production of services of level 1 building in various resources (taken from the config data addon)
        private LevelVisibilitySection[] _visSections;
        public string ReadableName
        {
            get
            {
                if (StringsTranslator.HasString("BuildingName" + Type))
                    return StringsTranslator.GetString("BuildingName" + Type);
                else
                    return "<" + Type + ">";
            }
        }
        public string Description
        {
            get
            {
                if (StringsTranslator.HasString("BuildingDesc" + Type))
                    return StringsTranslator.GetString("BuildingDesc" + Type);
                else
                    return "<" + Type + ">";
            }
        }

        //string with readable level information
        public virtual string LevelInfo()
        {
            return String.Format(StringsTranslator.GetString("building_level_info"), Level.ToString());
        }
        //string with readable information of production
        public virtual string ProductionInfo()
        {
            return LevelInfo();
        }

        public override void SaveGame(IGameState gameState)
        {
            base.SaveGame(gameState);
            gameState.SetKey("_bsb" + Type + "_l", Level);
        }
        public override void LoadGame(IGameState gameState)
        {
            base.LoadGame(gameState);
            Level = gameState.GetIntKey("_bsb" + Type + "_l");
            RefreshLevelVisibilitySections();
        }

        
        //get list of costs for building level level (ONLY this level!)
        public List<ResourceData> GetCostOfLevel(int level)
        {
            List<ResourceData> retList = new List<ResourceData>();
            foreach(ResourceData rd in _costLevel1)
            {
                ResourceData costRD = new ResourceData(rd.Type);
                BigInteger cost = rd.CurrentValue * BigInteger.Pow(QCost, level)/* Mathf.Pow(QCost, level )*/;
                costRD.MaximumValue = cost;
                costRD.CurrentValue = cost;
                retList.Add(costRD);
            }
            return retList;
        }
        //get list of costs for building aLevels levels from the current level
        public List<ResourceData> GetCostOfAllLevels(int aLevel)
        {
            List<ResourceData> allCosts = new List<ResourceData>();
            foreach (ResourceData rd in _costLevel1)
            {
                BigInteger costLev = rd.CurrentValue * (BigInteger.Pow(QCost, Level) /*Mathf.Pow(QCost, Level)*/ * (BigInteger.Pow(QCost, aLevel) /*Mathf.Pow(QCost, aLevel)*/ - 1)) / (QCost - 1);
                ResourceData costRes = new ResourceData(rd.Type);
                costRes.MaximumValue = costLev;
                costRes.CurrentValue = costLev;
                allCosts.Add(costRes);
            }

            /*
             * //iterational algorythm
                for (int i = 0; i < aLevel; i++)
            {
                List<ResourceData> cost1 = GetCostOfLevel(Level + i);
                foreach (ResourceData rd in cost1)
                {
                    ResourceData resFromAllCosts = null;
                    foreach (ResourceData resAll in allCosts)
                        if (resAll.Type == rd.Type)
                            resFromAllCosts = resAll;
                    if (resFromAllCosts == null)
                        allCosts.Add(new ResourceData(rd));
                    else
                    {
                        resFromAllCosts.MaximumValue += rd.CurrentValue;
                        resFromAllCosts.CurrentValue += rd.CurrentValue;
                    }
                }
            }*/
            return allCosts;
        }
        //get list of production every second. For Storages production means storage capacity added to all storage capacity
        public List<ResourceData> GetProductionOnLevel(int level)
        {
            List<ResourceData> retList = new List<ResourceData>();
            foreach (ResourceData rd in _prodLevel1)
            {
                ResourceData prodRD = new ResourceData(rd.Type);
                int bigUpgrades = BigUpgrade > 0 ? (level - 1) / BigUpgrade : 0;
                BigInteger cost = rd.CurrentValue * BigInteger.Pow(QProd, level - 1) /*Mathf.Pow(QProd, level - 1)*/ * BigInteger.Pow(2, bigUpgrades) /*Mathf.Pow(2, bigUpgrades)*/;
                prodRD.MaximumValue = cost;
                prodRD.CurrentValue = cost;
                retList.Add(prodRD);
            }
            return retList;
        }

        //how many levels could we buy for given amount of resources
        public int LevelsForResources(List<ResourceData> resList)
        {
            int retVal = int.MaxValue;
            foreach (ResourceData rd in _costLevel1)
            {
                int localLevel = 0;
                ResourceData resOwned = null;
                foreach (ResourceData r in resList)
                    if (r.Type == rd.Type)
                        resOwned = r;
                if(resOwned != null)
                {
                    double innerValcValue = ((resOwned.CurrentValue * (QCost - 1)) / (rd.CurrentValue * BigInteger.Pow(QCost,Level) /* Mathf.Pow(QCost, Level)*/)).ToDouble() + 1;
                    localLevel = (int)Math.Floor(Math.Log(innerValcValue, QCost)) ;
                }
                if (localLevel < 0)
                    retVal = 0;
                else if (localLevel < retVal)
                    retVal = localLevel;
            }
            if (retVal == int.MaxValue)
                return 0;
            return retVal;
        }
        //readable info of production on given level
        public string ProductionToString(int level)
        {
            List<ResourceData> prodList = GetProductionOnLevel(level);
            return ResourceData.ListToString(prodList);
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
                        rd.MaximumValue = new BigInteger(innerTokens[1]);
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
        //upgrade aLevels levels (it will calculate if every level upgrade is possible and do upgrade one by one for every possible level)
        public void LevelUp(int aLevels)
        {
            for(int i = 0; i < aLevels; i++)
            {
                List<ResourceData> cost = GetCostOfLevel(Level);
                if (IdleGameModuleManager.Instance.SubstractResourceIfPossible(cost))
                    Level++;
            }
            RefreshLevelVisibilitySections();
        }
        protected override void OnAwake()
        {

        }

        protected override void OnStart()
        {
            Level = 1;
            _costLevel1 = DecodeResConfigString((string)TheData["costL1"]);
            _prodLevel1 = DecodeResConfigString((string)TheData["prodL1"]);
            QCost = (float)TheData["qCost"];
            QProd = (float)TheData["qProd"];
            BigUpgrade = (int)TheData["bigUpgrade"];
            TheLabel = gameObject.AddComponent<UI.SceneBuildingLabel>();
            TheLabel.TheBuilding = this;
            _visSections = transform.GetComponentsInChildren<LevelVisibilitySection>();
            RefreshLevelVisibilitySections();
        }
        protected override void OnUpdate()
        {

        }
        private void OnSecond()
        {

        }
        private void RefreshLevelVisibilitySections()
        {

            foreach (LevelVisibilitySection lvs in _visSections)
                lvs.RefreshVisibility();
        }
    }

}