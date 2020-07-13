// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.

using UnityEngine;

namespace ClientAbstract
{
    public class BasePlayerController: MonoBehaviour
    {
        public virtual void MovePlayer(Terrain terrain, Vector3 newPosition)
        {
            gameObject.transform.position = newPosition;
        }
        public virtual void MovePlayerWithoutTerrainLocation(Vector3 newPosition)
        {
            gameObject.transform.position = newPosition;
        }

        public virtual Vector3 GetPlayerPosition()
        {
            return transform.position;
        }
        public virtual void LoadGame(IGameState theGameState)
        {
            //do nothing in base class
        }
        public virtual void SaveGame(IGameState theGameState)
        {
            //do nothing in base class
        }
        public virtual void LocateOnTerrain()
        {
            //do nothing in base class
        }
        public virtual void SetCameraFOV(int fov)
        {
            //do nothing in base class
        }


    }


}
