using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class DifficultySelectUI : MonoBehaviour
{
    UnityAction<int> startAction;

    Button startBtn;

    [SerializeField] UIDocument UIDoc;
    // DataBinding
    [SerializeField, HideInInspector] string DifficultyText = "Difficulty : [3]";
    [SerializeField, HideInInspector] int Difficulty = 3;

    public void Show(UnityAction<int> startAction = null)
    {
        gameObject.SetActive(true);

        this.startAction = startAction;
    }
    public void Hide() => gameObject.SetActive(false);

    void OnEnable()
    {
        var main = UIDoc.rootVisualElement.Q("Main");

        var slider = main.Q("Difficulty") as SliderInt;
        slider.dataSource = this;
        slider.RegisterCallback<ChangeEvent<int>>((evt) =>
        {
            DifficultyText = $"Difficulty : [{evt.newValue}]";
        });
        slider.value = 3;

        startBtn = main.Q("Start") as Button;
        startBtn.RegisterCallback<ClickEvent>(OnStartClick);
    }

    void OnDisable()
    {
        startBtn.UnregisterCallback<ClickEvent>(OnStartClick);
    }

    void OnStartClick(ClickEvent evt)
    {
        startAction?.Invoke(Difficulty);
    }
}