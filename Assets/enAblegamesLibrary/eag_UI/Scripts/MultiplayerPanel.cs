using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerPanel : MonoBehaviour
{
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void Open()
    {
        _animator.SetBool("Opened", true);
    }
    public void Close()
    {
        _animator.SetBool("Opened", false);
    }
}
