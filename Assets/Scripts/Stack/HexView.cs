using UnityEngine;

namespace Stack
{
    public class HexView : MonoBehaviour
    {
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private float height;

        public float Height => height;
        
        public void SetColor(Color color)
        {
            meshRenderer.material.color = color;
        }
    }
}