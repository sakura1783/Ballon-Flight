using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TItleObjectController : MonoBehaviour
{
    [SerializeField]
    private GameObject titleObj;

    [SerializeField]
    private GameDirector gameDirector;

    [SerializeField]
    private UIManager uiManager;

    IEnumerator Start()
    {
        yield return new WaitUntil(() => uiManager.isTitleClicked == true);

        MoveTitleObject();
    }

    private void MoveTitleObject()
    {
        titleObj.transform.DOMoveX(15, 2.0f).SetEase(Ease.Linear).OnComplete(() => { titleObj.SetActive(false); });
    }
}
