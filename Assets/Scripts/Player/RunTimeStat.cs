using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

[System.Serializable]
public class RunTimeStat
{
    /// <summary>
    /// 스탯의 현재 레벨.
    /// </summary>
    private int currentLevel;

    /// <summary>
    /// 스탯의 값.
    /// </summary>
    private IReadOnlyList<float> value;

    /// <summary>
    /// RunTimeStat 생성자.
    /// </summary>
    /// <param name="value">스탯의 값.</param>
    public RunTimeStat(IReadOnlyList<float> value)
    {
        this.value = value;
        currentLevel = 0;
    }

    public float Value => value[Mathf.Clamp(currentLevel, 0, value.Count - 1)];

    /// 현재 레벨이 최대 레벨인지 여부.
    /// currentLevel이 values.Count - 1 이상이면 최대 레벨로 간주한다.
    public bool IsMax => currentLevel >= value.Count - 1;

    /// 스탯 레벨을 1 증가시킨다.
    /// Mathf.Clamp를 사용하여
    /// 0 미만 또는 최대 레벨(values.Count - 1)을 초과하지 않도록 제한한다.
    public void LevelUp()
    {
        if (IsMax)
        {
            return;
        }

        currentLevel = Mathf.Clamp(currentLevel + 1, 0, value.Count - 1);
    }

    /// <summary>
    /// 스탯의 다음 값을 반환하는 함수.
    /// </summary>
    /// <returns></returns>
    public float GetNextValue()
    {
        // 해당 값이 최대 레벨이면 실행 종료.
        if (IsMax)
        {
            return Value;
        }

        int nextLevel = Mathf.Clamp(currentLevel + 1, 0, value.Count - 1);
        return value[nextLevel];
    }

    /// <summary>
    /// UI 표시에 쓸 함수.
    /// 다음 레벨의 값에서 현재 레벨을 뺀 값을 반환한다.
    /// </summary>
    /// <returns></returns>
    public float GetDelta()
    {
        return GetNextValue() - Value;
    }
}
