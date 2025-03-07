using UnityEngine;

public static class GameObjectUtils
{

    /// <summary>
    /// Disables "DontDestroyOnLoad" by parenting the object to another, destructable object.
    /// There is no other way at present to do so.
    /// </summary>
    public static void DisableDontDestroyOnLoad(GameObject targetGameObject)
    {
        Transform destructableParent = (new GameObject()).transform;
        destructableParent.name = "Temporary Destructable Parent";
        targetGameObject.transform.SetParent(destructableParent);

    }

}