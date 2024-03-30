using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="CustomWeaponData", menuName ="Weapon Data")]
public class WeaponData : ScriptableObject
{
    public System.Action<int> OnWeaponFired = delegate { };

    [SerializeField] FireType type;
    [SerializeField] float rate = 0.15f;
    [SerializeField] int maxAmmo;
    [SerializeField] int damageValue;
    [SerializeField] bool defaultWeapon;
    [SerializeField] GameObject muzzleFX;
    [SerializeField] AudioGetter gunShotSfx, reloadSfx, emptySfx, reloadWarningSfx;
    [SerializeField] float fxScale = 0.1f;
    [SerializeField] Sprite weaponIcon;

    private Camera cam;
    private ParticleSystem cachedFX;
    private PlayerScript player;
    private int currentAmmo;
    private float nextFireTime;

    public Sprite GetIcon { get => weaponIcon; }

    public void SetupWeapon(Camera cam, PlayerScript player)
    {
        this.cam = cam;
        this.player = player;
        nextFireTime = 0f;
        currentAmmo = maxAmmo;
        OnWeaponFired(currentAmmo);
        if (muzzleFX != null)
        {
            GameObject temp = Instantiate(muzzleFX);
            temp.transform.localScale = Vector3.one * fxScale;
            player.SetMuzzleFx(temp.transform);
            cachedFX = temp.GetComponent<ParticleSystem>();
        }
    }

    public void WeaponUpdate()
    {
        if (type == FireType.SINGLE)
        {
            if (Input.GetMouseButtonDown(0) && currentAmmo > 0)
            {
                Fire();
                currentAmmo--;
                OnWeaponFired(currentAmmo);
            }
            else if (Input.GetMouseButtonDown(0) && currentAmmo <= 0)
            {
                //ammo runs out
                AudioPlayer.Instance.PlaySFX(emptySfx, player.transform);
                AudioPlayer.Instance.PlaySFX(reloadWarningSfx, player.transform);
            }
        }
        else
        {
            if (Input.GetMouseButton(0) && Time.time > nextFireTime && currentAmmo > 0)
            {
                Fire();
                currentAmmo--;
                OnWeaponFired(currentAmmo);
                nextFireTime = Time.time + rate;
            }
            else if (Input.GetMouseButton(0) && Time.time > nextFireTime && currentAmmo > 0)
            {
              
                AudioPlayer.Instance.PlaySFX(emptySfx, player.transform);
                AudioPlayer.Instance.PlaySFX(reloadWarningSfx, player.transform);
            }
        }

        if (defaultWeapon && Input.GetMouseButtonDown(1))
        {
            currentAmmo = maxAmmo;
            AudioPlayer.Instance.PlaySFX(reloadSfx, player.transform);
            OnWeaponFired(currentAmmo);
        }

        if (!defaultWeapon && currentAmmo <= 0)
        {
            //switch back to default the weapon
            player.SwitchWeapon();
        }
    }

    private void Fire()
    {
        AudioPlayer.Instance.PlaySFX(gunShotSfx, player.transform);
        GameManager.Instance.ShotsFired();

        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (cachedFX != null) 
        { 
        Vector3 muzzlePos = cam.ScreenToViewportPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.2f));
            cachedFX.transform.position = muzzlePos;
            cachedFX.transform.rotation = Quaternion.LookRotation(ray.direction);
            cachedFX.Play();
        }
        if (Physics.Raycast(ray, out hit, 50f))
        {
            if (hit.collider != null)
            {
                IHitable[] hitables = hit.collider.GetComponents<IHitable>();

                if (hitables != null && hitables.Length > 0)
                {
                    foreach (var hitable in hitables)
                    {
                        hitable.Hit(hit, damageValue);

                        if (hitable is EnemyScript || hitable is WeaponPickUp)
                        {
                            GameManager.Instance.ShotHit();
                            

                        }
                        
                    }
                }

                Debug.Log(hit.collider.gameObject.name);
            }
            
        }
        
    }
}

public enum FireType
{
    SINGLE,
    RAPID,
    Blast
}
