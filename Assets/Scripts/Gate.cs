using UnityEngine;
using TMPro;

public enum GateType
{
    Add,
    Multiply,
    Subtract,
    Divide
}

public class Gate : MonoBehaviour
{
    public GateType gateType;
    public int value;
    
    // We will use 3D Text (TextMeshPro) to show the value on the gate
    public TextMeshPro textMesh;

    private bool hasBeenUsed = false;

    void Start()
    {
        UpdateGateText();
    }

    // Called in the editor when we change values so we can see the text update instantly
    void OnValidate()
    {
        if (textMesh != null)
        {
            UpdateGateText();
        }
    }

    private void UpdateGateText()
    {
        if (textMesh == null) return;

        switch (gateType)
        {
            case GateType.Add:
                textMesh.text = "+" + value;
                break;
            case GateType.Multiply:
                textMesh.text = "x" + value;
                break;
            case GateType.Subtract:
                textMesh.text = "-" + value;
                break;
            case GateType.Divide:
                textMesh.text = "/" + value;
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasBeenUsed) return;

        if (other.CompareTag("Player"))
        {
            CrowdManager crowd = other.GetComponent<CrowdManager>();
            if (crowd != null)
            {
                ApplyGateEffect(crowd);
                hasBeenUsed = true;
                
                // Do a fun squishy bounce animation!
                StartCoroutine(BounceAnimation());
            }
        }
    }

    private System.Collections.IEnumerator BounceAnimation()
    {
        float elapsed = 0f;
        float duration = 0.3f;
        Vector3 originalScale = transform.localScale;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // Squish on Y, expand on X/Z using a Sine curve
            float scaleY = Mathf.Lerp(1f, 0.4f, Mathf.Sin(t * Mathf.PI));
            float scaleXZ = Mathf.Lerp(1f, 1.3f, Mathf.Sin(t * Mathf.PI));
            
            transform.localScale = new Vector3(originalScale.x * scaleXZ, originalScale.y * scaleY, originalScale.z * scaleXZ);
            yield return null;
        }
        
        transform.localScale = originalScale;
    }

    private void ApplyGateEffect(CrowdManager crowd)
    {
        int currentCount = crowd.units.Count;
        int amountToChange = 0;
        string floatString = "";
        Color floatColor = Color.green;

        switch (gateType)
        {
            case GateType.Add:
                amountToChange = value;
                crowd.AddUnits(amountToChange);
                floatString = "+" + amountToChange;
                break;
            case GateType.Multiply:
                amountToChange = (currentCount * value) - currentCount;
                crowd.AddUnits(amountToChange);
                floatString = "+" + amountToChange;
                break;
            case GateType.Subtract:
                // Don't let it remove more units than we actually have!
                amountToChange = Mathf.Min(value, currentCount); 
                crowd.RemoveUnits(amountToChange);
                floatString = "-" + amountToChange;
                floatColor = Color.red;
                break;
            case GateType.Divide:
                int newCount = currentCount / value;
                amountToChange = currentCount - newCount;
                crowd.RemoveUnits(amountToChange);
                floatString = "-" + amountToChange;
                floatColor = Color.red;
                break;
        }
        
        if (VFXManager.instance != null)
        {
            VFXManager.instance.SpawnFloatingText(transform.position, floatString, floatColor);
        }
        
        Debug.Log("Gate Hit! Changed crowd by: " + amountToChange);
    }
}
