using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour
{
    [SerializeField] List<Tentacle> tentacles = new List<Tentacle>();
    
    private void SetParentStructure(Tentacle newParent)
    {
        List<Tentacle> children = tentacles;
        children.Remove(newParent);
        foreach(Tentacle child in children)
        {
            child.transform.SetParent(transform);
        }
        transform.SetParent(newParent.transform);
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
