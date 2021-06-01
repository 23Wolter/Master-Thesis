using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    [SerializeField] ParticleSystem portalParticles1; 
    [SerializeField] ParticleSystem portalParticles2;
    [SerializeField] Collider portalCollider; 
    [SerializeField] Transform portalDestination; 
    
    public void Activate()
    {
        portalParticles1.Play();
        portalParticles2.Play();
        portalCollider.enabled = true;
    }

    public Transform GetDestination()
    {
        return portalDestination; 
    }
}
