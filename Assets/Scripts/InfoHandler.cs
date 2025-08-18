using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class InfoHandler : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private ComputeShader computeShader;
    [SerializeField] private RenderTexture entireScreenTexture; // 전체 화면 텍스처

    [Serializable, StructLayout(LayoutKind.Sequential, Size = 32)]
    public struct ColorMap
    {
        public int Id;
        [HideInInspector] public Vector3 _Padding;
        public Color Color;
    }
    const int COLORMAPSTRUCTSIZE = 32;

    [Tooltip("Mapping of color - id.")]
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
        _FallbackIDID = Shader.PropertyToID("FallbackColorID"),
        _CompareThresholdID = Shader.PropertyToID("CompareThreshold"),
        _AlphaThresholdID = Shader.PropertyToID("AlphaThreshold"),
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
        debugTexture.filterMode = FilterMode.Point;

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

        int kernel = computeShader.FindKernel("CSMain");
        ComputeBuffer resultCB = new(pixelCnt, sizeof(int));

        ComputeBuffer colorMapCB = new(colorMaps.Length, COLORMAPSTRUCTSIZE);
        colorMapCB.SetData(colorMaps);

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
        computeShader.SetVector(_FallbackColorID, fallbackMap.Color);

        Vector2Int threadGroups = Vector2Int.CeilToInt((Vector2)sampleSize / 8f);
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
}
