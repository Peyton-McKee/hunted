using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class EnemyBehavior : MonoBehaviour
{
    [Header("Enemy Management")]
    private float health;
    public static Transform playerOrientation;
    public static WeaponList.WeaponData gunData;
    public static GameObject gun;
    public static GameObject gunAmmo;

    public static AudioManager audioManager;
    public Transform firePoint;
    private float currentAmmo;
    private bool readyToShoot = true;
    private float count = 6;
    public Animator reloadAnimator;
    public string reloadAnimation;
    public Rigidbody rb;
    public static GameManagement gameManagement;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentAmmo = gunData.ammoAmount;
        health = 100;
        
    }

    // Update is called once per frame
    void Update()
    {
        var target = playerOrientation;

        transform.LookAt(target);

        shoot();

        checkDeath();
        SpeedControl();
    }

    private bool canSeePlayer()
    {
       // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 0;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask);
        if (hit.distance - Vector3.Distance(transform.position, playerOrientation.position) > -0.5f)
        {
            return true;
        }
        return false;
    }
    private void shoot()
    {
        if (count > 0 && currentAmmo > 0 && readyToShoot && canSeePlayer())
        {
            readyToShoot = false;
            launchProjectile();
            count--;
            Invoke(nameof(resetShot), gunData.fireRate);
        }
        else if (count == 0 && readyToShoot)
        {
            readyToShoot = false;
            resetCount();
            Invoke(nameof(resetShot), 1.5f);
        }
        else if (currentAmmo == 0 && readyToShoot)
        {
            readyToShoot = false;
            reload();
            Invoke(nameof(resetShot), 2);
        }
        else if(!canSeePlayer())
        {
            moveTowardsPlayer();
        }
        
    }
    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > 5)
        {
            Vector3 limitedVel = flatVel.normalized * 5;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }
    private void moveTowardsPlayer()
    {
        var moveDirection = transform.forward;
        
        rb.AddForce(moveDirection.normalized * 5 , ForceMode.Force);
        
    }

    void checkDeath()
        {
            if (health < 0)
            {
                Destroy(this.gameObject);
                gameManagement.CreateEnemy();
            }
        }
    void reload()
    {
        reloadAnimator.SetBool("isReloading", true);
        currentAmmo = gunData.ammoAmount; 
    }
    void resetCount(){
        count = 6;
    }
    void launchProjectile()
    {
        var projectileInstance = Instantiate(gunAmmo, firePoint.position, firePoint.rotation);
        projectileInstance.GetComponent<Rigidbody>().AddForce(firePoint.forward * gunData.bulletVelocity);
        currentAmmo -= 1;
        projectileInstance.tag = "7.62x39";
        audioManager.Play("AutoGun_3p_01");
    }
    void resetShot()
    {
        readyToShoot = true;
    }
    void OnTriggerEnter(Collider col)
    {
        if(col.tag == "7.62x39")
        {
            this.health = health - 23;
            
            audioManager.Play("HitMarker");
        }
    }

}
