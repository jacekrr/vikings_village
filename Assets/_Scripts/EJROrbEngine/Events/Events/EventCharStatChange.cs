﻿// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.


using EJROrbEngine.Characters;
using UnityEngine;

namespace EJROrbEngine.Events
{
    //change statistics of a character (player or npc)
    public class EventCharStatChange : BaseEvent
    {
        public SceneCharacter CharacterObject;      //if not set it will affect the player
        public string[] StatNames;
        public string[] StatValues;
        
        public override void FireEvent(BaseEventActivator activator)
        {

            if (StatNames.Length != StatValues.Length)
                Debug.LogError("Invalid arrays in EventCharStateChange of:" + name);
            else
            {
                Characters.BaseCharacter theChar = CharacterObject == null ? CharactersModuleManager.Instance.ThePlayer : CharacterObject.TheCharacter;
                for (int i = 0; i < StatNames.Length; i++)
                    theChar.SetSkillValue(StatNames[i], float.Parse(StatValues[i]));
            }
        }
    }
}