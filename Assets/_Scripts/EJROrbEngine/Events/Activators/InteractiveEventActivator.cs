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
    //InteractiveEventActivator is activated by manipulating it with UI (click or other ui activity)
    public class InteractiveEventActivator: BaseEventActivator
    {
        public string TouchSpeech;
        public bool MultiUse = true;

        private bool _firstUpdate;
        private bool _eventsAlreadyFired;
        private bool _initDone;
       
        void Start () 
	    {
            _initDone = false;
            _firstUpdate = true;
		    _eventsAlreadyFired = false;
        }
	
	    void Update () 
	    {
            if (_firstUpdate)
            {
                init();
                _firstUpdate = false;            
            }
        }
       
        private void init()
	    {
            _initDone = true;

            if (GameManager.Instance.TheGameState.KeyExists(gameObject.name + "io"))
		    {
			    _eventsAlreadyFired = true;
		    }
        }
	  
        public void HandleInteraction()
	    {		
		    if(TouchSpeech != null && TouchSpeech != "")
			    FPPUIManager.Instance.PokazMaleInfo(StringsTranslator.GetString(TouchSpeech));
    	    DoEvents(true);		
	    }
	
	    public override void DoEvents(bool doAnimEvents)
	    {
            if (MultiUse || !_eventsAlreadyFired)
            {
                GameManager.Instance.TheGameState.SetKey(gameObject.name + "io", 1);
                BaseEvent[] events = GetComponents<BaseEvent>();
                foreach (BaseEvent ev in events)
                    ev.FireEvent(this);
                _eventsAlreadyFired = true;
                //Debug.Log("event hit: " + gameObject.name);
            }
	    }

	    public override bool EventsAlreadyFired()
	    {
		    return _eventsAlreadyFired;
	    }
    }

}