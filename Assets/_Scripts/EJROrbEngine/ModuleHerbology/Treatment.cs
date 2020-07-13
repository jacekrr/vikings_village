// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.

using System;
using System.Globalization;
using System.Xml.Linq;
using UnityEngine;

namespace EJROrbEngine.Herbology
{

    //informacja o tym jak mocne jest leczenie konkretnego zdarzenia przez substancje lecznicza
    public class Treatment
    {
        public string EventType;                     // jakie zdarzenie
        public BodyPart WhereTreats;                   // w jakich lokalizacjach jest w stanie leczyc zdarzenie
        public float TreatmentValue;                          // sredni spadek sily zdarzenia na godzine, UWAGA: wartosc ujemna jest mozliwa i oznacza substancje szkodzaca a nie leczaca
        public float MinimalDose;                  // minimalna liczba jednostek bazowych substancji leczniczej od ktorej aktywuje sie leczenie/zatrucie
        public WayOfDose AllowedWayOfDose;       // sposob podania substancji leczniczej wymagany do aktywacji leczenia/zatrucia

        //konstruktor dla tworzenia struktury ktorej dane beda ladowane z wezla XML
        public Treatment(XElement elementXMLDanych)
        {
            if (elementXMLDanych.Attribute("eventType") == null)
                Debug.LogError("Brak atrybutu eventType w elemencie danych " + elementXMLDanych.ToString());
            else EventType = elementXMLDanych.Attribute("eventType").Value;

            if (elementXMLDanych.Attribute("whereTreats") == null)
                Debug.LogError("Brak atrybutu whereTreats w elemencie danych " + elementXMLDanych.ToString());
            else
                WhereTreats = (BodyPart)Enum.Parse(typeof(BodyPart), elementXMLDanych.Attribute("whereTreats").Value);

            if (elementXMLDanych.Attribute("treatment") == null)
                Debug.LogError("Brak atrybutu treatment w elemencie danych " + elementXMLDanych.ToString());
            else TreatmentValue = float.Parse(elementXMLDanych.Attribute("treatment").Value, CultureInfo.InvariantCulture);

            if (elementXMLDanych.Attribute("minimalDose") == null)
                MinimalDose = 1;
            else MinimalDose = float.Parse(elementXMLDanych.Attribute("minimalDose").Value, CultureInfo.InvariantCulture);

            if (elementXMLDanych.Attribute("way") == null)
                Debug.LogError("Brak atrybutu way w elemencie danych " + elementXMLDanych.ToString());
            else AllowedWayOfDose = (WayOfDose)Enum.Parse(typeof(WayOfDose), elementXMLDanych.Attribute("way").Value);

        }

        //konstruktor kopiujacy dane z klona struktury
        public Treatment(Treatment klonujZTejStruktury)
        {
            EventType = klonujZTejStruktury.EventType;
            WhereTreats = klonujZTejStruktury.WhereTreats;
            TreatmentValue = klonujZTejStruktury.TreatmentValue;
            MinimalDose = klonujZTejStruktury.MinimalDose;
            AllowedWayOfDose = klonujZTejStruktury.AllowedWayOfDose;
        }

    }

   
}
