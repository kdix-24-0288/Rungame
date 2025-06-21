using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("プレイヤー設定")]
    public int health = 3;
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Header("ジャンプ設定")]
    public int maxJumpCount = 2; // 最大ジャンプ回数（2にすれば二段ジャンプ）
    private int currentJumpCount = 0; // 現在のジャンプ回数を記録する

    [Header("接地判定（Raycast用）")]
    public LayerMask groundLayer;           // Inspectorで設定する地面のレイヤー
    public Transform groundCheckPoint;      // Inspectorで設定する地面判定の開始位置
    public float groundCheckDistance = 0.55f; // Inspectorで調整する地面判定の距離

    private Rigidbody2D rb;
    private Animator animator;
    private bool isGrounded = false;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        FindObjectOfType<GameManager>().UpdateHealthUI(health);
    }

    void FixedUpdate()
    {
        // 常に右方向に一定の速度で移動
        rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
    }

    void Update()
    {
        // --- 接地判定 ---
        // isGroundedの判定は、アニメーターとジャンプ回数のリセットのために使用
        isGrounded = Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckDistance, groundLayer);


        // --- ジャンプ回数をリセットする処理 ---
        // もし地面にいて、かつ、ジャンプ回数が0より大きい場合（ジャンプ後に着地した瞬間）
        if (isGrounded && currentJumpCount > 0)
        {
            currentJumpCount = 0;
        }


        // --- ジャンプ処理 ---
        // もしジャンプキーが押されて、かつ、現在のジャンプ回数が最大回数未満なら
        if (Input.GetKeyDown(KeyCode.Space) && currentJumpCount < maxJumpCount)
        {
            // 2回目のジャンプの高さを安定させるため、一度Y軸の速度をリセットする
            rb.velocity = new Vector2(rb.velocity.x, 0);

            // 上方向に力を加える
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            // ジャンプ回数を1増やす
            currentJumpCount++;
        }


        // --- Animatorのパラメータを更新 ---
        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("velocityY", rb.velocity.y);


        // --- デバッグ用のログ（必要に応じてコメントアウトを外す） ---
        // Debug.Log("接地状態: " + isGrounded + " | ジャンプ回数: " + currentJumpCount);
    }


    // --- トリガーによる当たり判定 ---
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 落下死ゾーンとの判定のみを行う
        if (other.CompareTag("DeadZone"))
        {
            GameOver();
        }

        if (other.CompareTag("Enemy"))
        {
            // ダメージ処理
            health--;
            FindObjectOfType<GameManager>().UpdateHealthUI(health);

            Debug.Log("敵に当たった！ 残りライフ: " + health);
            if (health <= 0)
            {
                GameOver();
            }
        }
    }

    // --- ゲームオーバー処理 ---
    public void GameOver()
    {
        Debug.Log("GAME OVER");
        Time.timeScale = 0f;
        this.enabled = false;
        FindObjectOfType<GameManager>().ShowGameOverUI();
    }
}