// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia projektu Gułag obejmującego serię gier o tematyce ucieczki z Gułagu, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of Gulag project including a series of games about escape from Gulag, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.
//#define OPSIVE_ON

using UnityEngine;
using ClientAbstract;
using EJROrbEngine.EndlessWorld;
#if OPSIVE_ON
using Opsive.UltimateCharacterController.Character;
#endif
namespace ClientWinPC
{
    public class WinPCPlayerControllerUCC: BasePlayerController
    {
#if OPSIVE_ON
        private bool _ruchWylaczony, _wNastepnejKlatceWlaczRuch;
        private Terrain _teren;
        UltimateCharacterLocomotion _locomotion;
               

        private void Awake()
        {
            _ruchWylaczony = false;
            _wNastepnejKlatceWlaczRuch = false;
            _teren = FindObjectOfType<Terrain>();
            _locomotion = GetComponentInChildren< UltimateCharacterLocomotion>();
        }
        private void Update()
        {
            if (_wNastepnejKlatceWlaczRuch)
            {
                _ruchWylaczony = false;
                _wNastepnejKlatceWlaczRuch = false;
            }
            else
                if (_ruchWylaczony)
                    _wNastepnejKlatceWlaczRuch = true;
            //      float wysokoscTerenu = _teren.SampleHeight(transform.position);
            //    if (transform.position.y < wysokoscTerenu)
            //      MovePlayer(_teren, new Vector3(transform.position.x, wysokoscTerenu + 0.01f, transform.position.z));
        }

        public override void MovePlayerWithoutTerrainLocation(Vector3 newPosition)
        {

            _locomotion.SetPosition(newPosition);
            _ruchWylaczony = true;
        }

        public override void MovePlayer(Terrain teren, Vector3 newPosition)
        {
            float wysokoscTerenu = teren.SampleHeight(transform.position) ;
            if (newPosition.y < wysokoscTerenu)
                newPosition = new Vector3(newPosition.x, wysokoscTerenu + 5.05f, newPosition.z);

            _locomotion.SetPosition(newPosition);
        }

        public override void LoadGame(IGameState theGameState)
        {
            if (theGameState.GetIntKey("player main_transf") == 1)
            {
                MovePlayerWithoutTerrainLocation(new Vector3(theGameState.GetFloatKey("player main_px"), theGameState.GetFloatKey("player main_py") + 5.5f, theGameState.GetFloatKey("player main_pz")));
            }
            else
            {
                MovePlayerWithoutTerrainLocation(new Vector3(WorldManager.Instance.TheMapConfig.StartPosition.x, 0.1f, WorldManager.Instance.TheMapConfig.StartPosition.z));
            }
        }

        public override void SaveGame(IGameState theGameState)
        {
            theGameState.SetKey("player main_px", WorldManager.Instance.GetCurrentWorldX());
            theGameState.SetKey("player main_py", WorldManager.Instance.GetCurrentWorldY());
            theGameState.SetKey("player main_pz", WorldManager.Instance.GetCurrentWorldZ());
            theGameState.SetKey("player main_transf", 1);
        }

        public override void LocateOnTerrain()
        {
            if (WorldManager.Instance.CurrentBiom != null)
                MovePlayer(WorldManager.Instance.CurrentBiom.TheTerrain.GetComponent<Terrain>(), GetPlayerPosition());
        }

        public override Vector3 GetPlayerPosition()
        {
            return _locomotion.transform.position;
        }
#endif
    }


}

