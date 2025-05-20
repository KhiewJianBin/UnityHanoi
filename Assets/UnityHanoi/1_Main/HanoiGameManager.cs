using System.Collections.Generic;
using UnityEngine;

public class HanoiGameManager : MonoBehaviour
{
    [SerializeField] GameObject tower1;
    [SerializeField] GameObject tower2;
    [SerializeField] GameObject tower3;

    [SerializeField] GameObject diskPrefab;

    int numberOfDisk;
    List<GameObject> disks;

    // Spawn Settings
    float diskLevelOffset = 0.1f;
    float diskRadiusGrowth = 0.1f;

    void GameClear()
    {
        if(disks != null)
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
            var newPlate = Instantiate(diskPrefab, tower1.transform);
            newPlate.name = $"Disk {numberOfDisk - i}";

            var transform = newPlate.transform;

            // Set Position
            var pos = transform.position;
            pos.y = (numberOfDisk - i) * diskLevelOffset;
            transform.position = pos;

            // Set Scale
            var scale = Vector3.one;
            scale.x = scale.z = i * diskRadiusGrowth;
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

    }

    public struct GameSettings
    {
        public int DiskNum;
    }








    void Start()
    {
        Test3();
    }

    #region Test
    [ContextMenu("Test1")]
    void Test1()
    {
        GameClear();
        GameSetup(new GameSettings { DiskNum = 1 });

        GameStart();
    }
    [ContextMenu("Test2")]
    void Test2()
    {
        GameClear();
        GameSetup(new GameSettings { DiskNum = 2 });

        GameStart();
    }
    [ContextMenu("Test50")]
    void Test3()
    {
        GameClear();
        GameSetup(new GameSettings { DiskNum = 50 });

        GameStart();
    }
    #endregion
}