using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawnable : MonoBehaviour
{
    Vector3 spawnPosition;
    public float fallLevelY = -15f;

    void Start()
    {
        spawnPosition = transform.position;
    }

    private void Update()
    {
        if (transform.position.y <= fallLevelY)
            Respawn();
    }

    public void Respawn()
    {
        transform.position = spawnPosition;
        var body = gameObject.GetComponent<Rigidbody>();
        if(body != null)
        {
            body.velocity = Vector3.zero;
        }
    }
}
