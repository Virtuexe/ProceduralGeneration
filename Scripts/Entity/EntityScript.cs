using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using static UnityEngine.GraphicsBuffer;

public class EntityScript : MonoBehaviour
{
	public CharacterController controller;
	public Transform head;
	//VARIABLES
	//movement
	public int jumpSpeed;
	public float strafe;
	public float speed;
	public float acceleration;
	public float rotationSpeed;
	public float lookUpSpeed;
	public float viewPitchRange;
	public float gravity;
	//VALUES
	float pitch = 0;
	Vector2 walkDirection = Vector2.zero;
	public Vector3 velocity = Vector3.zero;
	public void Move(Vector2 direction)
	{
		this.walkDirection = direction;
	}
	public void Jump()
	{
		if (controller.isGrounded)
		{
			velocity.y = jumpSpeed;
		}

	}
	public void Look(Vector2 look)
	{
		//yaw (look.x)
		float yaw = look.x * rotationSpeed * Time.deltaTime;
		transform.Rotate(new Vector3(0, yaw, 0));

		//pitch (look.y)
		pitch += look.y * lookUpSpeed * Time.deltaTime;
		pitch = Mathf.Clamp(pitch, -viewPitchRange, viewPitchRange);
		head.transform.localRotation = Quaternion.Euler(-pitch, 0, 0);
	}
	public void Update()
	{
		Movement();
	}
	private void Movement()
	{
		if (!controller.isGrounded)
		{
			velocity.y -= gravity * Time.deltaTime;
			velocity += controller.velocity;
		}
		else
		{
			Vector3 desiredDirection = (transform.right * walkDirection.x + transform.forward * walkDirection.y).normalized;
			Vector3 desiredVelocity = desiredDirection * speed;
			velocity += Vector3.MoveTowards(controller.velocity, desiredVelocity, acceleration * Time.deltaTime);
			velocity.y -= 0.5f;
		}
		// Move the controller
		controller.Move(velocity * Time.deltaTime);
		velocity = Vector3.zero;
	}
}
