using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GoalChecker : MonoBehaviour
{
    public float moveSpeed = 0.001f;

    private float stopPos = 5.73f;

    private bool isGoal;

    private GameDirector gameDirector;

    [SerializeField]
    private GameObject secletFloorObj;

    void Update()
    {
        if (transform.position.x > stopPos)
        {
            transform.position += new Vector3(-moveSpeed, 0, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player" && isGoal == false)
        {
            isGoal = true;

            Debug.Log("ゲームクリア");

            PlayerController playerController = col.gameObject.GetComponent<PlayerController>();

            playerController.uiManager.GenerateResultPopUp(playerController.coinPoint);

            gameDirector.GoalClear();

            secletFloorObj.SetActive(true);  //この部分を書かないとsecletFloorObjが表示されなくなる

            secletFloorObj.transform.DOLocalMoveY(0.45f, 2.5f).SetEase(Ease.Linear).SetRelative();
        }
    }

    public void SetUpGoalHouse(GameDirector gameDirector)
    {
        this.gameDirector = gameDirector;

        secletFloorObj.SetActive(false);　　　
    }
}
