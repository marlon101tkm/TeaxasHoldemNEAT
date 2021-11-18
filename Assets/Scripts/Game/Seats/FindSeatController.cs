using System.Collections;
using System.Collections.Generic;
using SharpNeat.Phenomes;
using UnityEngine;
using UnitySharpNEAT;
using PokerCats;

public class FindSeatController : MonoBehaviour
{
    // private List<Vector3> positions = new List<Vector3>();
    private static List<Vector3> positions = new List<Vector3>(new Vector3[] {
        new Vector3(-18f, -171f, 0),
        new Vector3(-274f, -50f, 0),
        new Vector3(-202f, 100f, 0),
        new Vector3(0f, 149.9f, 0),
        new Vector3(206f, 100f, 0),
        new Vector3(261f, -50f, 0)
    });
    private TableFillerController tbc;

    void Start()
    {
        // tbc = GameObject.Find("TableFiller").GetComponent<TableFillerController>();
        // setTbc(GameObject.Find("TableFiller").GetComponent<TableFillerController>());
        // SeatController table = tbc.getTable();
        // seat(table);
    }

    public void seat(SeatController table)
    {
        if (table != null)
        {
            
            transform.SetParent(table.transform);
            name = "Player" + table.getSLOTS();
            transform.localPosition = positions[table.getSLOTS() - 1];
           // GetComponent<PlayerBase>().dialogBox = transform.parent.gameObject.FindComponentInChildWithTag<Transform>("dialog").gameObject.transform.GetChild(table.getSLOTS() - 1).gameObject;

            if (table.getSLOTS() > 5)
            {
                //Debug.Log("Completou "+ table.name );
                table.GetComponent<GameController>().PrepareGame();
            }
        }
    }

    public TableFillerController getTbc()
    {
        return this.tbc;
    }

    public void setTbc(TableFillerController tbc)
    {
        this.tbc = tbc;
    }
}
