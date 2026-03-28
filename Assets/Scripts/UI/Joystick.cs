using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    /// <summary>
    /// 조이스틱의 바깥 원.
    /// </summary>
    [SerializeField]
    private Image background;

    /// <summary>
    /// 조이스틱의 핸들러.
    /// 실제로 움직이는 것은 핸들러다.
    /// </summary>
    [SerializeField]
    private Image handler;

    [SerializeField]
    private Image fill;

    /// <summary>
    /// 핸들러의 최대 위치.
    /// 핸들러가 바깥 원에서 나가지 못하게 막는 역할이다.
    /// </summary>
    float joystickRadius;
    /// <summary>
    /// 처음으로 터치한 위치.
    /// </summary>
    private Vector2 touchPosition;
    /// <summary>
    /// 이동 방향.
    /// </summary>
    private Vector2 moveDir;
    /// <summary>
    /// 캔버스 그룹의 알파값을 가져오기 위한 컴포넌트
    /// </summary>
    private CanvasGroup canvasGroup;

    public Vector2 MoveDir => moveDir;

    private void Start()
    {
        joystickRadius = background.gameObject.GetComponent<RectTransform>().sizeDelta.y / 2;
        canvasGroup = GetComponent<CanvasGroup>();
    }

    /// <summary>
    /// 화면을 터치하면 실행될 함수.
    /// </summary>
    /// <param name="eventData">화면을 터치한 위치.</param>
    public void OnPointerDown(PointerEventData eventData)
    {
        // 캔버스의 알파값을 1로 설정.
        canvasGroup.alpha = 1;
        // 바깥 원과 핸들러의 위치를 터치한 화면의 위치로 바꾼다.
        background.transform.position = eventData.position;
        handler.transform.position = eventData.position;
        // 손을 땠을 때 핸들러의 위치를 원위치 시키기 위해 
        // 처음으로 터치한 위치도 저장해 놓는다.
        touchPosition = eventData.position;
    }

    /// <summary>
    /// 화면을 드래그할 때 실행될 함수.
    /// </summary>
    /// <param name="eventData">드래그하고 있는 위치.</param>
    public void OnDrag(PointerEventData eventData)
    {
        // 처음 화면을 터치한 위치와 현재 화면을 누르고 있는 위치의 거리를 저장한다.
        Vector2 touchDir = eventData.position - touchPosition;

        // 위에서 계산한 거리와 joystickRadius 중에서 최소값인 것을 저장한다.
        float moveDistance = Mathf.Min(touchDir.magnitude, joystickRadius);

        // 맨 위에서 구한 거리를 정규화해준다.
        moveDir = touchDir.normalized * (moveDistance / joystickRadius);
        // 처음에 누른 위치 + 이동 방향 * 위에서 구한 최소값 으로 계산을 해주고
        Vector2 newPosition = touchPosition + moveDir * moveDistance;
        
        // 그 값을 핸들러에 적용시킨다.
        handler.transform.position = newPosition;
    }

    /// <summary>
    /// 화면에서 손을 땠을 때 실행될 함수.
    /// </summary>
    /// <param name="eventData">손을 땠을 때의 위치. 딱히 필요 없다.</param>
    public void OnPointerUp(PointerEventData eventData)
    {
        // 핸들러의 위치를 처음 눌렀던 위치로 돌려놓는다.
        handler.transform.position = touchPosition;
        // 방향은 없도록 설정.
        moveDir = Vector2.zero;
        // 캔버스의 알파값을 0으로 설정.
        canvasGroup.alpha = 0;
    }
}
