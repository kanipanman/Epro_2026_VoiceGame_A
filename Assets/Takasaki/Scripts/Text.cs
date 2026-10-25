using UnityEngine;
using TMPro;

public class Text : MonoBehaviour
{
    public TextMeshProUGUI text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.isGameOver)
        {
            text.text = ("GAME OVER Score:" + GameManager.instance.score);
        }
        else if (!GameManager.instance.isGameOver)
        {
            text.text = ("");
        }
    }
}
