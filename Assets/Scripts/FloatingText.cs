using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float fadeSpeed = 1.5f;
    
    private TextMeshPro textMesh;
    private Color textColor;

    void Start()
    {
        textMesh = GetComponent<TextMeshPro>();
        if (textMesh != null)
        {
            textColor = textMesh.color;
        }
        
        // Destroy this object after 2 seconds no matter what
        Destroy(gameObject, 2f);
    }

    void Update()
    {
        // Float upwards
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        // Fade out
        if (textMesh != null)
        {
            textColor.a -= fadeSpeed * Time.deltaTime;
            textMesh.color = textColor;
        }
    }

    public void Setup(string text, Color color, float scale = 1f)
    {
        textMesh = GetComponent<TextMeshPro>();
        if (textMesh != null)
        {
            textMesh.text = text;
            textMesh.color = color;
        }
        
        // Apply the custom scale!
        transform.localScale = Vector3.one * scale;
    }
}
