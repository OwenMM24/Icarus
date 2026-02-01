using System;
using System.Collections;
using System.Collections.Generic;
using extOSC;
using TMPro;
using UnityEngine;
using Michsky.MUIP;

public class ManualOSC : MonoBehaviour
{
    [SerializeField] private TMP_InputField ipField;

    [SerializeField] private SwitchManager portSwitch;

    [SerializeField] private GameObject transmitterObject;

    [SerializeField] private Transform transmitterHolder;

    private void Start()
    {
        string previousIP = PlayerPrefs.GetString("lastIP", "0.0.0.0");
        if (previousIP != "0.0.0.0")
        {
            ipField.text = previousIP;
        }
    }

    public void ManualConnection()
    {
        var newTransmitter = Instantiate(transmitterObject, transmitterHolder).GetComponent<TransmitterHandler>();
        newTransmitter.OscTransmitter.RemoteHost = ipField.text;
        newTransmitter.OscTransmitter.RemotePort = portSwitch.isOn ? 7778 : 7779;
        newTransmitter.Init();
        PlayerPrefs.SetString("lastIP", ipField.text);
    }
}
