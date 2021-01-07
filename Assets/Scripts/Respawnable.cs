using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Can <a href="https://tvtropes.org/pmwiki/pmwiki.php/Main/RespawnPoint">Respawn</a>
/// </summary>
public class Respawnable : MonoBehaviour
{
    Vector3 spawnPosition;

    void Start()
    {
        spawnPosition = transform.position;
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
