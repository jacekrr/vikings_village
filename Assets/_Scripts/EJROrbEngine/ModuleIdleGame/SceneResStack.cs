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
    public class SceneResStack : BaseSceneObject
    {
        public BaseDataAddon TheData { get; private set; }
        public ResourceData InnerResources { get; private set; }
        private float _autoGrabTime;

        public override void SaveGame(IGameState gameState)
        {
            base.SaveGame(gameState);

        }
        public override void LoadGame(IGameState gameState)
        {
            base.LoadGame(gameState);

        }

        //click reaction
        public void OnClick()
        {
            ResourceData added = IdleGameModuleManager.Instance.AddResourcesToStorage(InnerResources);
            InnerResources -= added;
            if(InnerResources.CurrentValue == 0)
            {
                GetComponent<ActiveObjects.ActiveObject>().RemoveFromGame();
            }
        }

        protected override void OnAwake()
        {
            TheData = GetComponent<PrefabTemplate>().DataObjects.GetDataAddon("idle_res_stacks");
            InnerResources = new ResourceData((string)TheData["resName"]);
            InnerResources.MaximumValue = (float)TheData["defaultAmmount"]; 
            InnerResources.CurrentValue = (float)TheData["defaultAmmount"];
            _autoGrabTime = (float)TheData["autoGrab"];
        }
        protected override void OnStart()
        {
           

        }
        protected override void OnUpdate()
        {
            if(_autoGrabTime > 0)
            {
                _autoGrabTime -= Time.deltaTime;
                if(_autoGrabTime <= 0)
                {
                    _autoGrabTime = 0;
                    OnClick();
                }
            }
        }
    }

}