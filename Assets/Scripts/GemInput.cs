using System;
using UnityEngine;
public class GemInput : MonoBehaviour
{
    private IGemsInput UsedGemsInput => GemsInput.Instance;
    
    private void OnMouseDown()
    {
        UsedGemsInput.OnGemStartInput(gameObject);
    }
}