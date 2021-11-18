using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helper
{
    public static T[] FindComponentsInChildrenWithTag<T>(this GameObject parent, string tag, bool forceActive = false) where T : Component
    {
        if (parent == null) {
            Debug.Log("parent vasio");
            throw new System.ArgumentNullException(); }
        if (string.IsNullOrEmpty(tag) == true) {
            Debug.Log("tag vasia");
            throw new System.ArgumentNullException(); }
        List<T> list = new List<T>(parent.GetComponentsInChildren<T>(forceActive));
        if (list.Count == 0) {
            Debug.Log("lista vasia");
            return null; }
       // Debug.Log("lista de entidades"+ list.Count );
        for (int i = list.Count - 1; i >= 0; i--)
        {
            if (list[i].CompareTag(tag) == false)
            {
                list.RemoveAt(i);
            }
        }
        return list.ToArray();
    }

    public static T FindComponentInChildWithTag<T>(this GameObject parent, string tag, bool forceActive = false) where T : Component
    {
        if (parent == null) { throw new System.ArgumentNullException(); }
        if (string.IsNullOrEmpty(tag) == true) { throw new System.ArgumentNullException(); }

        T[] list = parent.GetComponentsInChildren<T>(forceActive);
        int i = 0;
        foreach (T t in list)
        {
            if (t.CompareTag(tag) == true)
            {
                return list[i];
            }
            i++;
        }
        return null;
    }
}
