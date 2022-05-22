using UnityEngine;
using System.Collections;


/*
	CamControl is the scrpipt for the control of camera rig.
	
    CamControlはデモシーンにおいて、カメラと照明が含まれた__Camというオブジェクト群を操作し、.
    常に画面上にキャラクターが移るようにするためのスクリプトです。.
	
	CamControl은 데모신의 카메라, 조명이 포함된 __Cam 오브젝트를 제어하여
	화면상에 항상 캐릭터를 비추도록 하는 스크립트입니다.
	
	2016.04.23
*/



public class CamControl : MonoBehaviour {

	// Object for Control
	// スクリプトで制御するオブジェクト.
	// 스크립트가 제어하는 오브젝트 
	public Camera cam; // Main camera
	public Transform pivot; // SubObject for vertical rocation
	public Transform target; // Main Character
	public Transform ground; // Ground
	
	// for Camera Rotation
	public float rotateSpeed = 10.0f; // how fast rotate cam.
	public float tiltMax = 40.0f; // limit of vertical rotation 
	public float tiltMin = 30.0f;

	private bool rotateEnable = true; // Allows rotation of the camera
	private bool UIArea = false; // Allows rotation of the camera
	public bool AutoRotate = false;
	private Vector3 rotation; // Input value of  the left click and drag 
	
	// Used to control the camera zoom
	public int[] zoom = new int[2]; // Camera's FOV values​​ to zoom.
	public float smooth = 5f; // speed to zoom

	private int zoomIdx = 0;
	
	
	void Update () {
		// When Left Click & Drag screen.
		// 左クリック＆ドラッグ時の処理。
		// 왼쪽클릭 드래그시에 rotation에 입력치를 저장합니다.
		if (Input.GetMouseButton(0)) {
			rotation.y = Input.GetAxis("Mouse X") * rotateSpeed;
			rotation.x = Input.GetAxis("Mouse Y") * rotateSpeed;
		}
		else{
			rotation = Vector3.zero;
		}
		
		// Camera rotates by rotation variable.
		if(rotateEnable && !UIArea){
			CamRotate(rotation);
		}
		if(AutoRotate){
			CamRotate(new Vector3(0, rotateSpeed * 3 * Time.deltaTime, 0));
		}

		
		// When Right Click.
		// Camera zoom.
		// 右クリック時は、画面をズームさせます.
		// 오른쪽 클릭시에는 카메라 줌을 한다.
		if(Input.GetMouseButtonDown(1)){
			CamZoom();
		}
		cam.fieldOfView = Mathf.Lerp(cam.fieldOfView,zoom[zoomIdx],Time.deltaTime*smooth);
	}
	
	// In LateUpdate, camera and ground are move to place of character model.
	// LateUpdateでは、キャラクターの位置を見て、カメラと、地面を移動させます。.
	// LateUpdate에서는 캐릭터모델을 쫓아 카메라와 지면을 이동시킨다.
	void LateUpdate() {	
		// Show that ground like infinity place, 
		// Distance of ground and character leaves 5 ​​or more , ground move.
		// キャラクター(target)と、地面(ground)の位置差が5以上離れると、地面を5移動させる。.
		// 地面(ground)が無限に続いているような気持になる。.
		// 지면이 무한히 이어져있는듯이 보이도록하기위해,
		// 캐릭터와 지면이 5이상 떨어졌을때, 지면을 5이동시킨다.
		Vector3 groundPos = ground.position;
		if ((target.position.x - ground.position.x) >= 5f) {
			groundPos.x += 5f;
			ground.position = groundPos;
		}
		else if((target.position.x - ground.position.x) <= -5f){
			groundPos.x -= 5f;
			ground.position = groundPos;
		}
		if((target.position.z - ground.position.z) >= 5f){
			groundPos.z += 5f;
			ground.position = groundPos;
		}
		else if((target.position.z - ground.position.z) <= -5f){
			groundPos.z -= 5f;
			ground.position = groundPos;
		}

		// __cam always follow characer(target object).
		transform.position = target.position;
	}	
	
	// Function for rotate Camera Rig.
	void CamRotate(Vector3 rot){
		// horizontal rotate 
		// 横回転.
		// 수평 회전 
		float newRot = transform.rotation.eulerAngles.y + rot.y;
		transform.rotation = Quaternion.Euler (0f, newRot, 0f);
		
		// vertical rotate 
		// 縦の回転は、水平を保ったままにしたいので.
		// 本体ではなくpivotオブジェクトを回す。.
		// 수직 회전 , 카메라가 수평을 유지한채 회전하기위해
		// 본체가 아닌 pivot오브젝트를 회전시킨다.	
		Vector3 tiltAngle = new Vector3((rot.x * 3f), 0f, 0f);
		pivot.Rotate(tiltAngle, Space.Self);

		float pivotX = pivot.localRotation.eulerAngles.x;
		if(pivotX > 180f){
			pivotX -= 360f;
			if( pivotX < -tiltMin)
				pivot.localRotation = Quaternion.Euler (-tiltMin, 0f, 0f);
		}
		else if(pivotX > tiltMax)
			pivot.localRotation = Quaternion.Euler (tiltMax, 0f, 0f);
	}
	
	
	// Function for camera zoom
	// FOV value is in zoom array.
	// This function is just change zoomIdx value.
	// CamZoom() is called from GUIControl or Right click.
	public void CamZoom(){
		zoomIdx++;
		zoomIdx = (int)Mathf.Repeat(zoomIdx, zoom.Length);
	}
	
	// RotateOption () is called from GUIControl.
	// It is control the rotate enable or disable.
	public void RotateOption(bool enable){
		rotateEnable = enable;
	}

	// Event Trigger use.
	// if Mouse pointer is hovering on uGUI, UIArea is set to true.
	// if UIArea is true, left click drag will not affect camera rotation.
	public void isUIArea(bool param){
		UIArea = param;
	}


}
