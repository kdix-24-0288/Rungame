using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("追従するターゲット")]
    public Transform player; // プレイヤーのTransformコンポーネント

    [Header("カメラとプレイヤーの距離（オフセット）")]
    public Vector3 offset = new Vector3(4f, 1f, -10f); // X, Y, Zの距離を調整

    // 全てのUpdate処理が終わった後に呼ばれるメソッド
    void LateUpdate()
    {
        // プレイヤーオブジェクトが設定されていなければ、何もしない（エラー防止）
        if (player == null)
        {
            return;
        }

        // カメラの目標となる位置を計算する
        // カメラのX座標は、常にプレイヤーのX座標に合わせる
        // カメラのY座標とZ座標は、プレイヤーの動きに関係なく一定に保つ
        Vector3 desiredPosition = new Vector3(player.position.x + offset.x, offset.y, offset.z);

        // カメラの位置を、計算した目標位置に更新する
        transform.position = desiredPosition;
    }
}