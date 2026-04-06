using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// 현재 스테이지 데이터를 기준으로 몬스터를 스폰하고,
/// ObjectPool을 이용하여 몬스터를 재사용하는 클래스.
/// 
/// 이 클래스는 더 이상 스테이지 시간을 직접 관리하지 않는다.
/// 현재 스테이지 시간은 StageManager가 관리하고,
/// 이 클래스는 그 시간을 읽어서 지금 등장 가능한 몬스터만 생성한다.
/// </summary>
public class MonsterObjectPool : MonoBehaviour
{
    /// <summary>
    /// 몬스터가 생성될 최소 거리.
    /// 플레이어와 너무 가까운 위치에 생성되는 것을 막기 위해 사용한다.
    /// </summary>
    [Header("스폰 범위")]
    [SerializeField]
    private float minSpawnPos;

    /// <summary>
    /// 몬스터가 생성될 최대 거리.
    /// 플레이어 기준 이 거리 안쪽 범위에서 생성된다.
    /// </summary>
    [SerializeField]
    private float maxSpawnPos;

    /// <summary>
    /// 공용 몬스터 프리팹.
    /// 실제 몬스터 종류는 Setup에서 MonsterStats를 주입하여 결정한다.
    /// </summary>
    [Header("참조")]
    [SerializeField]
    private Monster monsterPrefab;

    /// <summary>
    /// 플레이어의 Rigidbody2D.
    /// 몬스터의 추적 대상이며, 생성 위치 계산의 기준점이 된다.
    /// </summary>
    [SerializeField]
    private Rigidbody2D target;

    /// <summary>
    /// 현재 스테이지 정보와 현재 스테이지 시간을 제공하는 매니저.
    /// </summary>
    [SerializeField]
    private StageManager stageManager;

    [SerializeField]
    private ExpOrbSpawner expOrbSpawner;

    /// <summary>
    /// 몬스터를 재사용하기 위한 ObjectPool.
    /// </summary>
    private IObjectPool<Monster> pool;

    /// <summary>
    /// 각 스폰 데이터별 다음 스폰 시간을 기록하는 Dictionary.
    /// 
    /// Key   : StageSpawnData
    /// Value : 다음에 스폰 가능한 스테이지 시간
    /// 
    /// 예를 들어 spawnInterval이 2초라면,
    /// 한 번 스폰한 뒤 다음 허용 시간을 현재시간 + 2초로 기록한다.
    /// </summary>
    private readonly Dictionary<StageSpawnData, float> nextSpawnTimeMap = new Dictionary<StageSpawnData, float>();

    /// <summary>
    /// 각 몬스터 데이터별 현재 활성 몬스터 수를 기록하는 Dictionary.
    /// 
    /// Key   : MonsterStats
    /// Value : 현재 활성화된 몬스터 수
    /// 
    /// maxAliveCount를 체크할 때 사용한다.
    /// </summary>
    private readonly Dictionary<MonsterStats, int> aliveCountMap = new Dictionary<MonsterStats, int>();

    private void Awake()
    {
        // ObjectPool 생성
        pool = new ObjectPool<Monster>(
            CreateMonster,
            OnGetMonster,
            OnReleaseMonster,
            OnDestroyMonster);
    }

    private void Start()
    {
        // 몬스터 생성 루프 시작
        StartCoroutine(SpawnRoutine());
    }

    /// <summary>
    /// 몬스터를 계속 확인하며 생성하는 코루틴.
    /// 
    /// 이 함수는 매 프레임 현재 StageManager의 시간과 StageSO를 확인하고,
    /// 현재 시점에 등장 가능한 몬스터가 있으면 스폰한다.
    /// </summary>
    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            TrySpawnMonsters();

            yield return null;
        }
    }

    /// <summary>
    /// 현재 스테이지 데이터와 현재 시간을 기준으로
    /// 등장 가능한 몬스터들을 검사하고 스폰을 시도하는 함수.
    /// </summary>
    private void TrySpawnMonsters()
    {
        // StageManager가 없으면 실행할 수 없음
        if (stageManager == null)
        {
            return;
        }

        // 현재 스테이지 데이터가 없으면 실행할 수 없음
        if (stageManager.CurrentStageData == null)
        {
            return;
        }

        // 현재 스테이지의 진행 시간
        float currentTime = stageManager.CurrentStageTime;

        // 현재 스테이지의 스폰 데이터 목록
        List<StageSpawnData> spawnDatas = stageManager.CurrentStageData.spawnDatas;

        if (spawnDatas == null || spawnDatas.Count == 0)
        {
            return;
        }

        for (int i = 0; i < spawnDatas.Count; i++)
        {
            StageSpawnData spawnData = spawnDatas[i];

            // 현재 시간이 이 몬스터의 등장 시간 범위 안이 아니면 스킵
            if (currentTime < spawnData.startTime || currentTime > spawnData.endTime)
            {
                continue;
            }

            // 다음 스폰 시간이 아직 안 됐다면 스킵
            if (nextSpawnTimeMap.TryGetValue(spawnData, out float nextSpawnTime))
            {
                if (currentTime < nextSpawnTime)
                {
                    continue;
                }
            }

            // 현재 살아있는 몬스터 수 확인
            int currentAliveCount = GetAliveCount(spawnData.monsterData);

            // 최대 생존 수를 이미 채웠다면 스폰하지 않음
            if (currentAliveCount >= spawnData.maxAliveCount)
            {
                continue;
            }

            // 실제로 생성할 수 있는 수 계산
            int canSpawnCount = spawnData.maxAliveCount - currentAliveCount;
            int currentSpawnCount = Mathf.Min(spawnData.spawnCount, canSpawnCount);

            for (int j = 0; j < currentSpawnCount; j++)
            {
                Monster monster = pool.Get();

                // 어떤 몬스터인지 데이터 주입
                monster.Setup(spawnData.monsterData, target, expOrbSpawner);

                // 생성 위치 설정
                monster.transform.position = SpawnPosition();
            }

            // 다음 스폰 가능 시간 갱신
            nextSpawnTimeMap[spawnData] = currentTime + spawnData.spawnInterval;
        }
    }

    /// <summary>
    /// 플레이어 주변의 랜덤 위치를 계산하는 함수.
    /// 
    /// - 랜덤 방향을 구한다.
    /// - 최소 ~ 최대 거리 사이의 랜덤 거리를 구한다.
    /// - 플레이어 위치를 기준으로 최종 스폰 위치를 계산한다.
    /// </summary>
    private Vector2 SpawnPosition()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        float randomDistance = Random.Range(minSpawnPos, maxSpawnPos);

        Vector2 playerPosition = target.position;
        Vector2 spawnPosition = playerPosition + randomDirection * randomDistance;

        return spawnPosition;
    }

    /// <summary>
    /// 현재 특정 몬스터 데이터가 몇 마리 활성화되어 있는지 반환한다.
    /// </summary>
    private int GetAliveCount(MonsterStats monsterData)
    {
        if (monsterData == null){
            return 0;
        }

        if (aliveCountMap.TryGetValue(monsterData, out int count))
        {
            return count;
        }

        return 0;
    }

    /// <summary>
    /// 풀에 새로운 몬스터가 필요할 때 호출되는 생성 함수.
    /// 
    /// 공용 몬스터 프리팹을 하나 생성하고,
    /// 풀 참조를 주입한다.
    /// </summary>
    private Monster CreateMonster()
    {
        Monster clone = Instantiate(monsterPrefab);

        clone.SetPool(pool);
        clone.transform.parent = transform;

        return clone;
    }

    /// <summary>
    /// 몬스터를 풀에서 꺼낼 때 호출되는 함수.
    /// 
    /// 이 시점에는 아직 어떤 몬스터 데이터인지 결정되지 않았을 수 있으므로
    /// 여기서는 단순히 활성화만 처리한다.
    /// 
    /// 실제 몬스터 종류 설정은 Setup에서 처리한다.
    /// </summary>
    private void OnGetMonster(Monster monster)
    {
        monster.gameObject.SetActive(true);
    }

    /// <summary>
    /// 몬스터를 풀로 반환할 때 호출되는 함수.
    /// 
    /// 현재 몬스터가 들고 있던 MonsterStats를 기준으로
    /// 활성 몬스터 수를 감소시킨 뒤 비활성화한다.
    /// </summary>
    private void OnReleaseMonster(Monster monster)
    {
        MonsterStats monsterData = monster.MonsterStat;

        if (monsterData != null)
        {
            if (!aliveCountMap.ContainsKey(monsterData))
            {
                aliveCountMap.Add(monsterData, 0);
            }

            aliveCountMap[monsterData]--;

            if (aliveCountMap[monsterData] < 0)
            {
                aliveCountMap[monsterData] = 0;
            }
        }

        monster.gameObject.SetActive(false);
    }

    /// <summary>
    /// 몬스터를 완전히 제거할 때 호출되는 함수.
    /// </summary>
    private void OnDestroyMonster(Monster monster)
    {
        Destroy(monster.gameObject);
    }

    /// <summary>
    /// 몬스터가 Setup을 마친 뒤,
    /// 현재 활성 몬스터 수를 증가시키기 위해 호출하는 함수.
    /// 
    /// Monster.Setup 내부에서 호출되도록 연결할 수 있다.
    /// </summary>
    public void RegisterAliveMonster(MonsterStats monsterData)
    {
        if (monsterData == null)
        {
            return;
        }

        if (!aliveCountMap.ContainsKey(monsterData))
        {
            aliveCountMap.Add(monsterData, 0);
        }

        aliveCountMap[monsterData]++;
    }
}