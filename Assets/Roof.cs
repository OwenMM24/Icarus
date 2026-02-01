using UnityEngine;

public class Roof : MonoBehaviour
{

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("player hit haahahahahahaahahahahaahahah");

        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("player hit haahahahahahaahahahahaahahah");
            collision.gameObject.GetComponent<NewPlayerMove>().RoofBounce();
        }
    }
}
