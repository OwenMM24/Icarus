using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] Image stability_bar;
    float stability = 100f;
    public GameObject player;
    public GameObject particles;
    bool play_game = false;
    public EndSequence end_sequence;
    [SerializeField] Color transparent;
    [SerializeField] TextMeshProUGUI text1;
    [SerializeField] TextMeshProUGUI text2;

    float fade_timer = 1;
    float fade_time = 1f;

    [SerializeField] GameObject retry_button;

    // Start is called before the first frame update
    void Start()
    {
        particles.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (play_game)
        {
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
                end_sequence.Sequence();
                StartCoroutine(UIFade());
            }


            stability -= Time.deltaTime * 3f;
            stability_bar.fillAmount = Mathf.Clamp(stability, 0f, 100f) / 100f;
            if (stability <= 0f)
            {
                player.GetComponent<Rigidbody2D>().gravityScale = .5f;
            }
        }
    }

    IEnumerator UIFade()
    {
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


    public void ChangeStability(float amount)
    {
        stability += amount;
        stability_bar.fillAmount = Mathf.Clamp(stability, 0f, 100f) / 100f;
    }
    
    public void startGame()
    {
        play_game = true;
    }
}
