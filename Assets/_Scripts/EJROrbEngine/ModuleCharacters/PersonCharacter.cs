// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.


using EJROrbEngine.SceneObjects;

namespace EJROrbEngine.Characters
{
    //A character that contain all human typical stats. Should be used for non-generic, "interesting" characters like important NPCs, player. PersonCharacter will be affected by diseases also.
    public class PersonCharacter : LivingCharacter
    {
        public Patient InternalPatient { get; private set; }

        public PersonCharacter(BaseDataAddon configValues, string name) : base(configValues, name)
        {
            InternalPatient = new Patient(this);
        }
        protected override void resetStats()
        {
            base.resetStats();

            //resource-like stats
            AddStat("Food", new CharacterStat("Food", 100, 100, 1, 0, 0));
            AddStat("Thirst", new CharacterStat("Thirst", 100, 100, 1, 0, 0));
            AddStat("Vitality", new CharacterStat("Vitality", 100, 100, 1, 0, 0));
            AddStat("Vigor", new CharacterStat("Vigor", 100, 100, 1, 0, 0));
            //main stats
            AddStat("Endurance", new CharacterStat("Endurance", 10, 100, 0, 0.49f, 1.01f));
        }
       
        public override void OnSecondChange(int nonthing)
        {
            base.OnSecondChange(nonthing);
            float mult = 1f - getSkillValue("Endurance") / 100f;
            if (mult < 0.1f)
                mult = 0.1f;
            if (mult > 2f)
                mult = 2f;

            if (getSkillValue("Food") > 0)
                _stats["Food"].ChangeValue(-0.1f * mult - 0.05f);
            else
                _stats["Health"].ChangeValue(-0.05f * mult);
            if (getSkillValue("Thirst") > 0)
                _stats["Thirst"].ChangeValue(-0.1f * mult - 0.05f);
            else
                _stats["Health"].ChangeValue(-0.05f * mult);
        }
        public override void OnHourChange(int currentVal)
        {
            InternalPatient.OnHourPassed();
            InternalPatient.HourlyCreateRandomDiseases(null);
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
    

    }
}