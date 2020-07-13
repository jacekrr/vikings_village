// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.


using ClientAbstract;
using System.Text;
using UnityEngine;

namespace ClientAndroid

{
    public class AndroidGameStateManager : BaseGameStateManager
    {
        public AndroidGameStateManager() :base()
        {
           
        }

        protected override void DeleteFile(string nazwaPliku)
        {
            if (PlayerPrefs.HasKey(SciezkaZapisu() + "_" + nazwaPliku))
                PlayerPrefs.DeleteKey(SciezkaZapisu() + "_" + nazwaPliku);          
        }
        protected override string LoadFile(string nazwaPliku)
        {
            string zawartosc = PlayerPrefs.GetString(SciezkaZapisu() + "_" + nazwaPliku, "");
            return zawartosc;            
        }

        protected override void SaveFile(string nazwaPliku)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string key in _zmienne.Keys)
                sb.Append(key + "=" + _zmienne[key] + '\n');
            string zawartosc = sb.ToString();
            PlayerPrefs.SetString(SciezkaZapisu() + "_" + nazwaPliku, zawartosc);
        }
    }
}
