using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite[] sprites;
    private int spriteIndex;

    public BackgroundScroll background_scroll;
    public GameManager gameManager;
    float background_speed;

    Rigidbody2D rb;
    float dive_force = -0.3f;
    float dive_time = 0f;
   // bool level_peak = false;

    float character_z_rot = 0f;


    float start_peak_vector;
  //  bool first_time = true;

    float input_lockout_time, input_lockout_timer = .3f;

    //Player audio varibles
    public AudioSource flap;

    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        background_speed = background_scroll.GetSpeed();
    }

    private void Update()
    {
        //when space is pressed down is resets variables
        if (Input.GetKeyDown("space") && input_lockout_time >= input_lockout_timer)
        {
            dive_time = 0f;
            rb.velocity = Vector3.zero;
            spriteRenderer.sprite = sprites[1];
        }
        else if (Input.GetKeyUp("space") && input_lockout_time >= input_lockout_timer)
        {
            input_lockout_time = 0f;
            spriteRenderer.sprite = sprites[0];
        }
    }


    void FixedUpdate()
    {
        input_lockout_time += Time.deltaTime;

        if (character_z_rot > 20f)
            character_z_rot = 20f;
        else if (character_z_rot < -20f)
            character_z_rot = -20f;

        if (dive_time <= 0f && character_z_rot > 0)
            character_z_rot -= Time.deltaTime * 10;
        else if (dive_time <= 0f && character_z_rot < 0)
            character_z_rot += Time.deltaTime * 10;

        transform.rotation = Quaternion.Euler(0f, 0f, character_z_rot);





        //while space is being held down character accelerates downwards and counts how long its happening for
        if (Input.GetKey("space") && input_lockout_time >= input_lockout_timer)
        {

            if (character_z_rot > 0f)
                character_z_rot -= Time.deltaTime * 100;
            else
                character_z_rot -= Time.deltaTime * 50;

            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + dive_force);
            dive_time += Time.deltaTime;

        }


        //on release space the player shoots back up for the amount of time space was held down for
        if (!Input.GetKey("space") && dive_time > 0f)
        {
            if (character_z_rot < 0f)
                character_z_rot += Time.deltaTime * 100;
            else
                character_z_rot += Time.deltaTime * 50;



            rb.gravityScale = .65f;

            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + -dive_force *2f);
            dive_time -= Time.deltaTime;

        }

        if (dive_time <= 0f)
        {
            rb.gravityScale = .1f;
        }
    }

    private void Animatesprite()
    {
        spriteIndex++;

        if (spriteIndex >= sprites.Length)
        {
            spriteIndex = 0;
        }

        spriteRenderer.sprite = sprites[spriteIndex];
    }
}
