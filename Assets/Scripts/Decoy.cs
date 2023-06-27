using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Decoy : NetworkBehaviour{

    [SerializeField] private float maxSpeed;

    [SerializeField] private float jumpingPower;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Animator animator;
    [SerializeField] private Health playerHealth;
    [SerializeField] private GameObject parent;

    private float horizontal;
    [SerializeField] private float lowerExist;
    [SerializeField] private float higherExist;

    void Start() {
        if(!IsHost) return;
        if(transform.rotation.y < -0.1) transform.Rotate(0, 180, 0);
        StartCoroutine(disapear());
        rb.velocity = new Vector2(UnityEngine.Random.Range(-maxSpeed, maxSpeed), UnityEngine.Random.Range(-5, 17f));
    }

    private void FixedUpdate() {
        if(!IsHost) return;
        if(UnityEngine.Random.Range(1, 200) == 1) { 
            if(!playerHealth.dead.Value && IsGrounded()) {
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            }
        }
        animator.SetBool("InAir", !IsGrounded());
        if(!playerHealth.dead.Value && IsGrounded() && UnityEngine.Random.Range(1, 100) == 1 && rb.velocity.x < 0.1f) rb.velocity = new Vector2(UnityEngine.Random.Range(-maxSpeed, maxSpeed), 0f);
        if(Mathf.Abs(rb.velocity.x) > 10f) rb.velocity = new Vector2(Mathf.Abs(rb.velocity.x)/rb.velocity.x*maxSpeed, rb.velocity.y);

        if(IsGrounded() && playerHealth.dead.Value) rb.velocity = new Vector2(0f, rb.velocity.y);
        if((rb.velocity.x > 0 && transform.localScale.x < 0) || (rb.velocity.x < 0 && transform.localScale.x > 0)) transform.localScale = new Vector3(transform.localScale.x*-1, transform.localScale.y, transform.localScale.z);
        if(Mathf.Abs(rb.velocity.x) > 0.1f)animator.SetFloat("Speed", 1);
        else animator.SetFloat("Speed", 0);
    }

    private bool IsGrounded() {
        return Physics2D.OverlapCircle(groundCheck.position, 0.4f, groundLayer);
    }

    private IEnumerator disapear() {
        yield return new WaitForSeconds(UnityEngine.Random.Range(lowerExist, higherExist));
        Destroy(parent);
    }
}
