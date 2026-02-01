using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

public class HostPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ipText;
    private Animator _animator;
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        GetLocalIPAddress();
    }

    // Update is called once per frame
    void Update() 
    {
        
    }

    public void Open()
    {
        transform.SetSiblingIndex(-1);
        _animator.SetBool("Opened", true);
    }
    
    public void Close()
    {
        _animator.SetBool("Opened", false);
    }
    
    public string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                ipText.text = ip.ToString();
            }
        }
        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }
}
