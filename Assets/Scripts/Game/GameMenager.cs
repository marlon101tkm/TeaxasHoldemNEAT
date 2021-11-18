using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PokerCats;
using System.IO;




public class GameMenager : SingletonMonoBehaviour<GameMenager>
{
    List<TableProgress> tableProgress = new List<TableProgress>();


    [SerializeField]
    public string TxtFileName = " file.txt ";


    private bool neatTrialInprogress = true;

    public bool NeatTrialInprogress
    {
        get { return neatTrialInprogress; }
        set { neatTrialInprogress = value; }
    }

    private bool allUnitsAreActive = false;

    public bool AllUnitsAreActtive
    {
        get { return allUnitsAreActive; }
        set { allUnitsAreActive = value; }
    }





    private bool allAifinished = false;

    public bool AllAiFinished
    {
        get { return allAifinished; }
        set { allAifinished = value ; }
    }

    //private bool canPlayNextHand = false;

    //public bool CanPlayNextHand
    //{
    //    get { return canPlayNextHand; }
    //    set { canPlayNextHand = value; }
    //}


    private int _currentTrial = 0;
    public int CurrentTrial
    {
        get { return _currentTrial; }
        set { _currentTrial = value; }
    }


    private int _currentGeneration = 0;
    public int CurrentGeneration
    {
        get { return _currentGeneration; }
        set { _currentGeneration = value; }
    }



    public void CreateText(string caminho,string content,string firstLine )
    {
        string path = Application.dataPath + "/" + caminho;
            
        if (!File.Exists(path))
        {
            File.WriteAllText(path, firstLine+"\n");
        }
        

        File.AppendAllText(path,content);
    }

    public void ReadText(string caminho, string data)
    {
        string path = Application.dataPath + "/" + caminho;

        if (!File.Exists(path))
        {
            File.WriteAllText(path, "Rodada\n\n");
        }
        StreamReader reader = new StreamReader(path);
        string linha,chave,valor;
        char virgula = ',';
        while (reader.EndOfStream)
        {
             linha = reader.ReadLine();

            chave = linha.TrimStart(virgula);
            valor = linha.TrimEnd(virgula);

        }

        reader.Close();
        
    }


    public bool getIfTrialEnded()
    {

        
        //Debug.Log("Get if trial ended");
        foreach (TableProgress tableprog in tableProgress)
        {
            //Debug.Log(tableprog.Name + "  "+ tableprog.TrialEnded );
            if (tableprog.TrialEnded)
            {
                //Debug.Log(tableprog.Name + ": true");
                return true;
            }
        }
        //canPlayNextHand = true;
        // NeatTrialInprogress = false;
        //printLog("false");
        return false;
    }


    public bool getIfCanPlayHand()
    {
        //Debug.Log("Get if Can play hand");
        foreach (TableProgress tableprog in tableProgress)
        {
           // Debug.Log(tableprog.Name + "  " + tableprog.CanPlayNextHand);
            if (!tableprog.CanPlayNextHand)
            {
                //printLog(tableprog.Name + "true");
                return false;
            }
        }
        // NeatTrialInprogress = false;
        //printLog("false");
        return true;
    }


    public void addTables(List<SeatController> tables)
    {
        foreach (SeatController table in tables)
        {
            addTable(table.name);
        }
    }


    
    

    public void addTable(string tableName)
    {
        GameObject obj = new GameObject();
        
        TableProgress newTable = obj.AddComponent<TableProgress>();
        newTable.TrialEnded = true;
        newTable.Name = tableName;
        tableProgress.Add(newTable);
    }

    public void setCanPlayHand(string tableName, bool valor)
    {
        foreach (TableProgress table in tableProgress)
        {
            if (table.Name.Equals(tableName))
            {
                table.CanPlayNextHand = valor;
            }
        }
    }

    public void setTrialEnded(string tableName, bool valor)
    {
        foreach (TableProgress table in tableProgress)
        {
            if (table.Name.Equals(tableName))
            {
                table.TrialEnded = valor;
            }
        }
    }

    public bool getTrialEnded(string tableName)
    {
        foreach (var table in tableProgress)
        {
            if (table.Name.Equals(tableName))
            {
                return table.TrialEnded;
            }
        }

        return false;
    }

    public void SetCanPlayHand()
    {
        //printLog("Set ALL");
        foreach (TableProgress tab in tableProgress)
        {
            tab.CanPlayNextHand = false;
            //printLog(tab.trialEnded);
        }

    }

    public void SetAllTrialEndedFalse()
    {
        foreach (TableProgress tab in tableProgress)
        {
            tab.TrialEnded = false;
            //printLog(tab.trialEnded);
        }
    }

    public void SetAllTrialEnded()
    {
        //printLog("Set ALL");
        foreach (TableProgress tab in tableProgress)
        {
            tab.TrialEnded = true;
            //printLog(tab.trialEnded);
        }
        
        
    }

    public void atualizaNeatTrialInprogress()
    {
         NeatTrialInprogress = getIfTrialEnded();
    }
    // Start is called before the first frame update
    void Start()
    {
        // SetAllTrialEnded();
        //CreateText();
    }

    // Update is called once per frame
    void Update()
    {
       // atualizaNeatTrialInprogress();
    }
}
