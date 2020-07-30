// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.

using ClientAbstract;
using UnityEngine;

namespace ClientAndroid
{
    public class AndroidInputManager : MonoBehaviour, IInputManager
    {
        private event ExecuteLogicalActionDelegate ZdarzenieLogicznejAkcji;
        private Vector2 _lastScreenInput;
        public Vector2 GetLastScreenInput()
        {
            return _lastScreenInput;
        }

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
            _lastScreenInput = Vector2.zero;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
                ZdarzenieLogicznejAkcji?.Invoke(LogicalAction.ExpandInventory);
            else if (Input.GetKeyDown(KeyCode.O))
                ZdarzenieLogicznejAkcji?.Invoke(LogicalAction.ZoomOut);
            else if (Input.GetAxis("Horizontal") < 0)
                ZdarzenieLogicznejAkcji?.Invoke(LogicalAction.MoveLeft);
            else if (Input.GetAxis("Horizontal") > 0)
                ZdarzenieLogicznejAkcji?.Invoke(LogicalAction.MoveRight);
            else if (Input.GetAxis("Vertical") > 0)
                ZdarzenieLogicznejAkcji?.Invoke(LogicalAction.MoveForward);
            else if (Input.GetAxis("Vertical") < 0)
                ZdarzenieLogicznejAkcji?.Invoke(LogicalAction.MoveBackward);
            else if (Input.GetAxis("Mouse ScrollWheel") > 0)
                ZdarzenieLogicznejAkcji?.Invoke(LogicalAction.ZoomIn);
            else if (Input.GetAxis("Mouse ScrollWheel") < 0)
                ZdarzenieLogicznejAkcji?.Invoke(LogicalAction.ZoomOut);


            if (Input.GetMouseButtonDown(0))
            {
                _lastScreenInput = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                ZdarzenieLogicznejAkcji?.Invoke(LogicalAction.Interaction);
                ZdarzenieLogicznejAkcji?.Invoke(LogicalAction.Any);

            }
            else if (Input.touchCount > 0)
            {
                _lastScreenInput = Input.GetTouch(0).position;
                ZdarzenieLogicznejAkcji?.Invoke(LogicalAction.Interaction);
                ZdarzenieLogicznejAkcji?.Invoke(LogicalAction.Any);
            }
            else _lastScreenInput = Vector2.zero;
        }
    }
}