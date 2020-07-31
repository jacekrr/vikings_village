// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.

using ClientAbstract;
using EJROrbEngine.SceneObjects;
using System.Collections.Generic;
using UnityEngine;

namespace EJROrbEngine.IdleGame
{
    public class SceneStub : BaseSceneObject
    {
        public BaseDataAddon TheData { get; protected set; }
        public BaseDataAddon TheTargetData { get; protected set; }
        public UI.SceneBuildingLabel TheLabel;
        private List<ResourceData> _cost;                       //cost of building
        private string _targetPrefab;                           //what should be build upon building
        private List<string> _requires;                         //what must be built before if user want to build this

        public override void SaveGame(IGameState gameState)
        {
            base.SaveGame(gameState);
        }
        public override void LoadGame(IGameState gameState)
        {
            base.LoadGame(gameState);
        }   
        //returns true if all required building types are already built
        public bool RequiredBuildinsBuilt()
        {
            bool requiresMet = true;
            foreach (string requiredBuildingType in _requires)
                if (!IdleGameModuleManager.Instance.IsBuiltBuildingType(requiredBuildingType))
                    requiresMet = false;
            return requiresMet;
        }
        //returns true if player has got enough resources to build this building
        public bool EnoughResources()
        {
            return IdleGameModuleManager.Instance.FindIfEnoughResources(_cost);
        }
        public string ReadableName()
        {
            bool requiresMet = RequiredBuildinsBuilt();
            TheLabel.TheLabelText.color = EnoughResources() ? Color.green : Color.red;
            if(requiresMet)
                return string.Format(StringsTranslator.GetString("build_now"),  StringsTranslator.GetString("BuildingName" + TheTargetData.Type));
            else
                return StringsTranslator.GetString("building_unavailable");
        }
        public string Description()
        {
            if(RequiredBuildinsBuilt())
                return StringsTranslator.GetString("BuildingDesc" + TheTargetData.Type);
            else
                return StringsTranslator.GetString("building_unavailable");
        }
        public string BuildCostsAsString()
        {
            return ResourceData.ListToString(_cost);
        }
        public string BuildRequiresAsString()
        {
            string retStr = "";
            foreach (string requiredBuildingType in _requires)
                if (!IdleGameModuleManager.Instance.IsBuiltBuildingType(requiredBuildingType))
                    retStr += (retStr == "" ? "" : ", " ) + StringsTranslator.GetString("BuildingName" + requiredBuildingType);
            return retStr;
        }
        public void RefreshLabel()
        {
            TheLabel.RefreshLabelText();
        }
        //produce what is to be produced and remove itself (remove the whole gameobject)
        public void Build()
        {
            if (IdleGameModuleManager.Instance.SubstractResourceIfPossible(_cost))
            {
                Destroy(TheLabel.TheLabelText.gameObject);
                IdleGameModuleManager.Instance.BuildBuilding(this);
            }
        }
        public override void OnConfigure()
        {
            TheData = GetComponent<PrefabTemplate>().DataObjects.GetDataAddon("idle_stubs");
            _cost = BaseSceneBuilding.DecodeResConfigString((string)TheData["cost"]);
            _targetPrefab = (string)TheData["targetPrefab"];
            _requires = new List<string>();
            if ((string)TheData["requires"] != "")
            {
                string[] requiresArray = ((string)TheData["requires"]).Split(';');
                _requires.AddRange(requiresArray);
            }
            TheTargetData = IdleGameModuleManager.Instance.FindAnyBuilding(_targetPrefab);
        }
        protected override void OnAwake()
        {
           
        }
        protected override void OnStart()
        {
            TheLabel = gameObject.AddComponent<UI.SceneBuildingLabel>();
            TheLabel.TheBuilding = this;
            if ((bool)TheData["autoBuild"] == true)
                Build();
        }
        protected override void OnUpdate()
        {
        }
    }

}