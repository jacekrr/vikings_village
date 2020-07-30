// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.

using ClientAbstract;
using EJROrbEngine.EndlessWorld;
using EJROrbEngine.ActiveObjects;
using UnityEngine;
using EJROrbEngine.Characters;
using EJROrbEngine.TimeAndWeather;
using EJROrbEngine.SceneObjects;
using GameStarter;
using System.Collections.Generic;
using EJROrbEngine.EnviroTimeAndWeather;

namespace EJROrbEngine
{
    //main game manager and holder of other objects
    public sealed class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public bool IsGameLoaded { get; private set; }                                                  //Is the game already loaded from the game state?
        public bool IsGameLoadedAndStarted { get { return IsGameLoaded && !_worldLoadingBioms; } }    //Is the game already loaded from the game state AND bioms and objects are fully initialized?
        public BasePlayerController ThePlayerController { get;  set; }               //Player controller responsible for player movement
        public IGameState TheGameState { get; private set; }
        public bool IsStarted { get; private set; }
        public int LoadingProgress { get { return IsGameLoaded ?  100 : 0; } }
      
        
        public BaseDayNightWeather TheTimeWeatherSystem { get { if (IsModuleLoaded("EnviroTimeAndWeather")) return EnviroTimeAndWeatherModuleManager.Instance.TimeComponent; if (IsModuleLoaded("TimeAndWeather")) return TimeAndWeatherModuleManager.Instance.TimeComponent; return null; }  }
        public string MapName { get; private set; }
        
        private bool _worldLoadingBioms;
        private Dictionary<string, Component> _gameModules;     //loaded game Modules

        public void LoadGame()
        {
            if( IsModuleLoaded("EndlessWorld") )
                          EndlessWorldModuleManager.Instance.TheMapManager.LoadMap(false); //not depending from game state loading, but it's a part of loading process
            TheGameState.LoadGame();
            ActiveObjectsManager.Instance.LoadGame(TheGameState);       //AOM przed WM
            SettingsManager.Instance.LoadGame(TheGameState);            

            foreach (Component module in _gameModules.Values)
                (module as IEngineModule).OnLoad(TheGameState);

            if (EJRConsts.Instance["usePlayerController"] == "true")
                ThePlayerController.LoadGame(TheGameState);

            if(IsModuleLoaded("FPPGame"))
               FPPGame.UI.FPPUIManager.Instance.AfterGameLoad();
            _worldLoadingBioms = IsModuleLoaded("EndlessMap") ? true : false;

            foreach (ABInfo abInfo in EJRConsts.Instance.AssetBundlesToLoad)
            {
                foreach (string objName in abInfo.ObjectsToLoad)
                    GetComponent<PrefabPool>().GetPrefab(objName, false);
            }


            IsGameLoaded = true;
        }
        public void SaveGame()
        {
            foreach (Component module in _gameModules.Values)
                (module as IEngineModule).CleanupBeforeSave();

            BaseSceneObject[] sds = GameObject.FindObjectsOfType<BaseSceneObject>();
            foreach (BaseSceneObject sd in sds)
                sd.SaveGame(TheGameState);

            foreach (Component module in _gameModules.Values)
                (module as IEngineModule).OnSave(TheGameState);
               
            ActiveObjectsManager.Instance.SaveGame(TheGameState);
            if (EJRConsts.Instance["usePlayerController"] == "true")
                ThePlayerController.SaveGame(TheGameState);
            SettingsManager.Instance.SaveGame(TheGameState);
            TheGameState.SaveGame();
        }
        public bool IsModuleLoaded(string name)
        {
            return _gameModules.ContainsKey(name);
        }
        //when PrefabTemplate is instanced
        public void OnConfigureObjectRequest(PrefabTemplate obj)
        {
            foreach (Component module in _gameModules.Values)
                (module as IEngineModule).OnConfigureObjectRequest(obj);
        }
        //player died and the game needs to be restarted
        public void RestartAfterDeath()
        {
            TheGameState.ClearAllKeys();
            TheGameState.SaveGame();
            StartManager.Instance.ReloadGame();
        }
        public void PlayerWasHited(float amnt)
        {
            if (IsModuleLoaded("Characters"))
                CharactersModuleManager.Instance.ThePlayer.GetHit(amnt);
        }
        public void OnSecondChanged(int nothing)
        {
            if (IsModuleLoaded("Characters"))
                CharactersModuleManager.Instance.ThePlayer.OnSecondChange(nothing);
        }
        public void OnMinuteChanged(int currentVal)
        {
            //nothing yet
        }
        public void OnHourChanged(int currentVal)
        {
            if (IsModuleLoaded("Characters"))
                CharactersModuleManager.Instance.ThePlayer.OnHourChange(currentVal);
        }

        private void Start()
        {
            foreach (Component module in _gameModules.Values)
                (module as IEngineModule).OnConfigure();

            TheGameState = GameStarter.StartManager.Instance.Platform.CreateGameStateManager();
           
            IsStarted = true;
        }
        private void Update()
        {
            if (_worldLoadingBioms)
            {
                if (!EndlessWorldModuleManager.Instance.IsLoading && !MapObjectsManager.Instance.IsLoading)
                {
                    _worldLoadingBioms = false;
                    ThePlayerController.LocateOnTerrain(); // position player on the ground just now, after the first biom has been loaded                                                           
                }
            }
        }
        private void Awake()
        {
            if (Instance != null && Instance != this)
                throw new System.Exception("Niedozwolone tworzenie kolejnej kopii klasy GameManager");
            Instance = this;
            IsGameLoaded = false;
            IsStarted = false;
            _worldLoadingBioms = false;

            _gameModules = new Dictionary<string, Component>();
            foreach (string strMod in EJRConsts.Instance.ModulesToLoad)
            {
                System.Type t = System.Type.GetType("EJROrbEngine." + strMod + "." + strMod + "ModuleManager");
                if (t == null)
                    Debug.LogError("Can't load module " + strMod);
                else _gameModules.Add(strMod, gameObject.AddComponent(t));
            }
        }
        private void OnApplicationQuit()
        {
            SaveGame();
        }
     
    }
}