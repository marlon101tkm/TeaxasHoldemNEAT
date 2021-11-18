using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace PokerCats
{
    public enum PlayerState {
        Invalid = -1,
        IsAI,
        CanFoldCallRaiseCheck,
        CanFoldCallRaise,
        CanFoldCheckBet,
        CanFoldCheckRaise,
        CanFoldCall,
        IsAllIn,
        Count
    };

    //public class ActivePlayerState : Singleton<ActivePlayerState>
    public class ActivePlayerState : MonoBehaviour
    {
        private PlayerState _currentState;
        public GameController gameController ;
        public GameController GameController
        {
            get { return gameController; }
            set { gameController = value; }
        }

        public void SetState(PlayerState state)
        {
            if (!IsStateValid(state)) {
                Debug.LogError("SetState: trying to set invalid state!");
                return;
            }

           // gameController.HideAllButtons();

            _currentState = state;
            
            //switch (state) {
            //    case PlayerState.CanFoldCallRaise:
            //        gameController.ShowFoldButton(true);
            //        gameController.ShowCallButton(true);
            //        gameController.ShowRaiseButton(true);
            //        gameController.ShowBetSliderAndBetAmountInputField(true);
            //        break;

            //    case PlayerState.CanFoldCheckBet:
            //        gameController.ShowFoldButton(true);
            //        gameController.ShowCheckButton(true);
            //        gameController.ShowBetButton(true);
            //        gameController.ShowBetSliderAndBetAmountInputField(true);
            //        break;

            //    case PlayerState.CanFoldCheckRaise:
            //        gameController.ShowFoldButton(true);
            //        gameController.ShowCheckButton(true);
            //        gameController.ShowRaiseButton(true);
            //        gameController.ShowBetSliderAndBetAmountInputField(true);
            //        break;

            //    case PlayerState.CanFoldCall:
            //        gameController.ShowFoldButton(true);
            //        gameController.ShowCallButton(true);
            //        break;

            //    case PlayerState.IsAI:
            //    case PlayerState.IsAllIn:
            //        // TODO: check if we need to do smthg here
            //        break;
            //}
        }

        public bool IsStateValid(PlayerState state)
        {
            if (PlayerState.Invalid < state && state < PlayerState.Count) {
                return true;
            }

            return false;
        }
	}
}
