using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] MainMenuUI mainMenuUI;
    [SerializeField] DifficultySelectUI difficultySelectUI;

    string savedGame;

    void Start()
    {
        savedGame = PlayerPrefs.GetString("savedGame");
        bool hasPrevGame = savedGame != string.Empty;

        mainMenuUI.Display(hasPrevGame, StartGame, NewGame);
    }

    void NewGame()
    {
        mainMenuUI.Hide();
        difficultySelectUI.Display(StartGame);
    }

    async void StartGame()
    {
        difficultySelectUI.Hide();

        await SceneManager.LoadSceneAsync("1_Main", LoadSceneMode.Additive);

        var test = GameObject.Find("HanoiGameManager");
        //savedGame

        await SceneManager.UnloadSceneAsync("0_MainMenu");
    }
}
