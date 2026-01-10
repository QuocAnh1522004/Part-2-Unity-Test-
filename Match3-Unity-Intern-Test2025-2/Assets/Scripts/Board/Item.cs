using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

[Serializable]
public class Item
{
    public Cell Cell { get; private set; }

    public Transform View { get; private set; }

    private Image uiImage;
    public RectTransform UIRect { get; private set; }
    public RectTransform GetUIRect()
    {
        return uiImage != null ? uiImage.rectTransform : null;
    }
    public void ConvertToUI(RectTransform slot)
    {
        if (View == null) return;
        SpriteRenderer sr = View.GetComponent<SpriteRenderer>();
        if (sr == null) return;
        GameObject go = new GameObject("UI_Item");
        uiImage = go.AddComponent<Image>();
        uiImage.sprite = sr.sprite;
        uiImage.preserveAspect = true;
        RectTransform rt = uiImage.rectTransform;
        rt.SetParent(slot);
        rt.localPosition = Vector3.zero;
        rt.localRotation = Quaternion.identity;
        rt.localScale = new Vector3(0.5f,0.5f,0.5f);
        GameObject.Destroy(View.gameObject);
        View = null;
    }
    public virtual void SetView()
    {
        string prefabname = GetPrefabName();

        if (!string.IsNullOrEmpty(prefabname))
        {
            GameObject prefab = Resources.Load<GameObject>(prefabname);
            if (prefab)
            {
                View = GameObject.Instantiate(prefab).transform;
            }
        }
    }
  
    protected virtual string GetPrefabName() { return string.Empty; }

    public virtual void SetCell(Cell cell)
    {
        Cell = cell;
    }

    internal void AnimationMoveToPosition()
    {
        if (View == null) return;

        View.DOMove(Cell.transform.position, 0.2f);
    }

    public void SetViewPosition(Vector3 pos)
    {
        if (View)
        {
            View.position = pos;
        }
    }

    public void SetViewRoot(Transform root)
    {
        if (View)
        {
            View.SetParent(root);
        }
    }

    public void SetSortingLayerHigher()
    {
        if (View == null) return;

        SpriteRenderer sp = View.GetComponent<SpriteRenderer>();
        if (sp)
        {
            sp.sortingOrder = 1;
        }
    }


    public void SetSortingLayerLower()
    {
        if (View == null) return;

        SpriteRenderer sp = View.GetComponent<SpriteRenderer>();
        if (sp)
        {
            sp.sortingOrder = 0;
        }

    }

    internal void ShowAppearAnimation()
    {
        if (View == null) return;

        Vector3 scale = View.localScale;
        View.localScale = Vector3.one * 0.1f;
        View.DOScale(scale, 0.1f);
    }

    internal virtual bool IsSameType(Item other)
    {
        return false;
    }

    public bool IsSameItem(Item other)
    {
        return other != null && IsSameType(other);
    }

    internal virtual void ExplodeView()
    {
        if (View)
        {
            View.DOScale(0.1f, 0.1f).OnComplete(
                () =>
                {
                    GameObject.Destroy(View.gameObject);
                    View = null;
                }
                );
        }
    }



    internal void AnimateForHint()
    {
        if (View)
        {
            View.DOPunchScale(View.localScale * 0.1f, 0.1f).SetLoops(-1);
        }
    }

    internal void StopAnimateForHint()
    {
        if (View)
        {
            View.DOKill();
        }
    }

    internal void Clear()
    {
        Cell = null;

        if (View)
        {
            GameObject.Destroy(View.gameObject);
            View = null;
        }
    }


}
