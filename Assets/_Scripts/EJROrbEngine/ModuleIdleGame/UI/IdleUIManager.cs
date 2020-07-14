// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.


using ClientAbstract;
using System;
using UnityEngine;

namespace EJROrbEngine.IdleGame.UI
{


    public class IdleUIManager : UIManager
    {
        public new static IdleUIManager Instance { get; private set; }
        private void Awake()
        {
            if (Instance != null && Instance != this)
                throw new Exception("Niedozwolone tworzenie kolejnej kopii klasy IdleUIManager");
            Instance = this;
        }

        protected override void OnStart()
        {
            base.OnStart();
           
        }

        protected override void LookTargetChanged()
        {
            base.LookTargetChanged();
         
        }
        protected override void OnUpdate()
        {
            base.OnUpdate();
        }

        public override void OnLogicalAction(LogicalAction action)
        {
            base.OnLogicalAction(action);
            
            if (action == LogicalAction.Interaction)
            {
                Vector3 hitPoint;
                GameObject lookedObj = LastScreenInputLooksOn(1000, out hitPoint);
                if (lookedObj != null)
                {
                    if (lookedObj.GetComponent<SceneResStack>() != null)
                        lookedObj.GetComponent<SceneResStack>().OnClick();
                }
            }
        }
    }
}