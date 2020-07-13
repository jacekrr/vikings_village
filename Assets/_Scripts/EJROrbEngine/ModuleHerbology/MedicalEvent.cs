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

    public class MedicalEvent: BaseDataAddon
    {

        public BodyPart Place;                  // informacja gdzie zdarzenie wystapilo
        public float Strength;                      // intensywnosc zdarzenia ( aktualna)
        public float Intensity;               // przewlekosc/ostrosc zdarzenia czyli tempo narastania, jest to ulamek bedacy srednim przyrostem sily na dobe
        public List<Harm> GeneralHarm;                  // lista wplywow na parametry ogolne postaci

        public MedicalEvent() : base()
        {
            GeneralHarm = new List<Harm>();
        }
        public MedicalEvent(MedicalEvent wzorzec): base(wzorzec)
        {            
            Place = BodyPart.Dowolna;
            Strength = 0.5f;
            Intensity = 0;
            GeneralHarm = new List<Harm>();
            foreach (Harm harm in wzorzec.GeneralHarm)
                GeneralHarm.Add(new Harm(harm));
        }

        //zaladuj dane wzorca zdarzenia z pojedynczego wezla XML zdarzen
        public override void LoadData(XmlDataInfo dataInfo, XElement elementXMLDanych)
        {
            base.LoadData(dataInfo, elementXMLDanych);
            foreach (XElement podElement in elementXMLDanych.Elements())
            {
                if (podElement.Name == "harm")
                {
                    Harm szkodliwosc = new Harm(podElement);
                    GeneralHarm.Add(szkodliwosc);
                }
            }
        }
        // cogodzinne przetworzenie wewnetrznego stanu zdarzenia
        public void OnHourPassed()
        {
            Strength += Intensity / 12;
            Intensity += 2*( Random.Range(0, 0.1f) - 0.05f);
            Normalize();
        }
        //chcemy aby sila byla w zakresie MinimalnaSila..MaksymalnaSila a Zaostrzenie -0.5..0.5
        public void Normalize()
        {
            if ((float)this["min"] < BodyPartMechanics.MinimumStrengthOfBodyPart(Place))
                this["min"] = BodyPartMechanics.MinimumStrengthOfBodyPart(Place);
            if ((float)this["max"] > BodyPartMechanics.MaximumStrengthOfBodyPart(Place))
                this["max"] = BodyPartMechanics.MaximumStrengthOfBodyPart(Place);
            if (Strength < (float)this["min"])
                Strength = (float)this["min"];
            if (Strength > (float)this["max"])
                Strength = (float)this["max"];
            if (Intensity < -0.5)
                Intensity = -0.5f;
            if (Intensity > 0.5)
                Intensity = 0.5f;
        }

        public string StrengthFriendly()
        {
            if (Strength < 0.25f)
                return StringsTranslator.GetString("ZdarzenieNiskie");
            if (Strength < 0.5f)
                return StringsTranslator.GetString("ZdarzenieSrednie");
            if (Strength < 0.75f)
                return StringsTranslator.GetString("ZdarzeniePowazne");
            return StringsTranslator.GetString("ZdarzenieKrytyczne");
        }
        public int StrengthAsInt()
        {
            if (Strength < 0.25f)
                return 0;
            if (Strength < 0.5f)
                return 1;
            if (Strength < 0.75f)
                return 2;
            return 3;
        }
        public string FriendlyName()
        {
            if (StringsTranslator.HasString(Type + Place) )
                return StringsTranslator.GetString(Type + Place) ;
            else if (StringsTranslator.HasString(Type) && StringsTranslator.HasString(Place + "dop"))
                return StringsTranslator.GetString(Type) + " " + StringsTranslator.GetString(Place + "dop");
            else
                return "<" + Type + " " + Place + ">";
        }
    }


}