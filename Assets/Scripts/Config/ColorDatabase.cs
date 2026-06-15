using System.Collections.Generic;
using UnityEngine;

namespace Config
{
    [CreateAssetMenu(menuName = "Configs/ColorDatabase")]
    public class ColorDatabase : ScriptableObject
    {
        public List<ColorEntity> colors;

        public Color GetColor(ColorType type)
        {
            var colorEntity = colors.Find(x => x.type == type);
            
            return colorEntity?.color ?? new Color(); 
        }
    }
}