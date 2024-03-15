using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinPlat : MonoBehaviour
{
   /* GameManager gameManager;
    PlayerMovement playerMovement;
    BackgroundScroll backgroundScroll;
    GameObject player;
    bool rot = false;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();
        backgroundScroll = GameObject.Find("Background").GetComponent<BackgroundScroll>();
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Awake()
    {
        StartCoroutine(SlidePlat());
    }

    void OnCollisionEnter2D(Collision2D collider)
    {
        gameManager.startWin();
        backgroundScroll.enabled = false;
        playerMovement.enabled = false;
        rot = true;
    }

    IEnumerator SlidePlat()
    {
        float temp_x = transform.position.x;
        while (true)
        {
            if(rot)
                player.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            player.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            transform.position = new Vector3(temp_x -= (Time.deltaTime *16), transform.position.y, 0);
            yield return null;
        }
    }
*/


}
