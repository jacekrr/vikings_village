// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.


using UnityEngine;

namespace ClientAbstract
{

    public enum LogicalAction
    {
        MoveLeft, MoveRight, MoveForward,MoveBackward, Jump, PickupItem, Interaction, Crafting, ExpandInventory, InventoryRight, InventoryLeft, DropItem, TimeSlower, TimeFaster, TimeClick, ExitToMenu, Help, Guide, NewGame, ContinueGame, ExitGame, ChangeDifficultyLevel,ChangeQualityLevel, MainAction, SecondaryAction, UseItem, SwitchMode, Any
    }
    public enum LogicalBooleanState
    {
        None, MainActionHold, SecondaryActionHold
    }

    public delegate void ExecuteLogicalActionDelegate(LogicalAction action);

    public interface IInputManager 
    {
        void ListenInput(ExecuteLogicalActionDelegate listener);
        void EndInputListening(ExecuteLogicalActionDelegate listener);
        bool IsLogicalBooleanState(LogicalBooleanState state);
        Vector2 GetLastScreenInput();
    }
}