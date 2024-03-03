using UnityEngine.InputSystem;
using UnityEngine;
using Generation;
using PathFinding;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour {
	public GeneratorManagerScript manager;

	public EntityScript entity;
	public InventoryObject inventory;
	public InteractablePropertiesScript properties;
	//UI
	public GameObject inventoryTab;
	public GameObject hudInteractBar;
	public GameObject hudInteractBarSlider;

	public bool inventoryOpen;

	public GameObject player;
	public GameObject hand;
	public GameObject item;

	public InteractableScript _selectedObject;

	public HudManager hud;
	//keybindings
	public InputActionAsset playerInput;
	public InputAction looking;
	public InputAction use;
	public InputAction sprint;
	public InputAction primary;
	public InputAction secondary;
	public InputAction movement;
	public InputAction jumping;
	public InputAction inventoryIn;
	public InputAction selecting;
	public InputAction slotIn;


	public Camera fpsCamera;
	public CharacterController controller;
	void Awake() {
		var gameplayActionMap = playerInput.FindActionMap("Player");
		movement = gameplayActionMap.FindAction("Walking");
		movement.performed += OnMovementChanged;
		movement.canceled += OnMovementChanged;
		movement.Enable();
		jumping = gameplayActionMap.FindAction("Jumping");
		jumping.performed += OnJumpChanged;
		jumping.Enable();
		use = gameplayActionMap.FindAction("Use");
		use.performed += context => OnUsePreformed(context, 0);
		use.canceled += context => OnUseCanceled(context, 0);
		use.Enable();
		sprint = gameplayActionMap.FindAction("Sprint");
		sprint.performed += context => OnSprint(context, true);
		sprint.canceled += context => OnSprint(context, false);
		sprint.Enable();
		//primary = gameplayActionMap.FindAction("Primary");
		//primary.performed += context => OnUsePreformed(context, 1);
		//primary.canceled += context => OnUseCanceled(context, 1);
		//primary.Enable();
		secondary = gameplayActionMap.FindAction("Secondary");
		secondary.performed += context => OnUsePreformed(context, 2);
		secondary.canceled += context => OnUseCanceled(context, 2);
		secondary.Enable();
		looking = gameplayActionMap.FindAction("Looking");
		looking.performed += OnMouseMovement;
		looking.Enable();
		inventoryIn = gameplayActionMap.FindAction("Inventory");
		inventoryIn.performed += OnInventoryPreformed;
		inventoryIn.Enable();
		//slots
		slotIn = gameplayActionMap.FindAction("Slot");
		slotIn.performed += OnSlotPreformed;
		slotIn.Enable();
		hudInteractBar.SetActive(false);

		GameEventsScript.GameEnd += End;
	}
	void End() {
		movement.performed -= OnMovementChanged;
		movement.canceled -= OnMovementChanged;
		jumping.performed -= OnJumpChanged;
		use.performed -= context => OnUsePreformed(context, 0);
		use.canceled -= context => OnUseCanceled(context, 0);
		sprint.performed -= context => OnSprint(context, true);
		sprint.canceled -= context => OnSprint(context, false);
		sprint.Enable();
		//primary.performed -= context => OnUsePreformed(context, 1);
		//primary.canceled -= context => OnUseCanceled(context, 1);
		secondary.performed -= context => OnUsePreformed(context, 2);
		secondary.canceled -= context => OnUseCanceled(context, 2);
		looking.performed -= OnMouseMovement;
		inventoryIn.performed -= OnInventoryPreformed;
		//slots
		slotIn.performed -= OnSlotPreformed;
	}
	void Start() {
		inventoryTab.SetActive(false);
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}
	void OnSlotPreformed(InputAction.CallbackContext context) {
		var value = (int)context.ReadValue<float>();
		if (inventory.container.ItemList().Count <= value || inventory.container.ItemList()[value] == null)
			return;
		if (item != null) {
			Destroy(item.gameObject);
		}
		item = Instantiate(inventory.container.ItemList()[value].item.prefab, new Vector3(0, 0, 0), Quaternion.identity);
		item.transform.parent = hand.transform;
		item.transform.localPosition = new Vector3(0, 0, 0);
		item.transform.localRotation = new Quaternion(0, 0, 0, 0);
		item.transform.localScale = new Vector3(1, 1, 1);
		var itemInteraction = item.GetComponent<InteractableScript>();
		itemInteraction.properties = properties;
		foreach (GameAction a in itemInteraction.actions.actionList) {
			properties.actions.actionList.Add(a);
		}
	}
	void OnMovementChanged(InputAction.CallbackContext context) {
		entity.Move(context.ReadValue<Vector2>());
	}
	void OnJumpChanged(InputAction.CallbackContext context) {
		entity.Jump();
	}
	void OnUsePreformed(InputAction.CallbackContext context, int value) {
		Hurt();
		controller.enabled = false;
		this.transform.position = GenerationProp.TileCoordinatesToRealCoordinates(GenerationProp.FindAccessibleTile(GenerationProp.playerTileCoordinates));
		controller.enabled = true;

		GameAction action = null;
		foreach (GameAction i in properties.actions.actionList) {
			if (i.type == (ActionType)value) {
				if (action == null || action.priority < i.priority) {
					action = i;
				}
			}
		}
		if (action != null)
			action.action.Invoke(true);
	}
	void OnUseCanceled(InputAction.CallbackContext context, int value) {
		GameAction action = null;
		foreach (GameAction i in properties.actions.actionList) {
			if (i.type == (ActionType)value) {
				if (action == null || action.priority < i.priority) {
					action = i;
				}
			}
		}
		if (action != null)
			action.action.Invoke(false);
	}
	void OnSprint(InputAction.CallbackContext context, bool isActive) {
		entity.sprinting = isActive;
	}

	void OnMouseMovement(InputAction.CallbackContext context) {
		entity.Look(context.ReadValue<Vector2>());
	}
	void OnInventoryPreformed(InputAction.CallbackContext context) {
		//open inventory or close
		inventoryOpen = !inventoryOpen;
		inventoryTab.SetActive(inventoryOpen);
		Cursor.visible = inventoryOpen;
		if (inventoryOpen) {
			looking.Disable();
			use.Disable();
			primary.Disable();
			selecting.Enable();
			Cursor.lockState = CursorLockMode.Confined;

		} else {
			looking.Enable();
			use.Enable();
			primary.Enable();
			secondary.Enable();
			selecting.Disable();
			Cursor.lockState = CursorLockMode.Locked;
		}
	}
	void Update() {
		SelectTick();
		if (hurt) {
			HurtAwaite();
		}
	}
	private void SelectTick() {
		var ray = fpsCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit)) {
			var selection = hit.transform;
			var selectedObject = selection.GetComponent<InteractableScript>();
			if (_selectedObject != selectedObject && selectedObject != null) {
				if (_selectedObject != null) {
					foreach (GameAction a in _selectedObject.actions.actionList) {
						///set to false only already Invoked actions
						a.action.Invoke(false);
						properties.actions.actionList.Remove(a);
					}
					_selectedObject = null;
				}

				selectedObject.properties = properties;
				foreach (GameAction a in selectedObject.actions.actionList) {
					properties.actions.actionList.Add(a);
				}
				_selectedObject = selectedObject;
			}
			else if (selectedObject != _selectedObject && _selectedObject != null) {
				foreach (GameAction a in _selectedObject.actions.actionList) {
					///set to false only already Invoked actions
					a.action.Invoke(false);
					properties.actions.actionList.Remove(a);
				}
				_selectedObject = null;
			}
		}
		else if (_selectedObject != null) {
			foreach (GameAction a in _selectedObject.actions.actionList) {
				///set to false only already Invoked actions
				a.action.Invoke(false);
				properties.actions.actionList.Remove(a);
			}
			_selectedObject = null;
		}
	}
	private void OnApplicationQuit() {
		inventory.container.ItemListClear();
	}
	private bool hurt;
	public void Hurt() {
		hud.blink.Close();
		hurt = true;
	}
	private void HurtAwaite() {
		if (!hud.blink.blinking) {
			HurtEnd();
		}
	}
	private void HurtEnd() {
		GameEventsScript.MainMenu();
	}
}
