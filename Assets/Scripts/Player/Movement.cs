using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Movement : NetworkBehaviour {
    [SerializeField] private float maxSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float decceleration;
    [SerializeField] private float airAccel;
    [SerializeField] private float airDeccel;
    private float speed = 0;

    [SerializeField] private float jumpingPower;
    [SerializeField] private float jumpDelay;
    [SerializeField] private float preJumpDelay;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    public Animator animator;
    [SerializeField] private Health playerHealth;
    [SerializeField] private KeyBindData keyBind;


    public List<Tuple<float, Vector3>> transformRec = new();
    public List<Tuple<float, Vector3>> t_transformRec = new();
    public List<Tuple<float, Vector2>> rbRec = new();
    public List<Tuple<float, Vector2>> t_rbRec = new();

    private float horizontal;
    [SerializeField] private bool isFacingRight;
    float framesSinceGround = 1000;
    float framesSinceJump = 1000;


    void Update() {
        if(!IsOwner) return;

        framesSinceGround+=Time.deltaTime;
        framesSinceJump+=Time.deltaTime;
        if(IsGrounded() && !playerHealth.dead.Value) framesSinceGround = 0;
        if(keyBind.inType != 3)if((Input.GetButtonDown("Jump") && keyBind.inType==0) || (Input.GetKeyDown(KeyCode.W) && keyBind.inType != 2) || (Input.GetKeyDown(KeyCode.UpArrow) && keyBind.inType != 1) && !playerHealth.dead.Value) framesSinceJump = 0;
        animator.SetBool("InAir", framesSinceGround == 0 ? false : true);
        if(framesSinceJump < preJumpDelay && framesSinceGround < jumpDelay) {
            if(!playerHealth.dead.Value) {
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
                framesSinceGround = jumpDelay;
            }
        }

        if(!playerHealth.dead.Value) flip();
    }

    private void FixedUpdate() {
        if(!IsOwner) return;

        speed = rb.velocity.x;
        horizontal = Input.GetAxisRaw("Horizontal");
        if(keyBind.inType == 2) {
            horizontal = 0;
            if(Input.GetKey(KeyCode.LeftArrow)) horizontal--;
            if(Input.GetKey(KeyCode.RightArrow)) horizontal++;
        }
        if(playerHealth.dead.Value || keyBind.inType == 3) horizontal = 0;
        if(IsGrounded()) speed += horizontal * acceleration * Time.fixedDeltaTime;
        else speed += horizontal * airAccel * acceleration * Time.fixedDeltaTime;
        if(Mathf.Abs(speed) < 0.3f && horizontal == 0) speed = 0;
        if(IsGrounded()) speed -= decceleration * Time.fixedDeltaTime * (speed);
        else speed -= decceleration * airDeccel * Time.fixedDeltaTime * (speed);
        speed = Mathf.Min(speed, maxSpeed);
        speed = Mathf.Max(speed, -maxSpeed);
        // Debug.Log(speed);

        rb.velocity = new Vector2(speed, rb.velocity.y);
        animator.SetFloat("Speed", Mathf.Abs(horizontal));
    }

    

    public bool IsGrounded() {
        return Physics2D.OverlapCircle(groundCheck.position, 0.4f, groundLayer);
    }

    private void flip() {
        if((isFacingRight && horizontal > 0f) || (!isFacingRight && horizontal < 0f)) {
            isFacingRight = !isFacingRight;
            transform.Rotate(0f, 180f, 0f);
        }
    }


    public void updateTrans(float t_time, float dtime) {
        t_transformRec.Clear();
        t_transformRec.Add(Tuple.Create(dtime, transform.position));
        for(int transformstamp = 0; transformstamp < transformRec.Count; transformstamp++) {
            if(dtime >= t_time) break;
            t_transformRec.Add(transformRec[transformstamp]);
            dtime += transformRec[transformstamp].Item1;
        }
        transformRec.Clear();
        transformRec = t_transformRec.ToListPooled();
    }
    public void updateRb(float t_time, float dtime) { 
        t_rbRec.Clear();
        t_rbRec.Add(Tuple.Create(dtime, rb.velocity));
        for(int rbstamp = 0; rbstamp < rbRec.Count; rbstamp++) {
            if(dtime >= t_time) break;
            t_rbRec.Add(rbRec[rbstamp]);
            dtime += rbRec[rbstamp].Item1;
        }
        rbRec.Clear();
        rbRec = t_rbRec.ToListPooled();
    }
    public void setMove() {
        transform.position = new Vector2(transformRec[transformRec.Count - 1].Item2.x, transformRec[transformRec.Count - 1].Item2.y);
        // transform.rotation = new Vector2(transformRec[transformRec.Count - 1].Item2.rotation.x, transformRec[transformRec.Count - 1].Item2.rotation.y);
        rb.velocity = new Vector2(rbRec[rbRec.Count - 1].Item2.x, rbRec[rbRec.Count - 1].Item2.y);
    }
}
