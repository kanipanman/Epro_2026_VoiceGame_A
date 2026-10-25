using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;// Singleton instance
    public int score { get; private set; } = 0; // Player's score
    public int highScore = 0;
    public bool isGameOver = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // ƒV[ƒ“‘JˆÚ‚µ‚Ä‚à‰ó‚³‚È‚¢ê‡
        }
        else
        {
            Destroy(gameObject); // d•¡‚µ‚½GameManager‚ğ”jŠü‚·‚é
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isGameOver)
        {
            score += 1; // Increase score over time
        }
    }
}
