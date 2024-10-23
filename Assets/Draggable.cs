using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    private Ray GetMouseWorldPos()
    {
        return Camera.main.ScreenPointToRay(Input.mousePosition);
    }

    private void OnMouseDrag()
    {
        if(Physics.Raycast(GetMouseWorldPos(), out RaycastHit raycastHit))
        {
            Debug.Log(raycastHit.point);
            transform.position = new Vector3(raycastHit.point.x, raycastHit.point.y, 0);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
