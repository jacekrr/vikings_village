// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.

using UnityEngine;

namespace EJROrbEngine.Events
{
    //NonInteractiveEventsActivator is activating itself upon first Update or any Update when variable conditions are met
    public class NonInteractiveEventsActivator : BaseEventActivator
    {
        public float CHECKING_TIME = 0.55f;
        public bool activateOnFirstUpdate = false;
        public bool activateOnVariable = false;
        public string VarName;
        public bool checkStrEquals;
        public string strValue;             //valid only if checkStrEquals is set
        public bool checkNumConditions;
        public float numValueGreaterThan = float.MinValue;       //valid only if checkNumConditions is set
        public float numValueSmallerThan = float.MaxValue;       //valid only if checkNumConditions is set

        private bool _firstUpdate;
        private bool _eventsFired;
        private float _checkTime;

        public void ResetActivator()
        {
            _eventsFired = false;
        }

        void Start()
        {
            _firstUpdate = true;
            _eventsFired = false;
            _checkTime = CHECKING_TIME;
        }

        void Update()
        {
            if (_firstUpdate)
            {
                if (activateOnFirstUpdate)
                    DoEvents(true);
                _firstUpdate = false;
            }
            if (activateOnVariable && !_eventsFired)
            {
                _checkTime -= Time.deltaTime;
                if (_checkTime <= 0)
                {
                    if (checkStrEquals)
                    {
                        if (GameManager.Instance.TheGameState.GetStringKey(VarName) == strValue)
                            DoEvents(false);
                    }
                    else if (checkNumConditions)
                    {
                        float val = GameManager.Instance.TheGameState.GetFloatKey(VarName);
                        if (val > numValueGreaterThan && val < numValueSmallerThan)
                            DoEvents(false);
                    }
                    _checkTime = CHECKING_TIME;
                }
            }
        }

        public override void DoEvents(bool doAnimEvents)
        {
            _eventsFired = true;
            BaseEvent[] events = GetComponents<BaseEvent>();
            foreach (BaseEvent ev in events)
                ev.FireEvent(this);
     //       if (GetComponent<StaticSerializator>() != null)
          //      GetComponent<StaticSerializator>().setActive(false);

        }

        public override bool EventsAlreadyFired()
        {
            return _eventsFired;
        }
    }
    //
}