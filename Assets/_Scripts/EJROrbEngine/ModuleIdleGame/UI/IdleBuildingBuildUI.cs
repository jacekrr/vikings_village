// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EJROrbEngine.IdleGame.UI
{

    //building window - leveling/buing/detailed info
    public class IdleBuildingBuildUI : MonoBehaviour
    {
        public TextMeshProUGUI BuildingNameText, BuildingDescriptionText,  BuildingBuildInfoText;
        public Button ButtonBuild, ButtonClose;

        private SceneStub _buildingStub;

        public void Show(SceneStub stub)
        {
            _buildingStub = stub;
            ButtonBuild.GetComponentInChildren<Text>().text = StringsTranslator.GetString("button_build");
            ButtonClose.GetComponentInChildren<Text>().text = StringsTranslator.GetString("button_close");

            gameObject.SetActive(true);            
            RefreshLabels();
            RefreshButtons();
        }
        public void Hide()
        {
            gameObject.SetActive(false);
        }
       
        public void ButtonBuildClicked()
        {
            _buildingStub.Build();
            Hide();
        }
        private void Awake()
        {
            if (BuildingNameText == null)
                Debug.LogError("BuildingNameText is null in IdleBuildingBuildUI");
            if (BuildingDescriptionText == null)
                Debug.LogError("BuildingDescriptionText is null in IdleBuildingBuildUI");
            if (BuildingBuildInfoText == null)
                Debug.LogError("BuildingBuildInfoText is null in IdleBuildingBuildUI");
            if (ButtonBuild == null)
                Debug.LogError("ButtonBuild is null in IdleBuildingBuildUI");
            if (ButtonClose == null)
                Debug.LogError("ButtonClose is null in IdleBuildingInfoUI");
            
        }
 
        private void RefreshButtons()
        {
            bool upgradeEnabled = _buildingStub.EnoughResources() && _buildingStub.RequiredBuildinsBuilt();           
            ButtonBuild.image.color = upgradeEnabled ? Color.green : Color.red;
            ButtonBuild.enabled = upgradeEnabled;
            
        }
        private void RefreshLabels()
        {
            BuildingNameText.text = _buildingStub.ReadableName() ;
            BuildingDescriptionText.text = _buildingStub.Description();
            BuildingBuildInfoText.text = string.Format(StringsTranslator.GetString("building_build_cost"), _buildingStub.BuildCostsAsString());
            if(!_buildingStub.RequiredBuildinsBuilt())
                BuildingBuildInfoText.text += string.Format(StringsTranslator.GetString("building_build_requires"), _buildingStub.BuildRequiresAsString());
        }

    }
}