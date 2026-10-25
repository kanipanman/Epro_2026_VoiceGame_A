using System.Collections; // コルーチンに必要
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; // スペースキーテスト用

[RequireComponent(typeof(AudioSource))]
public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private AudioSource audioSource;
    private string deviceName;
    private bool isMicInitialized = false;

    [Header("UI設定")]
    public Slider volumeSlider;

    [Header("ジャンプ調整設定")]
    public float loudnessThreshold = 0f; // テストのため0に設定
    public float baseJumpForce = 15f;

    private const int sampleSize = 256;
    private float[] waveData = new float[sampleSize];

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        if (Microphone.devices.Length > 0)
        {
            deviceName = Microphone.devices[0];
            // フリーズを防ぐため、マイクの開始処理をコルーチン（別枠）で実行
            StartCoroutine(InitMicrophone());
        }
        else
        {
            Debug.LogError("❌ マイクが1つも接続されていません！");
        }
    }

    // 💡 フリーズ（無限ループ）を絶対に防ぐマイク初期化処理
    IEnumerator InitMicrophone()
    {
        Debug.Log($"[マイク開始] {deviceName} で録音を試みます...");
        audioSource.clip = Microphone.Start(deviceName, true, 1, 44100);

        // マイクが動き出すのを待つ（1フレームずつUnityを動かしながら待つのでフリーズしません）
        float timer = 0f;
        while (!(Microphone.GetPosition(deviceName) > 0))
        {
            timer += Time.deltaTime;
            if (timer > 2f) // 2秒経ってもダメなら諦めて抜ける（安全弁）
            {
                Debug.LogWarning("⚠️ マイクの応答がタイムアウトしました。物理挙動テストのため処理を進めます。");
                break;
            }
            yield return null;
        }

        audioSource.Play();
        isMicInitialized = true;
        Debug.Log("⭕ マイク処理の準備が完了しました。");
    }

    void Update()
    {
        // 🛠️ 【テスト機能】スペースキーを押して動くか確認
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, baseJumpForce);
            Debug.Log("⌨️ スペースキーによる強制ジャンプ成功！");
        }

        // マイクの準備ができていない場合はこれ以降（音声ジャンプ）をスキップ
        if (!isMicInitialized) return;

        float currentLoudness = GetLoudnessFromMic();

        if (volumeSlider != null)
        {
            volumeSlider.value = currentLoudness;
        }

        // 音声によるジャンプ判定
        if (currentLoudness > loudnessThreshold)
        {
            float calculatedJumpForce = currentLoudness * baseJumpForce;
            calculatedJumpForce = Mathf.Clamp(calculatedJumpForce, 3f, baseJumpForce);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, calculatedJumpForce);
            Debug.Log($"🚀 音声ジャンプ！ 現在の音量: {currentLoudness}");
        }
    }

    float GetLoudnessFromMic()
    {
        if (audioSource == null || !Microphone.IsRecording(deviceName)) return 0f;

        audioSource.GetOutputData(waveData, 0);

        float totalVolume = 0f;
        foreach (var sample in waveData)
        {
            totalVolume += Mathf.Abs(sample);
        }

        return totalVolume / sampleSize;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Finish"))
        {
            GameManager.instance.isGameOver = true;
            Debug.Log("Game Over");
        }
    }
}
