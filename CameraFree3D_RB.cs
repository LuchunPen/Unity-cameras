/*
Copyright (c) Luchunpen (bwolf88).  All rights reserved.
Date: 05/02/2016 21:46
*/

using System;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
[AddComponentMenu("Cameras/CameraFree3D_RB")]
public class CameraFree3D_RB: MonoBehaviour
{
    //private static readonly string stringID = "EBC6F74AF0FBEC02";
    private Transform _camTrans;
    private Transform _myTrans;
    private Rigidbody _rb;

    [SerializeField]private Camera ActiveCamera;
    [SerializeField]private float _camSpeed = 10;
    private float _camSpeedCur; 
    [SerializeField]private float mouseSensitivity = 200;
    [SerializeField]private float acceleration = 1.01f;

    public bool RBActive = false;

    void Start ()
	{
	    if (ActiveCamera == null) { throw new Exception("Add camera"); }

        _camTrans = ActiveCamera.transform;

        _myTrans = this.transform;
        _rb = this.GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
        _rb.isKinematic = true;
        _camSpeedCur = _camSpeed;
    }
	
	void Update ()
	{
        Move();
        KeyControll();
    }

    private void Move()
    {
        float speedMod = _camSpeedCur * 0.0166f;
        float sensMod = mouseSensitivity * 0.0166f;

        if (Input.GetMouseButton(1))
        {
            _camTrans.Rotate(-Input.GetAxis("Mouse Y") * sensMod, 0, 0, Space.Self);
            _myTrans.Rotate(0, Input.GetAxis("Mouse X") * sensMod, 0, Space.World);
        }

        _myTrans.Translate(Vector3.right * Input.GetAxis("Horizontal") * speedMod, Space.Self);

        if (RBActive) { _myTrans.position += _myTrans.forward * Input.GetAxis("Vertical") * speedMod; }
        else
        {
            if (Input.GetKey(KeyCode.Space)) { _myTrans.position += Vector3.up * speedMod; }
            else { _myTrans.position += _camTrans.forward * Input.GetAxis("Vertical") * speedMod; }
        }
    }

    private void KeyControll()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (RBActive)  { RBActive = false; _rb.isKinematic = true; }
            else  { RBActive = true;  _rb.isKinematic = false;  }
        }

        if (RBActive)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            { _rb.AddForce(0,_rb.mass * 5,0,ForceMode.Impulse); }
        }
        
        if (Input.GetKey(KeyCode.LeftShift))
        {
            float toSpeed = _camSpeed * 2.5f;
            if (_camSpeedCur < toSpeed) { _camSpeedCur *= acceleration; }
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _camSpeedCur = _camSpeed;
        }
    }
}
