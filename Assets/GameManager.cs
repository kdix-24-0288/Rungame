using UnityEngine;
using UnityEngine.SceneManagement; // シーンの再読み込みに必要
using UnityEngine.UI; // UIを扱うために必要
using System.Collections.Generic; // Listを扱うために必要
using TMPro;


public class GameManager: MonoBehaviour
{
    public GameObject gameOverUI; // Inspectorで設定するUIパネル
    [Header("HP表示UI")]
    public List<Image> heartImages; // ハートのImageコンポーネントのリスト
    public Sprite fullHeartSprite;  // 満タンのハートの画像
    public Sprite emptyHeartSprite; // 空のハートの画像

    [Header("スコア関連")]
    public TextMeshProUGUI scoreText; // Inspectorで設定するスコア表示用テキスト
    public Transform player;          // Inspectorで設定するプレイヤー

    private float startPositionX;
    private int score;
    private bool isGameOver = false; // ゲームオーバー状態を管理するフラグ

    void Start()
    {
        gameOverUI.SetActive(false); // ゲーム開始時は非表示
        isGameOver = false;          // ゲームオーバーフラグをリセット
        Time.timeScale = 1f;         // 時間の停止を解除

        // プレイヤーの開始位置を記録
        startPositionX = player.position.x;
        // スコア表示を初期化
        scoreText.text = "Score: 0";
    }
    void Update()
    {
        // ゲームオーバーになっていなければ、スコアを更新し続ける
        if (!isGameOver)
        {
            // 開始位置からの移動距離を計算
            float distance = player.position.x - startPositionX;
            // スコアを整数にする（10倍すると数字が大きくなって気持ちいい）
            int distanceScore = Mathf.Max(0, (int)(distance * 1f));
            scoreText.text = "Score: " + (score + distanceScore);
        }
    }
    // ゲームオーバー画面を表示するためのメソッド
    public void ShowGameOverUI()
    {
        // 非表示にしていたUIパネルを表示する
        gameOverUI.SetActive(true);
        isGameOver = true;
    }

    // リトライボタンから呼び出すためのメソッド
    public void RetryGame()
    {
        // 時間の停止を解除
        Time.timeScale = 1f;
        // 現在のシーンを再読み込みする
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    // GameManager.cs の中に、この新しいメソッドを追加

    public void UpdateHealthUI(int currentHealth)
    {
        for (int i = 0; i < heartImages.Count; i++)
        {
            if (i < currentHealth)
            {
                // HPの現在値より前のハートは「満タン」画像にする
                heartImages[i].sprite = fullHeartSprite;
            }
            else
            {
                // HPの現在値より後のハートは「空」画像にする
                heartImages[i].sprite = emptyHeartSprite;
            }
        }
    }

    public void AddScore(int amount)
    {
        // 現在のスコアに、引数で受け取った点数を加算する
        score += amount;

        // UIの表示も忘れずに更新する
        scoreText.text = "Score: " + score;
    }
}