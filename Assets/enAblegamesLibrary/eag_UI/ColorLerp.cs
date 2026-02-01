using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Enablegames.Suki;


public class ColorLerp : MonoBehaviour
{
    public Color startColor;
    public Color endColor;
    public float speed = 1.0f;
    public float waitTime = 1.0f;

    private Image image;
    public float time = 0f;

    float floatTimer = 5;

    bool isConnected = false;

    /*
    //PING AND FADE.
    bool startTimer = false;

    void Start()
    {
        image = GetComponent<Image>();
        StartCoroutine(StartLerp());
    }

    IEnumerator StartLerp()
    {
        startTimer = true;
        yield return new WaitForSeconds(waitTime);
        startTimer = false;
        time = 1;
        image.color = endColor;
    }

    void Update()
    {
        if (time > 0)
        {
            time -= Time.unscaledDeltaTime * speed;

            // Lerping between the startColor and endColor based on sine wave
            Color lerpedColor = Color.Lerp(endColor, startColor, time);

            // Applying the lerped color to the Image component
            image.color = lerpedColor;
        }
        else if(!startTimer)
        {
            StartCoroutine(StartLerp());
        }
    }
    */

    //SIMPLE SINE WAVE.
    void Start()
    {
        image = GetComponent<Image>();
    }

    void Update()
    {
        time += Time.unscaledDeltaTime * speed;
        float t = Mathf.Sin(time);

        if (isConnected)
        {
            // Lerping between the startColor and endColor based on sine wave
            Color lerpedColor = Color.Lerp(endColor, startColor, (t + 1) / 2);

            // Applying the lerped color to the Image component
            image.color = lerpedColor;
        }
        else
        {
            image.color = startColor;
        }


        //OSC RECIEVER CHECK FOR PACKAGES. ALSO, THIS SHOULD BE USED FOR GETTING SUKI DATA IF GAME IS PAUSED.s
        if (floatTimer <= 0)
        {
            //GetSukiData();
            //print("updating?? " + SukiInput.Instance.Updating);
            isConnected = SukiInput.Instance.Updating;
            floatTimer = 50;
        }
        else
        {
            floatTimer -= 0.1f;
        }
    }

    public void ResetValues()
    {
        time = 0;
        image.color = startColor;
    }
}