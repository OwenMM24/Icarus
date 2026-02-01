using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] float coinMoveSpeed;

    void Update()
    {
        transform.position = new Vector3(transform.position.x + (Time.deltaTime * coinMoveSpeed), transform.position.y, transform.position.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<NewPlayerMove>().AddCoin();
            Destroy(gameObject);
        }
    }

}
