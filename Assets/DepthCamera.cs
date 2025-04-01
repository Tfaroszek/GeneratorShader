using UnityEngine;

[RequireComponent(typeof(Camera))]
public class DepthCamera : MonoBehaviour
{
    public RenderTexture depthTexture;
    private Camera cam;
    private Light directionalLight;

    void Start()
    {
        cam = GetComponent<Camera>();

        if (depthTexture == null)
        {
            depthTexture = new RenderTexture(1024, 1024, 24, RenderTextureFormat.RFloat);
        }

        cam.targetTexture = depthTexture;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.black;

      
        directionalLight = FindObjectOfType<Light>();
        if (directionalLight != null && directionalLight.type == LightType.Directional)
        {
            Shader.SetGlobalVector("_LightDir", directionalLight.transform.forward);
            Shader.SetGlobalMatrix("_LightVP", directionalLight.transform.localToWorldMatrix);
        }

        // Ustawienie shaderów dla głębokości
        Shader depthShader = Shader.Find("Custom/DepthShader");
        if (depthShader != null)
        {
            cam.SetReplacementShader(depthShader, ""); // Tylko dla renderowania głębokości
        }

        // Przekazanie tekstury głębokości do shaderów cienia
        Shader.SetGlobalTexture("_ShadowMap", depthTexture);
    }

    void LateUpdate()
    {
     
        if (directionalLight != null)
        {
            Shader.SetGlobalVector("_LightDir", directionalLight.transform.forward);
        }

        // Aktualizacja tekstury głębokości
        Shader.SetGlobalTexture("_ShadowMap", depthTexture);
    }
}
