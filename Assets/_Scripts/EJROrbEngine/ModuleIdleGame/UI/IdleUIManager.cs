// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.


using ClientAbstract;
using System;
using TMPro;
using UnityEngine;

namespace EJROrbEngine.IdleGame.UI
{
    //Idle module UI Manager, responds on actions, coordinates UI behaviours
    public class IdleUIManager : UIManager
    {
        public new static IdleUIManager Instance { get; private set; }
        public IdleBuildingInfoUI BuildingInfoUI;
        public IdleBuildingBuildUI BuildingBuildUI;
        public GameObject BuildingLabelTemplate;

        private void Awake()
        {
            if (Instance != null && Instance != this)
                throw new Exception("Niedozwolone tworzenie kolejnej kopii klasy IdleUIManager");
            Instance = this;
        }

        protected override void OnStart()
        {
            base.OnStart();
            BuildingInfoUI = FindObjectOfType<IdleBuildingInfoUI>();
            BuildingBuildUI = FindObjectOfType<IdleBuildingBuildUI>();
            BuildingLabelTemplate = GameObject.Find("BuildingLabelTemplate");
            if (BuildingInfoUI == null)
                Debug.LogError("BuildingInfoUI is null in IdleUIManager");
            if (BuildingBuildUI == null)
                Debug.LogError("BuildingBuildUI is null in IdleUIManager");
            if (BuildingLabelTemplate == null)
                Debug.LogError("BuildingLabelTemplate is null in IdleUIManager");
            BuildingInfoUI.Hide();
            BuildingBuildUI.Hide();
            BuildingLabelTemplate.gameObject.SetActive(false);
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
                if (lookedObj != null && !IsAnyUIOpened() )
                {
                    if (lookedObj.GetComponent<SceneResStack>() != null)
                        lookedObj.GetComponent<SceneResStack>().OnClick();
                    if (lookedObj.GetComponent<BaseSceneBuilding>() != null)
                        BuildingInfoUI.Show(lookedObj.GetComponent<BaseSceneBuilding>());
                    if (lookedObj.GetComponent<SceneStub>() != null)
                        BuildingBuildUI.Show(lookedObj.GetComponent<SceneStub>());
                }
            }
            else if (action == LogicalAction.MoveLeft)
                Camera.main.gameObject.GetComponent<IdleCameraController>().OnCameraMove(new Vector3(1, 0, 0));
            else if (action == LogicalAction.MoveRight)
                Camera.main.gameObject.GetComponent<IdleCameraController>().OnCameraMove(new Vector3(-1, 0, 0));
            else if(action == LogicalAction.MoveForward)
                Camera.main.gameObject.GetComponent<IdleCameraController>().OnCameraMove(new Vector3(0, 0, -1));
            else if(action == LogicalAction.MoveBackward)
                Camera.main.gameObject.GetComponent<IdleCameraController>().OnCameraMove(new Vector3(0, 0, 1));
            else if(action == LogicalAction.ZoomIn)
                Camera.main.gameObject.GetComponent<IdleCameraController>().OnCameraMove(new Vector3(0, -1, 0));
            else if(action == LogicalAction.ZoomOut)
                Camera.main.gameObject.GetComponent<IdleCameraController>().OnCameraMove(new Vector3(0, 1, 0));
        }
        private bool IsAnyUIOpened()
        {
            return BuildingInfoUI.isActiveAndEnabled || BuildingBuildUI.isActiveAndEnabled;
        }
    }
}