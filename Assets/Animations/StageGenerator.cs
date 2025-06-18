using System.Collections.Generic;
using UnityEngine;

public class StageGenerator : MonoBehaviour
{
    [Header("ステージ部品のプレハブリスト")]
    public List<GameObject> stagePrefabs;

    [Header("ステージ生成の設定")]
    public Transform player;
    public float stageLength = 20f;
    public int initialStageCount = 5;

    private List<GameObject> generatedStages = new List<GameObject>();
    private float lastSpawnX;

    void Start()
    {
        lastSpawnX = player.position.x - stageLength;

        // --- ゲーム開始時のステージ生成 ---
        // 最初のステージは、リストの0番目（一番上）のプレハブを必ず生成する
        if (stagePrefabs.Count > 0)
        {
            SpawnStage(stagePrefabs[0]); // 0番目のプレハブを指定して生成
        }

        // 残りの初期ステージはランダムに生成
        for (int i = 1; i < initialStageCount; i++)
        {
            SpawnStage();
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

    // --- SpawnStageメソッドを2種類用意する ---

    // 引数なし版：ランダムなステージを生成する
    void SpawnStage()
    {
        // リストの中からランダムにプレハブを選ぶ
        int randomIndex = Random.Range(0, stagePrefabs.Count);
        // 選んだプレハブを使って、下のもう一つのSpawnStageメソッドを呼び出す
        SpawnStage(stagePrefabs[randomIndex]);
    }

    // 引数あり版：指定されたプレハブでステージを生成する
    void SpawnStage(GameObject prefabToSpawn)
    {
        Vector3 spawnPos = new Vector3(lastSpawnX + stageLength -0.01f, 0, 0);
        GameObject newStage = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

        generatedStages.Add(newStage);
        lastSpawnX = newStage.transform.position.x;
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