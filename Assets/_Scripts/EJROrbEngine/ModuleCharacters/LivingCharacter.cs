// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.

using EJROrbEngine.SceneObjects;

namespace EJROrbEngine.Characters
{
    //A character that is common for every living being like player, npc, animal and so on. It implements basic stats for all living characters like Health
    public class LivingCharacter : BaseCharacter
    {
        public LivingCharacter(BaseDataAddon configValues, string name) : base(configValues, name)
        {

        }
        protected override void resetStats()
        {
            base.resetStats();

            //resource-like stats
            AddStat("Health", new CharacterStat("Health", 100, 100, 1, 0, 0));            
            AddStat("Stamina", new CharacterStat("Stamina", 100, 100, 1, 0, 0));
            //main stats
            AddStat("Strength", new CharacterStat("Strength", 10, 100, 0, 0.495f, 1.005f));
            AddStat("Perception", new CharacterStat("Perception", 10, 100, 0, 0.495f, 1.005f));
            AddStat("Toughness", new CharacterStat("Toughness", 10, 100, 0, 0.245f, 1.005f));
        }
       
        public override void OnSecondChange(int nothing)
        {
            base.OnSecondChange(nothing);
        }
        //return value - 0: character alive, other: character is dead, 1 - no health, bigger numbers - derived classes
        public virtual int isDead()
        {
            if(getSkillValue("Health") <= 0)	return 1;
            return 0;
        }
        public virtual int isAlmostDead()
        {
            if(getSkillValue("Health") <= 0.1f * getSkillMaxValue("Health")) return 1;
            return 0;
        }
        

/*        public void GetHit(NPCAI enemy, float amnt)
        {
            float mult = 1f - getSkillValue("Perception") / 100f;
            mult -= getSkillValue("cls_def_" + enemy.Config.NPC_Class) / 100f;
            if (mult < 0.1f)
                mult = 0.1f;
            if (mult > 2f)
                mult = 2f;
            amnt *= mult;
            GameManager.TheInstance.TheEngineGUIManager.ShowSmallMsg(string.Format(StringsReader.GetString("player_hit"), EngineGUIManager.FormatHPNumber(amnt)));
            forceChangeStat("Health", -amnt);
            upgradeSkill("Toughness", true);
            upgradeSkill("cls_def_" + enemy.Config.NPC_Class, true);
            ControlHealth();

        }
        */
    
        /*
        public void MadeEnemyHit(NPCAI enemy, HitType hittype)
        {
            if (hittype == HitType.MagicHit)
                upgradeSkill("Magic", true);
            else if (hittype == HitType.MeleeHit)
                upgradeSkill("Strength", true);
            else if (hittype == HitType.RangeHit)
                upgradeSkill("Perception", true);
            upgradeSkill("cls_att_" + enemy.Config.NPC_Class, true);

        }
        public float HitMultiplierFromSkills(NPCAI enemy, HitType hittype)
        {
            float baseAmnt = 1f;
            baseAmnt += getSkillValue("cls_att_" + enemy.Config.NPC_Class) / 100f;
            if (hittype == HitType.MagicHit)
                return baseAmnt + getSkillValue("Magic") / 100f;
            else if (hittype == HitType.MeleeHit)
                return baseAmnt + getSkillValue("Strength") / 100f;
            else if (hittype == HitType.RangeHit)
                return baseAmnt + getSkillValue("Perception") / 100f;
            return baseAmnt;
        }
        */


    }
}