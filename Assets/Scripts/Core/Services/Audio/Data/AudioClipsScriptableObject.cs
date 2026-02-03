using System;
using System.Collections.Generic;

using UnityEngine;

namespace Services.AudioService
{
    [Serializable]
    public class AudioClipEntry
    {
        public AudioClipType type;
        public AudioClip clip;
    }
    
    [CreateAssetMenu(fileName = "AudioClips", menuName = "PuzzleRoom/Audio/AudioClips")]
    public class AudioClipsScriptableObject : ScriptableObject
    {
        [SerializeField]
        private List<AudioClipEntry> _clips = new();

        private Dictionary<AudioClipType, AudioClip> _lookup;

        private void OnEnable()
        {
            BuildLookup();
        }

        private void BuildLookup()
        {
            _lookup = new Dictionary<AudioClipType, AudioClip>();

            foreach (var entry in _clips)
            {
                if (entry == null || entry.clip == null)
                    continue;

                if (_lookup.ContainsKey(entry.type))
                {
                    Debug.LogWarning($"Duplicate AudioClipType: {entry.type} in {name}");
                    continue;
                }

                _lookup.Add(entry.type, entry.clip);
            }
        }

        public bool TryGetClip(AudioClipType type, out AudioClip clip)
        {
            if (_lookup == null)
                BuildLookup();

            return _lookup.TryGetValue(type, out clip);
        }

        public AudioClip GetClip(AudioClipType type)
        {
            if (TryGetClip(type, out var clip))
                return clip;

            Debug.LogError($"AudioClip not found for type: {type} in {name}");
            return null;
        }
    }
}