using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public BackgroundScroll background_scroll;
    [SerializeField] Image stability_bar;
    float stability = 100f;
    public GameObject player;
    public GameObject particles;
    public bool play_game = false;
    public EndSequence end_sequence;
    [SerializeField] Color transparent;
    [SerializeField] TextMeshProUGUI text1;
    [SerializeField] TextMeshProUGUI text2;

    float fade_timer = 1;
    float fade_time = 1f;

    [SerializeField] GameObject retry_button;

    float player_speed = 3f;
    [SerializeField] TMP_Text speed_text;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        speed_text.text = "speed\n" + player_speed.ToString("f") + " m/s";
        particles.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (play_game)
        {
            player_speed += .005f;
            speed_text.text = "speed\n" + player_speed.ToString("f") + " m/s";
            background_scroll.SetSpeed(player_speed / 3);

            if (player.transform.position.y > 5.5)
            {
                particles.SetActive(true);
                ChangeStability(-.3f);
            }
            else
                particles.SetActive(false);


            if (player.transform.position.y < -6f)
            {
                player.SetActive(false);
                play_game = false;
                background_scroll.SetSpeed(0f);
                StartCoroutine(UIFade());
            }


            stability -= Time.deltaTime * 2f;
            stability_bar.fillAmount = Mathf.Clamp(stability, 0f, 100f) / 100f;
            if (stability <= 0f)
            {
                player.GetComponent<Rigidbody2D>().gravityScale = .5f;
            }
        }
    }

    IEnumerator UIFade()
    {
        yield return new WaitForSeconds(1f);
        end_sequence.Sequence();
        yield return new WaitForSeconds(0.5f);

        while (fade_timer < 2f) {
            fade_timer += Time.deltaTime;
            text1.color = Color.Lerp(text1.color, transparent, Time.deltaTime * fade_time);
            yield return null; 
        }

        while (fade_timer < 4f)
        {
            fade_timer += Time.deltaTime;
            text2.color = Color.Lerp(text2.color, transparent, Time.deltaTime * fade_time);
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        retry_button.SetActive(true);
    }

    public void SetPlayerSpeed(float value)
    {
        player_speed += value;
        if (player_speed < 3f)
            player_speed = 3f;
    }

    public void ChangeStability(float amount)
    {
        stability += amount;
        if (stability > 100f)
            stability = 100f;
        stability_bar.fillAmount = Mathf.Clamp(stability, 0f, 100f) / 100f;
    }
    
    public void startGame()
    {
        play_game = true;
    }
}
