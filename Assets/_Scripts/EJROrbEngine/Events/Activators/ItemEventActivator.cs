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
    //ItemEventActivator is activated by touching or throwing an item to it
    public class ItemEventActivator: BaseEventActivator
    {        
        //if activation needs that player got N items it should be constructed as "itemID_multiN" other way it should be itemID
        //only items with multi flag could be stacked in inventory
        public string[] AcceptItems;
        public string ActivationSpeech;
        public string WrongDragSpeech;
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
        void OnCollisionEnter(Collision collision)
        {
            if (!_initDone) init();
            if (collision.transform.GetComponent<InteractiveEventActivator>() != null )
            {
                if (isObjectAccepted(collision.transform.gameObject) == 1)
                {
                    DoEvents( false);
                    Debug.Log("I am: " + name + " and I'm hit by a: " + collision.transform.name);
                }
            }
           
        }
        private void OnTriggerEnter(Collider other)
        {
            if (!_initDone) init();
            if (other.transform.GetComponent<InteractiveEventActivator>() != null )
            {
                if (isObjectAccepted(other.transform.gameObject) == 1)
                {
                    DoEvents( false);
                    Debug.Log("I am: " + name + " and I'm hit by a: " + other.transform.name);
                }
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
	  
	    public override void DoEvents(bool doAnimEvents)
	    {
            if (MultiUse || !_eventsAlreadyFired)
            {
                GameManager.Instance.TheGameState.SetKey(gameObject.name + "io", 1);

                if (ActivationSpeech != null && ActivationSpeech != "")
                    FPPUIManager.Instance.PokazMaleInfo(StringsTranslator.GetString(ActivationSpeech));

                BaseEvent[] events = GetComponents<BaseEvent>();
                foreach (BaseEvent ev in events)
                    ev.FireEvent(this);

                _eventsAlreadyFired = true;
                //Debug.Log("event hit: " + gameObject.name);
            }
	    }

        //returns how many times item shouild be used, 0 means that item is not accepted
        public int isObjectAccepted(GameObject itemObj)
        {
            return isObjectAccepted(itemObj, "");
        }

        public int isObjectAccepted( GameObject itemObj, string action)
	    {		
		    int accept = 0;
		    foreach (var a_obj in AcceptItems) 
		    {
			    if(a_obj == itemObj.name + action)
				  accept = 1;
		        if(accept == 0)
			        badDragSpeech(WrongDragSpeech);
		    }
		    return accept;
	    }
	    private void badDragSpeech(string dragSp)
	    {
            /*
		    if (dragSp == null || dragSp == "")
		    {
			    float x = Random.value;
			    if( x < 0.33)
				    transform.root.gameObject.GetComponent<GUIManagerBase>().setActiveSpeech(AndroidStringsReader.GetString("general_inavliddrag1"), null);
			    else if( x < 0.66)
                    transform.root.gameObject.GetComponent<GUIManagerBase>().setActiveSpeech(AndroidStringsReader.GetString("general_inavliddrag2"), null);
			    else
                    transform.root.gameObject.GetComponent<GUIManagerBase>().setActiveSpeech(AndroidStringsReader.GetString("general_inavliddrag3"), null);
		    }
		    else
                    transform.root.gameObject.GetComponent<GUIManagerBase>().setActiveSpeechById(dragSp, null);
                    */
	    }

	    public override bool EventsAlreadyFired()
	    {
		    return _eventsAlreadyFired;
	    }
    }
//
}