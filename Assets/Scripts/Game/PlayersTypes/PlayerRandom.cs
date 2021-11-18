using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PokerCats;
using SharpNeat.Phenomes;

public class PlayerRandom : Player
{
    public PlayerRandom(string playerName, int startingChips, PlayerType playerType, int index) : base(playerName, startingChips, playerType, index)
    {

    }

    public override void TomaDecisao()
    {
        _currentGame.GameController.printLog("Player Random");
        int randon = RandomNumber.GetRandomNumber(100);

       // int bigBlindSize = _currentGame.GameController.CurrentGame.BigBlindSize;

        //int smallBlindSite = bigBlindSize / 2;
        ////if (_currentGame.GetPlayerAction())
        ////{
        ////    _currentGame.GameController.printLog(" deu check " + currentPlayer.Name);
        ////    return TurnType.Check;
        ////}

        //_currentGame.GameController.printLog("nao pode dar check");
        //if (randon <= 50)
        //{
        //    _currentGame.GameController.printLog(" deu call " + _playerName);
        //    return TurnType.Call;
        //}
        //else if (randon >= 51 && randon <= 75)
        //{
        //    _currentGame.GameController.printLog(" deu raise " + _playerName);
        //    amount = (int)(bigBlindSize);
        //    return TurnType.Raise;
        //}


        //_currentGame.GameController.printLog(" deu fold " + _playerName);
        //return TurnType.Fold;




        PlayerState opJogadas = _currentGame.GetActivePlayerState();

        switch (opJogadas)
        {
            
            case PlayerState.CanFoldCallRaiseCheck:
                // _currentGame.GameController.printLog("Fold Call Raise Check ");
                _decisao = foldCallRaiseCheck(randon);
                break;
                
            case PlayerState.CanFoldCallRaise:
                // _currentGame.GameController.printLog("Fold Call Raise");
                
                _decisao = foldCallRaise(randon);
                break;
            case PlayerState.CanFoldCheckRaise:
                // _currentGame.GameController.printLog("Fold Check Raise");
                
                _decisao = foldCheckRaise(randon);
                break;
            case PlayerState.CanFoldCall:
                //_currentGame.GameController.printLog("Fold Call");
                
                _decisao = foldCall(randon);
                break;
            case PlayerState.CanFoldCheckBet:
                // _currentGame.GameController.printLog("Fold Check Bet");
                
                _decisao = foldCheckBet(randon);
                break;
            //case PlayerState.IsAllIn:
            //   // _currentGame.GameController.printLog("All In");
            //    _decisao = TurnType.Check;
            //break;
            default:
                _decisao = TurnType.NotMade;

                break;
        }


    }
    
    public TurnType foldCheckRaise(int randon)
    {
        
        if (randon <= 33)
        {
            _currentGame.GameController.printLog(" deu check " + _playerName);

            return TurnType.Check;
        }
        else if ( randon <= 66)
        {
            _currentGame.GameController.printLog(" deu raise " + _playerName);
            
            return TurnType.Raise;
        }

        return TurnType.Fold;
    }

    public TurnType foldCall(int randon)
    {
        if (randon <= 50)
        {
            _currentGame.GameController.printLog(" deu call " + _playerName);

            return TurnType.Call;
        }

        return TurnType.Fold;
    }

    public TurnType foldCallRaise(int randon)
    {
        if (randon <= 33)
        {
            _currentGame.GameController.printLog(" deu call " + _playerName);

            return TurnType.Call;
        }
        else if ( randon <= 66)
        {
            _currentGame.GameController.printLog(" deu raise " + _playerName);
            return TurnType.Raise;
        }

        return TurnType.Fold;
    }

    public TurnType foldCheckBet(int randon)
    {
        if (randon <= 33)
        {
            _currentGame.GameController.printLog(" deu call " + _playerName);

            return TurnType.Call;
        }
        else if (randon <= 66)
        {
            _currentGame.GameController.printLog(" deu raise " + _playerName);
            return TurnType.Check;
        }

        return TurnType.Fold;
    }
        
    public TurnType foldCallRaiseCheck(int randon)
    {
        if (randon <= 25)
        {
            _currentGame.GameController.printLog(" deu call " + _playerName);
            return TurnType.Call;
        }
        else if ( randon <= 50)
        {
            _currentGame.GameController.printLog(" deu raise " + _playerName);
            
            return TurnType.Raise;
        }else if (randon <= 75)
        {
            _currentGame.GameController.printLog(" deu Check " + _playerName);
            return TurnType.Check;

        }


        _currentGame.GameController.printLog(" deu fold " + _playerName);
        return TurnType.Fold;
    }
    
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    protected override void UpdateBlackBoxInputs(ISignalArray inputSignalArray)
    {
        base.UpdateBlackBoxInputs(inputSignalArray);
    }

    protected override void UseBlackBoxOutpts(ISignalArray outputSignalArray)
    {
        base.UseBlackBoxOutpts(outputSignalArray);
    }

    public override float GetFitness()
    {
        return base.GetFitness();
    }

    protected override void HandleIsActiveChanged(bool newIsActive)
    {
        base.HandleIsActiveChanged(newIsActive);
    }
}
