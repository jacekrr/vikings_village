// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.

using EJROrbEngine.SceneObjects;

namespace EJROrbEngine.Characters
{

    public class SceneCharacter : BaseSceneObject
    {      
        public PersonCharacter TheCharacter { get; private set;}
        protected override void OnAwake()
        {

        }
        protected override void OnStart()
        {
            TheCharacter = new PlayerCharacter(CharactersModuleManager.Instance.FindCharacterData(Type));
        }
        protected override void OnUpdate()
        {
        }

    }

}