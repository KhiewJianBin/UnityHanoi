using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class DifficultySelectUI : MonoBehaviour
{
    [SerializeField] UIDocument UIDoc;

    UnityAction startAction;

    Button startBtn;

    [SerializeField] string Difficulty = "123";
    [SerializeField] string test = "123";

    public void Display(UnityAction startAction = null)
    {
        gameObject.SetActive(true);

        this.startAction = startAction;
    }
    public void Hide() => gameObject.SetActive(false);


    void OnEnable()
    {
        var main = UIDoc.rootVisualElement.Q("Main");

        var slider = main.Q("Title");//Difficulty
        slider.dataSource = this;

        var buttonGrp = main.Q("ButtonGroup");
        startBtn = buttonGrp.Q("Start") as Button;

        startBtn.RegisterCallback<ClickEvent>(OnStartClick);
    }

    void OnDisable()
    {
        startBtn.UnregisterCallback<ClickEvent>(OnStartClick);
    }

    void OnStartClick(ClickEvent evt)
    {
        startAction?.Invoke();
    }
}