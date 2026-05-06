using System;
using UnityEngine;

namespace CMGTSA.Audio
{
    /// <summary>
    /// Slice-7 polish: SO Factory keyed by <see cref="AudioSlot"/>. Designers maintain the
    /// list in the Inspector; <see cref="SFXController"/> looks up clips by slot. Same
    /// data-driven pattern as <c>EnemyData</c>, <c>ItemData</c>, <c>SkillData</c>.
    /// </summary>
    [CreateAssetMenu(fileName = "AudioCatalog", menuName = "Scriptable Objects/Audio/AudioCatalog")]
    public class AudioCatalog : ScriptableObject
    {
        [Serializable]
        public class Entry
        {
            public AudioSlot slot;
            public AudioClip clip;
            [Range(0f, 1f)] public float volume = 1f;
        }

        [SerializeField] private Entry[] entries = Array.Empty<Entry>();

        public bool TryGet(AudioSlot slot, out AudioClip clip, out float volume)
        {
            for (int i = 0; i < entries.Length; i++)
            {
                if (entries[i].slot == slot && entries[i].clip != null)
                {
                    clip = entries[i].clip;
                    volume = entries[i].volume;
                    return true;
                }
            }
            clip = null; volume = 0f;
            return false;
        }
    }
}
