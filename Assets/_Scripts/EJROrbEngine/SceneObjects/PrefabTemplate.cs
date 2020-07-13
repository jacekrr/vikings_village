// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.

using EJROrbEngine.ActiveObjects;
using System.Collections.Generic;
using UnityEngine;
namespace EJROrbEngine.SceneObjects
{
    //Main component that should be added to the scene objects prefabs (along with LODInfo if needed). Type identifies what classes should be aplied. Every class will produce SceneXXX and XXXDataAddon (it will be done via GameConfig or modules managers). DataAddOns are stored in an object of DataAddonContainer.
    public class PrefabTemplate : MonoBehaviour
    {
        public string Type;
        public int LOD;
        public bool isActive;
        public DataAddonContainer DataObjects;

        private bool _isConfigured = false;

        private void Awake()
        {
            DataObjects = new DataAddonContainer();         
            Configure();
        }

        public void Configure()
        {
            if (!_isConfigured)
            {
                _isConfigured = true;
                //it might be null only if object with this component is created directly on the scene, what is incorrect but we still bypass this to allow test and debug uses
                if (GameManager.Instance != null)
                    GameManager.Instance.OnConfigureObjectRequest(this);
                if (isActive && GetComponent<ActiveObject>() == null)
                    gameObject.AddComponent<ActiveObject>();
            }
        }
    }

}