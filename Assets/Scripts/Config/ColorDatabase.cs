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
            var color = colorEntity?.color ?? Color.white;
            color.a = 1f;
            return color;
        }
    }
}