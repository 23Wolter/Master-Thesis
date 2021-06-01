using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DummyHealth : MonoBehaviour
{
    [SerializeField] Slider healthbar;
    [SerializeField] AudioSource hello;
    [SerializeField] AudioSource hurt;
    [SerializeField] AudioSource dead;
    [SerializeField] ParticleSystem explosion; 
    [SerializeField] ParticleSystem explosion1; 
    [SerializeField] ParticleSystem explosion2; 
    [SerializeField] ParticleSystem explosion3; 
    [SerializeField] ParticleSystem explosion4; 
    [SerializeField] ParticleSystem explosion5; 

    private bool alive = true;



    private void Start()
    {
        StartCoroutine(SayHello()); 
    }


    public void TakeDamage(int damage)
    {
        if (!hurt.isPlaying) hurt.Play(); 

        healthbar.value -= damage; 

        if(healthbar.value <= 0)
        {
            DestroyDummy(); 
        }
    }

    private void DestroyDummy()
    {
        alive = false;
        if(!dead.isPlaying) dead.Play();
        explosion.Play();
        explosion1.Play();
        explosion2.Play();
        explosion3.Play();
        explosion4.Play();
        explosion5.Play();
        StartCoroutine(Kill()); 
    }


    private IEnumerator SayHello()
    {
        while(alive)
        {
            float rand = Random.Range(10, 30);
            yield return new WaitForSeconds(rand);
            hello.Play(); 
        }
    }


    private IEnumerator Kill()
    {
        yield return new WaitForSeconds(2);
        Destroy(this.gameObject);
    }
}
