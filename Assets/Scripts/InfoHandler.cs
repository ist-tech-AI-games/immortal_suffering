using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

// public struct ScreenInfo
// {
//     public Color[] pixels;
//     public float updatedTime;

//     public ScreenInfo(Color[] pixels, float updatedTime)
//     {
//         this.pixels = pixels;
//         this.updatedTime = updatedTime;
//     }
// }

public class InfoHandler : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private ComputeShader computeShader;
    [SerializeField] private RenderTexture entireScreenTexture; // 전체 화면 텍스처

    [Serializable]
    public struct ColorMap
    {
        public int Id;
        public Color Color;
    }
    const int COLORMAPSTRUCTSIZE = sizeof(int) + 4 * sizeof(float);

    [Tooltip("Mapping of color - id. Warning: max 16 entries.")]
    [SerializeField] private ColorMap[] colorMaps;
    [SerializeField] private ColorMap fallbackMap;
    [SerializeField] private Vector2Int sampleSize = new(160, 90);
    [SerializeField, Range(0f, 1f)] private float compareThreshold = .2f;
    [SerializeField, Range(0f, 1f)] private float alphaThreshold = .5f;
    [Header("Outputs - You can modify these for debugging")]
    [SerializeField] private RawImage rawImage;

    [Header("Outputs - Don't Modify")]
    [SerializeField] private RenderTexture debugTexture;
    [SerializeField] private int[] result;

    // === Property IDs ===
    static readonly int
        _ResultID = Shader.PropertyToID("Result"),
        _DebugTexID = Shader.PropertyToID("DebugTex"),
        _SourceTexID = Shader.PropertyToID("SourceTex"),
        _ColorCountID = Shader.PropertyToID("ColorCount"),
        _ColorMapID = Shader.PropertyToID("ColorMap"),
        // _ColorIDsID = Shader.PropertyToID("ColorIDs"),
        _FallbackIDID = Shader.PropertyToID("FallbackColorID"),
        _CompareThresholdID = Shader.PropertyToID("CompareThreshold"),
        _AlphaThresholdID = Shader.PropertyToID("AlphaThreshold"),
        // _ColorValuesID = Shader.PropertyToID("ColorValues"),
        _FallbackColorID = Shader.PropertyToID("FallbackColor"),
        _WidthID = Shader.PropertyToID("Width"),
        _HeightID = Shader.PropertyToID("Height");

    void OnEnable()
    {
        InitMinimapMapping();
    }

    int frame = 0;
    void FixedUpdate()
    {
        if (frame++ % 3 != 0) return;

        RunMinimapMapping(result);
    }

    public void InitMinimapMapping()
    {
        var desc = entireScreenTexture.descriptor;
        desc.width = sampleSize.x; desc.height = sampleSize.y;
        desc.enableRandomWrite = true;
        debugTexture = new RenderTexture(desc);

        if (rawImage != null) rawImage.texture = debugTexture;
        result = new int[sampleSize.x * sampleSize.y];
    }

    public bool RunMinimapMapping(int[] resultBuffer)
    {
        int pixelCnt = sampleSize.x * sampleSize.y;
        if (resultBuffer.Length != sampleSize.x * sampleSize.y)
        {
            Debug.LogError("Assertion Failed: resultBuffer.Length == sampleSize.x * sampleSize.y");
            return false;
        }
        int maxMapping = 16;
        if (colorMaps.Length > maxMapping)
        {
            Debug.LogError("Assertion Failed: colorMaps.Length <= maxMapping");
            return false;
        }

        int kernel = computeShader.FindKernel("CSMain");
        ComputeBuffer resultCB = new(pixelCnt, sizeof(int));

        ComputeBuffer colorMapCB = new(colorMaps.Length, COLORMAPSTRUCTSIZE);
        colorMapCB.SetData(colorMaps);

        // int[] ids = new int[maxMapping];
        // Vector4[] colors = new Vector4[maxMapping];

        // for (int i = 0; i < colorMaps.Length; i++)
        // {
        //     ids[i] = colorMaps[i].Id;
        //     colors[i] = colorMaps[i].Color;
        // }

        computeShader.SetTexture(kernel, _SourceTexID, entireScreenTexture);
        computeShader.SetTexture(kernel, _DebugTexID, debugTexture);
        computeShader.SetBuffer(kernel, _ResultID, resultCB);
        computeShader.SetBuffer(kernel, _ColorMapID, colorMapCB);
        computeShader.SetInt(_WidthID, sampleSize.x);
        computeShader.SetInt(_HeightID, sampleSize.y);
        computeShader.SetInt(_ColorCountID, colorMaps.Length);
        computeShader.SetInt(_FallbackIDID, fallbackMap.Id);
        computeShader.SetFloat(_CompareThresholdID, compareThreshold);
        computeShader.SetFloat(_AlphaThresholdID, alphaThreshold);
        // computeShader.SetInts(_ColorIDsID, ids);
        computeShader.SetVector(_FallbackColorID, fallbackMap.Color);
        // computeShader.SetVectorArray(_ColorValuesID, colors);

        Vector2Int threadGroups = Vector2Int.CeilToInt(new Vector2(entireScreenTexture.width, entireScreenTexture.height) / 8f);
        computeShader.Dispatch(kernel, threadGroups.x, threadGroups.y, 1);

        resultCB.GetData(resultBuffer);
        resultCB.Release();

        return true;
    }

    // --debug--
    [ContextMenu("Debug: Write to file")]
    public void WriteResultToFile()
    {
        StringBuilder stringBuilder = new();
        for (int i = 0; i < sampleSize.y; i++)
        {
            for (int j = 0; j < sampleSize.x; j++)
                stringBuilder.AppendFormat("{0:00} ", result[i * sampleSize.x + j]);
            stringBuilder.Append("\n");
        }

        File.WriteAllText(Path.Combine(Application.dataPath, "ColorIds.txt"), stringBuilder.ToString());
    }



    // [SerializeField] private float updatedTime;
    // [SerializeField] private Color[] pixels;

    // void FixedUpdate()
    // {
    //     // Debug.Log("FixedUpdate called. Time.deltaTime: " + Time.fixedTime);
    //     AsyncGPUReadback.Request(entireScreenTexture, 0, OnCompleteReadback);
    // }

    // void OnCompleteReadback(AsyncGPUReadbackRequest request)
    // {
    //     // Read the pixel data from the request
    //     Color[] pixels = request.GetData<Color>().ToArray();
    //     updatedTime = Time.fixedTime;
    //     // Debug.Log(Time.fixedTime + " Readback completed successfully. Pixel count: " + pixels.Length);
    // }

    // public ScreenInfo GetScreenInfo()
    // {
    //     return new ScreenInfo(pixels, updatedTime);
    // }
}
