using TMPro;
using UnityEngine;

public class Leaderboard : MonoBehaviour
{
    public TMP_Text usernameText;
    public TMP_Text xpText;

    public void NewScoreElement (string _username, float _xp)
    {
        usernameText.text = _username;
        xpText.text = _xp.ToString();
    }

}
