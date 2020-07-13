// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia projektu Gułag obejmującego serię gier o tematyce ucieczki z Gułagu, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of Gulag project including a series of games about escape from Gulag, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.


using System.Text;
using System.IO;
using ClientAbstract;

namespace ClientWinPC
{
    public class WinPCGameStateManager : BaseGameStateManager
    {
        public WinPCGameStateManager() :base()
        {
           
        }

        protected override void DeleteFile(string nazwaPliku)
        {
            if (File.Exists(SciezkaZapisu() + "/" + nazwaPliku + ".dat"))
                File.Delete(SciezkaZapisu() + "/" + nazwaPliku + ".dat");
        }
        protected override string LoadFile(string nazwaPliku)
        {
            if (!Directory.Exists(SciezkaZapisu()))
                Directory.CreateDirectory(SciezkaZapisu());
            string sciezka = SciezkaZapisu() + "/" + nazwaPliku + ".dat";
            if (File.Exists(sciezka))
            {
                StreamReader strumien = File.OpenText(sciezka);
                string zawartosc = strumien.ReadToEnd();
                strumien.Close();
                return zawartosc;
            }
            else return "";
        }

        protected override void SaveFile(string nazwaPliku)
        {
            if (!Directory.Exists(SciezkaZapisu()))
                Directory.CreateDirectory(SciezkaZapisu());
            StringBuilder sb = new StringBuilder();
            foreach (string key in _zmienne.Keys)
                sb.Append(key + "=" + _zmienne[key] + '\n');
            string zawartosc = sb.ToString();
            StreamWriter strumien;
            strumien = File.CreateText(SciezkaZapisu() + "/" + nazwaPliku + ".dat");
            strumien.WriteLine(zawartosc);
            strumien.Close();
        }
    }
}
