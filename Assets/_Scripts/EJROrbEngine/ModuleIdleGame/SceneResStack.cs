// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.


using ClientAbstract;
using EJROrbEngine.IdleGame.UI;
using EJROrbEngine.SceneObjects;
using UnityEngine;

namespace EJROrbEngine.IdleGame
{
    public class SceneResStack : BaseSceneObject
    {
        public BaseDataAddon TheData { get; private set; }
        public ResourceData InnerResources { get; private set; }
        public int FillPercent { get { return InnerResources.MaximumValue != BigInteger.Zero ? ((int) Mathf.Round((float)(100 * (InnerResources.CurrentValue / InnerResources.MaximumValue).ToDouble()))) : 0;  } }
        private float _autoGrabTime;
        private LevelVisibilitySection[] _visSections;
        private Color _labelColor;
        private Vector3 _labelDirection;

        public override void SaveGame(IGameState gameState)
        {
            base.SaveGame(gameState);
            gameState.SetKey("_srs_" + _AOComponent.UniqueID, InnerResources.ToSaveGameValue());
        }
        public override void LoadGame(IGameState gameState)
        {
            base.LoadGame(gameState);
            string resStr = gameState.GetStringKey("_srs_" + +_AOComponent.UniqueID);
            InnerResources.FromSaveGameValue(resStr);
            RefreshLevelVisibilitySections();
        }
        public void SetProduction(ResourceData produced)
        {
            InnerResources = new ResourceData(produced);
        }

        //click reaction
        public void OnClick()
        {
            ConsumeStack();
            if ( InnerResources.CurrentValue <= BigInteger.Zero)
            {
                GetComponent<ActiveObjects.ActiveObject>().RemoveFromGame();
            }
        }
        //consume - add to storage
        public void ConsumeStack()
        {
            ResourceData added = IdleGameModuleManager.Instance.AddResourcesToStorage(InnerResources);
            InnerResources -= added;
            GameObject template = IdleUIManager.Instance.ResourceLabelTemplate;
            GameObject theLabel = GameObject.Instantiate(template);
            theLabel.transform.parent = template.transform.parent;
            theLabel.AddComponent<MovingLabel>();
            theLabel.SetActive(true);
            theLabel.transform.position = transform.position;
            theLabel.GetComponent<MovingLabel>().Show("+" + added.CurrentValue.ToSaveGameValue(), _labelColor, _labelDirection);
            RefreshLevelVisibilitySections();
        }
        public override void OnConfigure()
        {
            TheData = GetComponent<PrefabTemplate>().DataObjects.GetDataAddon("idle_res_stacks");
            InnerResources = new ResourceData((string)TheData["resName"]);
            InnerResources.MaximumValue = new BigInteger((string)TheData["defaultAmmount"]);
            InnerResources.CurrentValue = new BigInteger((string)TheData["defaultAmmount"]);
            string[] colors = ((string)TheData["labelColor"]).Split(',');
            string[] dirs = ((string)TheData["labelDirection"]).Split(',');
            if (colors.Length == 3)
                _labelColor = new Color(float.Parse(colors[0]), float.Parse(colors[1]), float.Parse(colors[2]));
            else
                _labelColor = Color.gray;
            if (dirs.Length == 3)
                _labelDirection = new Vector3(float.Parse(dirs[0]), float.Parse(dirs[1]), float.Parse(dirs[2]));
            else
                _labelDirection = Vector3.zero;

            _autoGrabTime = (float)TheData["autoGrab"];
        }

        protected override void OnAwake()
        {
           
        }
        protected override void OnStart()
        {
            _visSections = transform.GetComponentsInChildren<LevelVisibilitySection>();
            RefreshLevelVisibilitySections();
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
        private void RefreshLevelVisibilitySections()
        {
            foreach (LevelVisibilitySection lvs in _visSections)
                lvs.RefreshVisibility();
        }
    }

}