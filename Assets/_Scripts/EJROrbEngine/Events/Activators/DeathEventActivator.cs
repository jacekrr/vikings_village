// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.


using EJROrbEngine.FPPGame.UI;
using UnityEngine;

namespace EJROrbEngine.Events
{
    //DeathEventActivator is activated after NPC's death
    public class DeathEventActivator: BaseEventActivator
    {
        public string DeathSpeech; 
        private bool _eventsAlreadyFired;
       
        void Start () 
	    {
		    _eventsAlreadyFired = false;
        }
	     	  
        public void OnDeath()
	    {		
		    if(DeathSpeech != null && DeathSpeech != "")
			    FPPUIManager.Instance.PokazMaleInfo(StringsTranslator.GetString(DeathSpeech));
    	    DoEvents(true);		
	    }
	
	    public override void DoEvents(bool doAnimEvents)
	    {
    	    BaseEvent[] events = GetComponents<BaseEvent>();
	        foreach (BaseEvent ev in events) 
                ev.FireEvent( this);			 
		    _eventsAlreadyFired = true;
		    //Debug.Log("event hit: " + gameObject.name);
	    }

	    public override bool EventsAlreadyFired()
	    {
		    return _eventsAlreadyFired;
	    }
    }

}