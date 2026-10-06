using UnityEngine;
using System.Collections;
public class Gun : MonoBehaviour
{

    public float reloadTime =1f;
    public float fireRate = 0.15f;
    public int magSize = 20;
    public GameObject bullet;
    public Transform bulletSpawnpoint;
    public GameObject droppedweapon;
    public GameObject weaponFlash;
    public float recoilDistance = 0.1f;
    public float recoilSpeed = 15f;
    private int currentAmmo;
    private bool isReloading =false;
    private float nextTimeToFire =0f;

    public AudioClip shootSound;

    private Quaternion initialRotation;
    private Vector3 initalPosition;
    private Vector3 reloadRotationOffset= new Vector3(66,50,50);
    void Start()
    {
        currentAmmo =magSize;
        initialRotation= transform.localRotation;
        initalPosition = transform.localPosition;
        UIManager.Instance.ammoText.text= currentAmmo.ToString() + "/" + magSize.ToString();
        
    }
     public void Shoot()
    {
         if(isReloading) return ;
         if (Time.time < nextTimeToFire) return;
         if(currentAmmo <=0)
        {
            StartCoroutine(Reload());
            return;

        }
        nextTimeToFire = Time.time + fireRate;
        currentAmmo--;
        UIManager.Instance.ammoText.text= currentAmmo.ToString() + "/" + magSize.ToString();
        AudioManager.Instance.PlaySFX(shootSound, 0.25f);
        Quaternion adjustRotation = bulletSpawnpoint.rotation * Quaternion.Euler(-6f, -3f, 0f);

        Instantiate(bullet, bulletSpawnpoint.position, adjustRotation);
        Instantiate(weaponFlash, bulletSpawnpoint.position, bulletSpawnpoint.rotation);
        StopCoroutine((Recoil()));
        StartCoroutine((Recoil()));
    }

    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");
        Quaternion targetRotation = Quaternion.Euler(initialRotation.eulerAngles + reloadRotationOffset);
        float halfReload = reloadTime/2f;
        float t =0f;

        while(t < halfReload)
        {
            t += Time.deltaTime;
            float lerpFactor = t/halfReload;
            transform.localRotation = Quaternion.Slerp(initialRotation, targetRotation, lerpFactor);
            yield return null;
        }
        t=0f;
        while(t < halfReload)
        {
            t += Time.deltaTime;
            float lerpFactor = t/halfReload;
            transform.localRotation = Quaternion.Slerp(targetRotation, initialRotation, lerpFactor);
            yield return null;
        }

        currentAmmo = magSize;
        UIManager.Instance.ammoText.text= currentAmmo.ToString() + "/" + magSize.ToString();
        isReloading = false;

    }

    public void TryReload()
    {
        if(isReloading) return;
        if(currentAmmo == magSize) return;
        StartCoroutine(Reload());
    }
    private IEnumerator Recoil()
    {
        Vector3 recoilTarget = initalPosition + new Vector3(recoilDistance, 0, 0);
        float t = 0f;

        while(t< 1f)
        {
            t += Time.deltaTime * recoilSpeed;
            transform.localPosition = Vector3.Lerp(initalPosition, recoilTarget, t);
            yield return null;
        }

        t = 0f;

        while(t< 1f)
        {
            t += Time.deltaTime * recoilSpeed;
            transform.localPosition = Vector3.Lerp(recoilTarget, initalPosition, t);
            yield return null;
        }

        transform.localPosition =initalPosition;
    } 

    public void Drop()
    {
        UIManager.Instance.ammoText.text= "";
        Instantiate(droppedweapon, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
