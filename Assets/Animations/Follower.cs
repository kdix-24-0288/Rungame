using UnityEngine;

public class Follower : MonoBehaviour
{
    public Transform target; // 追いかける対象（プレイヤー）

    void LateUpdate()
    {
        if (target != null)
        {
            // 自分のX座標を、常にターゲットのX座標に合わせる
            // Y座標とZ座標は元の位置を保つ
            transform.position = new Vector3(target.position.x, transform.position.y, transform.position.z);
        }
    }
}