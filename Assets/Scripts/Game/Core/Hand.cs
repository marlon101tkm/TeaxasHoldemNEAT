using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace PokerCats
{
    public class Hand
    {
        private List<Player> _playersInvolved = new List<Player>();
        private List<Pot> _pots = new List<Pot>();

        private int _bigBlindSize;
        private int _anteSize;

        public int BigBlindSize
        {
            get { return _bigBlindSize; }
        }

        public Pot MainPot
        {
            get { return _pots[0]; }
        }

        public List<Pot> Pots
        {
            get { return _pots; }
        }

        public List<Player> PlayersInvolved
        {
            get { return _playersInvolved; }
        }

        public void SetPlayersPositions()
        {
            foreach (Player player in _playersInvolved) {
                SetNewPlayerPosition(player);
            }
        }

        public void SetNewPlayerPosition(Player player)
        {
            Position firstPositionAfterBB = Position.Count - _playersInvolved.Count;
            Position newPosition = player.Position - 1;
            if (newPosition < firstPositionAfterBB || !Utils.IsPositionValid(newPosition)) {
                newPosition = Position.BB;
            }
            player.Position = newPosition;
        }

        public void SetBlindsAndAntes(int bigBlindSize, int anteSize)
        {
            if (bigBlindSize <= 0 || anteSize < 0) {
                Debug.LogError("SetBlindsAndAntes: attempt to set wrong BB or ante amount! Big blind: " + bigBlindSize + ", ante: " + anteSize);
                return;
            }

            _bigBlindSize = bigBlindSize;
            _anteSize = anteSize;
        }

        public void PostBlindsAndAntes()
        {
            foreach (Player player in _playersInvolved) {
                if (Utils.IsOnBlinds(player.Position)) {
                    if (player.IsOnSB) {
                        player.PostBlind(_bigBlindSize / 2);
                    } else if (player.IsOnBB) {
                        player.PostBlind(_bigBlindSize);
                    }
                }
                if (_anteSize > 0) {
                    player.PostAnte(_anteSize);
                }
            }
        }

        public void AddPlayers(List<Player> players)
        {
            _playersInvolved.AddRange(players);
        }

        public void AddPlayer(Player player)
        {
            _playersInvolved.Add(player);
        }

        public void RemovePlayer(Player player)
        {
            _playersInvolved.Remove(player);
            foreach (Pot pot in _pots) {
                if (pot.PlayersInPot.Contains(player)) {
                    pot.PlayersInPot.Remove(player);
                }
            }
        }

        public bool HasPlayersLeft()
        {
            return _playersInvolved.Count > 0;
        }

        public bool HasOnePlayerLeft()
        {
            return _playersInvolved.Count == 1;
        }

        public bool IsPlayerInvolved(Player player)
        {
            return _playersInvolved.Contains(player);
        }

        public int GetHighestBetNotInPot()
        {
            int highestBet = 0;

            foreach (Player player in _playersInvolved) {
                if (player.CurrentBet > highestBet) {
                    highestBet = player.CurrentBet;
                }
            }

            return highestBet;
        }

        // even if somebody has called, this will return open raiser/first bettor position
        public Position GetPlayerWithHighestBetPosition()
        {
            Position position = Position.Invalid;
            int highestBet = 0;

            foreach (Player player in _playersInvolved) {
                if (player.CurrentBet > highestBet) {
                    highestBet = player.CurrentBet;
                    position = player.Position;
                }
            }

            return position;
        }

        public List<Player> GetWinners(Pot pot)
        {
            List<Player> winners = new List<Player>();

            // TODO: cache winner value because this method is called more than once at the end of each hand
            if (_playersInvolved.Count == 1)
            {
                winners.Add(_playersInvolved[0]);
                return winners;
            }

            PlayerHandInfo strongestHand;
            strongestHand.HandType = HandType.Invalid;
            strongestHand.HandColour = Colour.Invalid;
            strongestHand.MainRank = Rank.Invalid;
            strongestHand.SecondRank = Rank.Invalid;

            Player winner = null;

            foreach (Player player in pot.PlayersInPot)
            {
               // Debug.Log( player.PClass+" "+player.CurrentHandInfo.toString());
                if (player.CurrentHandInfo.HandType > strongestHand.HandType)
                {
                    strongestHand = player.CurrentHandInfo;
                    winner = player;
                }
                else if (player.CurrentHandInfo.HandType == strongestHand.HandType)
                {
                    if (player.CurrentHandInfo.MainRank != Rank.Invalid)
                    {
                        if (player.CurrentHandInfo.MainRank > strongestHand.MainRank)
                        {
                            strongestHand = player.CurrentHandInfo;
                            winner = player;
                        }
                        else if (player.CurrentHandInfo.MainRank == strongestHand.MainRank)
                        {
                            if (player.CurrentHandInfo.SecondRank != Rank.Invalid)
                            {
                                if (player.CurrentHandInfo.SecondRank > strongestHand.SecondRank)
                                {
                                    strongestHand = player.CurrentHandInfo;
                                    winner = player;
                                }
                                else if (player.CurrentHandInfo.SecondRank == strongestHand.SecondRank)
                                {
                                    // Hands are completely equal
                                    winners.Add(winner);
                                    winner = player;
                                }
                            }
                            else
                            {
                                // Hand type and main rank are equal and there's no second rank
                                winners.Add(winner);
                                winner = player;
                            }
                        }
                    }
                }
                //Debug.Log("Jogador" + player.Name + " Cartas: " + player.CurrentHandInfo.HandType );
            }

            winners.Add(winner);

            return winners;
        }

        public void GivePotsToWinners()
        {
            //PlayerHandInfo handinfo;// = new PlayerHandInfo();

             //GameMenager.Instance.CreateText("Results5.txt",  (" ---------------Partida-------------- \n"));
            //foreach (Pot pot in _pots)
            //{
            //foreach (Player player in _playersInvolved)
            //{
            //    Debug.Log(" Jogador " + player.PClass + " Position: " + player.Position + "  Chips qtd: " + player.ChipCount + "  " + player._currentGame.GameController.HandChecker.GetFullHandTypeString(out handinfo, player.HoleCards, player._currentGame.Board.FullBoard));
            //}
           // Debug.Log("---------------Mãos-------------");
            Pot pot = _pots[0];
            List<Player> winners = GetWinners(pot);
            //Debug.Log("Indice pot : " + _pots.IndexOf(pot));
           // Debug.Log("---------------Vencedores-------------");
            foreach (Player winner in winners)
            {
                winner.AddChips(pot.Size / winners.Count);

                
                //winner.Wins++;

                winner.ROI = winner.CalculoRoi();
                //if (SceneManager.GetActiveScene().name == "TESTS")
                //{
                //    //if (winner.Position != Position.SB)
                //    //{
                //    GameMenager.Instance.CreateText("Results5.txt", (winner.PClass + ";" + winner.Position + ";" + winner.ROI + "\n"));
                //    //GameMenager.Instance.CreateText("Result.txt", "pot: " + _pots.IndexOf(pot) + "  " + winner._currentGame.GameController.HandChecker.GetFullHandTypeString(out handinfo, winner.HoleCards, winner._currentGame.Board.FullBoard)+"\n");
                //    // }
                //}
                // Debug.Log(winner.PClass + " Position: " + " ROI: " +winner.ROI+ " Ganhador: " +  winner.Position + "  Chips qtd: " + winner.ChipCount + "  " + winner._currentGame.GameController.HandChecker.GetFullHandTypeString(out handinfo, winner.HoleCards, winner._currentGame.Board.FullBoard));

            }
            //}

        }

        public void GivePotsToWinners(int partida)
        {
            //PlayerHandInfo handinfo;// = new PlayerHandInfo();

            //GameMenager.Instance.CreateText("Results5.txt",  (" ---------------Partida-------------- \n"));
            //foreach (Pot pot in _pots)
            //{
            //foreach (Player player in _playersInvolved)
            //{
            //    Debug.Log(" Jogador " + player.PClass + " Position: " + player.Position + "  Chips qtd: " + player.ChipCount + "  " + player._currentGame.GameController.HandChecker.GetFullHandTypeString(out handinfo, player.HoleCards, player._currentGame.Board.FullBoard));
            //}
            // Debug.Log("---------------Mãos-------------");
            Pot pot = _pots[0];
            List<Player> winners = GetWinners(pot);
            //Debug.Log("Indice pot : " + _pots.IndexOf(pot));
            // Debug.Log("---------------Vencedores-------------");
            foreach (Player winner in winners)
            {
                winner.AddChips(pot.Size / winners.Count);


                //winner.Wins++;

                winner.ROI = winner.CalculoRoi();
                if (SceneManager.GetActiveScene().name == "TESTS")
                {
                    //if (winner.Position != Position.SB)
                    //{
                    GameMenager.Instance.CreateText(GameMenager.Instance.TxtFileName, (winner.PClass + ";" + System.Math.Round(winner.ROI *100) + "\n"),"Jogador;ROI");
                    //GameMenager.Instance.CreateText("Result.txt", "pot: " + _pots.IndexOf(pot) + "  " + winner._currentGame.GameController.HandChecker.GetFullHandTypeString(out handinfo, winner.HoleCards, winner._currentGame.Board.FullBoard)+"\n");
                    // }
                }
                // Debug.Log(winner.PClass + " Position: " + " ROI: " +winner.ROI+ " Ganhador: " +  winner.Position + "  Chips qtd: " + winner.ChipCount + "  " + winner._currentGame.GameController.HandChecker.GetFullHandTypeString(out handinfo, winner.HoleCards, winner._currentGame.Board.FullBoard));

            }
            //}

        }


        public void AddPot(Pot pot)
        {
            _pots.Add(pot);
        }
    }
}
