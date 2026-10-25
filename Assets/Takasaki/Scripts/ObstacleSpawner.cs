using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject obstaclePrefab; // 障害物のプレハブ
    public float spawnRate = 2f; // 障害物の出現間隔
    private float timer = 0f; // タイマー

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime; // タイマーを更新
        if (!GameManager.instance.isGameOver)
        {
            if (timer >= spawnRate)
            {
                Instantiate(obstaclePrefab, new Vector3(10, Random.Range(-1.5f, 1.5f), 0), Quaternion.identity); // 障害物を生成
                timer = 0f; // タイマーをリセット
            }
        }
    }
}
