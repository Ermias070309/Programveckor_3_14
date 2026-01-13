using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class ElectricityManager : MonoBehaviour
{
    public static ElectricityManager instance;

    public int totalWires = 7;
    private int connectedWires = 0;

    public TextMeshProUGUI messageText;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        messageText.gameObject.SetActive(false);
    }

    public void WireConnected()
    {
        connectedWires++;

        if (connectedWires >= totalWires)
        {
            ElectricityRestored();
        }
    }

    void ElectricityRestored()
    {
        messageText.text = "Electricity restored";
        messageText.gameObject.SetActive(true);
    }
}