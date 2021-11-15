using UnityEngine;

class ScrollAndPinch : MonoBehaviour
{
    public Camera camera;
    protected Plane Plane;

    public bool ConstrainPosition;
    public Vector3 MinPosition;
    public Vector3 MaxPosition;

    public float _scrollSpeed = 5;


    private Vector3 touchStart;

    private void Awake()
    {
        if (camera == null)
            camera = Camera.main;
    }

    private void Update()
    {
        if (!TargetsManager.Instance.isDrawLine)
        {
            if (Input.touchCount == 1)
            {
                Plane.SetNormalAndPosition(transform.up, transform.position);
                var Delta = Vector3.zero;
                Delta = PlanePositionDelta(Input.GetTouch(0));

                if (Input.GetTouch(0).phase == TouchPhase.Moved)
                {
                    camera.transform.Translate(Delta, Space.World);
                }
            }


            if (Input.touchCount < 1)
            {

                if (Input.GetMouseButtonDown(0))
                {
                    touchStart = camera.ScreenToWorldPoint(Input.mousePosition);
                }

                if (Input.GetMouseButton(0))
                {
                    Vector3 direction = touchStart - camera.ScreenToWorldPoint(Input.mousePosition);

                    camera.transform.position += BorderChecker(direction);
                }
            }

            if (Input.touchCount == 0 && Input.GetMouseButtonUp(0))
            {
                TargetsManager.Instance.securityUpdater.isMove = true;
                TargetsManager.Instance.homeUpdater.isMove = true;
            }

        }

        if (TargetsManager.Instance.isDrawLine)
        {

            //Vector3 direction = (new Vector3(Screen.width / 2, Screen.height/2, 0) - Input.mousePosition);
            //Debug.Log(Input.mousePosition);
            float edgeSize = 120;
            Vector3 tmp = Vector3.zero;


            bool right = Input.mousePosition.x > Screen.width - edgeSize;
            bool up = Input.mousePosition.y > Screen.height - edgeSize;
            bool left = Input.mousePosition.x < edgeSize;
            bool down = Input.mousePosition.y < edgeSize;
           
            float k = 0.7f;


            if (up && right)
            {
                tmp = new Vector3(k, tmp.y, -k);
                camera.transform.position += BorderChecker(tmp) / _scrollSpeed;
                return;
            }

            if (up && left)
            {
                tmp = new Vector3(k, tmp.y, k);
                camera.transform.position += BorderChecker(tmp) / _scrollSpeed;
                return;
            }

            if (down && left)
            {
                tmp = new Vector3(-k, tmp.y, k);
                camera.transform.position += BorderChecker(tmp) / _scrollSpeed;
                return;
            }

            if (down && right)
            {
                tmp = new Vector3(-k, tmp.y, -k);
                camera.transform.position += BorderChecker(tmp) / _scrollSpeed;
                return;
            }


            if (up)
            {
                tmp = new Vector3(1, tmp.y, tmp.z);
                camera.transform.position += BorderChecker(tmp) / _scrollSpeed;
            }

            if (right) 
            {
                tmp = new Vector3(tmp.x, tmp.y, -1);
                camera.transform.position += BorderChecker(tmp) / _scrollSpeed;
            }
                  
            if (left) 
            {
                tmp = new Vector3(tmp.x, tmp.y, 1);
                camera.transform.position += BorderChecker(tmp) / _scrollSpeed;
            }     
            
            if (down) 
            {
                tmp = new Vector3(-1, tmp.y, tmp.z);
                camera.transform.position += BorderChecker(tmp) / _scrollSpeed;
            }








            /*
                        if (
            Input.mousePosition.x > Screen.width - edgeSize || 
            Input.mousePosition.y > Screen.height - edgeSize || 
            Input.mousePosition.x < edgeSize || 
            Input.mousePosition.y < edgeSize) //1100
                        {
                            Vector3 tmp = new Vector3(-direction.y, 0, direction.x).normalized;   
                            camera.transform.position += BorderChecker(new Vector3(Mathf.Round(tmp.x), Mathf.Round(tmp.y), Mathf.Round(tmp.z))) / 14;
                        }*/

        }

    }

    private Vector3 BorderChecker(Vector3 vec) {

        if (!ConstrainPosition) {
            CheckSpeedCamera(vec);
            return vec; 
        }

        if (vec.x + camera.transform.position.x > MaxPosition.x || vec.x + camera.transform.position.x < MinPosition.x)
        {
            vec.x = 0;
        }

        vec.y = 0;

        if (vec.z + camera.transform.position.z > MaxPosition.z || vec.z + camera.transform.position.z < MinPosition.z)
        {
            vec.z = 0;
        }

        CheckSpeedCamera(vec);
        return vec;
    }

    private void CheckSpeedCamera(Vector3 vec) {
        if (Mathf.Abs(vec.x) > 0.05f || Mathf.Abs(vec.y) > 0.05f)
        {
            TargetsManager.Instance.securityUpdater.isMove = false;
            TargetsManager.Instance.homeUpdater.isMove = false;
        }
    }

    protected Vector3 PlanePositionDelta(Touch touch)
    {
        if (touch.phase != TouchPhase.Moved)
            return Vector3.zero;

        var rayBefore = camera.ScreenPointToRay(touch.position - touch.deltaPosition);
        var rayNow = camera.ScreenPointToRay(touch.position);
        if (Plane.Raycast(rayBefore, out var enterBefore) && Plane.Raycast(rayNow, out var enterNow)) {

            var Coor = rayBefore.GetPoint(enterBefore) - rayNow.GetPoint(enterNow);

            return BorderChecker(Coor);
        }
            
        return Vector3.zero;
    }




    private void OnDrawGizmosSelected()
    {
        if (ConstrainPosition)
        {
            var size = MaxPosition - MinPosition;
            var center = MinPosition + size / 2;
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(center, size);
        }
    }
}