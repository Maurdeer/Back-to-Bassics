using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFade : MonoBehaviour
{
    Light l;
    Transform t;
    private PlayerController player;
    public float fadeintensity;
    public float time;
    // Start is called before the first frame update
    void Start()
    {
        l = GetComponent<Light>();
        t= GetComponent<Transform>();
        player = FindObjectOfType<PlayerController>();
        StartCoroutine(fadeRoutine()); 
    }

    // Update is called once per frame
    void Update()
    {
        t.position = player.transform.position;
    }

    public IEnumerator fadeRoutine() {
        while (l.intensity > 0) {
            yield return new WaitForSeconds(time);
            l.intensity -= fadeintensity;
        }
        
    }
}
