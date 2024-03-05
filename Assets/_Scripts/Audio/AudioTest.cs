using Audio;
using UnityEngine;
using UnityEngine.Serialization;
using AudioType = Audio.AudioType;

public class AudioTest : MonoBehaviour
{
    [FormerlySerializedAs("AudioController")] public PlayerAudioController playerAudioController;
#if UNITY_EDITOR
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.C))
        {
            playerAudioController.PlayAudio(AudioType.ST_01);
        }
        
        if (Input.GetKeyUp(KeyCode.V))
        {
            playerAudioController.PlayAudio(AudioType.SFX_01);
        }
        
        if (Input.GetKeyUp(KeyCode.B))
        {
            playerAudioController.PlayAudio(AudioType.SFX_02);
        }
    }
#endif
}
