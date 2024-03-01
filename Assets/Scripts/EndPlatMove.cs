using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPlatMove : MonoBehaviour
{

    private void Awake()
    {
        StartCoroutine(SlidePlat());
    }

    IEnumerator SlidePlat()
    {
        float temp_x = transform.position.x;
        while (transform.position.x > 2.5f)
        {
            transform.position = new Vector3(temp_x -= (Time.deltaTime * 16), transform.position.y, 0);
            yield return null;
        }
    }
}
