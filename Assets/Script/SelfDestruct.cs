using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public float timer;
    void Start()
    {
        StartCoroutine(KillSelf());
    }

    IEnumerator KillSelf()
    {
        yield return new WaitForSeconds(timer);
        Destroy(transform.gameObject);
        yield return null;
    }
}
