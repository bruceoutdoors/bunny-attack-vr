// Patrol.cs
using UnityEngine;
using System.Collections;
using DG.Tweening;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DuckMover : MonoBehaviour
{
    public float PauseDuration;
    public PlayerHealth playerHealth;
    public ExplosionGlow GlowThisBitches;
    public GameObject TorchEffect;
    public float KamikazeTime;
    public GameObject ExplosionBunBun;
    public float sinkSpeed = 2.5f;              // The speed at which the enemy sinks through the floor when
    public GvrAudioSource explosionSound;
    public GvrAudioSource bombTimerSound;
    public RendezvousPoint Rendezvous;

    UnityEngine.AI.NavMeshAgent agent;
    Animator anim;
    bool isResting = false;

    bool isTurning = false;
    Quaternion from;
    Quaternion toPlayer;
    Sequence seq;
    bool isSinking = false; // Whether the enemy has started sinking through the floor.

    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        anim = GetComponent<Animator>();
        seq = DOTween.Sequence();

        // Disabling auto-braking allows for continuous movement
        // between points (ie, the agent doesn't slow down as it
        // approaches a destination point).
        //agent.autoBraking = false;

        transform.position = Rendezvous.position;

        GotoNextPoint();
    }


    void GotoNextPoint()
    {
        isResting = false;

        if (Rendezvous.IsLastPoint())
        {
            // dead!
            enabled = false;
            return;
        }

        Rendezvous = Rendezvous.GetRendezvous();

        // Set the agent to go to the currently selected destination.
        agent.destination = Rendezvous.position;

        anim.SetBool("GoSomePlace", true);
    }


    void Update()
    {
        if (agent.enabled 
            && !agent.pathPending 
            && agent.remainingDistance < 0.5f 
            && !isResting)
        {
            // Choose the next destination point when the agent gets
            // close to the current one.
            StartCoroutine(pauseBeforeContinue());
        }
    }

    void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (Rendezvous != null)
        {
            Handles.Label(transform.position, string.Format("Target: {0}", Rendezvous.gameObject.name));
        }
#endif
    }

    IEnumerator pauseBeforeContinue()
    {
        agent.updateRotation = false;
        agent.enabled = false;
        from = transform.rotation;
        toPlayer = Quaternion.LookRotation(Camera.main.transform.position - transform.position);
        isTurning = true;
        
        if (!Rendezvous.IsLastPoint())
        {
            seq.Insert(0, transform.DOLocalRotateQuaternion(
                toPlayer * Quaternion.Euler(0, 0, Rendezvous.tiltLeft ? -30 : 30), 1f));
        }
        else
        {
            seq.Insert(0, transform.DOLocalRotateQuaternion(toPlayer, 1.0f));
            bombTimerSound.Play();
            GlowThisBitches.enabled = true;
            TorchEffect.SetActive(true);
            StartCoroutine(Kamikaze());
        }

        isResting = true;
        anim.SetBool("GoSomePlace", false); // chill a bit
        yield return new WaitForSeconds(PauseDuration);

        agent.enabled = true;
        isTurning = false;
        agent.updateRotation = true;
        GotoNextPoint();
    }

    IEnumerator Kamikaze()
    {
        yield return new WaitForSeconds(KamikazeTime);

        if (!isSinking)
        {
            GlowThisBitches.enabled = false;
            TorchEffect.SetActive(false);

            foreach (var c in GetComponents<Collider>())
            {
                c.enabled = false;
            }

            GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
            GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
            GetComponentInChildren<SpriteRenderer>().enabled = false;

            bombTimerSound.Stop();
            ExplosionBunBun.GetComponent<ParticleSystem>().Play();
            explosionSound.Play();
            playerHealth.AffectHealth(-1);
            Destroy(gameObject, 3f);
        }
    }

    public void StartSinking()
    {
        bombTimerSound.Stop();
        // Find and disable the Nav Mesh Agent.
        GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;

        // Find the rigidbody component and make it kinematic (since we use Translate to sink the enemy).
        GetComponent<Rigidbody>().isKinematic = true;

        // The enemy should no sink.
        isSinking = true;
        seq.Insert(0, transform.DOLocalRotate(new Vector3(0, transform.localRotation.y, 0), 1f))
            .Insert(0, transform.DOMoveY(-1.65f, 1f));


        // After 2 seconds destory the enemy.
        Destroy(gameObject, 1f);
    }
}