using UnityEngine;
using UnityEngine.SceneManagement; // シーンの再読み込みに必要

public class GameManager: MonoBehaviour
{
    public GameObject gameOverUI; // Inspectorで設定するUIパネル

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
}