using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] GameObject baseModel;
    
    public void SetBaseScale(int diskNum)
    {
        var scale = baseModel.transform.localScale;

    }
}