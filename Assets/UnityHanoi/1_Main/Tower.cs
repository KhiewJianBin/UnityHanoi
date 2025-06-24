using DG.Tweening;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] GameObject diskHolder;
    [SerializeField] GameObject baseModel;
    [SerializeField] GameObject highlight;
    [SerializeField] Transform diskinoutPoint;

    public int Id { get; set; }

    public void Highlight()
    {
        highlight.SetActive(true);
    }
    public void Unhighlight()
    {
        highlight.SetActive(false);
    }

    public async Awaitable<GameObject> TakeOutLastDisk()
    {
        var lastDisk = transform.GetChild(transform.childCount - 1);

        await lastDisk.DOMove(diskinoutPoint.position, 0.3f).AsyncWaitForCompletion();

        return lastDisk.gameObject;
    }
    public async Awaitable TakeInDisk(GameObject disk, Vector3 endPos)
    {
        await disk.transform.DOMove(diskinoutPoint.position, 0.3f).AsyncWaitForCompletion();
        await disk.transform.DOMove(endPos, 0.3f).AsyncWaitForCompletion();
    }
}