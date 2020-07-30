// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.

using ClientAbstract;
using UnityEngine;

namespace EJROrbEngine.TimeAndWeather
{
   
    public sealed class TimeAndWeatherModuleManager : MonoBehaviour, IEngineModule
    {
        public static TimeAndWeatherModuleManager Instance { get; private set; }
        public BaseDayNightWeather TimeComponent;

        private void Awake()
        {
            if (Instance != null && Instance != this)
                throw new System.Exception("Niedozwolone tworzenie kolejnej kopii klasy TimeAndWeatherModuleManager");
            Instance = this;
            GameObject sun = new GameObject();
            sun.name = "Sun";
            sun.AddComponent<Light>().type = LightType.Directional;
            sun.GetComponent<Light>().intensity = 1f;
            sun.GetComponent<Light>().shadows = LightShadows.Hard;
            TimeComponent = GameManager.Instance.gameObject.AddComponent<BaseDayNightWeather>();
            TimeComponent.Sun = sun.GetComponent<Light>();
            TimeComponent.SecondChanged += GameManager.Instance.OnSecondChanged;
            TimeComponent.MinuteChanged += GameManager.Instance.OnMinuteChanged;
            TimeComponent.HourChanged += GameManager.Instance.OnHourChanged;
        }

        public void OnLoad(IGameState gameState)
        {
            TimeComponent.LoadGame(gameState);
        }
        public void CleanupBeforeSave()
        {
            // do nothing
        }
        public void OnSave(IGameState gameState)
        {
            TimeComponent.SaveGame(gameState);
        }
        public void OnNewGame()
        {
            // do nothing
        }
        public void OnConfigure()
        {
            // do nothing
        }
        public void OnConfigureObjectRequest(SceneObjects.PrefabTemplate ao)
        {
            // do nothing
        }
        
    }
}

