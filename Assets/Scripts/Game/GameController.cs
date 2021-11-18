using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;
using UnitySharpNEAT;
using System.Threading.Tasks;


namespace PokerCats
{
    public enum TurnType
    {
        Invalid = -1,
        NotMade,
        Fold,
        Check,
        Call,
        Bet,
        Raise,
        Count
    };

    public class GameController : MonoBehaviour //SingletonMonoBehaviour<GameController>
    {
        public GameObject CardPrefab;
        public GameObject OpponentCardPrefab;

        public GameObject Board;
        public GameObject PotsParent;

        public GameObject PlayersParent;

        public GameObject ChipPrefab;
        public GameObject ButtonPrefab;
        public GameObject DeckPrefab;

        public NeatSupervisor neatSupervisor;

        // Buttons
        public RectTransform FoldButton;
        public RectTransform CheckButton;
        public RectTransform CallButton;
        public RectTransform BetButton;
        public RectTransform RaiseButton;

        public InputField BetInputField;
        public Slider BetSlider;

        public Text HandTypeHintText;



        // Actions
        public Action OnBlindsPosted;
        public Action OnCardsDealt;
        public Action<TurnType, int> OnPlayerTurnEnded;
        public Action OnStreetDealt;
        public Action OnShowdownEnded;

        //public List<Player> players;


        //private NeatSupervisor neatSupervisor;
        [SerializeField]
        public GameObject prefabPlayer;

        private Game _currentGame;

        public Game CurrentGame
        {
            get { return _currentGame; }
            set { _currentGame = value; }
        }

        public TableFillerController tableFillerController;

        private ChipDenominations[] _chipDenominations;
        private int _currentPlayerIndex = -1;
        public int CurrentPlayerIndex
        {
            get { return _currentPlayerIndex; }
            set { _currentPlayerIndex = value; }
        }

        private ActivePlayerState _activePlayerState;

        public ActivePlayerState ActivePlayerState
        {
            get { return _activePlayerState; }
            set { _activePlayerState = value; }
        }

        private CurrentHandState _currentHandState;

        public CurrentHandState CurrentHandState
        {
            get { return _currentHandState; }
            set { _currentHandState = value; }
        }

        private HandChecker _handChecker;

        public HandChecker HandChecker
        {
            get { return _handChecker; }
            set { _handChecker = value; }
        }


        //public float _handCount;
        /*
        public float HandCounter
        {
            get { return _handCount; }
            set { _handCount = value; }
        }
        */
        private int _currentAmountToBet;
        public int CurrentAmoutToBet {
            get { return _currentAmountToBet; }
            set { _currentAmountToBet = value; } }

        public bool IsFlopDealt { get; set; }
        public bool IsTurnDealt { get; set; }
        public bool IsRiverDealt { get; set; }

        private PlayerWidget _localPlayerWidget;

        private bool _isGameInProgress = true;
        public bool GameRunning
        {
            get { return _isGameInProgress; }
            set { _isGameInProgress = value; }
        }
        private bool _isAITurnDelayInProgress;

        private bool reset;
        //private bool _isANNInProgress = true;

        //public bool ANNInProgress
        //{
        //    get { return _isANNInProgress; }
        //    set { _isANNInProgress = value; }
        //}

        private string sceneName;
               

        public string SceneName
        {
            get { return sceneName; }
            set { sceneName = value; }
        }

        public bool preparationMade = false;
        public bool PreparationsMade
        {
            get { return preparationMade; }
            set { preparationMade = value; }
        }

        public bool debug = true;

        private bool canPlayHand = false;
        public bool CanPlayHand
        {
            get { return canPlayHand; }
            set { canPlayHand = value; }
        }

        private bool canPlayNextTurn = false;
        public bool CanPlayNextTurn
        {
            get { return canPlayNextTurn; }
            set { canPlayNextTurn = value; }
        }


        private int _lastTrail = 0;
        public int LastTrial
        {
            get { return _lastTrail; }
            set { _lastTrail = value; }
        }

        private int _currentTrail = 0;
        public int CurrentTrial
        {
            get { return _currentTrail;  }
            set {  _currentTrail = value  ; }
        }

        private int _lastGeneration = 0;
        public int LastGeneration
        {
            get { return _lastGeneration; }
            set { _lastGeneration = value; }
        }

        private int _currentGeneration = 0;
        public int CurrentGeneration
        {
            get { return _currentGeneration; }
            set { _currentGeneration = value; }
        }

        void Start()
        {
            
            sceneName = SceneManager.GetActiveScene().name;
            //printLog("Executou: " + this.name);
            //GameObject.Find("Neat Supervisor").GetComponent<NeatSupervisor>().game = this;
            preparationMade = false;
            
            //debug = true;
            //canPlayHand = false;
            //neatSupervisor.CanStartNewHand = true;
            tableFillerController = GameObject.Find("TableFiller").GetComponent<TableFillerController>();
           // preparationMade = true;
            /*
            if (tableFillerController != null)
            {
                printLog("table filler não esta nulo");
            }
            */
            //PrepareGame();
            //_isGameInProgress = false;

           // players = new List<Player>(gameObject.FindComponentsInChildrenWithTag<Player>("player"));
        }

        void Update()
        {

            //if (!_isGameInProgress) {

            //    if (_localPlayerWidget != null) {

            //        if (!_localPlayerWidget.IsSeatEmpty) {
            //            printLog("Agora Começou");
            //           

            //            StartGame();

            //        }
            //    }
            //}


            if (CanPlayNextTurn)
            {
                 printLog(" --------------------------------------------------------------------Executou proximo Turno  "+ name);
                CanPlayNextTurn = false;
                // neatSupervisor.CanStartNewHand = true;
                CurrentGame.checkNextTurn();
            }

            // if (/* canPlayHand */GameMenager.Instance.getIfCanPlayHand()  /*&& canStartNewHand()*/)
            //printLog("Testou hands" + GameMenager.Instance.NeatTrialInprogress);
            //printLog(preparationMade);
            if (!_isGameInProgress)
            {
                _isGameInProgress = true;

                StartGame();
            }

            if (sceneName == "NEAT") {
                if ( tableFillerController.waitOtherTabbles() /*canPlayHand*/ && transform.childCount == 8 && neatSupervisor.GenerationEnded )
                {
                    //printLog("Destruiu: "+ name);
                   // neatSupervisor.CanStartNewHand = true;
                    tableFillerController.setCurrentTables(tableFillerController.getCurrentTables() - 1);
                    tableFillerController.getTables().Remove(GetComponent<SeatController>());
                    Destroy(gameObject);
                }
            }



            if (sceneName == "TESTS")
            {
                if (preparationMade)
                {
                    //printLog( " fora loop "+ GameMenager.Instance.getIfTrialEnded("fora loop"));

                    //printLog( name + " Can start  " + canStartNewHand() + " can play " + canPlayHand + " Trial ended "+ GameMenager.Instance.getIfTrialEnded() );
                    if (CanPlayHand && CurrentGame.HandCount <= 1000 /* GameMenager.Instance.AllUnitsAreActtive  &&  canPlayHand*/ /* &&  GameMenager.Instance.getIfTrialEnded()*/)
                    {
                        //neatSupervisor.SetTempoTrial(false);
                        // GameMenager.Instance.setTrialEnded(name, true);
                        printLog("Executou Mao nova " + name);
                        CurrentGame.PlayHand();
                    }

                }
            }

        }
        
        public bool checkIfGenerartionIsSame()
        {
            CurrentGeneration = GameMenager.Instance.CurrentGeneration;  ;//neatSupervisor.CurrentTrial;

            if (CurrentGeneration == LastGeneration)
            {
                return true;
            }
            else
            {
                //GameMenager.Instance.setTrialEnded(name, true);
                //neatSupervisor.SetTempoTrial(true);
                LastGeneration = CurrentGeneration;
            }
            return false;

        }
        
        public bool IsAllPlayerActive()
        {
            //printLog("Testou se player ativos"+ name );
            foreach (Player player in CurrentGame.Players)
            {
                ///printLog(player.Name +"  "+ player.IsActive );
                if (!player.IsActive)
                {
                    return false;
                }
            }
            //printLog("Todos  players ativos" + name);
            return true;
        }
        
        public bool canStartNewHand()
        {
            // if ( CurrentGame != null ) {
            if (!IsAllPlayerActive())
            {
                return false;
            }
            //}
            // printLog(" can play next hand "+GameMenager.Instance.CanPlayNextHand);
            return canPlayHand ;//  && !checkIfGenerartionIsSame() ;
        }

        public void printLog(string message)
        {
            if (debug)
            {
                Debug.Log(message);
            }
        }
        
        public void PrepareGame()
        {
            _chipDenominations = (ChipDenominations[])Enum.GetValues(typeof(ChipDenominations));

            // test
            GameStartingInfo gameStartingInfo;
            gameStartingInfo.BigBlindSize = 10;
            gameStartingInfo.AnteSize = 0;
            gameStartingInfo.StartingChips = 1500;
            gameStartingInfo.PlayerCount = 6;

            _currentAmountToBet = gameStartingInfo.BigBlindSize;
            // printLog( "Inicializou em o qtdPraAposta: "+ _currentAmountToBet);

            GameObject obj = new GameObject();

            obj.transform.SetParent(this.transform);
            ActivePlayerState =  obj.AddComponent<ActivePlayerState>();

            CurrentHandState =  obj.AddComponent <CurrentHandState>();

            HandChecker = obj.AddComponent <HandChecker>();
            
            
            //new ActivePlayerState();
            //new CurrentHandState();
            //new HandChecker();
            //new ANNInprogress();
            //new PlayerAI();

            ActivePlayerState.GameController = this;
            HandChecker.GameController = this;
            
            //ActivePlayerState.Instance.GameController = this;
            //HandChecker.Instance.GameController = this;
            
            // TODO: initialize this earlier
            //new JSONReader();
            //JSONReader.Instance.PrepareConfig();

            _currentGame = new RingGame(gameStartingInfo);

            

            _currentGame.GameController = this;

            _currentGame.PrepareToStart();
           
            //PlayerAI.Instance.CurrentGame = _currentGame;
        }

        public void StartGame()
        {
            if (_currentGame != null) {

                //printLog("Começou");

                _currentGame.Start();
            }
        }

       

        public List<Player> GetNeatInstances()
        {
            //GameObject gameObject = GameObject.Find("Table1");

            //printLog("Name: " +this.name);
            List<Player> list = new List<Player>(gameObject.FindComponentsInChildrenWithTag<Player>("player"));

            //printLog( "Qtd total jogadores :"+ list.Count);


            return list;

            //return players;
        }

        public void CreatePlayer( Player newPlayer ,  Vector3 position, int playerIndex)
        {
           
            //PlayerWidget playerWidget;
            //playerWidget = newPlayer.GetComponent<PlayerWidget>();
            //playerWidget.PlayerLink = newPlayer;
            //PlayerAI playerAI = playerObject.GetComponent<PlayerAI>();
            //newPlayer.AI = playerAI;

            newPlayer.name = "Player" + playerIndex;
            //Transform stackSizeTransform = newPlayer.transform.Find("PlayerCard/StackSizeText");
            //stackSizeTransform.gameObject.GetComponent<Text>().text = newPlayer.ChipCount.ToString();
            //Transform playerNameTransform = newPlayer.transform.Find("PlayerCard/PlayerNameText");
            //playerNameTransform.gameObject.GetComponent<Text>().text = newPlayer.Name.ToString();
            
            newPlayer.ROI = 0;

            //FindSeatController findSeatController = newPlayer.GetComponent<FindSeatController>();
            //findSeatController.setTbc(GameObject.Find("TableFiller").GetComponent<TableFillerController>());
            //findSeatController.seat(findSeatController.getTbc().getTable());
            //printLog(newPlayer.Name);
        }

        /*
        public void CreatePlayer(Player newPlayer, Vector3 position ,  int playerIndex)
        {
            GameObject player;
            PlayerWidget playerWidget;

            if (SceneName == "NEAT")
            {
                player = GameObject.Instantiate(prefabPlayer);
                player.transform.SetParent(GameObject.Find("MainBoard").transform);
                player.transform.position = position;
                //printLog("Instanciou agente neat");
            }
            else
            {
                player = PlayersParent.transform.Find("Player" + playerIndex).gameObject;
            }
            

            playerWidget = player.GetComponent<PlayerWidget>();
            playerWidget.PlayerLink = newPlayer;
           // PlayerAI playerAI = playerObject.GetComponent<PlayerAI>();
           // newPlayer.AI = playerAI;
            player.name = "Player" + playerIndex;
            Transform stackSizeTransform = player.transform.Find("PlayerCard/StackSizeText");
            stackSizeTransform.gameObject.GetComponent<Text>().text = newPlayer.ChipCount.ToString();
            Transform playerNameTransform = player.transform.Find("PlayerCard/PlayerNameText");
            playerNameTransform.gameObject.GetComponent<Text>().text = newPlayer.Name.ToString();

            newPlayer.Wins = 0;
    
            FindSeatController findSeatController = player.GetComponent<FindSeatController>();
            findSeatController.setTbc(GameObject.Find("TableFiller").GetComponent<TableFillerController>());
            findSeatController.seat(findSeatController.getTbc().getTable());
            //if (newPlayer.Type == PlayerType.Local) {
            //    _localPlayerWidget = playerWidget;
            //}


            //if (playerIndex == 0)
            //{
            //    _localPlayerWidget = playerWidget;
            //}
            // printLog("Criou jogador " + newPlayer.Name +"Tipo: "+ newPlayer.Type);
        }
        */
       
        public void DealCards(Dictionary<int, HoleCards> cardsToDeal)
        {
            if (cardsToDeal == null) {
                Debug.LogError("GameController.DealCards: cardsToDeal is null!");
                return;
            }

            if (sceneName != "NEAT"  &&  sceneName != "TESTS")
            {
                StartCoroutine(DealCardsCoroutine(cardsToDeal));
            }
            else
            {
                OnCardsDealt?.Invoke();
            }

        }

        // TODO: create animation controller class and move this method there
        private IEnumerator DealCardsCoroutine(Dictionary<int, HoleCards> cardsToDeal)
        {
            int localPlayerIndex = _currentGame.GetLocalPlayerIndex();

            if (localPlayerIndex < 0)
            {
                Debug.LogError("GameController.DealCardsCoroutine: could not get local player index!");
                yield break;
            }

            // TODO: this is only for 2 hole cards case
            for (int cardIndex = 0; cardIndex < Defines.HOLE_CARDS_COUNT; cardIndex++) {
                foreach (KeyValuePair<int, HoleCards> cards in cardsToDeal) {
                    int playerIndex = cards.Key;
                    Card cardToDeal = cardIndex == 0 ? cards.Value.First : cards.Value.Second;
                    bool isLocalPlayer = false;// (playerIndex == localPlayerIndex);

                    // TODO: check if deck object is active
                    int dealerIndex = _currentGame.GetPlayerOnTheButtonIndex();
                    Transform dealerTransform = PlayersParent.transform.Find("Player" + dealerIndex);
                    Transform deckTransform = dealerTransform.Find("Deck");

                    Transform playerTransform = PlayersParent.transform.Find("Player" + playerIndex);
                    Transform holeCardsTransform = playerTransform.Find("HoleCards");

                    GameObject cardToDealObject = GameObject.Instantiate(isLocalPlayer ? CardPrefab : OpponentCardPrefab);
                    CardWidget cardToDealWidget = cardToDealObject.GetComponent<CardWidget>();
                    cardToDealWidget.CardLink = cardToDeal;

                    //cardToDealObject.transform.parent = deckTransform;

                    cardToDealObject.transform.SetParent(deckTransform,false);

                    Vector3 startingCardPosition = deckTransform.position;
                    Vector3 finalCardPosition = holeCardsTransform.position;

                    Vector3 startingCardScale = new Vector3(0.1f, 0.1f, 0.1f);
                    Vector3 finalCardScale = Vector3.one;

                    float cardDealingTime = 0;

                    while (cardDealingTime < Defines.CARD_DEALING_TIME) {
                        cardDealingTime += Time.deltaTime;
                        float timeSpentPercentage = cardDealingTime / Defines.CARD_DEALING_TIME;
                        cardToDealObject.transform.position = Vector3.Lerp(startingCardPosition, finalCardPosition, timeSpentPercentage);
                        cardToDealObject.transform.localScale = Vector3.Lerp(startingCardScale, finalCardScale, timeSpentPercentage);
                        yield return null;
                    }

                    //cardToDealObject.transform.parent = holeCardsTransform;
                   cardToDealObject.transform.SetParent(holeCardsTransform,false);

                }
            }

            //if (OnCardsDealt != null) {
            //    OnCardsDealt();
            //}
            OnCardsDealt?.Invoke();
        }

        private TurnTimer GetCurrentPlayerTurnTimer()
        {
            Transform currentPlayerTransform = PlayersParent.transform.Find("Player" + CurrentPlayerIndex);
            if (currentPlayerTransform == null) {
                return null;
            }

            Transform timerTransform = currentPlayerTransform.Find("TurnTimer");
            if (timerTransform == null) {
                return null;
            }

            TurnTimer timer = timerTransform.gameObject.GetComponent<TurnTimer>();
            return timer;
        }

        public void StartTurnTimer()
        {
            TurnTimer timer = GetCurrentPlayerTurnTimer();
            if (timer != null) {
                timer.StartTimer();
            } else {
                Debug.LogError("GameController.StartTurnTimer: timer is null!");
            }
        }

        public void StopTurnTimer()
        {
            TurnTimer timer = GetCurrentPlayerTurnTimer();
            if (timer != null) {
                timer.StopTimer();
            } else {
                Debug.LogError("GameController.StopTurnTimer: timer is null!");
            }
        }

        public void EndPlayerTurnOnTimeout()
        {
            int uncalledBet = _currentGame.CurrentHand.GetHighestBetNotInPot() - _currentGame.CurrentPlayer.CurrentBet;
            bool canCheck = uncalledBet == 0;

            if (canCheck) {
                DisplayPlayerActionText(CurrentPlayerIndex, TurnType.Check);
                OnPlayerTurnEnded(TurnType.Check, 0);
            } else {
                DisplayPlayerActionText(CurrentPlayerIndex, TurnType.Fold);
                OnPlayerTurnEnded(TurnType.Fold, 0);
            }
        }

        public void DealFlop(List<Card> flopCards)
        {
            if (flopCards.Count != Defines.FLOP_CARDS_COUNT)
            {
                Debug.LogError("GameController.DealFlop: invalid amount of flop cards to deal.");
                return;
            }
            printLog("Executando Flop");


            //StartCoroutine(DealFlopCoroutine(flopCards));
            IsFlopDealt = true;
            OnStreetDealt?.Invoke();

        }

        private IEnumerator DealFlopCoroutine(List<Card> flopCards)
        {
           if (SceneName != "NEAT") {
                List<Card> cachedFlopCards = new List<Card>();
                cachedFlopCards.AddRange(flopCards);

                int flopCardIndex = 0;

                yield return new WaitForSeconds(Defines.DELAY_BETWEEN_DEALING_STREETS);

                foreach (Card flopCard in cachedFlopCards)
                {
                    DealBoardCard(flopCardIndex, flopCard);
                    flopCardIndex++;
                    yield return new WaitForSeconds(Defines.DELAY_BETWEEN_DEALING_CARDS);
                }
            }
           

            IsFlopDealt = true;

            //if (OnStreetDealt != null)
            //{
            //    OnStreetDealt();
            //}

            OnStreetDealt?.Invoke();
            yield return null;
        }

        public void DealTurn(Card turnCard)
        {
            
            //StartCoroutine(DealTurnCoroutine(turnCard));
            while (!IsFlopDealt)
            {
                return;
            }
            IsTurnDealt = true;

            OnStreetDealt?.Invoke();
        }

        private IEnumerator DealTurnCoroutine(Card turnCard)
        {
            Card cachedTurnCard = turnCard;
            int turnCardIndex = Defines.FLOP_CARDS_COUNT;

            while (!IsFlopDealt) {
                yield return null;
            }
            if (sceneName != "NEAT"  &&  sceneName != "TESTS")
            {
                yield return new WaitForSeconds(Defines.DELAY_BETWEEN_DEALING_STREETS);

                DealBoardCard(turnCardIndex, cachedTurnCard);
            }
            
            IsTurnDealt = true;

            //if (OnStreetDealt != null) {
            //    OnStreetDealt();
            //}

            OnStreetDealt?.Invoke();
           // IsRiverDealt = true;
            yield return null;
        }

        public void DealRiver(Card riverCard)
        {
            //StartCoroutine(DealRiverCoroutine(riverCard));
            while (!IsTurnDealt)
            {
                return;
            }
        
            IsRiverDealt = true;
            OnStreetDealt?.Invoke();
        }

        private IEnumerator DealRiverCoroutine(Card riverCard)
        {
            Card cachedRiverCard = riverCard;
            int riverCardIndex = Defines.CARDS_COUNT_ON_TURN;

            while (!IsTurnDealt) {
                yield return null;
            }
            if (SceneName != "NEAT")
            {
                yield return new WaitForSeconds(Defines.DELAY_BETWEEN_DEALING_STREETS);

                DealBoardCard(riverCardIndex, cachedRiverCard);

                // TODO: move this somewhere else (it's to prevent immediate showdown when all the players are all in before river)
                yield return new WaitForSeconds(Defines.DELAY_BETWEEN_DEALING_STREETS);
            }
            
            IsRiverDealt = true;

            //if (OnStreetDealt != null) {
            //    OnStreetDealt();
            //}

            OnStreetDealt?.Invoke();
            yield return null;
        }

        public void DealBoardCard(int cardIndex, Card card)
        {
            GameObject cardObject = GameObject.Instantiate(CardPrefab);
            CardWidget cardWidget = cardObject.GetComponent<CardWidget>();
            cardWidget.CardLink = card;

            Vector3 cardPosition = Board.transform.position;
            cardPosition.x += cardWidget.ImageWidth * cardIndex;

            //cardObject.transform.parent = Board.transform;
            cardObject.transform.SetParent(Board.transform,false);
            cardObject.transform.localPosition = cardPosition;
            cardObject.transform.localScale = Vector3.one;
        }

        public void PostBlinds(int smallBlindIndex, int bigBlindIndex, int bigBlindSize)
        {
            if (sceneName != "NEAT"  &&  sceneName != "TESTS")
            {
                
                StartCoroutine(PostBlindsCoroutine(smallBlindIndex, bigBlindIndex, bigBlindSize));
            }
            else
            {
                OnBlindsPosted?.Invoke();
            }
        }

        private IEnumerator PostBlindsCoroutine(int smallBlindIndex, int bigBlindIndex, int bigBlindSize)
        {
            yield return new WaitForSeconds(Defines.DELAY_IN_BLIND_POSTING);

            PostPlayerBlind(smallBlindIndex, bigBlindSize / 2);

            yield return new WaitForSeconds(Defines.DELAY_IN_BLIND_POSTING);

            PostPlayerBlind(bigBlindIndex, bigBlindSize);

            yield return new WaitForSeconds(Defines.DELAY_IN_BLIND_POSTING);

            //if (OnBlindsPosted != null) {
            //    OnBlindsPosted();
            //}

            OnBlindsPosted?.Invoke();
        }

        private void PostPlayerBlind(int playerIndex, int blindSize)
        {
            Transform playerTransform = PlayersParent.transform.Find("Player" + playerIndex);
            Transform blindsTransform = playerTransform.Find("Blinds");
            PlayerWidget playerWidget = playerTransform.gameObject.GetComponent<PlayerWidget>();

            if (playerWidget == null) {
                Debug.LogError("GameController.PostPlayerBlind: PlayerWidget is null. Player index = " + playerIndex);
                return;
            }

            ChipAlignment chipAlignment = playerWidget.ChipAlignment;
            PutChipsInfrontPlayer(blindsTransform, blindSize, chipAlignment);
        }

        public void PostPlayerAnte(int playerIndex, int anteSize)
        {

        }

        public void GiveButtonToPlayer(int playerIndex)
        {
            Transform playerTransform = PlayersParent.transform.Find("Player" + playerIndex);
            Transform buttonTransform = playerTransform.Find("Button");
            GameObject buttonObject = GameObject.Instantiate(ButtonPrefab);

            //buttonObject.transform.parent = buttonTransform;

            buttonObject.transform.SetParent(buttonTransform,false);
            buttonObject.transform.localPosition = buttonTransform.position;
            buttonObject.transform.localScale = Vector3.one;
        }

        public void GiveDeckToPlayer(int playerIndex)
        {
            Transform playerTransform = PlayersParent.transform.Find("Player" + playerIndex);
            Transform deckTransform = playerTransform.Find("Deck");
            GameObject deckObject = GameObject.Instantiate(DeckPrefab);

            //deckObject.transform.parent = deckTransform;
            deckObject.transform.SetParent(deckTransform,false);
            deckObject.transform.localPosition = deckTransform.position;
            deckObject.transform.localScale = Vector3.one;
        }

        public void UpdatePlayerCard(int playerIndex, Player player)
        {
            // TODO: cache this transform to reduce Find calls
            Transform playerTransform = PlayersParent.transform.Find("Player" + playerIndex);

            Transform stackSizeTransform = playerTransform.Find("PlayerCard/StackSizeText");
            stackSizeTransform.gameObject.GetComponent<Text>().text = player.IsAllIn ? "All-in!" : player.ChipCount.ToString();
        }

        public void HighlightActivePlayer(bool highlight)
        {
            Transform currentPlayerTransform = PlayersParent.transform.Find("Player" + CurrentPlayerIndex);

            if (currentPlayerTransform == null) {
                return;
            }

            Transform playerAvatarFrame = currentPlayerTransform.Find("Avatar/AvatarFrame");

            if (playerAvatarFrame == null) {
                return;
            }

            Image playerAvatarFrameImage = playerAvatarFrame.GetComponent<Image>();

            if (playerAvatarFrameImage == null) {
                return;
            }

            playerAvatarFrameImage.color = highlight ? Color.green : Color.white;
        }

        public void AddPot()
        {
            for (int potIndex = 0; potIndex < PotsParent.transform.childCount; potIndex++) {
                Transform potTransform = PotsParent.transform.Find("Pot" + potIndex);
                if (potTransform == null) {
                    Debug.LogError("PutChipsIntoPot: could not find pot transform, pot index = " + potIndex);
                    break;
                }

                if (!potTransform.gameObject.activeSelf) {
                    potTransform.gameObject.SetActive(true);
                    break;
                }
            }
        }

        public void PutChipsIntoPot()
        {
            bool areThereChipsNotInPot = false;

            for (int playerIndex = 0; playerIndex < _currentGame.Players.Count; playerIndex++) {
                Player player = _currentGame.Players[playerIndex];
                Transform playerTransform = PlayersParent.transform.Find("Player" + playerIndex);
                Transform blindsTransform = playerTransform.Find("Blinds");

                for (int chipIndex = blindsTransform.childCount - 1; chipIndex >= 0; chipIndex--) {
                    if (!areThereChipsNotInPot) {
                        areThereChipsNotInPot = true;
                    }
                    Transform childTransform = blindsTransform.GetChild(chipIndex);
                    GameObject.Destroy(childTransform.gameObject);
                }

                blindsTransform.DetachChildren();
            }

            if (!areThereChipsNotInPot) {
                return;
            }

            int potIndex = 0;
            // TODO: reset chipPosition.y between pots
            foreach (Pot pot in _currentGame.CurrentHand.Pots) {
                Transform potTransform = PotsParent.transform.Find("Pot" + potIndex);
                if (potTransform == null) {
                    Debug.LogError("PutChipsIntoPot: could not find pot transform, pot index = " + potIndex);
                    break;
                }

                int potSize = pot.Size;

                for (int chipDenominationIndex = _chipDenominations.Length - 1; chipDenominationIndex >= 0; chipDenominationIndex--) {
                    int denomination = (int)_chipDenominations[chipDenominationIndex];
                    if (denomination > potSize) {
                        continue;
                    }

                    while (potSize >= denomination) {
                        GameObject chipObject = GameObject.Instantiate(ChipPrefab);
                        ChipWidget chipWidget = chipObject.GetComponent<ChipWidget>();
                        chipWidget.Denomination = _chipDenominations[chipDenominationIndex];

                        Vector3 chipPosition = potTransform.position;

                        // one child is pot size text
                        int chipsCount = potTransform.childCount - 1;
                        if (chipsCount > 0) {
                            int chipsInStack = chipsCount;
                            int stackIndex = 0;
                            if (chipsCount >= Defines.MAX_CHIPS_IN_ONE_STACK_IN_BETS) {
                                stackIndex = chipsCount / Defines.MAX_CHIPS_IN_ONE_STACK_IN_BETS;
                                chipsInStack = chipsCount % Defines.MAX_CHIPS_IN_ONE_STACK_IN_BETS;
                            }
                            // TODO: correct this magic
                            if (stackIndex > 0) {
                                chipPosition.x += (chipWidget.ImageWidth * stackIndex);
                            }
                            chipPosition.y += (chipWidget.ImageHeight * chipsInStack) / 6;
                        }
                        //chipObject.transform.parent = potTransform;
                        chipObject.transform.SetParent(potTransform,false);

                        chipObject.transform.localPosition = chipPosition;
                        chipObject.transform.localScale = Vector3.one;

                        potSize -= denomination;
                    }
                }

                if (potSize > 0) {
                    Debug.LogError("PutChipsIntoPot: wrong bet amount (could not be presented with current chip denominations).");
                }

                potIndex++;
            }

                // old variant with all the players chips in pot unchanged
                //if (player.CurrentBet > 0) {
                //    Transform playerTransform = PlayersParent.transform.Find("Player" + playerIndex);
                //    Transform blindsTransform = playerTransform.Find("Blinds");

                //    for (int chipIndex = blindsTransform.childCount - 1; chipIndex >= 0; chipIndex--) {
                //        Transform childTransform = blindsTransform.GetChild(chipIndex);
                //        GameObject chipObject = childTransform.gameObject;
                //        ChipWidget chipWidget = chipObject.GetComponent<ChipWidget>();
                //        Vector3 chipPosition = Pot.transform.position;
                //        int chipsCountInPot = Pot.transform.childCount;

                //        if (chipsCountInPot > 0) {
                //            int chipsInStack = chipsCountInPot;
                //            int stackColumn = 0, stackRow = 0;
                //            if (chipsCountInPot >= Defines.MAX_CHIPS_IN_ONE_STACK_IN_BETS) {
                //                stackColumn = (chipsCountInPot / Defines.MAX_CHIPS_IN_ONE_STACK_IN_BETS) % Defines.MAX_STACKS_IN_ONE_ROW_IN_BETS;
                //                stackRow = (chipsCountInPot / Defines.MAX_CHIPS_IN_ONE_STACK_IN_BETS) / Defines.MAX_STACKS_IN_ONE_ROW_IN_BETS;
                //                chipsInStack = chipsCountInPot % Defines.MAX_CHIPS_IN_ONE_STACK_IN_BETS;
                //            }
                //            // TODO: correct this magic
                //            if (stackColumn > 0) {
                //                chipPosition.x += (chipWidget.ImageWidth * stackColumn);
                //            }
                //            if (stackRow > 0) {
                //                chipPosition.y -= (chipWidget.ImageHeight * stackRow);
                //            }
                //            chipPosition.y += (chipWidget.ImageHeight * chipsInStack) / 6;
                //        }

                //        childTransform.parent = Pot.transform;
                //        childTransform.localPosition = chipPosition;
                //    }

                //    blindsTransform.DetachChildren();
                //}
            //}

            _currentAmountToBet = 0;
        }

        public void ClearPlayerActions()
        {
            for (int playerIndex = 0; playerIndex < _currentGame.Players.Count; playerIndex++) {
                Transform playerTransform = PlayersParent.transform.Find("Player" + playerIndex);
                Transform playerCard = playerTransform.Find("PlayerCard");
                if (playerCard != null) {
                    Transform playerActionTransform = playerTransform.Find("PlayerCard/LastActionText");
                    if (playerActionTransform != null) {
                        Text playerActionText = playerActionTransform.gameObject.GetComponent<Text>();
                        if (playerActionText != null) {
                            playerActionText.text = "";
                        }
                    }
                }
            }
        }

        public void ProcessShowdown(List<int> playerIndexes)
        {
            if (sceneName != "NEAT"  &&  sceneName != "TESTS")
            {
                StartCoroutine(ShowPlayersCards(playerIndexes));
            }
            else
            {
                OnShowdownEnded?.Invoke();
            }

        }

        private IEnumerator ShowPlayersCards(List<int> playerIndexes)
        {
            // TODO: check the order of players
            foreach (int playerIndex in playerIndexes)
            {
                Transform playerTransform = PlayersParent.transform.Find("Player" + playerIndex);
                Transform holeCardsTransform = playerTransform.Find("HoleCards");

                for (int cardIndex = holeCardsTransform.childCount - 1; cardIndex >= 0; cardIndex--)
                {
                    Transform childTransform = holeCardsTransform.GetChild(cardIndex);
                    CardWidget cardWidget = childTransform.gameObject.GetComponent<CardWidget>();

                    if (cardWidget != null)
                    {
                        // TODO: move to separate method
                        Card card = cardWidget.CardLink;
                        card.IsOpened = true;
                        cardWidget.CardLink = card;
                    }
                }

                yield return new WaitForSeconds(Defines.SHOW_CARDS_TIME);
            }

            //if (OnShowdownEnded != null){
            //   OnShowdownEnded();
            //}
            OnShowdownEnded?.Invoke();
        }

        public void EndHand()
        {
            //ANNInprogress.Instance.setTableProgress("Table1",true);
            //ClearPlayers();
            ClearBoard();
            ResetBetSliderAndInput();
        }

        public void UpdatePotSizeTexts(List<Pot> pots)
        {
            int potIndex = 0;
            foreach (Pot pot in pots) {
                Transform potTransform = PotsParent.transform.Find("Pot" + potIndex + "/PotSizeText");
                if (potTransform == null) {
                    Debug.LogError("UpdatePotSizeTexts: could not find pot transform, pot index = " + potIndex);
                    break;
                }

                Text potSizeText = potTransform.gameObject.GetComponent<Text>();
                if (potSizeText == null) {
                    Debug.LogError("UpdatePotSizeTexts: could not find pot size text, pot index = " + potIndex);
                    break;
                }

                string potSizeString = (potIndex == 0) ? "Main pot: " : "Side pot: ";
                potSizeString += pot.Size.ToString();

                potSizeText.text = potSizeString;

                potIndex++;
            }
        }

        public void HideAllButtons()
        {
            ShowFoldButton(false);
            ShowCheckButton(false);
            ShowCallButton(false);
            ShowBetButton(false);
            ShowRaiseButton(false);
            ShowBetSliderAndBetAmountInputField(false);
        }

        public void ShowFoldButton(bool show)
        {
            if (FoldButton != null) {
                FoldButton.gameObject.SetActive(show);
            }
        }

        public void ShowCheckButton(bool show)
        {
            if (CheckButton != null) {
                CheckButton.gameObject.SetActive(show);
            }
        }

        public void ShowCallButton(bool show)
        {
            if (CallButton != null) {
                CallButton.gameObject.SetActive(show);
            }
        }

        public void SetCallButtonText(string text)
        {
            if (CallButton != null) {
                Text CallButtonText = CallButton.GetComponentInChildren<Text>();
                CallButtonText.text = "Call " + text;
            }
        }

        public void ShowBetButton(bool show)
        {
            if (BetButton != null) {
                BetButton.gameObject.SetActive(show);
            }
        }

        public void SetBetButtonText(string text)
        {
            if (BetButton != null) {
                Text BetButtonText = BetButton.GetComponentInChildren<Text>();
                if (BetButtonText != null) {
                    BetButtonText.text = "Bet " + text;
                }
            }
        }

        public void ShowRaiseButton(bool show)
        {
            if (RaiseButton != null) {
                RaiseButton.gameObject.SetActive(show);
            }
        }

        public void SetRaiseButtonText(string text)
        {
            if (RaiseButton != null) {
                Text RaiseButtonText = RaiseButton.GetComponentInChildren<Text>();
                if (RaiseButtonText != null) {
                    RaiseButtonText.text = "Raise " + text;
                }
            }
        }

        public void ShowBetSliderAndBetAmountInputField(bool show)
        {
            if (BetSlider != null) {
                BetSlider.gameObject.SetActive(show);
            }

            if (BetInputField != null) {
                BetInputField.gameObject.SetActive(show);
            }
        }

        public void OnBackClicked()
        {
            SceneManager.LoadScene("Main");
        }

        public void OnFoldClicked()
        {
            if (CurrentPlayerIndex < 0) {
                Debug.LogError("OnFoldClicked: invalid current player index.");
                return;
            }

           // DisplayPlayerActionText(CurrentPlayerIndex, TurnType.Fold);

            if (sceneName != "NEAT"  &&  sceneName != "TESTS")
            {
                StartCoroutine(FoldAnimation());
            }
            else
            {

                OnPlayerTurnEnded?.Invoke(TurnType.Fold, 0);
            }
        }

        private IEnumerator FoldAnimation()
        {
            int dealerIndex = _currentGame.GetPlayerOnTheButtonIndex();
            Transform dealerTransform = PlayersParent.transform.Find("Player" + dealerIndex);
            Transform deckTransform = dealerTransform.Find("Deck");

            Transform playerTransform = PlayersParent.transform.Find("Player" + CurrentPlayerIndex);
            Transform holeCardsTransform = playerTransform.Find("HoleCards");
            Transform cardTransform;

            Vector3 startingCardPosition = holeCardsTransform.position;
            Vector3 finalCardPosition = deckTransform.position;

            Vector3 startingCardScale = Vector3.one;
            Vector3 finalCardScale = new Vector3(0.1f, 0.1f, 0.1f);

            for (int cardIndex = holeCardsTransform.childCount - 1; cardIndex >= 0; cardIndex--) {
                cardTransform = holeCardsTransform.GetChild(cardIndex);

                float cardDealingTime = 0;


                while (cardDealingTime < Defines.CARD_DEALING_TIME)
                {
                    cardDealingTime += Time.deltaTime;
                    float timeSpentPercentage = cardDealingTime / Defines.CARD_DEALING_TIME;


                    if (cardTransform != null)
                    {
                       // printLog("Card transform do " + _currentGame.CurrentPlayer.Name + " esta nulo ");


                        cardTransform.position = Vector3.Lerp(startingCardPosition, finalCardPosition, timeSpentPercentage);
                        cardTransform.localScale = Vector3.Lerp(startingCardScale, finalCardScale, timeSpentPercentage);
                    }
                        yield return null;
                }

                if (cardTransform != null)
                {
                    GameObject.Destroy(cardTransform.gameObject);
                }
            }

            holeCardsTransform.DetachChildren();

            //if (OnPlayerTurnEnded != null) {
            //    OnPlayerTurnEnded(TurnType.Fold, 0);
            //}

            OnPlayerTurnEnded?.Invoke(TurnType.Fold, 0);
        }

        public void OnCheckClicked()
        {
            if (sceneName != "NEAT"  &&  sceneName != "TESTS")
            {
                DisplayPlayerActionText(CurrentPlayerIndex, TurnType.Check);
            }
            if (OnPlayerTurnEnded != null)
            {
                OnPlayerTurnEnded(TurnType.Check, 0);
            }
        }

        public void OnCallClicked()
        {


            //Transform playerTransform = PlayersParent.transform.Find("Player" + CurrentPlayerIndex);
            //Transform blindsTransform = playerTransform.Find("Blinds");

            //PlayerWidget playerWidget = playerTransform.gameObject.GetComponent<PlayerWidget>();

            //if (playerWidget == null)
            //{
            //    Debug.LogError("GameController.OnCallClicked: PlayerWidget is null. Player index = " + CurrentPlayerIndex);
            //    return;
            //}

            //ChipAlignment chipAlignment = playerWidget.ChipAlignment;


            Hand currentHand = _currentGame.CurrentHand;
            Player currentPlayer = _currentGame.CurrentPlayer;

            printLog(" maior aposta nao no pot:  " + currentHand.GetHighestBetNotInPot() + " Aposta Atual:  " + currentPlayer.CurrentBet);
            //int amountToCall = Math.Min((currentHand.GetHighestBetNotInPot() + CurrentGame.BigBlindSize) - currentPlayer.CurrentBet, currentPlayer.ChipCount);
            int amountToCall = Math.Min((currentHand.GetHighestBetNotInPot()) - currentPlayer.CurrentBet, currentPlayer.ChipCount);
            if (currentHand.GetHighestBetNotInPot() <= 0)
            {
                amountToCall = CurrentGame.BigBlindSize;
            }

            printLog("Jogador "+currentPlayer.Name + " Call: " + amountToCall);
           

            //printLog("QTD que esta dando Call: "+ amountToCall );
            //if (sceneName != "NEAT"  &&  sceneName != "TESTS")
            //{
            //    PutChipsInfrontPlayer(blindsTransform, amountToCall, chipAlignment);
            //    DisplayPlayerActionText(CurrentPlayerIndex, TurnType.Call, amountToCall);
            //}

            //if (OnPlayerTurnEnded != null) {
            //    OnPlayerTurnEnded(TurnType.Call, amountToCall);
            //}

            OnPlayerTurnEnded?.Invoke(TurnType.Call, amountToCall);
        }

        public void OnBetClicked()
        {
            Transform playerTransform = PlayersParent.transform.Find("Player" + CurrentPlayerIndex);
            Transform blindsTransform = playerTransform.Find("Blinds");

            PlayerWidget playerWidget = playerTransform.gameObject.GetComponent<PlayerWidget>();

            if (playerWidget == null) {
                Debug.LogError("GameController.OnBetClicked: PlayerWidget is null. Player index = " + CurrentPlayerIndex);
                return;
            }

            ChipAlignment chipAlignment = playerWidget.ChipAlignment;

            int amountToBet = _currentAmountToBet > 0 ? _currentAmountToBet : _currentGame.BigBlindSize;
            if (sceneName != "NEAT"  &&  sceneName != "TESTS")
            {
                PutChipsInfrontPlayer(blindsTransform, amountToBet, chipAlignment);
                DisplayPlayerActionText(CurrentPlayerIndex, TurnType.Bet, amountToBet);
            }

            //if (OnPlayerTurnEnded != null) {
            //    OnPlayerTurnEnded(TurnType.Bet, amountToBet);
            //}
            OnPlayerTurnEnded?.Invoke(TurnType.Bet, amountToBet);
        }

        public void OnRaiseClicked()
        {

            //Transform playerTransform = PlayersParent.transform.Find("Player" + CurrentPlayerIndex);
            //Transform blindsTransform = playerTransform.Find("Blinds");

            //PlayerWidget playerWidget = playerTransform.gameObject.GetComponent<PlayerWidget>();

            //if (playerWidget == null)
            //{
            //    Debug.LogError("GameController.OnRaiseClicked: PlayerWidget is null. Player index = " + CurrentPlayerIndex);
            //    return;
            //}

            //ChipAlignment chipAlignment = playerWidget.ChipAlignment;



            Hand currentHand = _currentGame.CurrentHand;
            Player currentPlayer = _currentGame.CurrentPlayer;

            //int defaultAmountToRaise = Math.Min(currentHand.GetHighestBetNotInPot() * 2 - currentPlayer.CurrentBet, currentPlayer.ChipCount);
            //int defaultAmountToRaise = Math.Min((currentHand.GetHighestBetNotInPot() + currentHand.BigBlindSize) - currentPlayer.CurrentBet, currentPlayer.ChipCount);

            printLog(" maior aposta nao no pot:  " +currentHand.GetHighestBetNotInPot()+ "  Qtd padrão pro raise Anterior: " + _currentAmountToBet + " Aposta Atual:  "+ currentPlayer.CurrentBet  );

            int defaultAmountToRaise = Math.Min((currentHand.GetHighestBetNotInPot() + _currentAmountToBet) - currentPlayer.CurrentBet, currentPlayer.ChipCount);
            //int defaultAmountToRaise = Math.Min((currentHand.GetHighestBetNotInPot() + currentHand.BigBlindSize) - currentPlayer.CurrentBet, currentPlayer.ChipCount);
          

            //printLog("Qtd padrão pro raise: " + defaultAmountToRaise);

            //_currentAmountToBet = defaultAmountToRaise;
            CurrentAmoutToBet = defaultAmountToRaise;
            //int amountToRaise = _currentAmountToBet > 0 ? _currentAmountToBet : defaultAmountToRaise;
            int amountToRaise = _currentAmountToBet;
            //if (sceneName != "NEAT"  &&  sceneName != "TESTS")
            //{
            //    PutChipsInfrontPlayer(blindsTransform, amountToRaise, chipAlignment);
            //    DisplayPlayerActionText(CurrentPlayerIndex, TurnType.Raise, amountToRaise);
            //}
            printLog( "Quanto foi o raise: "+ amountToRaise);

            //if (OnPlayerTurnEnded != null)
            //{
            //    OnPlayerTurnEnded(TurnType.Raise, amountToRaise);
            //}

            OnPlayerTurnEnded?.Invoke(TurnType.Raise, amountToRaise);
        }

        public void OnBetAmountEntered()
        {
            printLog("OnBetAmountEntered");
            string amountString = BetInputField.text;
            int amount;
            bool isParsed = int.TryParse(amountString, out amount);

            if (!isParsed) {
                Debug.LogError("OnBetAmountEntered: invalid amount entered!");
                return;
            }

            _currentAmountToBet = amount;

            if (BetButton.gameObject.activeSelf) {
                SetBetButtonText(amountString);
            } else if (RaiseButton.gameObject.activeSelf) {
                SetRaiseButtonText(amountString);
            }

            // TODO: adjust bet slider(incorrect now!)
            if (BetSlider != null && BetSlider.gameObject.activeSelf) {
                int maxAmount = _currentGame.CurrentPlayer.ChipCount;
                float value = (float)amount / (float)maxAmount;

                if (BetSlider.value != value) {
                    BetSlider.value = value;
                }
            }
        }

        public void OnBetSliderMoved()
        {
            printLog("OnBetSliderMoved");
            Hand currentHand = _currentGame.CurrentHand;
            Player currentPlayer = _currentGame.CurrentPlayer;

            int minAmount = currentHand.GetHighestBetNotInPot() * 2;
            int maxAmount = _currentGame.CurrentPlayer.ChipCount;
            float value = BetSlider.value;
            int amount = (int)(maxAmount * value);
            int amountInBB = amount / _currentGame.BigBlindSize;
            amount = amountInBB * _currentGame.BigBlindSize;

            amount = Math.Max(amount, minAmount);

            // adjust bet input field
            if (BetInputField != null && BetInputField.gameObject.activeSelf) {
                int currentBetFieldValue;
                // TODO: do we need isParsed here?
                bool isParsed = int.TryParse(BetInputField.text, out currentBetFieldValue);
                if (currentBetFieldValue != amount) {
                    BetInputField.text = amount.ToString();
                }
            }
        }

        public void SetHandTypeHintText(string text)
        {
            if (HandTypeHintText != null) {
                HandTypeHintText.text = text;
            }
        }

        public void MakeAITurn()
        {
            printLog("GameController.MakeAITurn, current player: " + _currentGame.CurrentPlayer.Name);
            if (!_currentGame.CurrentPlayer.IsAI)
            {
                Debug.LogError("GameController.MakeAITurn: current player is not AI!");
                return;
            }

            //if (CurrentHandState.Instance.TurnoAtual != HandState.Preflop)
            //{
            //    StartCoroutine(_currentGame.CurrentPlayer.PotencialMao());
            //}


            //printLog(CurrentGame.CurrentPlayer.Name + " Trial in progress ?  " + GameMenager.Instance.NeatTrialInprogress);
            //if (GameMenager.Instance.NeatTrialInprogress)
            //{
            // StartCoroutine(WaitBeforeAITurn());
            ProcessAITurnNorm();
            //StartCoroutine(ProcessAITurn());
            //ProcessAITurn();
            //}
            //else
            //{
                //StartCoroutine(ProcessAITurn());
            //}
        }

        private IEnumerator WaitBeforeAITurn()
        {
            _isAITurnDelayInProgress = true;
            //printLog("Começous o delay");
            //neatSupervisor.SetTempoTrial(true);
            yield return new WaitForSeconds(Defines.AI_TURN_TIME);
           
            _isAITurnDelayInProgress = false;
            //printLog("Terminou o delay");
        }

   

        public bool StartIAIf()
        {
            if (CurrentHandState.TurnoAtual != HandState.Preflop)
            {
                return  _currentGame.CurrentPlayer._handPotencialExecutando;
            }
            
            return _isAITurnDelayInProgress;
        }

        //public bool podeExecutar()
        //{
        //    printLog("Delay "+ _isAITurnDelayInProgress +" Neat tempo trial: " + !neatSupervisor.GetTempoTrial() +" Result: " +  (_isAITurnDelayInProgress && !neatSupervisor.GetTempoTrial()));
        //    return _isAITurnDelayInProgress && !neatSupervisor.GetTempoTrial();
        //}

        public bool checkIfTrialIsSame(string tipo)
        {
            CurrentTrial = GameMenager.Instance.CurrentTrial; //neatSupervisor.CurrentTrial;
            printLog("Current trial  "+CurrentTrial+" LastTrial: "+LastTrial);
            if (CurrentTrial == LastTrial)
            {
                return false;
            }
            else
            {
                if (tipo != "update") {
                    GameMenager.Instance.setTrialEnded(name, true);
                }
                //neatSupervisor.SetTempoTrial(true);
                LastTrial = CurrentTrial;
            }
            return true;

        }

        private void ProcessAITurnNorm()
        {
            //CurrentGame.CurrentPlayer.TomouDecisao = false;

            
            printLog(name + " GameController.ProcessAITurn start, current player: " + _currentGame.CurrentPlayer.Name);//+ "  " + _currentGame.CurrentPlayer.IsAI);


            TurnType turnType;

            _currentGame.CurrentPlayer.TomaDecisao();
            

            turnType = _currentGame.CurrentPlayer.Decisao;

            
            printLog(name + " Jogador: " + _currentGame.CurrentPlayer.Name + " Decisao  " + turnTypeToString(turnType));
             


            printLog(name + "  ProcessAITurn end, current player: " + _currentGame.CurrentPlayer.Name);
            
            switch (turnType)
            {
                case TurnType.Check:
                    OnCheckClicked();
                    break;
                case TurnType.Fold:
                    OnFoldClicked();
                    break;
                case TurnType.Call:
                    OnCallClicked();
                    break;
                case TurnType.Raise:
                    OnRaiseClicked();
                    break;
                case TurnType.Bet:
                    OnBetClicked();
                    break;
            }
        }
        
        private IEnumerator ProcessAITurn()
        {
            CurrentGame.CurrentPlayer.TomouDecisao = false;

            // neatSupervisor.TempoTrial = false;
            printLog(name + " GameController.ProcessAITurn start, current player: " + _currentGame.CurrentPlayer.Name);//+ "  " + _currentGame.CurrentPlayer.IsAI);

            //if (GameMenager.Instance.NeatTrialInprogress)
            //{
             yield return new WaitWhile(() => checkIfTrialIsSame(""));
            if ( !CurrentGame.CurrentPlayer.IsActive )
            {

                yield return null;
            }


            //yield return new WaitWhile(() => _isAITurnDelayInProgress);
            
            
            //neatSupervisor.SetTempoTrial(true);

            //yield return new WaitWhile(() => podeExecutar());
            // printLog("Teste");
            //}
            //else
            //{
            // yield return new WaitUntil(() => GameMenager.Instance.NeatTrialInprogress);
            //}
            // GameMenager.Instance.setTrialEnded(this.name, true);
            //ANNInProgress = true;
            // neatSupervisor.SetTempoTrial(true);
            //printLog(_currentGame.CurrentPlayer.PPOT);

            if (!_currentGame.CurrentPlayer.IsAI) {
                Debug.LogError("GameController.ProcessAITurn: current player is not AI!");
                yield break;
            }

            //int amount = 0;

            //if (_currentGame.CurrentPlayer.Index == 0)
            //{
            //    printLog("Jogador é do Indice 0 " + _currentGame.CurrentPlayer.Name );
            //}

            //TurnType turnType = PlayerAI.Instance.MakeDecision(out amount);


            //TurnType turnType = _currentGame.CurrentPlayer.TomaDecisao(out amount);
            //_currentGame.CurrentPlayer.tomaDeciscao();
            TurnType turnType;

            //if (CurrentHandState.Instance.TurnoAtual != HandState.Preflop) {
            //    //printLog("Iniciar potencial Mao");
            //   yield return _currentGame.CurrentPlayer.PotencialMao();
            //}

            //ANNInprogress.Instance.setTrialEnded("Table1", false);
            _currentGame.CurrentPlayer.TomaDecisao();
            // yield return new WaitWhile(()=> neatSupervisor.TempoTurno);
            //neatSupervisor.TempoTurno = false;

            yield return new WaitWhile(() => !CurrentGame.CurrentPlayer.TomouDecisao);

            turnType = _currentGame.CurrentPlayer.Decisao;

           // printLog("Jogador: " + _currentGame.CurrentPlayer.Name + " Decisao  " + turnTypeToString(turnType));
            //TurnType turnType = _currentGame.CurrentPlayer.Decisao; //PlayerAI.Instance.Decisao(out amount);

            printLog(name +" Jogador: "+ _currentGame.CurrentPlayer.Name +" Decisao  " + turnTypeToString(turnType));


            //if (turnType == TurnType.Bet || turnType == TurnType.Raise)
            //{
            //    _currentAmountToBet = CurrentGame.BigBlindSize;
            //}
            //else if (turnType == TurnType.Call)
            //{
            //    _currentAmountToBet = CurrentGame.BigBlindSize/2;
            //}



            printLog(name +"  ProcessAITurn end, current player: " + _currentGame.CurrentPlayer.Name);
            //printLog("ProcessAITurn end, current player: " + CurrentPlayerIndex);
            switch (turnType) {
                case TurnType.Check:
                    OnCheckClicked();
                    break;
                case TurnType.Fold:
                    OnFoldClicked();
                    break;
                case TurnType.Call:
                    OnCallClicked();
                    break;
                case TurnType.Raise:
                    OnRaiseClicked();
                    break;
                case TurnType.Bet:
                    OnBetClicked();
                    break;
            }
        }

        public string turnTypeToString(TurnType turn)
        {
            switch (turn)
            {
                case TurnType.Fold:
                    return "Fold";
                    
                case TurnType.Check:
                    return"Check";
                    
                case TurnType.Call:
                    return "Call ";
                    
                case TurnType.Bet:
                    return "Bet ";
                    
                case TurnType.Raise:
                    return "Raise ";
                    
            }

            return "not made";
        }

        public TurnType numToTurnType(int num)
        {
            switch (num)
            {
                case 1 :
                    return TurnType.Fold;


                case 2:
                    return TurnType.Check;
                    
                    
                case 3:
                    return TurnType.Call;


                case 4:
                    return TurnType.Bet;



                case 5:
                    return TurnType.Raise;
                    
                    
            }

            return TurnType.NotMade;
        }

        private void DisplayPlayerActionText(int playerIndex, TurnType moveType, int amount = 0)
        {
            Transform playerTransform = PlayersParent.transform.Find("Player" + playerIndex);

            if (playerTransform == null) {
                // TODO: log error
                return;
            }

            Transform playerActionTransform = playerTransform.Find("PlayerCard/LastActionText");

            if (playerActionTransform == null) {
                return;
            }

            Text playerActionText = playerActionTransform.gameObject.GetComponent<Text>();

            if (playerActionText == null) {
                return;
            }

            string amountString = amount > 0 ? amount.ToString() : string.Empty;

            switch (moveType) {
                case TurnType.Fold:
                    playerActionText.text = "Fold";
                    break;
                case TurnType.Check:
                    playerActionText.text = "Check";
                    break;
                case TurnType.Call:
                    playerActionText.text = "Call " + amountString;
                    break;
                case TurnType.Bet:
                    playerActionText.text = "Bet " + amountString;
                    break;
                case TurnType.Raise:
                    playerActionText.text = "Raise " + amountString;
                    break;
            }
        }

        private void PutChipsInfrontPlayer(Transform place, int betAmount, ChipAlignment chipAlignment = ChipAlignment.Right)
        {
            if (chipAlignment == ChipAlignment.Invalid) {
                Debug.LogWarning("PutChipsInfrontPlayer: chipAlignment is invalid, applying right alignment.");
                chipAlignment = ChipAlignment.Right;
            }

            for (int chipDenominationIndex = _chipDenominations.Length - 1; chipDenominationIndex >= 0; chipDenominationIndex--) {
                int denomination = (int)_chipDenominations[chipDenominationIndex];
                if (denomination > betAmount) {
                    continue;
                }

                while (betAmount >= denomination) {
                    GameObject chipObject = GameObject.Instantiate(ChipPrefab);
                    ChipWidget chipWidget = chipObject.GetComponent<ChipWidget>();
                    chipWidget.Denomination = _chipDenominations[chipDenominationIndex];

                    Vector3 chipPosition = place.position;
                    int chipsCount = place.childCount;
                    if (chipsCount > 0) {
                        int chipsInStack = chipsCount;
                        int stackIndex = 0;
                        if (chipsCount >= Defines.MAX_CHIPS_IN_ONE_STACK_IN_BETS) {
                            stackIndex = chipsCount / Defines.MAX_CHIPS_IN_ONE_STACK_IN_BETS;
                            chipsInStack = chipsCount % Defines.MAX_CHIPS_IN_ONE_STACK_IN_BETS;
                        }
                        // TODO: correct this magic
                        if (stackIndex > 0) {
                            if (chipAlignment == ChipAlignment.Right) {
                                chipPosition.x += (chipWidget.ImageWidth * stackIndex);
                            } else if (chipAlignment == ChipAlignment.Left) {
                                chipPosition.x -= (chipWidget.ImageWidth * stackIndex);
                            }
                        }
                        chipPosition.y += (chipWidget.ImageHeight * chipsInStack) / 6;
                    }
                    //chipObject.transform.parent = place;
                    chipObject.transform.SetParent(place,false);


                    chipObject.transform.localPosition = chipPosition;
                    chipObject.transform.localScale = Vector3.one;

                    betAmount -= denomination;
                }
            }

            if (betAmount > 0) {
                Debug.LogError("PutChipsInfrontPlayer: wrong bet amount (could not be presented with current chip denominations).");
            }
        }

        private void ClearPlayers()
        {
            for (int playerIndex = 0; playerIndex < _currentGame.Players.Count; playerIndex++) {
                Transform playerTransform = PlayersParent.transform.Find("Player" + playerIndex);
                Transform blindsTransform = playerTransform.Find("Blinds");
                Transform holeCardsTransform = playerTransform.Find("HoleCards");
                Transform buttonTransform = playerTransform.Find("Button");
                Transform deckTransform = playerTransform.Find("Deck");

                for (int chipIndex = blindsTransform.childCount - 1; chipIndex >= 0; chipIndex--) {
                    Transform childTransform = blindsTransform.GetChild(chipIndex);
                    GameObject.Destroy(childTransform.gameObject);
                }

                blindsTransform.DetachChildren();

                for (int cardIndex = holeCardsTransform.childCount - 1; cardIndex >= 0; cardIndex--) {
                    Transform childTransform = holeCardsTransform.GetChild(cardIndex);
                    GameObject.Destroy(childTransform.gameObject);
                }

                holeCardsTransform.DetachChildren();

                for (int buttonIndex = buttonTransform.childCount - 1; buttonIndex >= 0; buttonIndex--) {
                    Transform childTransform = buttonTransform.GetChild(buttonIndex);
                    GameObject.Destroy(childTransform.gameObject);
                }

                buttonTransform.DetachChildren();

                for (int deckIndex = deckTransform.childCount - 1; deckIndex >= 0; deckIndex--) {
                    Transform childTransform = deckTransform.GetChild(deckIndex);
                    GameObject.Destroy(childTransform.gameObject);
                }

                deckTransform.DetachChildren();

                Transform playerCard = playerTransform.Find("PlayerCard");
                if (playerCard != null) {
                    Transform playerActionTransform = playerTransform.Find("PlayerCard/LastActionText");
                    if (playerActionTransform != null) {
                        Text playerActionText = playerActionTransform.gameObject.GetComponent<Text>();
                        if (playerActionText != null) {
                            playerActionText.text = "";
                        }
                    }
                }
            }
        }

        private void ClearBoard()
        {
            if (sceneName != "NEAT"  &&  sceneName != "TESTS")
            {

                for (int cardIndex = Board.transform.childCount - 1; cardIndex >= 0; cardIndex--)
                {
                    Transform childTransform = Board.transform.GetChild(cardIndex);
                    GameObject.Destroy(childTransform.gameObject);
                }

                for (int potIndex = PotsParent.transform.childCount - 1; potIndex >= 0; potIndex--)
                {
                    Transform potTransform = PotsParent.transform.Find("Pot" + potIndex);

                    // first child is pot size text (should not be destoyed)
                    for (int chipIndex = potTransform.childCount - 1; chipIndex >= 1; chipIndex--)
                    {
                        Transform childTransform = potTransform.GetChild(chipIndex);
                        GameObject.Destroy(childTransform.gameObject);
                    }

                    potTransform.gameObject.SetActive(false);
                }


            }

            IsFlopDealt = false;
            IsTurnDealt = false;
            IsRiverDealt = false;
        }

        private void ResetBetSliderAndInput()
        {
            if (BetSlider != null)
            {
                BetSlider.value = BetSlider.minValue;
            }

            SetBetButtonText("");
            SetRaiseButtonText("");

            if (BetInputField != null)
            {
                BetInputField.text = "";
            }
        }
    }
}
