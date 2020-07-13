// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia projektu Gułag obejmującego serię gier o tematyce ucieczki z Gułagu, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of Gulag project including a series of games about escape from Gulag, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.

using UnityEngine;
using ClientAbstract;
using EJROrbEngine.EndlessWorld;
using UnityStandardAssets.Characters.FirstPerson;
using EJROrbEngine;
using EJROrbEngine.FPPGame.UI;

namespace ClientWinPC
{
    public class WinPCPlayerController: BasePlayerController
    {
        private bool _ruchWylaczony, _wNastepnejKlatceWlaczRuch;
        private Terrain _teren;
        private FirstPersonController _controller;
               
        private void Awake()
        {
            _ruchWylaczony = false;
            _wNastepnejKlatceWlaczRuch = false;
            _teren = FindObjectOfType<Terrain>();
           _controller = GetComponent<FirstPersonController>();
        }
        private void Update()
        {
            if (_wNastepnejKlatceWlaczRuch)
            {
                _ruchWylaczony = false;
                _wNastepnejKlatceWlaczRuch = false;
                _controller.enabled = true;
            }
            else
                if (_ruchWylaczony)
                    _wNastepnejKlatceWlaczRuch = true;
            float speed = _controller.GetComponent<CharacterController>().velocity.magnitude;
            FPPUIManager.Instance.HandsController.ChangeMoveState(speed > 0.1f && speed <= 5f, speed > 5f);
            //      float wysokoscTerenu = _teren.SampleHeight(transform.position);
            //    if (transform.position.y < wysokoscTerenu)
            //      MovePlayer(_teren, new Vector3(transform.position.x, wysokoscTerenu + 0.01f, transform.position.z));
        }

        public override void MovePlayerWithoutTerrainLocation(Vector3 newPosition)
        {
            _controller.enabled = false;
            gameObject.transform.position = newPosition;
            _ruchWylaczony = true;
        }

        public override void MovePlayer(Terrain teren, Vector3 newPosition)
        {
            float wysokoscTerenu = teren.SampleHeight(transform.position) ;
            if (newPosition.y < wysokoscTerenu)
                newPosition = new Vector3(newPosition.x, wysokoscTerenu + 5.05f, newPosition.z);
            _ruchWylaczony = true;        
            _controller.enabled = false;
            gameObject.transform.position = newPosition;
        }

        public override void LoadGame(IGameState theGameState)
        {
            if (theGameState.GetIntKey("player main_transf") == 1)
            {
                MovePlayerWithoutTerrainLocation(new Vector3(theGameState.GetFloatKey("player main_px"), theGameState.GetFloatKey("player main_py") + 5.5f, theGameState.GetFloatKey("player main_pz")));
            }
            else
            {
                MovePlayerWithoutTerrainLocation(new Vector3(EndlessWorldModuleManager.Instance.TheMapConfig.StartPosition.x, EndlessWorldModuleManager.Instance.TheMapConfig.StartPosition.y, EndlessWorldModuleManager.Instance.TheMapConfig.StartPosition.z));
            }
            SetCameraFOV(int.Parse(EJRConsts.Instance["defaultTargetFov"]));
        }

        public override void SaveGame(IGameState theGameState)
        {
            theGameState.SetKey("player main_px", EndlessWorldModuleManager.Instance.GetCurrentWorldX());
            theGameState.SetKey("player main_py", EndlessWorldModuleManager.Instance.GetCurrentWorldY());
            theGameState.SetKey("player main_pz", EndlessWorldModuleManager.Instance.GetCurrentWorldZ());
            theGameState.SetKey("player main_transf", 1);
        }

        public override void LocateOnTerrain()
        {
            if (EndlessWorldModuleManager.Instance.CurrentBiom != null)
                MovePlayer(EndlessWorldModuleManager.Instance.CurrentBiom.TheTerrain.GetComponent<Terrain>(), GetPlayerPosition());
        }

        public override Vector3 GetPlayerPosition()
        {
            return base.GetPlayerPosition();
        }

        public override void SetCameraFOV(int fov)
        {
            base.SetCameraFOV(fov);
            Camera.main.fieldOfView = fov;
        }
    }
}
