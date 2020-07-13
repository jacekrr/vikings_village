// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.


using EJROrbEngine;
using ClientAbstract;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace GameStarter
{
    //phase of the game
    public enum GamePhase
    {
        //initialization states
        INIT_STATE,                      // nothing was done yet, this is the inital state after play start
        AB_LOADING,                      // there are no ABs loaded yet, hence StartManager opened loading scene and started bundles loading, that may be still loading, internal scene is not loaded and not initialized, 
        AB_LOGO_SCENE,                   // ABs are still being loaded, but logoscene AB has been loaded and the logo scene created, it will be destroyed on PLAYING_GAME phase
        SCENE_NOT_CONFIGURED,            // all ABs are loaded, but scene was not configured yet, it's loading has been started,
        SCENE_NOT_INITIALIZED,           // all ABs are loaded, and scene was configured (main objects created) but scene was not initialized yet, main objects are not iniialized
        SCENE_INITIALIZED_NO_LOAD,       // all ABs are and scene is loaded and initialized (main objects are awaked and started) but game save was not loaded yet (and some objects might be initalizing after game load now)
        //during play states
        PLAYING_GAME,                    // everything is initialized, loaded, working and player may play the game
        PLAYER_DEATH,                    // player died during the game, and now it will be restarted with default state (delete save game)
        LOAD_OTHER_SCENE,                 // player choosen a teleport during the game which will procees to restart it on other scene without deleting the save game
        //error states
        BUNDLES_LOAD_ERROR,              // tried to load asset bundles but failed, no more initialization was done
        SCENE_INIT_ERROR                // all ABs are loaded, but scene initialization failed
    }

    public class StartManager : MonoBehaviour
    {
        public GamePhase CurrentGamePhase { get; private set; }
        public PlatformSelector Platform { get; private set; }
        public static StartManager Instance { get; private set; }

        private ABsManager _absManager;
        private GameObject _mainSceneRoot;
        private Scene _mainScene;
        private ISceneConfigurator _configurator;

        public void ReloadGame()
        {
            SceneManager.LoadScene("ejrLogoScene", LoadSceneMode.Single);
            CurrentGamePhase = GamePhase.SCENE_NOT_CONFIGURED;
            LoadMainScene();
        }
        public string GetGamePhaseInfoText()
        {
            switch (CurrentGamePhase)
            {
                case GamePhase.INIT_STATE:
                    return StringsTranslator.GetString("game_init");
                case GamePhase.AB_LOADING:
                    return StringsTranslator.GetString("game_loadingabs");
                case GamePhase.SCENE_NOT_CONFIGURED:
                    return StringsTranslator.GetString("game_configure");
                case GamePhase.SCENE_NOT_INITIALIZED:
                    return StringsTranslator.GetString("game_initializing");
                case GamePhase.SCENE_INITIALIZED_NO_LOAD:
                    return StringsTranslator.GetString("game_loading") + " " + _mainSceneRoot.GetComponent<GameManager>().LoadingProgress + "%";
                default:
                    return StringsTranslator.GetString("game_inprogress");
            }
        }

        private void Awake()
        {
            CurrentGamePhase = GamePhase.INIT_STATE;
            if (Instance != null && Instance != this)
                Destroy(gameObject);    //This can happen when you start game from mainScene and it will be reloaded after loading bundles. Starter object could be duplicated and the duplicate must be destroyed now.
            else
            {
                Instance = this;
                Platform = GetComponent<PlatformSelector>();
                if (Platform == null)
                    throw new System.Exception("Brak obiektu PlatformSelector");
                _configurator = Platform.CreateSceneConfigurator();
                _mainSceneRoot = null;
                _absManager = null;
                DontDestroyOnLoad(this.gameObject);
            }
        }

        void Update()
        {
            //state machine of game phases
            switch(CurrentGamePhase)
            {
                case GamePhase.INIT_STATE:
                    CurrentGamePhase = GamePhase.AB_LOADING;
                    LoadABs();
                    break;
                case GamePhase.AB_LOADING:
                    if(_absManager.IsBundleLoaded("logoscene") || EJRConsts.Instance.InitializingAssetBundle == null)
                    {
                        LoadGameLogoScene();
                        CurrentGamePhase = GamePhase.AB_LOGO_SCENE;
                    }
                    break;
                case GamePhase.AB_LOGO_SCENE:
                    if (!_absManager.IsLoading)
                    {
                        CurrentGamePhase = GamePhase.SCENE_NOT_CONFIGURED;
                        LoadMainScene();
                    }
                    break;
                case GamePhase.SCENE_NOT_CONFIGURED:
                    if (SceneManager.GetActiveScene() != null)
                    {
                        _mainScene = SceneManager.GetSceneByName("mainScene");
                        if (_mainScene.isLoaded)
                        {
                            SceneManager.SetActiveScene(_mainScene);
                            CurrentGamePhase = GamePhase.SCENE_NOT_INITIALIZED;
                            ConfigureMainScene();
                        }
                    }
                    break;
                case GamePhase.SCENE_NOT_INITIALIZED:
                    if(IsMainSceneInitialized())
                    {
                        CurrentGamePhase = GamePhase.SCENE_INITIALIZED_NO_LOAD;
                        _mainSceneRoot.GetComponent<GameManager>().LoadGame();
                    }
                    break;
                case GamePhase.SCENE_INITIALIZED_NO_LOAD:
                    if (_mainSceneRoot.GetComponent<GameManager>().IsGameLoaded && _mainSceneRoot.GetComponent<GameManager>().IsGameLoadedAndStarted)
                    {
                        CurrentGamePhase = GamePhase.PLAYING_GAME;
                        SceneManager.UnloadSceneAsync("ejrLogoScene");
                    }
                    break;
                default:
                    // do nothing
                    break;
            }
        }

        //start loading ABs and load logo/loading scene
        private void LoadABs()
        {
            _absManager = gameObject.AddComponent<ABsManager>();
            if(EJRConsts.Instance.InitializingAssetBundle != null)
                _absManager.AddBundleToQueue(EJRConsts.Instance.InitializingAssetBundle.Name);
            List<ABInfo> absInfo = EJRConsts.Instance.AssetBundlesToLoad;
            foreach (ABInfo abInfo in absInfo)
                _absManager.AddBundleToQueue(abInfo.Name);

            SceneManager.LoadScene("ejrLogoScene", LoadSceneMode.Single);
        }

        //load specific game's logo scene after logoscene AB is loaded
        private void LoadGameLogoScene()
        {
            if (EJRConsts.Instance.InitializingAssetBundle != null)
            {
                AssetBundle pakiet = ABsManager.Instance.GetBundle(EJRConsts.Instance.InitializingAssetBundle.Name);
                foreach (string objStr in EJRConsts.Instance.InitializingAssetBundle.ObjectsToLoad)
                {
                    GameObject logoSceneRootTemplate = pakiet.LoadAsset<GameObject>(objStr);
                    GameObject logoSceneRoot = GameObject.Instantiate(logoSceneRootTemplate, null);
                }
            }
        }
        //start loading main scene after ABs are loaded
        private void LoadMainScene()
        {
            SceneManager.LoadScene("mainScene", LoadSceneMode.Additive);
        }

        //start configuring main scene after it's loaded
        private void ConfigureMainScene()
        {            
            _mainSceneRoot = new GameObject();
            _mainSceneRoot.name = "sceneRoot";
            _configurator.CreateSceneGameObjects(_mainSceneRoot);
        }
        
        //is main scene and it's main objects initialized already?
        private bool IsMainSceneInitialized()
        {
            if (_mainSceneRoot != null && _mainSceneRoot.GetComponent<GameManager>() != null && _mainSceneRoot.GetComponent<GameManager>().IsStarted)
                return true;
            else
                return false;
        }
    }
}