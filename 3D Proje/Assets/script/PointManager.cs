
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointManager : MonoBehaviour
{
    public List<Transform> pointLocations;

    [SerializeField] puan pointPrefeb;

    public static Action onpointCollected;

    private int totalPoint;

   
    private void Start()
    {
        SpawnPoints();
    }

    private void IncreasePoint()
    {
        totalPoint++;
    }

    private void OnEnable()
    {
        onpointCollected += IncreasePoint;
    }

    private void OnDisable()
    {
        onpointCollected -= IncreasePoint;
    }


    public void SpawnPoints()
    {
        foreach (var point in pointLocations)
        {
            Instantiate(pointPrefeb, point);
        }

    }


    private IEnumerator ReSpawn()
    {
        gameObject.SetActive(false);
        yield return new WaitForSeconds(5F);
        gameObject.SetActive(true);
    }
   



}
