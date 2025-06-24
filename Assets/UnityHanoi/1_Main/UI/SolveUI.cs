using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class SolveUI : MonoBehaviour
{
    [SerializeField] UIDocument UIDoc;

    UnityAction solveAction;

    Button solveBtn;

    public void Show(UnityAction solveAction = null)
    {
        gameObject.SetActive(true);

        this.solveAction = solveAction;
    }
    public void Hide() => gameObject.SetActive(false);

    void OnEnable()
    {
        var main = UIDoc.rootVisualElement.Q("Main");

        solveBtn = main.Q("Solve") as Button;
        solveBtn.RegisterCallback<ClickEvent>(OnSolveClick);
    }

    void OnDisable()
    {
        solveBtn.UnregisterCallback<ClickEvent>(OnSolveClick);
    }

    void OnSolveClick(ClickEvent evt)
    {
        solveAction?.Invoke();
    }
}
