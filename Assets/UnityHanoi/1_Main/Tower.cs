using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] GameObject baseModel;
    [SerializeField] GameObject highlight;

    public void SetBaseScale(int diskNum)
    {
        var scale = baseModel.transform.localScale;
    }
    public void Highlight()
    {
        highlight.SetActive(true);
    }
    public void Unhighlight()
    {
        highlight.SetActive(false);
    }
}