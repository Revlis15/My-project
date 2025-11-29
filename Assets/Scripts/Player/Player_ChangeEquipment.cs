using UnityEngine;

public class Player_ChangeEquipment : MonoBehaviour
{

    public Player_combat combat;
    public Player_bow bow;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("ChangeEquipment"))
        {
            combat.enabled = !combat.enabled;
            bow.enabled = !bow.enabled;
        }
    }
}
