using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    [SerializeField]
    private GameObject weaponVisual; // 실제 칼
    [SerializeField]
    private WeaponStat weaponStat;

    [SerializeField]
    private float duration;
    [SerializeField]
    private float startZ;
    [SerializeField]
    private float endZ;

    private float timer;
    private bool isAttacking;
    private Vector3 originLocalPosition;
    private PlayerMovement playerMovement;
    private WeaponRunTimeStat weaponRunTimeStat;
    private PlayerRunTimeStat playerRunTimeStat;

    private void Awake()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
        originLocalPosition = transform.localPosition;
        playerRunTimeStat = GetComponentInParent<Player>().runTimeStat;
    }

    private void Start()
    {
        weaponRunTimeStat = new WeaponRunTimeStat(weaponStat);

        StartCoroutine(Attack());
    }

    private void Update()
    {
        if (isAttacking)
        {
            return;
        }

        if(playerMovement.MoveInput.x < 0)
        {
            transform.localPosition = new Vector3(-originLocalPosition.x, originLocalPosition.y, originLocalPosition.z);
        }
        else
        {
            transform.localPosition = new Vector3(originLocalPosition.x, originLocalPosition.y, originLocalPosition.z);
        }
    }

    private IEnumerator Attack()
    {
        while (true)
        {
            isAttacking = true;

            int count = playerRunTimeStat.GetIntStat(StatType.ProjectileCount) * weaponRunTimeStat.GetIntStat(WeaponStatType.ProjectileCount);

            /// 첫 번째 공격 방향을 정한다.
            /// 플레이어가 왼쪽을 보고 있으면 -1, 아니면 1
            float firstDirection = playerMovement.MoveInput.x < 0 ? -1f : 1f;

            float coolTime = playerRunTimeStat.GetStat(StatType.CoolTime) * weaponRunTimeStat.GetStat(WeaponStatType.CoolTime);

            for (int i = 0; i < count; i++)
            {
                /// 짝수 번째 공격은 첫 방향,
                /// 홀수 번째 공격은 반대 방향으로 공격한다.
                float currentDirection = (i % 2 == 0) ? firstDirection : -firstDirection;

                /// 방향에 따라 칼의 위치도 바꿔준다.
                transform.localPosition = new Vector3(
                    originLocalPosition.x * currentDirection,
                    originLocalPosition.y,
                    originLocalPosition.z);

                float currentStartZ = startZ * currentDirection;
                float currentEndZ = endZ * currentDirection;

                weaponVisual.SetActive(true);

                yield return StartCoroutine(Swing(currentStartZ, currentEndZ));

                weaponVisual.SetActive(false);

                yield return null;
            }

            /// 공격이 끝난 뒤에는 마지막 방향이 아니라
            /// 현재 바라보는 방향 기준으로 다시 원위치시켜도 된다.
            float idleDirection = playerMovement.MoveInput.x < 0 ? -1f : 1f;

            transform.localPosition = new Vector3(
                originLocalPosition.x * idleDirection,
                originLocalPosition.y,
                originLocalPosition.z);

            isAttacking = false;

            yield return new WaitForSeconds(coolTime);
        }
    }

    private IEnumerator Swing(float start, float end)
    {
        timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / duration);
            float angle = Mathf.Lerp(start, end, t);

            transform.localRotation = Quaternion.Euler(0f, 0f, angle);

            yield return null;
        }

        transform.localRotation = Quaternion.Euler(0f, 0f, start);
    }
}
