using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] UIDocument UIDoc;

    UnityAction playAgainAction;

    Button playAgainBtn;

    public void Show(UnityAction playAgainAction = null)
    {
        gameObject.SetActive(true);

        this.playAgainAction = playAgainAction;
    }
    public void Hide() => gameObject.SetActive(false);

    void OnEnable()
    {
        var main = UIDoc.rootVisualElement.Q("Main");

        playAgainBtn = main.Q("PlayAgain") as Button;
        playAgainBtn.RegisterCallback<ClickEvent>(OnPlayAgainClick);
    }

    void OnDisable()
    {
        playAgainBtn.UnregisterCallback<ClickEvent>(OnPlayAgainClick);
    }

    void OnPlayAgainClick(ClickEvent evt)
    {
        playAgainAction?.Invoke();
    }
}
