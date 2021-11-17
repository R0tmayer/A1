using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;

public class Window_QuestPointer : MonoBehaviour {

    [SerializeField] public Camera uiCamera;
    [SerializeField] private Sprite arrowSprite;
    [SerializeField] private bool isEnableCrossSprite;
    [SerializeField] private Sprite crossSprite;
    [SerializeField] float borderSize = 30f;

    [SerializeField] float borderSizeOffsetXLeft = 0f;
    [SerializeField] float borderSizeOffsetXRight = 0f;
    [SerializeField] float borderSizeOffsetYTop = 0f;
    [SerializeField] float borderSizeOffsetYBottom = 0f;

    [SerializeField] bool isRotation;

    [SerializeField] bool isOffImage;
    



    public Transform Target;
    private Vector3 targetPosition;
    private RectTransform pointerRectTransform;
    private Image pointerImage;

    private void Awake() {

        if (uiCamera == null)
            uiCamera = Camera.main;

        pointerRectTransform = transform.Find("Pointer").GetComponent<RectTransform>();
        pointerImage = transform.Find("Pointer").GetComponent<Image>();

        Target = gameObject.GetComponent<Transform>().parent.gameObject.GetComponent<Transform>().parent;
        targetPosition = Target.transform.position;
    }

    
    
    private void Update() {
        Target = gameObject.GetComponent<Transform>().parent.gameObject.GetComponent<Transform>().parent;
        Vector3 targetPositionScreenPoint = Camera.main.WorldToScreenPoint(targetPosition);
        bool isOffScreen = 
            targetPositionScreenPoint.x <= borderSize + borderSizeOffsetXLeft || 
            targetPositionScreenPoint.x >= Screen.width - borderSize - borderSizeOffsetXRight || 
            targetPositionScreenPoint.y <= borderSize + borderSizeOffsetYBottom || 
            targetPositionScreenPoint.y >= Screen.height - borderSize - borderSizeOffsetYTop;

        if (isOffScreen) {
            RotatePointerTowardsTargetPosition();
            if (!isOffImage)
            {
                pointerImage.enabled = true;
                pointerImage.sprite = arrowSprite;
            }
            Vector3 cappedTargetScreenPosition = targetPositionScreenPoint;
            if (cappedTargetScreenPosition.x <= borderSize + borderSizeOffsetXLeft) cappedTargetScreenPosition.x = borderSize + borderSizeOffsetXLeft;
            if (cappedTargetScreenPosition.x >= Screen.width - borderSize - borderSizeOffsetXRight) cappedTargetScreenPosition.x = Screen.width - borderSize - borderSizeOffsetXRight;
            if (cappedTargetScreenPosition.y <= borderSize + borderSizeOffsetYBottom) cappedTargetScreenPosition.y = borderSize + borderSizeOffsetYBottom;
            if (cappedTargetScreenPosition.y >= Screen.height - borderSize - borderSizeOffsetYTop) cappedTargetScreenPosition.y = Screen.height - borderSize - borderSizeOffsetYTop;

            Vector3 pointerWorldPosition = uiCamera.ScreenToWorldPoint(cappedTargetScreenPosition);
            pointerRectTransform.position = pointerWorldPosition;
            pointerRectTransform.localPosition = new Vector3(pointerRectTransform.localPosition.x, pointerRectTransform.localPosition.y, 0f);
        } else {
            if (!isOffImage)
            {
                if (!isEnableCrossSprite)
                {
                    pointerImage.enabled = false;
                }
                else
                {
                    pointerImage.sprite = crossSprite;
                }
            }
           
            Vector3 pointerWorldPosition = uiCamera.ScreenToWorldPoint(targetPositionScreenPoint);
            pointerRectTransform.position = pointerWorldPosition;
            pointerRectTransform.localPosition = new Vector3(pointerRectTransform.localPosition.x, pointerRectTransform.localPosition.y, 0f);

            pointerRectTransform.localEulerAngles = Vector3.zero;
        }
    }



    private void RotatePointerTowardsTargetPosition() {
        if (!isRotation) return;
        Vector3 toPosition = targetPosition;
        Vector3 fromPosition = Camera.main.transform.position;
        fromPosition.z = 0f;
        Vector3 dir = (toPosition - fromPosition).normalized;
        float angle = UtilsClass.GetAngleFromVectorFloat(dir);
        pointerRectTransform.localEulerAngles = new Vector3(0, 0, angle);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }   
    
    public void HideImage(bool state) {
        isOffImage = !state;
        pointerImage.enabled = state;
    }

   [ContextMenu("Show")]
    public void Show(/*Vector3 targetPosition*/) {
        gameObject.SetActive(true);
     //   targetPosition = targetObject.position;
    }
}
