using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

public class StartingActions : MonoBehaviour
{
    bool start_sequence, controls_availible, jumpped = false;

    public HazardSpawner hazard_spawner;
    public PlayerMovement playerMovement;
    public GameManager gameManager;

    float sequence_time = 0f;
    Rigidbody2D rb;
    [SerializeField] GameObject start_text, start_platform, player;
    Vector3 platform_start_pos;

    public BackgroundScroll background_scroll;




    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        platform_start_pos = start_platform.transform.position;
    }

/*    // Update is called once per frame
    void Update()
    {

    }*/

    void Update()
    {
        //pressing space to start
        if (Input.GetKeyDown("space"))
        {
            start_sequence = true;
        }

        //start sequence is the little bit of character running and jumpping at start
        if (start_sequence)
        {
            //sequence time acts as a world clock starting after space is pressed
            sequence_time += Time.deltaTime;
            start_text.SetActive(false);

            //moving the platform to the left and starting background scroll
            start_platform.transform.position = platform_start_pos + new Vector3 (-sequence_time * 5f, 0, 0);
            background_scroll.SetSpeed(1.6f);

            //this is when the character auto jumps at edge
            if (sequence_time > 1.5f)
            {
                if (!jumpped)
                {
                    player.GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, 3.6f), ForceMode2D.Impulse);
                    hazard_spawner.enabled = true;
                    jumpped = true;
                    playerMovement.enabled = true;
                    gameManager.startGame();
                }
            }

            //this turns on player movement and turns off this script as well as deletes the platform
            if (sequence_time > 3f)
            {
                Destroy(start_platform);
                enabled = false;
            }
        }
    }

}
