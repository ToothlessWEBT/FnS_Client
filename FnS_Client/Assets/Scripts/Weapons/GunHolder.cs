using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunHolder : MonoBehaviour
{
    [SerializeField]
    private Transform[] allSlots;

    [SerializeField] private Transform gunHolder;

    private WeaponSlot[] weaponSlots = {new WeaponSlot(), new WeaponSlot(), new WeaponSlot(), new WeaponSlot()};

    private void Start()
    {
       for (var i = 0; i < weaponSlots.Length; i++)
       {
            weaponSlots[i].SetSlotTrans(allSlots[i]);
       }
    }

    private bool facingRight = true;

    private void Update()
    {
        float dot = Vector2.Dot(gunHolder.right, Vector2.right);

        //print(dot);

        if(dot > 0 && !facingRight)
        {
            FlipSlots();
            facingRight = true;
        }
        else if(dot < 0 && facingRight)
        {
            FlipSlots();
            facingRight = false;
        }
    }

    private void FlipSlots()
    {
        foreach (WeaponSlot slot in weaponSlots)
        {
            slot.Flip();
        }
    }

    public void SetActiveSlot(ushort I)
    {
        for(ushort i = 0; i<= weaponSlots.Length; i++)
        {
            allSlots[i].gameObject.SetActive(I == i);
        }
    }

    public void AddWeapon(ushort index, ushort weaponId)
    {
        weaponSlots[index].Equipt(WeaponManager.Singleton.GetWeapon(weaponId));
    }

    public void RemoveWeapon(ushort index)
    {
        weaponSlots[index].Drop();
    }
}
