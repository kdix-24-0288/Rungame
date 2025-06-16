using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("プレイヤー設定")]
    public int health = 3;
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

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
    }

    void FixedUpdate()
    {
        // 常に右方向に一定の速度で移動
        rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
    }

    void Update()
    {
        // --- 接地判定をRaycastで行う ---
        isGrounded = Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckDistance, groundLayer);

        // --- Animatorのパラメータを更新 ---
        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("velocityY", rb.velocity.y);

        // --- ジャンプ処理 ---
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        // --- デバッグ用のログ（必要に応じてコメントアウトを外す） ---
        // Debug.Log("接地状態: " + isGrounded + " | Y軸の速度: " + rb.velocity.y);
    }

    // --- 物理的な衝突判定 ---
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 敵との衝突判定のみを行う
        if (collision.gameObject.CompareTag("Enemy"))
        {
            health--;
            Debug.Log("敵に当たった！ 残りライフ: " + health);

            if (health <= 0)
            {
                GameOver();
            }
        }
    }

    // --- トリガーによる当たり判定 ---
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 落下死ゾーンとの判定のみを行う
        if (other.CompareTag("DeadZone"))
        {
            GameOver();
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