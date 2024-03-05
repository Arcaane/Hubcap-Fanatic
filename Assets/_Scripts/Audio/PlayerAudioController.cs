using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    public class PlayerAudioController : MonoBehaviour
    {
        public static PlayerAudioController instance;
        
        public bool debug;
        public AudioTrack[] tracks;
        private Hashtable m_AudioTable; // relationship of audio types (key) and tracks (value)

        [SerializeField] private AudioSource[] sources;
        [SerializeField] private List<AudioSource> audioSourceList = new List<AudioSource>();
        
        [Serializable]
        public class AudioObject
        {
            public AudioType type;
            public AudioClip clip;
            public float delay = 0.0f;
            public bool isPriority = false;
        }

        [Serializable]
        public class AudioTrack
        {
            public AudioObject[] audio;
        }
        
        #region Unity Functions
        private void Awake()
        {
            if (!instance) {
                Configure();
            }
        }
        #endregion

        #region Public Functions

        public void PlayAudio(AudioType _type, float _delay = 0.0F)
        {
            if (_delay == 0.0f) { AssignAudioClipToListenerAndPlay(_type); }
            else { StartCoroutine(DelaySound(_type, _delay)); } 
        }
        #endregion

        #region Private Functions

        private void Configure()
        {
            instance = this;
            m_AudioTable = new Hashtable();
            GenerateAudioTable();
            GenerateAudioSources();
        }

        private void GenerateAudioSources()
        {
            var childSources = GetComponentsInChildren<AudioSource>();
            sources = new AudioSource[childSources.Length];
            
            for (int i = 0; i < sources.Length; i++)
            {
                sources[i] = childSources[i];
            }
            
            foreach (var t in sources)
            {
                audioSourceList.Add(t);
            }
        }

        private void GenerateAudioTable()
        {
            foreach (AudioTrack _track in tracks)
            {
                foreach (AudioObject _obj in _track.audio)
                {
                    // do not duplicate keys
                    if (m_AudioTable.ContainsKey(_obj.type))
                    {
                        LogWarning("You are trying to register audio [" + _obj.type + "] that has already been registered.");
                    }
                    else
                    {
                        m_AudioTable.Add(_obj.type, _obj.clip);
                        Log("Registering audio KEY = [" + _obj.type + "] => Value =  [" + _obj.clip + "]");
                    }
                }
            }
        }

        private bool IsAudioPriority(AudioType _type)
        {
            foreach (AudioTrack _track in tracks)
            {
                foreach (AudioObject aObj in _track.audio)
                {
                    if (aObj.type == _type) return aObj.isPriority;
                }
            }

            LogWarning($"{_type} sound priority not found");
            return false;
        }

        private AudioClip GetAudioClip(AudioType _type)
        {
            if (!m_AudioTable.ContainsKey(_type))
            {
                LogWarning($"AudioClip from [{_type}] not found");
                return null;
            }

            return (AudioClip) m_AudioTable[_type];
        }
        
        private IEnumerator DelaySound(AudioType _type, float _delay)
        {
            yield return new WaitForSeconds(_delay);
            AssignAudioClipToListenerAndPlay(_type);
        }

        private void AssignAudioClipToListenerAndPlay(AudioType _type)
        {
            StartCoroutine(DequeueSource(_type));
        }

        private IEnumerator DequeueSource(AudioType _type)
        {
            if (audioSourceList.Count == 0) {
                Log("audioSourceList full -> If console got +50 at the end of the game pls add more listener");
                yield break;
            }
            
            Debug.Log((float)audioSourceList.Count / sources.Length);

            if ((float)audioSourceList.Count / sources.Length < 0.18f)
            {
                LogWarning($"AUDIO SOURCES POOL SOON EMPTY -> ONLY PRIORITY SOUND WILL BE PLAYED");
                if (!IsAudioPriority(_type)) yield break;
            }

            AudioClip _clip = GetAudioClip(_type);
            AudioSource _source = audioSourceList[0];
            
            Debug.Log(_source.gameObject.name);
            audioSourceList.Remove(_source);
            _source.PlayOneShot(_clip);
            yield return new WaitForSeconds(_clip.length);
            EnQueueSource(_source);
        }

        private void EnQueueSource(AudioSource _source)
        {
            audioSourceList.Add(_source);
        }
        
        private void Log(string _msg)
        {
            if (!debug) return;
            Debug.Log("[Audio Controller]: " + _msg);
        }

        private void LogWarning(string _msg)
        {
            if (!debug) return;
            Debug.LogWarning("[Audio Controller]: " + _msg);
        }
        #endregion
    }
}