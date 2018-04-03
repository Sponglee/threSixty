using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attractor : MonoBehaviour {
    
    public Rigidbody2D rb;

    public static List<Attractor> Attractors;

    void OnEnable()
    {
        if (Attractors == null)
            Attractors = new List<Attractor>();

        Attractors.Add(this);
    }

    private void OnDisable()
    {
        Attractors.Remove(this);
    }

    private void FixedUpdate()
    {

        foreach(Attractor attractor in Attractors)
        {
            if (attractor!=this)
                Attract(attractor);
        }

    }
    public void Attract(Attractor objectToAttract)
    {
        Rigidbody2D rbToAttract = objectToAttract.rb;

        Vector3 direction = rb.position - rbToAttract.position;
        float distance = direction.magnitude;

        float forceMagnitude = (rb.mass * rbToAttract.mass) / Mathf.Pow(distance, 2);

        //force to a direction 
        Vector3 force = direction.normalized * forceMagnitude;


        rbToAttract.AddForce(force);

        
    }
}
