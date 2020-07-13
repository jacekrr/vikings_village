// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.


using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EJROrbEngine.EndlessWorld

{
    public class LODReplace : MonoBehaviour
    {
        public int minLOD;
        public int maxLOD;
        public GameObject LODReplaceObject;
        private float _timer, _randomTimerAddon;

        public void RefreshVisibility()
        {
            Vector3 playerPos = GameManager.Instance.ThePlayerController.GetPlayerPosition();
            float dX = playerPos.x - transform.position.x;
            float dZ = playerPos.z - transform.position.z;
            float distanceDoubled = dX * dX + dZ * dZ;
            int currentLOD = CalculateLOD(distanceDoubled);
            bool showReplacer = currentLOD >= minLOD && currentLOD <= maxLOD;
            if (GetComponent<Renderer>() != null)
                GetComponent<Renderer>().enabled = !showReplacer;
            if (GetComponent<Collider>() != null)
                GetComponent<Collider>().enabled = !showReplacer;
            if (LODReplaceObject.GetComponent<Renderer>() != null)
                LODReplaceObject.GetComponent<Renderer>().enabled = showReplacer;
        }

        private void Start()
        {
            _randomTimerAddon = Random.Range(0f, 1f); //randomize in order that other objects do their tasks not at the sime time
            _timer = 1.15f + _randomTimerAddon;
        }
        private void Update()
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0)
            {
                RefreshVisibility();
                _timer = 1.15f + _randomTimerAddon;
            }
        }
        private void OnEnable()
        {
            RefreshVisibility();
            _timer = 1.15f + _randomTimerAddon;
        }
        private int CalculateLOD(float distanceDoubled)
        {
            if (distanceDoubled < SettingsManager.LOD0_SQRDIST)
                return 0;
            else if (distanceDoubled < SettingsManager.LOD1_SQRDIST)
                return 1;
            else if (distanceDoubled < SettingsManager.LOD2_SQRDIST)
                return 2;
            else if (distanceDoubled < SettingsManager.LOD3_SQRDIST)
                return 3;
            else if (distanceDoubled < SettingsManager.LOD4_SQRDIST)
                return 4;
            else if (distanceDoubled < SettingsManager.LOD5_SQRDIST)
                return 5;
            else
                return 6;
        }

    }

   
}


