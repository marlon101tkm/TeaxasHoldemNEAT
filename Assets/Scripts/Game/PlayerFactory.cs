using PokerCats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySharpNEAT;

public class PlayerFactory : MonoBehaviour
{
    
   
    public GameObject playerLA;
    public GameObject playerTA;
    public GameObject playerTP;
    public GameObject playerLP;
    public GameObject playerRandom;

    private static List<Vector3> positions = new List<Vector3>(new Vector3[] {
        new Vector3(-871.5f, -419f, 0),
        new Vector3(-871.5f, 418.5f, 0),
        new Vector3(871.5f, 418.5f, 0),
        new Vector3(871.5f, -419f, 0)
    });

    void Start()
    {
        
        NeatSupervisor neatSupervisor = GetComponent<NeatSupervisor>();
        //Transform parent = GameObject.Find("NeatSupervisor").transform;
        //Transform parent = this.gameObject.transform;
        Transform parent = GameObject.Find("MainBoard").transform;
        if (neatSupervisor != null)
        {
          
            //neatSupervisor.RunBest();
            for (int i = 1; i < 6; i++)
            {
                switch (i)
                {
                    case 1:
                        Debug.Log("Jogador LA");
                        createPlayer(playerLA, parent);
                        break;
                    case 2:
                        Debug.Log("Jogador TA");
                        createPlayer(playerTA, parent);
                        break;
                    case 3:
                        Debug.Log("Jogador TP");
                        createPlayer(playerTP, parent);
                        break;
                    case 4:
                        Debug.Log("Jogador LP");
                        createPlayer(playerLP, parent);
                        break;
                    case 5:
                        Debug.Log("Jogador Random");
                        createPlayer(playerRandom, parent);
                        break;

                }                
                
            }



        }
      
    }

    public void Initilaze()
    {   
        
        //NeatSupervisor neatSupervisor = GetComponent<NeatSupervisor>();
        //Transform parent = GameObject.Find("NeatSupervisor").transform;
        //Transform parent = this.gameObject.transform;
        Transform parent = GameObject.Find("MainBoard").transform;
       // if (neatSupervisor != null)
        //{

            //neatSupervisor.RunBest();
            for (int i = 1; i < 6; i++)
            {
                switch (i)
                {
                    case 1:
                       // Debug.Log("Jogador LA");
                        createPlayer(playerLA, parent);
                        break;
                    case 2:
                       // Debug.Log("Jogador TA");
                        createPlayer(playerTA, parent);
                        break;
                    case 3:
                       // Debug.Log("Jogador TP");
                        createPlayer(playerTP, parent);
                        break;
                    case 4:
                       // Debug.Log("Jogador LP");
                        createPlayer(playerLP, parent);
                        break;
                    case 5:
                       // Debug.Log("Jogador Random");
                        createPlayer(playerRandom, parent);
                        break;

                }

            }



        //}
    }

      

    private void createPlayer(GameObject prefab, Transform parent )
    {
        GameObject player = Instantiate(prefab);
        player.transform.SetParent(parent);

        

        FindSeatController findSeatController = player.GetComponent<FindSeatController>();
        if (findSeatController != null)
        {
          //  Debug.Log("Seat Controller não esta null");
        }

        TableFillerController tableFiller = GameObject.Find("TableFiller").GetComponent<TableFillerController>();
        if (tableFiller != null)
        {
          //  Debug.Log(" Table Filler ");
        }
        findSeatController.setTbc(tableFiller);
        //findSeatController.setTbc(GameObject.Find("TableFiller").GetComponent<TableFillerController>());

        SeatController table = findSeatController.getTbc().getTable();
        if (table != null)
        {
           // Debug.Log(table.name);
        }
        //findSeatController.seat(findSeatController.getTbc().getTable());
        findSeatController.seat(table);
    }
}
