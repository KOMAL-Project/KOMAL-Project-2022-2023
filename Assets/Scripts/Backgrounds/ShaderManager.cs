using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderManager : MonoBehaviour
{
    public ComputeShader bgShader;
    public RenderTexture _rTexture;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (_rTexture == null) 
        {
            //_rTexture = new RenderTexture();
        }
    }
}
