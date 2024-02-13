using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeAway : MonoBehaviour
{
    public float fadeInTime = 3f; // Time to fade in the image
    public float dissolveTime = 5f; // Time to dissolve the image
    public int nextSceneIndex = 1;

    private Image image;
    private float fadeInTimer;
    private bool dissolveStarted;

    void Start()
    {
        image = GetComponent<Image>();
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0f); // Start with a transparent image
        fadeInTimer = 0f;
        dissolveStarted = false;
    }

    void Update()
    {
        // Gradually fade in the image when the scene starts
        if (fadeInTimer < fadeInTime)
        {
            fadeInTimer += Time.deltaTime;
            float fadeProgress = Mathf.Clamp01(fadeInTimer / fadeInTime);
            Color imageColor = image.color;
            imageColor.a = fadeProgress;
            image.color = imageColor;
        }
        else if (!dissolveStarted)
        {
            // Check for user click to start the dissolve effect
            if (Input.GetMouseButtonDown(0))
            {
                dissolveStarted = true;
            }
        }

        // Start dissolving the image when user clicks
        if (dissolveStarted)
        {
            float dissolveProgress = Mathf.Clamp01(Time.time / dissolveTime);
            Color imageColor = image.color;
            imageColor.a = 1f - dissolveProgress;
            image.color = imageColor;

            // Check if the image is fully transparent
            if (dissolveProgress >= 1f)
            {
                // Load the next scene
                LoadNextScene();
            }
        }
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneIndex);
    }
}
