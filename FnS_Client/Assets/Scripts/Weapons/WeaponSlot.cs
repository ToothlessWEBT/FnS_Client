using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlot// : MonoBehaviour
{
    private Transform slotTran;

    private Transform weapon;

    public void SetSlotTrans(Transform slotT)
    {
        slotTran = slotT;
    }

    public void Equipt(Transform weaponNew)//theheheheheheheheheh2
    {
        weapon = weaponNew;

        weapon.transform.SetParent(slotTran);

        weapon.transform.localPosition = Vector3.zero;

        weapon.transform.localRotation = Quaternion.identity;
    }

    public void Drop()
    {
        weapon.transform.SetParent(null);
    }

    public void Flip()
    {
        slotTran.RotateAround(slotTran.position, slotTran.right, 180f);
    }
}
