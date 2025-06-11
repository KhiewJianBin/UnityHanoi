using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HanoiGameManager : MonoBehaviour
{
    [SerializeField] Tower towerLeft;
    [SerializeField] Tower towerMid;
    [SerializeField] Tower towerRight;

    [SerializeField] GameObject diskPrefab;

    [SerializeField] GameOverUI gameOverUI;

    // Spawn Settings
    float diskLevelOffset = 0.1f;
    float diskRadiusGrowth = 0.1f;
    List<Color> colors = new()
    {
        Color.red,
        Color.blue,
        Color.green,
        Color.yellow,
        new Color(1f, 0.5f, 0f),       // orange
        new Color(0.5f, 0f, 0.5f),     // purple
        Color.black,
        Color.white,
        Color.gray,
        Color.cyan
    };

    // User Interaction
    enum TowerSelectMode { Start, End }
    TowerSelectMode selectMode = TowerSelectMode.Start;
    Tower selectedStartTower;

    // Game Cache
    string prevGameState;
    string gameState;
    List<GameObject> disks = new();

    void Awake()
    {
        towerLeft.Id = 0;
        towerMid.Id = 1;
        towerRight.Id = 2;
    }

    public void GameStart(int difficulty)
    {
        StringBuilder sb = new();
        for (int i = difficulty; i > 0; i--)
        {
            sb.Append(i);
        }
        sb.Append("__");
        gameState = sb.ToString();

        GameStart(gameState);
    }
    public void GameStart(string gameState)
    {
        this.gameState = gameState;

        GameSetup();
        if (VerifyGameState())
        {
            LoadGameState();
        }
    }
    void OnSelectTower(GameObject towerGO)
    {
        var tower = towerGO.GetComponent<Tower>();

        if (selectMode == TowerSelectMode.Start)
        {
            if (selectedStartTower != null && selectedStartTower != tower)
            {
                selectedStartTower.Unhighlight();
            }

            selectedStartTower = tower;
            selectedStartTower.Highlight();

            selectMode = TowerSelectMode.End;
        }
        else if (selectMode == TowerSelectMode.End)
        {
            UpdateGameState(selectedStartTower, tower);
            if (!VerifyGameState())
            {
                gameState = prevGameState;
            }
            else
            {
                SaveGameStateToFile();
                LoadGameState();
            }

            if (VerifyWinGameState())
            {
                EndGame();
            }

            selectedStartTower.Unhighlight();
            selectedStartTower = null;

            selectMode = TowerSelectMode.Start;
        }
    }
    void OnDeselectTower()
    {
        if (selectedStartTower != null)
        {
            selectedStartTower.Unhighlight();
            selectedStartTower = null;

            selectMode = TowerSelectMode.Start;
        }
    }

    void UpdateGameState(Tower towerStart, Tower towerEnd)
    {
        var data = gameState.Split("_");
        var towerLeft_data = data[0];
        var towerMid_data = data[1];
        var towerRight_data = data[2];

        if (data[towerStart.Id].Length == 0) return;

        var lastChar = data[towerStart.Id][data[towerStart.Id].Length - 1];
        data[towerEnd.Id] = data[towerEnd.Id] + lastChar;
        data[towerStart.Id] = data[towerStart.Id].Remove(data[towerStart.Id].Length - 1);

        prevGameState = gameState;
        gameState = string.Join('_', data);
    }
    bool VerifyWinGameState()
    {
        var data = gameState.Split("_");
        var towerLeft_data = data[0];
        var towerMid_data = data[1];
        var towerRight_data = data[2];

        if (towerLeft_data.Length == 0 && towerMid_data.Length == 0 && towerRight_data.Length != 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void GameSetup()
    {
        Selector.Instance.RegisterSelection(LayerMask.GetMask("Tower"), OnSelectTower, OnDeselectTower);
    }
    bool VerifyGameState()
    {
        var data = gameState.Split("_");
        var towerLeft_data = data[0];
        var towerMid_data = data[1];
        var towerRight_data = data[2];

        // Verify Tower Data
        if (!VerifyMissingOrDuplicates(gameState))
        {
            Debug.LogWarning("GameState/Data Corrupted");
            return false;
        }
        bool correctOrder = VerifyOrder(towerLeft_data) && VerifyOrder(towerMid_data) && VerifyOrder(towerRight_data);
        if (!correctOrder)
        {
            Debug.LogWarning("GameState/Data Corrupted");
            return false;
        }

        return true;

        bool VerifyMissingOrDuplicates(string data)
        {
            data = data.Replace("_", "");

            List<int> numbers = new(data.Length);
            for (int i = 0; i < data.Length; i++)
            {
                char c = data[i];
                if (!char.IsDigit(c)) return false;

                numbers.Add(int.Parse(c.ToString()));
            }

            return numbers.Count == numbers.Distinct().Count();
        }
        bool VerifyOrder(string towerData)
        {
            for (int i = 0; i < towerData.Length - 1; i++)
            {
                if (towerData[i] < towerData[i + 1])
                {
                    return false;
                }
            }
            return true;
        }
    }
    void LoadGameState()
    {
        var data = gameState.Split("_");
        var towerLeft_data = data[0];
        var towerMid_data = data[1];
        var towerRight_data = data[2];
        var maxNumberOfDisk = towerLeft_data.Length + towerMid_data.Length + towerRight_data.Length;

        //GameClear
        foreach (var disk in disks)
        {
            Destroy(disk);
        }
        disks = new();

        LoadTower(towerLeft, towerLeft_data, maxNumberOfDisk);
        LoadTower(towerMid, towerMid_data, maxNumberOfDisk);
        LoadTower(towerRight, towerRight_data, maxNumberOfDisk);

        void LoadTower(Tower tower, string towerData, int maxNumberOfDisk)
        {
            for (int i = 0; i < towerData.Length; i++)
            {
                var number = int.Parse(towerData[i].ToString());

                var newDisk = Instantiate(diskPrefab, tower.transform);
                newDisk.name = $"Disk {maxNumberOfDisk - number}";

                var transform = newDisk.transform;

                // Set Position
                var pos = transform.position;
                pos.y = i * diskLevelOffset;
                transform.position = pos;

                // Set Scale
                var scale = transform.localScale;
                scale.x = scale.z = 1 + (number * diskRadiusGrowth);
                transform.localScale = scale;

                // Set Color
                newDisk.GetComponent<MeshRenderer>().material.color = colors[number-1];

                disks.Add(newDisk);
            }
        }
    }
    void EndGame()
    {
        Selector.Instance.UnRegisterSelection(LayerMask.GetMask("Tower"), OnSelectTower, OnDeselectTower);

        gameOverUI.Display(PlayAgain);

        PlayerPrefs.DeleteKey("GameState");
        PlayerPrefs.Save();
    }
    void PlayAgain()
    {
        SceneManager.LoadScene("0_MainMenu");
    }

    void SaveGameStateToFile()
    {
        PlayerPrefs.SetString("GameState", gameState);
        PlayerPrefs.Save();
    }

    #region Test
    [ContextMenu("TestStart")]
    void Test()
    {
        GameStart("321__");
    }
    #endregion
}