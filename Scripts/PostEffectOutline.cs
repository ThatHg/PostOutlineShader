using UnityEngine;
public class PostEffectOutline : MonoBehaviour
{
    public Color _outlineColor = Color.red;
    public float _outlineWidth = 5f;
    public string _layerName = "Outline";

    private Camera _attachedCamera;
    private Camera _tempCam;
    private Material _postMat;
    private Color _previousColor;
    private Shader _postOutline;
    private Shader _drawSimple;

    void Start() {
        // Find shader
        _postOutline = Shader.Find("Custom/PostOutline");
        _drawSimple = Shader.Find("Custom/DrawSimple");

        // Get attached camera
        _attachedCamera = GetComponent<Camera>();

        // Setup temp cam
        _tempCam = new GameObject().AddComponent<Camera>();
        _tempCam.enabled = false;
        _tempCam.transform.SetParent(_attachedCamera.transform);
        _tempCam.CopyFrom(_attachedCamera);
        _tempCam.clearFlags = CameraClearFlags.Color;
        _tempCam.backgroundColor = Color.black;
        _tempCam.cullingMask = 1 << LayerMask.NameToLayer(_layerName);

        // Create our post outline material
        _postMat = new Material(_postOutline);
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination) {
        // Set the camera's target texture when rendering
        var rendTex = RenderTexture.GetTemporary(source.width, source.height);
        _tempCam.targetTexture = rendTex;

        // Set scene texture
        _postMat.SetTexture("_SceneTex", source);
        _postMat.SetFloat("_Outline", _outlineWidth);

        if (_previousColor != _outlineColor) {
            _previousColor = _outlineColor;
            _postMat.SetColor("_Color", _outlineColor);
        }

        // Render all objects this camera can render, but with our custom shader.
        _tempCam.RenderWithShader(_drawSimple, "");

        // Copy the temporary RT to the final image
        Graphics.Blit(rendTex, destination, _postMat);

        RenderTexture.ReleaseTemporary(rendTex);
    }
}