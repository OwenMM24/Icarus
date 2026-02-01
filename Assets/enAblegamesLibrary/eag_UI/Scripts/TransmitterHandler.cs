using System.Collections;
using System.Collections.Generic;
using extOSC;
using TMPro;
using UnityEngine;

public class TransmitterHandler : MonoBehaviour
{
    public string ID = "Default User";
    public string IP = "127.0.0.1";

    public OSCTransmitter OscTransmitter;
    
    [SerializeField] private TMP_Text idLable;
    [SerializeField] private TMP_Text ipLable;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Init()
    {
        IP = OscTransmitter.RemoteHost;
        ipLable.text = IP;
    }

    public void Disconnect()
    {
        Destroy(gameObject);
    }
}
