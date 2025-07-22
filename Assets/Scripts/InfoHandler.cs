using UnityEngine;
using UnityEngine.Rendering;

public struct ScreenInfo
{
    public Color[] pixels;
    public float updatedTime;

    public ScreenInfo(Color[] pixels, float updatedTime)
    {
        this.pixels = pixels;
        this.updatedTime = updatedTime;
    }
}

public class InfoHandler : MonoBehaviour
{
    [SerializeField] private RenderTexture entireScreenTexture; // 전체 화면 텍스처
    [SerializeField] private float updatedTime;
    [SerializeField] private Color[] pixels;

    void FixedUpdate()
    {
        // Debug.Log("FixedUpdate called. Time.deltaTime: " + Time.fixedTime);
        AsyncGPUReadback.Request(entireScreenTexture, 0, OnCompleteReadback);
    }

    void OnCompleteReadback(AsyncGPUReadbackRequest request)
    {
        // Read the pixel data from the request
        Color[] pixels = request.GetData<Color>().ToArray();
        updatedTime = Time.fixedTime;
        // Debug.Log(Time.fixedTime + " Readback completed successfully. Pixel count: " + pixels.Length);
    }

    public ScreenInfo GetScreenInfo()
    {
        return new ScreenInfo(pixels, updatedTime);
    }
}
