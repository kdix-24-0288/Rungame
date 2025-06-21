using UnityEngine;
using UnityEngine.SceneManagement; // シーンの再読み込みに必要
using UnityEngine.UI; // UIを扱うために必要
using System.Collections.Generic; // Listを扱うために必要


public class GameManager: MonoBehaviour
{
    public GameObject gameOverUI; // Inspectorで設定するUIパネル
    [Header("HP表示UI")]
    public List<Image> heartImages; // ハートのImageコンポーネントのリスト
    public Sprite fullHeartSprite;  // 満タンのハートの画像
    public Sprite emptyHeartSprite; // 空のハートの画像

    // ゲームオーバー画面を表示するためのメソッド
    public void ShowGameOverUI()
    {
        // 非表示にしていたUIパネルを表示する
        gameOverUI.SetActive(true);
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
}