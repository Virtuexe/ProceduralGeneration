using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
	//movement
	public float maxHealth = 100;
	public float speed = 10;
	public float jumpSpeed = 5;
	public float rotationSpeed = 100;
	public float lookUpSpeed = 100;
	public float viewPitchRange = 90f;
	public float gravity = 20;
	//outside script
	public HealthSystemScript health;
	public InventoryObject inventory;
	public InteractablePropertiesScript properties;

	public GameObject inventoryTab;
	public GameObject hudInteractBar;
	public GameObject hudInteractBarSlider;

	public bool inventoryOpen;



	public GameObject player;
	public GameObject hand;
	public GameObject item;

	public InteractableScript _selectedObject;

	public InputActionAsset playerInput;
	public InputAction looking;
	public InputAction use;
	public InputAction primary;
	public InputAction secondary;
	public InputAction movement;
	public InputAction selecting;


	float pitch = 0;
	public Vector2 direction = Vector2.zero;
	public Vector3 moveDirection = Vector3.zero;
	public Vector2 look = Vector2.zero;
	public Vector3 lookDirection = Vector3.zero;


	public Camera fpsCamera;
	public CharacterController controller;
	void Awake()
	{
		health = new HealthSystemScript(maxHealth);
		var gameplayActionMap = playerInput.FindActionMap("Player");
		movement = gameplayActionMap.FindAction("Walking");
		movement.performed += OnMovementChanged;
		movement.canceled += OnMovementChanged;
		movement.Enable();
		var jumping = gameplayActionMap.FindAction("Jumping");
		jumping.performed += OnJumpChanged;
		jumping.Enable();
		use = gameplayActionMap.FindAction("Use");
		use.performed += context =>OnUsePreformed(context, 0);
		use.canceled += context => OnUseCanceled(context, 0);
		use.Enable();
		primary = gameplayActionMap.FindAction("Primary");
		primary.performed += context =>OnUsePreformed(context,1);
		primary.canceled += context => OnUseCanceled(context, 1);
		primary.Enable();
		secondary = gameplayActionMap.FindAction("Secondary");
		secondary.performed += context => OnUsePreformed(context, 2);
		secondary.canceled += context => OnUseCanceled(context, 2);
		secondary.Enable();
		looking = gameplayActionMap.FindAction("Looking");
		looking.performed += OnMouseMovement;
		looking.Enable();
		var inventory = gameplayActionMap.FindAction("Inventory");
		inventory.performed += OnInventoryPreformed;
		inventory.Enable();
		//slots
		var slot = gameplayActionMap.FindAction("Slot");
		slot.performed += OnSlotPreformed;
		slot.Enable();
		hudInteractBar.SetActive(false);
	}
	void Start()
	{
		health.damage(10);
        inventoryTab.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}
	void OnSlotPreformed(InputAction.CallbackContext context)
	{
		var value = (int)context.ReadValue<float>();
		if (inventory.container.ItemList().Count <= value || inventory.container.ItemList()[value] == null)
			return;
		if (item != null)
		{
			Destroy(item.gameObject);
		}
		item = Instantiate(inventory.container.ItemList()[value].item.prefab,new Vector3(0,0,0), Quaternion.identity);
		item.transform.parent = hand.transform;
		item.transform.localPosition = new Vector3(0, 0,0);
		item.transform.localRotation = new Quaternion(0,0,0,0);
		item.transform.localScale = new Vector3(1, 1, 1);
		var itemInteraction = item.GetComponent<InteractableScript>();
		itemInteraction.properties = properties;
		foreach (Action a in itemInteraction.actions.actionList)
		{
			properties.actions.actionList.Add(a);
        }
		
	}
	void OnMovementChanged(InputAction.CallbackContext context)
	{
		direction = context.ReadValue<Vector2>();
	}
	void OnJumpChanged(InputAction.CallbackContext context)
    {
		if (controller.isGrounded)
		{
			moveDirection.y = jumpSpeed;
		}
	}
	void OnUsePreformed(InputAction.CallbackContext context, int value)
    {
		Action action = null;
		foreach (Action i in properties.actions.actionList)
		{
			if (i.type == (ActionType)value)
			{
				if (action == null || action.priority < i.priority)
				{
					action = i;
				}
			}
		}
		if (action != null)
			action.action.Invoke(true);
	}
	void OnUseCanceled(InputAction.CallbackContext context, int value)
	{
		Action action = null;
		foreach (Action i in properties.actions.actionList)
		{
			if (i.type == (ActionType)value)
			{
				if (action == null || action.priority < i.priority)
				{
					action = i;
				}
			}
		}
		if (action != null)
			action.action.Invoke(false);
	}

	void OnMouseMovement(InputAction.CallbackContext context)
	{
		look = context.ReadValue<Vector2>();

		//yaw (look.x)
		float yaw = look.x * rotationSpeed * Time.deltaTime;
		transform.Rotate(new Vector3(0, yaw, 0));

		//pitch (look.y)
		pitch += look.y * lookUpSpeed * Time.deltaTime;
		pitch = Mathf.Clamp(pitch, -viewPitchRange, viewPitchRange);
		fpsCamera.transform.localRotation = Quaternion.Euler(-pitch, 0, 0);

		//fpsCamera.transform.Rotate(new Vector3(-pitch, 0, 0));
		//Debug.Log(fpsCamera.transform.localEulerAngles);

	}
	void OnInventoryPreformed(InputAction.CallbackContext context)
    {
		//open inventory or close
		inventoryOpen = !inventoryOpen;
		inventoryTab.SetActive(inventoryOpen);
		Cursor.visible = inventoryOpen;
		if (inventoryOpen)
		{
			looking.Disable();
			use.Disable();
			primary.Disable();
			selecting.Enable();
			Cursor.lockState = CursorLockMode.Confined;

		}
			
		else
        {
			looking.Enable();
			use.Enable();
			primary.Enable();
			secondary.Enable();
			selecting.Disable();
			Cursor.lockState = CursorLockMode.Locked;
		}
	}
	void Update()
	{
		// Directional movement & jumping
		if (controller.isGrounded)
		{
			moveDirection = new Vector3(direction.x, moveDirection.y, direction.y);
			moveDirection = transform.TransformDirection(moveDirection);
			moveDirection.x *= speed;
			moveDirection.z *= speed;
		}
		else
		{
			moveDirection.y -= gravity * Time.deltaTime;
		}
		
		controller.Move(moveDirection * Time.deltaTime);
		//selecting
		var ray = fpsCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit))
		{
			var selection = hit.transform;
			var selectedObject = selection.GetComponent<InteractableScript>();
			if (_selectedObject != selectedObject && selectedObject != null)
			{
				if (_selectedObject != null)
				{
					foreach (Action a in _selectedObject.actions.actionList)
					{
						///set to false only already Invoked actions
						a.action.Invoke(false);
						properties.actions.actionList.Remove(a);
					}
					_selectedObject = null;
				}

				selectedObject.properties = properties;
				foreach (Action a in selectedObject.actions.actionList)
                {
					properties.actions.actionList.Add(a);
                }
				_selectedObject = selectedObject;
			}
			else if (selectedObject != _selectedObject && _selectedObject != null)
			{
				foreach (Action a in _selectedObject.actions.actionList)
				{
					///set to false only already Invoked actions
					a.action.Invoke(false);
					properties.actions.actionList.Remove(a);
				}
				_selectedObject = null;
			}
		}
		else if (_selectedObject != null)
        {
			foreach (Action a in _selectedObject.actions.actionList)
			{
				///set to false only already Invoked actions
				a.action.Invoke(false);
				properties.actions.actionList.Remove(a);
			}
			_selectedObject = null;
		}
	}
    private void OnApplicationQuit()
    {
		inventory.container.ItemListClear();
    }

}
