using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] Image stability_bar;
    float stability = 100f;
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        stability -= Time.deltaTime * 3f;
        stability_bar.fillAmount = Mathf.Clamp(stability, 0f, 100f) / 100f;
        if (stability <= 0f)
        {
            player.GetComponent<Rigidbody2D>().gravityScale = .5f;
        }
    }

    public void changeStability(float amount)
    {
        stability += amount;
        stability_bar.fillAmount = Mathf.Clamp(stability, 0f, 100f) / 100f;
    }


}
