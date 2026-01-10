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

        Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(null, slot.position);
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        worldPos.z = item.View.position.z;

        item.View.DOMove(worldPos, 0.3f)
            .OnComplete(() =>
            {
                item.ConvertToUI(slot);
                CheckAndExplodeMatches();
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
            RectTransform ui = storedItems[i].GetUIRect();
            RectTransform targetSlot = slots[i + 1] as RectTransform;

            // Ensure same parent
            ui.SetParent(targetSlot.parent, true);

            // UI animation ONLY
            ui.DOAnchorPos(targetSlot.anchoredPosition, 0.25f);
        }
    }

    private void CheckAndExplodeMatches()
    {
        if (storedItems.Count < 3) return;

        int count = 1;

        for (int i = 1; i < storedItems.Count; i++)
        {
            if (storedItems[i].IsSameItem(storedItems[i - 1]))
            {
                count++;

                if (count >= 3)
                {
                    int startIndex = i - count + 1;
                    ExplodeHotbarItems(startIndex, count);
                    return;
                }
            }
            else
            {
                count = 1;
            }
        }
    }

    private void ExplodeHotbarItems(int startIndex, int length)
    {
        List<Item> toRemove = new List<Item>();

        for (int i = startIndex; i < startIndex + length; i++)
        {
            toRemove.Add(storedItems[i]);
        }

        Sequence seq = DOTween.Sequence();

        foreach (var item in toRemove)
        {
            RectTransform ui = item.GetUIRect();
            seq.Join(ui.DOScale(0f, 0.15f));
        }

        seq.OnComplete(() =>
        {
            // Destroy UI safely AFTER animation
            foreach (var item in toRemove)
            {
                Destroy(item.GetUIRect().gameObject);
                storedItems.Remove(item);
            }

            // Re-layout remaining items
            RelayoutAllItems();

            // Optional: chain reactions
            CheckAndExplodeMatches();
        });
    }

    private void RelayoutAllItems()
    {
        for (int i = 0; i < storedItems.Count; i++)
        {
            RectTransform ui = storedItems[i].GetUIRect();
            RectTransform slot = slots[i] as RectTransform;

            ui.SetParent(slot.parent, true);
            ui.DOAnchorPos(slot.anchoredPosition, 0.25f)
              .OnComplete(() =>
              {
                  ui.SetParent(slot, false);
                  //ui.anchoredPosition = Vector2.zero;
              });
        }
    }
}
