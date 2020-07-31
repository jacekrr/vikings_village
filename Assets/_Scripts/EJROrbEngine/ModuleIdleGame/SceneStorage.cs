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
using UnityEngine;

namespace EJROrbEngine.IdleGame
{
    public class SceneStorage : BaseSceneBuilding
    {

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
            return String.Format(StringsTranslator.GetString("building_current_stor"), Level.ToString(), ProductionToString(Level));
        }
        public override void LevelUp(int aLevels)
        {
            base.LevelUp(aLevels);
            IdleGameModuleManager.Instance.AddNewStorage(GetProductionOnLevel(Level - 1), GetProductionOnLevel(Level));
        }
        public override void OnConfigure()
        {
            TheData = GetComponent<PrefabTemplate>().DataObjects.GetDataAddon("idle_res_storages");
            base.OnConfigure();
        }
        protected override void OnAwake()
        {
          
            base.OnAwake();            
        }
        protected override void OnStart()
        {
            base.OnStart();
            

        }
        protected override void OnUpdate()
        {
            base.OnUpdate();
        }

    }

}