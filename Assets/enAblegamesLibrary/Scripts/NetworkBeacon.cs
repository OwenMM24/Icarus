using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using extOSC;
using NetworkDiscoveryUnity;

public class NetworkBeacon : MonoBehaviour
{
    private const string _LAST_USED_USER_NAME = "lastUsedUserName";
    public static NetworkBeacon Instance;
    public string beaconName;
    public OSCReceiver oscReceiver;
    [SerializeField] private int port;

    public NetworkDiscovery _beacon;
    
    // Start is called before the first frame update
    private void Awake()
    {
        if (!(Instance is null) &&  Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        SetDiscoverable();
        _beacon.EnsureServerIsInitialized();
        _beacon.RegisterResponseData("Name", PlayerPrefs.GetString(_LAST_USED_USER_NAME));
        oscReceiver.Bind("/pause", OnReceivePause);
        oscReceiver.Bind("/resume", OnReceiveResume);
        //osc.SetAllMessageHandler(OnReceivePause);
    }

    void OnReceivePause(OSCMessage message)
    {
        print("Pause Received");
        try
        {
            FindObjectOfType<egUIManager>().PauseGame();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public void SetDiscoverable()
    {
        _beacon.EnsureServerIsInitialized();
    }

    public void OnReceiveResume(OSCMessage message)
    {
        print("UnPause Received");
        try
        {
            FindObjectOfType<egUIManager>().UnPauseGame();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private void OnDestroy()
    {
        _beacon.CloseServerUdpClient();
    }
}