using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomRenderer : MonoBehaviour
{
    public Camera renderCamera;

    // Use this for initialization
    [ContextMenu("Save Texture")]
    public void SaveTexture()
    {
        renderCamera.Render();

        byte[] bytes = toTexture2D(renderCamera.targetTexture).EncodeToPNG();
        System.IO.File.WriteAllBytes("C:/Users/willi/Downloads/Custom Render.png", bytes);
    }
    Texture2D toTexture2D(RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(renderCamera.targetTexture.width, renderCamera.targetTexture.height, TextureFormat.RGBA32, false);
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex;
    }
}
