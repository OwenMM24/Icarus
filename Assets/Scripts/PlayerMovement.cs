using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public BackgroundScroll background_scroll;
    public GameManager gameManager;
    float background_speed;

    Rigidbody2D rb;
    float dive_force = -0.3f;
    float dive_time = 0f;
    bool level_peak = false;

    float start_peak_vector;
    bool first_time = true;

    float input_lockout_time, input_lockout_timer = .5f;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        background_speed = background_scroll.GetSpeed();
    }

    // Update is called once per frame

    private void Update()
    {
        //when space is pressed down is resets variables
        if (Input.GetKeyDown("space") && input_lockout_time >= input_lockout_timer)
        {
            dive_time = 0f;
            level_peak = false;
            rb.velocity = Vector3.zero;
            first_time = true;
        }
        if (Input.GetKeyUp("space"))
            input_lockout_time = 0f;
        
    }

    void FixedUpdate()
    {

        input_lockout_time += Time.deltaTime;

        //while space is being held down character accelerates downwards and counts how long its happening for
        if (Input.GetKey("space") && input_lockout_time >= input_lockout_timer)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + dive_force);
            dive_time += Time.deltaTime;
            background_speed += .01f;
            background_scroll.SetSpeed(background_speed);
            gameManager.changeStability(-.1f);
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
