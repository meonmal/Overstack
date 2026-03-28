using UnityEngine;

public class RePositionMap : MonoBehaviour
{
    /// <summary>
    /// 청크가 움직이는 기준이 될 플레이어.
    /// </summary>
    [SerializeField]
    private Transform player;
    /// <summary>
    /// 청크 참조. 총 9개이다.
    /// </summary>
    [SerializeField]
    private Transform[] chunks;

    /// <summary>
    /// 청크 하나의 크기.
    /// </summary>
    private Vector2Int chunkCellSize = new Vector2Int(36, 20);

    /// <summary>
    /// 셀 한 칸의 크기.
    /// </summary>
    private Vector2 gridCellSize = new Vector2(1f, 1f);

    /// <summary>
    /// 기준점.
    /// </summary>
    private Vector2 originWorld = Vector2.zero;

    private Vector2 chunkWorldSize;

    private void Awake()
    {
        chunkWorldSize = new Vector2(chunkCellSize.x * gridCellSize.x, chunkCellSize.y * gridCellSize.y);
    }

    private void LateUpdate()
    {
        Vector2 p = (Vector2)player.position - originWorld;

        int baseX = Mathf.FloorToInt(p.x / chunkCellSize.x);
        int baseY = Mathf.FloorToInt(p.y / chunkCellSize.y);

        /*
         이제 3x3을 항상 플레이어 주변으로 배치한다.

         (-1,1)   (0,1)   (1,1)
         (-1,0)   (0,0)   (1,0)
         (-1,-1)  (0,-1)  (1,-1)

         baseX, baseY가 플레이어가 속한 중심 청크.
        */

        int index = 0;

        for(int y = -1; y <= 1; y++)
        {
            for(int x = -1; x <= 1; x++)
            {
                Vector2 target = originWorld + new Vector2((baseX + x) * chunkWorldSize.x, (baseY + y) * chunkWorldSize.y);

                Vector2 targetPosition = new Vector2(target.x, target.y);

                chunks[index].position = targetPosition;

                index++;
            }
        }
    }
}
