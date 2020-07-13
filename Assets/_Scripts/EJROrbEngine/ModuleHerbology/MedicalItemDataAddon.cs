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

    //przedmiot ktory moze zawierac rozne substancje leczace/trujace
    public class MedicalItemDataAddon : BaseDataAddon
    {

        public int DurationRemained;                     // czas działania (godziny)
        public List<Substance> ActiveSubstances;         // lista substancji leczniczych (sa to klony substancji, nie sa to substancje wprost z listy wzorcow w MenedzerzeZielarstwa)
        public int Duration {  get { return (int)this["duration"]; } }
        public MedicalItemDataAddon()
        {
            ActiveSubstances = new List<Substance>();
        }
        //skopiuj z innego przedmiotu-wzorca
        public MedicalItemDataAddon(MedicalItemDataAddon wzorzec):base(wzorzec)
        {
            
            ActiveSubstances = new List<Substance>();
            Type = wzorzec.Type;            
            DurationRemained = wzorzec.DurationRemained;
            foreach (Substance wzorzecSubstancja in wzorzec.ActiveSubstances)
                ActiveSubstances.Add(new Substance(wzorzecSubstancja));
        }
        //zaladuj dane przedmiotu z pojedynczego wezla XML 
        public void LoadData(XmlDataInfo dataInfo, XElement elementXMLDanych, List<Substance> listaWzorcowSubstancji)
        {
            base.LoadData(dataInfo, elementXMLDanych);         
            DurationRemained = Duration ;
            foreach (XElement podElement in elementXMLDanych.Elements())
            {
                if (podElement.Name == "substance")
                {
                    if(podElement.Attribute("type") == null)
                        Debug.LogError("Brak atrybutu type w elemencie danych " + podElement.ToString());
                    else
                    {
                        string nazwaSubstancji = podElement.Attribute("type").Value;
                        Substance ZnalezionaSubstancja = null;
                        foreach(Substance subst in listaWzorcowSubstancji)
                        {
                            if (subst.Type == nazwaSubstancji)
                                ZnalezionaSubstancja = new Substance(subst);
                        }
                        if(ZnalezionaSubstancja == null)
                            Debug.LogError("Nie można znaleźć substancji o nazwie " + nazwaSubstancji);
                        else
                        {
                            if (podElement.Attribute("ammount") == null)
                                ZnalezionaSubstancja.Ammount = 1;
                            else
                                ZnalezionaSubstancja.Ammount = int.Parse(podElement.Attribute("ammount").Value );
                            ActiveSubstances.Add(ZnalezionaSubstancja);
                        }
                    }
                }
            }
        }
        public void UseDose()
        {
            DurationRemained--;
        }
        public float GetHourTreatment(MedicalEvent zdarzenie)
        {
           foreach (Substance subst in ActiveSubstances)
           {
                foreach (Treatment leczenie in subst.TreatmentBySubstance)
                    if (leczenie.MinimalDose <= subst.Ammount && leczenie.EventType == zdarzenie.Type && (leczenie.WhereTreats & zdarzenie.Place) != 0 && DurationRemained > 0)
                        return leczenie.TreatmentValue / Duration;
            }
            return 0;
        }
        public List<MedicalEvent>  GetRandomEvent()
        {

            List<MedicalEvent> zdarzenia = new List<MedicalEvent>();
            /*
            foreach (Substance subst in ActiveSubstances)
            {
                foreach (Treatment leczenie in subst.TreatmentBySubstance)
                {
                    float noweZaostrzenie = Random.Range(0f, 1f) - 0.5f;
                    if (leczenie.TreatmentValue < 0 && leczenie.MinimalDose <= subst.Ammount && Random.Range(0, 1f) < -leczenie.TreatmentValue && DurationRemained > 0)
                        zdarzenia.Add(MenedzerZielarstwa.Instancja.UtworzZdarzenieMedyczneNaPodstawieTypu(leczenie.EventType, leczenie.WhereTreats, Random.Range(0, -leczenie.TreatmentValue), noweZaostrzenie));
                }
            }*/
            return zdarzenia;
        }
        public List<Harm> GetGeneralChange()
        {
            List<Harm> wplywOgolny = new List<Harm>();
            foreach (Substance subst in ActiveSubstances)
                wplywOgolny.AddRange(subst.GeneralChange);
            return wplywOgolny;
        }
        public void ResetDuration()
        {
            DurationRemained = Duration;
        }

    }


}
