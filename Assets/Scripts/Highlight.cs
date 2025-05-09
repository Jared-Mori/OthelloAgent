using UnityEngine;

public class Highlight : MonoBehaviour
{
    [SerializeField] private Color normalColor;
    [SerializeField] private Color mouseOverColor;

    private Material material;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        material = GetComponent<Renderer>().material;
        material.color = normalColor;
    }

    private void OnMouseEnter()
    {
        material.color = mouseOverColor;
    }

    private void OnMouseExit()
    {
        material.color = normalColor;
    }

    private void Oestroy()
    {
        Destroy(material);        
    }
}
