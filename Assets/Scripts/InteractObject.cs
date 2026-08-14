using Mono.Cecil;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractObject : MonoBehaviour
{
    public enum InteractObjectType
    {
        Door,
        Switch,
        PickableItem,
        AmmoRefill
    }

    [SerializeField] public InteractObjectType objectType;
    [SerializeField] private string screenText;
    [SerializeField] private Transform player;

    [Header("Door")]
    [SerializeField] private Animator objectAnimator;
    [SerializeField] private bool flippedDoor;
    [SerializeField] private bool lockedDoor;
    [SerializeField] private bool XComparison; //choose x or z based on arrows in editor on open/close object to compare player position to 
    [SerializeField] private bool ZComparison;
    [SerializeField] private AudioSource lockedSFX;
    [SerializeField] private AudioSource unlockedSFX;
    [SerializeField] private ItemData keyItem;

    [Header("Light Switch")]
    [SerializeField] private GameObject lightObject;
    [SerializeField] private Material emissiveMaterial;

    [Header("Pickable Item")]
    [SerializeField] private ItemData itemResource;
    private InventoryManager inventoryManager;

    [Header("Ammo Refill")]
    [SerializeField] private Revolver gun;
    [SerializeField] private GameObject ammoPlant;
    [SerializeField] private int ammoGiven;
    [SerializeField] private GameObject plantedSeed;
    [SerializeField] private ItemData seedItem;
    [SerializeField] private ItemData waterItem;
    private bool ammoTaken = false;
    private bool seedPlanted = false;

    private bool isOpen = false;
    private bool isOn = false;

    private bool interactedOnce = false;
    private String returnText = "";

    public void Awake()
    {
        inventoryManager = player.GetComponentInChildren<InventoryManager>();
    }
    public void Start()
    {
        if (objectType == InteractObjectType.Switch)
            emissiveMaterial.DisableKeyword("_EMISSION");

        if (flippedDoor)
            objectAnimator.SetBool("flipped", true);
    }
    public string GetInteractText()
    {
        if (objectType == InteractObjectType.AmmoRefill) //change on screen prompts depending on ammo plant state
        {
            if (!ammoTaken)
                return "Refill? (E)";

            if (inventoryManager.CheckIfItemInInventory(seedItem) && seedPlanted == false)
                return "Plant? (E)";

            if (inventoryManager.CheckIfItemInInventory(waterItem) && seedPlanted == true)
                return "Water? (E)";

            return "";

        }

        if (objectType == InteractObjectType.Door && lockedDoor)
            if (interactedOnce)
                return returnText;


        return screenText;
    }

    public void OpenClose()
    {
        if (lockedDoor)
        {
            if (inventoryManager.CheckIfItemInInventory(keyItem) && lockedDoor)
            {
                lockedDoor = false;
                returnText = "Interact (E)";
                unlockedSFX.Play();
                inventoryManager.RemoveItemInInventory(keyItem);
                return;
            }
            lockedSFX.Play();
            interactedOnce = true;
            returnText = "Interact(Locked) (E)";
            return;
        }    
        isOpen = !isOpen; //set is open to the opposite of what it previously was (always started closed -> false)

        Vector3 localPlayerPos = transform.InverseTransformPoint(player.position); //gets the players transform in local space
        if (!flippedDoor)
        {
            if (XComparison) //compare x axis
            {
                if ((localPlayerPos.x > 0 || objectAnimator.GetBool("openObject")) && objectAnimator.GetBool("openObjectInside") != true) //if the player is above 0 we are outside, if we triggered a bool play "close" in the animation state tree regardless of comparison
                {
                    objectAnimator.SetBool("openObject", isOpen);
                }
                else //else we are inside so play inside open/close animations
                {
                    objectAnimator.SetBool("openObjectInside", isOpen); //play close or open animation based on the bool
                }
            }
            else if (ZComparison) //compare z axis
            {
                if ((localPlayerPos.z > 0 || objectAnimator.GetBool("openObject")) && objectAnimator.GetBool("openObjectInside") != true)
                {
                    objectAnimator.SetBool("openObject", isOpen);
                }
                else
                {
                    objectAnimator.SetBool("openObjectInside", isOpen);

                }
            }
            else
            {
                objectAnimator.SetBool("openObject", isOpen);
            }
        }
        else if (flippedDoor)
        {
            print(localPlayerPos);
            if ((localPlayerPos.x > 0 || objectAnimator.GetBool("openObject")) && objectAnimator.GetBool("openObjectInside") != true) //if the player is above 0 we are outside, if we triggered a bool play "close" in the animation state tree regardless of comparison
            {
                objectAnimator.SetBool("openObject", isOpen);
            }
            else //else we are inside so play inside open/close animations
            {
                objectAnimator.SetBool("openObjectInside", isOpen); //play close or open animation based on the bool
            }
        }
        
    }

    public void TurnOnOff()
    {
        isOn = !isOn; //set is on to the opposite of what it previously was (always started off -> on(true))
        var light = lightObject.GetComponent<Light>();
        light.enabled = isOn;

        if (light.enabled)
            emissiveMaterial.EnableKeyword("_EMISSION");
        else
            emissiveMaterial.DisableKeyword("_EMISSION");
    }    

    public void AddItemToInventory()
    {
        inventoryManager.AddItem(itemResource);
        Destroy(gameObject);
    }

    public void AmmoRefill()
    {
        if (!ammoTaken) //destroy ammo plant and give ammo when pressed
        {
            gun.reserveAmmoCount += ammoGiven;
            ammoTaken = true;
            ammoPlant.SetActive(false);
            return;
        }

        if (ammoTaken)
        {
            if (inventoryManager.CheckIfItemInInventory(seedItem) && !seedPlanted) //if we have the item (seed) in inventory show the seed in game
            {
                plantedSeed.SetActive(true);
                seedPlanted = true;
                inventoryManager.RemoveItemInInventory(seedItem);
            }
            else if(inventoryManager.CheckIfItemInInventory(waterItem) && seedPlanted)
            {
                ammoPlant.SetActive(true);
                plantedSeed.SetActive(false);
                ammoTaken = false;
                seedPlanted = false;
                inventoryManager.RemoveItemInInventory(waterItem);
            }
            return;
        }
    }
}