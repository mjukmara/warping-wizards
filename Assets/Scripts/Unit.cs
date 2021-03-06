﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

  public GameObject spawnPrefabOnDeath;
  public GameObject deathEffectPrefab;
  public GameObject soulPrefab;
  public float movementSmoothing = 1.0f;
  public float speed = 2f;
  public int hp = 3;
  public int maxHp = 3;

  protected Animator animator;
  protected Rigidbody2D rb;
  protected Fsm fsm;
  private Vector2 currentVelocity;

  private Transform follow;
  private Vector2 input;


  void Awake() {
    animator = GetComponent<Animator>();
    rb = GetComponent<Rigidbody2D>();
    fsm = GetComponent<Fsm>();

    fsm.AddStateName("Idle");
    fsm.AddStateName("Run");
  }

  void Start() {
  }

  public void SetInput(Vector2 input) {
    this.input = input;
  }

  public void SetFollow(Transform follow) {
    this.follow = follow;
  }

  public bool IsFollowing(Transform transform) {
    return (follow == transform);
  }

  public bool IsFollowing() {
    return follow;
  }

  public Transform GetFollowing() {
    return follow;
  }

  protected void UpdateVelocity() {

    if (hp == 0) return;

    Vector2 direction = GetDirectionVector();
    Vector2 desiredVelocity = direction * speed;
    Vector2 actualVelocity = Vector2.Lerp(currentVelocity, desiredVelocity, 1 / movementSmoothing * Time.deltaTime);
    rb.velocity = actualVelocity;
    currentVelocity = rb.velocity;
  }

  protected void UpdateAnimator() {

    animator.SetFloat("velocity", currentVelocity.magnitude);
    if (currentVelocity.x > 0) {
      animator.SetBool("facing_right", true);
      transform.localScale = new Vector3(1, 1, 1);
    }
    if (currentVelocity.x < 0) {
      animator.SetBool("facing_right", false);
      transform.localScale = new Vector3(-1, 1, 1);
    }
  }

  Vector2 GetDirectionVector() {

    Vector2 direction = Vector3.zero;
    if (follow != null) {
      direction = (follow.position - transform.position).normalized;
    } else {
      direction = input;
    }
    return direction;
  }

  void OnFsmStateChangeEvent(GameObject go, Fsm fsm, string stateName) {

  }

  protected virtual void OnDeath() {

    animator.SetBool("dead", true);
    Destroy(GetComponent<Collider2D>());

    if (deathEffectPrefab) {
      Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
    }
    if (spawnPrefabOnDeath && Random.Range(0.0f, 1.0f) > 0.5f) {
      //Instantiate(spawnPrefabOnDeath, transform.position + new Vector3(Random.Range(-0.25f, 0.25f), Random.Range(-0.25f, 0.25f)), Quaternion.identity);
    }
    if (soulPrefab) {
      Instantiate(soulPrefab, transform.position + new Vector3(Random.Range(-0.25f, 0.25f), Random.Range(-0.25f, 0.25f)), Quaternion.identity);
    }

    Destroy(gameObject, 15f);
  }

  protected virtual void OnHit() {

    if (hp > 0) {
      hp -= 1;
      if (hp <= 0) {
        OnDeath();
      }
    }
  }

  public virtual void OnCollisionEnter2D(Collision2D collision) {
    if (collision.collider.gameObject.tag == "Projectile") {
      GameObject projectile = collision.collider.gameObject;
      rb.AddForce(projectile.transform.forward * 2000);
      OnHit();
    }
  }

  public virtual void OnTriggerEnter2D(Collider2D other) {

  }

  protected virtual void OnEnable() {
    Fsm.OnFsmStateChangeEvent += OnFsmStateChangeEvent;
  }

  protected virtual void OnDisable() {
    Fsm.OnFsmStateChangeEvent -= OnFsmStateChangeEvent;
  }

  public bool isDead() {
    return animator.GetBool("dead");
  }
}
