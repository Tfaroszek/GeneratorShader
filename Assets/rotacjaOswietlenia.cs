using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class rotacjaOswietlenia : MonoBehaviour
{
  
    public GameObject swiatlo;
    public float obrotSpeed = 10f; 
    private bool stop = true;
   public void rotacja(){
    if(stop){
        stop = false;
    }else{
        stop = true;
    }

   }
    void Update()
    {
        if(!stop)
        swiatlo.transform.Rotate(0, obrotSpeed * Time.deltaTime, 0);
    }
}
