// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.



using EJROrbEngine.FightSystem;
using EJROrbEngine.SceneObjects;
using UnityEngine;

namespace EJROrbEngine.PlayerInventory
{

    public class WeaponDataAddon : BaseDataAddon
    {       
        public bool IsMeleeWeapon { get { return this["meleeMin"] != null && this["meleeMax"] != null && ((int)this["meleeMin"] > 0 || (int)this["meleeMax"] > 0); } }
        public bool IsShootingWeapon { get { return this["shootMin"] != null && this["shootMax"] != null && ((int)this["shootMin"] > 0 || (int)this["shootMax"] > 0); } }
        public bool IsThrowWeapon { get { return this["throwMin"] != null && this["throwMax"] != null && ((int)this["throwMin"] > 0 || (int)this["throwMax"] >0); } }
        public bool TripleFireModeEnabled {  get { return (bool)this["fireModeTriple"]; } }
        public bool AutoFireModeEnabled { get { return (bool)this["fireModeAuto"]; } }
        public FireMode CurrentFireMode
        {
            get
            {
                return _fireMode;
            }
            set
            {
                if (value == FireMode.Single)
                { 
                    _fireMode = value;
                    Debug.Log("przelaczenie: single");
                }
                else if (value == FireMode.Triple && TripleFireModeEnabled)
                {
                    _fireMode = value;
                    Debug.Log("przelaczenie: Triple");
                }
                else if (value == FireMode.Auto && AutoFireModeEnabled)
                {
                    _fireMode = value;
                    Debug.Log("przelaczenie: auto");
                }
            }
        }
        private FireMode _fireMode;

        public WeaponDataAddon()
        {
                _fireMode = FireMode.Single;
        }
        

    }

}