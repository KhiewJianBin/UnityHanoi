using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] UIDocument UIDoc;

    bool showContinue;
    UnityAction continueAction;
    UnityAction newAction;

    Button continueBtn;
    Button newBtn;

    public void Display(bool showContinue,
        UnityAction continueAction = null, UnityAction newAction = null)
    {
        gameObject.SetActive(true);

        this.showContinue = showContinue;
        this.continueAction = continueAction;
        this.newAction = newAction;
    }
    public void Hide() => gameObject.SetActive(false);

    void OnEnable()
    {
        var main = UIDoc.rootVisualElement.Q("Main");

        var buttonGrp = main.Q("ButtonGroup");
        continueBtn = buttonGrp.Q("Continue") as Button;
        newBtn = buttonGrp.Q("New") as Button;

        continueBtn.RegisterCallback<ClickEvent>(OnContinueClick);
        newBtn.RegisterCallback<ClickEvent>(OnNewClick);

        if (!showContinue)
        {
            continueBtn.style.display = DisplayStyle.None;
        }
    }

    void OnDisable()
    {
        continueBtn.UnregisterCallback<ClickEvent>(OnContinueClick);
        newBtn.UnregisterCallback<ClickEvent>(OnNewClick);
    }

    void OnContinueClick(ClickEvent evt)
    {
        continueAction?.Invoke();
    }
    void OnNewClick(ClickEvent evt)
    {
        newAction?.Invoke();
    }
}