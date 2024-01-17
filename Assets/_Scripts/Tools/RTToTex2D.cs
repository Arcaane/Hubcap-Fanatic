using UnityEngine;

public class RTToTex2D : MonoBehaviour
{
    public Camera cameraWithRenderTexture;
    public RenderTexture renderTexture;
    
    public string savePath = "Assets/_Art/Textures/RT";
    public string textureName = "Map_1";

    [ContextMenu("Convert RenderTexture to Texture2D and Save as PNG")]
    void ConvertRenderTextureToTexture2DAndSave()
    {
        if (cameraWithRenderTexture.targetTexture != null)
        {
            renderTexture = cameraWithRenderTexture.targetTexture;
        }
        else
        {
            Debug.LogError("No RenderTexture assigned to camera.");
            return;
        }
        
        RenderTexture.active = renderTexture;

        Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height);
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();

        RenderTexture.active = null;


        byte[] pngBytes = texture2D.EncodeToPNG();
        System.IO.File.WriteAllBytes(  textureName + ".png", pngBytes);
    }
}