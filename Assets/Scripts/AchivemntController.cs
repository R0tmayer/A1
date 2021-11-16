using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BGGames.Core;

public class AchivemntController : SceneSingleton<AchivemntController>
{
    [ContextMenu("ResetAchivement")]
    public void ResetAchivement()
    {
        AchievementManager.instance.ResetAchievementState();
    }

    // Когда будет обучение
    public void AchTutorial() {
        AchievementManager.instance.Unlock(0);
    }

    public void AchFirstUpdate() {
        AchievementManager.instance.Unlock(1);
    }

    // Если будет открыта третья локация
    public void AchOpenAllLocation() {
        AchievementManager.instance.Unlock(2);
    }

    [ContextMenu("AchAddUpdateHome")]
    public void AchAddUpdateHome() {
        AchievementManager.instance.AddAchievementProgress("house", 1);
    }

    [ContextMenu("AchUpdate")]
    public void AchUpdate() {
        AchievementManager.instance.AddAchievementProgress("zone", 1);
    }

    public void AchGameScore()
    {
        AchievementManager.instance.Unlock(5);
    }

    public void AchProtectedAbsolute()
    {
        AchievementManager.instance.Unlock(6);
    }

}
