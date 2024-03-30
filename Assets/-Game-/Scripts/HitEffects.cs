using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffects : MonoBehaviour, IHitable
{


    [SerializeField] GameObject effectsPrefabs;
    [SerializeField] AudioGetter hitSfx;

    private ParticleSystem effectsCache;
    public void Hit(RaycastHit hit, int damage = 1)
    {
       if (effectsPrefabs != null)
        {
            effectsCache.transform.position = hit.point;
            effectsCache.transform.rotation = Quaternion.LookRotation(hit.normal);
            AudioPlayer.Instance.PlaySFX(hitSfx, effectsCache.transform);
            effectsCache.Play();
        }
    }
   
    void Start()
    {
        if (effectsPrefabs !=null)
        {
            GameObject effectsTemp = Instantiate(effectsPrefabs, transform);
            effectsCache = effectsTemp.GetComponent<ParticleSystem>();
        }
    }
}
