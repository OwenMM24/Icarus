using UnityEngine;
using Enablegames;

namespace SpaceShooterDemo
{
    public class SSBonus : MonoBehaviour
    {
        [SerializeField]
        private AudioClip pickUpSound;
        [SerializeField]
        private int scoreBonus = 1;

        public GameObject bonusVFX;

        //when colliding with another object, if another objct is 'Player', sending command to the 'Player'
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "Player")
            {
                SpaceShooterPlayer.instance.PlaySoundOneShot(pickUpSound);
                Instantiate(bonusVFX, collision.transform.position, Quaternion.identity);

                if (PlayerShooting.instance.weaponPower < PlayerShooting.instance.maxweaponPower)
                {
                    SpaceShooterPlayer.instance.GetPowerUp();
                }
                else
                {
                    PlayerShooting.instance.weaponPower = 1;
                    SpaceShooterPlayer.instance.UpdateLevel();
                }

                Tracker.Instance.Message("Powered-up collected: " + PlayerShooting.instance.weaponPower);

                Destroy(gameObject);
                SpaceShooterPlayer.instance.AddScore(scoreBonus);
            }
        }
    }
}