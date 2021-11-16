using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BGGames.Core;

public class TargetsManager : SceneSingleton<TargetsManager>
{
    public List<Transform> robberSpawnPositions;
    public List<House> houses;
    public List<House> robberTargetPositions;
    public List<GameObject> robbersInLevel;
    public FactoriesSecurity factoriesSecurity;

    public SecurityUpdater securityUpdater;
    public HomeUpdater homeUpdater;

    public bool isDrawLine ;
    public bool isBuildMode ;

    public int countUpdate ;
    public int countProtect ;
}
