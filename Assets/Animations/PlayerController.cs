using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("プレイヤー設定")]
    public int health = 3;
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Header("ジャンプ設定")]
    public int maxJumpCount = 2; // 最大ジャンプ回数（2にすれば二段ジャンプ）
    private int currentJumpCount = 0; // 現在のジャンプ回数を記録する

    [Header("可変ジャンプ設定")]
    public float sustainedJumpForce = 25f;  // ジャンプボタンを押し続けている間に加わる力
    public float maxJumpTime = 0.2f;       // ジャンプを続けられる最大時間（秒）
    private float currentJumpTime;         // 現在のジャンプ時間を記録するタイマー
    private bool isJumping;

    [Header("接地判定（Raycast用）")]
    public LayerMask groundLayer;           // Inspectorで設定する地面のレイヤー
    public Transform groundCheckPoint;      // Inspectorで設定する地面判定の基準点
    public float groundCheckDistance = 0.55f; // Inspectorで調整する地面判定の距離
    public Vector2 groundCheckSize = new Vector2(0.7f, 0.25f); // 判定範囲（幅と高さ）

    [Header("壁判定")]
    public LayerMask wallLayer;             // 壁として認識するレイヤー
    public Transform wallCheckPoint;        // 壁を検知する線の開始位置
    public float wallCheckDistance = 0.5f;  // 壁を検知する線の長さ
    private bool isTouchingWall;            // 壁に触れているかどうかの状態

    private float stuckTime = 0f; // 脱出判定に使う時間カウンタ
    private const float STUCK_THRESHOLD = 0.1f; // 0.1秒以上ハマっていたら脱出

    [Header("無敵時間設定")]
    public float invincibilityDuration = 1.5f; // 無敵時間（秒）
    private bool isInvincible = false;       // 現在無敵かどうかの状態
    private SpriteRenderer spriteRenderer;   // 点滅させるためのスプライトレンダラー

    private Rigidbody2D rb;
    private Animator animator;
    private bool isGrounded = false;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        FindObjectOfType<GameManager>().UpdateHealthUI(health);
    }

    void FixedUpdate()
    {
        if (rb.velocity.x < moveSpeed)
        {
            // 前に進むための力を加える
            // ForceMode2D.Forceは、質量を考慮した継続的な力です。
            // moveSpeedに10を掛けるなどして、力の大きさを調整してください。
            rb.AddForce(Vector2.right * moveSpeed * 10f);
        }

        // もし何かの拍子で速度が速くなりすぎた場合、最高速度に制限する（安全装置）
        if (rb.velocity.x > moveSpeed)
        {
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
        }

        // ★★★ ここから追加 ★★★
        // --- ジャンプボタンを押し続けている間の、継続的な上昇処理 ---
        if (isJumping)
        {
            // ジャンプ時間が上限に達していなければ
            if (currentJumpTime < maxJumpTime)
            {
                // 上昇し続ける力を加える
                rb.AddForce(Vector2.up * sustainedJumpForce);
                // 時間を加算
                currentJumpTime += Time.fixedDeltaTime;
            }
            else
            {
                // 上限に達したら、強制的にジャンプを終了
                isJumping = false;
            }
        }
    }

    void Update()
    {
        // --- 接地判定 ---
        //isGrounded = Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckDistance, groundLayer); //Raycastでの着地判定
        //isGrounded = Physics2D.OverlapBox(groundCheckPoint.position, groundCheckSize, 0f, groundLayer); //着地判定に地面があれば反応(うまくいかなかった)
        Vector2 origin = groundCheckPoint.position;
        // 下方向に BoxCast（足元チェック）
        bool downHit = Physics2D.BoxCast(origin, groundCheckSize, 0f, Vector2.down, groundCheckDistance, groundLayer);
        // 右方向に BoxCast（右横チェック）
        bool rightHit = Physics2D.BoxCast(origin, groundCheckSize, 0f, Vector2.right, groundCheckDistance, groundLayer);
        // どちらかに接触していれば接地扱いにする
        //着地判定に、地面が右から触れても下から触れても反応する(ジャンプ不可防止)
        isGrounded = downHit || rightHit;

        // --- 壁判定のRaycast ---
        isTouchingWall = Physics2D.Raycast(wallCheckPoint.position, Vector2.right, wallCheckDistance, wallLayer);

        // --- 角でハマった時の脱出処理 ---
        // 条件：壁に触れていて、かつ、地面にいて、かつ、水平方向の速度がほぼゼロ（＝ハマっている）
        bool isStuck = isTouchingWall && isGrounded && Mathf.Abs(rb.velocity.x) < 0.05f;
        if (isStuck)
        {
            stuckTime += Time.deltaTime;
            if (stuckTime >= STUCK_THRESHOLD)
            {
                rb.AddForce(Vector2.up * 6f, ForceMode2D.Impulse);
                stuckTime = 0f; // リセット
            }
        }
        else
        {
            stuckTime = 0f;
        }
        //if (isTouchingWall && isGrounded && Mathf.Abs(rb.velocity.x) < 0.05f)
        //{
        //小さく上に押し出して、角を乗り越えさせる
        //ここの力の値(3f)は、キャラクターに合わせて微調整してください
        //rb.AddForce(Vector2.up * 0.2f, ForceMode2D.Impulse);//変更前の脱出処理
        //}

        // --- 着地時のリセット処理 ---
        if (isGrounded && rb.velocity.y <= 0)
        {
            currentJumpCount = 0;
            isJumping = false; // ジャンプ状態もリセット
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }

        // --- ジャンプ開始の処理 (GetKeyDown) ---
        if (Input.GetKeyDown(KeyCode.Space) && currentJumpCount < maxJumpCount)
        {
            // Y軸の速度をリセットして、毎回同じ高さでジャンプできるようにする
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse); // 最初のひと蹴り

            currentJumpCount++; // ジャンプ回数をカウント
            isJumping = true;     // ジャンプボタンが押されている状態にする
            currentJumpTime = 0f; // ジャンプ時間タイマーをリセット
        }

        // --- ジャンプ上昇を止める処理 (GetKeyUp) ---
        if (Input.GetKeyUp(KeyCode.Space))
        {
            isJumping = false;
        }

        // --- Animatorの更新 ---
        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("velocityY", rb.velocity.y);
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
            // ▼▼▼ このif文で、ダメージ処理全体を囲む ▼▼▼
            if (!isInvincible) // もし、無敵状態じゃなければ
            {
                health--;
                FindObjectOfType<GameManager>().UpdateHealthUI(health);
                other.gameObject.SetActive(false);

                if (health <= 0)
                {
                    GameOver();
                }
                else
                {
                    // ★★★ ダメージを受けたら、無敵コルーチンを開始する ★★★
                    StartCoroutine(InvincibilityCoroutine());
                }
            }
        }
        if (other.CompareTag("Coin"))
        {
            // GameManagerにスコアを増やすようにお願いする
            FindObjectOfType<GameManager>().AddScore(100); // 1枚あたり100点加算

            // 触れたコインを消す
            Destroy(other.gameObject);
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

    // 無敵時間と点滅を管理するコルーチン
    private IEnumerator InvincibilityCoroutine()
    {
        // 1. 無敵状態を開始
        isInvincible = true;

        float blinkInterval = 0.2f; // 点滅の間隔（0.2秒ごとに表示・非表示を切り替え）
        float invincibleTimer = 0f;   // 無敵時間のタイマー

        // 2. 無敵時間が終わるまで、ループし続ける
        while (invincibleTimer < invincibilityDuration)
        {
            // キャラクターの表示・非表示を切り替える
            spriteRenderer.enabled = !spriteRenderer.enabled;

            // 点滅の間隔だけ処理を待つ
            yield return new WaitForSeconds(blinkInterval);

            // タイマーを加算
            invincibleTimer += blinkInterval;
        }

        // 3. 無敵状態を終了
        // 念のため、最後は必ず表示状態に戻す
        spriteRenderer.enabled = true;
        isInvincible = false;
    }
    void OnDrawGizmosSelected()
{
    //Sceneビューでの着地判定可視化用
    if (groundCheckPoint == null) return;

    Gizmos.color = Color.green;
    Gizmos.DrawWireCube(groundCheckPoint.position, groundCheckSize);
}

} // ← クラスの最後の閉じ括弧
 
