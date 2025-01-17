using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class WaterfallEffect : MonoBehaviour
{
    [Header("Texture Movement")]
    public Vector2 flowSpeed = new Vector2(0, -0.5f); // Velocidade do movimento (vertical)

    [Header("Foam Settings")]
    public float foamSpeed = 0.3f; // Velocidade da espuma
    public Vector2 foamFlowDirection = new Vector2(0.1f, -0.7f); // Direção da espuma

    private Renderer rend;
    private Material mat;

    void Start()
    {
        // Obtém o material do objeto
        rend = GetComponent<Renderer>();
        mat = rend.material;
    }

    void Update()
    {
        // Calcula o deslocamento da textura
        Vector2 offset = Time.time * flowSpeed;

        // Aplica o deslocamento da textura principal
        if (mat.HasProperty("_MainTex"))
        {
            mat.SetTextureOffset("_MainTex", offset);
        }

        // Aplica o deslocamento para espuma (se houver)
        if (mat.HasProperty("_FoamTex"))
        {
            Vector2 foamOffset = Time.time * foamFlowDirection * foamSpeed;
            mat.SetTextureOffset("_FoamTex", foamOffset);
        }
    }
}
