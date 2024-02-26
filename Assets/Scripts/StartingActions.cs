using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class StartingActions : MonoBehaviour
{
    bool start_sequence = false;
    float sequence_time = 0f;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            start_sequence = true;
        }
    }

    void FixedUpdate()
    {
        sequence_time += Time.deltaTime;
        if(sequence_time > 3f)
        {
            rb.AddForce(new Vector2(0f, 3f), ForceMode2D.Impulse);
            enabled = false;
        }

    }

}
