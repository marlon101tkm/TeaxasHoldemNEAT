using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

using UnityEngine;

namespace PokerCats
{
    public class Deck
    {
        private List<Card> _deckOfCards;
        private List<HoleCards> _listaHoleCards;

        public Deck()
        {
            _deckOfCards = new List<Card>();
            _listaHoleCards = new List<HoleCards>();
        }

        public List<Card> DeckOfCards
        {
            get { return _deckOfCards; }
        }

        public List<HoleCards> ListaHoleCards
        {
            get { return _listaHoleCards; }
        }

        public void Init()
        {
            for (Colour colour = Colour.Clubs; colour < Colour.Count; ++colour) {
                for (Rank rank = Rank.Deuce; rank < Rank.Count; ++rank) {
                    Card newCard = new Card(rank, colour);
                    // TODO: check if we should store go or it's script component in the list
                    _deckOfCards.Add(newCard);
                }
            }

            // test
            //    for (Rank rank = Rank.Deuce; rank < Rank.Count; ++rank) {
            //        for (Colour colour = Colour.Clubs; colour < Colour.Count; ++colour) {
            //        Card newCard = new Card(rank, colour);
            //        // TODO: check if we should store go or it's script component in the list
            //        _deckOfCards.Add(newCard);
            //    }
            //}
        }

        /*
        public List<HoleCards> removeBoardCards(List<HoleCards> lista , HoleCards cartamao)
        {
            int index = 0;
            foreach (var cards in lista)
            {
                if (cartamao.Equals(cards)){
                    index = lista.IndexOf(cards); 
                }
            }

            lista.RemoveAt(index);

            return lista;
        }
        */

        public void RemoveDoubleHoleCards(HoleCards player, HoleCards op)
        {
            List<int> index = new List<int>();

            foreach (Card carta in _deckOfCards)
            {
                if (carta.Equals(player.First) | carta.Equals(player.Second) | carta.Equals(player.First) | carta.Equals(player.Second))
                {
                    index.Add(_deckOfCards.IndexOf(carta));
                }
            }
            foreach (int i in index)
            {
                _deckOfCards.RemoveAt(i);
            }
                        
        }

        public void RemoveHoleTableCards(HoleCards player,List<Card> tableCards )
        {
            List<int> index = new List<int>();

            List<Card> listaExcluir = new List<Card>();
            foreach (Card carta in _deckOfCards)
            {
                if ( carta.Equals(player.First) | carta.Equals(player.Second))
                {
                    //Debug.Log("Indice:" + _deckOfCards.IndexOf(carta) + " Carta: " + carta.GetTextInfo());
                    //index.Add(_deckOfCards.IndexOf(carta));
                    listaExcluir.Add(carta);
                }
                else
                {
                    foreach (Card cartasMesa in tableCards)
                    {
                        if (carta.Equals(cartasMesa))
                        {
                            //Debug.Log("Indice:" + _deckOfCards.IndexOf(carta) + " Carta: " + carta.GetTextInfo());
                            //index.Add(_deckOfCards.IndexOf(carta));
                            listaExcluir.Add(carta);
                        }
                    }
                }
            }

            //Debug.Log(" Qtd Escluir lista: "+index.Count);
            //Debug.Log(" Deck Count: " + _deckOfCards.Count);
            //foreach (int i in index)
            foreach( Card c in listaExcluir )
            {
                //Debug.Log("Index: "+i);
                _deckOfCards.Remove(c);
                
                //_deckOfCards.RemoveAt(i);
            }

        }

        public void RemoveHoleCards(Card pri, Card seg)
        {
            //int i1 = -1, i2 = -1;

            List<int> index = new List<int>();
             //Debug.Log("--------------------------------------------Buscando carta para remover hole -----------------------");
            foreach (Card carta in _deckOfCards)
            {
                // Debug.Log("Pri: "+pri.GetTextInfo() +" ? "+ carta.Equals(pri)  + " Seg:  " + seg.GetTextInfo()+" ? "+ carta.Equals(seg) + " Carta: " + carta.GetTextInfo() );
                // Debug.Log("Contem pri ?: "+ _deckOfCards.Contains(pri) +"Contem seg ?: "+ _deckOfCards.Contains(seg));


                if (carta.Equals(pri) | carta.Equals(seg))
                {
                   // Debug.Log("Indice:" + _deckOfCards.IndexOf(carta) + " Carta: " + carta.GetTextInfo());
                    index.Add(_deckOfCards.IndexOf(carta));
                    // _deckOfCards.Remove(carta);
                }

                //if (carta.Equals(pri))
                //{
                //       // Debug.Log("Indice 1:" + _deckOfCards.IndexOf(carta) + " Carta: " + carta.GetTextInfo());

                //    i1 = _deckOfCards.IndexOf(carta);
                //}

                //if (carta.Equals(seg))
                //{
                //        Debug.Log("Indice 2:" + _deckOfCards.IndexOf(carta) + " Carta: " + carta.GetTextInfo());

                //    i2 = _deckOfCards.IndexOf(carta);
                //}
            }

            //Debug.Log("Qtd indice:"+ index.Count);
            //if ( index.Count > 0 ) {
            // Debug.Log("----------Remove HoleCards----------"+ index.Count );

            //Debug.Log("Deck Count:" + _deckOfCards.Count);
            // Debug.Log("last Index:" + _deckOfCards.IndexOf(_deckOfCards.Last()));
            // Debug.Log(" Index 1 :" + i1);
            //  Debug.Log(" Index 2 :" + i2);
            //Debug.Log("--------------------------------------------Cartas para remover -----------------------");
            foreach (int i in index)
            {
                // Debug.Log(" Index  :"+ i);
                if (i == _deckOfCards.Count)
                {
                    _deckOfCards.RemoveAt(i-1);
                }
                else
                {
                    _deckOfCards.RemoveAt(i);
                }
            }


           // _deckOfCards.RemoveAt(i1);
            //_deckOfCards.RemoveAt(i2);

        }
        public List<Card> RemoveHoleCardsUltimaCarta(List<Card> lista , HoleCards holecard)
        {
            
            List<int> index = new List<int>();

            foreach (Card carta in lista)
            {
                if (carta.Equals(holecard.First)|| carta.Equals(holecard.Second))
                {
                    index.Add(lista.IndexOf(carta));
                }

                
            }

            foreach (int i in index)
            {
                lista.RemoveAt(i);
            }
            return lista;

        }


        public void RemoveTableCards(List<Card> tableCards)
        {
            List<int> index= new List<int>();
            //Debug.Log("--------------------------------------------Buscando carta para remover table -----------------------");
            foreach (Card cartaCheck in tableCards)
            {
                if (_deckOfCards.Contains(cartaCheck))
                {
                    //Debug.Log("Indice:" + _deckOfCards.IndexOf(cartaCheck) + " Carta: " + cartaCheck.GetTextInfo());
                    index.Add(_deckOfCards.IndexOf(cartaCheck));
                }

                //foreach (Card carta in _deckOfCards)
                //{
                //    Debug.Log(" Carta Check: " + cartaCheck.GetTextInfo() +"Carta Deck: "+carta.GetTextInfo());
                //    if (cartaCheck.Equals(carta))
                //    {
                //        index.Add(_deckOfCards.IndexOf(carta));
                //    }


                //}
            }

            //Debug.Log(" Contagen de cartas pra remover" + index.Count);
            foreach (int i in index)
            {
                if (i == _deckOfCards.Count)
                {
                    _deckOfCards.RemoveAt(i - 1);
                }
                else
                {
                    _deckOfCards.RemoveAt(i);
                }
            }
        }

        public List<HoleCards> removeFromTurnRiver(List<HoleCards> lista, HoleCards holecards)
        {
            int index = 0;
            foreach (HoleCards cards in lista)
            {

                if (cards.Equals(holecards))
                {
                    index = lista.IndexOf(cards);
                }
            }


            lista.RemoveAt(index);

            return lista;
        }

        public List<HoleCards> removeFromTurnRiver(List<HoleCards> lista, List<Card> board)
        {
            List<int> index = new List<int>();
            foreach (HoleCards cards in lista)
            {
                foreach ( var tableCard in board ) {
                    if (cards.First.Equals(tableCard) || cards.Second.Equals(tableCard))
                    {
                        
                        index.Add(lista.IndexOf(cards));
                    }
                }
            }

            foreach (int i in index)
            {
                lista.RemoveAt(i);
            }

            

            return lista;
        }
        public bool TesteSeContains(List<HoleCards> listaTurnERiver, HoleCards holeCards)
        {
            foreach (var item in  listaTurnERiver)
            {
                if (holeCards.First.Equals(item.First)
                    || holeCards.Second.Equals(item.Second)
                    || holeCards.Second.Equals(item.First)
                    || holeCards.First.Equals(item.Second))
                {
                    return false;
                }


            }

            return true;
        } 

        public List<List<Card>> RemoverHoleCardFromFlopCombination(List<List<Card>> lista , HoleCards holecards)
        {
            List<int> index = new List<int>();
            foreach (var sublista in lista  )
            {
                foreach (var card in sublista)
                {
                    if (card.Equals(holecards.First) || card.Equals(holecards.First))
                    {
                        index.Add(lista.IndexOf(sublista));
                    }
                }
            }

            foreach (int i in index)
            {
                lista.RemoveAt(i);
            }

            return lista;
        }

        public List<List<Card>> gerenateFlopCombination(HoleCards holeCards)
        {

            Clear();
            Init();
            RemoveHoleCards(holeCards.First, holeCards.Second);
            //int index = 0;
            List<List<Card>> generatedFlop = new List<List<Card>>();
            List<Card> listCards = new List<Card>();


            for (int i = 0; i < _deckOfCards.Count - 4; i++)
            {
                for (int j = i + 1; j < _deckOfCards.Count - 3; j++)
                {
                    for (int k = j + 1; k < _deckOfCards.Count - 2; k++)
                    {
                        listCards.Clear();
                        listCards.Add(_deckOfCards.ElementAt(i));
                        listCards.Add(_deckOfCards.ElementAt(j));
                        listCards.Add(_deckOfCards.ElementAt(k));
                        //Debug.Log("Carta: " + _deckOfCards.ElementAt(i).GetTextInfo()
                        //        + "Carta: " + _deckOfCards.ElementAt(j).GetTextInfo()
                        //        + "Carta: " + _deckOfCards.ElementAt(k).GetTextInfo()
                        //        + "Carta: " + _deckOfCards.ElementAt(w).GetTextInfo()
                        //        + "Carta: " + _deckOfCards.ElementAt(y).GetTextInfo());
                        generatedFlop.Add(listCards);
                        //index++;

                    }
                }
            }
            return generatedFlop;

        }

        public List<HoleCards> generateDeckCombination( HoleCards holeCards)
        {
            Clear();
            Init();
            RemoveHoleCards(holeCards.First, holeCards.Second);
            
            List<HoleCards> listaComb = new List<HoleCards>();
            HoleCards novoHoleCards;
            // Debug.Log("------------------------------------------------Inicou geração das Cartas--------------------------------------------");
            for (int i = 0; i < _deckOfCards.Count - 1; i++)
            {
                for (int j = i + 1; j < _deckOfCards.Count; j++)
                {
                    novoHoleCards.First = _deckOfCards.ElementAt(i);
                    novoHoleCards.Second = _deckOfCards.ElementAt(j);
                    // Debug.Log("I: " + novoHoleCards.First.GetTextInfo() + " J: " + novoHoleCards.Second.GetTextInfo());
                    listaComb.Add(novoHoleCards);
                    

                }
            }
            // Debug.Log("------------------------------------------------Acabou geração das Cartas--------------------------------------------");

            return listaComb;
        }

        public IEnumerator generateRiverDeckCombination(List<Card> boardCards, HoleCards holeCards, HoleCards holeCardsOP)
        {
            Clear();
            Init();
            RemoveDoubleHoleCards(holeCards, holeCardsOP);
            RemoveTableCards(boardCards);

            yield return null;
        }

        public IEnumerator generateNewDeckCombination(List<Card> boardCards, HoleCards holeCards , HoleCards holeCardsOP)
        {
           // Debug.Log(" -------------------------------------------------------------------------------------------------------iniciou  Geração TurnERiver");
            Clear();
            Init();
            _listaHoleCards.Clear();
            RemoveDoubleHoleCards(holeCards, holeCardsOP);
            
            // RemoveHoleCards(holeCards.First, holeCards.Second);
            
            //RemoveHoleCards(holeCardsOP.First, holeCardsOP.Second);
            
            RemoveTableCards(boardCards);
            //Debug.Log(" Contagende cartas Turne river" + _deckOfCards.Count);


            //List<HoleCards> listaComb = new List<HoleCards>();
            HoleCards novoHoleCards;
            // Debug.Log("------------------------------------------------Inicou geração das Cartas--------------------------------------------");
            for (int i = 0; i < _deckOfCards.Count - 1; i++)
            {
                for (int j = i + 1; j < _deckOfCards.Count; j++)
                {
                    novoHoleCards.First = _deckOfCards.ElementAt(i);
                    novoHoleCards.Second = _deckOfCards.ElementAt(j);
                    // Debug.Log("I: " + novoHoleCards.First.GetTextInfo() + " J: " + novoHoleCards.Second.GetTextInfo());
                    _listaHoleCards.Add(novoHoleCards);

                }
            }
           // Debug.Log(" -------------------------------------------------------------------------------------------------------Terminou  Geração TurnERiver: " + _deckOfCards.Count + "    " + _listaHoleCards.Count);
            yield return false;
        }


        public IEnumerator generateNewDeckCombination(List<Card> boardCards, HoleCards holeCards)
        {
           // Debug.Log("-------------------------------------------------------------------------------------------------------iniciou  Geração HoleCardsOP");
            Clear();
            Init();
            _listaHoleCards.Clear();
            RemoveHoleCards(holeCards.First, holeCards.Second);
            
            RemoveTableCards(boardCards);
           // Debug.Log(" Contagen de cartas iniciais " + _deckOfCards.Count);
            //Debug.Log(" Contagende cartas iniciais " + _deckOfCards.Count);
            //List<HoleCards> listaComb = new List<HoleCards>();
            HoleCards novoHoleCards;
            // Debug.Log("------------------------------------------------Inicou geração das Cartas--------------------------------------------");
            for (int i = 0; i < _deckOfCards.Count - 1; i++)
            {
                for (int j = i + 1; j < _deckOfCards.Count; j++)
                {
                    novoHoleCards.First = _deckOfCards.ElementAt(i);
                    novoHoleCards.Second = _deckOfCards.ElementAt(j);
                    // Debug.Log("I: " + novoHoleCards.First.GetTextInfo() + " J: " + novoHoleCards.Second.GetTextInfo());
                    _listaHoleCards.Add(novoHoleCards);

                }
            }
           // Debug.Log(" ------------------------------------------------------------------------------------------------------- Terminou  Geração HoleCardsOP:  " + _deckOfCards.Count + "    " + _listaHoleCards.Count   );
            yield return null;
        }

        public List<HoleCards> generateDeckCombination(List<Card> boardCards, HoleCards holeCards)
        {
            Clear();
            Init();
            //RemoveHoleTableCards(holeCards, boardCards);
            RemoveHoleTableCards(holeCards,boardCards);
            //RemoveHoleCards(holeCards.First, holeCards.Second);
            //RemoveTableCards(boardCards);
            List<HoleCards> listaComb = new List<HoleCards>();
            HoleCards novoHoleCards;
           // Debug.Log("------------------------------------------------Inicou geração das Cartas--------------------------------------------");
            for (int i = 0;i< _deckOfCards.Count-1; i++ )
            {
                for (int j = i+1;j < _deckOfCards.Count;j++ )
                {
                    novoHoleCards.First = _deckOfCards.ElementAt(i);
                    novoHoleCards.Second = _deckOfCards.ElementAt(j);
                   // Debug.Log("I: " + novoHoleCards.First.GetTextInfo() + " J: " + novoHoleCards.Second.GetTextInfo());
                    listaComb.Add(novoHoleCards);
                    
                }
            }
           // Debug.Log("------------------------------------------------Acabou geração das Cartas--------------------------------------------");

            return listaComb;
        }




        public List<HoleCards> generateDeckCombination(List<Card> boardCards)
        {
            Clear();
            Init();
            RemoveTableCards(boardCards);
            List<HoleCards> listaComb = new List<HoleCards>();
            HoleCards novoHoleCards;

            for (int i = 0; i < _deckOfCards.Count -1 ; i++)
            {
                for (int j = i + 1; j < _deckOfCards.Count; j++)
                {
                    novoHoleCards.First = _deckOfCards.ElementAt(i);
                    novoHoleCards.Second = _deckOfCards.ElementAt(j);
                    Debug.Log("Carta P: " + novoHoleCards.First.GetTextInfo() + "\n Carta S:" + novoHoleCards.First.GetTextInfo());
                    listaComb.Add(novoHoleCards);
                }
            }


            return listaComb;
        }

        public List<Card> genereteDeckWithoutBoardAndHoleCards(List<Card> tableCards , HoleCards holeCards )
        {
            Clear();
            Init();
            //RemoveTableCards(tableCards);
            //RemoveHoleCards(holeCards.First,holeCards.Second);
            RemoveHoleTableCards(holeCards,tableCards);
            return _deckOfCards;
        }



        public void Shuffle()
        {
            System.Random random = new System.Random();

            for (int i = 0; i < _deckOfCards.Count; i++)
            {
                int j = random.Next(i, _deckOfCards.Count);
                Card temporary = _deckOfCards[i];
                _deckOfCards[i] = _deckOfCards[j];
                _deckOfCards[j] = temporary;
            }
        }


        //public void Shuffle()
        //{
        //    int n = _deckOfCards.Count;

        //    while (n > 1)
        //    {

        //        n--;
        //        int k = RandomNumber.GetRandomNumber(n + 1);
        //        Card card = _deckOfCards[k];
        //        _deckOfCards[k] = _deckOfCards[n];
        //        _deckOfCards[n] = card;
        //        //Debug.Log("Qtd Cartas: " + n);
        //    }
        //}

        public void Clear()
        {
            _deckOfCards.Clear();
        }

        public void StartNewHand()
        {
            Clear();
            Init();
            Shuffle();
        }

        public Card DealTopCard()
        {
            Card cardToDeal = _deckOfCards.First();
            _deckOfCards.Remove(cardToDeal);

            return cardToDeal;
        }

        public void BurnTopCard()
        {
            _deckOfCards.RemoveAt(0);
        }

        public List<Card> DealFlop()
        {
            List<Card> flopCardsToDeal = new List<Card>();

            BurnTopCard();

            for (int flopCardIndex = 0; flopCardIndex < Defines.FLOP_CARDS_COUNT; flopCardIndex++) {
                Card flopCard = _deckOfCards.First();
                _deckOfCards.Remove(flopCard);
                flopCardsToDeal.Add(flopCard);
            }

            return flopCardsToDeal;
        }

        public Card DealTurnOrRiver()
        {
            BurnTopCard();

            return DealTopCard();
        }



    }
}
