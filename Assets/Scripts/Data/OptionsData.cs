// BURGER

using UnityEngine;

[CreateAssetMenu(fileName = "OptionsData", menuName = "ScriptableObjects/OptionsData")]
public class OptionsData : ScriptableObject
{
    public TextEffect textEffect { get ; set; } = TextEffect.TypeWriter; // change to nothing later, let it be set
}
