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

        protected override void OnAwake()
        {
            base.OnAwake();
            TheData = GetComponent<PrefabTemplate>().DataObjects.GetDataAddon("idle_res_storages");
        }
        protected override void OnStart()
        {
            base.OnStart();
            IdleGameModuleManager.Instance.RefreshStorage();

        }
        protected override void OnUpdate()
        {
            base.OnUpdate();
        }
    }

}