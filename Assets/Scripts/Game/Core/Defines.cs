using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PokerCats
{
    public class Defines
    {
        // Game rules
        public static readonly int DECK_CARDS_COUNT = 52;
        public static readonly int HOLE_CARDS_COUNT = 2;
        public static readonly int FLOP_CARDS_COUNT = 3;
        public static readonly int CARDS_COUNT_ON_TURN = 4;
        public static readonly int CARDS_COUNT_ON_RIVER = 5;
        public static readonly int CARDS_IN_HAND_COUNT = 5;

        // Animation delays
        public static readonly float DELAY_IN_BLIND_POSTING = 0.5f;
        public static readonly float DELAY_BETWEEN_DEALING_CARDS = 0.25f;
        public static readonly float CARD_DEALING_TIME = 0.2f;
        public static readonly float DELAY_BETWEEN_DEALING_STREETS = 0.5f;
        public static readonly float SHOW_CARDS_TIME = 1.0f;

        // Table settings
        public static readonly int MAX_CHIPS_IN_ONE_STACK_IN_BETS = 5;
        public static readonly int MAX_STACKS_IN_ONE_ROW_IN_BETS = 5;

        // Timers
         public static readonly float SECONDS_FOR_TURN = 1.0f;
         public static readonly float AI_TURN_TIME = 0.5f;

        public static List<Vector3> positions = new List<Vector3>(new Vector3[] {
        new Vector3(-18f, -171f, 0),
        new Vector3(-274f, -50f, 0),
        new Vector3(-202f, 100f, 0),
        new Vector3(0f, 149.9f, 0),
        new Vector3(206f, 100f, 0),
        new Vector3(261f, -50f, 0)
    });

    }

    public static class PlayersAreasConstants
    {
        public const string middle = "MiddleArea";
        public const string player0 = "Player0";
        public const string player1 = "Player1";
        public const string player2 = "Player2";
        public const string player3 = "Player3";
        public const string player4 = "Player4";
        public const string player5 = "Player5";
        public const string noPlayer = "none";
        public static readonly Dictionary<string, string> playersAreaDictionary = new Dictionary<string, string>
    {
        { player0, "Jogador 0" },
        { player1, "Jogador 1" },
        { player2, "Jogador 2" },
        { player3, "Jogador 3" },
        { player4, "Jogador 4" },
        { player5, "Jogador 5" },
        { noPlayer, "Nenhum" },
    };
        public static readonly Dictionary<string, string> playersInverseDictionary = new Dictionary<string, string>
    {
        {  "Jogador 0" , player0},
        { "Jogador 1", player1 },
        {  "Jogador 3", player2 },
        {  "Jogador 4", player3 },
        { "Jogador 5", player4 },
        {  "Jogador 6", player5 },
        { "Nenhum", noPlayer},
    };

        public static readonly Dictionary<string, Dictionary<string, string>> playersPositionRelatives = new Dictionary<string, Dictionary<string, string>>
    {
        { player1, new Dictionary<string, string>(){
            {player1, playersAreaDictionary[player1]},
            {player2, playersAreaDictionary[player2]},
            {player3, playersAreaDictionary[player3]},
            {player4, playersAreaDictionary[player4]}}
        },
        { player2, new Dictionary<string, string>(){
            {player2, playersAreaDictionary[player1]},
            {player3, playersAreaDictionary[player2]},
            {player4, playersAreaDictionary[player3]},
            {player1, playersAreaDictionary[player4]}
        }},
        { player3, new Dictionary<string, string>(){
            {player3, playersAreaDictionary[player1]},
            {player4, playersAreaDictionary[player2]},
            {player1, playersAreaDictionary[player3]},
            {player2, playersAreaDictionary[player4]}
        }},
        { player4, new Dictionary<string, string>(){
            {player4, playersAreaDictionary[player1]},
            {player1, playersAreaDictionary[player2]},
            {player2, playersAreaDictionary[player3]},
            {player3, playersAreaDictionary[player4]}
        }},
    };
        public static readonly Dictionary<string, Dictionary<string, string>> playersPositionRelativesInverse = new Dictionary<string, Dictionary<string, string>>
    {
        { player1, new Dictionary<string, string>(){
            {playersAreaDictionary[player1], player1},
            {playersAreaDictionary[player2], player2},
            {playersAreaDictionary[player3], player3},
            {playersAreaDictionary[player4], player4}}
        },
        { player2, new Dictionary<string, string>(){
            {playersAreaDictionary[player1], player2},
            {playersAreaDictionary[player2], player3},
            {playersAreaDictionary[player3], player4},
            {playersAreaDictionary[player4], player1}
        }},
        { player3, new Dictionary<string, string>(){
            {playersAreaDictionary[player1], player3},
            {playersAreaDictionary[player2], player4},
            {playersAreaDictionary[player3], player1},
            {playersAreaDictionary[player4], player2}
        }},
        { player4, new Dictionary<string, string>(){
            {playersAreaDictionary[player1], player4},
            {playersAreaDictionary[player2], player1},
            {playersAreaDictionary[player3], player2},
            {playersAreaDictionary[player4], player3}
        }},
    };
    }


}
