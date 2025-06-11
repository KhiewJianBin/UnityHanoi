using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] MainMenuUI mainMenuUI;
    [SerializeField] DifficultySelectUI difficultySelectUI;

    string savedGame;

    void Start()
    {
        savedGame = PlayerPrefs.GetString("GameState");
        bool hasPrevGame = savedGame != string.Empty;

        mainMenuUI.Display(hasPrevGame, ContinueGame, NewGame);
    }

    async void ContinueGame()
    {
        difficultySelectUI.Hide();

        await SceneManager.LoadSceneAsync("1_Main", LoadSceneMode.Additive);

        FindAnyObjectByType<HanoiGameManager>().GameStart(savedGame);

        await SceneManager.UnloadSceneAsync("0_MainMenu");
    }

    void NewGame()
    {
        mainMenuUI.Hide();
        difficultySelectUI.Display(StartGame);
    }

    async void StartGame(int difficulty)
    {
        difficultySelectUI.Hide();

        await SceneManager.LoadSceneAsync("1_Main", LoadSceneMode.Additive);

        FindAnyObjectByType<HanoiGameManager>().GameStart(difficulty);

        await SceneManager.UnloadSceneAsync("0_MainMenu");
    }
}
