using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField]
    private StageSO[] stageDatas;

    private int currentStageIndex;

    public StageSO CurrentStageData { get; private set; }
    public float CurrentStageTime { get; private set; }

    private void Start()
    {
        SoundManager.Instance.PlayBgm(BgmType.Game);
        StartStage(0);
    }

    private void Update()
    {
        if (CurrentStageData == null)
        {
            return;
        }

        CurrentStageTime += Time.deltaTime;
    }

    private void StartStage(int index)
    {
        currentStageIndex = index;
        CurrentStageData = stageDatas[currentStageIndex];
        CurrentStageTime = 0f;
    }
}
