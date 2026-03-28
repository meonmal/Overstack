using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    /// <summary>
    /// 플레이어를 움직이게 할 joystick.
    /// </summary>
    [SerializeField]
    private Joystick joystick;

    /// <summary>
    /// 플레이어의 리지드바디2D.
    /// </summary>
    private Rigidbody2D rigid;

    /// <summary>
    /// 플레이어의 런타임 스탯.
    /// ScriptableObejct의 원본 데이터가 아니라
    /// 버프/레벨업 등이 반영된 실제 게임 중 수치를 가져오기 위해 사용한다.
    /// </summary>
    private PlayerRunTimeStat runTimeStats;

    /// <summary>
    /// 플레이어의 이동 방향.
    /// </summary>
    private Vector2 moveInput;

    /// <summary>
    /// 플레이어의 이동 속도.
    /// </summary>
    private float moveSpeed;

    /// <summary>
    /// 외부에서 런타임 스탯을 연결하는 함수.
    /// </summary>
    public void Init(PlayerRunTimeStat playerRunTimeStat)
    {
        runTimeStats = playerRunTimeStat;
    }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // 조이스틱이 연결되어 있지 않으면 입력을 0으로 처리.
        if (joystick == null)
        {
            moveInput = Vector2.zero;
            return;
        }

        // 조이스틱 방향값을 읽어온다.
        moveInput = joystick.MoveDir;
    }

    private void FixedUpdate()
    {
        // 런타임 스탯이 연결되지 않았다면 이동하지 않는다.
        if (runTimeStats == null)
        {
            rigid.linearVelocity = Vector2.zero;
            return;
        }

        // 현재 이동속도 스탯을 가져온다.
        moveSpeed = runTimeStats.GetStat(StatType.MoveSpeed);

        // 최종 속도 = 방향 * 이동속도
        Vector2 velocity = moveInput * moveSpeed;

        // 리지드바디2D를 사용해 이동시킨다.
        rigid.linearVelocity = velocity;
    }
}
