using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardBehavior : MonoBehaviour
{
    float speed, x_pos, x_scale, y_scale;
    public enum Type { slow, drain, speed, repair }
    float change_speed, change_stability = 0f;
    GameManager game_manager;

    // Start is called before the first frame update
    void Awake()
    {
        game_manager = GameObject.Find("GameManager").GetComponent<GameManager>();

        speed = Random.Range(3f, 5.5f);
        x_scale = Random.Range(6f, 12f);
        y_scale = Random.Range(3f, 6f);

        Type obstacle_type;
        if (gameObject.name == "SlowDown(Clone)") {
            obstacle_type = Type.slow;
            change_speed = -.02f;
        }
        else if(gameObject.name == "WingDrain(Clone)")
        {
            obstacle_type = Type.drain;
            change_stability = -.03f;
        }
        else if (gameObject.name == "SpeedUp(Clone)")
        {
            obstacle_type = Type.speed;
            change_speed = .02f;
        }
        else
        {
            obstacle_type = Type.repair;
            change_stability = .03f;
        }

        transform.localScale = new Vector2(x_scale, y_scale);
        if (obstacle_type == Type.slow || obstacle_type == Type.drain) {
            speed = -speed; x_pos = transform.position.x;
        }
        else
        {
            x_pos = -19f;
        }

    }

    // Update is called once per frame
    void Update()
    {
        x_pos += Time.deltaTime * speed;
        transform.position = new Vector2(x_pos, transform.position.y);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        StartCoroutine(HazardEffects());
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        StopAllCoroutines();
    }

    IEnumerator HazardEffects()
    {

        while (true)
        {
            game_manager.SetPlayerSpeed(change_speed);
            game_manager.ChangeStability(change_stability);
            yield return null;
        }
    }

}
