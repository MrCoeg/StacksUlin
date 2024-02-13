using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class LevelData : MonoBehaviour
{
    public string url;
    public Configuration config;
    public Main main;


    IEnumerator getJSONData()
    {
        using (UnityWebRequest www = UnityWebRequest.Get("http://localhost:3000/stacks/configuration"))
        {
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success)
            {
                var text = www.downloadHandler.text;
                config =  JsonUtility.FromJson<Configuration>(text);
                main.dayToNightSmoother = config.backgroundDayToNightSmoother;
                main.cubespeed = config.cubeSpeedMultiplier;
                main.snapperThreshold = config.cubeSnappingRange;
                main.smoother = config.cubeColorInterpolationChangeSpeed;
                main.cameraSmoother = config.cameraInterpolationSpeed;
                main.StartGame();
            }
            else
            {
                Debug.Log(www.error);
            }
        }
    }

}

public struct Configuration {

    public float cubeSnappingRange;
    public float cubeSpeedMultiplier;
    public int cubeColorInterpolationChangeSpeed;
    public float cameraInterpolationSpeed;
    public int backgroundDayToNightSmoother;
}

