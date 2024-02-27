using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{


    Rigidbody2D rb;
    float dive_force = -0.3f;
    float dive_time = 0f;
    bool level_peak = false;

    float start_peak_vector;
    bool first_time = true;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame

    private void Update()
    {

        //when space is pressed down is resets variables
        if (Input.GetKeyDown("space"))
        {
            dive_time = 0f;
            level_peak = false;
            rb.velocity = Vector3.zero;
        }
    }

    void FixedUpdate()
    {
        //while space is being held down character accelerates downwards and counts how long its happening for
        if (Input.GetKey("space"))
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + dive_force);
            dive_time += Time.deltaTime;
        }

        //on release space the player shoots back up for the amount of time space was held down for
        if(!Input.GetKey("space") && dive_time > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + -dive_force * 2f);
            dive_time -= Time.deltaTime;
            //instead of instantly stopping at peak, this starts smoothing it so it looks nicer 
            if (dive_time <= 0f)
                level_peak = true;
        }

        if(level_peak == true)
        {
            //makes sure start_peak_vector isnt assigned to the velocity anymore than just the first time this is hit
            if (first_time)
            {
                start_peak_vector = rb.velocity.y;
                first_time = false;
            }
            //proccess of smoothly making y velocity to 0
            if (start_peak_vector > 0f)
            {
                rb.velocity = new Vector3(rb.velocity.x, start_peak_vector);
                start_peak_vector -= Time.deltaTime * 5f;
                //Debug.Log(start_peak_vector);
            }
            //finished, reset values
            else
            {
                level_peak = false;
                rb.velocity = Vector3.zero;
                first_time = true;
            }
        }
    }
}
