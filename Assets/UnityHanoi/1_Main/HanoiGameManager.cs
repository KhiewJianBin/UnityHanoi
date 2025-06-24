using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HanoiGameManager : MonoBehaviour
{
    [SerializeField] Tower towerLeft;
    [SerializeField] Tower towerMid;
    [SerializeField] Tower towerRight;

    [SerializeField] GameObject diskPrefab;

    [SerializeField] SolveUI solveUI;
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
    string PrevGameState;
    string GameState;
    List<GameObject> Disks = new();

    void Awake()
    {
        towerLeft.Id = 0;
        towerMid.Id = 1;
        towerRight.Id = 2;

        solveUI.Show(Solve);
    }

    public void GameStart(int difficulty)
    {
        StringBuilder sb = new();
        for (int i = difficulty - 1; i >= 0; i--)
        {
            sb.Append(i);
        }
        sb.Append("__");
        GameState = sb.ToString();

        GameStart(GameState);
    }
    public void GameStart(string gameState)
    {
        this.GameState = gameState;

        GameSetup();
        if (VerifyGameState())
        {
            LoadGameState();
        }
        else
        {
            //Save file corrupted
        }
    }

    void Solve()
    {
        DoSolve();
    }
    async Awaitable DoSolve()
    {
        Selector.Instance.UnRegisterSelection(LayerMask.GetMask("Tower"), OnSelectTower, OnDeselectTower);
        DeSelectTower();
        solveUI.Hide();

        var a = towerLeft;
        var b = towerMid;
        var c = towerRight;

        var data = GameState.Split("_");
        var towerLeft_data = data[0];
        var towerMid_data = data[1];
        var towerRight_data = data[2];

        Move(a, c, b,
            towerLeft_data, towerRight_data, towerMid_data);

        while (MoveQueue.Count > 0)
        {
            var (towerStart, towerEnd) = MoveQueue.Dequeue();

            UpdateGameState(towerStart, towerEnd);
            await AnimateUpdateGameState(towerStart, towerEnd);
            LoadGameState();
        }
        EndGame();
    }

    Queue<(Tower, Tower)> MoveQueue = new Queue<(Tower, Tower)>();
    void Move(Tower from, Tower to, Tower spare,
        string fromdata, string todata, string sparedata)
    {
        if (fromdata.Length == 0)
        {
            return;
        }

        // 1
        Move(from, spare, to,
            fromdata.Remove(fromdata.Length - 1), sparedata, todata);

        // 2
        MoveQueue.Enqueue((from, to));

        // 3
        Move(spare, to, from, fromdata.Remove(fromdata.Length - 1), todata, fromdata);
    }


    async Awaitable OnSelectTower(GameObject towerGO)
    {
        var tower = towerGO.GetComponent<Tower>();

        if (selectMode == TowerSelectMode.Start)
        {
            selectedStartTower = tower;
            selectedStartTower.Highlight();

            selectMode = TowerSelectMode.End;
        }
        else if (selectMode == TowerSelectMode.End)
        {
            if (selectedStartTower == tower)
            {
                DeSelectTower();
                return;
            }

            UpdateGameState(selectedStartTower, tower);
            if (!VerifyGameState())
            {
                GameState = PrevGameState;
            }
            else
            {
                await AnimateUpdateGameState(selectedStartTower, tower);
                SaveGameStateToFile();
                LoadGameState();
            }

            if (VerifyWinGameState())
            {
                EndGame();
            }

            DeSelectTower();
        }
    }
    void DeSelectTower()
    {
        if (selectedStartTower)
        {
            selectedStartTower.Unhighlight();
            selectedStartTower = null;
        }
        selectMode = TowerSelectMode.Start;
    }
    async Awaitable AnimateUpdateGameState(Tower towerStart, Tower towerEnd)
    {
        GameObject lastDisk = await towerStart.TakeOutLastDisk();

        var data = GameState.Split("_");

        var pos = towerEnd.transform.position;
        pos.y = data[towerEnd.Id].Length * diskLevelOffset;
        await towerEnd.TakeInDisk(lastDisk, pos);
    }
    void OnDeselectTower()
    {
        DeSelectTower();
    }

    void UpdateGameState(Tower towerStart, Tower towerEnd)
    {
        var data = GameState.Split("_");
        var towerLeft_data = data[0];
        var towerMid_data = data[1];
        var towerRight_data = data[2];

        if (data[towerStart.Id].Length == 0) return;

        var lastChar = data[towerStart.Id][data[towerStart.Id].Length - 1];
        data[towerEnd.Id] = data[towerEnd.Id] + lastChar;
        data[towerStart.Id] = data[towerStart.Id].Remove(data[towerStart.Id].Length - 1);

        PrevGameState = GameState;
        GameState = string.Join('_', data);
    }
    bool VerifyWinGameState()
    {
        var data = GameState.Split("_");
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
        var data = GameState.Split("_");
        var towerLeft_data = data[0];
        var towerMid_data = data[1];
        var towerRight_data = data[2];

        // Verify Tower Data
        if (!VerifyMissingOrDuplicates(GameState))
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
        var data = GameState.Split("_");
        var towerLeft_data = data[0];
        var towerMid_data = data[1];
        var towerRight_data = data[2];
        var maxNumberOfDisk = towerLeft_data.Length + towerMid_data.Length + towerRight_data.Length;

        //GameClear
        foreach (var disk in Disks)
        {
            Destroy(disk);
        }
        Disks = new();

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
                newDisk.GetComponent<MeshRenderer>().material.color = colors[number];

                Disks.Add(newDisk);
            }
        }
    }
    void EndGame()
    {
        Selector.Instance.UnRegisterSelection(LayerMask.GetMask("Tower"), OnSelectTower, OnDeselectTower);

        gameOverUI.Show(PlayAgain);

        PlayerPrefs.DeleteKey("GameState");
        PlayerPrefs.Save();
    }
    void PlayAgain()
    {
        SceneManager.LoadScene("0_MainMenu");
    }

    void SaveGameStateToFile()
    {
        PlayerPrefs.SetString("GameState", GameState);
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