using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace PokerCats
{
    public struct GameStartingInfo
    {
        public int BigBlindSize;
        public int AnteSize;
        public int StartingChips;

        public int PlayerCount;
    }

    public abstract class Game 
    {
        protected int _bigBlindSize;
        protected int _anteSize;
        protected int _startingChips;

        public int StartingChips
        {
            get { return _startingChips; }
            
        }

        protected int _handCount;
        public int HandCount
        {
            get { return _handCount; }
            set { _handCount = value; }
        }

        protected GameController gameController;
        
        public GameController  GameController
        {
            get { return gameController; }
            set { gameController = value; }
        }
        
        protected int _playerCount;
        public int NumPlayers
        {
            get { return _playerCount; }
        }
        protected int _currentPlayerIndex = -1;

        protected List<Player> _players = new List<Player>();
        protected List<Hand> _hands = new List<Hand>();

        protected Deck _deck;
        protected Board _board = new Board();

        public Board Board
        {
            get { return _board; }
        }

        public int SmallBlindSize
        {
            get { return _bigBlindSize / 2; }
        }

        public int BigBlindSize
        {
            get { return _bigBlindSize; }
        }

        public int AnteSize
        {
            get { return _anteSize; }
        }

        public List<Player> Players
        {
            get { return _players; }
        }

        public int CurrentPlayerIndex
        {
            get { return _currentPlayerIndex; }
        }

        public bool GetPlayerAction()
        {
            foreach(Player player in _players)
            {
                GameController.printLog(" BET OU RAISE "+ player.HasBetOrRaise);
                if (player.HasBetOrRaise)
                {
                    return true;
                }
            }
            return false;
        }

        public Player CurrentPlayer
        {
            get
            {
                if (_currentPlayerIndex < 0 || _currentPlayerIndex >= _playerCount) {
                    Debug.LogError("CurrentPlayer: invalid current player index.");
                    return null;
                }

                return _players[_currentPlayerIndex];
            }
        }

        public Hand CurrentHand
        {
            get { return _hands.Last(); }
        }

        public bool IsFirstHand
        {
            get { return _hands.Count == 0; }
        }

        public int CountHand
        {
            get { return _hands.Count; }
        }

        public virtual void PrepareToStart()
        {

        }

        public virtual void Start()
        {

        }

        public virtual void PlayHand()
        {

        }

        public virtual Player GetLocalPlayer()
        {
            return null;
        }

        public virtual int GetLocalPlayerIndex()
        {
            return -1;// -1 ;
        }

        public virtual void SetChipsToPlayers()
        {

        }

      

        public virtual void PlayTurn()
        {
            GameController.printLog("Game PlayTurn");
            //_currentPlayerIndex++;

            //if (_currentPlayerIndex >= _playerCount) {
            //    _currentPlayerIndex = 0;
            //}

            //// TODO: check if this causes some issues
            //if (!CurrentHand.IsPlayerInvolved(CurrentPlayer) || CurrentPlayer.IsAllIn) {
            //    PlayTurn();
            //}


            IncrementPlayerIndex();
            while (/*!CurrentHand.IsPlayerInvolved(CurrentPlayer)*/ CurrentPlayer.IsAllIn || !CurrentHand.IsPlayerInvolved(CurrentPlayer)  )
            {
                IncrementPlayerIndex();
            }
            //GameController.printLog("Indice do jogador: " + _currentPlayerIndex);

        }

        public abstract PlayerState GetActivePlayerState();

        public virtual void ClearBoard()
        {
            //GameController.printLog("Mesa:" + _board.Flop[0].GetTextInfo()+"   "+ _board.Flop[1].GetTextInfo() + "   " + _board.Flop[2].GetTextInfo() + "   " + _board.TurnCard.GetTextInfo() + "   " + _board.RiverCard.GetTextInfo());
            _board.Flop.Clear();
            _board.TurnCard = null;
            _board.RiverCard = null;
        }

        public List<Player> Shuffle(List<Player> players )
        {
            System.Random random = new System.Random();

            for (int i = 0; i < players.Count; i++)
            {
                int j = random.Next(i, players.Count);
                Player temporary = players[i];
                players[i] = players[j];
                players[j] = temporary;
            }

            return players;
        }

        protected void SetPlayersInitialPositions()
        {
            Position position = Position.Count - _playerCount;

            List<Player> players;

            if (GameController.SceneName ==" NEAT")
            {
                players = Shuffle(_players);
            }
            else
            {
                players = _players;
            }

             

            foreach (Player player in players) {
                if (Utils.IsPositionValid(position)) {
                    player.Position = position;
                    position++;
                } else {
                    Debug.LogError("SetPlayersInitialPositions: invalid position!");
                    break;
                }
            }
        }

        public int GetPlayerOnTheButtonIndex()
        {
            bool isPlayerOnTheButtonFound = false;
            int playerOnBUIndex = 0;
            bool isPlayerOnSBFound = false;
            int playerOnSBIndex = 0;

            for (int playerIndex = 0; playerIndex < _playerCount; playerIndex++) {
                Player player = _players[playerIndex];
                if (player != null) {
                    if (player.Position == Position.BU) {
                        playerOnBUIndex = playerIndex;
                        isPlayerOnTheButtonFound = true;
                    } else if (player.Position == Position.SB) {
                        playerOnSBIndex = playerIndex;
                        isPlayerOnSBFound = true;
                    }
                }
            }

            // Most probably, HU case, when SB should receive the button
            if (isPlayerOnTheButtonFound) {
                return playerOnBUIndex;
            } else if (isPlayerOnSBFound) {
                return playerOnSBIndex;
            } else {
                Debug.LogError("GetPlayerOnTheButtonIndex: no player found with position BU or SB.");
                return -1;
            }
        }

        protected int GetPlayerOnTheSmallBlindIndex()
        {
            for (int playerIndex = 0; playerIndex < _playerCount; playerIndex++) {
                Player player = _players[playerIndex];
                if (player != null) {
                    if (player.Position == Position.SB) {
                        return playerIndex;
                    }
                }
            }

            Debug.LogError("GetPlayerOnTheSmallBlindIndex: no player found with position SB.");
            return -1;
        }

        

        public virtual void checkNextTurn() { }

        protected int GetPlayerOnTheBigBlindIndex()
        {
            for (int playerIndex = 0; playerIndex < _playerCount; playerIndex++) {
                Player player = _players[playerIndex];
                if (player != null) {
                    if (player.Position == Position.BB) {
                        return playerIndex;
                    }
                }
            }

            Debug.LogError("GetPlayerOnTheBigBlindIndex: no player found with position BB.");
            return -1;
        }

        

        private void IncrementPlayerIndex()
        {
            _currentPlayerIndex++;
            if (_currentPlayerIndex >= _playerCount) {
                _currentPlayerIndex = 0;
            }
        }
	}
}
