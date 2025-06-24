using UnityEngine;
using UnityEngine.Events;

public class Selector : Singleton<Selector>
{
    LayerMask selectionLayerMask;
    AwaitableSelect onSelectionAction;
    UnityAction onEmptySelectionAction;

    public delegate Awaitable AwaitableSelect(GameObject go);

    public void RegisterSelection(int layerMask,
        AwaitableSelect onSelectionAction, UnityAction onEmptySelectionAction)
    {
        selectionLayerMask |= layerMask;

        this.onSelectionAction += onSelectionAction;
        this.onEmptySelectionAction += onEmptySelectionAction;
    }

    public void UnRegisterSelection(int layerMask,
        AwaitableSelect onSelectionAction, UnityAction onEmptySelectionAction)
    {
        selectionLayerMask &= ~layerMask;

        this.onSelectionAction -= onSelectionAction;
        this.onEmptySelectionAction -= onEmptySelectionAction;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, selectionLayerMask))
            {
                var selectedObject = hit.transform.gameObject;

                onSelectionAction?.Invoke(selectedObject);
            }
            else
            {
                onEmptySelectionAction?.Invoke();
            }
        }
    }
}
