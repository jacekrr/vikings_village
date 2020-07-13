// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.


namespace EJROrbEngine.Events
{
    //change variable state (it may also add a numeric value to a numeric variable)
    public class EventChangeVar : BaseEvent
    {

        public string VarName;
        public bool NumericValue;
        public string strValue;             //valid only if NumeriValue is not set
        public bool resetNumValue;          //valid only if NumericValue is set,  resets value to 0 before adding numAddValue
        public float numAddValue;           //valid only if NumericValue is set 
        public bool DeleteVar;
        public bool ResetActivator = false;  //let non interactive activator run events again when its conditions are met (watch out for infitinitive event activation)

        public override void FireEvent(BaseEventActivator activator)
        {
            if (DeleteVar)
                GameManager.Instance.TheGameState.DeleteKey(VarName);
            else
            if (NumericValue)
            {
                float val = GameManager.Instance.TheGameState.GetFloatKey(VarName);
                if (resetNumValue)
                    val = 0f;
                val += numAddValue;
                GameManager.Instance.TheGameState.SetKey(VarName, val);
            }
            else
            {
                GameManager.Instance.TheGameState.SetKey(VarName, strValue);
            }
            if (ResetActivator && GetComponent<NonInteractiveEventsActivator>())
                GetComponent<NonInteractiveEventsActivator>().ResetActivator();
        }


    }
}