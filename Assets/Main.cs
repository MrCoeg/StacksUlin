using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.SocialPlatforms.Impl;

public class Main : MonoBehaviour {

    public GameObject CurrentCube;
    public GameObject LastCube;
    public Transform startpos;
/*    public TextMeshProUGUI Text;*/
    public int Level;
    public bool Done;

    public List<Color> endColors = new List<Color>();
    public int smoother;
    public float cameraSmoother;

    public int dayToNightSmoother;

    public int colorIndex;
    public float interpolation;
    public float height;
    public float time;

    public Vector3 cameraPos;
    public float deltaY;
    public float cameraViewAngle;
    public float smoothspeed;

    public Material mat;
    public float cubespeed;
    public float cubeSpeedSmoother;
    public float multiplayer;
    public AudioSource audioSource;
    public AudioSource notPerfect;

    public GameObject image;

    public float snapperThreshold;

    public TextMeshProUGUI text;
    public bool isNewBlock;
    public float gameTime;

    public int xMove, yMove;
    private void Awake()
    {
        StartGame();
    }
    public void StartGame()
    {
        cameraPos = Camera.main.transform.position;
        newBlock();
        mat.SetFloat("_Exposure", 0.5f);
    }


    private void newBlock()
    {
        if(LastCube != null)
        {
            CurrentCube.transform.position = new Vector3(Mathf.Round(CurrentCube.transform.position.x),
             CurrentCube.transform.position.y,
             Mathf.Round(CurrentCube.transform.position.z));

            var localx = Mathf.Abs(CurrentCube.transform.position.x - LastCube.transform.position.x);
            var localz = Mathf.Abs(CurrentCube.transform.position.z - LastCube.transform.position.z);
            var lastY = LastCube.transform.position.y;
            if (localx + localz <= snapperThreshold)
            {
                audioSource.Play();
                CurrentCube.transform.localScale = new Vector3(LastCube.transform.localScale.x,
                                               LastCube.transform.localScale.y,
                                               LastCube.transform.localScale.z);
                CurrentCube.transform.position = LastCube.transform.position;
                CurrentCube.transform.position = Vector3.Lerp(CurrentCube.transform.position, LastCube.transform.position, 0.5f) + Vector3.up * LastCube.transform.localScale.y;
            }
            else
            {
                notPerfect.Play();
                CurrentCube.transform.localScale = new Vector3(LastCube.transform.localScale.x - localx,
                                               LastCube.transform.localScale.y,
                                               LastCube.transform.localScale.z - localz);
                CurrentCube.transform.position = Vector3.Lerp(CurrentCube.transform.position, LastCube.transform.position, 0.5f) + Vector3.up * LastCube.transform.localScale.y / 2;

            }

            deltaY = CurrentCube.transform.position.y - lastY;
            if (CurrentCube.transform.localScale.x <= 0f ||
               CurrentCube.transform.localScale.z <= 0f)
            {
                CurrentCube.transform.localScale = Vector3.zero;
                Done = true;
/*                Text.gameObject.SetActive(true);
                Text.text = "Your Score " + Level;*/
                image.gameObject.SetActive(true);
                return;
            }
                
        }
        text.text = Level.ToString();
        LastCube = CurrentCube;
        CurrentCube = Instantiate(LastCube);
        CurrentCube.name = Level + "";
        interpolation = (float)Level / smoother % 1;
        CurrentCube.GetComponent<MeshRenderer>().material.SetColor("_Color",Color.Lerp(endColors[colorIndex], endColors[colorIndex+1], interpolation) );
        Level++;
        if (Level % smoother == 0)
        {

            colorIndex += 1;
            if (colorIndex >= endColors.Count)
            {
                colorIndex = 0;
            }
        }
        mat.SetFloat("_Exposure", mat.GetFloat("_Exposure") - height/dayToNightSmoother);
        Camera.main.backgroundColor = Color.Lerp(Color.white, Color.black, (float)Level / dayToNightSmoother);

        isNewBlock = true;

        /*        if (smoothspeed > 1)
                {
                    return;
                }*/
        /*        smoothspeed += Time.deltaTime;
                Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, CurrentCube.transform.position + cameraPos, smoothspeed);
                Camera.main.transform.LookAt(CurrentCube.transform.position + Vector3.down * cameraViewAngle);*/

        /*        Camera.main.transform.position = new Vector3(cameraPos.x, cameraPos.y + deltaY , cameraPos.z);
                cameraPos = Camera.main.transform.position;*/
        StartCoroutine(smoothCameraUp());
    }

    // Update is called once per frame
    void Update () {
        if (Done)
            return;
        if (isNewBlock)
        {
            gameTime = 0;
            isNewBlock = false;
        }
        gameTime += Time.deltaTime * cubespeed;
        time = Mathf.Abs( (gameTime % 2f)-1f);

        var center = LastCube.transform.position + Vector3.up * multiplayer;
        var pos1 = center + ((Level % 2 == 0) ? Vector3.left  * yMove : Vector3.back *xMove);
        var pos2 = center + ((Level % 2 == 0) ? Vector3.right * xMove : Vector3.forward *yMove);

        if (Level % 2 == 0)
            CurrentCube.transform.position = Vector3.Lerp(pos2, pos1, time);
        else
            CurrentCube.transform.position = Vector3.Lerp(pos1, pos2, time);

        if (Input.GetMouseButtonDown(0))
        {

            newBlock();
        }
	}

    IEnumerator smoothCameraUp()
    {
        var time = 0f;
        var pos = Camera.main.transform.position;
        var camTrans = Camera.main.transform;
        var lookedAtTransform = Camera.main.transform;
        lookedAtTransform.LookAt(CurrentCube.transform.position + Vector3.down * cameraViewAngle);
        lookedAtTransform.rotation = new Quaternion(Camera.main.transform.rotation.x, 
            lookedAtTransform.rotation.y,
            lookedAtTransform.rotation.z,
            lookedAtTransform.rotation.w);
        var NextPos = new Vector3(cameraPos.x, cameraPos.y + deltaY, cameraPos.z);
        while (time < 1)
        {
            time += Time.deltaTime / cameraSmoother;
            Camera.main.transform.position = Vector3.Lerp(pos, NextPos, time);
            Camera.main.transform.rotation = Quaternion.Lerp(camTrans.rotation, lookedAtTransform.rotation, time);
            yield return null;
        }
        cameraPos = Camera.main.transform.position;

    }

    public void addNew()
    {
        newBlock();
    }

    public void Restart()
    {
        StartCoroutine(AddScore());
    }
    public void Log()
    {
        StartCoroutine(LogFile());
    }

    public IEnumerator LogFile()
    {

        var texture = ScreenCapture.CaptureScreenshotAsTexture();
        var imageByte = texture.EncodeToPNG();

        using (UnityWebRequest www = UnityWebRequest.PostWwwForm("http://localhost:3000/LogStacks", "POST"))
        {

            // Attach the form to the request
            www.uploadHandler = new UploadHandlerRaw(imageByte);
            www.uploadHandler.contentType = "multipart/form-data";
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Berhasil");
            }
            else
            {
                Debug.Log(www.error);
            }
        }
    }
    IEnumerator AddScore()
    {
        using (UnityWebRequest www = UnityWebRequest.PostWwwForm("http://localhost:3000/stacks/" + Level, "POST"))
        {
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Success");
                yield return new WaitForSeconds(1f);
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

            }
            else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                Debug.Log(www.error);
            }
        }

    }
}

[System.Serializable]
public class LineConnectData
{
    public int Score;
}
