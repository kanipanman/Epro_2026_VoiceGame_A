using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Score : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    private int score; // Current score

    // Update is called once per frame
    void Update()
    {
        scoreText.text = GameManager.instance.score.ToString();
    }
}
