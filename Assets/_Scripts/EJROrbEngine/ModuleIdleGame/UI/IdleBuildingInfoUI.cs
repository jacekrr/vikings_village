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
    public class IdleBuildingInfoUI : MonoBehaviour
    {
        public TextMeshProUGUI BuildingNameText, BuildingDescriptionText, BuildingCurrentProductionText, BuildingNextBigUpgradeText, BuildingUpgradeInfoText;
        public Button ButtonP1, ButtonP10, ButtonP100, ButtonPMax, ButtonUpgrade, ButtonClose;

        private BaseSceneBuilding _building;
        private Button _selectedButton;
        private int _calulatedMaxLevels;

        public void Show(BaseSceneBuilding building)
        {
            _building = building;
            gameObject.SetActive(true);            
            ButtonPMax.GetComponentInChildren<Text>().text = StringsTranslator.GetString("button_max");
            ButtonUpgrade.GetComponentInChildren<Text>().text = StringsTranslator.GetString("button_upgrade");
            ButtonClose.GetComponentInChildren<Text>().text = StringsTranslator.GetString("button_close");
            RefreshLabels();
            ButtonP1Clicked();
        }
        public void Hide()
        {
            gameObject.SetActive(false);
        }
        
        public void ButtonP1Clicked()
        {
            _selectedButton = ButtonP1;
            RefreshButtons();
        }
        public void ButtonP10Clicked()
        {
            _selectedButton = ButtonP10;
            RefreshButtons();
        }
        public void ButtonP100Clicked()
        {
            _selectedButton = ButtonP100;
            RefreshButtons();
        }
        public void ButtonPMaxClicked()
        {
            _selectedButton = ButtonPMax;
            ButtonPMax.GetComponentInChildren<Text>().text = "+" + (_calulatedMaxLevels > 0 ? _calulatedMaxLevels.ToString() : "1");
            RefreshButtons();
        }
        public void ButtonUpgradeClicked()
        {
            if (_selectedButton == ButtonP1)
                _building.LevelUp(1);
            else if (_selectedButton == ButtonP10)
                _building.LevelUp(10);
            else if (_selectedButton == ButtonP100)
                _building.LevelUp(100);
            else if (_selectedButton == ButtonPMax)
                _building.LevelUp(_calulatedMaxLevels);
            _calulatedMaxLevels = IdleGameModuleManager.Instance.LevelsForResources(_building);
            _building.TheLabel.RefreshLabelText();
            ButtonPMax.GetComponentInChildren<Text>().text = StringsTranslator.GetString("button_max");
            if (_selectedButton == ButtonPMax)
                _selectedButton = ButtonP1;
            RefreshLabels();
            RefreshButtons();
        }
        private void Awake()
        {
            if (BuildingNameText == null)
                Debug.LogError("BuildingNameText is null in IdleBuildingInfoUI");
            if (BuildingDescriptionText == null)
                Debug.LogError("BuildingDescriptionText is null in IdleBuildingInfoUI");
            if (BuildingCurrentProductionText == null)
                Debug.LogError("BuildingCurrentProductionText is null in IdleBuildingInfoUI");
            if (BuildingNextBigUpgradeText == null)
                Debug.LogError("BuildingNextBigUpgradeText is null in IdleBuildingInfoUI");
            if (BuildingUpgradeInfoText == null)
                Debug.LogError("BuildingUpgradeInfoText is null in IdleBuildingInfoUI");
            if (ButtonP1 == null)
                Debug.LogError("ButtonP1 is null in IdleBuildingInfoUI");
            if (ButtonP10 == null)
                Debug.LogError("ButtonP10 is null in IdleBuildingInfoUI");
            if (ButtonP100 == null)
                Debug.LogError("ButtonP100 is null in IdleBuildingInfoUI");
            if (ButtonPMax == null)
                Debug.LogError("ButtonPMax is null in IdleBuildingInfoUI");
            if (ButtonUpgrade == null)
                Debug.LogError("ButtonUpgrade is null in IdleBuildingInfoUI");
            if (ButtonClose == null)
                Debug.LogError("ButtonClose is null in IdleBuildingInfoUI");
        }
 
        private void RefreshButtons()
        {
            _calulatedMaxLevels = IdleGameModuleManager.Instance.LevelsForResources(_building);
            ButtonP1.image.color = _selectedButton == ButtonP1 ? Color.green : Color.red;
            ButtonP10.image.color = _selectedButton == ButtonP10 ? Color.green : Color.red;
            ButtonP100.image.color = _selectedButton == ButtonP100 ? Color.green : Color.red;
            ButtonPMax.image.color = _selectedButton == ButtonPMax ? Color.green : Color.red;
            bool upgradeEnabled = false;
            if (_selectedButton == ButtonP1 && _calulatedMaxLevels >= 1)
                upgradeEnabled = true;
            else if (_selectedButton == ButtonP10 && _calulatedMaxLevels >= 10)
                upgradeEnabled = true;
            else if (_selectedButton == ButtonP100 && _calulatedMaxLevels >= 100)
                upgradeEnabled = true;
            else if (_selectedButton == ButtonPMax && _calulatedMaxLevels >= 1)
                upgradeEnabled = true;
            if (_selectedButton == ButtonP1)
                BuildingUpgradeInfoText.text = string.Format(StringsTranslator.GetString("building_cost1"), ResourceData.ListToString(_building.GetCostOfAllLevels(1)));
            else if (_selectedButton == ButtonP10)
                BuildingUpgradeInfoText.text = string.Format(StringsTranslator.GetString("building_cost"), 10, ResourceData.ListToString(_building.GetCostOfAllLevels(10)));
            else if (_selectedButton == ButtonP100)
                BuildingUpgradeInfoText.text = string.Format(StringsTranslator.GetString("building_cost"), 100, ResourceData.ListToString(_building.GetCostOfAllLevels(100)));
            else if (_selectedButton == ButtonPMax)
                BuildingUpgradeInfoText.text = string.Format(StringsTranslator.GetString("building_cost"), _calulatedMaxLevels > 0 ? _calulatedMaxLevels.ToString() : "1", ResourceData.ListToString(_building.GetCostOfAllLevels(_calulatedMaxLevels > 0 ? _calulatedMaxLevels : 1)));
            else BuildingUpgradeInfoText.text = "";
            ButtonUpgrade.image.color = upgradeEnabled ? Color.green : Color.red;
            ButtonUpgrade.enabled = upgradeEnabled;
            
        }
        private void RefreshLabels()
        {
            BuildingNameText.text = _building.ReadableName + " - " + System.String.Format(StringsTranslator.GetString("building_level_info"), _building.Level.ToString()); ;
            BuildingDescriptionText.text = _building.Description;
            BuildingCurrentProductionText.text = _building.ProductionInfo();
            BuildingNextBigUpgradeText.text = string.Format(StringsTranslator.GetString("building_next_upgrade"), _building.BigUpgrade.ToString());
        }

    }
}