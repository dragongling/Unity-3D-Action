using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Can have <a href="https://tvtropes.org/pmwiki/pmwiki.php/Main/CriticalExistenceFailure">Critical Existence Failure</a>
/// </summary>
public class Killable : MonoBehaviour
{
    [SerializeField]
    public event EventHandler OnDeath;

    public float fallLevelY = -15f;

    private void Update()
    {
        if (transform.position.y <= fallLevelY)
            OnDeath(gameObject, EventArgs.Empty);
    }
}
