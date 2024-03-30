using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickUp : MonoBehaviour, IHitable
{
    [SerializeField] WeaponData weapon;
    [SerializeField] float rotateSpeed = 90f;
    [SerializeField] AudioGetter pickupSfx;


    private PlayerScript player;

    public void Hit(RaycastHit hit, int damage = 1)
    {
        player.SwitchWeapon(weapon);
        AudioPlayer.Instance.PlaySFX(pickupSfx);
        Destroy(gameObject);
    }


    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerScript>();
    }


    void Update()
    {
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
    }
   
}
