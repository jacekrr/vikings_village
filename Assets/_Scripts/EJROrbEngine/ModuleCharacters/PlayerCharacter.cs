// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.


using System.Collections.Generic;
using UnityEngine;
using ClientAbstract;
using EJROrbEngine.SceneObjects;

namespace EJROrbEngine.Characters
{
    //Character class only for PLAYER. To all PersonCharacter stats it adds leveling, experience and game control flow (like game over after death)
    public class PlayerCharacter : PersonCharacter
    {
        public const bool INDESTRUCTIBLE = false;      //change to false on release, it's for debug only !
        public const int DEF_CHARACTERPOINTS_PL = 3;                        //default character points per level
        public const int MAX_LEVEL = 100;                                   //maximum character level

        private const int DEF_BASE_EXP = 1000;                              //how much exp for first level
        private const int DEF_ACT_EXP = 1;                                  //default exp gained by a small single action
        private const float DEF_LEVELING_EXP_CURVE = 1f;                    //How much more exp pointa are needed for every next level (1 == the same amount)

        private int[] _expForLevel;				                    	//how much exp is needed to gain a level

        public PlayerCharacter(BaseDataAddon configData) : base(configData, "Player")
        {
            _expForLevel = new int[MAX_LEVEL + 1];
            _expForLevel[0] = 0;
            _expForLevel[1] = DEF_BASE_EXP;
            for (int i = 2; i < MAX_LEVEL; i++)
                _expForLevel[i] = _expForLevel[i - 1] + (int)Mathf.Round((_expForLevel[i - 1] - _expForLevel[i - 2]) * DEF_LEVELING_EXP_CURVE);
        }
        protected override void resetStats()
        {
            base.resetStats();

            AddStat("Experience", new CharacterStat("Experience", 0, 1000000, 0, 0, 0));
            AddStat("Level", new CharacterStat("Level", 0, 99, 0, 0, 0));
            AddStat("CharacterPoints", new CharacterStat("CharacterPoints", 0, 99 * DEF_CHARACTERPOINTS_PL, 0, 0, 0));
        }
       
        public override void OnSecondChange(int nonthing)
        {
            /* TODO auto uzycie przedmiotow dodajacych zasob na granicy wyczerpania
            if (getSkillValue("Health") < 15f)
                GameManager.TheInstance.TheInventory.UseItemForResource("Health");
            if (getSkillValue("Thirst") < 15f)
                GameManager.TheInstance.TheInventory.UseItemForResource("Mana");
            if (getSkillValue("Food") < 15f)
                GameManager.TheInstance.TheInventory.UseItemForResource("Food");
                */
            base.OnSecondChange(nonthing);
            ControlHealth();
        }
      
        public void addExperience(int baseExpCount)
        {
            int locBaseLev = 1;
            SetSkillValue("Experience", getSkillValue("Experience") + DEF_ACT_EXP * baseExpCount * (1f - 0.05f * (getSkillValue("Level") - locBaseLev)));
            expLevelingHandle();

        }

        //return value - 0: character alive, other: character is dead, 1 - no health, 2 - died on disease
        public override int isDead()
        {
            int reason = base.isDead();
            if (reason != 0)
                return reason;
            //own logic here
            return 0;
        }
        public override int isAlmostDead()
        {
            int reason = base.isAlmostDead();
            if (reason != 0)
                return reason;
            //own logic here
            return 0;
        }
        public void GetHit(float ammount)
        {
            SetSkillValue("Health", getSkillValue("Health") - ammount);
        }
       
        private void ControlHealth()
        {
            if (!INDESTRUCTIBLE && getSkillValue("Health") <= 0)
            {
                resetStats();
                GameManager.Instance.RestartAfterDeath();
            }
        }
       
        protected void expLevelingHandle()
        {
            while ((int)getSkillValue("Level") < MAX_LEVEL && getSkillValue("Experience") > _expForLevel[(int)getSkillValue("Level") + 1])
            {
                SetSkillValue("Level", getSkillValue("Level") + 1);
                SetSkillValue("CharacterPoints", getSkillValue("CharacterPoints")+ DEF_CHARACTERPOINTS_PL);
            }
        }


    }
}