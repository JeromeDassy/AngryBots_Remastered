using UnityEngine;

public enum FootType
{
    Player,
    Mech,
    Spider
}

public class FootstepHandler : MonoBehaviour
{
    public AudioSource audioSource;
    public FootType footType;

    private PhysicMaterial physicMaterial;

    private void OnCollisionEnter(Collision collision)
    {
        physicMaterial = collision.collider.sharedMaterial;
    }

    public void OnFootstep()
    {
        if (!audioSource.enabled)
        {
            return;
        }

        AudioClip sound;
        switch (footType)
        {
            case FootType.Player:
                sound = MaterialImpactManager.GetPlayerFootstepSound(physicMaterial);
                break;
            case FootType.Mech:
                sound = MaterialImpactManager.GetMechFootstepSound(physicMaterial);
                break;
            case FootType.Spider:
                sound = MaterialImpactManager.GetSpiderFootstepSound(physicMaterial);
                break;
            default:
                sound = null;
                break;
        }
        audioSource.pitch = Random.Range(0.98f, 1.02f);
        audioSource.PlayOneShot(sound, Random.Range(0.8f, 1.2f));
    }
}
