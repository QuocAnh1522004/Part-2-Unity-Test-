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

    //public event EventHandler addedItem;
    //public class OnAddedItemEventArgs
    //{
    //    public Item Item;
    //}
    public void AddItem(Item item)
    {
        if (!HasSpace) return;

        storedItems.Add(item);
        int index = storedItems.Count - 1;

        RectTransform slot = slots[index] as RectTransform;

        Vector3 screenPos = slot.position;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

        worldPos.z = item.View.position.z;

        item.View.DOMove(worldPos, 0.3f)
        .OnComplete(() =>
        {
            item.ConvertToUI(slots[index] as RectTransform);
        });
    }

    


}
