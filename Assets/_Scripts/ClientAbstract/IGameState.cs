// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.

namespace ClientAbstract
{
    public interface IGameState
    {
        // utworzenie nowej gry, skasowanie starego zapisu
        void CreateNewGame();

        //zaladowanie stanu zmiennych gry z trwalego zapisu
        void LoadGame();

        //zapisanie stanu zmiennych gry do trwalego zapisu
        void SaveGame();

        //pobranie zmiennej typu int o nazwie nazwaKlucza 
        int GetIntKey(string keyName);

        //pobranie zmiennej typu float o nazwie nazwaKlucza 
        float GetFloatKey(string keyName);

        //pobranie zmiennej typu string o nazwie nazwaKlucza 
        string GetStringKey(string keyName);

        //zapisanie zmiennej zmienna pod nazwa nazwaKlucza
        void SetKey(string keyName, object varValue);

        //usuniecie klucza o nazwie nazwaKlucza
        void DeleteKey(string keyName);

        //zwraca true jesli istnieje klucz o nazwie nazwaKlucza
        bool KeyExists(string keyName);

        //usuniecie wszystkich kluczy
        void ClearAllKeys();

    }
}