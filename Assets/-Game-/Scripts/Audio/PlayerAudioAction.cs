using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioAction : MonoBehaviour
{
    [SerializeField] AudioGetter audioSfx;
    [SerializeField] bool twoDSound;
    [SerializeField] float delay;

    private void OnEnable()
    {
        this.DelayedAction(delegate
        {
            AudioPlayer.Instance.PlaySFX(audioSfx, twoDSound ? null : transform);
        }, delay);
    }
}
