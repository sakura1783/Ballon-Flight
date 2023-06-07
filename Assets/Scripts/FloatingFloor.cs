using UnityEngine;
using DG.Tweening;

public class FloatingFloor : MonoBehaviour
{
    private Vector2 pos;

    private float randomValue;

    void Start()
    {
        pos = transform.position;
        randomValue = Random.Range(0.1f, 0.3f);

        transform.DOMoveY(pos.y + randomValue, 2.0f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
    }
}
