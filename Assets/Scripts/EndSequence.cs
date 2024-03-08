using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndSequence : MonoBehaviour
{
    public BackgroundScroll background_scroll;

    bool first, plat_appear, lost = false;
    float lost_timer, scroll_speed = 0f;
    [SerializeField] GameObject platform;

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!first && lost)
        {
            first = true;
            StartCoroutine(PlatAppear());
        }

        if (lost) {
            lost_timer += Time.deltaTime;
            if (scroll_speed >= 0)
                scroll_speed = Mathf.Sin(lost_timer*1.5f);
            else { lost = false; scroll_speed = 0f; plat_appear = true; }
            background_scroll.SetSpeed(scroll_speed * 100f);
        }
    }



    IEnumerator PlatAppear()
    {
        yield return new WaitForSeconds(1.27f);  
        Instantiate(platform, new Vector3(16f, -4.5f, 0f), Quaternion.identity);
        yield return null;
    }

    public void Sequence()
    {
        lost = true;
    }

}
