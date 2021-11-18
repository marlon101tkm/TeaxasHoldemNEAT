using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
//using System.Linq;
//using System.Text;
using UnityEngine.UI;
using UnityEngine;



namespace PokerCats
{
    // TODO: move A LOT of methods into parent class
	public class RingGame : Game
	{   
        
        public RingGame(GameStartingInfo gameStartingInfo)
        {
            _bigBlindSize = gameStartingInfo.BigBlindSize;
            _anteSize = gameStartingInfo.AnteSize;
            _startingChips = gameStartingInfo.StartingChips;

            _playerCount = gameStartingInfo.PlayerCount;
        }

        public override void PrepareToStart()
        {
            gameController.OnBlindsPosted = OnBlindsPosted;
            gameController.OnCardsDealt = OnCardsDealt;
            gameController.OnPlayerTurnEnded = OnPlayerTurnEnded;
            gameController.OnStreetDealt = OnStreetDealt;
            gameController.OnShowdownEnded = OnShowdownEnded;


            _players = gameController.GetNeatInstances();
            //GameController.printLog("QTD Players na mesa "+GameController.name   +"  "+ _playerCount );
            CreatePlayers();
           // GameController.tableFillerController.realocatePlayers(_players);

            SetPlayersInitialPositions();


            _deck = new Deck();
             GameController.GameRunning = false;
            
            //GameController.printLog(" Prepare To Start finished" +  GameController.name  );
           // Start();
        }

        public override void Start()
        {
            gameController.preparationMade = true;
            
            PlayHand();
        }

        public override void PlayHand()
        {
            //ANNInprogress.Instance.setTableProgress("Table1",false);
            // gameController.neatSupervisor.FimTrial = true;
            //gameController.ANNInProgress = true;
            //gameController.neatSupervisor.SendMessage("SetTempoTrial", true);

            GameController.CanPlayHand = false;
            GameController.neatSupervisor.CanStartNewHand = true;
            //GameController.CanPlayNextTurn = false;
            //GameMenager.Instance.setCanPlayHand(gameController.name, true);

            _deck.StartNewHand();
            
            Hand newHand = new Hand();

            SetChipsToPlayers();
            //gameController.HandCounter++;

            //GameController.printLog("iniciou mao: "+ gameController.HandCounter);
            GameController.printLog( GameController.name +"--------------------------------------------------------------iniciou mao: " + this.CountHand);
            newHand.AddPlayers(_players);
            // For the first hand, players positions are already set in Start method
            if (!IsFirstHand) {
                newHand.SetPlayersPositions();
            }
            newHand.SetBlindsAndAntes(_bigBlindSize, _anteSize);
            newHand.PostBlindsAndAntes();
            _hands.Add(newHand);

            addaHandtoPlayer();
            //GameController.CurrentHandState.SetState(HandState.Preflop);
            GameController.CurrentHandState.SetState(HandState.Preflop);

            Pot mainPot = new Pot();
            newHand.AddPot(mainPot);
            gameController.AddPot();

            PostBlindsAndAntes();
        }

        public void addaHandtoPlayer()
        {
           foreach(Player play in  _players){
                play.HandParticipated++;
            }
        }

        public override void SetChipsToPlayers()
        {
            foreach (Player player in _players)
            {
                player.ChipCount = StartingChips;
            }
        }

        public override void PlayTurn()
        {
            GameController.printLog("RingGame PlayTurn");
            base.PlayTurn();
            UpdateHUD();
            MakeAITurnIfNeeded();
        }

        private bool HasEnded()
        {
            return false;
        }

        public override Player GetLocalPlayer()
        {
            foreach (Player player in _players)
            {
                if (player.IsLocal)
                {
                    return player;
                }
            }

            return null;
        }

        public override int GetLocalPlayerIndex()
        {
            //for (int playerIndex = 0; playerIndex < _playerCount; playerIndex++) {
            //    Player player = _players[playerIndex];
            //    if (player.IsLocal) {
            //        return playerIndex;
            //    }
            //}

            //return -1;
            return 0;
        }

        private void OnBlindsPosted()
        {
            //PlaceButtonAndDeck();
            DealCardsToPlayers();
        }

        private void OnCardsDealt()
        {
            // TODO: move this to separate method
            if (GameController.SceneName != "NEAT") {
                //Player localPlayer = GetLocalPlayer();
                Player localPlayer = _players[0];
                PlayerHandInfo playerHandInfo;
                //string handTypeString = HandChecker.Instance.GetFullHandTypeString(out playerHandInfo, localPlayer.HoleCards);
                string handTypeString =  GameController.HandChecker.GetFullHandTypeString(out playerHandInfo, localPlayer.HoleCards);

                if (string.IsNullOrEmpty(handTypeString))
                {
                    Debug.LogError("DealCardsToPlayers: could not evaluate local player's hand.");
                    return;
                }

                gameController.SetHandTypeHintText(handTypeString);
            }

            BeginNewBettingRound();
        }

        private void BeginNewBettingRound()
        {
            GameController.printLog("BeginNewBettingRound " + GameController.name );
            // GameController.printLog("Tempo trial: " + GameController.neatSupervisor.GetTempoTrial() );
            //GameMenager.Instance.NeatTrialInprogress = true;
            
            GameController.CurrentAmoutToBet = BigBlindSize;
            SetPlayerFirstToAct();

            UpdateHUD();

            if (GameController.SceneName != "NEAT" && GameController.SceneName != "TESTS")
            {
                gameController.ClearPlayerActions();
            }

            MakeAITurnIfNeeded();
        }

        private void UpdateHUD()
        {
            //GameController.printLog("UpdateHUD");
            gameController.CurrentPlayerIndex = _currentPlayerIndex;
            // GameController.printLog("estado do jogador" + GetActivePlayerState().ToString());
            //GameController.printLog("?????????????");
            // ActivePlayerState.Instance.SetState(GetActivePlayerState());
            if (GameController.SceneName != "NEAT" && GameController.SceneName != "TESTS")
            {
                gameController.HighlightActivePlayer(true);
                
                UpdateButtonsText();
                gameController.StartTurnTimer();
            }
        }

        private void MakeAITurnIfNeeded()
        {
            GameController.printLog("MakeAITurnIfNeeded, current player: " + CurrentPlayer.Name +"Player Class:"+ CurrentPlayer.PClass);//+" Position "+CurrentPlayer.Position);
            //GameController.printLog("MakeAITurnIfNeeded, current player: " + CurrentPlayer.Name + " é IA?: " + CurrentPlayer.IsAI + " é AllIn: " + !CurrentPlayer.IsAllIn + " é jogador está envolvido:  " + CurrentHand.IsPlayerInvolved(CurrentPlayer));
            
            if (NeedToMakeAITurn()) {
               // GameController.printLog("Vai fazer o turno");
                gameController.MakeAITurn();
            }
        }

        private bool NeedToMakeAITurn()
        {
            //GameController.printLog( " é IA? "+ CurrentPlayer.IsAI + " Jogador esta envolvido na mão?  "+ CurrentHand.IsPlayerInvolved(CurrentPlayer) +" é All In: "+ CurrentPlayer.IsAllIn);
            return CurrentPlayer.IsAI &&
                CurrentHand.IsPlayerInvolved(CurrentPlayer)  &&
                !CurrentPlayer.IsAllIn;
        }

        public PlayerClass setPlayerClass(Player prefab)
        {
            if (prefab is PlayerTA)
            {
                return PlayerClass.TA;
            }
            if (prefab is PlayerTP)
            {
                return PlayerClass.TP;
            }
            if (prefab is PlayerLA)
            {
                return PlayerClass.LA;
            }

            if (prefab is PlayerLP)
            {
                return PlayerClass.LP;
            }

            if (prefab is PlayerRandom)
            {
                return PlayerClass.Random;
            }

            if (prefab is Player)
            {
                return PlayerClass.Neat;
            }

            return PlayerClass.Invalid;

        }


        private void CreatePlayers()
        {
            if (GameController.SceneName == "NEAT" || GameController.SceneName == "TESTS")
            {
                //GameController.printLog("Executando NEAT");
                int playerIndex = 0;
                //GameController.printLog("Criando Jogadores: "+ gameController.name);
                foreach (var player in _players)
                {
                    // GameController.printLog( player.name);
                    CreatePlayer(player, playerIndex, Defines.positions[playerIndex], _startingChips);
                    playerIndex++;
                }
            }
            else
            {
                for (int playerIndex = 0; playerIndex < _playerCount; playerIndex++)
                {
                    // TODO: this is for test, return _startingChips
                    CreatePlayer(playerIndex, Defines.positions[playerIndex], /*playerIndex * 100 + 500*/ _startingChips);
                }
            }

        }


        private void CreatePlayer(Player playerToAdd ,int playerIndex, Vector3 position, int startingChips)
        {
            if (GameController.SceneName == "TESTS")
            { 
                playerToAdd.PClass = setPlayerClass(playerToAdd);
            }
            playerToAdd.Index = playerIndex;
            playerToAdd.Type = PlayerType.AI;
            playerToAdd.ChipCount = startingChips;
            playerToAdd.InitChipCount = startingChips;

            playerToAdd.Name = "Bot Cat " + playerIndex;
            playerToAdd._currentGame = this;
            gameController.CreatePlayer(playerToAdd, position, playerIndex);
        }

        private void CreatePlayer(int playerIndex, Vector3 position , int startingChips)
        {
            Player playerToAdd;

            // TODO: this is for debug only
            /*
            if (playerIndex == 0) {
               playerToAdd = new Player("Real Player", startingChips, PlayerType.Local);
           } else {
                playerToAdd = new Player("Bot Cat " + playerIndex, startingChips, PlayerType.AI);
            }
            */

            //PlayerAI ai = new PlayerAI();
            //ai.CurrentGame = this; 

            if (gameController.SceneName == "NEAT")
            {
                playerToAdd = new Player("Bot Cat " + playerIndex, startingChips, PlayerType.AI, playerIndex);
            }
            else
            {
                playerToAdd = gameController.PlayersParent.transform.Find("Player" + playerIndex).gameObject.GetComponent<Player>();

                playerToAdd.Index = playerIndex;
                playerToAdd.Type = PlayerType.AI;
                playerToAdd.ChipCount = startingChips;
                playerToAdd.Name = "Bot Cat " + playerIndex;
            }



            //playerToAdd = new Player("Bot Cat " + playerIndex, startingChips, PlayerType.AI, playerIndex );
            //GameController.printLog("Indice do Jogador: "+ playerToAdd.Index );
            _players.Add(playerToAdd);
            gameController.CreatePlayer(playerToAdd,position ,playerIndex );
        }

        private void DealCardsToPlayers()
        {
            Dictionary<int, HoleCards> cardsToDeal = new Dictionary<int, HoleCards>();

            int playerIndex = GetPlayerOnTheSmallBlindIndex();
            int playersReady = 0;

            while (playersReady < _playerCount) {
                Player player = _players[playerIndex];
                
                for (int cardIndex = 0; cardIndex < Defines.HOLE_CARDS_COUNT; cardIndex++)
                {
                    //if (player.HoleCards.First == null | player.HoleCards.First == null)
                    //{
                        // GameController.printLog("Cartas do jogador: " + player.Name);
                        Card cardToDeal = _deck.DealTopCard();
                        player.AddHoleCard(cardToDeal);
                        //cardToDeal.IsOpened = true;                    

                        // TODO: add check that we have only one local player
                        if (!player.IsLocal)
                        {
                            cardToDeal.IsOpened = false;
                        }
                    //}
                }
            

                cardsToDeal.Add(playerIndex, player.HoleCards);

                playerIndex++;
                playersReady++;
                if (playerIndex >= _playerCount) {
                    playerIndex = 0;
                }
            }

            gameController.DealCards(cardsToDeal);
        }

        private void PlaceButtonAndDeck()
        {
            if (GameController.SceneName != "NEAT" && GameController.SceneName != "TESTS") {
                int playerOnTheButtonIndex = GetPlayerOnTheButtonIndex();
                gameController.GiveButtonToPlayer(playerOnTheButtonIndex);
                // TODO: decide if we should have separate dealer
                gameController.GiveDeckToPlayer(playerOnTheButtonIndex);
            }
        }

        private void PostBlindsAndAntes()
        {
            int mainPotSize = 0;
            int smallBlindIndex = 0;
            int bigBlindIndex = 0;

            for (int playerIndex = 0; playerIndex < _playerCount; playerIndex++) {
                Player player = _players[playerIndex];
                if (player != null) {
                    // TODO: in Game controller don't search player at new to post blind after ante (optimization)
                    if (_anteSize > 0) {
                        mainPotSize += _anteSize;
                        gameController.PostPlayerAnte(playerIndex, _anteSize);
                    }

                    if (player.IsOnSB) {
                        mainPotSize += _bigBlindSize / 2;
                        CurrentHand.MainPot.PlayersInPot.Add(player);
                        smallBlindIndex = playerIndex;
                    } else if (player.IsOnBB) {
                        mainPotSize += _bigBlindSize;
                        CurrentHand.MainPot.PlayersInPot.Add(player);
                        bigBlindIndex = playerIndex;
                    }
                    if (GameController.SceneName != "NEAT" && GameController.SceneName != "TESTS" && GameController.SceneName != "TESTS" ) {
                        gameController.UpdatePlayerCard(playerIndex, player);
                    }
                }
            }

            CurrentHand.MainPot.Size += mainPotSize;
            //gameController.UpdatePotSizeTexts(CurrentHand.Pots);

            gameController.PostBlinds(smallBlindIndex, bigBlindIndex, _bigBlindSize);
        }

        public override PlayerState GetActivePlayerState()
        {
            //if (CurrentPlayer.IsAI) {
            //    return PlayerState.IsAI;
            //}

            // GameController.printLog("Posição atual jogador: "+  CurrentPlayer.Position);
           // GameController.printLog("-------------------------------------------------Checando Estado Jogador: "+ CurrentPlayer.Name + "--------------------------------------------------------");
            //GameController.printLog(" Maior aposta nao no pot: "+ CurrentHand.GetHighestBetNotInPot() + " Aposta Atual: " + CurrentPlayer.CurrentBet);
            //GameController.printLog("------------------------------------------Terminou Checagem estado jogador---------------------------------------------------------------------------------");
            int uncalledBet = CurrentHand.GetHighestBetNotInPot() - CurrentPlayer.CurrentBet;
            bool canCall = uncalledBet > 0;
            bool canCheck = uncalledBet == 0;//IfPodeDarCheck(uncalledBet);
            //bool canBet = canCheck && !GameController.CurrentHandState.IsPreflop;
            bool canBet = canCheck && !GameController.CurrentHandState.IsPreflop;
            bool canRaise = uncalledBet >= 0 && CurrentPlayer.ChipCount > uncalledBet; // && !GameController.CurrentHandState.IsPreflop;

            //GameController.printLog("Call ? " + canCall + " Check ? " + canBet + " Bet ? " + canBet + " Raise ? " + canRaise);

            if (CurrentPlayer.IsAllIn) {
                return PlayerState.IsAllIn;
            
            //    //addicionado
            //} else if (canCall && canRaise && canCheck ) {
            //    return PlayerState.CanFoldCallRaiseCheck;
            //    //
            } else if (canCall && canRaise) {
                return PlayerState.CanFoldCallRaise;
            } else if (canCheck && canBet) {
                return PlayerState.CanFoldCheckBet;
            } else if (canCheck && canRaise) {
                return PlayerState.CanFoldCheckRaise;
            } else if (canCall) {
                return PlayerState.CanFoldCall;
            }

            return PlayerState.Invalid;
        }

        private bool IfPodeDarCheck(int uncalledBet)
        {
            if (CurrentPlayer.Position == Position.SB && GameController.CurrentHandState.IsPreflop )
            {
                return uncalledBet ==  SmallBlindSize;
                                
            }

            return uncalledBet == 0;
        }

        private void UpdateButtonsText()
        {
            SetCallButtonTextIfNeeded();
            SetBetButtonTextIfNeeded();
            SetRaiseButtonTextIfNeeded();
        }

        private void SetCallButtonTextIfNeeded()
        {
            int uncalledBet = CurrentHand.GetHighestBetNotInPot() - CurrentPlayer.CurrentBet;
            if (uncalledBet > 0) {
                gameController.SetCallButtonText(uncalledBet.ToString());
            }
        }

        private void SetBetButtonTextIfNeeded()
        {
            int uncalledBet = CurrentHand.GetHighestBetNotInPot() - CurrentPlayer.CurrentBet;
            //bool canBet = uncalledBet == 0 && !GameController.CurrentHandState.IsPreflop;
            bool canBet = uncalledBet == 0 && !GameController.CurrentHandState.IsPreflop;
            if (canBet) {
                gameController.SetBetButtonText(_bigBlindSize.ToString());
            }
        }

        private void SetRaiseButtonTextIfNeeded()
        {
            int uncalledBet = CurrentHand.GetHighestBetNotInPot() - CurrentPlayer.CurrentBet;
            bool canRaise = uncalledBet > 0 && CurrentPlayer.ChipCount > uncalledBet;
            if (canRaise) {
                int minRaiseAmount = Math.Min(CurrentHand.GetHighestBetNotInPot() * 2, CurrentPlayer.ChipCount);
                gameController.SetRaiseButtonText(minRaiseAmount.ToString());
            }
        }


        private void UpdatePlayerCardAndPot()
        {
            gameController.UpdatePlayerCard(CurrentPlayerIndex, CurrentPlayer);
            gameController.UpdatePotSizeTexts(CurrentHand.Pots);
        }

        private void OnPlayerTurnEnded(TurnType turnType, int amount)
        {
            GameController.printLog("OnPlayerTurnEnded");
            if (turnType == TurnType.Invalid)
            {
                GameController.printLog("OnPlayerTurnEnded: invalid turn type.");
                return;
            }

            if (turnType == TurnType.Fold)
            {
                CurrentHand.RemovePlayer(CurrentPlayer);

                GameController.printLog("Jogador " + CurrentPlayer.Name + " deu Fold ");

            }
            else if (turnType == TurnType.Call)
            {

                //GameController.printLog("Jogador " + CurrentPlayer.Name + " deu Call");

                PutChipsToPot(CurrentPlayer, amount);
                GameController.printLog("Jogador " + CurrentPlayer.Name + " deu Call Aposta atual: "+ CurrentPlayer.CurrentBet );
                if (gameController.SceneName != "NEAT" && gameController.SceneName != "TESTS") { UpdatePlayerCardAndPot(); }

            }
            else if (turnType == TurnType.Raise)
            {

                GameController.printLog("Jogador " + CurrentPlayer.Name + " deu Raise");

                PutChipsToPot(CurrentPlayer, amount);
                if (gameController.SceneName != "NEAT" && gameController.SceneName != "TESTS") { UpdatePlayerCardAndPot(); }

            }
            else if (turnType == TurnType.Bet)
            {

                GameController.printLog("Jogador " + CurrentPlayer.Name + " deu Bet");

                PutChipsToPot(CurrentPlayer, amount);
                if (gameController.SceneName != "NEAT" && gameController.SceneName != "TESTS") { UpdatePlayerCardAndPot(); }

            }
            else if (turnType == TurnType.Check)
            {

                GameController.printLog("Jogador " + CurrentPlayer.Name + " deu check");
                // TODO
            }

            CurrentPlayer.LastTurn = turnType;

            if (gameController.SceneName != "NEAT" && gameController.SceneName != "TESTS")
            {
                gameController.HighlightActivePlayer(false);
                gameController.StopTurnTimer();
            }
             GameController.CanPlayNextTurn = true;

            // GameController.neatSupervisor.CanStartNewHand = true;
            // GameController.printLog("Can Start New Hand " + GameController.neatSupervisor.CanStartNewHand);
            //GameController.CanPlayHand = false;

            //if (!HandHasWinner(CurrentHand))
            //{
            //    // GameController.printLog("Turno nao acabou");

            //    // GameController.printLog( "Turno atual: "+ GameController.CurrentHandState.QualTurno() );
            //    if (NeedToDealNextStreet())
            //    {
            //        // GameController.printLog(" Proxima rodada");
            //        DealNextStreet();
            //    }
            //    else
            //    {
            //        //GameController.printLog(" Proximo jogador");
            //        PlayTurn();
            //    }
            //}
            //else
            //{
            //    // GameController.printLog("Acabou a mão: " + this.CountHand);
            //    EndHand(CurrentHand);
            //}
        }

        public override void checkNextTurn()
        {
            
            if (!HandHasWinner(CurrentHand))
            {
                // GameController.printLog("Turno nao acabou");

                // GameController.printLog( "Turno atual: "+ GameController.CurrentHandState.QualTurno() );
                if (NeedToDealNextStreet())
                {
                    // GameController.printLog(" Proxima rodada");
                    DealNextStreet();
                }
                else
                {
                    //GameController.printLog(" Proximo jogador");
                    PlayTurn();
                }
            }
            else
            {
                // GameController.printLog("Acabou a mão: " + this.CountHand);
                EndHand(CurrentHand);
            }
        }


        // TODO: check if we should apply this function for blinds posting
        private void PutChipsToPot(Player player, int amount)
        {
            Pot potToPutChips = null;

            foreach (Pot pot in CurrentHand.Pots) {
                bool hasPlayersAllIn = false;
                foreach (Player playerInPot in pot.PlayersInPot) {
                    if (playerInPot.IsAllIn) {
                        hasPlayersAllIn = true;
                        break;
                    }
                }
                if (!hasPlayersAllIn) {
                    potToPutChips = pot;
                    break;
                }
            }

            if (potToPutChips == null) {
                Pot newPot = new Pot();
                CurrentHand.AddPot(newPot);
                gameController.AddPot();
                potToPutChips = newPot;
            }

            player.PutChipsToPot(potToPutChips, amount);
        }

        private void SetPlayerFirstToAct()
        {
            //int playerFirstToActIndex = GameController.CurrentHandState.IsPreflop ? GetPlayerOnTheBigBlindIndex() + 1 : GetPlayerOnTheButtonIndex() + 1;
            int playerFirstToActIndex = GameController.CurrentHandState.IsPreflop ? GetPlayerOnTheBigBlindIndex() + 1 : GetPlayerOnTheButtonIndex() + 1;
            //GameController.printLog("Primeiro jogador a agir" + playerFirstToActIndex);
            if (playerFirstToActIndex >= _playerCount) {
                playerFirstToActIndex = 0;
            }
            
            Player playerFirstToAct = _players[playerFirstToActIndex];
            
            while (!CurrentHand.IsPlayerInvolved(playerFirstToAct)) {
                playerFirstToActIndex++;
                if (playerFirstToActIndex >= _playerCount) {
                    playerFirstToActIndex = 0;
                }
                playerFirstToAct = _players[playerFirstToActIndex];
            }
            
            _currentPlayerIndex = playerFirstToActIndex;
           // GameController.printLog("Primeiro jogador a agir: " + _currentPlayerIndex);
        }

        private bool AreTherePlayersLeftInHand(Hand hand)
        {
            return hand.HasPlayersLeft();
        }

        private bool HandHasWinner(Hand hand)
        {
            return hand.HasOnePlayerLeft();
        }

        private bool testUncalledBetPreFlop(Player player, int uncalledBet) {

            int comp = GameController.CurrentAmoutToBet - player.CurrentBet;
            if (GameController.CurrentHandState.IsPreflop && player.Position == Position.SB )
            {
                return !((uncalledBet / 2 ) == player.CurrentBet);
            }
            return true;
        }


        private bool NeedToDealNextStreet()
        {
            bool allThePlayersMadeTurn = true;
            bool noUncalledBets = true;
            int playerNotAllInCount = 0;
            string palavra = "Jogadores involvidos "+ CurrentHand.PlayersInvolved.Count +" Testando se pode fazer proximo turno: ";
            //GameController.printLog("Testando se pode fazer proximo turno: ");
            foreach (Player player in CurrentHand.PlayersInvolved) {
                
                if (player.IsAllIn) {

                    continue;
                }

                playerNotAllInCount++;

                int uncalledBet = CurrentHand.GetHighestBetNotInPot() - player.CurrentBet;

                if (  uncalledBet > 0  )
                {
                    if (testUncalledBetPreFlop(player,uncalledBet)) {
                     //   GameController.printLog(" Jogador:  " + player.Name + " Highest Bet Not In Pot: " + CurrentHand.GetHighestBetNotInPot() + " CurrentBet: " + player.CurrentBet);
                        noUncalledBets = false;
                    }
                }


                if (!player.HasMadeTurn) {
                   // GameController.printLog("Jogador: "+ player.Name+" ------------------Jogador não realizou turno");
                    allThePlayersMadeTurn = false;
                }
                //else
                //{
                //    GameController.printLog("Jogador: " + player.Name + " ------------------ realizou turno  "+ player.LastTurn );
                //}
            }

            if (noUncalledBets) {
                if (allThePlayersMadeTurn || playerNotAllInCount <= 1) {
                    GameController.printLog(palavra +" pode");
                    return true;
                }
            }

            GameController.printLog(palavra+" nao pode");
            return false;
        }

        private void DealNextStreet()
        {
            //if (GameController.SceneName != "NEAT" && GameController.SceneName != "TESTS")
            //{
            //    gameController.PutChipsIntoPot();
            //}
            //gameController.PutChipsIntoPot();
            ResetInvolvedPlayersBets();
            ResetLastPlayersTurn();
            GameController.printLog(/*"Jogador: " + CurrentPlayer.Name*/ GameController.name  + "--------------------------------------------------------------Turno atual: " + GameController.CurrentHandState.QualTurno());
            //string palavra = "";
            //foreach (Player play in CurrentHand.PlayersInvolved)
            //{
            //    //  GameController.printLog(play.Name + "QTD pot:" + play.ChipCount);
            //    palavra += "  // " + play.Name + "  QTD pot: " + play.ChipCount + " // ";
            //}
            //GameController.printLog("Qtd de Jogadores envolvidos na mão  " + CurrentHand.PlayersInvolved.Count + palavra);

            if (GameController.CurrentHandState.IsPreflop)
            {
                DealFlop();
            }
            else if (GameController.CurrentHandState.IsFlop)// && gameController.IsFlopDealt)
            {
                DealTurn();
            }
            else if (GameController.CurrentHandState.IsTurn )//&& gameController.IsTurnDealt)
            {
                DealRiver();
            }
            else if (GameController.CurrentHandState.IsRiver)// && gameController.IsRiverDealt)
            {
                BeginShowdown();
            }
                    


            //if (GameController.CurrentHandState.IsPreflop)
            //{
            //    DealFlop();
            //}
            //else if (GameController.CurrentHandState.IsFlop && gameController.IsFlopDealt)
            //{
            //    DealTurn();
            //}
            //else if (GameController.CurrentHandState.IsTurn && gameController.IsTurnDealt)
            //{
            //    DealRiver();
            //}
            //else if (GameController.CurrentHandState.IsRiver && gameController.IsRiverDealt)
            //{
            //    BeginShowdown();
            //}

            //if (!GameController.CurrentHandState.IsShowdown)
            //{
            //    BeginNewBettingRound();
            //}

        }

        private void ResetLastPlayersTurn()
        {
            foreach (Player player in CurrentHand.PlayersInvolved) {
                player.LastTurn = TurnType.NotMade;
            }
        }

        private void ResetInvolvedPlayersBets()
        {
            foreach (Player player in CurrentHand.PlayersInvolved) {
                player.CurrentBet = 0;
            }
        }

        private void ResetAllPlayersBets()
        {
            foreach (Player player in _players) {
                player.CurrentBet = 0;
            }
        }

        private void UpdatePlayersHands()
        {
            foreach (Player player in CurrentHand.PlayersInvolved) {
                if (player.IsLocal) {
                    UpdateLocalPlayerHand(player);
                } else {
                    PlayerHandInfo playerHandInfo = GameController.HandChecker.GetPlayerHandInfo(player.HoleCards, _board.FullBoard);

                    if (playerHandInfo.HandType == HandType.Invalid) {
                        Debug.LogError("UpdatePlayersHands: could not get player hand type.");
                        continue;
                    }

                    player.CurrentHandInfo = playerHandInfo;
                }
            }
        }

        private void UpdateLocalPlayerHand(Player localPlayer)
        {
            if (!localPlayer.IsLocal) {
                Debug.LogError("UpdateLocalPlayerHand: player is not local!");
                return;
            }

            PlayerHandInfo playerHandInfo;
            String handTypeString = GameController.HandChecker.GetFullHandTypeString(out playerHandInfo, localPlayer.HoleCards, _board.FullBoard);

            if (playerHandInfo.HandType == HandType.Invalid) {
                Debug.LogError("UpdateLocalPlayerHand: could not get local player hand type.");
                return;
            }

            localPlayer.CurrentHandInfo = playerHandInfo;

            if (string.IsNullOrEmpty(handTypeString)) {
                Debug.LogError("UpdateLocalPlayerHand: could not get local player hand type string.");
                return;
            }

            gameController.SetHandTypeHintText(handTypeString);
        }

        private void DealFlop()
        {
            _board.Flop = _deck.DealFlop();
            GameController.CurrentHandState.SetState(HandState.Flop);
            //gameController.DealFlop(_board.Flop);
            OnStreetDealt();
        }

        private void DealTurn()
        {
            GameController.printLog("Executando Turn ");
            _board.TurnCard = _deck.DealTurnOrRiver();
            GameController.CurrentHandState.SetState(HandState.Turn);
            //gameController.DealTurn(_board.TurnCard);
            OnStreetDealt();
        }

        private void DealRiver()
        {
            GameController.printLog("Executando River");
            _board.RiverCard = _deck.DealTurnOrRiver();
            GameController.CurrentHandState.SetState(HandState.River);
            //gameController.DealRiver(_board.RiverCard);
            OnStreetDealt();
        }

        private void OnStreetDealt()
        {
            UpdatePlayersHands();
            //GameController.printLog("Vai fazer proxima Etapa? " + NeedToDealNextStreet());
            if (NeedToDealNextStreet()) {
                DealNextStreet();
            }
            else
            {
                BeginNewBettingRound();
            }
        }

        private void BeginShowdown()
        {
            GameController.printLog(GameController.name +"  Executando Showdown Cont mao: "+ HandCount );
            // TODO: add muck possibility
            GameController.CurrentHandState.SetState(HandState.Showdown);

            List<int> playerIndexes = new List<int>();

            for (int playerIndex = 0; playerIndex < _playerCount; playerIndex++) {
                Player player = _players[playerIndex];
                if (CurrentHand.IsPlayerInvolved(player) /*&& !player.IsLocal*/) {
                    playerIndexes.Add(playerIndex);
                    //GameController.printLog("Jogador envolvido: "+ player.Name );
                }
            }
            gameController.ProcessShowdown(playerIndexes);
        }

        private void OnShowdownEnded()
        {
            EndHand(CurrentHand);
        }

        private void EndHand(Hand hand)
        {
            if (GameController.SceneName =="TESTS")
            {
                hand.GivePotsToWinners(HandCount);
            }
            else
            {
                hand.GivePotsToWinners();
            }
            

            for (int playerIndex = 0; playerIndex < _playerCount; playerIndex++) {
                Player player = _players[playerIndex];

                player.ClearHoleCards();
                
            }

            ResetAllPlayersBets();
            ClearBoard();

            gameController.EndHand();

            
            if (!HasEnded()) {
                GameController.printLog("Terminou "+ GameController.name);
                
                HandCount++;
                
                GameController.CanPlayHand = true;
                GameController.neatSupervisor.CanStartNewHand = !GameController.tableFillerController.waitOtherTabbles();
                GameController.printLog( "pode comecar uma mao nova"+ GameController.neatSupervisor.CanStartNewHand);
                
            }
        }
	}
}
