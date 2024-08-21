using UnityEngine;

public class AI : MonoBehaviour
{
    public MonoBehaviour behaviourOnSpotted;
    public AudioClip soundOnSpotted;
    public MonoBehaviour behaviourOnLostTrack;

    private Transform character;
    private Transform player;
    private bool insideInterestArea = true;

    private void Awake()
    {
        character = transform;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void OnEnable()
    {
        behaviourOnLostTrack.enabled = true;
        behaviourOnSpotted.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == player && CanSeePlayer())
        {
            OnSpotted();
            Debug.Log("Spider Enemy spotted player");
        }
    }

    public void OnEnterInterestArea()
    {
        insideInterestArea = true;
    }

    public void OnExitInterestArea()
    {
        insideInterestArea = false;
        OnLostTrack();
    }

    public void OnSpotted()
    {
        if (!insideInterestArea)
            return;

        if (!behaviourOnSpotted.enabled)
        {
            behaviourOnSpotted.enabled = true;
            behaviourOnLostTrack.enabled = false;

            if (GetComponent<AudioSource>() && soundOnSpotted)
            {
                GetComponent<AudioSource>().clip = soundOnSpotted;
                GetComponent<AudioSource>().Play();
            }
        }
    }

    public void OnLostTrack()
    {
        if (!behaviourOnLostTrack.enabled)
        {
            behaviourOnLostTrack.enabled = true;
            behaviourOnSpotted.enabled = false;
        }
    }

    public bool CanSeePlayer()
    {
        Vector3 playerDirection = player.position - character.position;
        RaycastHit hit;
        Physics.Raycast(character.position, playerDirection, out hit, playerDirection.magnitude);
        if (hit.collider && hit.collider.transform == player)
        {
            return true;
        }
        return false;
    }
}
