using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Ballon : MonoBehaviour
{
    private PlayerController playerController;

    private Tweener tweener;

    private bool isDetached;
    private Rigidbody2D rb;
    private Vector2 pos;

    public void SetUpBallon(PlayerController playerController)
    {
        this.playerController = playerController;

        Vector3 scale = transform.localScale;
        transform.localScale = Vector3.zero;

        transform.DOScale(scale, 2.0f).SetEase(Ease.InBounce);

        tweener = transform.DOLocalMoveX(0.02f, 0.2f).SetEase(Ease.Flash).SetLoops(-1, LoopType.Yoyo);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            tweener.Kill();

            playerController.DestroyBallon(this);
        }
    }

    public void FloatingBallon()
    {
        tweener.Kill();  //書かないと左右に動くDOTweenの挙動のままバルーンが飛んでいかない。

        rb = gameObject.AddComponent<Rigidbody2D>();

        rb.gravityScale = 0;

        rb.freezeRotation = true;

        GetComponent<CapsuleCollider2D>().enabled = false;

        pos = transform.position;

        transform.SetParent(null);   //書かないとヒエラルキーの親子状態が解除されない。

        isDetached = true;
    }

    void FixedUpdate()
    {
        if (isDetached == false)
        {
            return;
        }

        pos.y += 0.05f;

        rb.MovePosition(new Vector2(pos.x + Mathf.PingPong(Time.time, 1.5f), pos.y));

        if (transform.position.y > 5.0f)
        {
            Destroy(gameObject);
        }
    }
}
