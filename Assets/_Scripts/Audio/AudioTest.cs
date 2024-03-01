using Audio;
using UnityEngine;
using AudioType = Audio.AudioType;

public class AudioTest : MonoBehaviour
{
    public AudioController AudioController;
#if UNITY_EDITOR
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.C))
        {
            AudioController.PlayAudio(AudioType.ST_01);
        }

        if (Input.GetKeyUp(KeyCode.V))
        {
            AudioController.StopAudio(AudioType.ST_01);
        }
            
        if (Input.GetKeyUp(KeyCode.X))
        {
            AudioController.RestartAudio(AudioType.ST_01);
        }
    }
#endif
}
