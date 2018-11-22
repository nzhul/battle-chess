using UnityEngine;

public static class Common
{
    public static void Empty(Transform parent)
    {
        foreach (Transform child in parent)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}