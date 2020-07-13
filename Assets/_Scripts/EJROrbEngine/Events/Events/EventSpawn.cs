// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia projektu Gułag obejmującego serię gier o tematyce ucieczki z Gułagu, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of Gulag project including a series of games about escape from Gulag, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.

using EJROrbEngine.ActiveObjects;

namespace EJROrbEngine.Events
{
    //force spawner to spawn (the game object must have ActiveObjectsSpawner component)
    public class EventSpawn : BaseEvent
    {
        public ActiveObjectsSpawner Spawner;

        public override void FireEvent(BaseEventActivator activator)
        {
            if (Spawner == null)
                Spawner = GetComponent<ActiveObjectsSpawner>();
            if (Spawner != null)
            {
                Spawner.ResetSpawner();
                Spawner.Spawn();
            }
        }


    }
}