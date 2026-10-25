using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private AudioSource audioSource;
    private string deviceName;
    private bool isMicInitialized = false;

    [Header("UI設定")]
    public Slider volumeSlider;
    [Tooltip("ゲームオーバー時に表示するリセットボタン")]
    public GameObject retryButton; // 💡【追加】ボタンのゲームオブジェクト

    [Header("ジャンプ調整設定")]
    public float loudnessThreshold = 0.03f;
    public float baseJumpForce = 15f;

    private const int sampleSize = 512;
    private float[] waveData = new float[sampleSize];

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        // 💡【追加】ゲーム開始時は念のためボタンを確実に非表示にしておく
        if (retryButton != null)
        {
            retryButton.SetActive(false);
        }

        if (Microphone.devices.Length > 0)
        {
            deviceName = Microphone.devices[0];
            StartCoroutine(InitMicrophone());
        }
        else
        {
            Debug.LogError("❌ マイクが1つも接続されていません！");
        }
    }

    IEnumerator InitMicrophone()
    {
        audioSource.clip = Microphone.Start(deviceName, true, 10, 44100);
        float timer = 0f;
        while (!(Microphone.GetPosition(deviceName) > 0))
        {
            timer += Time.deltaTime;
            if (timer > 2f) break;
            yield return null;
        }

        audioSource.mute = false;
        audioSource.volume = 0f;
        audioSource.Play();
        isMicInitialized = true;
        Debug.Log("⭕ マイク処理の準備が完了しました。（完全消音モード）");
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, baseJumpForce);
        }

        if (!isMicInitialized) return;

        float currentLoudness = GetLoudnessFromMicClip();

        if (volumeSlider != null)
        {
            volumeSlider.value = currentLoudness;
        }

        if (currentLoudness > loudnessThreshold)
        {
            float calculatedJumpForce = currentLoudness * baseJumpForce;
            calculatedJumpForce = Mathf.Clamp(calculatedJumpForce, 5f, baseJumpForce);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, calculatedJumpForce);
        }
    }

    float GetLoudnessFromMicClip()
    {
        if (audioSource == null || audioSource.clip == null || !Microphone.IsRecording(deviceName)) return 0f;

        int micPosition = Microphone.GetPosition(deviceName);
        int startPosition = micPosition - sampleSize;
        if (startPosition < 0) return 0f;

        audioSource.clip.GetData(waveData, startPosition);

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

            // 💡【追加】ゲームオーバーになったのでリセットボタンを表示する
            if (retryButton != null)
            {
                retryButton.SetActive(true);
            }
        }
    }

    public void ResetStage()
    {
        if (Microphone.IsRecording(deviceName))
        {
            Microphone.End(deviceName);
        }

        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
        GameManager.instance.isGameOver = false;
    }
}
