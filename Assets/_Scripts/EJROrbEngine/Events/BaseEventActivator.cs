// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.


using UnityEngine;

namespace EJROrbEngine.Events
{

    public delegate void EventFinishedHandler(GameObject item);

    public abstract class BaseEventActivator : MonoBehaviour
    {
        public abstract void DoEvents(bool doAnimEvents);
        public abstract bool EventsAlreadyFired();
    }
    public abstract class BaseEvent : MonoBehaviour
    {
        public abstract void FireEvent(BaseEventActivator activator);
	    
    }

}