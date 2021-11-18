using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableProgress : MonoBehaviour
{
    private bool trialEnded;

    private string tableName;

    private bool canPlayNextHand;

    public bool CanPlayNextHand
    {
        get { return canPlayNextHand; }
        set {  canPlayNextHand = value ; }
    }


    public string Name
    {
        get { return tableName; }
        set { tableName = value; }
    }

    public bool TrialEnded
    {
        get { return trialEnded; }
        set { trialEnded = value; }

    }

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
