using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PokerCats;
using SharpNeat.Phenomes;

public class PlayerTA : Player
{
    public PlayerTA(string playerName, int startingChips, PlayerType playerType, int index) : base(playerName, startingChips, playerType, index)
    {
    }

    public override void TomaDecisao()
    {
        //_currentGame.GameController.printLog("Player TA");
        double hs = forcaMao(_holeCards, _currentGame.Board.FullBoard);
        _currentGame.GameController.printLog("Player TA força da mão: " + hs);
        double pv = System.Math.Round(hs, 5, System.MidpointRounding.AwayFromZero);
        float alpha = 0.5f;
        float beta = 0.6f;

        PlayerState opJogadas = _currentGame.GetActivePlayerState();

        switch (opJogadas)
        {
            case PlayerState.CanFoldCallRaiseCheck:
                _currentGame.GameController.printLog("Fold Call Raise Check ");
                _decisao = foldCallRaiseCheck(pv,alpha,beta);
                break;
            case PlayerState.CanFoldCallRaise:
                // _currentGame.GameController.printLog("Fold Call Raise");

                _decisao = foldCallRaise(pv, alpha, beta);
                break;
            case PlayerState.CanFoldCheckRaise:
                // _currentGame.GameController.printLog("Fold Check Raise");

                _decisao = foldCheckRaise(pv, alpha, beta);
                break;
            case PlayerState.CanFoldCall:
                //_currentGame.GameController.printLog("Fold Call");

                _decisao = foldCall(pv, alpha);
                break;
            case PlayerState.CanFoldCheckBet:
                // _currentGame.GameController.printLog("Fold Check Bet");

                _decisao = foldCheckBet(pv, alpha, beta);
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

    public TurnType foldCheckRaise(double pv, float alpha, float beta)
    {

        if (pv >= alpha)
        {
            if (pv >= beta)
            {
                _currentGame.GameController.printLog(" deu raise " + _playerName);
                return TurnType.Raise;
            }

        }

        _currentGame.GameController.printLog(" deu check " + _playerName);

        return TurnType.Check;


    }

    public TurnType foldCall(double pv, float alpha)
    {
        if (pv >= alpha)
        {

            _currentGame.GameController.printLog(" deu call " + _playerName);

            return TurnType.Call;

        }

        return TurnType.Fold;
    }

    public TurnType foldCallRaise(double pv, float alpha, float beta)
    {
        if (pv >= alpha)
        {
            if (pv >= beta)
            {
                _currentGame.GameController.printLog(" deu raise " + _playerName);
                return TurnType.Raise;
            }
            _currentGame.GameController.printLog(" deu call " + _playerName);

            return TurnType.Call;
        }

        return TurnType.Fold;
    }

    public TurnType foldCheckBet(double pv, float alpha, float beta)
    {
        if (pv >= alpha)
        {
            if (pv >= beta)
            {
                _currentGame.GameController.printLog(" deu call " + _playerName);
                return TurnType.Call;
            }

            _currentGame.GameController.printLog(" deu raise " + _playerName);
            return TurnType.Check;
        }

        return TurnType.Fold;
    }

    
    public TurnType foldCallRaiseCheck(double pv, float alpha, float beta)
    {
        if (pv >= alpha )
        {
            if (pv >= beta)
            {
                _currentGame.GameController.printLog(" deu raise " + _playerName);

                return TurnType.Raise;
            }
            _currentGame.GameController.printLog(" deu call " + _playerName);
            return TurnType.Call;
        }


        else if ((_currentGame.CurrentHand.GetHighestBetNotInPot() - CurrentBet)  < 0  )
        {
            _currentGame.GameController.printLog(" deu Check " + _playerName);
            return TurnType.Check;

        }


        _currentGame.GameController.printLog(" deu fold " + _playerName);
        return TurnType.Fold;
    }
    




    public override float GetFitness()
    {
        return base.GetFitness();
    }

    protected override void HandleIsActiveChanged(bool newIsActive)
    {
        base.HandleIsActiveChanged(newIsActive);
    }

    protected override void UpdateBlackBoxInputs(ISignalArray inputSignalArray)
    {
        base.UpdateBlackBoxInputs(inputSignalArray);
    }

    protected override void UseBlackBoxOutpts(ISignalArray outputSignalArray)
    {
        base.UseBlackBoxOutpts(outputSignalArray);
    }




    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
