using UnityEngine;
using System.Collections;

/*
    ChrAnimatorControl is script to control the characters in demoscene.
    will move character , play animation , the position of the weapon , play effect , reaction of key input.
    
	ChrAnimatorControlはデモシーンに配置されたキャラクターを制御するスクリプトです。.
    キャラクターの移動、アニメーションの再生、武器の表示位置、エフェクトの発生、キー入力に対するリアクションを行います。.

    ChrAnimatorControl은 데모신에 배치된 캐릭터를 제어하기위한 스크립트 입니다.
    캐릭터의 이동, 애니메이션의 재생, 무기의 위치, 이펙트의 발생, 키 입력에 대한 반응을 합니다. 
	
	2015.03.01
*/

public class NoteAnimatorControl : MonoBehaviour {

	// required Object or component
	//　制御に必要なオブジェクトなど。
	//　필요한 컴포넨트, 오브젝트 등등.
	public Animator chrAnimator;    // Animator component of character.
	public RuntimeAnimatorController[] chrAnimatorController;// AnimatorController for viewer and interactive
	public CharacterController chrController;    // CharacterController component.
	public GameObject[] meshData; // character and weapon , the object mesh data is included.

	// to control movement of characters , such as jumps.
	//　キャラクターの移動や、アニメータをコントロールするもの。.
	//　캐릭터의 이동, 점프등을 제어하기 위해서 필요한것.
	public float jumpSpeed = 8.0f;
	private float jumpInput = 0.0f;
	private float runParam = 0.0f;
	private Vector3 moveDirection = Vector3.zero;
	private float gravity = 10.0f;
	private AnimatorStateInfo stateInfo; // Save the state in playing now.
	
	void Update() 
	{
		// Save the state in playing now.
		//　再生中のステートの情報を入れる。.
		// 재생중인 스테이트를 저장.
		stateInfo = chrAnimator.GetCurrentAnimatorStateInfo(0);

		// Bool parameter reset to false. 
		if(!stateInfo.IsTag("InIdle")){
			chrAnimator.SetBool("LookAround", false);
			chrAnimator.SetBool("Attack", false);
			chrAnimator.SetBool("Jiggle", false);
			chrAnimator.SetBool("Dead", false);
		}
		
		// reaction of key input.
		// キー入力に対するリアクションを起こす。.
		// 키입력에 대한 반응.
		// for Attack
		if(Input.GetButtonDown("Fire1"))	chrAnimator.SetBool("Attack", true);
		// LookAround
		if(Input.GetKeyDown("z"))	chrAnimator.SetBool("LookAround", true);
		// Jiggle
		if(Input.GetKeyDown("x"))	chrAnimator.SetBool("Jiggle", true);
		// Happy!!
		if(Input.GetKeyDown("c"))
		{
			chrAnimator.SetBool("Happy", !chrAnimator.GetBool("Happy"));
			if(chrAnimator.GetBool("Happy") == true)	chrAnimator.SetBool("Sad", false);
		}
		// Sad
		if(Input.GetKeyDown("v"))
		{
			chrAnimator.SetBool("Sad", !chrAnimator.GetBool("Sad"));
			if(chrAnimator.GetBool("Sad") == true)	chrAnimator.SetBool("Happy", false);
		}
		// for Dead
		if(Input.GetKeyDown("b"))	chrAnimator.SetBool("Dead", true );

		
		// movement.
		// Input of character moves	
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");
		
		Vector3 axisInput = new Vector3(h, 0, v);
		float axisInputMag = axisInput.magnitude;
		if(axisInputMag > 1){
			axisInputMag = 1;
			axisInput.Normalize();
		}
		runParam = 0f;
		if(axisInputMag != 0){
			// for run
			if(Input.GetButton("Fire2")){
				runParam = 1.0f;
			}
			// character rotate 
			axisInput = Camera.main.transform.rotation * axisInput;
			axisInput.y = 0;
			transform.forward = axisInput;
		}
		chrAnimator.SetFloat("Speed", (axisInputMag + runParam));
		
		// Jump
		// while in jump, I am using Character Controller instead Root Motion, to move the Character.
		// ジャンプ時は、キャラクターコントローラを使ってキャラクターを移動させます。.
		// 점프시에는 캐릭터 컨트롤러를 이용하여 캐릭터를 이동시키고 있습니다.	
		// in ground.
		if(chrController.isGrounded){
			// jump parameter set to false.
			chrAnimator.SetInteger("Jump", 0);
			// moveDirection set 0, to prevent to move by Character controller.
			// moveDirectionはゼロにして、キャラクターコントローラがキャラクターを動かさないように。.
			// moveDirection은 0으로 돌려서, 캐릭터 컨트롤러가 캐릭터를 움직이지 않도록한다.
			moveDirection = new Vector3(0, jumpInput, 0);
			
			// press Jump button. make jump
			// if Animator parameter "Jump" is true, 
			// animator will play state of "na_Jump_00" and "na_Jump_00_up"
			// then animation event of "na_Jump_00_up" will call SetJump()
			// Jumpパラメータからアニメーションが遷移し、.
			// "na_Jump_00_up"のときにイベントでSetJump()ファンクションを呼ぶ。.
			// Jump파라메터를 통해 스테이트가 점프애니메이션을 재생하고,
			// "na_Jump_00_up"스테이트를 재생할때 SetJump()를 부른다.
			if(Input.GetButtonDown("Jump"))
				SetJump ();
		}
		// While in Air
		else if(!chrController.isGrounded){
			// press Jump button. can jump once more.
			if(Input.GetButtonDown("Jump"))
				SetJump ();

			// It is moved with Character Controller while in the air,
			// moveDirection is use Axis Input.
			// 空中にいるときはmoveDirectionを使って移動するので、.
			// 方向キーの入力を渡しておく。.
			// 공중에 있는 동안은 캐릭터 컨트롤러를 사용하여 이동하기때문에.
			// 방향키의 입력을 moveDirection에게 전달해준다.
			moveDirection = new Vector3(transform.forward.x * axisInputMag * 4, moveDirection.y, transform.forward.z * axisInputMag * 4);
			moveDirection.y -= gravity * Time.deltaTime;
		}
		
		// character is move by moveDirection.
		chrController.Move(moveDirection * Time.deltaTime);
	}

	// change Animator Controller.
	// this function is called from GUIControl.
	// アニメータコントローラを変更する。 .
	// GUIControlスクリプトから呼ばれる。 .
	// ビューアモード、インタラクティブモードが切り替わるときに、各モード用にアニメータコントローラを差し替える。.
	// 애니메이터 컨트롤러를 변경한다.
	// GUIControl 스크립트로부터 불려진다.
	// 뷰어모드, 인터렉티브 모드 사이를 오갈때, 각각의 모드에 맞는 애니메이터를 설정한다.
	public void ControllerChange(int idx){
		chrAnimator.runtimeAnimatorController = chrAnimatorController[idx];
		if(this.gameObject.activeSelf)
			PlayClip("Appear");
	}

	// play animation state.
	// for viewer mode
	public void PlayClip(string stateName){
		chrAnimator.CrossFade( stateName, 0.05f);
	}

	// when pressed jump button
	void SetJump(){
		// when in ground.
		if(chrAnimator.GetInteger("Jump") == 0){
			// execute only when State tag is InIdle or InMove , moveDirection use jumpSpeed.
			if(stateInfo.IsTag("InIdle") || stateInfo.IsTag("InMove")){
				chrAnimator.SetInteger("Jump", 1);
				moveDirection.y += jumpSpeed;
			}
		}
		// when in air, can jump once more.
		else if(chrAnimator.GetInteger("Jump") == 1){
			// jump with half power
			moveDirection.y += jumpSpeed /2;
			if(chrController.velocity.y < 0){
				moveDirection.y -= chrController.velocity.y;
			}
			chrAnimator.SetInteger("Jump", 2);
		}
	}

	// read 3D model information.
	// vertex count, triangles, and joint of character and weapon.
	// this function is called from GUIControl.
	public string GetMeshData(){
		int[] verts = new int[2]; // vertex.
		int[] tris = new int[2]; // triangle.
		int[] bones = new int[2]; // joint.
		string mdlInfo = "  " + gameObject.name + "\n"; // text.
		for(int i = 0; i < meshData.Length; i++){
			SkinnedMeshRenderer skinnedMeshData = meshData[i].GetComponent<SkinnedMeshRenderer>();
			// skinned model.
			if(skinnedMeshData){
				verts[i] = skinnedMeshData.sharedMesh.vertices.Length;
				tris[i] = skinnedMeshData.sharedMesh.triangles.Length / 3;
				bones[i] = skinnedMeshData.bones.Length;
				mdlInfo += "\nVertex : " + verts[i].ToString();
				mdlInfo += " ,  Tris : " + tris[i].ToString();
				mdlInfo += " ,  Bones : " + bones[i].ToString();
			}
			// mesh only.
			else{
				verts[i] = meshData[i].GetComponent<MeshFilter>().sharedMesh.vertices.Length;
				tris[i] = meshData[i].GetComponent<MeshFilter>().sharedMesh.triangles.Length / 3;
				bones[i] = 0;
				mdlInfo += "\nVertex : " + verts[i].ToString();
				mdlInfo += " ,  Tris : " + tris[i].ToString();
				mdlInfo += " ,  Bones : no use.";
			}
		}
		return (mdlInfo);
	}
	
	// this function is called from GUIControl.
	public void SetShader(string ShaderName){
		for(var i = 0; i < meshData.Length; i++){
			SkinnedMeshRenderer skinnedMeshData = meshData[i].GetComponent<SkinnedMeshRenderer>();
			if(skinnedMeshData){
				skinnedMeshData.material.shader = Shader.Find(ShaderName);
			}
			else{
				meshData[i].GetComponent<MeshRenderer>().material.shader = Shader.Find(ShaderName);
			}
		}
	}

	// change Color of material
	// this function is called from GUIControl.
	public void SetColor(){
		Color newColor;
		newColor.r = (Random.Range(0, 17) / 16.0f);
		newColor.g = (Random.Range(0, 17) / 16.0f);
		newColor.b = (Random.Range(0, 17) / 16.0f);
		newColor.a = 1.0f;
		
		for(var i = 0; i < meshData.Length; i++){
			SkinnedMeshRenderer skinnedMeshData = meshData[i].GetComponent<SkinnedMeshRenderer>();
			if(skinnedMeshData){
				skinnedMeshData.material.color = newColor;
			}
			else{
				meshData[i].GetComponent<MeshRenderer>().material.color = newColor;
			}
		}
	}
}
