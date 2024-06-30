using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeTime : MonoBehaviour
{
    [SerializeField] private float _lifeTime = 1f;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject,_lifeTime);
    }


}
