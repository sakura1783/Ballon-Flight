using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private string horizontal = "Horizontal";
    private string jump = "Jump";

    private Rigidbody2D rb;
    private Animator anim;

    private float limitPosX = 8.6f;
    private float limitPosY = 4.47f;

    private bool isGameOver = false;

    //private int ballonCount;

    public bool isFirstGenerateBallon;

    public float moveSpeed;

    private float scale;

    public float jumpPower;

    [SerializeField, Header("Linecast用 地面判定レイヤー")]
    private LayerMask groundLayer;

    [SerializeField]
    private StartChecker startChecker;

    [SerializeField]
    private AudioManager audioManager;

    [SerializeField]
    private AudioClip knockbackSE;

    [SerializeField]
    private GameObject knockbackEffectPrefab;

    [SerializeField]
    private AudioClip coinSE;

    [SerializeField]
    private GameObject coinEffectPrefab;

    [SerializeField]
    private Joystick joystick;

    [SerializeField]
    private Button btnJump;

    [SerializeField]
    private Button btnDeath;

    public bool isGrounded;

    //public GameObject[] ballons;
    public List<Ballon> ballonList = new List<Ballon>();

    public int maxBallonCount;
    public Transform[] ballonTrans;

    //public GameObject ballonPrefab;
    public Ballon ballonPrefab;

    public float generateTime;
    public bool isGenerating;

    public float knockbackPower;

    public int coinPoint;

    public UIManager uiManager;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        scale = transform.localScale.x;

        //ballons = new GameObject[maxBallonCount];

        btnJump.onClick.AddListener(OnClickJump);
        btnDeath.onClick.AddListener(OnClickDeathOrGenerate);
    }

    void Update()
    {
        isGrounded = Physics2D.Linecast(transform.position + transform.up * 0.4f, transform.position - transform.up * 0.9f, groundLayer);
        Debug.DrawLine(transform.position + transform.up * 0.4f, transform.position - transform.up * 0.9f, Color.red, 1.0f);

        if (ballonList.Count > 0)
        {
            if (Input.GetButtonDown(jump))
            {
                Jump();
            }

            if (Input.GetKeyDown(KeyCode.R) && isGrounded == false)
            {
                DetachBallons();
            }

            if (isGrounded == false && rb.velocity.y < 0.15f)
            {
                anim.SetTrigger("Fall");
            }
        }
        else
        {
        }

        if (rb.velocity.y > 5.0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, 5.0f);
        }

        if (isGrounded == true && isGenerating == false && ballonList.Count < maxBallonCount)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                StartCoroutine(GenerateBallon(1, generateTime));
            }
        }
    }

    private void Jump()
    {
        rb.AddForce(transform.up * jumpPower);

        //Jump(Up,Mid)アニメーションを再生させる
        anim.SetTrigger("Jump");
    }

    void FixedUpdate()
    {
        if (isGameOver == true)
        {
            return;
        }

        Move();
    }

    private void Move()
    {
#if UNITY_EDITOR
        float x = Input.GetAxis(horizontal);
        //x = joystick.Horizontal;
#else
        float x = joystick.Horaizontal;
#endif

        if (x != 0)
        {
            rb.velocity = new Vector2(x * moveSpeed, rb.velocity.y);

            Vector3 temp = transform.localScale;
            temp.x = x;

            if (temp.x > 0)
            {
                temp.x = scale;
            }
            else
            {
                temp.x = -scale;
            }
            transform.localScale = temp;

            anim.SetBool("Idle", false);
            anim.SetFloat("Run", 0.5f);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);

            anim.SetFloat("Run", 0.0f);
            anim.SetBool("Idle", true);
        }

        float posX = Mathf.Clamp(transform.position.x, -limitPosX, limitPosX);
        float posY = Mathf.Clamp(transform.position.y, -limitPosY, limitPosY);

        transform.position = new Vector2(posX, posY);
    }

    private IEnumerator GenerateBallon(int ballonCount, float waitTime)
    {
        if (ballonList.Count >= maxBallonCount)
        {
            yield break;
        }

        isGenerating = true;

        if (isFirstGenerateBallon == false)
        {
            isFirstGenerateBallon = true;
            Debug.Log("初回のバルーン作成");

            startChecker.SetInitialSpeed();
        }

        //if (ballons[0] == null)
        //{
        //ballons[0] = Instantiate(ballonPrefab, ballonTrans[0]);

        //ballons[0].GetComponent<Ballon>().SetUpBallon(this);
        //}
        //else
        //{
        //ballons[1] = Instantiate(ballonPrefab, ballonTrans[1]);

        //ballons[1].GetComponent<Ballon>().SetUpBallon(this);
        //}

        //ballonCount++;

        //yield return new WaitForSeconds(generateTime);

        for (int i = 0; i < ballonCount; i++)
        {
            Ballon ballon;

            if (ballonTrans[0].childCount == 0)
            {
                ballon = Instantiate(ballonPrefab, ballonTrans[0]);
            }
            else
            {
                ballon = Instantiate(ballonPrefab, ballonTrans[1]);
            }

            ballon.SetUpBallon(this);

            ballonList.Add(ballon);

            yield return new WaitForSeconds(waitTime);
        }

        isGenerating = false;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            Vector3 direction = (transform.position - col.transform.position).normalized;

            transform.position += direction * knockbackPower;

            AudioSource.PlayClipAtPoint(knockbackSE, transform.position);

            GameObject knockbackEffect = Instantiate(knockbackEffectPrefab, col.transform.position, Quaternion.identity);

            Destroy(knockbackEffect, 0.5f);
        }
    }

    public void DestroyBallon(Ballon ballon)
    {
        ballonList.Remove(ballon);  //リストから要素を削除
        Destroy(ballon.gameObject);　　//シーンビューからゲームオブジェクトを削除

        //if (ballons[1] != null)
        //{
            //Destroy(ballons[1]);
        //}
        //else if (ballons[0] != null)
        //{
            //Destroy(ballons[0]);
        //}

        //ballonCount--;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Coin")
        {
            coinPoint += col.gameObject.GetComponent<Coin>().point;

            uiManager.UpdateDisplayScore(coinPoint);

            Destroy(col.gameObject);

            AudioSource.PlayClipAtPoint(coinSE, transform.position);

            GameObject coinEffect = Instantiate(coinEffectPrefab, col.transform.position, Quaternion.identity);

            Destroy(coinEffect, 0.5f);
        }
    }

    public void GameOver()
    {
        isGameOver = true;
        Debug.Log(isGameOver);

        uiManager.DisplayGameOverInfo();

        StartCoroutine(audioManager.PlayBGM(3));
    }

    private void OnClickJump()
    {
        if (ballonList.Count > 0)
        {
            Jump();
        }
    }

    private void OnClickDeathOrGenerate()
    {
        if (ballonList.Count > 0 && !isGrounded)
        {
            DetachBallons();
        }
        else if (isGrounded == true && isGenerating == false && ballonList.Count < maxBallonCount)
        {
            StartCoroutine(GenerateBallon(1, generateTime));
        }
    }

    private void DetachBallons()
    {
        for (int i = 0; i < ballonList.Count; i++)
        {
            ballonList[i].FloatingBallon();
        }

        ballonList.Clear();
    }
}
