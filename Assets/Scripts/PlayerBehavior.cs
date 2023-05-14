using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerBehavior : MonoBehaviour
{
    [Header("Health")]
    public float health;
    public GameObject self;

    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;
    public float stepCoolDown;
    private bool readyToStep;

    [Header("Ground Check")]
    public float playerHeight;
    bool grounded;
    
    public Transform orientation;
    public Transform cameraOrientation;
    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;
    public Rigidbody rb;

    public Vector3 direction = -Vector3.up;
    public RaycastHit hit;
    public float Maxdistance = 10;
    public LayerMask layermask;
    [SerializeField] private Animator reloadAnimator = null;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode shootKey = KeyCode.Mouse0;
    public KeyCode aimKey = KeyCode.Mouse1;
    public KeyCode reloadKey = KeyCode.R;
    public KeyCode runKey = KeyCode.LeftShift;

    
    [Header("Weapon")]
    public static WeaponList.WeaponData gunData;
    public static GameObject gun;
    public static GameObject gunAmmo;
    public Transform firePoint;
    private Vector3 shotDirection;
    private bool readyToShoot;
    private Vector3 rotation;
    private float currentAmmo;
    public static AudioManager audioManager;
    public static GameManagement gameManagement;
    private bool isReloading;
    private void Awake()
    {
        EnemyBehavior.playerOrientation = transform;
    }
    private void Start()
    {
        isReloading = false;
        currentAmmo = gunData.ammoAmount;
        readyToShoot = true;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        readyToStep = true;
        health = 100;
        
    }
    private void Update()
    {   
        Vector3 angles = orientation.eulerAngles;
        
        transform.eulerAngles = angles;
        orientateFiringPoint();
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f);
        Debug.DrawRay(transform.position, Vector3.down, Color.yellow);
        MyInput();
        SpeedControl();
        if(grounded)
        {
            rb.drag = groundDrag;
        }    
        else
        {
            rb.drag = 0;
        }
        if(health < 0)
        {
            Destroy(this.gameObject);
            gameManagement.CreatePlayer();
        }
    }

    void FixedUpdate()
    {
        MovePlayer();
    }
    
    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        print(grounded + " " + readyToJump);
        if(Input.GetKey(shootKey) && readyToShoot && currentAmmo > 0 && !isReloading)
        {
            readyToShoot = false;
            launchProjectile();
            Invoke(nameof(resetShot), gunData.fireRate);
            
        }
        if(Input.GetKey(aimKey))
        {
            aim();
        }
        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
        if((Input.GetKey(reloadKey) && currentAmmo < gunData.ammoAmount) || currentAmmo == 0)
        {
            isReloading = true;
            reload();
            
            Invoke(nameof(resetIsReloading), 2);
        }
        if(Input.GetKey(runKey) && !isReloading)
        {
            reloadAnimator.SetBool("isRunning", true);
            moveSpeed = 10;
        }
        if(!Input.GetKey(runKey))
        {
            reloadAnimator.SetBool("isRunning", false);
            moveSpeed = 7;
        }
    }
    void resetIsReloading()
    {
        isReloading = false;
    }
    
    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        // if(isSwimming)
        // {
        //     Swim();
        // }
       
        if((verticalInput != 0 || horizontalInput != 0) && readyToStep && grounded)
        {
            readyToStep = false;
            audioManager.Play("Ground_running_loop0");
            Invoke(nameof(resetStep), stepCoolDown);
            reloadAnimator.SetBool("isWalking", true);
        }
        if((verticalInput == 0 && horizontalInput == 0) || !grounded)
        {
            audioManager.stopPlaying("Ground_running_loop0");
            resetStep();
            reloadAnimator.SetBool("isWalking", false);
        }
        if(grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
            
        }
        else if(!grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }
    private void resetStep()
    {
        readyToStep = true;
    }
    private void resetShot()
    {
        readyToShoot = true;
    }
    private void aim()
    {

    }
    private void reload()
    {
        reloadAnimator.SetBool("isReloading", true);
        currentAmmo = gunData.ammoAmount;
        Invoke(nameof(resetReload), .25f); 
    }
    private void resetReload()
    {
        reloadAnimator.SetBool("isReloading", false);
    }
    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z );
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
    // public void IsSwimming(bool swim)
    // {
    //     isSwimming = swim;
    // }
    // public void Swim(){
        
    // }
    // // public void OnTriggerEnter(Collider other){
    // //     if(other.CompareTag("Water"))
    // //         SendMessage("IsSwimming", true);
    // // }
    // // public void OnTriggerExit(Collider other){
    // //     if(other.CompareTag("Water"))
    // //         SendMessage("IsSwimming", false);
    // // }
    public void launchProjectile()
    {
        var projectileInstance = Instantiate(gunAmmo, firePoint.position, firePoint.rotation);
        projectileInstance.GetComponent<Rigidbody>().AddForce(firePoint.forward * gunData.bulletVelocity);
        currentAmmo -= 1;
        projectileInstance.tag = "7.62x39";
        audioManager.Play("AutoGun_3p_01");
    }
    public void orientateFiringPoint()
    {
        
        firePoint.rotation = cameraOrientation.rotation;
        
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
