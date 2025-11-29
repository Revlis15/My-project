using UnityEngine;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public int gold;
    public TMP_Text goldText;
    public InventorySlot[] itemSlots;
    public UseItem useItem;
    public GameObject lootPrefab;
    public Transform player;

    private void Start()
    {
        foreach (var slot in itemSlots)
        {
            slot.UpdateUI();
        }
    }

    private void OnEnable()
    {
        Loot.OnItemLooted += AddItem;
    }

    private void OnDisable()
    {
        Loot.OnItemLooted -= AddItem;
    }

    public void AddItem(ItemSO itemSO, int quantity) 
    {
        if (itemSO.isGold)
        {
            gold += quantity;
            goldText.text = gold.ToString();
            return;
        }

        foreach (var slot in itemSlots) //It it is the SAME item AND there is ROOM left
        {
            if (slot == null) continue;

            if (slot.itemSO == itemSO && slot.quantity < itemSO.stackSize)
            {
                int availableSpace = itemSO.stackSize - slot.quantity;
                int amountToAdd = Mathf.Min(availableSpace, quantity);

                slot.quantity += amountToAdd;
                quantity -= amountToAdd;

                slot.UpdateUI();
                if (quantity <= 0)
                    return;
            }
        }
       
        foreach (var slot in itemSlots)
        {
            if (slot == null) continue;

            if (slot.itemSO == null)
            {
                int amountToAdd = Mathf.Min(itemSO.stackSize, quantity);
                slot.itemSO = itemSO;
                slot.quantity = amountToAdd; // The original code has a logic error here (explained below)
                slot.UpdateUI();
                return;
            }
        }

        if (quantity > 0)
        {
            DropLoot(itemSO, quantity);
        }
    }
    public void DropItem(InventorySlot slot)
    {
        // 1. Drop the physical item into the world
        DropLoot(slot.itemSO, 1);

        // 2. Reduce the quantity in the inventory slot
        slot.quantity--;

        // 3. Check if the slot is now empty
        if (slot.quantity <= 0)
        {
            slot.itemSO = null;
        }

        // 4. Update the visual display
        slot.UpdateUI();
    }

    private void DropLoot(ItemSO itemSO, int quantity)
    {
        // 1. Instantiation and Setup
        Loot loot = Instantiate(lootPrefab, player.position, Quaternion.identity).GetComponent<Loot>();

        // 2. Data Initialization
        loot.Initialize(itemSO, quantity);
    }
    public void UseItem(InventorySlot slot)
    {
        if (slot.itemSO != null && slot.quantity >= 0)
        {
            useItem.ApplyItemEffects(slot.itemSO);

            slot.quantity--;
            if (slot.quantity <= 0)
            {
                slot.itemSO = null;
            }
            slot.UpdateUI();
        }
    }
}
