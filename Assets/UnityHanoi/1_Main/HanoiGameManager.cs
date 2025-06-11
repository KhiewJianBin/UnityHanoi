using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HanoiGameManager : MonoBehaviour
{
    [SerializeField] GameObject towerLeft;
    [SerializeField] GameObject towerMid;
    [SerializeField] GameObject towerRight;

    [SerializeField] GameObject diskPrefab;

    int numberOfDisk;
    List<GameObject> disks;

    // Spawn Settings
    float diskLevelOffset = 0.1f;
    float diskRadiusGrowth = 0.1f;

    [SerializeField] GameOverUI gameOverUI;

    void GameClear()
    {
        if (disks != null)
        {
            foreach (var disk in disks)
            {
                Destroy(disk);
            }
            disks = null;
        }

        numberOfDisk = 0;
    }
    public void GameSetup(GameSettings settings)
    {
        numberOfDisk = settings.DiskNum;

        SpawnTowerAndDisk();
    }
    void SpawnTowerAndDisk()
    {
        disks = new();

        for (int i = 0; i < numberOfDisk; i++)
        {
            var newPlate = Instantiate(diskPrefab, towerLeft.transform);
            newPlate.name = $"Disk {numberOfDisk - i}";

            var transform = newPlate.transform;

            // Set Position
            var pos = transform.position;
            pos.y = (numberOfDisk - i) * diskLevelOffset;
            transform.position = pos;

            // Set Scale
            var scale = transform.localScale;
            scale.x = scale.z = 1 + (i * diskRadiusGrowth);
            transform.localScale = scale;

            // Set Color
            newPlate.GetComponent<MeshRenderer>().material.color = Color.red;
        }
    }
    void GameStart()
    {

    }

    void GameEnd()
    {
        gameOverUI.Display(PlayAgain);
    }

    void PlayAgain()
    {
        SceneManager.LoadScene("0_MainMenu");
    }

    public struct GameSettings
    {
        public int DiskNum;
    }


    void Start()
    {
        Test3();
        GameEnd();
    }

    #region Test
    [ContextMenu("Test3")]
    void Test1()
    {
        GameClear();
        GameSetup(new GameSettings { DiskNum = 4 });

        GameStart();
    }
    [ContextMenu("Test50")]
    void Test3()
    {
        GameClear();
        GameSetup(new GameSettings { DiskNum = 1 });

        GameStart();
    }
    #endregion
}