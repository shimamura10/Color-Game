using UnityEngine;
using UnityEngine.UI;

public class GrabPassTest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject originalLayer;
    public Material originalMaterial;
    void Start()
    {
        Image image = originalLayer.GetComponent<Image>();
        Material material = image.material;
        material.SetColor("_Color", new Color(0.8f, 0, 0, 1));
        material.SetFloat("_Composition", 2);

        for (int i = 0; i < 100; i++)
        {
            InstantiateLayer();
        }
    }

    void InstantiateLayer() {
        // このゲームオブジェクトを複製
        GameObject layer = Instantiate(originalLayer, originalLayer.transform.parent);
        var newMaterial = Instantiate(originalMaterial);
        newMaterial.SetColor("_Color", new Color(0, 0.5f, 0, 1));
        newMaterial.SetFloat("_Composition", 3);
        layer.GetComponent<Image>().material = newMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
