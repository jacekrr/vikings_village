// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.

using ClientAbstract;
using EJROrbEngine.SceneObjects;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

namespace EJROrbEngine.Herbology
{
   
    public sealed class HerbologyModuleManager : MonoBehaviour, IEngineModule
    {
        private Dictionary<string, MedicalEvent> _medicalEventTemplates;                 //lista zdarzen med. zaladowane z konfiguracji
        private List<Substance> _substancesTemplates;                       // dostepne w grze listy substancji medycznych zaladowane z konfiguracji
        private Dictionary<string, MedicalItemDataAddon> _medicalItemTemplates;      // wzorce kompletnych przedmiotow leczniczych, klucz: typ przedmiotu, wartosc: struktura z informacjami

        private List<BaseDataAddon> _primalDiseasesTemplates;               // list przejsc miedzy zdarzeniami z konfiguracji, tylko przejscia pierowtne (poczatek choroby)
        private List<BaseDataAddon> _derivedDiseasesTemplates;              // list przejsc miedzy zdarzeniami z konfiguracji, tylko przejscia zalezne (nowe objawy choroby)

        public static HerbologyModuleManager Instance { get; private set; }
        private void Awake()
        {
            if (Instance != null && Instance != this)
                throw new System.Exception("Niedozwolone tworzenie kolejnej kopii klasy HerbologyModuleManager");
            Instance = this;

            _medicalEventTemplates = new Dictionary<string, MedicalEvent>();
            _substancesTemplates = new List<Substance>();
            _medicalItemTemplates = new Dictionary<string, MedicalItemDataAddon>();
            _primalDiseasesTemplates = new List<BaseDataAddon>();
            _derivedDiseasesTemplates = new List<BaseDataAddon>();

        }

        public void OnLoad(IGameState gameState)
        {
            // do nothing
        }
        public void CleanupBeforeSave()
        {
            // do nothing
        }
        public void OnSave(IGameState gameState)
        {
            // do nothing
        }
        public void OnNewGame()
        {
            // do nothing
        }
        public void OnConfigure()
        {
            if (_medicalEventTemplates.Count == 0)
            {
                //zdarzenia   
                XmlDataInfo eventsInfo = Utils.LoadXmlAssetFile("data/medical_events", "medical_events");
                if (eventsInfo != null)
                {
                    foreach (XElement element in eventsInfo.MainNodeElements)
                    {
                        MedicalEvent noweZdarzenie = new MedicalEvent();
                        noweZdarzenie.LoadData(eventsInfo, element);
                        _medicalEventTemplates.Add(noweZdarzenie.Type, noweZdarzenie);
                    }
                }
                //substancje
                XmlDataInfo substancesInfo = Utils.LoadXmlAssetFile("data/medical_substances", "substances");
                if (substancesInfo != null)
                {
                    foreach (XElement element in substancesInfo.MainNodeElements)
                    {
                        Substance nowaSubstancja = new Substance();
                        nowaSubstancja.LoadData(substancesInfo, element);
                        _substancesTemplates.Add(nowaSubstancja);
                    }
                }
                //przedmioty lecznicze (ladowanie substancji musi tu byc skonczone poniewaz sa one uzywane do tworzenia klonow substancji aktywnych w przedmiotach)
                XmlDataInfo medicalItemsInfo = Utils.LoadXmlAssetFile("data/medical_items", "medical_items");
                if (medicalItemsInfo != null)
                {
                    foreach (XElement element in medicalItemsInfo.MainNodeElements)
                    {
                        MedicalItemDataAddon nowyPrzedmiot = new MedicalItemDataAddon();
                        nowyPrzedmiot.LoadData(medicalItemsInfo, element, _substancesTemplates);
                        _medicalItemTemplates.Add(nowyPrzedmiot.Type, nowyPrzedmiot);
                    }
                }

            }
            if (_primalDiseasesTemplates.Count == 0)
            {
                try
                {
                    //szanse 
                    XmlDataInfo chancesInfo = Utils.LoadXmlAssetFile("data/medical_chances", "chances");
                    if (chancesInfo != null)
                    {
                        foreach (XElement element in chancesInfo.MainNodeElements)
                        {
                            BaseDataAddon nowaSzansaNaZdarzenie = new BaseDataAddon();
                            nowaSzansaNaZdarzenie.LoadData(chancesInfo, element);
                            if ((string)nowaSzansaNaZdarzenie["last"] == "")
                                _primalDiseasesTemplates.Add(nowaSzansaNaZdarzenie);
                            else
                                _derivedDiseasesTemplates.Add(nowaSzansaNaZdarzenie);
                        }
                    }

                }
                catch (System.Exception wyjatek)
                {
                    Debug.LogError("Ogólny wyjątek: " + wyjatek.Message);
                }
            }

        }
        public void OnConfigureObjectRequest(SceneObjects.PrefabTemplate ao)
        {
            MedicalItemDataAddon medItem = FindMedicalItem(ao.Type);
            if (medItem != null)
                ao.DataObjects.AddDataAddon("medical_items", medItem);
        }
        //próbuje dopasować wzorzec zdarzenia medycznego do podanego typu i jeśli mu się uda - tworzy klon i zwraca go
        public MedicalEvent FindMedicalEvent(string type)
        {
            if (_medicalEventTemplates.ContainsKey(type))
                return new MedicalEvent(_medicalEventTemplates[type]);
            return null;
        }

        //próbuje dopasować wzorzec przedmiotu medycznego do podanego typu i jeśli mu się uda - tworzy klon i zwraca go
        public MedicalItemDataAddon FindMedicalItem(string type)
        {
            if (_medicalItemTemplates.ContainsKey(type))
                return new MedicalItemDataAddon(_medicalItemTemplates[type]);
            return null;
        }


        /*
        //losuje choroby tak dlugo az wylosuje conajmniej rowna poziomTrudnosci / 2, ale nie mniej niz 100 losowan i nie wiecej niz 1000 losowan, a takze nie wiecej chorob niz MAKSIMUM_CHOROB
        public void CreateRandomDiseases(bool krytyczne, int poziomTrudnosci)
        {
            int minimalnaLiczbaZdarzen = poziomTrudnosci < 2 ? 1 : poziomTrudnosci / 2;
            int liczbaLosowan = 0;
            do
            {
                MedicalEvent zdarzenie = MenedzerZielarstwa.Instancja.UtworzPierwotneMedicalEvent();
                if (zdarzenie != null && (! krytyczne || zdarzenie.Sila >= 0.5f) && (krytyczne || zdarzenie.Sila <= 0.5f))
                    AddNewEvent(zdarzenie);
                liczbaLosowan++;
            } while (_currentEvents.Count < KonfiguratorGry.MaksimumChorobPacjenta && _currentEvents.Count < minimalnaLiczbaZdarzen && (liczbaLosowan < 100 || (liczbaLosowan < 1000 && _currentEvents.Count == 0)));
            SortujZdarzenia();
            if (krytyczne)
            {
                LicznikOczekiwania = -1;
                _pacjentLezacy = true;
            }
            else
            {
                LicznikOczekiwania = KonfiguratorGry.MaksimumOczekiwaniaPacjenta;
                _pacjentLezacy = false;
            }
        }
        */

        /*
        //tworzy nowe, losowe pierwotne zdarzenie medyczne (nowa chorobe), poniewaz bierze pod uwage prawdopodobienstwo zajscia chorob, moze zwrocic null oznaczajacy ze zadna nie zostala wylosowana
        public ZdarzenieMedyczne UtworzPierwotneZdarzenieMedyczne()
        {
            SzansaNaZdarzenie wylosowaneZdarzenie = null;
            foreach (SzansaNaZdarzenie szansa in _wzorceChorobPierwotnych)
                if (Random.Range(0, 1f) < szansa.Prawdopodobienstwo)
                    wylosowaneZdarzenie = szansa;
            if (wylosowaneZdarzenie != null)
            {
                float noweZaostrzenie = Random.Range(0.05f, 0.5f);
                ZdarzenieMedyczne noweZdarzenie = UtworzZdarzenieMedyczneNaPodstawieSzansy(wylosowaneZdarzenie, wylosowaneZdarzenie.LokalizacjaNastepna, 0, noweZaostrzenie);           
                noweZdarzenie.Sila = Random.Range(noweZdarzenie.MinimalnaSila + (noweZdarzenie.MinimalnaSila < 0.1f ? 0.1f : 0), noweZdarzenie.MaksymalnaSila);
                
                return noweZdarzenie;
            }
            else return null;
        }*/

        // Na zlecenie pacjenta wykonuje cogodzinna zmiane stanu zdarzenia medycznego.
        // Zdarzenie podane jako parametr moze zmienic swoj status, jesli jego sila spadnie do 0 - powinno byc usuniete
        // Zwracana wartosc to lista nowych zdarzen wyniklych z rozwoju choroby (moze byc pusta)
        public List<MedicalEvent> PreprocessMedicalEvent(MedicalEvent anEvent)
        {
            //proba wylosowania nowego zdarzenia
            List<MedicalEvent> newEventList = new List<MedicalEvent>();
            foreach (BaseDataAddon szansa in _derivedDiseasesTemplates)
            {
                if (((BodyPart)szansa["lastPlace"] & anEvent.Place) != 0 && (string)szansa["last"] == anEvent.Type && Random.Range(0, 1f) < (float)szansa["prob"])
                {
                    MedicalEvent newEvent = CreateMedicalEventOfChance(szansa, (BodyPart)szansa["lastPlace"] == BodyPart.Dowolna ? anEvent.Place : (BodyPart)szansa["nextPlace"], anEvent.Strength, anEvent.Intensity);
                    newEventList.Add(newEvent);
                }
            }
            //zmiana stanu istniejacego zdarzenia
            anEvent.OnHourPassed();
            return newEventList;
        }


        //tworzy ZdarzenieMedyczne na podstawie typu
        public MedicalEvent CreateMedicalEventOfType(string eventName, BodyPart lokalizacjaZdarzenia, float sila, float zaostrzenie)
        {
            MedicalEvent eventTemplate = FindMedicalEvent(eventName);
            return UtworzZdarzenieMedyczneNaPodstawieWzorca(eventTemplate, lokalizacjaZdarzenia, sila, zaostrzenie);
        }


        //tworzy ZdarzenieMedyczne na podstawie szansy
        private MedicalEvent CreateMedicalEventOfChance(BaseDataAddon chance, BodyPart newPlace, float sila, float zaostrzenie)
        {
            MedicalEvent eventTemplate = FindMedicalEvent((string)chance["next"]);
            return UtworzZdarzenieMedyczneNaPodstawieWzorca(eventTemplate, newPlace, sila, zaostrzenie);
        }
        private MedicalEvent UtworzZdarzenieMedyczneNaPodstawieWzorca(MedicalEvent newEvent, BodyPart newPlace, float sila, float zaostrzenie)
        {
            newEvent.Place = newPlace;
            newEvent.Strength = sila;
            newEvent.Intensity = zaostrzenie;
            return newEvent;
        }
    }
}

