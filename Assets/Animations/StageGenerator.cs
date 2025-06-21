using System.Collections.Generic;
using UnityEngine;

public class StageGenerator : MonoBehaviour
{
    [Header("ステージ部品のプレハブリスト")]
    public List<GameObject> stagePrefabs;

    // ★★★ ここから追加 ★★★
    [Header("敵のプレハブ")]
    public GameObject enemyPrefab; // 生成する敵のプレハブ

    [Header("敵の出現確率")]
    [Range(0, 100)] // Inspectorで0から100のスライダーとして表示
    public int enemyChancePerSpawnPoint = 25; // 敵が出現する確率(%)
    // ★★★ ここまで追加 ★★★

    [Header("ステージ生成の設定")]
    public Transform player;
    public float stageLength = 20f;
    public int initialStageCount = 5;

    private List<GameObject> generatedStages = new List<GameObject>();
    private float lastSpawnX;
    private float lastSpawnY;

    void Start()
    {
        // 開始位置を設定
        lastSpawnX = player.position.x - stageLength;
        lastSpawnY = transform.position.y;

        // --- ここからが新しいロジック ---

        // 1. 最初のステージは、必ずリストの0番目を生成する
        //    リストが空でないことを安全のために確認
        if (stagePrefabs.Count > 0)
        {
            // SpawnStageメソッドに、どのプレハブを使うか直接渡してあげる
            SpawnStage(stagePrefabs[0]);
        }
        else
        {
            Debug.LogError("ステージプレハブがリストに設定されていません！");
            return; // リストが空なら、ここで処理を中断
        }

        // 2. 残りの初期ステージをランダムに生成する
        //    ループを 'i = 1' から始めることで、最初の1つをスキップする
        for (int i = 1; i < initialStageCount; i++)
        {
            SpawnStage(); // 引数なしで呼び出すと、ランダムなステージが選ばれる
        }
    }

    void Update()
    {
        if (player.position.x > lastSpawnX - (stageLength * (initialStageCount - 1)))
        {
            SpawnStage();
            DestroyOldStage();
        }
    }

    void SpawnStage()
    {
        // リストが空でないことを確認
        if (stagePrefabs.Count == 0) return;

        int randomIndex = Random.Range(0, stagePrefabs.Count);
        // 下のSpawnStageメソッドに、選んだプレハブを渡してあげる
        SpawnStage(stagePrefabs[randomIndex]);
    }

    // --- バージョン2：引数あり（指定されたプレハブを実際に生成する）---
    void SpawnStage(GameObject prefabToSpawn)
    {
        // ステージの高さを、前のステージと同じ高さに維持する
        float nextSpawnY = lastSpawnY;

        // 計算した位置にステージを生成する
        Vector3 spawnPos = new Vector3(lastSpawnX + stageLength, nextSpawnY, 0);
        GameObject newStage = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

        // 最後に生成した位置を、次のために記憶しておく
        generatedStages.Add(newStage);
        lastSpawnX = newStage.transform.position.x;
        lastSpawnY = newStage.transform.position.y;

        // ★敵のスポーン処理も忘れずに行う
        TrySpawnEnemy(newStage);
    }


    // ★★★ ここから新しいメソッドを追加 ★★★
    // 確率で敵を生成するメソッド
    // StageGenerator.cs の中のメソッド

    void TrySpawnEnemy(GameObject stage)
    {
        // 敵プレハブが設定されていなければ何もしない
        if (enemyPrefab == null) return;

        // ステージ内の全てのスポーンポイントを順番にチェックする
        foreach (Transform spawnPoint in stage.transform)
        {
            // オブジェクトの名前が "EnemySpawnPoint" で始まっているかチェック
            if (spawnPoint.name.StartsWith("EnemySpawnPoint"))
            {
                // ★★★ ここからが新しいロジック ★★★
                // このスポーンポイントに敵を出現させるかどうか、確率で決定する
                if (Random.Range(0, 100) < enemyChancePerSpawnPoint)
                {
                    // 高さを自動調整して敵を生成する（以前のコードと同じ）
                    float enemyHeight = enemyPrefab.GetComponent<SpriteRenderer>().bounds.size.y;
                    Vector3 spawnPosition = spawnPoint.position + new Vector3(0, enemyHeight / 2, 0);

                    Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
                }
            }
        }
    }

            void DestroyOldStage()
    {
        if (generatedStages.Count > initialStageCount + 2)
        {
            GameObject oldStage = generatedStages[0];
            generatedStages.RemoveAt(0);
            Destroy(oldStage);
        }
    }
}