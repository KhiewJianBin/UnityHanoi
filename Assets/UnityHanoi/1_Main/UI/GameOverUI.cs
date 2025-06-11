using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] UIDocument UIDoc;

    UnityAction playAgainAction;

    Button playAgainBtn;

    public void Display(UnityAction playAgainAction = null)
    {
        gameObject.SetActive(true);

        this.playAgainAction = playAgainAction;
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
