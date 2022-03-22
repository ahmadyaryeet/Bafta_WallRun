using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
     [Header("Wallrunning")]
     public LayerMask whatIsWall;
     public LayerMask whatIsGround;
     public float wallRunForce;
     public float maxWallRunTIme;
     private float wallRunTimer;

     [Header("Input")]
     private float horizontalInput;
     private float verticalInput;

     [Header("Detection")]
     public float wallCheckDistance;
     public float minJumpHeight;
     private RaycastHit leftWallhit;
     private RaycastHit rightWallhit;
     private bool wallLeft;
     private bool wallRight;

     [Header("Refrences")]
     public Transform orientation;
     private PlayerMovement pm;
     private Rigidbody rb;

     private void Start()
     {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
     }

     private void Update()
     {
         CheckForWall();
         StateMachine();
     }
     
   
     
     private void FixedUpdate()
     {
         if(pm.wallrunning)
            WallRunningMovement();
     }
     
     private void CheckForWall()
     {
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallhit, wallCheckDistance, whatIsWall);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallhit, wallCheckDistance, whatIsWall);
     }

     private bool AboveGround()
     {
         return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
     }


     private void StateMachine()
     {
         //GettingInputs
         horizontalInput = Input.GetAxisRaw("Horizontal");
         verticalInput = Input.GetAxisRaw("Vertical");

         //State 1 
         if((wallLeft || wallRight) && verticalInput > 0 && AboveGround())
         {
            Debug.Log("runnign");
            if(!pm.wallrunning)
                StartWallRun();
         }

         //state 3
         else if(!((wallLeft || wallRight) && verticalInput > 0 && AboveGround()))
         {
            Debug.Log("hello");
            StopWallRun();
         }
     }

     private void StartWallRun()
     {
         pm.wallrunning = true;
     }

     private void WallRunningMovement()
     {
        rb.useGravity = false;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
     
        Vector3 wallNormal = wallRight ? rightWallhit.normal : leftWallhit.normal;
         Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);
        
         if((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
            wallForward = -wallForward;

         rb.AddForce(wallForward * wallRunForce, ForceMode.Force);
     }

     private void StopWallRun()
     {
         pm.wallrunning = false;
         rb.useGravity = true;
     }
}
