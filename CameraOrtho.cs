using UnityEngine;

public abstract class CameraOrtho: MonoBehaviour
{
    [SerializeField]
    protected Camera _activeCamera;
    public Camera ActiveCamera
    {
        get { return _activeCamera; }
        set { InitializeActiveCamera(value); }
    }

    protected Transform _camTransform;

    protected ICamShaker _cameraShaker;
    protected float _maxShakeDistance;

    protected Rect? _moveBound;
    public Rect? MoveBound
    {
        get { return _moveBound; }
        set { _moveBound = value; }
    }

    [SerializeField]
    protected float MinZoom;
    [SerializeField]
    protected float MaxZoom;

    [SerializeField]
    protected float _currentZoom;
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

    protected float _zoomSpeed;
    [SerializeField] protected float _camSpeed;
    public float CamSpeed { get { return _camSpeed; } }

    public bool LockedZoom = false;

    private void Update()
    {
        OnUpdate();
    }

    public abstract void OnUpdate();
    protected abstract void InitializeActiveCamera(Camera cam);
    public virtual void SetCameraShaker(ICamShaker shaker, float maxShakeDistance)
    {
        _cameraShaker = shaker;
        _maxShakeDistance = Mathf.Clamp(maxShakeDistance, 0, float.MaxValue);
    }
    public virtual void Shake(Vector3 shakeSource, float power)
    {
        if (_camTransform != null || _cameraShaker != null) return;
        float dist = Vector2.Distance(shakeSource, _camTransform.position);
        float powermod = 0;
        if (_maxShakeDistance > dist) { powermod = dist / _maxShakeDistance; }
        if (_cameraShaker != null && powermod > 0) { _cameraShaker.SetShake(powermod * powermod); }
    }

    protected virtual void ScrollingZoom()
    {
        if (!LockedZoom)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0.0f) { _currentZoom -= scroll * _activeCamera.orthographicSize; }
        }

        if (_currentZoom <= MinZoom) { _currentZoom = MinZoom; }
        else if (_currentZoom >= MaxZoom) { _currentZoom = MaxZoom; }

        //плавная остановка
        _activeCamera.orthographicSize = Mathf.Lerp(_activeCamera.orthographicSize, _currentZoom, _currentZoom * Time.deltaTime);
        //с резкой остановкой
        //cam.orthographicSize = Mathf.MoveTowards(cam.orthographicSize, currentZoom, currentZoom);

        _camTransform.position = CameraBoundControl(_camTransform.position);
    }
    public void SetZoomRange(float minzoom, float maxzoom)
    {
        MinZoom = minzoom;
        MaxZoom = maxzoom;
        CheckZoomRange();
    }
    protected void CheckZoomRange()
    {
        if (MinZoom <= 0) MinZoom = 1;
        if (MaxZoom < MinZoom) MaxZoom = MinZoom;
    }

    protected abstract void CameraMover();
    public abstract void MoveTo(Vector2 position);
    protected Vector3 CameraBoundControl(Vector3 targetposition)
    {
        Vector3 newTarget = targetposition;
        if (_moveBound != null)
        {
            float acpect = _activeCamera.aspect;
            float widthSize = _activeCamera.orthographicSize * acpect;
            float heightSize = _activeCamera.orthographicSize;

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
