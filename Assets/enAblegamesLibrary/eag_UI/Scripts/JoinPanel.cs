using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.EventSystems;
using TMPro;

public class JoinPanel : MonoBehaviour
{
    private bool pointerOn = false;
    
    private Animator _animator;
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
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
    
}
