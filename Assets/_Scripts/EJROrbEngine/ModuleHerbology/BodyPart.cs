// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.

using System;
using UnityEngine;

namespace EJROrbEngine.Herbology
{
    [Flags]
    public enum BodyPart
    {
        Glowa = 0,
        Oczy =  0x01,
        Uszy = 0x02,
        Nos = 0x04,
        Mozg = 0x08,
        JamaUstna = 0x10,
        Krtan = 0x20,
        Gardlo = 0x40,
        Tchawica = 0x80,
        Oskrzela = 0x100,
        Pluca = 0x200,
        Serce = 0x400,
        Ramiona = 0x800,
        Lokcie = 0x1000,
        Dlonie = 0x2000,
        Szyja = 0x4000,
        Zebra = 0x8000,
        Zoladek = 0x10000,
        Watroba = 0x20000,
        Trzustka = 0x40000,
        Pecherz = 0x80000,
        JelitoCienkie = 0x100000,
        JelitoGrube = 0x200000,
        NarzadyRozrodcze = 0x400000,
        Posladki = 0x800000,
        Uda = 0x1000000,
        Lydki = 0x2000000,
        Stopy = 0x4000000,
        Kostki = 0x8000000,
        Nerki = 0x10000000,
        Brzuch = 0x20000000,
        WieleNarzadow = 0x40000000,
        //zbiorcze
        GorneDrogiOddechowe = JamaUstna | Gardlo | Krtan | Tchawica,
        DolneDrogiOddechowe = Oskrzela | Pluca,
        DrogiOddechowe = GorneDrogiOddechowe | DolneDrogiOddechowe,
        Rece = Ramiona | Lokcie | Dlonie,
        Nogi = Uda | Lydki | Kostki | Stopy,
        Tulow = Brzuch | Zebra,
        UkladTrawienny = Zoladek | JelitoCienkie | JelitoGrube | Watroba | Trzustka,
        UkladMoczowy = Pecherz | Nerki,
        Zewnetrzne = Glowa | Rece | Tulow | Nogi | Uszy | Nos |Szyja | Posladki,
        Wewnetrzne = DrogiOddechowe | UkladTrawienny | UkladMoczowy | NarzadyRozrodcze | Serce | Oczy | Mozg | Zebra,
        //wszystko
        Dowolna = Zewnetrzne | Wewnetrzne | WieleNarzadow
     
    };

    public class BodyPartMechanics
    {
        public static float BodyPartMultiplier(BodyPart czesc)
        {
            switch (czesc)
            {
                case BodyPart.Glowa: return 0.35f;
                case BodyPart.Oczy: return 0.25f;
                case BodyPart.Mozg: return 1f;
                case BodyPart.Tchawica: return 0.15f;
                case BodyPart.Oskrzela: return 0.25f;
                case BodyPart.Pluca: return 0.45f;
                case BodyPart.Serce: return 1f;
                case BodyPart.Szyja: return 0.2f;
                case BodyPart.Zoladek: return 0.25f;
                case BodyPart.Watroba: return 0.4f;
                case BodyPart.Trzustka: return 0.3f;
                case BodyPart.Pecherz: return 0.2f;
                case BodyPart.JelitoCienkie: return 0.2f;
                case BodyPart.JelitoGrube: return 0.2f;
                case BodyPart.Nerki: return 0.35f;
                case BodyPart.Brzuch: return 0.15f;
                case BodyPart.WieleNarzadow: return 1f;
                case BodyPart.GorneDrogiOddechowe: return 0.4f;
                case BodyPart.DolneDrogiOddechowe: return 0.6f;
                case BodyPart.DrogiOddechowe: return 0.8f;
                case BodyPart.Rece: return 0.3f;
                case BodyPart.Nogi: return 0.3f;
                case BodyPart.Tulow: return 0.2f;
                case BodyPart.UkladTrawienny: return 0.5f;
                case BodyPart.UkladMoczowy: return 0.5f;
                case BodyPart.Zewnetrzne: return 0.6f;
                case BodyPart.Wewnetrzne: return 0.9f;
                case BodyPart.Dowolna: return 1f;
            }
            return 0.1f;
        }
        public static float MinimumStrengthOfBodyPart(BodyPart czesc)
        {
            switch (czesc)
            {
                case BodyPart.Glowa: return 0.15f;
                case BodyPart.Oczy: return 0.1f;
                case BodyPart.Mozg: return 0.5f;
                case BodyPart.Oskrzela: return 0.1f;
                case BodyPart.Pluca: return 0.25f;
                case BodyPart.Serce: return 0.5f;
                case BodyPart.Zoladek: return 0.1f;
                case BodyPart.Watroba: return 0.15f;
                case BodyPart.Trzustka: return 0.1f;
                case BodyPart.Pecherz: return 0.1f;
                case BodyPart.JelitoCienkie: return 0.1f;
                case BodyPart.JelitoGrube: return 0.1f;
                case BodyPart.Nerki: return 0.2f;
                case BodyPart.WieleNarzadow: return 0.35f;
                case BodyPart.DolneDrogiOddechowe: return 0.15f;
                case BodyPart.UkladTrawienny: return 0.1f;
                case BodyPart.UkladMoczowy: return 0.1f;
            }
            return 0f;
        }
        public static float MaximumStrengthOfBodyPart(BodyPart czesc)
        {
            switch (czesc)
            {
                case BodyPart.Tchawica: return 0.8f;
                case BodyPart.Oskrzela: return 0.9f;
                case BodyPart.Brzuch: return 0.9f;
                case BodyPart.GorneDrogiOddechowe: return 0.9f;
                case BodyPart.Rece: return 0.8f;
                case BodyPart.Nogi: return 0.9f;
                case BodyPart.Tulow: return 0.9f;
            }
            return 1f;
        }
        public static Vector2[] GetBodyMapPoint(BodyPart czesc)
        {
            Vector2[] lista;
            switch (czesc)
            {

                case BodyPart.Glowa: lista = new Vector2[] { new Vector2 { x = 0.51f, y = 0.083f } }; break;
                case BodyPart.Oczy: lista = new Vector2[] { new Vector2 { x = 0.48f, y = 0.1125f }, new Vector2 { x = 0.536f, y = 0.1125f } }; break;
                case BodyPart.Uszy: lista = new Vector2[] { new Vector2 { x = 0.406f, y = 0.119f }, new Vector2 { x = 0.612f, y = 0.119f } }; break;
                case BodyPart.Nos: lista = new Vector2[] { new Vector2 { x = 0.51f, y = 0.0958f } }; break;
                case BodyPart.JamaUstna: lista = new Vector2[] { new Vector2 { x = 0.51f, y = 0.149f } }; break;
                case BodyPart.Mozg: lista = new Vector2[] { new Vector2 { x = 0.51f, y = 0.1325f } }; break;
                case BodyPart.Krtan: lista = new Vector2[] { new Vector2 { x = 0.51f, y = 0.165f } }; break;
                case BodyPart.Gardlo: lista = new Vector2[] { new Vector2 { x = 0.51f, y = 0.15f } }; break;
                case BodyPart.Tchawica: lista = new Vector2[] { new Vector2 { x = 0.51f, y = 0.18f } }; break;
                case BodyPart.Oskrzela: lista = new Vector2[] { new Vector2 { x = 0.51f, y = 0.213f } }; break;
                case BodyPart.Pluca: lista = new Vector2[] { new Vector2 { x = 0.404f, y = 0.258f }, new Vector2 { x = 0.616f, y = 0.258f } }; break;
                case BodyPart.Serce: lista = new Vector2[] { new Vector2 { x = 0.612f, y = 0.2758f } }; break;
                case BodyPart.Ramiona: lista = new Vector2[] { new Vector2 { x = 0.276f, y = 0.2625f }, new Vector2 { x = 0.748f, y = 0.2625f } }; break;
                case BodyPart.Lokcie: lista = new Vector2[] { new Vector2 { x = 0.208f, y = 0.4f }, new Vector2 { x = 0.816f, y = 0.4f } }; break;
                case BodyPart.Dlonie: lista = new Vector2[] { new Vector2 { x = 0.17f, y = 0.5416f }, new Vector2 { x = 0.866f, y = 0.5416f } }; break;
                case BodyPart.Szyja: lista = new Vector2[] { new Vector2 { x = 0.51f, y = 0.1625f } }; break;
                case BodyPart.Zebra: lista = new Vector2[] { new Vector2 { x = 0.51f, y = 0.267f } }; break;
                case BodyPart.Zoladek: lista = new Vector2[] { new Vector2 { x = 0.638f, y = 0.3375f } }; break;
                case BodyPart.Watroba: lista = new Vector2[] { new Vector2 { x = 0.432f, y = 0.325f } }; break;
                case BodyPart.Trzustka: lista = new Vector2[] { new Vector2 { x = 0.51f, y = 0.35f } }; break;
                case BodyPart.Pecherz: lista = new Vector2[] { new Vector2 { x = 0.51f, y = 0.484f } }; break;
                case BodyPart.JelitoCienkie: lista = new Vector2[] { new Vector2 { x = 0.51f, y = 0.433f } }; break;
                case BodyPart.JelitoGrube: lista = new Vector2[] { new Vector2 { x = 0.51f, y = 0.471f } }; break;
                case BodyPart.NarzadyRozrodcze: lista = new Vector2[] { new Vector2 { x = 0.51f, y = 0.52f } }; break;
                case BodyPart.Posladki: lista = new Vector2[] { new Vector2 { x = 0.406f, y = 0.52f }, new Vector2 { x = 0.62f, y = 0.52f } }; break;
                case BodyPart.Uda: lista = new Vector2[] { new Vector2 { x = 0.372f, y = 0.61f }, new Vector2 { x = 0.652f, y = 0.61f } }; break;
                case BodyPart.Lydki: lista = new Vector2[] { new Vector2 { x = 0.29f, y = 0.8025f }, new Vector2 { x = 0.734f, y = 0.8025f } }; break;
                case BodyPart.Stopy: lista = new Vector2[] { new Vector2 { x = 0.268f, y = 0.935f }, new Vector2 { x = 0.755f, y =0.935f } }; break;
                case BodyPart.Kostki: lista = new Vector2[] { new Vector2 { x = 0.282f, y = 0.893f }, new Vector2 { x = 0.74f, y = 0.893f } }; break;
                case BodyPart.Nerki: lista = new Vector2[] { new Vector2 { x = 0.43f, y = 0.4292f }, new Vector2 { x = 0.59f, y = 0.4292f } }; break;
                case BodyPart.Brzuch: lista = new Vector2[] { new Vector2 { x = 0.51f, y = 0.3875f } }; break;
                case BodyPart.WieleNarzadow: lista = new Vector2[] { new Vector2 { x = 0.51f, y = 0.1033f }, new Vector2 { x = 0.51f, y = 0.297f }, new Vector2 { x = 0.51f, y = 0.4567f }, new Vector2 { x = 0.66f, y = 0.3258f } }; break;
                case BodyPart.GorneDrogiOddechowe: lista = new Vector2[] { new Vector2 { x = 0.51f, y = 0.164f } }; break;
                case BodyPart.DolneDrogiOddechowe: lista = new Vector2[] { new Vector2 { x = 0.51f, y = 0.2458f } }; break;
                case BodyPart.DrogiOddechowe: lista = new Vector2[] { new Vector2 { x = 0.51f, y = 0.204f } }; break;
                case BodyPart.Rece: lista = new Vector2[] { new Vector2 { x = 0.208f, y = 0.4f }, new Vector2 { x = 0.816f, y = 0.4f } }; break;
                case BodyPart.Nogi: lista = new Vector2[] { new Vector2 { x = 0.366f, y = 0.6741f }, new Vector2 { x = 0.66f, y = 0.6741f } }; break;
                case BodyPart.Tulow: lista = new Vector2[] { new Vector2 { x = 0.51f, y = 0.335f } }; break;
                case BodyPart.UkladTrawienny: lista = new Vector2[] { new Vector2 { x = 0.51f, y = 0.4075f } }; break;
                case BodyPart.UkladMoczowy: lista = new Vector2[] { new Vector2 { x = 0.51f, y = 0.458f } }; break;
                case BodyPart.Zewnetrzne: lista = new Vector2[] { new Vector2 { x = 0.198f, y = 0.2958f }, new Vector2 { x = 0.832f, y = 0.2958f }, new Vector2 { x = 0.292f, y = 0.674f }, new Vector2 { x = 0.74f, y = 0.674f } }; break;
                case BodyPart.Wewnetrzne: lista = new Vector2[] { new Vector2 { x = 0.51f, y = 0.2958f }, new Vector2 { x = 0.51f, y = 0.419f } }; break;
                //PG   -11.23 6
                //LD   -6.23 -6
                //WIDTH = 5  HEIGHT = 12
                default: lista = new Vector2[] { }; break;

            }
            for (int i = 0; i < lista.Length; i++)
            {
                lista[i].x = lista[i].x * 5 - 11.23f;
                lista[i].y = 6 - 12 * lista[i].y;
            }
            return lista;
        }
    }

     
}