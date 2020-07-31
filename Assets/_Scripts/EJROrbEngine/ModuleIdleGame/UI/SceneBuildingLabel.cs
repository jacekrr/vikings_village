// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.

using EJROrbEngine.SceneObjects;
using TMPro;
using UnityEngine;

namespace EJROrbEngine.IdleGame.UI
{
    //label of buildings showed on the scene
    public class SceneBuildingLabel : MonoBehaviour
    {
        public TextMeshProUGUI TheLabelText;
        public BaseSceneObject TheBuilding;
        private bool _firstUpdate;
        void Awake()
        {
            TextMeshProUGUI template = IdleUIManager.Instance.BuildingLabelTemplate.GetComponent<TextMeshProUGUI>();
            GameObject labelClone = GameObject.Instantiate(template.gameObject);
            labelClone.SetActive(true);
            labelClone.transform.parent = template.transform.parent;
            TheLabelText = labelClone.GetComponent<TextMeshProUGUI>();
            _firstUpdate = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (_firstUpdate)
            {
                RefreshLabelText();
                CIBuildingLabel labelanchor = transform.GetComponentInChildren<CIBuildingLabel>();
                if (labelanchor != null)
                    TheLabelText.transform.position = labelanchor != null ? labelanchor.transform.position : TheBuilding.transform.position;
                else
                    TheLabelText.gameObject.SetActive(false);
                _firstUpdate = false;
            }
        }

        public void RefreshLabelText()
        {
            if(TheBuilding is BaseSceneBuilding)
                TheLabelText.text = ((BaseSceneBuilding)TheBuilding).ReadableName + " " + ((BaseSceneBuilding)TheBuilding).LevelInfo();
            else if (TheBuilding is SceneStub)
                TheLabelText.text = ((SceneStub)TheBuilding).ReadableName();
            else
                TheLabelText.text = TheBuilding.GetComponent<ActiveObjects.ActiveObject>().RedableName;
        }

    }
}