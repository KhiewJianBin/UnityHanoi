using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class DifficultySelectUI : MonoBehaviour
{
    UnityAction<int> startAction;

    Button startBtn;

    [SerializeField] UIDocument UIDoc;
    // DataBinding
    [SerializeField] string DifficultyText = "Difficulty : [1]";
    [SerializeField] int Difficulty = 0;

    public void Display(UnityAction<int> startAction = null)
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
        slider.value = 1;

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