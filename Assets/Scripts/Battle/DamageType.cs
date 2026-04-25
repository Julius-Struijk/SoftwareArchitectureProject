using UnityEngine;

/// <summary>
/// This ScriptableObject class defines a unique damage type and a VFX played
/// when hit with this damage type, basically an enhanced enum.
/// </summary>
[CreateAssetMenu(fileName = "DamageType", menuName = "Scriptable Objects/DamageType")]
public class DamageType : ScriptableObject
{
    public ParticleSystem VFX;
}