using UnityEngine;
using System.Collections;

public class CharacterMovementController : MonoBehaviour {

	// Use this for initialization
	public float speed;
	public float walkspeed;
	public float runSpeed;
	public float jumpSpeed;
	
	Vector3 moveDirection = Vector3.zero;
	
	public Vector3 movementSpeed = Vector3.zero;
	
	CharacterController controller;
	
	Animator animator;
	
	
	void Start(){
		animator = GetComponent<Animator>();
		controller = GetComponent<CharacterController>();
	}
	
	void Update() {
		
		transform.Rotate(0,Input.GetAxis("Mouse X"),0);
		
		
		if (controller.isGrounded) {
			// We are grounded, so recalculate
			// move direction directly from axes
			moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			movementSpeed = moveDirection;
			
			if(Input.GetButton("Sprint")){
				speed = Mathf.Lerp(speed, runSpeed, Time.deltaTime * 2);
			}else{
				speed = Mathf.Lerp(speed, walkspeed, Time.deltaTime * 2);
			}
			movementSpeed *= speed;
			animator.SetFloat("MovementX", movementSpeed.x);
			animator.SetFloat("MovementZ", movementSpeed.z);
			
			moveDirection = transform.TransformDirection(moveDirection);
			moveDirection *= speed;
			}
}