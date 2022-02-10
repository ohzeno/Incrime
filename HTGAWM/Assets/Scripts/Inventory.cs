using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    /// <summary> 사용하기 애매함
    ///     
    public static bool inventoryActivated = false;
    [SerializeField]
    private GameObject go_InventoryBase;
    /// </summary>
    /// 
    [SerializeField]
    private GameObject go_SlotsParent;

    private Slot[] slots;

    // Start is called before the first frame update
    void Start()
    {
        slots = go_SlotsParent.GetComponentsInChildren<Slot>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AcquireProof(Proof proof)
    {
        for(int i = 0; i < slots.Length; i++)
        {
            if(slots[i].proof.objectName.Length == 0)
            {
                Debug.Log("슬롯에 증거 추가");
                slots[i].AddProof(proof);
                return;
            }
        }
    }
}
