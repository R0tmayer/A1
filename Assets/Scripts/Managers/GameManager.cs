using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BGGames.Core;

public class GameManager : SceneSingleton<GameManager>
{

    [Header("Setting")]
    //public int carsCount;
    public int maxCarsRob = 5;
    
    [SerializeField] private float _spawnInterval;
    [SerializeField] private Factory Factory;
    public int currentCarsCount;
    public int timeToStartGame = 5;

    [SerializeField] private GameObject[] _interfaceElements;

    private void Update()
    {
        if (Metric.Instance.isOnMetric)
        {
            _spawnInterval =  Metric.Instance.spawnRobTIme.GetComponent<MetricaVal>().value ;
            maxCarsRob = (int)Metric.Instance.maxRobAtTime.GetComponent<MetricaVal>().value;

        }

    }

    public void StartGame()
    {
        foreach (var item in _interfaceElements)
        {
            item.SetActive(true);
        }
        StartCoroutine(DelayRobSpawn());
    }

    
    IEnumerator RubberSpawner()
    {
        while (true)
        {
            if (TargetsManager.Instance.robberTargetPositions.Count > 0 || TargetsManager.Instance.robberTargetPositions.Count <= maxCarsRob)
            {
                Factory.SpawnRubbers();
            }
            
            yield return new WaitForSeconds(_spawnInterval);
        }
    }    
    
    IEnumerator DelayRobSpawn()
    {
        yield return new WaitForSeconds(timeToStartGame);
   
        StartCoroutine(RubberSpawner());
    }
}
