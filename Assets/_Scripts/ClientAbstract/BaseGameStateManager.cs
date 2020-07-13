// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.


using UnityEngine;
using System.Collections.Generic;

namespace ClientAbstract
{
    public abstract class BaseGameStateManager : IGameState
    { 
        protected Dictionary<string, string> _zmienne;
        public const string DEFAULT_SAVE_NAME = "autozapis";
        public const string DEFAULT_SAVE_DIR = "Zapis";
      
        // ***** konstruktor chroniony 
        protected BaseGameStateManager()
        {
            _zmienne = new Dictionary<string, string>();
        }

        // ***** implementacja interfejsu IStanGry
        public void CreateNewGame()
    	{
            DeleteFile(DEFAULT_SAVE_NAME);    	
    		_zmienne.Clear();
    	}
        public void LoadGame()
        {
            string zawartosc = LoadFile(DEFAULT_SAVE_NAME);
            KonwertujZawartoscDoZmiennych(zawartosc);
        }

        public void SaveGame()
        {
            SaveFile(DEFAULT_SAVE_NAME);
        }
        
        public int GetIntKey(string nazwaKlucza)
        {
            if (_zmienne.ContainsKey(nazwaKlucza))
            {
                if (_zmienne[nazwaKlucza] == "")
                    return 0;
                try
                {
                    return int.Parse(_zmienne[nazwaKlucza]);
                }
                catch (System.Exception )
                { Debug.LogError("Błąd konwersji: " + nazwaKlucza + ":" + _zmienne[nazwaKlucza]); }
            }
            return 0;
        }
        public float GetFloatKey(string nazwaKlucza)
        {
            if (_zmienne.ContainsKey(nazwaKlucza))
            {
                if (_zmienne[nazwaKlucza] == "")
                    return 0f;
                try
                {
                    return float.Parse(_zmienne[nazwaKlucza], System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (System.Exception )
                { Debug.LogError("Błąd konwersji: " + nazwaKlucza + ":" + _zmienne[nazwaKlucza]); }
            }
            return 0f;
        }
        public string GetStringKey(string nazwaKlucza)
        {
            if (_zmienne.ContainsKey(nazwaKlucza))
            {
                return _zmienne[nazwaKlucza];
            }
            return "";
        }

        public void SetKey(string nazwaKlucza, object zmienna)
        {
            if (!_zmienne.ContainsKey(nazwaKlucza))
                _zmienne.Add(nazwaKlucza, zmienna.ToString());
            else _zmienne[nazwaKlucza] = zmienna.ToString();
        }

        public void DeleteKey(string nazwaKlucza)
        {
            if (_zmienne.ContainsKey(nazwaKlucza))
                _zmienne.Remove(nazwaKlucza);
        }

        public bool KeyExists(string nazwaKlucza)
        {
            return  _zmienne.ContainsKey(nazwaKlucza);
        }

        public void ClearAllKeys()
        {
            _zmienne.Clear();
        }

        // ***** prywatne i chronione metody wewnetrzne
        protected virtual string SciezkaZapisu()
        {
            return DEFAULT_SAVE_DIR;
        }       
       
        private void KonwertujZawartoscDoZmiennych(string zawartosc)
        {
            _zmienne = new Dictionary<string, string>();
             string[] tokeny = zawartosc.Split('\n');
             for (int i = 0; i < tokeny.Length; i++)
             {
                 int pozycjaWartosci = tokeny[i].IndexOf("=");
                 if (pozycjaWartosci > 0)
                     SetKey(tokeny[i].Substring(0, pozycjaWartosci), tokeny[i].Substring(pozycjaWartosci + 1));
            }
        }

        // ***** metody wewnetrzne implementowane w klasach wyprowadzonych
        protected abstract void DeleteFile(string fileName);
        protected abstract string LoadFile(string fileName);
        protected abstract void SaveFile(string fileName);
    
    }
}