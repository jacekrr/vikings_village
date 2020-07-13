// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia projektu Gułag obejmującego serię gier o tematyce ucieczki z Gułagu, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of Gulag project including a series of games about escape from Gulag, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.


using ClientAbstract;
using UnityEngine;
using Valve.VR;

namespace ClientWinVR
{
    public class WinVRInputManager : MonoBehaviour, IInputManager
    {
        public GameObject KontrolerGracza;

        private event ExecuteLogicalActionDelegate ZdarzenieLogicznejAkcji;

        public bool IsLogicalBooleanState(LogicalBooleanState state)
        {
           
           return false;
           
        }

        public void ListenInput(ExecuteLogicalActionDelegate sluchacz)
        {
            ZdarzenieLogicznejAkcji += sluchacz;
        }
        public void EndInputListening(ExecuteLogicalActionDelegate sluchacz)
        {
            ZdarzenieLogicznejAkcji -= sluchacz;
        }

        void Start()
        {
            
        }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
                ZdarzenieLogicznejAkcji?.Invoke(LogicalAction.ExpandInventory);
        }
    
    }

}
