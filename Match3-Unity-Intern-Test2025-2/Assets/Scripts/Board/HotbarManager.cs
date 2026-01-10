using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotbarManager : MonoBehaviour
{
    [SerializeField] private Transform[] slots;

    private List<Item> storedItems = new List<Item>();
    public bool HasSpace => storedItems.Count < slots.Length;


    public void AddItem(Item item)
    {
        if (!HasSpace) return;

        int insertIndex = GetInsertIndex(item);

        ShiftUIItemsRight(insertIndex);

        storedItems.Insert(insertIndex, item);

        RectTransform slot = slots[insertIndex] as RectTransform;
        Vector3 screenPos = slot.position;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        worldPos.z = item.View.position.z;

        item.View.DOMove(worldPos, 0.3f)
            .OnComplete(() =>
            {
                item.ConvertToUI(slot);
            });
    }

    private int GetInsertIndex(Item newItem)
    {
        int insertIndex = storedItems.Count;

        for (int i = 0; i < storedItems.Count; i++)
        {
            if (storedItems[i].IsSameType(newItem))
            {
                insertIndex = i + 1;
            }
        }

        return insertIndex;
    }

    private void ShiftUIItemsRight(int fromIndex)
    {
        for (int i = storedItems.Count - 1; i >= fromIndex; i--)
        {
            RectTransform rt = storedItems[i].GetUIRect();
            rt.DOMove(slots[i + 1].position, 0.2f);
        }
    }


}
