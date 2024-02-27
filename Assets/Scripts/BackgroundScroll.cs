using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    
    [SerializeField] float speed = 1f;
    Material material;
    float offset;

    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<Renderer>().material;    
    }

    // Update is called once per frame
    void Update()
    {
        offset += ((Time.deltaTime / 10) * speed);
        material.SetTextureOffset("_MainTex", new Vector2(offset, 0));

    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
    public float GetSpeed()
    {
        return speed;
    }

}
