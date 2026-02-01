using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script defines 'Enemy's' health and behavior. 
/// </summary>

namespace SpaceShooterDemo
{
    public class Enemy : MonoBehaviour
    {

        #region FIELDS
        [Tooltip("Health points in integer")]
        public int health;

        [Tooltip("Enemy's projectile prefab")]
        public GameObject Projectile;

        [Tooltip("VFX prefab generating after destruction")]
        public GameObject destructionVFX;
        public GameObject hitEffect;

        public int shotChance; //probability of 'Enemy's' shooting during tha path
        [HideInInspector] public float shotTimeMin, shotTimeMax; //max and min time for shooting from the beginning of the path

        [SerializeField]
        private AudioClip deathSound;
        #endregion

        private int maxHealth;

        public bool movingRight = true;

        public SpaceShooterPlayer player;



        private void Start()
        {
            player = SpaceShooterPlayer.instance;

            Invoke("ActivateShooting", Random.Range(shotTimeMin, shotTimeMax));
            maxHealth = health;
        }


        private void Update()
        {
            if (player.bombActive)
                Destruction();
        }



        //coroutine making a shot
        void ActivateShooting()
        {
            if (Random.value < (float)shotChance / 35)                             //if random value less than shot probability, making a shot
            {
                GameObject newBullet = Instantiate(Projectile, gameObject.transform.position, Quaternion.identity);
                if (movingRight)
                    newBullet.GetComponent<DirectMoving>().speedRight = 1;
                else
                    newBullet.GetComponent<DirectMoving>().speedRight = -1;
            }
        }



        //method of getting damage for the 'Enemy'
        public void GetDamage(int damage)
        {
            health -= damage;           //reducing health for damage value, if health is less than 0, starting destruction procedure
            if (health <= 0)
                Destruction();
            else
                Instantiate(hitEffect, transform.position, Quaternion.identity, transform);
        }

        //if 'Enemy' collides 'Player', 'Player' gets the damage equal to projectile's damage value
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "Player")
            {
                if (Projectile.GetComponent<Projectile>() != null)
                    player.Damage(Projectile.GetComponent<Projectile>().damage);
                else
                    player.Damage(1);
            }
        }

        //method of destroying the 'Enemy'
        void Destruction()
        {
            player.PlaySoundOneShot(deathSound);
            player.AddScore(maxHealth);
            Instantiate(destructionVFX, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}