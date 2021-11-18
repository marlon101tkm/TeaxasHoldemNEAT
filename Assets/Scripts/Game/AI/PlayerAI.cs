using UnityEngine;
using System;
using System.Collections.Generic;
using UnitySharpNEAT;
using SharpNeat.Phenomes;

namespace PokerCats
{
    // TODO: split AI based on difficulty levels (better split JSONs, not classes)
    //public class PlayerAI : Singleton<PlayerAI>
    public class PlayerAI : MonoBehaviour //: UnitController
    {
        private Game _currentGame;
        public Game CurrentGame
        {
            get { return _currentGame; }
            set { _currentGame = value; }
        }
        private Player _player ;

        public Player Player
        {
            get { return _player; }
            set { _player = value;  }
        }

        private Dictionary<Position, List<HoleCards>> _preflopOpenRaiseRanges = new Dictionary<Position, List<HoleCards>>();
        private Dictionary<Position, Dictionary<Position, List<HoleCards>>> _preflopColdCallRanges = new Dictionary<Position, Dictionary<Position, List<HoleCards>>>();
        private Dictionary<Position, Dictionary<Position, List<HoleCards>>> _preflop3BetRanges = new Dictionary<Position, Dictionary<Position, List<HoleCards>>>();
        private Dictionary<Position, Dictionary<Position, List<HoleCards>>> _preflop3BetCallingRanges = new Dictionary<Position, Dictionary<Position, List<HoleCards>>>();
        private Dictionary<Position, Dictionary<Position, List<HoleCards>>> _preflop4BetRanges = new Dictionary<Position, Dictionary<Position, List<HoleCards>>>();

        public PlayerAI()
        {
            for (Position position = Position.UTG1; position < Position.Count; position++)
            {
                _preflopOpenRaiseRanges.Add(position, new List<HoleCards>());

                _preflopColdCallRanges.Add(position, new Dictionary<Position, List<HoleCards>>());
                for (Position vsPosition = Position.UTG1; vsPosition < position; vsPosition++)
                {
                    _preflopColdCallRanges[position].Add(vsPosition, new List<HoleCards>());
                }

                _preflop3BetRanges.Add(position, new Dictionary<Position, List<HoleCards>>());
                for (Position vsPosition = Position.UTG1; vsPosition < position; vsPosition++)
                {
                    _preflop3BetRanges[position].Add(vsPosition, new List<HoleCards>());
                }

                _preflop3BetCallingRanges.Add(position, new Dictionary<Position, List<HoleCards>>());
                for (Position vsPosition = position + 1; vsPosition < Position.Count; vsPosition++)
                {
                    _preflop3BetCallingRanges[position].Add(vsPosition, new List<HoleCards>());
                }

                _preflop4BetRanges.Add(position, new Dictionary<Position, List<HoleCards>>());
                for (Position vsPosition = position + 1; vsPosition < Position.Count; vsPosition++)
                {
                    _preflop4BetRanges[position].Add(vsPosition, new List<HoleCards>());
                }
            }
        }

        public void AddHandToPreflopOpenRaiseRange(Position position, HoleCards hand)
        {
            _preflopOpenRaiseRanges[position].Add(hand);
        }

        public void AddHandToPreflopColdCallRange(Position position, Position vsPosition, HoleCards hand)
        {
            _preflopColdCallRanges[position][vsPosition].Add(hand);
        }

        public void AddHandToPreflop3BetRange(Position position, Position vsPosition, HoleCards hand)
        {
            _preflop3BetRanges[position][vsPosition].Add(hand);
        }

        public void AddHandToPreflop3BetCallingRange(Position position, Position vsPosition, HoleCards hand)
        {
            _preflop3BetCallingRanges[position][vsPosition].Add(hand);
        }

        public void AddHandToPreflop4BetRange(Position position, Position vsPosition, HoleCards hand)
        {
            _preflop4BetRanges[position][vsPosition].Add(hand);
        }

        //public TurnType MakeDecision(out int amount)
        //{
        //    amount = 0;

        //    //if (CurrentHandState.Instance.IsPreflop)
        //    if (CurrentHandState.Instance.IsPreflop)
        //    {
        //        return MakePreflopDecision(out amount);
        //    }
        //    else
        //    {
        //        // TODO: temporary checking through postflop and folding to any bet
        //        if (_currentGame.CurrentHand.GetHighestBetNotInPot() > 0)
        //        {
        //            return TurnType.Fold;
        //        }

        //        return TurnType.Check;
        //    }
        //}



        //função de teste antes de usar o NEAT
        public TurnType Decisao(out int amount)
        {
            amount = 0;
            int randon = RandomNumber.GetRandomNumber(100);
            Player currentPlayer = _currentGame.CurrentPlayer;
            int bigBlindSize = _currentGame.BigBlindSize;

            //if (_currentGame.GetPlayerAction())
            //{
            //    Debug.Log(" deu check " + currentPlayer.Name);
            //    return TurnType.Check;
            //}

            Debug.Log("nao pode dar check");
            if (randon <= 50)
            {
                Debug.Log(" deu call " + currentPlayer.Name);
                return TurnType.Call;
            }
            else if (randon >= 51 && randon <= 75)
            {
                Debug.Log(" deu raise " + currentPlayer.Name);
                amount = (int)(bigBlindSize);
                return TurnType.Raise;
            }


            Debug.Log(" deu fold " + currentPlayer.Name);
            return TurnType.Fold;


        }

        

        private TurnType MakePreflopDecision(out int amount)
        {
            amount = 0;

            Player currentPlayer = _currentGame.CurrentPlayer;
            int bigBlindSize = _currentGame.BigBlindSize;

            if (!HasPlayerPutMoneyInPot())
            {
                // open raising
                if (!WasPreflopRaiseMade())
                {
                    if (IsHandInPreflopOpenRaiseRange())
                    {
                        switch (currentPlayer.Position)
                        {
                            case Position.MP2:
                            case Position.MP3:
                            case Position.CO:
                            case Position.SB:
                                amount = (int)(bigBlindSize * 3.5);
                                break;
                            case Position.BU:
                                amount = (int)(bigBlindSize * 2.5);
                                break;
                        }

                        return TurnType.Raise;
                    }
                }
                else
                {
                    // preflop raise has been made by another player
                    // TODO: currently preflop callers/limpers are not taken into account at all
                    // TODO: preflop 3bet cold calling range should be completely different from open raise calling range!
                    Position vsPosition = _currentGame.CurrentHand.GetPlayerWithHighestBetPosition();
                    if (IsHandInPreflopColdCallRange(vsPosition))
                    {
                        return TurnType.Call;
                    }
                    else if (IsHandInPreflop3BetRange(vsPosition))
                    {
                        // TODO: add different 3bet sizings
                        amount = _currentGame.CurrentHand.GetHighestBetNotInPot() * 3;
                        return TurnType.Raise;
                    }
                }
            }
            else
            {
                // 3bet (or more) has been made
                // TODO: check more conditions here
                // TODO: calling 4bets and so on
                Position vsPosition = _currentGame.CurrentHand.GetPlayerWithHighestBetPosition();
                if (IsHandInPreflop3BetCallRange(vsPosition))
                {
                    return TurnType.Call;
                }
                else if (IsHandInPreflop4BetRange(vsPosition))
                {
                    amount = (int)(_currentGame.CurrentHand.GetHighestBetNotInPot() * 2.5);
                    return TurnType.Raise;
                }
                else
                {
                    return TurnType.Fold;
                }
            }

            return TurnType.Fold;
        }

        private void MakeFlopDecision()
        {

        }

        private void MakeTurnDecision()
        {

        }

        private void MakeRiverDecision()
        {

        }

        private bool IsHandInPreflopOpenRaiseRange()
        {
            Position currentPlayerPosition = _currentGame.CurrentPlayer.Position;
            HoleCards holeCards = _currentGame.CurrentPlayer.HoleCards;

            foreach (HoleCards hand in _preflopOpenRaiseRanges[currentPlayerPosition])
            {
                if (hand.First.Rank == holeCards.First.Rank)
                {
                    if (hand.Second.Rank == holeCards.Second.Rank)
                    {
                        if (hand.IsSuited == holeCards.IsSuited)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private bool IsHandInPreflopColdCallRange(Position vsPosition)
        {
            Position currentPlayerPosition = _currentGame.CurrentPlayer.Position;
            HoleCards holeCards = _currentGame.CurrentPlayer.HoleCards;

            foreach (HoleCards hand in _preflopColdCallRanges[currentPlayerPosition][vsPosition])
            {
                if (hand.First.Rank == holeCards.First.Rank)
                {
                    if (hand.Second.Rank == holeCards.Second.Rank)
                    {
                        if (hand.IsSuited == holeCards.IsSuited)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private bool IsHandInPreflop3BetRange(Position vsPosition)
        {
            Position currentPlayerPosition = _currentGame.CurrentPlayer.Position;
            HoleCards holeCards = _currentGame.CurrentPlayer.HoleCards;

            foreach (HoleCards hand in _preflop3BetRanges[currentPlayerPosition][vsPosition])
            {
                if (hand.First.Rank == holeCards.First.Rank)
                {
                    if (hand.Second.Rank == holeCards.Second.Rank)
                    {
                        if (hand.IsSuited == holeCards.IsSuited)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private bool IsHandInPreflop3BetCallRange(Position vsPosition)
        {
            Position currentPlayerPosition = _currentGame.CurrentPlayer.Position;
            HoleCards holeCards = _currentGame.CurrentPlayer.HoleCards;

            foreach (HoleCards hand in _preflop3BetCallingRanges[currentPlayerPosition][vsPosition])
            {
                if (hand.First.Rank == holeCards.First.Rank)
                {
                    if (hand.Second.Rank == holeCards.Second.Rank)
                    {
                        if (hand.IsSuited == holeCards.IsSuited)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private bool IsHandInPreflop4BetRange(Position vsPosition)
        {
            Position currentPlayerPosition = _currentGame.CurrentPlayer.Position;
            HoleCards holeCards = _currentGame.CurrentPlayer.HoleCards;

            foreach (HoleCards hand in _preflop4BetRanges[currentPlayerPosition][vsPosition])
            {
                if (hand.First.Rank == holeCards.First.Rank)
                {
                    if (hand.Second.Rank == holeCards.Second.Rank)
                    {
                        if (hand.IsSuited == holeCards.IsSuited)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private bool HasPlayerPutMoneyInPot()
        {
            return (_currentGame.CurrentPlayer.CurrentBet > _currentGame.BigBlindSize);
        }

        private bool WasPreflopRaiseMade()
        {
           // if (!CurrentHandState.Instance.IsPreflop)
           if(CurrentGame.GameController.CurrentHandState.IsPreflop)
            {
                return true;
            }

            return (_currentGame.CurrentHand.GetHighestBetNotInPot() > _currentGame.BigBlindSize);
        }

        // An open raise and a 3bet were made before our turn
        private bool WerePreflopRaiseAnd3BetMade()
        {
            return false;
        }



        //calculo da força basica da mão
        private double forcaMao(HoleCards holeCards,List<Card> boardCards )
        {
            double vence = 0, perde = 0, empate = 0;
            double forca = 0;
            double rankjogador = 0, rankOponente = 0;
            HandChecker checker  = new HandChecker();


            Deck cartas = new Deck();
            List<HoleCards> holeCardsOpentes = cartas.generateDeckCombination(holeCards); ;
            rankjogador = checker.RankMao(holeCards,boardCards);

            foreach (HoleCards holeCardsOp in holeCardsOpentes)
            {
                rankOponente = checker.RankMao(holeCardsOp, boardCards);

                if (rankjogador > rankOponente)
                {
                    vence++;
                }
                else if(rankjogador == rankOponente)
                {
                    empate++;
                }
                else
                {
                    perde++;
                }
            }


            forca = (vence + empate / 2) + (vence + empate + perde);

            Debug.Log("Forca da mão:" + forca);
            return forca;
        }



        // potencial da mão durante toda as etapas da rodada
        public double PotencialMao(HoleCards holeCards, List<Card> boardCards)
        {
            // No Array do Potencial da mão, cada índice representa se vence, se empata ou se perde
            double[,] PM = new double[3, 3]; // inicializado em 0
            double[] PMTotal = new double[3]; // inicializado em 0   
            int vence = 0, perde = 0, empate = 0;
            //double forca = 0;
            double rankJogador = 0, rankOponente = 0, melhorJogador = 0, melhorOponente=0;
            double Ppot = 0;
            int index = 0;

            HandChecker checker = new HandChecker();

            List<Card> mesa = new List<Card>();
             

            rankJogador = checker.RankMao(holeCards, boardCards);

            Deck cartas = new Deck();
            List<HoleCards> holeCardsOponentes = cartas.generateDeckCombination(holeCards);
            List<HoleCards> turnERiver = new List<HoleCards>();

            //Considera todas as combinações de duas cartas, para as cartas restantes para o oponente*/
            foreach (HoleCards holeCardsOp in holeCardsOponentes)
            {
                rankOponente = checker.RankMao(holeCardsOp, boardCards);
                if (rankJogador > rankOponente)
                {
                    index = vence;
                }
                else if (rankJogador == rankOponente)
                {
                    index = empate;
                }
                else
                {
                    index = perde;
                }
                PMTotal[index] += 1;


                if (boardCards.Count == 3) {
                    turnERiver = cartas.generateDeckCombination(boardCards);

                    // Todas as possíveis cartas que podem vir a mesa */
                    foreach (HoleCards turnRiver in turnERiver) {
                        // Final 5 cartas na mesa


                        mesa.AddRange(boardCards);
                        mesa.Add(turnRiver.First);
                        mesa.Add(turnRiver.Second);



                        melhorJogador = checker.RankMao(holeCards, mesa);
                        melhorOponente = checker.RankMao(holeCardsOp, mesa);

                        if (melhorJogador > melhorOponente)
                        {
                            PM[index, vence] += 1;
                        }
                        else if (melhorJogador == melhorOponente)
                        {
                            PM[index, empate] += 1;
                        }
                        else
                        {
                            PM[index, perde] += 1;
                        }

                        mesa.Clear();
                    }

                }
            }
            //Ppot: onde uma mão perdedora se torna uma mão vencedora
             Ppot = (PM[perde, vence] + PM[perde, empate] / 2 + PM[empate, vence] / 2) / (PMTotal[perde] + PMTotal[empate]);
            //NPot: onde uma mão vencedora se torna uma mão perdedora
            //Double Npot = (PM[vence, perde] + PM[empate, perde] / 2 + PM[vence, empate] / 2) / (PMTotal[vence] + PMTotal[empate]);

            return Ppot;
        }


        //protected override void UpdateBlackBoxInputs(ISignalArray inputSignalArray)
        //{

        //    double hs = forcaMao(Player.HoleCards,CurrentGame.Board.Flop);

        //    Debug.Log("Força da mão" + hs);

        //    double ppot = PotencialMao(Player.HoleCards, CurrentGame.Board.Flop);

        //    //Força efetiva da mão
        //    inputSignalArray[0] = hs + (1 - hs) * ppot;

        //    //Chance de lucro;
        //    inputSignalArray[1] = (Double)CurrentGame.BigBlindSize / ((Double)(CurrentGame.CurrentHand.MainPot.Size) + (Double)CurrentGame.BigBlindSize);

        //    //Rodada atual de aposta
        //    inputSignalArray[2] =  (Double) CurrentHandState.Instance.TurnoAtual;

        //    //Quantia total de dinheiro na mesa
        //    inputSignalArray[3] = (Double)(CurrentGame.CurrentHand.MainPot.Size);

        //    //Quantia de dinheiro do jogador
        //    inputSignalArray[4] = Player.ChipCount;

        //    //Quantia necessária para Call
        //    inputSignalArray[5] = (Double)CurrentGame.BigBlindSize;


        //    //Posição do Jogador na mesa
        //    inputSignalArray[6] = (Double)Player.Position;

        //    //Ultimas ações dos jogadores

        //    foreach(Player op in CurrentGame.Players)
        //    {
        //        inputSignalArray[op.Index + 7] = (Double)op.LastTurn;
        //    }

        //    /*
        //    inputSignalArray[7] =
        //    inputSignalArray[8] =
        //    inputSignalArray[9] =
        //    inputSignalArray[10] =
        //    inputSignalArray[11] = 
        //    inputSignalArray[12] = 
        //    */


        //}

        //protected override void UseBlackBoxOutpts(ISignalArray outputSignalArray)
        //{

        //    Double /*sinalFold, sinalCall, sinalRaise, sinalCheck,*/saida;

        //    int indexOp = 0;
        //    /*
        //         sinalFold = outputSignalArray[0];
        //         sinalCall = outputSignalArray[1];
        //         sinalRaise = outputSignalArray[2];
        //         sinalCheck =  outputSignalArray[3];
        //         */
        //    saida = outputSignalArray[0];
        //    for (int i= 1; i < 3;i++)
        //    {
        //        if (saida < outputSignalArray[i])
        //        {
        //            saida = outputSignalArray[i];
                   
        //            indexOp = i;
        //        }
        //    }

           
        //    switch (indexOp)
        //    {
        //        case 0:
        //           Player.Decisao = TurnType.Check;
        //            break;
        //        case 1:
        //            Player.Decisao = TurnType.Fold;
                    
        //            break;
        //        case 2:
        //            Player.Decisao = TurnType.Call;
                    
        //            break;
        //        case 3:
        //            Player.Decisao = TurnType.Raise;
        //            break;
                
        //    }


        //}

        //public override float GetFitness()
        //{
        //    if (gameController.HandCounter < 0)
        //    {
        //        return 0;
        //    }

        //    if(Player == null)
        //    {
        //        Debug.Log("jogador ta null");
        //    }

        //    float player = _player.Wins;
            
        //    float fit = Player.Wins / gameController.HandCounter;
        //    if (fit > 0)
        //    {
        //        return fit;
        //    }
        //    return 0;

            
        //}

        //protected override void HandleIsActiveChanged(bool newIsActive)
        //{

        //    if (newIsActive == false)
        //    {
        //        // the unit has been deactivated, IsActive was switched to false

        //        // reset transform
        //        Player.Wins = 0;
        //        Player.Decisao = TurnType.NotMade;

        //    }

        //    // hide/show children 
        //    // the children happen to be the car meshes => we hide this Unit when IsActive turns false and show it when it turns true
        //    foreach (Transform t in transform)
        //    {
        //        t.gameObject.SetActive(newIsActive);
        //    }
        //}
    }
}
