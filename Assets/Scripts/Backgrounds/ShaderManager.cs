using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShaderManager : MonoBehaviour
{
    public GameObject img;
    public Material material;
    private RenderTexture _rTexture;

    void Awake() 
    {
        _rTexture = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
        _rTexture.enableRandomWrite = true;
        _rTexture.name = "Background Texture";
        _rTexture.Create();

        img.GetComponent<RawImage>().texture = _rTexture;
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        Graphics.Blit(_rTexture, material);
    }
}
