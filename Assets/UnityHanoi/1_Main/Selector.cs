using UnityEngine;
using UnityEngine.Events;

public class Selector : Singleton<Selector>
{
    LayerMask selectionLayerMask;
    UnityAction<GameObject> onSelectionAction;
    UnityAction onEmptySelectionAction;

    public void RegisterSelection(int layerMask,
        UnityAction<GameObject> onSelectionAction, UnityAction onEmptySelectionAction)
    {
        selectionLayerMask |= layerMask;

        this.onSelectionAction += onSelectionAction;
        this.onEmptySelectionAction += onEmptySelectionAction;
    }

    public void UnRegisterSelection(int layerMask,
        UnityAction<GameObject> onSelectionAction, UnityAction onEmptySelectionAction)
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
