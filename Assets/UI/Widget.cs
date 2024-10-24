using UnityEngine;

public class Widget : MonoBehaviour
{
    private GameObject owner;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void SetOwner(GameObject newOwner)
    {
        owner = newOwner;
    }
}
