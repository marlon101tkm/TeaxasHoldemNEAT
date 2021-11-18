using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeatController : MonoBehaviour
{
    private const int MAX_SLOTS = 6;
    private int SLOTS = 0;

    public int getSLOTS()
    {
        return this.SLOTS;
    }

    public void setSLOTS(int SLOTS)
    {
        this.SLOTS = SLOTS;
    }

    public bool hasSlots()
    {
        return SLOTS < 6;
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
