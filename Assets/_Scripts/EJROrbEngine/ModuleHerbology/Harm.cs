// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.


using System.Globalization;
using System.Xml.Linq;
using UnityEngine;
namespace EJROrbEngine.Herbology
{
    //wplyw zdarzenia na parametr ogolny postaci
    public class Harm
    {
        public string ResName;     //id parametru (np. "health" - zdrowie ogolne)
        public float Value;       //wartosc bedaca srednia zmiana parametru na godzine

        //zaladuj dane  z pojedynczego wezla XML zdarzen
        public Harm(XElement elementXMLDanych)
        {
            if (elementXMLDanych.Attribute("res") == null)
                Debug.LogError("Brak atrybutu res w elemencie danych " + elementXMLDanych.ToString());
            else ResName = elementXMLDanych.Attribute("res").Value;
            if (elementXMLDanych.Attribute("val") == null)
                Debug.LogError("Brak atrybutu val w elemencie danych " + elementXMLDanych.ToString());
            else
                Value = float.Parse(elementXMLDanych.Attribute("val").Value, CultureInfo.InvariantCulture);
        }

        public Harm(Harm klonujZTego)
        {
            ResName = klonujZTego.ResName;
            Value = klonujZTego.Value;
        }
    }
  


}