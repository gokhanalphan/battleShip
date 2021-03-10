using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    [HideInInspector]
    public Vector3 whichWay;
    public float ballSpeed;

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(whichWay * ballSpeed * Time.deltaTime, Space.World);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //Debug.Log("CANNON TOUCH");
            Destroy(gameObject);
        }
        else if (other.gameObject.tag == "Obstacles")
        {
            Destroy(gameObject);
        }
        else
            Destroy(gameObject, 10f);
    }
}
