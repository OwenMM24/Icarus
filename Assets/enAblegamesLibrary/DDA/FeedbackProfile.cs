using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackProfile : MonoBehaviour
{
    public enum MusicTolerance
    {
        Full,
        High,
        Medium,
        Low
    }

    public enum TextCognitive
    {
        Full,
        High,
        Medium,
        Low
    }
    
    public enum ColorTolerance
    {
        Full,
        High,
        Medium,
        Low
    }

    public MusicTolerance musicTolerance;
    public TextCognitive textCognitive;
    public ColorTolerance colorTolerance;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
