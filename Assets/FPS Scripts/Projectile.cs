using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    public float speed = 20f;
    public int lifeDuration = 2;

    private float lifeTimer;

    // Start is called before the first frame update
    void Start() {
        lifeTimer = lifeDuration;
    }

    // Update is called once per frame
    void Update() {
        Debug.Log(transform.forward);
        transform.position += transform.forward * speed * Time.deltaTime;
        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0f) {
            Destroy(gameObject);
        }
    }
}
