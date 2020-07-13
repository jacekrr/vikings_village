// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.

using UnityEngine;
using System.Collections.Generic;
using System.Xml.Linq;

namespace EJROrbEngine
{
    public class StringsTranslator
    {
        private const string NAZWAPLIKU = "strings";
        private const string SCIEZKA = "Localization/"; //wzgledem katalogu Assets/Resources

        private static string _folderJezyka = "";
        private static List<string> _nakladki;
        private static Dictionary<string, string> _dane;

        public static string GetString(string nazwa)
        {
            if (_dane == null)              
                Zaladuj();
            if (_dane.ContainsKey(nazwa))
                return _dane[nazwa];
            else
                Debug.LogError("Nie znaleziono ciągu o nazwie: " + nazwa);
            return null;
        }
       
        public static bool HasString(string nazwa)
        {
            if (_dane == null)
                Zaladuj();
            return _dane.ContainsKey(nazwa);
        }

        public static void AddAddon(string nazwa)
        {
            if (_nakladki == null)
                _nakladki = new List<string>();
            _nakladki.Add(nazwa);
        }

        private static void Zaladuj()
        {
            string KodJezykaSystemu = KonwertujKodJezyka(Application.systemLanguage);
            if (KodJezykaSystemu.Equals("pl"))
                _folderJezyka = "-pl";
            else
                _folderJezyka = "";
            _dane = new Dictionary<string, string>();
            ZaladujZasoby(SCIEZKA + NAZWAPLIKU + _folderJezyka);
            if(_nakladki != null)
                foreach(string nakladka in _nakladki)
                    ZaladujZasoby(SCIEZKA + NAZWAPLIKU + "_" + nakladka + _folderJezyka);
        }



        private static string KonwertujKodJezyka(SystemLanguage jezyk)
        {
            switch (jezyk)
            {
                case SystemLanguage.English: return "en";
                case SystemLanguage.Polish: return "pl";
                default: return "";
            }
        }
        private static void ZaladujZasoby(string zasobXML)
        {
            //            XDocument dokument = Funkcje.ZaladujZasobXMLJakoDoc(zasobXML, null);
            //          XElement zawartosc = dokument.Element("ciagi");
            //        IEnumerable<XElement> nodeList = zawartosc.Descendants("ciag");
            XmlDataInfo info = Utils.LoadXmlAssetFile(zasobXML, "resources");
            foreach (XElement node in info.MainNodeElements)
            {
                string nazwa = node.Attribute("name").Value;
                string tekst = node.Value;
                tekst = tekst.Replace("\n", " ");
                tekst = tekst.Replace("\\n", "\n");
                if (nazwa == null)
                    Debug.LogError("ciąg ma wartość null: " + tekst + " w " + zasobXML);
                if (_dane.ContainsKey(nazwa))
                    _dane[nazwa] = tekst;
                else
                    _dane.Add(nazwa, tekst);
            }
        }
    }
}