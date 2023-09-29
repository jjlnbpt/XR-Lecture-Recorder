using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropTag : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<PropManager>().Register(this.gameObject);
    }

}
