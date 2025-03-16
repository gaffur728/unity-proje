using System.Collections;
using UnityEngine;

public class puan : MonoBehaviour
{
    private Renderer rend;
    private Collider col;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        col = GetComponent<Collider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(ReSpawn());
            PointManager.onpointCollected.Invoke();
            Debug.Log("Puan toplandi");
        }
    }

    private IEnumerator ReSpawn()
    {
        rend.enabled = false;
        col.enabled = false;
        yield return new WaitForSeconds(5f);
        rend.enabled = true;
        col.enabled=true;
    }

}
