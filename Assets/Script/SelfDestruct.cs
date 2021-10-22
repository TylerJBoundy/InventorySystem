using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public float timer;

    void Start() => StartCoroutine(StartTimer());

    IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(timer);
        Destroy(transform.gameObject);
        yield return null;
    }
}
