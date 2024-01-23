using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observer : MonoBehaviour
{
    [SerializeField]
    private MapCreator mapCreator;

    // Start is called before the first frame update
    void Start()
    {
        mapCreator.Create();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
