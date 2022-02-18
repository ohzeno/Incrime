using UnityEngine;
using System.Collections;

public class DestroyAfter : MonoBehaviour {
    
    public float destroyAfter;

    void Start() {
        Destroy(gameObject, destroyAfter);
    }
}
