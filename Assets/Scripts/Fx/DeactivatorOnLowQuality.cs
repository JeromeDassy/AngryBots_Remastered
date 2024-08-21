using UnityEngine;

public class DeactivatorOnLowQuality : MonoBehaviour
{
    public QualityManager.Quality qualityThreshold = QualityManager.Quality.High;

    private void Start()
    {
        if (QualityManager.quality < qualityThreshold)
        {
            gameObject.SetActive(false);
        }
        enabled = false;
    }
}
