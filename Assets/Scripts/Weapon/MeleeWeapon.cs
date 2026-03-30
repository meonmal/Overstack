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

            float direction = playerMovement.MoveInput.x < 0 ? -1f : 1f;

            float currentEndZ = endZ * direction;

            float coolTime = playerRunTimeStat.GetStat(StatType.CoolTime) * weaponRunTimeStat.GetStat(WeaponStatType.CoolTime);

            for(int i = 0; i < count; i++)
            {
                weaponVisual.SetActive(true);

                yield return StartCoroutine(Swing(startZ, currentEndZ));

                weaponVisual.SetActive(false);

                yield return null;
            }

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
