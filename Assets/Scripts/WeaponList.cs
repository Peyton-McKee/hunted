using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponList : MonoBehaviour
{
    public GameObject ak47Prefab;
    public GameObject ammo;
    private WeaponData[] weaponsData = {new WeaponData("ak-47", (float).11, 23, 30, 1600)};
    // Start is called before the first frame update
    void Awake()
    {
        PlayerBehavior.gunData = weaponsData[0];
        PlayerBehavior.gun = ak47Prefab;
        PlayerBehavior.gunAmmo = ammo;
        EnemyBehavior.gunData = weaponsData[0];
        EnemyBehavior.gun = ak47Prefab;
        EnemyBehavior.gunAmmo = ammo;
        // foreach(Transform child in ak47Prefab.transform){
        //     GameObject obj = child.gameObject;
        //     if(obj.name == "ak47")
        //     {
        //         foreach(Transform child2 in obj.transform)
        //         {
        //             GameObject ammoObj = child2.gameObject;
        //             System.Console.WriteLine(ammoObj.name);
        //             if(ammoObj.name == "7.62x39")
        //             {
        //                 ammo = ammoObj;
        //                 print(ammo.name);
        //             }
        //             PlayerBehavior.gun = ak47Prefab;
        //             PlayerBehavior.gunAmmo = ammo;
        //         }
        //     }
        // }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public struct WeaponData
    {
        public string name;
        public float fireRate;
        public float damage;
        public float ammoAmount;
        public float bulletVelocity;
        // public GameObject gunPrefab;
        // public GameObject ammoPrefab;
        // public Animator reloadAnimation;
        // public AudioSource audioSource;
        public WeaponData(string n, float f, float d, float a, float bV)
        {
            this.name = n;
            this.fireRate = f;
            this.damage = d;
            this.ammoAmount = a;
            this.bulletVelocity = bV;
        }   
    
    }
}

