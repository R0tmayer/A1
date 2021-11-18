using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory : MonoBehaviour
{

    [SerializeField] GameObject _car;
    [SerializeField] GameObject _carMiddle;
    [SerializeField] GameObject _carhard;
    [SerializeField] private int _maxSpawnAtATime = 2;
    [SerializeField] private float minDistance = 10;
    private int _lastSpawnPoint = -1;

    [SerializeField] [Range(-1, 2)] private int _spawnOnly = -1;

    private int GetRandomIndex(int countItems) {
        if (countItems <= 0) return 0;
        return Random.Range(0, countItems);
    }

    private int GetRandomSpawnIndex() {
        int index = GetRandomIndex(TargetsManager.Instance.robberSpawnPositions.Count);
        if (index == _lastSpawnPoint)
        {
            return GetRandomSpawnIndex();
        }
        else {
            return index;
        }
    }


    public void SpawnRubbers() {
/*        int carsLeft = GameManager.Instance.carsCount - GameManager.Instance.currentCarsCount;
*/

        int num = Random.Range(1, _maxSpawnAtATime + 1);
        /*
                if (*//*carsLeft < num ||*//* TargetsManager.Instance.robberTargetPositions.Count < num || TargetsManager.Instance.robberSpawnPositions.Count < num ) 
                {
                    num = 1;
                }*/

        if (TargetsManager.Instance.robberTargetPositions.Count < num)
        {
            return;
        }

        for (int i = 0; i < num ; i++)
        {
            SpawnOnceRubber();
        }
    }

    private void SpawnOnceRubber() {
        int randomSpawnIndex = GetRandomSpawnIndex();
        Transform spawnPointTransform = TargetsManager.Instance.robberSpawnPositions[randomSpawnIndex].transform;

        // if (TargetsManager.Instance.robberTargetPositions.Count == 0)
        // {
        //     Debug.Log("Spawn skipped. No targets to rob.");
        //     return;
        // }

        int randomTargetIndex = GetRandomIndex(TargetsManager.Instance.robberTargetPositions.Count);
        House randomHouse = TargetsManager.Instance.robberTargetPositions[randomTargetIndex];
        Transform randomPosition = randomHouse.transformForPathFinder;
        if (Vector3.Distance(randomPosition.position, spawnPointTransform.position) < minDistance)
        {
            Debug.Log("Spawn skipped. Distance to target is too short.");
            return;
        }
        _lastSpawnPoint = randomSpawnIndex;


        GameObject car;

        if (MainPlayer.Instance.Raiting < 1000)
        {
            if (_spawnOnly == -1)
            {
                car = Instantiate(_car, spawnPointTransform.position, spawnPointTransform.rotation);
                car.GetComponent<RoberryPathFinder>().SetTarget(randomHouse);
            }
            else
            {
                car = Instantiate(_spawnOnly == 3 ? _carhard : (_spawnOnly == 2 ? _carMiddle : _car), spawnPointTransform.position, spawnPointTransform.rotation);
                car.GetComponent<RoberryPathFinder>().SetTarget(randomHouse);
            }


        }
        else if (MainPlayer.Instance.Raiting < 1500)
        {
            int i;
            if (_spawnOnly == -1)
            {
                i = Random.Range(0, 2);
            }
            else {
                i = _spawnOnly;
            }
               
            if (i == 0)
            {
                car = Instantiate(_car, spawnPointTransform.position, spawnPointTransform.rotation);
                car.GetComponent<RoberryPathFinder>().SetTarget(randomHouse);
            }
            else
            {
                car = Instantiate(_carMiddle, spawnPointTransform.position, spawnPointTransform.rotation);
                car.GetComponent<RoberryPathFinder>().SetTarget(randomHouse);
            }
        }
        else {
            int i;
            if (_spawnOnly == -1)
            {
                i = Random.Range(0, 3);
            }
            else
            {
                i = _spawnOnly;
            }

            if (i == 0)
            {
                car = Instantiate(_car, spawnPointTransform.position, spawnPointTransform.rotation);
                car.GetComponent<RoberryPathFinder>().SetTarget(randomHouse);
            }
            else if (i == 1)
            {
                car = Instantiate(_carMiddle, spawnPointTransform.position, spawnPointTransform.rotation);
                car.GetComponent<RoberryPathFinder>().SetTarget(randomHouse);
            }
            else
            {
                car = Instantiate(_carhard, spawnPointTransform.position, spawnPointTransform.rotation);
                car.GetComponent<RoberryPathFinder>().SetTarget(randomHouse);
            }
        }

        




        TargetsManager.Instance.robberTargetPositions.Remove(randomHouse);
        TargetsManager.Instance.robbersInLevel.Add(car);
        
        GameManager.Instance.currentCarsCount++;
    }

}
