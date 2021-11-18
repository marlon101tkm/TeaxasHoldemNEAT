using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PokerCats;
using UnitySharpNEAT;

public class TableFillerController : MonoBehaviour
{
    public int TABLES_START_AMOUNT = 100;
    public int ADD_TABLES_AMOUNT = 25;
    public int minTableAmount = 50;
    public int currentTables = 0;
    public int tableCounter = 0;
    public GameObject tablePrefab;
    public List<SeatController> tables = new List<SeatController>();
    private Transform parent;
    private string sceneName;
    public GameObject _neatSupervisor;
    private NeatSupervisor neatSupervisor;
    public bool debugFlag = false;

    public List<SeatController> getTables()
    {
        return this.tables;
    }

    public void setTables(List<SeatController> tables)
    {
        this.tables = tables;
    }


    public int getCurrentTables()
    {
        return this.currentTables;
    }

    public void setCurrentTables(int currentTables)
    {
        this.currentTables = currentTables;
    }

    // Start is called before the first frame update
    void Start()
    {
        //new ANNInprogress();
       neatSupervisor = _neatSupervisor.GetComponent<NeatSupervisor>();
        sceneName = SceneManager.GetActiveScene().name;
        parent = GameObject.Find("MainBoard").transform;

        //Debug.Log("iniciou table filler");
        for (int i = 0; i < TABLES_START_AMOUNT; i++)
        {
            setCurrentTables(getCurrentTables() + 1);
            tableCounter++;
            GameObject table = Instantiate(tablePrefab, tablePrefab.transform.position, tablePrefab.transform.rotation);
            table.transform.SetParent(parent);
            table.name = "Table " + tableCounter;
            table.GetComponent<GameController>().debug = debugFlag;
            if (_neatSupervisor != null)
            {
                table.GetComponent<GameController>().neatSupervisor = neatSupervisor;
            }

            // ANNInprogress.Instance.addTable(table.name);
            // GameMenager.Instance.addTable(table.name);
            tables.Add(table.GetComponent<SeatController>());
        }

        if (sceneName == "TESTS")
        {
            PlayerFactory playFac = this.GetComponent<PlayerFactory>();
            playFac.Initilaze();
            //GameMenager.Instance.addTables(tables);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (sceneName.Equals("NEAT"))
        {
            if (getCurrentTables() < minTableAmount)
            {
                addTables();
            }
        }
        

        // InvokeRepeating("addTables", 2.0f * Time.timeScale / 100, 4.0f * Time.timeScale / 100);
    }

    public GameObject getTableByName(string name)
    {
        foreach (var table in tables)
        {
            if (table.name.Equals(name))
            {
                
                return table.gameObject;
            }

        }
        return null;
    }

    public bool countChildrenComponent(SeatController table)
    {
        return 7 == table.gameObject.transform.childCount;
        
        
    }

    public bool waitOtherTabbles()
    {
        GameController temp;
        foreach(SeatController table in tables)
        {
            if (!table.hasSlots())
            {
                temp = table.GetComponent<GameController>();
                if (!temp.CanPlayHand)
                {
                    return false;
                }
            }
          

        }
        return true;
    }


    public SeatController getTable()
    {
        //Debug.Log(" Mesas "+ tables.Count);
        foreach (var table in tables)
        {
            //Debug.Log(" Mesa tem Slots? "+table.hasSlots());
            if (table.hasSlots())
            {
                
                table.setSLOTS(table.getSLOTS() + 1);
                return table;
            }
            //else if (countChildrenComponent(table))
            //{
            //    table.setSLOTS(1);
            //    return table;
            //}

        }
        return null;
    }

    public void realocatePlayers(List<Player> players)
    {
        foreach (var player in players)
        {
            player.GetComponent<FindSeatController>().seat(getTable());
        }
    }

    public void addTable()
    {
       
        setCurrentTables(getCurrentTables() + 1);
        tableCounter++;
        GameObject table = Instantiate(tablePrefab, tablePrefab.transform.position, tablePrefab.transform.rotation);
        table.transform.SetParent(parent);
        tables.Add(table.GetComponent<SeatController>());
        table.name = "Table " + tableCounter;
        table.GetComponent<GameController>().debug = debugFlag;
        if (neatSupervisor != null)
        {
            table.GetComponent<GameController>().neatSupervisor = neatSupervisor;
        }
    }

    public void addTables()
    {
        //NeatSupervisor nEatSupervisor = neatSupervisor.GetComponent<NeatSupervisor>();
        for (int i = 0; i < ADD_TABLES_AMOUNT; i++)
        {
            setCurrentTables(getCurrentTables() + 1);
            tableCounter++;
            GameObject table = Instantiate(tablePrefab, tablePrefab.transform.position, tablePrefab.transform.rotation);
            table.transform.SetParent(parent);
            tables.Add(table.GetComponent<SeatController>());
            table.name = "Table " + tableCounter;
            table.GetComponent<GameController>().debug = debugFlag;
            if (neatSupervisor != null)
            {
                table.GetComponent<GameController>().neatSupervisor = neatSupervisor;
            }
        }
    }
}
