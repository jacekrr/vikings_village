// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.

using EJROrbEngine.SceneObjects;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

namespace EJROrbEngine.Herbology
{
    public enum WayOfDose { Dowolny, WewnetrznieMikstura, WewnetrznieWCalosci, ZewnetrznieNacieranie, ZewnetrznieKrem, Inhalacja ,
        Wewnetrznie = WewnetrznieMikstura | WewnetrznieWCalosci,
        Zewnetrznie = ZewnetrznieNacieranie | ZewnetrznieKrem | Inhalacja
    };
    
    // substancja leczaca/trujaca
    public class Substance : BaseDataAddon
    {
        public List<Treatment> TreatmentBySubstance;    // wartosc leczenia jednostki substancji
        public float Ammount;                      // liczba jednostek bazowych substancji
        public List<Harm> GeneralChange;            // lista wplywow na parametry ogolne postaci
        public Substance(): base()
        {
            TreatmentBySubstance = new List<Treatment>();
            GeneralChange = new List<Harm>();
        }

        //klon innej substancji
        public Substance(Substance klonujZTejSubstancji) :base(klonujZTejSubstancji)
        {
            TreatmentBySubstance = new List<Treatment>();
            foreach (Treatment klonLeczenia in klonujZTejSubstancji.TreatmentBySubstance)
                TreatmentBySubstance.Add(new Treatment(klonLeczenia));
            Ammount = klonujZTejSubstancji.Ammount;
            GeneralChange = new List<Harm>();
            foreach (Harm klonWplywu in klonujZTejSubstancji.GeneralChange)
                GeneralChange.Add(new Harm(klonWplywu));
        }

        //zaladuj dane substancji z pojedynczego wezla XML 
        public override void LoadData(XmlDataInfo dataInfo, XElement elementXMLDanych)
        {
            base.LoadData(dataInfo, elementXMLDanych);
            Ammount = 1;
            foreach (XElement podElement in elementXMLDanych.Elements())
            {
                if (podElement.Name == "treatment")
                {
                    Treatment leczenieElement = new Treatment(podElement);
                    TreatmentBySubstance.Add(leczenieElement);
                }
                if (podElement.Name == "generalChange")
                {
                    Harm wplyw = new Harm(podElement);
                    GeneralChange.Add(wplyw);
                }
            }
           
        }
    }

}
