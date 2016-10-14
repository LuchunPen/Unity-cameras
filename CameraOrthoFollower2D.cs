using UnityEngine;

public class CameraOrthoFollower2D: MonoBehaviour
{
    public Camera ActiveCamera;
    private Transform _camTransform;
    private ICamShaker _cameraShaker;
    public Transform _target;

    private Rect? _moveBound;
    private int _screenMargin;

    private Vector3 _cameraPos;

    public float MinZoom;
    public float MaxZoom;
    private float _currentZoom;

    private float _zoomSpeed;
    private float _camSpeed;

    public bool LockedZoom = false;
    public float CurrentZoom
    {
        get { return _currentZoom; }
        set
        {
            if (value < MinZoom) _currentZoom = MinZoom;
            else if (value > MaxZoom) _currentZoom = MaxZoom;
            else _currentZoom = value;
        }
    }
    public Rect? MoveBound
    {
        get { return _moveBound; }
        set { _moveBound = value; }
    }

    void Awake()
    {
        if (ActiveCamera == null) { ActiveCamera = this.GetComponent<Camera>(); }
    }

	void Start ()
    {
        _camTransform = ActiveCamera.transform;
        CurrentZoom = ActiveCamera.orthographicSize;
        CheckZoomRange();
        MoveBound = new Rect(-50, -50, 100, 100);
	}
	
	void Update ()
    {
	    if (ActiveCamera != null)
        {
            float dTime = Time.deltaTime;

            if (_cameraShaker != null) { _cameraShaker.ShakeUpdater(); }
            ScrollingZoom();
            CameraZoomControl(dTime);
            CameraMover(dTime);
            _camTransform.position = _cameraPos;
        }
    }

    public void SetCameraShaker(ICamShaker shaker)
    {
        _cameraShaker = shaker;
    }
    public void Shake(float power)
    {
        if (_cameraShaker != null)
        {
            _cameraShaker.SetShake(power);
        }
    }

    public void SetTarget(Transform target, bool fastTranslate)
    {
        _target = target;
        if (fastTranslate && _target != null)
        {
            Vector3 targetPos = new Vector3(_target.position.x, _target.position.y, _camTransform.position.z);
            _camTransform.transform.position = targetPos;
        }
    }
    public void SetZoomRange(float minzoom, float maxzoom)
    {
        MinZoom = minzoom;
        MaxZoom = maxzoom;
        CheckZoomRange();
    }

    private void CheckZoomRange()
    {
        if (MinZoom <= 0) MinZoom = 1;
        if (MaxZoom < MinZoom) MaxZoom = MinZoom;
    }

    private void ScrollingZoom()
    {
        if (!LockedZoom)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0.0f) { _currentZoom -= scroll * ActiveCamera.orthographicSize; }
        }
    }
    private void CameraMover(float dTime)
    {
        if (_target != null)
        {
            Vector3 camPos = _camTransform.position;
            Vector3 targetPos = _target.position; targetPos.z = camPos.z;
            float speed = Vector2.Distance(camPos, targetPos);
            speed = Mathf.Clamp(_currentZoom, speed, 20);

            targetPos = CameraBoundControl(targetPos);
            _cameraPos = Vector3.Lerp(camPos, targetPos, dTime * speed);
        }
    }
    private void CameraZoomControl(float dTime)
    {
        if (_currentZoom <= MinZoom) { _currentZoom = MinZoom; }
        else if (_currentZoom >= MaxZoom) { _currentZoom = MaxZoom; }

        //плавная остановка
        ActiveCamera.orthographicSize = Mathf.Lerp(ActiveCamera.orthographicSize, _currentZoom, _currentZoom * dTime);
        //с резкой остановкой
        //cam.orthographicSize = Mathf.MoveTowards(cam.orthographicSize, currentZoom, currentZoom);
    }
    private Vector3 CameraBoundControl(Vector3 target)
    {
        Vector3 newTarget = target;
        if (_moveBound != null)
        {
            float acpect = ActiveCamera.aspect;
            float widthSize = ActiveCamera.orthographicSize * acpect;
            float heightSize = ActiveCamera.orthographicSize;

            if (widthSize * 2 >= _moveBound.Value.width) { newTarget.x = _moveBound.Value.center.x; }
            else
            {
                if (newTarget.x - widthSize < _moveBound.Value.min.x) { newTarget.x = _moveBound.Value.min.x + widthSize; }
                else if (newTarget.x + widthSize > _moveBound.Value.max.x) { newTarget.x = _moveBound.Value.max.x - widthSize; }
            }

            if (heightSize * 2 >= _moveBound.Value.height) { newTarget.y = _moveBound.Value.center.y; }
            else
            {
                if (newTarget.y - heightSize < _moveBound.Value.min.y) { newTarget.y = _moveBound.Value.min.y + heightSize; }
                else if (newTarget.y + heightSize > _moveBound.Value.max.y) { newTarget.y = _moveBound.Value.max.y - heightSize; }
            }
        }
        return newTarget;
    }
}
