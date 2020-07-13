// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.

using ClientAbstract;
using UnityEngine;

namespace EJROrbEngine
{
    //load game settings and parameters from the save game
    public sealed class SettingsManager: MonoBehaviour
    {
        public static SettingsManager Instance { get; private set; }

        //difficulty level
        public int DifficultyLevel = 0;     // 0 - easy, 1 - hard
        public int QualitySettingsVal = 5;
        public bool UseCameraEffects { get; private set; }

        //All LODX consts are given in square meters calculated as (dx*dx) + (dz*dz)
        public static int LOD0_SQRDIST = 512;     //=16m. very small items, mushrooms, house furniture itp.
        public static int LOD1_SQRDIST = 2048;    //=32m. small items, small bushes itp.
        public static int LOD2_SQRDIST = 8192;    //=64m. medium and big items, bushes, small fallen trees, itp.
        public static int LOD3_SQRDIST = 32768;   //=128m. most items, small houses, small trees, itp.
        public static int LOD4_SQRDIST = 131072;  //=258m. very big items, trees, houses itp.
        public static int LOD5_SQRDIST = 524288;  //=512m. very big landmarks

        public static float BIOMSMANAGEMENT_MOVEMENT = 9;           //if player moves more meters than that number, we will check if bioms shopuld be managed
        public static int MAX_BIOMS = 7;                             //maximum visible bioms in every direction
        public static int LOD_DIST_BIOMS = 19;                        //maximum square distance where bioms have detailed terrains (calculated as dx*dx + dz*dz where dx and dz are distance in bioms unit)

        //visibility checking frequency for all LODs (in [s.])
        public static float[] LOD_DELTATIME = new float[] { 0.35f, 0.54f, 0.76f, 1.01f, 1.29f, 1.52f, 1.83f };


        public static int GetSqrtDistForLOD(int LOD)
        {
            return LOD == 0 ? LOD0_SQRDIST : (LOD == 1 ? LOD1_SQRDIST : (LOD == 2 ? LOD2_SQRDIST : (LOD == 3 ? LOD3_SQRDIST : (LOD == 4 ? LOD4_SQRDIST : LOD5_SQRDIST))));
        }
        //internals
        private bool firstUpdate = true;
     //   private float fogDensity;

        private void Awake()
        {
            if (Instance != null && Instance != this)
                throw new System.Exception("Niedozwolone tworzenie kolejnej kopii klasy SettingsManager");
            Instance = this;
        }
        private void Start()
        {
            firstUpdate = true;
      //a      fogDensity = 0.0004f;
            UseCameraEffects = true;

        }
        private void Update()
        {
            if (firstUpdate)
            {
                firstUpdate = false;
            }
          /*  if (RenderSettings.fogDensity < fogDensity)
            {
                RenderSettings.fog = true;
                RenderSettings.fogMode = FogMode.Exponential;
                RenderSettings.fogDensity = fogDensity;
            }
            */
        }

        //load saved game from autosave file
        public void LoadGame(IGameState aGameState)
        {

            if (aGameState.KeyExists("QualitySettings"))
            {
                QualitySettingsVal = aGameState.GetIntKey("QualitySettings");
                QualitySettings.SetQualityLevel(QualitySettingsVal);
            }
            if (aGameState.KeyExists("DifficultyLevel"))
                DifficultyLevel = aGameState.GetIntKey("DifficultyLevel");

        }

        //save game's state to autosave file
        public void SaveGame(IGameState aGameState)
        {
            aGameState.SetKey("QualitySettings", QualitySettingsVal);
            aGameState.SetKey("DifficultyLevel", DifficultyLevel);
            aGameState.SetKey("UseCameraEffects", UseCameraEffects ? 1 : 0);
        }

    }
}

