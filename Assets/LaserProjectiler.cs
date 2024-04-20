using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserProjectiler : MonoBehaviour
{
    public GameObject laserPrefab;
    
    public float laserCooldown = 0.5f;
    public float laserSpeed = 10f;
    
    private float laserCooldownTimer = 0f;
    
    private void Update()
    {
        laserCooldownTimer -= Time.deltaTime;

      if (laserCooldownTimer <= 0f)
      {
          ShootLaser();
          laserCooldownTimer = laserCooldown;
      }
    }

    public void ShootLaser()
    {
        //Spawn a laser shooting upwards
        
        GameObject laser = Instantiate(laserPrefab, transform.position, Quaternion.identity);
        laser.GetComponent<Rigidbody2D>().velocity = transform.up * laserSpeed;
    }
}
