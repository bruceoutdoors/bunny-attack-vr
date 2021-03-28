using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;

public class PlayerShooting : MonoBehaviour
{
    public int damagePerShot = 20;                  // The damage inflicted by each bullet.
    public float timeBetweenBullets = 0.15f;        // The time between each shot.
    public float range = 100f;                      // The distance the gun can fire.
    public GvrAudioSource gunAudio;                           // Reference to the audio source.
    public GvrAudioSource dryFireAudio;
    public Transform cameraTransform;
    public CurrentWord currentWord;
    public GvrReticlePointer pointer;
    public ScrabbleMan scrabbleMan;
    public HUDController hudcontroller;
    public Color scrabbleRackFullNotificationColor;

    float timer;                                    // A timer to determine when to fire.
    RaycastHit shootHit;                            // A raycast hit to get information about what was hit.
    int shootableMask;                              // A layer mask so the raycast only hits things on the shootable layer.
    ParticleSystem gunParticles;                    // Reference to the particle system.
    LineRenderer gunLine;                           // Reference to the line renderer.
    
    Light gunLight;                                 // Reference to the light component.
    public Light faceLight;                             // Duh
    float effectsDisplayTime = 0.2f;                // The proportion of the timeBetweenBullets that the effects will display for.


    void Awake()
    {
        // Create a layer mask for the Shootable layer.
        shootableMask = LayerMask.GetMask("Shootable");

        // Set up the references.
        gunParticles = GetComponent<ParticleSystem>();
        gunLine = GetComponent<LineRenderer>();
        gunLight = GetComponent<Light>();
        //faceLight = GetComponentInChildren<Light> ();
    }


    void Update()
    {
        // Add the time since Update was last called to the timer.
        timer += Time.deltaTime;

        // If the Fire1 button is being press and it's time to fire...
        if (Input.GetButton("Fire1") 
            && timer >= timeBetweenBullets 
            && Time.timeScale != 0 
            && !pointer.IsPointerHovering)
        {
            if (scrabbleMan.currentWord.Word.Length >= scrabbleMan.TileCountPerGame)
            {
                timer = 0.2f;
                dryFireAudio.Play();
                hudcontroller.FlashMessage("SCRABBLE RACK FULL!", scrabbleRackFullNotificationColor, 0.8f);
            }
            else
            {
                // ... shoot the gun.
                Shoot();
            }
            
        }

        // If the timer has exceeded the proportion of timeBetweenBullets that the effects should be displayed for...
        if (timer >= timeBetweenBullets * effectsDisplayTime)
        {
            // ... disable the effects.
            DisableEffects();
        }
    }


    public void DisableEffects()
    {
        // Disable the line renderer and the light.
        gunLine.enabled = false;
        faceLight.enabled = false;
        gunLight.enabled = false;
    }


    void Shoot()
    {
        // Reset the timer.
        timer = 0f;

        // Play the gun shot audioclip.
        gunAudio.Play();

        // Enable the lights.
        gunLight.enabled = true;
        faceLight.enabled = true;

        // Stop the particles from playing if they were, then start the particles.
        gunParticles.Stop();
        gunParticles.Play();

        // Enable the line renderer and set it's first position to be the end of the gun.
        gunLine.enabled = true;
        gunLine.SetPosition(0, transform.position);

        // Set the shootRay so that it starts at the end of the gun and points forward from the barrel.
        Ray firingRay = new Ray(cameraTransform.position, cameraTransform.forward);

        // Perform the raycast against gameobjects on the shootable layer and if it hits something...
        if (Physics.Raycast(firingRay, out shootHit, range, shootableMask))
        {
            if (shootHit.collider.tag == "ScrabbleTile")
            {
                GameObject go = shootHit.collider.gameObject;
                ScrabbleTileController str = go.GetComponent<ScrabbleTileController>();
                StartCoroutine(addLetter(str.Letter));
                
                str.BloodRelease(shootHit);
                str.Collect();

                foreach (Transform t in go.transform)
                {
                    if (t.name == "TileLetter")
                    {
                        t.parent = null;
                        t.gameObject.GetComponent<ScaleFromCamera>().enabled = false;
                        t.rotation = Quaternion.LookRotation(Camera.main.transform.position - transform.position);
                        Sequence jumpToFort = DOTween.Sequence();
                        jumpToFort.Insert(0, t.DOJump(new Vector3(0, 0.87f, 1.265f), 1.5f, 1, 0.5f))
                            .Insert(0, t.DOScale(0.1f, 0.5f))
                            .Insert(0, t.DORotate(new Vector3(0, 0, 0), 0.5f));
                        Destroy(t.gameObject, 0.5f);
                        break;
                    }
 }
            }
            // Set the second position of the line renderer to the point the raycast hit.
            gunLine.SetPosition(1, shootHit.point);
        }
        // If the raycast didn't hit anything on the shootable layer...
        else
        {
            // ... set the second position of the line renderer to the fullest extent of the gun's range.
            gunLine.SetPosition(1, firingRay.origin + firingRay.direction * range);
        }
    }

    IEnumerator addLetter(char c)
    {
        yield return new WaitForSeconds(0.5f);
        currentWord.AppendLetterToCurrentWord(c);
    }
}