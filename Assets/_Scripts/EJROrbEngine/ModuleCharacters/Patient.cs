// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.



using EJROrbEngine.Herbology;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EJROrbEngine.Characters
{         
    public class Patient
    {
        public const float MINIMUM_WAZNEGO_ZDARZENIA = 10;
       
        public bool IsHealthy { get { return _currentEvents.Count == 0; } }
        public string DeathReasons { get; private set; }
        public int CurrentEventsNumber {  get { return _currentEvents.Count; } }
        public int TreatmentItemsNUmber {  get { return _treatmentItems.Count; } }
        public List<string> ImportantEvents { get; private set; }          //istotne zdarzenie ktore zaszlo w trakcie obslugi godzinowej

        private List<MedicalItemDataAddon> _treatmentItems;        // lista przedmiotow akutalnie leczacych pacjenta
        private List<MedicalEvent> _currentEvents;         // lista aktualnych zdarzen medycznych
        private List<BodyPart> _amputatedPlaces;       //lista lokalizacji dla ktorych nie mozna juz utworzyc zdarzen medycznych bo... dana czesc ciala nie funkcjonuje calkowicie (np. amputowana reka nie moze zostac odmrozona)      
        private PersonCharacter _character;
        public Patient(PersonCharacter anCharacter) : base()
        {
            _character = anCharacter;
            ImportantEvents = new List<string>();
            _currentEvents = new List<MedicalEvent>();
            _amputatedPlaces = new List<BodyPart>();
            _treatmentItems = new List<MedicalItemDataAddon>();
            DeathReasons = "";
        }

        //dodaje nowe zdarzenie pierwotne (nowe choroby)
        public void AddNewEvent(MedicalEvent newEvent)
        {
            bool alreadyExists = false;
            foreach (MedicalEvent me in _currentEvents)
                if (me.Type == newEvent.Type)
                    alreadyExists = true;
            if(!alreadyExists)
                _currentEvents.Add(newEvent);
        }

        //obsluga zdarzen medycznych godzinowo
        public void OnHourPassed()
        {
            ImportantEvents.Clear();
            List<MedicalEvent> allNewEvents = new List<MedicalEvent>();  //skumulowana lista nowych zdarzen medycznych
            List<MedicalEvent> eventsToBeDeleted = new List<MedicalEvent>();     //zdarzenia do usuniecia
            foreach(MedicalEvent anEvent in _currentEvents)
            {
                List<MedicalEvent> newEvent = HerbologyModuleManager.Instance.PreprocessMedicalEvent(anEvent);
                allNewEvents.AddRange(newEvent);
                //obsluzmy jeszcze witalnosc pacjenta
                anEvent.Strength = anEvent.Strength * (1 - _character.getSkillValue("Vitality")  / 5000f) - 0.01f * (_character.getSkillValue("Vitality") / 50f);
                //oraz leczenie przedmiotami
                float treatment = GetHourTreatment(anEvent);
                anEvent.Strength -= 1f * treatment;
                anEvent.Intensity -= 0.5f * treatment;
                //obsluz wplyw zdarzenia na parametry ogolne
                HourlyEventChanges(anEvent);
                //smiertelnosc gwaltowna zdarzenia
                if ( (float)anEvent["mortality"] > 0 && Random.Range(0f, 1f) < (1.5f * (float)anEvent["mortality"] * BodyPartMechanics.BodyPartMultiplier(anEvent.Place)) && Random.Range(0f, 100f) > _character.getSkillValue("Vitality") * 0.75f)
                {
                    DeathReasons = string.Format("{0}", anEvent.FriendlyName());
                    _character.SetSkillValue("Zdrowie", -100);
                }
                //zdarzenie w wyniku przetworzenia moglo oslabnac do zera - do usuniecia
                if (anEvent.Strength <= (float)anEvent["min"])
                    eventsToBeDeleted.Add(anEvent);
                //zdarzenie jednorazowe bedzie usuniete po przetworzeniu
                if ((bool)anEvent["oneTimeEvent"])
                    eventsToBeDeleted.Add(anEvent);
                anEvent.Normalize();
            }
            //efekty uboczne przedmiotow
            List<MedicalEvent> zdarzeniaUboczne = GetHourSideEffects();
            _currentEvents.AddRange(zdarzeniaUboczne);
            _currentEvents.AddRange(allNewEvents);
            //wplyw na parametry ogolne od przedmiotow
            HourlyGeneralChangeOfItems();
            //nowe zdarzenia powinny być osłabione o działanie aktualnych leków (daje to działanie zapobiegawcze)
            foreach (MedicalEvent anEvent in allNewEvents)
            {
                anEvent.Strength -= GetHourTreatment(anEvent);
                if (anEvent.Strength <= (float)anEvent["min"])
                    _currentEvents.Remove(anEvent);
            }
            //musimy jeszcze przejrzec liste aby polaczyc zdarzenia, ktore moga byc identyczne i dotyczyc tego samego miejsca (stworzone z dwoch roznych chorob) np. dwa zapalenia gardla redukuja sie do jednego mocniejszego
            for(int i = 0; i < _currentEvents.Count; i++)
                for (int j = i + 1; j < _currentEvents.Count; j++)
                {
                    if(_currentEvents[i].Type == _currentEvents[j].Type && (_currentEvents[i].Place & _currentEvents[j].Place) != 0)
                    {
                        //zdarzenie bedzie suma sil zdarzen i najwiekszym zaostrzeniem, a zdarzenie2 zostanie usuniete
                        _currentEvents[i].Strength = _currentEvents[i].Strength + _currentEvents[j].Strength;
                        _currentEvents[i].Intensity = _currentEvents[i].Intensity > _currentEvents[j].Intensity ? _currentEvents[i].Intensity : _currentEvents[j].Intensity;
                        _currentEvents[i].Normalize();
                        eventsToBeDeleted.Add(_currentEvents[j]);
                    }
                }
            //obsluga amputacji 
            foreach (MedicalEvent anEvent in _currentEvents)
                if (anEvent.Type == "Amputacja" && !_amputatedPlaces.Contains(anEvent.Place))
                {
                    _amputatedPlaces.Add(anEvent.Place);
                }
            foreach (MedicalEvent anEvent in _currentEvents)
                if (_amputatedPlaces.Contains(anEvent.Place))
                    eventsToBeDeleted.Add(anEvent);
            //usuniecie niepotrzebnych zdarzen
            foreach (MedicalEvent anEvent in eventsToBeDeleted)
                _currentEvents.Remove(anEvent);
            HourlyUseOfItems();
            SortEvents();
        }
        
        public void AddTreatmentItem(MedicalItemDataAddon newItem)
        {
            newItem.ResetDuration();
            //first use of item is now, it may last longer if has duration > 1
            List<Harm> wplywPrzedmiotu = newItem.GetGeneralChange();
            foreach (Harm change in wplywPrzedmiotu)
                _character.SetSkillValue(change.ResName, _character.getSkillValue(change.ResName) + 1f * change.Value);
            newItem.UseDose();
            if(newItem.DurationRemained >= 1)
                _treatmentItems.Add(newItem);
        }

        public string EventsDump(bool debug)
        {
            StringBuilder log = new StringBuilder();
            log.Append(string.Format("{0}: {1} \n\r", StringsTranslator.GetString("res_Vitality"), FloatToStr(_character.getSkillValue("Vitality"))));
            foreach (MedicalItemDataAddon przedmiotL in _treatmentItems)
                log.Append(string.Format(" % {0} {1} \n\r", StringsTranslator.GetString("PrzedmiotLeczacy"), StringsTranslator.GetString("ItemName" + przedmiotL.Type)));
            foreach (BodyPart wykluczonaCzescCiala in _amputatedPlaces)
                log.Append(string.Format(" # {0}: {1} \n\r", StringsTranslator.GetString("Amputowano"), wykluczonaCzescCiala));
            if (debug)
                foreach (MedicalEvent zdarzenie in _currentEvents)
                    log.Append(" * " + zdarzenie.FriendlyName() + " " + FloatToStr(zdarzenie.Strength) + " (" + FloatToStr(zdarzenie.Intensity) + ")\n\r");
            else
                foreach (MedicalEvent zdarzenie in _currentEvents)
                    log.Append(" * " + zdarzenie.FriendlyName() + ", " + StringsTranslator.GetString("Zagrozenie") + zdarzenie.StrengthFriendly() + "\n\r");
            return log.ToString();
        }
        public MedicalEvent GetEvent(int i)
        {
            if (i >= 0 && i < CurrentEventsNumber)
                return _currentEvents[i];
            return null;
        }

        //losuje chorobe biorac pod uwage ze jest wywolywane raz na godzine i biorac pod uwage parametry postaci i parametry zewnetrzne
        public void HourlyCreateRandomDiseases(Dictionary<string, float> externalConditions)
        {
            //aspekty niedożywienia
            if(_character.getSkillValue("Food") < 50)
            {
                float rand =  _character.getSkillValue("Food") / 2 + _character.getSkillValue("Endurance") / 2 + _character.getSkillValue("Vitality") / 3 + 1;
                if (rand > 100)
                    rand = 100;
                if (Random.Range(0, (int) rand) == 0)
                    AddNewEvent(HerbologyModuleManager.Instance.CreateMedicalEventOfType("Niedozywienie", BodyPart.WieleNarzadow, Random.Range(5, 110 - rand), 0));
                if (Random.Range(0, (int) (rand * 2)) == 0)
                    AddNewEvent(HerbologyModuleManager.Instance.CreateMedicalEventOfType("Awitaminoza", BodyPart.WieleNarzadow, Random.Range(5, 110 - rand), 0));
            }
            //aspekty odwodnienia
            if (_character.getSkillValue("Thirst") < 50)
            {
                float rand = _character.getSkillValue("Thirst") / 2 + _character.getSkillValue("Endurance") / 2 + _character.getSkillValue("Vitality") / 3 + 1;
                if (rand > 100)
                    rand = 100;
                if (Random.Range(0, (int)rand) == 0)
                    AddNewEvent(HerbologyModuleManager.Instance.CreateMedicalEventOfType("Odwodnienie", BodyPart.WieleNarzadow, Random.Range(5, 110 - rand), 0));
            }
            SortEvents();          
        }

        private string FloatToStr(float liczba)
        {
            return liczba.ToString("F2");
        }
        private float GetHourTreatment(MedicalEvent anEvent)
        {
            foreach (MedicalItemDataAddon przedmiot in _treatmentItems)
            {
                float leczenie = przedmiot.GetHourTreatment(anEvent);
                if (leczenie != 0)
                    return leczenie;
            }
            return 0;
        }
        private List<MedicalEvent> GetHourSideEffects()
        {
            List<MedicalEvent> side = new List<MedicalEvent>();
            foreach (MedicalItemDataAddon item in _treatmentItems)
                side.AddRange(item.GetRandomEvent());
            return side;
        }
        private void HourlyGeneralChangeOfItems()
        {
            foreach (MedicalItemDataAddon item in _treatmentItems)
            {
                List<Harm> wplywPrzedmiotu = item.GetGeneralChange();
                foreach (Harm change in wplywPrzedmiotu)
                    _character.SetSkillValue(change.ResName, _character.getSkillValue(change.ResName) + 1f * change.Value );               
            }
        }
        private void HourlyUseOfItems()
        {
            List<MedicalItemDataAddon> doUsuniecia = new List<MedicalItemDataAddon>();
            foreach(MedicalItemDataAddon item in _treatmentItems)
            {
                item.UseDose();
                if (item.DurationRemained <= 0)
                    doUsuniecia.Add(item);
            }
            foreach (MedicalItemDataAddon item in doUsuniecia)
                _treatmentItems.Remove(item);
        }
        private void HourlyEventChanges(MedicalEvent anEvent)
        {
            foreach (Harm change in anEvent.GeneralHarm)
            {
                float wielkoscWplywu =  change.Value * anEvent.Strength * BodyPartMechanics.BodyPartMultiplier(anEvent.Place);
                if (wielkoscWplywu <= -MINIMUM_WAZNEGO_ZDARZENIA && change.ResName == "Health")
                    ImportantEvents.Add(StringsTranslator.GetString("PowaznyWplyw") + anEvent.FriendlyName());
                _character.SetSkillValue(change.ResName, _character.getSkillValue(change.ResName) + wielkoscWplywu);
            }
        }
        private void SortEvents()
        {
            for(int i = 0; i < CurrentEventsNumber; i++)
                for (int j =  i + 1; j < CurrentEventsNumber; j++)
                    if(_currentEvents[i].Strength < _currentEvents[j].Strength)
                    {
                        MedicalEvent tymczasowe = _currentEvents[i];
                        _currentEvents[i] = _currentEvents[j];
                        _currentEvents[j] = tymczasowe;
                    }
        }
    }
}