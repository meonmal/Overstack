using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageSpawnData
{
    public MonsterStats monsterData;

    public float startTime;
    public float endTime;

    public float spawnInterval;
    public int spawnCount;
    public int maxAliveCount;

    public int weight;
}

[CreateAssetMenu(fileName = "StageSO", menuName = "Scriptable Objects/StageSO")]
public class StageSO : ScriptableObject
{
    public string stageName;
    public float stageDuration;
    public List<StageSpawnData> spawnDatas;
}
