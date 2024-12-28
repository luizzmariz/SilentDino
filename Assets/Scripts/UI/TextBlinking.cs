using UnityEngine;
using TMPro;

public class TextFragmentEffect : MonoBehaviour
{
    public TMP_Text textMeshPro;

    public float fragmentSpeed = 2f; // Velocidade do movimento dos fragmentos
    public float reconstructionSpeed = 1f; // Velocidade de reconstrução
    public float fragmentationDistance = 50f; // Distância que os fragmentos se afastam

    public Color defaultColor = Color.white; // Cor normal do texto
    public Color flickerColor = Color.red; // Cor do flicker
    public float flickerDuration = 0.1f; // Duração do flicker
    public float flickerChance = 0.1f; // Chance do flicker acontecer a cada frame

    private TMP_TextInfo textInfo;
    private Vector3[][] originalVertices;
    private Vector3[][] currentVertices;

    private bool isFragmenting = true; // Controla se está fragmentando ou reconstruindo
    private float flickerTimer = 0f; // Timer para controlar o flicker

    void Start()
    {
        if (!textMeshPro)
            textMeshPro = GetComponent<TMP_Text>();

        textMeshPro.ForceMeshUpdate();
        textInfo = textMeshPro.textInfo;

        // Armazena as posições originais dos vértices
        originalVertices = new Vector3[textInfo.meshInfo.Length][];
        currentVertices = new Vector3[textInfo.meshInfo.Length][];
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            originalVertices[i] = textInfo.meshInfo[i].vertices;
            currentVertices[i] = (Vector3[])textInfo.meshInfo[i].vertices.Clone();
        }
    }

    void Update()
    {
        if (textInfo == null || textInfo.characterCount == 0) return;

        // Atualiza o mesh do texto
        textMeshPro.ForceMeshUpdate();
        textInfo = textMeshPro.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;

            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;

            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

            // Fragmentação
            if (isFragmenting)
            {
                Vector3 center = (vertices[vertexIndex] + vertices[vertexIndex + 2]) / 2;
                for (int j = 0; j < 4; j++)
                {
                    Vector3 direction = (vertices[vertexIndex + j] - center).normalized;
                    currentVertices[materialIndex][vertexIndex + j] =
                        Vector3.Lerp(vertices[vertexIndex + j], vertices[vertexIndex + j] + direction * fragmentationDistance, Time.deltaTime * fragmentSpeed);
                }
            }
            // Reconstrução
            else
            {
                for (int j = 0; j < 4; j++)
                {
                    currentVertices[materialIndex][vertexIndex + j] =
                        Vector3.Lerp(vertices[vertexIndex + j], originalVertices[materialIndex][vertexIndex + j], Time.deltaTime * reconstructionSpeed);
                }
            }
        }

        // Aplica os vértices alterados ao mesh
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            var meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = currentVertices[i];
            textMeshPro.UpdateGeometry(meshInfo.mesh, i);
        }

        // Alterna entre fragmentação e reconstrução
        if (isFragmenting && Random.Range(0f, 1f) < 0.01f)
        {
            isFragmenting = false;
        }
        else if (!isFragmenting && Random.Range(0f, 1f) < 0.01f)
        {
            isFragmenting = true;
        }

        // Controle do Flicker
        HandleFlicker();
    }

    private void HandleFlicker()
    {
        if (flickerTimer > 0)
        {
            flickerTimer -= Time.deltaTime;
            if (flickerTimer <= 0)
            {
                textMeshPro.color = defaultColor;
            }
        }
        else if (Random.Range(0f, 1f) < flickerChance)
        {
            textMeshPro.color = flickerColor;
            flickerTimer = flickerDuration;
        }
    }
}
