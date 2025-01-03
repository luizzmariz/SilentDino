using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using FMODUnity; // Importar FMOD Unity Integration
using FMOD.Studio;

public class InventarioController : MonoBehaviour
{


    public List<ItemInfo> itens; // Lista de itens configurada no Inspector
    public int indiceAtual = 0;

    public Transform itemViewerParent; // Local onde o item ser� instanciado (em frente � c�mera)
    public RawImage itemDisplay;
    public TextMeshProUGUI nomeDoItemText;
    public TextMeshProUGUI descricaoDoItemText;

    public GameObject leftArrow;  // Refer�ncia � seta esquerda
    public GameObject rightArrow; // Refer�ncia � seta direita

    public EventReference nextSound;

    // Velocidade de rota��o do item
    public float rotationSpeed = 20f;

    private GameObject itemAtualInstanciado;

    void Start()
    {
        AtualizarInventario();
    }

    void Update()
    {
       
        // Navega��o com A e D
        if (Input.GetKeyDown(KeyCode.A))
        {
            ItemAnterior();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            ProximoItem();
        }

        // Rota��o do item atual
        if (itemAtualInstanciado != null)
        {
            itemAtualInstanciado.transform.Rotate(Vector3.forward, rotationSpeed * Time.unscaledDeltaTime);
        }
    }
    public void AdicionarItem(ItemInfo novoItem)
    {
        itens.Add(novoItem);
        // Caso seja o primeiro item, o �ndice atual precisa estar correto
        if (itens.Count == 1)
        {
            indiceAtual = 0;
        }
        AtualizarInventario();
    }
    public void ProximoItem()
    {
        if (itens.Count == 0) return;
        indiceAtual = (indiceAtual + 1) % itens.Count;
        RuntimeManager.PlayOneShot(nextSound);
        AtualizarInventario();
    }

    public void ItemAnterior()
    {
        if (itens.Count == 0) return;
        indiceAtual = (indiceAtual - 1 + itens.Count) % itens.Count;
        RuntimeManager.PlayOneShot(nextSound);
        AtualizarInventario();
    }

    void AtualizarInventario()
    {
        // Limpa o item anterior
        foreach (Transform child in itemViewerParent)
        {
            Destroy(child.gameObject);
        }

        if (itens.Count == 0)
        {
            nomeDoItemText.text = "";
            descricaoDoItemText.text = "";
            leftArrow.SetActive(false);
            rightArrow.SetActive(false);
            return;
        }

        // Instancia o item atual
        itemAtualInstanciado = Instantiate(itens[indiceAtual].prefabModelo, itemViewerParent);
        itemAtualInstanciado.transform.localPosition = Vector3.zero;
        itemAtualInstanciado.transform.localRotation = Quaternion.identity;
        itemAtualInstanciado.transform.localScale = Vector3.one;

        // Atualiza textos
        nomeDoItemText.text = itens[indiceAtual].nome;
        descricaoDoItemText.text = itens[indiceAtual].descricao;

        // Controla setas
        if (itens.Count <= 1)
        {
            leftArrow.SetActive(false);
            rightArrow.SetActive(false);
        }
        else
        {
            leftArrow.SetActive(true);
            rightArrow.SetActive(true);
        }
    }
    public bool BuscarItem(string nome)
    {
        
        for(int x = 0; x < itens.Count; x++)
        {
            if (itens[x].nome == nome)
            {
                return (true);
            }
        }

        return false;
        
    }


    /// <summary>
    /// Remove um item do invent�rio pelo nome.
    /// </summary>
    public void RemoverItem(string nomeItem)
    {
        // Encontra o �ndice do item
        int indexToRemove = -1;
        for (int i = 0; i < itens.Count; i++)
        {
            if (itens[i].nome == nomeItem)
            {
                indexToRemove = i;
                break;
            }
        }

        // Se n�o encontrou o item, n�o faz nada
        if (indexToRemove == -1) return;

        itens.RemoveAt(indexToRemove);

        // Ajusta o �ndiceAtual caso tenha ficado fora do intervalo
        if (itens.Count == 0)
        {
            indiceAtual = 0;
        }
        else
        {
            // Garante que indiceAtual fique em um valor v�lido ap�s remo��o
            if (indiceAtual >= itens.Count)
            {
                indiceAtual = itens.Count - 1;
            }
        }

        AtualizarInventario();
    }
}
