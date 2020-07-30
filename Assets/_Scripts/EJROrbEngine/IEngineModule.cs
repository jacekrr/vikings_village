// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.

using ClientAbstract;

namespace EJROrbEngine
{

    //interface of module manager - class responsible of managing a large and separated part of engine
    //module managers should be created as a component of root game object with access in GameManager
    //in order to easily discover module managers they should be located in Module<Name> folder, EJROrbEngine.<Name> namespace and named as <Name>ModuleManager
    public interface IEngineModule 
    {
        //called by GameManager when game is being loaded
        void OnLoad(IGameState gameState);
        //called by GameManager before OnSave
        void CleanupBeforeSave();
        //called by GameManager when game is being saved
        void OnSave(IGameState gameState);
        //called by GameManager when new game is starting 
        void OnNewGame();
        //called by GameManager on start
        void OnConfigure();
        //called by ActiveObject in order to add module components to the game object
        void OnConfigureObjectRequest(SceneObjects.PrefabTemplate ao);


    }
}