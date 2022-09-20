using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class rank : MonoBehaviour
{
   
    public GameObject rank_object = null;
    public int rank_num = 0;
    // Start is called before the first frame update
    void Start()
    {
        
        int total = 10;
        if (total > 0)
        {
            Debug.Log("C");
        }
        else if (total >= 10 && total < 20)
        {
            Debug.Log("B");
        }
        else if (total >= 20 && total < 30) 
        {
            Debug.Log("A");
        }
        else
        {
            Debug.Log("S");
        }
  
    }

    // Update is called once per frame
    void Update()
    {
        

    }
}
