using System;
using UnityEngine;

public class CameraOrthoMove2D: CameraOrtho
{
    public static readonly int DefaultScreenMargin = 10;
    public static readonly float DefaultCameraSpeedMod = 5;

    [SerializeField]
    private int _screenMargin;
    public int ScreenMargin
    {
        get { return _screenMargin; }
        set { _screenMargin = Mathf.Clamp(value, 0, 20); }
    }

    private Vector3 _clickedPosition;

    private void Start ()
    {
        InitializeActiveCamera(_activeCamera);

        if (_camSpeed <= 0 || _camSpeed > 50) { _camSpeed = DefaultCameraSpeedMod; }
        if (_screenMargin < 0) { _screenMargin = DefaultScreenMargin; }
	}
	
	public override void OnUpdate ()
    {
        if (_activeCamera != null)
        {
            if (_cameraShaker != null) { _cameraShaker.ShakeUpdater(); }
            ScrollingZoom();
            CameraMover();
        }
    }

    protected override void InitializeActiveCamera(Camera cam)
    {
        _activeCamera = cam;
        if (_activeCamera != null)
        {
            _camTransform = _activeCamera.transform;
            _activeCamera.orthographic = true;
            CurrentZoom = _activeCamera.orthographicSize;
            CheckZoomRange();
        }
        else { _camTransform = null; }
    }

    protected override void CameraMover()
    {
        Vector3 mousePos = _activeCamera.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(2)) { _clickedPosition = mousePos; }

        if (Input.GetMouseButton(2)){
            CameraMouseClickMove(_camSpeed * 0.3f * Time.deltaTime, mousePos);
        }
        else if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0){
            CameraKeyMove(_camSpeed * Time.deltaTime);
        }
        else {
            ScreenMarginMove(_camSpeed * Time.deltaTime * 0.3f);
        }
    }

    private void CameraKeyMove(float speed)
    {
        if (speed < 0.1f) { speed = 0.1f; }

        Vector3 cameraPos = _camTransform.position;
        Vector3 targetPos = cameraPos;
        targetPos += Vector3.right * Input.GetAxis("Horizontal") * speed;
        targetPos += Vector3.up * Input.GetAxis("Vertical") * speed;
        targetPos = CameraBoundControl(targetPos);
        _camTransform.position = Vector3.Lerp(cameraPos, targetPos, speed);
    }

    private void ScreenMarginMove(float speed)
    {
        if (_screenMargin <= 0) return;

        Vector2 screenPosition = InvertedWorldToScreenPoint(_activeCamera.ScreenToWorldPoint(Input.mousePosition));
        Vector3 cameraPos = _camTransform.position;

        if (screenPosition.x < _screenMargin) { cameraPos -= Vector3.right * speed; }
        else if (screenPosition.x > Screen.width - _screenMargin) { cameraPos += Vector3.right * speed; }
        if (screenPosition.y < _screenMargin) { cameraPos += Vector3.up * speed; }
        else if (screenPosition.y > Screen.height - _screenMargin) { cameraPos -= Vector3.up * speed; }

        _camTransform.position = CameraBoundControl(cameraPos);
    }
    private void CameraMouseClickMove(float speed, Vector3 mousePos)
    {
        //резкое движение
        Vector3 cameraPosition = _camTransform.position;
        Vector3 targetPosition = Vector3.Lerp(cameraPosition, cameraPosition + (_clickedPosition - mousePos), speed);
        targetPosition = CameraBoundControl(targetPosition);
        _camTransform.position = targetPosition;
        //плавное движение
        //cameraPos = Vector3.MoveTowards(camTransform.position, cameraPos + (clickedMousePos - mousePos), speed * 2);
    }

    private Vector2 InvertedWorldToScreenPoint(Vector3 worldPos)
    {
        if (_activeCamera != null)
        {
            Vector3 vector = _activeCamera.WorldToScreenPoint(worldPos);
            vector.y = (float)Screen.height - vector.y;
            return new Vector2(vector.x, vector.y);
        }
        return worldPos;
    }

    public override void MoveTo(Vector2 position)
    {
        if (_activeCamera != null)
        {
            _camTransform.position = new Vector3(position.x, position.y, _camTransform.position.z);
        }
    }
}
