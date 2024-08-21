using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider))]
public class EndOfLevel : MonoBehaviour
{
    public float timeToTriggerLevelEnd = 2.0f;
    public string endSceneName = "3-4_Pain";

    private IEnumerator OnTriggeredPlayer(Collider other)
    {
        yield return FadeOutAudio();

        PlayerMoveController playerMove = other.GetComponent<PlayerMoveController>();
        playerMove.enabled = false;

        yield return null;

        float timeWaited = 0.0f;
        FreeMovementMotor playerMotor = other.GetComponent<FreeMovementMotor>();
        while (playerMotor.walkingSpeed > 0.0f)
        {
            playerMotor.walkingSpeed -= Time.deltaTime * 6.0f;
            playerMotor.walkingSpeed = Mathf.Max(playerMotor.walkingSpeed, 0.0f);
            timeWaited += Time.deltaTime;
            yield return null;
        }
        playerMotor.walkingSpeed = 0.0f;

        yield return new WaitForSeconds(Mathf.Clamp(timeToTriggerLevelEnd - timeWaited, 0.0f, timeToTriggerLevelEnd));
        Camera.main.SendMessage("WhiteOut");

        yield return new WaitForSeconds(2.0f);

        SceneManager.LoadScene(endSceneName);
    }

    private IEnumerator FadeOutAudio()
    {
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();

        foreach (AudioSource audioSource in audioSources)
        {
            float initialVolume = audioSource.volume;
            float targetVolume = 0.0f;
            float elapsedTime = 0.0f;
            float fadeDuration = 3.0f;

            while (elapsedTime < fadeDuration)
            {
                float t = elapsedTime / fadeDuration;
                audioSource.volume = Mathf.Lerp(initialVolume, targetVolume, t);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            audioSource.volume = targetVolume; // Ensure the volume is set to the target value at the end
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(OnTriggeredPlayer(other));
        }
    }
}
