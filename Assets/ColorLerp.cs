using UnityEngine;
using System.Collections.Generic;

public class ColorLerp : MonoBehaviour
{
    // List of end colors
    public List<Color> endColors = new List<Color> { Color.blue, Color.green, Color.yellow };

    // Interpolation parameter
    public float t = 0f;

    // Index to keep track of the current end color
    private int currentIndex = 0;

    void Update()
    {
        // Get the current start and end colors
        Color startColor = GetComponent<Renderer>().material.color;
        Color endColor = endColors[currentIndex];
        t += Time.deltaTime / 10;
        // Perform color interpolation
        Color interpolatedColor = Color.Lerp(startColor, endColor, t);

        // Apply the interpolated color to the material
        GetComponent<Renderer>().material.color = interpolatedColor;

        // Increment the index to use the next end color in the next iteration
/*        currentIndex = (currentIndex + 1) % endColors.Count;*/
    }
}
