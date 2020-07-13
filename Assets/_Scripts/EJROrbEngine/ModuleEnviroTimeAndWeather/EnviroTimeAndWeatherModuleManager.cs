// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.

using ClientAbstract;
using UnityEngine;

namespace EJROrbEngine.EnviroTimeAndWeather
{
   
    public sealed class EnviroTimeAndWeatherModuleManager : MonoBehaviour, IEngineModule
    {
        public static EnviroTimeAndWeatherModuleManager Instance { get; private set; }
        public EnviroDayNightWeather TimeComponent;
        private void Awake()
        {
            if (Instance != null && Instance != this)
                throw new System.Exception("Niedozwolone tworzenie kolejnej kopii klasy EnviroTimeAndWeatherModuleManager");
            Instance = this;
            AssetBundle pakiet = ABsManager.Instance.FindBundleWithAsset("enviroSky.prefab");
            GameObject enviroSzablon =  pakiet.LoadAsset<GameObject>("EnviroSky");
            GameObject.Instantiate(enviroSzablon, null);
            TimeComponent = GameManager.Instance.gameObject.AddComponent<EnviroDayNightWeather>();
            TimeComponent.SecondChanged += GameManager.Instance.OnSecondChanged;
            TimeComponent.MinuteChanged += GameManager.Instance.OnMinuteChanged;
            TimeComponent.HourChanged += GameManager.Instance.OnHourChanged;
        }

        public void OnLoad(IGameState gameState)
        {
            TimeComponent.LoadGame(gameState);
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

