using UnityEngine;
public class PostEffectOutline : MonoBehaviour
{
    public Color _outlineColor = Color.red;
    public float _outlineWidth = 5f;

    private Camera _attachedCamera;
    private Camera _tempCam;
    private Material _postMat;
    private Color _previousColor;
    private Shader _postOutline;
    private Shader _drawSimple;

    void Start() {
        _postOutline = Shader.Find("Custom/PostOutline");
        _drawSimple = Shader.Find("Custom/DrawSimple");
        _attachedCamera = GetComponent<Camera>();
        _tempCam = new GameObject().AddComponent<Camera>();
        _tempCam.enabled = false;
        _postMat = new Material(_postOutline);
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination) {
        Graphics.Blit(source, destination);

        // Setup temp cam
        _tempCam.CopyFrom(_attachedCamera);
        _tempCam.clearFlags = CameraClearFlags.Color;
        _tempCam.backgroundColor = Color.black;

        // Set the camera's target texture when rendering
        var rendTex = RenderTexture.GetTemporary(source.width, source.height);
        _tempCam.targetTexture = rendTex;

        // Cull any layer that isn't the outline
        _tempCam.cullingMask = 1 << LayerMask.NameToLayer("Outline");

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