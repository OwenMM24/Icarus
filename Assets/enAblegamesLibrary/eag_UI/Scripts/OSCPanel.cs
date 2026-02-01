using System.Collections;
using System.Collections.Generic;
using System.Net;
using extOSC;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OSCPanel : MonoBehaviour
{
    [SerializeField] private Image indicator;
    [SerializeField] private Image indicator1;
    [SerializeField] private TMP_Text ip;
    
    [SerializeField] private Transform OSCTransmittorHolder;

    [SerializeField] private OSCReceiver _oscReceiver;

    private bool opened = false;

    [SerializeField] private Animator _animatior;
    // Start is called before the first frame update
    void Start()
    {
        if (!_animatior)
        {
            _animatior = GetComponent<Animator>();
        }
        ip.text = _oscReceiver.LocalHost;
    }

    // Update is called once per frame
    void Update()
    {
        if (OSCTransmittorHolder.childCount == 0)
        {
            indicator.color = Color.red;
            indicator1.color = Color.red;
        }
        else
        {
            indicator.color = Color.green;
            indicator1.color = Color.green;
        }
    }

    public void ToggleOpen()
    {
        if (opened)
        {
            _animatior.Play("Closed");
        }
        else
        {
            _animatior.Play("Opened");
        }

        opened = !opened;
    }
    string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }

        return "127.0.0.1";
    }
}
