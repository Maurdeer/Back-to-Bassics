using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExtractRenderTexture : MonoBehaviour
{
    [SerializeField] private Material outlineMaterial;
    [SerializeField] private Camera outlineCam;
    [SerializeField] private Vector2Int textureSize;

    private RenderTexture _tex;
    
    void Awake() 
    
    {
        RenderTexture outlineTexture = new RenderTexture(textureSize.x, textureSize.y, 32, RenderTextureFormat.ARGB64);
        Debug.Assert(outlineTexture.Create(), "No textures? *vine boom*");
        
        RenderTexture mainTexture = new RenderTexture(textureSize.x, textureSize.y, 32, RenderTextureFormat.ARGB64);
        Debug.Assert(outlineTexture.Create(), "No textures? *vine boom*");
        
        outlineCam.targetTexture = outlineTexture;
        Camera.main.targetTexture = mainTexture;
    
        outlineMaterial.SetTexture("_MainTex", outlineTexture);
        outlineMaterial.SetTexture("_ScreenTex", mainTexture);
    }
}
