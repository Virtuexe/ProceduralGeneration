using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityScript : MonoBehaviour
{
	public CharacterController controller;
	public Transform head;
	//VARIABLES
	//movement
	public int jumpSpeed;
	public float strafe;
	private float _speed;
	public float walkSpeed;
	public float sprintSpeed;
	public bool sprinting;
	public float energy;
	public float _energy;
	public float energyRegen;
	public float sprintEnergyConsumption;
	public float jumpEnergyConsumption;
	public float acceleration;
	public float rotationSpeed;
	public float lookUpSpeed;
	public float viewPitchRange;
	public float gravity;
	//VALUES
	float pitch = 0;
	Vector2 walkDirection = Vector2.zero;
	public Vector3 velocity = Vector3.zero;
	private void Start() {
		energy = _energy;
	}
	public void Move(Vector2 direction)
	{
		this.walkDirection = direction;
	}
	public void Jump()
	{
		if (controller.isGrounded && _energy >= jumpEnergyConsumption)
		{
			_energy -= jumpEnergyConsumption;
			Vector3 desiredDirection = (transform.right * walkDirection.x + transform.forward * walkDirection.y).normalized;
			Vector3 desiredVelocity = desiredDirection * jumpSpeed;
			velocity += Vector3.MoveTowards(controller.velocity, desiredVelocity, acceleration * Time.deltaTime);
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
			ResolveSprint();
			Vector3 desiredDirection = (transform.right * walkDirection.x + transform.forward * walkDirection.y).normalized;
			Vector3 desiredVelocity = desiredDirection * _speed;
			velocity += Vector3.MoveTowards(controller.velocity, desiredVelocity, acceleration * Time.deltaTime);
			velocity.y -= 0.5f;
		}
		// Move the controller
		controller.Move(velocity * Time.deltaTime);
		velocity = Vector3.zero;
	}
	private void ResolveSprint() {
		if (sprinting && _energy >= sprintEnergyConsumption * Time.deltaTime) {
			_energy -= sprintEnergyConsumption * Time.deltaTime;
			_speed = sprintSpeed;
		}
		else {
			if(_energy >= energy) {
				_energy = energy;
			}
			else {
				_energy += energyRegen * Time.deltaTime;
			}
			_speed = walkSpeed;
		}
	}
}
