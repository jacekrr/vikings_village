// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia projektu Gułag obejmującego serię gier o tematyce ucieczki z Gułagu, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of Gulag project including a series of games about escape from Gulag, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.

using ClientAbstract;
using UnityEngine;

namespace ClientWinPC
{
    public class WinPCInputManager : MonoBehaviour, IInputManager
    {
        private event ExecuteLogicalActionDelegate ZdarzenieLogicznejAkcji;
        private bool _mainActionState, _secondaryActionState;

        public void ListenInput(ExecuteLogicalActionDelegate sluchacz)
        {
            ZdarzenieLogicznejAkcji += sluchacz;
        }
        public void EndInputListening(ExecuteLogicalActionDelegate sluchacz)
        {
            ZdarzenieLogicznejAkcji -= sluchacz;
        }

        public bool IsLogicalBooleanState(LogicalBooleanState state)
        {
            switch(state)
            {
                case LogicalBooleanState.MainActionHold:
                    return _mainActionState;
                case LogicalBooleanState.SecondaryActionHold:
                    return _secondaryActionState;
                default:
                    return false;
            }
        }

        void Start()
        {
            _mainActionState = false;
            _secondaryActionState = false;
        }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
                ZdarzenieLogicznejAkcji?.Invoke(LogicalAction.ExpandInventory);
            if (Input.GetKeyDown(KeyCode.LeftBracket))
                ZdarzenieLogicznejAkcji?.Invoke(LogicalAction.InventoryLeft);
            if (Input.GetKeyDown(KeyCode.RightBracket))
                ZdarzenieLogicznejAkcji?.Invoke(LogicalAction.InventoryRight);
            if (Input.GetKeyDown(KeyCode.E))
                ZdarzenieLogicznejAkcji?.Invoke(LogicalAction.Interaction);
            if (Input.GetKeyDown(KeyCode.F))
                ZdarzenieLogicznejAkcji?.Invoke(LogicalAction.UseItem);
            if (Input.GetKeyDown(KeyCode.X))
                ZdarzenieLogicznejAkcji?.Invoke(LogicalAction.DropItem);
            if (Input.GetKeyDown(KeyCode.C))
                ZdarzenieLogicznejAkcji?.Invoke(LogicalAction.Crafting);
            if (Input.GetKeyDown(KeyCode.M))
                ZdarzenieLogicznejAkcji?.Invoke(LogicalAction.SwitchMode);
            if (Input.GetKeyDown(KeyCode.F1))
            {
                ZdarzenieLogicznejAkcji?.Invoke(LogicalAction.Help);
                ZdarzenieLogicznejAkcji?.Invoke(LogicalAction.ContinueGame);
            }
            if (Input.GetKeyDown(KeyCode.F2))
                ZdarzenieLogicznejAkcji?.Invoke(LogicalAction.Guide);
            if (Input.GetKeyDown(KeyCode.F5))
                ZdarzenieLogicznejAkcji?.Invoke(LogicalAction.NewGame);
            if (Input.GetKeyDown(KeyCode.F4))
            {
                ZdarzenieLogicznejAkcji?.Invoke(LogicalAction.ExitToMenu);
                ZdarzenieLogicznejAkcji?.Invoke(LogicalAction.ExitGame);
            }
            if (Input.GetKeyDown(KeyCode.Equals))
                ZdarzenieLogicznejAkcji?.Invoke(LogicalAction.TimeFaster);
            if (Input.GetKeyDown(KeyCode.Minus))
                ZdarzenieLogicznejAkcji?.Invoke(LogicalAction.TimeSlower);
            if (Input.anyKeyDown || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
                ZdarzenieLogicznejAkcji?.Invoke(LogicalAction.Any);
            if (Input.GetKeyDown(KeyCode.F6))
                ZdarzenieLogicznejAkcji?.Invoke(LogicalAction.ChangeDifficultyLevel);
            if (Input.GetKeyDown(KeyCode.F7))
                ZdarzenieLogicznejAkcji?.Invoke(LogicalAction.ChangeQualityLevel);
            if (Input.GetMouseButtonDown(0))
            {
                ZdarzenieLogicznejAkcji?.Invoke(LogicalAction.MainAction);
                _mainActionState = true;
            }
            if (Input.GetMouseButtonUp(0))
                _mainActionState = false;
            if (Input.GetMouseButtonDown(1))
            {
                ZdarzenieLogicznejAkcji?.Invoke(LogicalAction.SecondaryAction);
                _secondaryActionState = true;
            }
            if (Input.GetMouseButtonUp(1))
                _secondaryActionState = false;

            if (Input.mouseScrollDelta.y > 0)
                ZdarzenieLogicznejAkcji?.Invoke(LogicalAction.InventoryLeft);
            if (Input.mouseScrollDelta.y < 0)
                ZdarzenieLogicznejAkcji?.Invoke(LogicalAction.InventoryRight);            
        }

    }
}