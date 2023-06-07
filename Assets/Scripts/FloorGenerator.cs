using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject aerialFloorPrefab;
    [SerializeField]
    private Transform generateTran;

    [Header("生成までの待機時間")]
    public float waitTime;

    private float timer;

    private GameDirector gameDirector;

    private bool isActive;

    void Update()
    {
        if (isActive == false)
        {
            return;
        }

        timer += Time.deltaTime;

        if (timer >= waitTime)
        {
            timer = 0;

            GenerateFloor();
        }
    }

    private void GenerateFloor()
    {
        GameObject obj = Instantiate(aerialFloorPrefab, generateTran);

        float randomPosY = Random.Range(-4.0f, 4.0f);

        obj.transform.position = new Vector2(obj.transform.position.x, obj.transform.position.y + randomPosY);

        gameDirector.GenerateCount++;
    }

    public void SetUpGenerator(GameDirector gameDirector)
    {
        this.gameDirector = gameDirector;  //これを書くことで、他のスクリプトからGameDirectorの情報を参照できるようになる。
    }

    public void SwitchActivation(bool isSwitch)
    {
        isActive = isSwitch;
    }
}
