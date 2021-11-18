using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;

using UnityEngine;

using UnitySharpNEAT;
using SharpNeat.Phenomes;
using System.Collections;

namespace PokerCats
{
    public enum PlayerType
    {
        Invalid = -1,
        Local,
        Remote,
        AI,
        Count
    };


    public enum PlayerClass
    {
        Invalid=-1,
        Neat,
        LA,
        LP,
        TA,
        TP,
        Random,
        Count
    };
    public enum Position
    {
        Invalid = -1,
        UTG1,
        UTG2,
        MP1,
        MP2,
        MP3,
        CO,
        BU,
        SB,
        BB,
        Count
    };

    public struct HoleCards
    {
        public Card First;
        public Card Second;

        public bool IsPocketPair
        {
            get
            {
                if (First == null || Second == null)
                {
                    Debug.LogError("HoleCards.IsPocketPair: cards are null!");
                    return false;
                }

                return (First.Rank == Second.Rank);
            }
        }

        public bool IsSuited
        {
            get
            {
                if (First == null || Second == null)
                {
                    Debug.LogError("HoleCards.IsSuited: cards are null!");
                    return false;
                }

                return (First.Colour == Second.Colour);
            }
        }

        public String HoleCardsToString()
        {
            return "First: " + First.GetTextInfo() + " Second: " + Second.GetTextInfo();
        }
    };

    public class Player : UnitController
    {
        protected HoleCards _holeCards = new HoleCards();

        public Game _currentGame;

        protected string _playerName;

        private int _initchipCount;

        private int _chipCount;

        private int _currentBet;

        private Position _position;

        private PlayerClass _pClass;
        public PlayerClass PClass
        {
            get { return _pClass; }
            set { _pClass = value;}
        }

        private int _index;

        private double _ppot;

        public double PPOT
        {
            get { return _ppot; }
        }

        protected TurnType _decisao = TurnType.NotMade;

        public TurnType Decisao
        {
            get { return _decisao; }
            set { _decisao = value; }
        }

        private TurnType _lastTurn = TurnType.NotMade;

        public TurnType LastTurn
        {
            get { return _lastTurn; }
            set { _lastTurn = value; }
        }

        private PlayerType _playerType = PlayerType.Invalid;

        //private PlayerAI _ai;
        //public PlayerAI AI
        //{
        //    get { return _ai; }
        //    set { _ai = value; }
        //}

        public bool _handPotencialExecutando;

        public bool _tomouDecisao = false;
        public bool TomouDecisao
        {
            get { return _tomouDecisao; }
            set { _tomouDecisao = value; }
        }

        private PlayerHandInfo _currentHandInfo;
        public PlayerHandInfo CurrentHandInfo
        {
            get { return _currentHandInfo; }
            set { _currentHandInfo = value; }
        }

        private int _handParticipated = 0;
        public int HandParticipated
        {
            get { return _handParticipated; }
            set { _handParticipated = value; }
        }

        private float _roi = 0;
        public float ROI
        {
            get { return _roi; }
            set { _roi = value; }
        }

        public Player(string playerName, int startingChips, PlayerType playerType, int index)
        {
            _playerName = playerName;
            _chipCount = startingChips;

            _playerType = playerType;

            _initchipCount = startingChips;
            //_ai = ai;

            _index = index;
            //_currentGame.GameController.printLog("Player Name:" + playerName);
        }

        public PlayerType Type
        {
            get { return _playerType; }
            set { _playerType = value; }
        }

        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }

        public string Name
        {
            get { return _playerName; }
            set { _playerName = value; }
        }
        public int InitChipCount
        {
            get { return _initchipCount; }
            set { _initchipCount = value ; }
        }

        public int ChipCount
        {
            get { return _chipCount; }
            set
            {
                _chipCount = value;
                if (_chipCount < 0)
                {
                    _chipCount = 0;
                    Debug.LogError("Jogador" + Name + "ChipCount set: attempt to set negative amount.");
                }
            }
        }


        public int CurrentBet
        {
            get { return _currentBet; }
            set { _currentBet = value; }
        }

        public HoleCards HoleCards
        {
            get { return _holeCards; }
        }

        public Position Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public bool HasButton
        {
            get { return Position == Position.BU; }
        }

        public bool IsOnSB
        {
            get { return Position == Position.SB; }
        }

        public bool IsOnBB
        {
            get { return Position == Position.BB; }
        }

        public bool IsLocal
        {
            get { return _playerType == PlayerType.Local; }
        }

        public bool IsAI
        {
            get { return _playerType == PlayerType.AI; }
        }

        public bool IsAllIn
        {
            get { return _chipCount == 0; }
        }

        public bool HasMadeTurn
        {
            get { return LastTurn != TurnType.NotMade; }
        }

        public bool HasFolded
        {
            get { return LastTurn == TurnType.Fold; }
        }

        public bool HasChecked
        {
            get { return LastTurn == TurnType.Check; }
        }

        public bool HasBetOrRaise
        {
            get { return LastTurn == TurnType.Bet || LastTurn == TurnType.Raise || LastTurn == TurnType.Call; }
        }

        public override void FixedUpdate()
        {

        }

        public void AddHoleCard(Card cardToAdd)
        {
            if (cardToAdd == null)
            {
                Debug.LogError("AddHoleCard: card to add is null!");
                return;
            }

            if (_holeCards.First == null)
            {
                _holeCards.First = cardToAdd;
            }
            else if (_holeCards.Second == null)
            {
                if (_holeCards.First.Rank == cardToAdd.Rank && _holeCards.First.Colour == cardToAdd.Colour)
                {
                    Debug.LogError("AddHoleCard: attempt to add the same hole card!");
                    return;
                }

                _holeCards.Second = cardToAdd;

                // sort cards in hand by rank
                if (_holeCards.Second.Rank > _holeCards.First.Rank)
                {
                    Card card = _holeCards.First;
                    _holeCards.First = _holeCards.Second;
                    _holeCards.Second = card;
                }
            }
            else
            {
                Debug.LogError( _currentGame.GameController.name + " AddHoleCard: attempt to add redundant hole card!");
            }
        }

        public void ClearHoleCards()
        {
            _holeCards.First = null;
            _holeCards.Second = null;
        }

        public void PostBlind(int blindSize)
        {
            if (!Utils.IsOnBlinds(Position))
            {
                Debug.LogError("PostBlind: attempt to post blind for player with wrong position!");
                return;
            }

            CurrentBet += blindSize;
            int newStackSize = ChipCount - blindSize;
            ChipCount = newStackSize;
        }

        public void PostAnte(int anteSize)
        {
            // TODO: check if we should add ante to _currentBet
            int newStackSize = ChipCount - anteSize;
            ChipCount = newStackSize;
        }

        public void PutChipsToPot(Pot pot, int amount)
        {
            int newStackSize = ChipCount - amount;
            ChipCount = newStackSize;

            CurrentBet += amount;
            if (!pot.IsPlayerInPot(this))
            {
                pot.PlayersInPot.Add(this);
            }
            pot.Size += amount;
        }

        public void AddChips(int amount)
        {
            int newStackSize = ChipCount + amount;
            ChipCount = newStackSize;
        }

        public float CalculoRoi()
        {
            
            return ((float)ChipCount - (float)_initchipCount) / (float)_initchipCount;
        }

        //calculo da força basica da mão
        protected double forcaMao(HoleCards holeCards, List<Card> boardCards)
        {
            double vence = 0, perde = 0, empate = 0;
            double forca = 0;
            double rankjogador = 0, rankOponente = 0;
            //HandChecker checker = new HandChecker();

            Deck cartas = new Deck();
            //List<HoleCards> holeCardsOpentes = cartas.generateDeckCombination(holeCards);
            List<HoleCards> holeCardsOpentes;
            //if (CurrentHandState.Instance.IsPreflop)
            if(_currentGame.GameController.CurrentHandState.IsPreflop)
            {
                holeCardsOpentes = cartas.generateDeckCombination(holeCards);
            }
            else
            {
                holeCardsOpentes = cartas.generateDeckCombination(boardCards,holeCards);
            }
            rankjogador = _currentGame.GameController.HandChecker.RankMao(holeCards, boardCards);
            // _currentGame.GameController.printLog("------------------------------------------------Inicia Força da Mão------------------------------------------------------");
            //_currentGame.GameController.printLog("Rank Jogador: "+ rankjogador );
            foreach (HoleCards holeCardsOp in holeCardsOpentes)
            {
                rankOponente = _currentGame.GameController.HandChecker.RankMao(holeCardsOp, boardCards);
                // _currentGame.GameController.printLog("Rank Oponente: " + rankOponente);
                if (rankjogador > rankOponente)
                {
                    vence++;
                }
                else if (rankjogador == rankOponente)
                {
                    empate++;
                }
                else
                {
                    perde++;
                }
            }

            //_currentGame.GameController.printLog("Cartas da Mão: "+holeCards.First.GetTextInfo() +" "+ holeCards.Second.GetTextInfo() + " Vence: "+vence+" Empate: "+empate+" Perde: "+perde);
            forca = (vence + empate / 2) / (vence + empate + perde);

            
            //_currentGame.GameController.printLog("Forca da mão:" + forca);
            return forca;
        }

        public void resetInstance()
        {
            ClearHoleCards();
           // Index = -1;
            //Name = "";
            ChipCount = _currentGame.StartingChips;
            CurrentBet = 0;
            Decisao = TurnType.NotMade;
            LastTurn = TurnType.NotMade;
           // Position = Position.Invalid;
            ROI = 0;
            _handParticipated = 0;
        }

        // potencial da mão durante toda as etapas da rodada
        
        //public IEnumerator PotencialMao()
        public double PotencialMao(HoleCards holeCards, List<Card> boardCards)
        {
            
            _currentGame.GameController.printLog("Iniciar potencial Mao");
            this._ppot = 0;
            _handPotencialExecutando = true;
       

            // No Array do Potencial da mão, cada índice representa se vence, se empata ou se perde
            double[,] PM = new double[3, 3]; // inicializado em 0
            double[] PMTotal = new double[3]; // inicializado em 0  
 
            int vence = 0, perde = 1, empate = 2;
            for (int i = 0; i < 3; i++)
            {
                PMTotal[i] = 0;
                PM[i,vence] = 0;
                PM[i, perde] = 0;
                PM[i, empate] = 0;

            }
            //double forca = 0;
            double rankJogador = 0, rankOponente = 0, melhorJogador = 0, melhorOponente = 0;
            double Ppot = 0;
            int index = -1;
            List<Card> mesa = new List<Card>();
            rankJogador = _currentGame.GameController.HandChecker.RankMao(holeCards, boardCards);
            Deck cartas = new Deck();
          
            List<HoleCards> holeCardsOponentes = cartas.generateDeckCombination(boardCards, holeCards);
            List<HoleCards> turnERiver;// = cartas.generateDeckCombination(boardCards, holeCards);
            
            //Considera todas as combinações de duas cartas, para as cartas restantes para o oponente*/
            foreach (HoleCards holeCardsOp in holeCardsOponentes.ToList())
            {
               
                rankOponente = _currentGame.GameController.HandChecker.RankMao(holeCardsOp, boardCards);

               
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

               

                if (_currentGame.GameController.CurrentHandState.IsFlop)
                {
              
                    turnERiver = cartas.generateDeckCombination(boardCards,holeCardsOp);
                                                        
                    foreach (HoleCards turnRiver in turnERiver)
                    {
                        // Final 5 cartas na mesa
                        mesa.AddRange(boardCards);
                        mesa.Add(turnRiver.First);
                        mesa.Add(turnRiver.Second);

                        //_currentGame.GameController.printLog("QTD na mesa:" + mesa.Count);
                        melhorJogador = _currentGame.GameController.HandChecker.RankMao(holeCards, mesa);
                        melhorOponente = _currentGame.GameController.HandChecker.RankMao(holeCardsOp, mesa);
                        //  _currentGame.GameController.printLog("Rank Jogador: " + melhorJogador + "RankOponente: " + melhorOponente);
                        if (melhorJogador > melhorOponente)
                        {
                            PM[index, vence] += 1;
                            // _currentGame.GameController.printLog("Index: " + index + " Vence: " + PM[index, vence]);
                        }
                        else if (melhorJogador == melhorOponente)
                        {
                            PM[index, empate] += 1;
                            // _currentGame.GameController.printLog("Index: " + index + " Empata:  " + PM[index, empate]);
                        }
                        else
                        {
                            PM[index, perde] += 1;
                            //_currentGame.GameController.printLog("Index: " + index + " Perde  " + PM[index, perde]);
                        }
                        mesa.Clear();
                       
                    }
                }else if (_currentGame.GameController.CurrentHandState.IsTurn)
                {
                    
                    List<Card> River = cartas.genereteDeckWithoutBoardAndHoleCards(boardCards, holeCards); 
                                                        
                    foreach (Card river in River)
                    {
                        // Final 5 cartas na mesa
                        mesa.AddRange(boardCards);
                        mesa.Add(river);
                        //mesa.Add(turnRiver.Second);

                        //_currentGame.GameController.printLog("QTD na mesa:" + mesa.Count);
                        melhorJogador = _currentGame.GameController.HandChecker.RankMao(holeCards, mesa);
                        melhorOponente = _currentGame.GameController.HandChecker.RankMao(holeCardsOp, mesa);
                        //  _currentGame.GameController.printLog("Rank Jogador: " + melhorJogador + "RankOponente: " + melhorOponente);
                        if (melhorJogador > melhorOponente)
                        {
                            PM[index, vence] += 1;
                            // _currentGame.GameController.printLog("Index: " + index + " Vence: " + PM[index, vence]);
                        }
                        else if (melhorJogador == melhorOponente)
                        {
                            PM[index, empate] += 1;
                            // _currentGame.GameController.printLog("Index: " + index + " Empata:  " + PM[index, empate]);
                        }
                        else
                        {
                            PM[index, perde] += 1;
                            //_currentGame.GameController.printLog("Index: " + index + " Perde  " + PM[index, perde]);
                        }
                        mesa.Clear();
                       
                    }

                }

            }


            _currentGame.GameController.printLog("("+ PM[perde, vence]+ " + "+ PM[perde, empate]+ "/ 2 "+" + "+ PM[empate, vence]+ "/ 2) /  ("+ PMTotal[perde] +" + "+ PMTotal[empate] + ")" );
            //Ppot: onde uma mão perdedora se torna uma mão vencedora
            Ppot = (PM[perde, vence] + PM[perde, empate] / 2 + PM[empate, vence] / 2) / (PMTotal[perde] + PMTotal[empate]);
            //NPot: onde uma mão vencedora se torna uma mão perdedora
            //Double Npot = (PM[vence, perde] + PM[empate, perde] / 2 + PM[vence, empate] / 2) / (PMTotal[vence] + PMTotal[empate]);

         
            return PPOT;
       
        }
        
        public virtual void TomaDecisao()
        {
          
            if (IsActive)
            {
                // feed the black box with input
                UpdateBlackBoxInputs(BlackBox.InputSignalArray);

                // calculate the outputs
                BlackBox.Activate();

                // do something with those outputs
                UseBlackBoxOutpts(BlackBox.OutputSignalArray);
            }
            
        }
              
        private IEnumerator WaitPotencialmao()
        {
            yield return new WaitWhile(()=>_handPotencialExecutando);
            
        }

        protected override void UpdateBlackBoxInputs(ISignalArray inputSignalArray)
        {
            //_currentGame.GameController.printLog("Jogador: " + this._playerName);

            //HoleCards cardsMao = _holeCards;
            //List<Card>  flop = _currentGame.GameController.CurrentGame.Board.Flop;
            //double hs = forcaMao(_holeCards, _currentGame.Board.Flop);
            double hs = forcaMao(_holeCards, _currentGame.Board.FullBoard);
            double chanceDeLucro = (Double)_currentGame.GameController.CurrentAmoutToBet / ((Double)(_currentGame.CurrentHand.MainPot.Size) + (Double)_currentGame.GameController.CurrentAmoutToBet);
           // _currentGame.GameController.printLog("Player NEAT forca da mão: "+ hs );
           



            //_currentGame.GameController.printLog(_currentGame.GameController.name  +" Jogador: " + Name + " Mão: " + _holeCards.HoleCardsToString() + "     Força da mão: " + hs); //+ " Turno  " + _currentGame.GameController.CurrentHandState.QualTurno());

            //Força efetiva da mão
           
            inputSignalArray[0] = hs; // System.Math.Round(hs, 5, System.MidpointRounding.AwayFromZero);
           
            
           // double ppot = PotencialMao(_holeCards, _currentGame.GameController.CurrentGame.Board.FullBoard);
           

            //_currentGame.GameController.printLog("Jogador: " + Name + " potencial mão: " + ((1 - hs) * _ppot));
             //inputSignalArray[0] = hs + (1 - hs) * _ppot;
            

            //_currentGame.GameController.printLog( _currentGame.GameController.CurrentAmoutToBet + "/" + _currentGame.CurrentHand.MainPot.Size + "+" + _currentGame.GameController.CurrentAmoutToBet);
            //_currentGame.GameController.printLog(_currentGame.GameController.name + " jogador: "+ this.Name + " QTD Chips: "+ _chipCount );

            //Chance de lucro;
            // inputSignalArray[1] = (Double)_currentGame.BigBlindSize / ((Double)(_currentGame.CurrentHand.MainPot.Size) + (Double)_currentGame.BigBlindSize);
            inputSignalArray[1] = chanceDeLucro; // System.Math.Round(chanceDeLucro, 5, System.MidpointRounding.AwayFromZero);

            //Rodada atual de aposta
            inputSignalArray[2] = (Double) _currentGame.GameController.CurrentHandState.TurnoAtual ;

            //Quantia total de dinheiro na mesa
            inputSignalArray[3] = (Double)(_currentGame.CurrentHand.MainPot.Size);

            //Quantia de dinheiro do jogador
            inputSignalArray[4] = _chipCount;

            //Quantia necessária para Call
            //inputSignalArray[5] = (Double)_currentGame.BigBlindSize;
            inputSignalArray[5] = (Double)_currentGame.GameController.CurrentAmoutToBet;


            //Posição do Jogador na mesa
            inputSignalArray[6] = (Double)_position;

            //Ultimas ações dos jogadores
            foreach (Player op in _currentGame.Players)
            {
                inputSignalArray[op.Index + 7] = (Double)op.Decisao;
            }


            //_currentGame.GameController.printLog("--------------------------------------Entradas---------------------------------------------");
            //for (int i = 0; i < 13; i++)
            //{
            //    if (i > 6)
            //    {
            //        _currentGame.GameController.printLog("Entrada " + i + " : " + _currentGame.GameController.numToTurnType((int) inputSignalArray[i]));
            //    }
            //    else
            //    {
            //        _currentGame.GameController.printLog("Entrada " + i + " : " + inputSignalArray[i]);
            //    }
                
            //}

            /*
            inputSignalArray[7] =
            inputSignalArray[8] =
            inputSignalArray[9] =
            inputSignalArray[10] =
            inputSignalArray[11] = 
            inputSignalArray[12] = 
            */


        }
        
        public TurnType foldCheckRaise(Dictionary<int, double> saidas)
        {
            int indexOp = -1;
            double max = saidas.Values.Max();

            foreach (var num in saidas)
            {
                if (max == num.Value)
                {
                    indexOp = num.Key;
                }
            }
            switch (indexOp)
            {
                case 0:
                    return TurnType.Fold;
                case 2:
                    return TurnType.Raise;
                case 3:
                    return TurnType.Check;
            }

            return TurnType.NotMade;
        }

        public TurnType foldCall(Dictionary<int, double> saidas)
        {
            int indexOp = -1;
            double max = saidas.Values.Max();

            foreach (var num in saidas)
            {
                if (max == num.Value)
                {
                    indexOp = num.Key;
                }
            }
            switch (indexOp)
            {
                case 0:
                    return TurnType.Fold;
                case 1:
                    return TurnType.Call;

            }

            return TurnType.NotMade;
        }

        public TurnType foldCallRaise(Dictionary<int, double> saidas)
        {
            int indexOp = -1;
            double max = saidas.Values.Max();

            foreach (var num in saidas)
            {
                if (max == num.Value)
                {
                    indexOp = num.Key;
                }
            }

            switch (indexOp)
            {
                case 0:
                    return TurnType.Fold;
                case 1:
                    return TurnType.Call;
                case 2:
                    return TurnType.Raise;
            }
            return TurnType.NotMade;
        }

        public TurnType foldCheckBet(Dictionary<int, double> saidas)
        {
            int indexOp = -1;
            double max = saidas.Values.Max();

            foreach (var num in saidas)
            {
                if (max == num.Value)
                {
                    indexOp = num.Key;
                }
            }

            switch (indexOp)
            {
                case 0:
                    return TurnType.Fold;
                case 1:
                    return TurnType.Call;
                case 3:
                    return TurnType.Check;
            }

            return TurnType.NotMade;
        }
       
        
        public TurnType foldCallRaiseCheck(Dictionary<int, double> saidas)
        {
            int indexOp = -1;
            double max = saidas.Values.Max();

            foreach (var num in saidas)
            {
                if (max == num.Value)
                {
                    indexOp = num.Key;
                }
            }

            switch (indexOp)
            {
                case 0:
                    return TurnType.Fold;
                case 1:
                    return TurnType.Call;
                case 2 :
                    return TurnType.Raise;
                case 3:
                    return TurnType.Check;
            }
            return TurnType.NotMade;
        }
        

        protected override void UseBlackBoxOutpts(ISignalArray outputSignalArray)
        {

         

            /*
                 sinalFold = outputSignalArray[0];
                 sinalCall = outputSignalArray[1];
                 sinalRaise = outputSignalArray[2];
                 sinalCheck =  outputSignalArray[3];
            */

            Dictionary<int, double> saidas = new Dictionary<int, double>();
            //_currentGame.GameController.printLog(Name +"  --------------------------------------Saidas------------------------------------");
            for (int i = 0; i <= 3; i++)
            {
                double val = System.Math.Round(outputSignalArray[i], 5, System.MidpointRounding.AwayFromZero);
                // _currentGame.GameController.printLog(" saida: " + i + " valor: " + val);


                saidas.Add(i, val);
            }
            //_currentGame.GameController.printLog("-------------------------------------------------------------------------------");

            /*
            saida = outputSignalArray[0];
            _currentGame.GameController.printLog(" saida: " + 0 + " valor: " + outputSignalArray[0]);
            for (int i = 1; i <= 3; i++)
            {
                _currentGame.GameController.printLog(" saida: " + i + " valor: " + outputSignalArray[i]);
                if (saida < outputSignalArray[i])
                {
                    saida = outputSignalArray[i];
                    indexOp = i;
                }
            }
            */

            PlayerState opJogadas = _currentGame.GetActivePlayerState();

            switch (opJogadas)
            {
                
                //case PlayerState.CanFoldCallRaiseCheck:
                //    _currentGame.GameController.printLog("Fold Call Raise Check ");
                //    _decisao = foldCallRaiseCheck(saidas);
                //    break;
                    
                case PlayerState.CanFoldCallRaise:
                    _currentGame.GameController.printLog("Fold Call Raise");
                    saidas.Remove(3);
                    _decisao = foldCallRaise(saidas);
                    break;
                case PlayerState.CanFoldCheckRaise:
                    _currentGame.GameController.printLog("Fold Check Raise");
                    saidas.Remove(1);
                    _decisao = foldCheckRaise(saidas);
                    break;
                case PlayerState.CanFoldCall:
                    _currentGame.GameController.printLog("Fold Call");
                    saidas.Remove(2);
                    saidas.Remove(3);
                    _decisao = foldCall(saidas);
                    break;
                case PlayerState.CanFoldCheckBet:
                    _currentGame.GameController.printLog("Fold Check Bet");
                    saidas.Remove(2);
                    _decisao = foldCheckBet(saidas);
                    break;
                //case PlayerState.IsAllIn:
                //   // _currentGame.GameController.printLog("All In");
                //    _decisao = TurnType.Check;
                    //break;
                default:
                    _decisao = TurnType.NotMade;
                  
                    break;
            }
            TomouDecisao = true;

        }

        public override float GetFitness()
        {
            /*
            if (_currentGame.GameController.HandCounter < 0)
            {
                return 0;
            }
            */
            //float player = _wins;
            //float HCount = _currentGame.HandCount;
            ////_currentGame.GameController.printLog("Calculando o Fitness-------------------------- Qtd Mao " +HCount  );
            //float fit = _wins / HandParticipated;// HCount;
            //if (fit > 0)
            //{
            //    return fit;
            //}

            //if (_wins > 0)
            //{
            //    return Wins;
            //}

            if (ROI > 0)
            {
                //Debug.Log(_currentGame.GameController.name +" "+this.Name + "  Fitness: " + System.Math.Round( ROI *100));
                return (float) System.Math.Round(ROI * 100); // ROI*100 ;
            }


            return 0;



        }

        protected override void HandleIsActiveChanged(bool newIsActive)
        {
          
            if (newIsActive)
            {
                //if (this != null) {
                //    _currentGame.GameController.printLog(this.name + " ativou ");
                //}
                FindSeatController findSeatController = GetComponent<FindSeatController>();
                findSeatController.setTbc(GameObject.Find("TableFiller").GetComponent<TableFillerController>());
                findSeatController.seat(findSeatController.getTbc().getTable());
                //  _currentGame.GameController.printLog( "Parent " + this.transform.parent.name);
            }
            
            


        }
    }
}
