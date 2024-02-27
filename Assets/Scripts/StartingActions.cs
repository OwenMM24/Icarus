using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

public class StartingActions : MonoBehaviour
{
    bool start_sequence, controls_availible, jumpped = false;

    public PlayerMovement playerMovement;

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

        if (start_sequence)
        {
            sequence_time += Time.deltaTime;
            start_text.SetActive(false);
            start_platform.transform.position = platform_start_pos + new Vector3 (-sequence_time * 2.3f, 0, 0);
            background_scroll.SetSpeed(.25f);

            if (sequence_time > 3f)
            {
                if (!jumpped)
                {
                    player.GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, 3.6f), ForceMode2D.Impulse);
                    jumpped = true;
                    playerMovement.enabled = true;
                }
            }
            if (sequence_time > 6f)
            {
                Debug.Log("hit1");
                Destroy(start_platform);
                Debug.Log("hit2");
                enabled = false;
            }
        }
    }

}
