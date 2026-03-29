using System.Security.Cryptography;
using UnityEngine;

/// <summary>
/// 몬스터가 플레이어를 추적하고,
/// 너무 멀어졌을 경우 플레이어 주변으로 재배치하는 이동 클래스.
/// 
/// - 기본 동작: 플레이어 방향으로 이동
/// - 보조 동작: 일정 거리 이상 떨어지면 순간이동 (RePosition)
/// </summary>
public class MonsterMovement : MonoBehaviour
{
    /// <summary>
    /// 플레이어와의 최대 거리.
    /// 이 거리 이상 벌어지면 몬스터를 강제로 재배치한다.
    /// </summary>
    [SerializeField]
    private float maxDistance;

    /// <summary>
    /// 재배치 시 최소 거리.
    /// 플레이어와 너무 가까운 위치에 생성되는 것을 방지한다.
    /// </summary>
    [SerializeField]
    private float minRandomPosition;

    /// <summary>
    /// 재배치 시 최대 거리.
    /// 플레이어 주변 어느 정도 범위 안에서만 재배치되도록 제한한다.
    /// </summary>
    [SerializeField]
    private float maxRandomPosition;

    /// <summary>
    /// 추적 대상 (플레이어).
    /// </summary>
    private Rigidbody2D target;

    /// <summary>
    /// 몬스터 자신의 Rigidbody2D.
    /// MovePosition을 이용한 물리 이동에 사용된다.
    /// </summary>
    private Rigidbody2D rigid;

    /// <summary>
    /// 몬스터의 스탯 정보를 담고 있는 컴포넌트.
    /// 이동 속도를 가져오기 위해 사용한다.
    /// </summary>
    private Monster monster;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        // 필요한 컴포넌트 캐싱
        monster = GetComponent<Monster>();
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// 추적할 타겟을 설정하는 함수.
    /// 스포너 또는 매니저에서 호출된다.
    /// </summary>
    public void SetTarget(Rigidbody2D target)
    {
        this.target = target;
    }

    private void FixedUpdate()
    {
        // 물리 기반 이동은 FixedUpdate에서 처리
        Movement();

        // 너무 멀어진 몬스터는 재배치
        RePosition();
    }

    /// <summary>
    /// 플레이어 방향으로 이동하는 함수.
    /// 
    /// - 플레이어까지의 거리 벡터를 구한다.
    /// - 이동 속도를 곱해 프레임 기준 이동량을 계산한다.
    /// - MovePosition으로 부드럽게 이동시킨다.
    /// </summary>
    private void Movement()
    {
        // 플레이어와의 방향 벡터 (목표 위치 - 현재 위치)
        Vector2 distance = target.position - rigid.position;

        // 방향만 남기고 길이를 제거
        Vector2 direction = distance.normalized;

        Vector2 movementDelta = direction * monster.MonsterStat.MoveSpeed * Time.fixedDeltaTime;

        spriteRenderer.flipX = target.transform.position.x < transform.position.x ? true : false;

        // Rigidbody를 이용한 이동
        rigid.MovePosition(rigid.position + movementDelta);
    }

    /// <summary>
    /// 몬스터가 플레이어와 너무 멀어졌을 때 위치를 재설정하는 함수.
    /// 
    /// - 거리 계산은 sqrMagnitude를 사용하여 성능 최적화
    /// - 일정 거리 이상이면 플레이어 주변 랜덤 위치로 이동
    /// </summary>
    private void RePosition()
    {
        // 플레이어와의 거리 벡터
        Vector2 offset = target.position - rigid.position;

        // 거리 비교 (루트 계산 없이 제곱값으로 비교)
        if (offset.sqrMagnitude >= maxDistance * maxDistance)
        {
            // 랜덤 방향 생성
            Vector2 randomDirection = Random.insideUnitCircle.normalized;

            // 랜덤 거리 설정 (최소 ~ 최대)
            float randomDistance = Random.Range(minRandomPosition, maxRandomPosition);

            // 최종 위치 계산 (플레이어 기준 원형 범위)
            Vector2 newPosition = target.position + randomDirection * randomDistance;

            // 즉시 위치 이동 (텔레포트)
            rigid.position = newPosition;
        }
    }
}
