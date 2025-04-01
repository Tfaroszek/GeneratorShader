using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;

public class HeightmapGenerator : MonoBehaviour
{
    public string heightmapPath;
    public string heightmapPath2;
    public float heightScale = 20.0f;
    public float noiseScale = 0.05f;
    public float noiseIntensity = 2.0f; 
    public Texture2D terrainTexture; 
    private Texture2D heightmapTexture;
    public InputField input;
    public InputField inputTextury;
    public Slider sliderNScale;
    public Slider sliderNIntensity;
    public Slider sliderheightScale;

    private string currentHeightmapPath; 

    void Start()
    {
        currentHeightmapPath = heightmapPath; 
        podmiana(currentHeightmapPath);
        sliderNScale.value = noiseScale;
        sliderNIntensity.value = noiseIntensity;
        sliderheightScale.value = heightScale;
    }

    public void Szum()
    {
        if (sliderNScale != null && sliderNIntensity != null)
        {
            noiseScale = sliderNScale.value;
            noiseIntensity = sliderNIntensity.value;
            heightScale =  sliderheightScale.value;
            podmiana(currentHeightmapPath); 
        }
        else
        {
            Debug.LogError("SliderNScale lub SliderNIntensity jest niezainicjowany.");
        }
    }

    public void Zmiana()
    {
         if (inputTextury != null && !string.IsNullOrEmpty(inputTextury.text))
        {
            string sciezkaTekstury = inputTextury.text;
            terrainTexture = LoadTextureFromPath(sciezkaTekstury);
            if (terrainTexture == null)
            {
                Debug.Log("Nie udało się wczytać tekstury z inputTextury.");
                return;
            }
        }
        if (input != null && !string.IsNullOrEmpty(input.text))
        {
            string nowaMapa = input.text;
            currentHeightmapPath = nowaMapa; 
            podmiana(currentHeightmapPath);
        }
        else
        {
          
            podmiana(currentHeightmapPath);
        }

       
    }

    public void pierwsza()
    {
        currentHeightmapPath = heightmapPath; 
        podmiana(currentHeightmapPath);
    }

    public void druga()
    {
        currentHeightmapPath = heightmapPath2; 
        podmiana(currentHeightmapPath);
    }

    private void podmiana(string mapPath)
    {
        if (string.IsNullOrEmpty(mapPath))
        {
            Debug.LogError("Nie podano ścieżki do tekstury wysokości.");
            return;
        }
        heightmapTexture = LoadTextureFromPath(mapPath);
        if (heightmapTexture == null)
        {
            Debug.LogError("Nie udało się wczytać tekstury.");
            return;
        }

        if (heightmapTexture.width > 256 || heightmapTexture.height > 256)
        {
            heightmapTexture = ScaleTexture(heightmapTexture, 256, 256);
            Debug.Log("Tekstura została przeskalowana do 256x256.");
        }

        Mesh mesh = GenerateMesh(heightmapTexture);
        GetComponent<MeshFilter>().mesh = mesh;
        ApplyTexture();
    }

    Texture2D LoadTextureFromPath(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError("Plik nie istnieje: " + path);
            return null;
        }

        byte[] fileData = File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(2, 2);
        if (texture.LoadImage(fileData))
        {
            return texture;
        }
        return null;
    }

    Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight);
        Color[] pixels = new Color[targetWidth * targetHeight];

        for (int y = 0; y < targetHeight; y++)
        {
            for (int x = 0; x < targetWidth; x++)
            {
                float u = (float)x / (float)targetWidth;
                float v = (float)y / (float)targetHeight;
                pixels[y * targetWidth + x] = source.GetPixelBilinear(u, v);
            }
        }

        result.SetPixels(pixels);
        result.Apply();
        return result;
    }

    Mesh GenerateMesh(Texture2D heightmap)
    {
        int width = heightmap.width;
        int height = heightmap.height;
        int size = Mathf.Max(width, height);
        List<Vector3> vertices = new List<Vector3>();
        List<int> indices = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        for (int y = 0; y < size; ++y)
        {
            for (int x = 0; x < size; ++x)
            {
                if (x < width && y < height)
                {
                    float heightValue = heightmap.GetPixel(x, y).grayscale;
                    float vertexHeight = heightValue * heightScale;
                    
                    // Dodanie szumu Perlin Noise dla wygładzenia
                    float perlinValue = Mathf.PerlinNoise(x * noiseScale, y * noiseScale) * noiseIntensity;
                    vertexHeight = Mathf.Lerp(vertexHeight, perlinValue, 0.3f);
                    
                    vertices.Add(new Vector3(x - size / 2.0f, vertexHeight, y - size / 2.0f));
                    uvs.Add(new Vector2((float)x / size, (float)y / size));

                    if (x < size - 1 && y < size - 1)
                    {
                        int topLeft = y * size + x;
                        int topRight = topLeft + 1;
                        int bottomLeft = (y + 1) * size + x;
                        int bottomRight = bottomLeft + 1;

                        indices.Add(topLeft);
                        indices.Add(bottomLeft);
                        indices.Add(topRight);

                        indices.Add(topRight);
                        indices.Add(bottomLeft);
                        indices.Add(bottomRight);
                    }
                }
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = indices.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();

        return mesh;
    }

    void ApplyTexture()
    {
        if (terrainTexture != null)
        {
            GetComponent<MeshRenderer>().material.mainTexture = terrainTexture;
        }
        else
        {
            Debug.LogWarning("Brak tekstury terenu, upewnij się, że przypisano ją w edytorze Unity.");
        }
    }
}
