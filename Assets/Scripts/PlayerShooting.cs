using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerShooting : MonoBehaviour
{
    public Gun gun; 
    public Transform gunHolder;
    public Camera playerCamera;
    private bool isHoldingShoot = false;

    void OnShoot()
    {
        isHoldingShoot =true;
    }

    void OnShootRelease()
    {
        isHoldingShoot = false;
    }

    void OnReleaseShoot()
    {
        isHoldingShoot = false;
    }

    void OnReload()
    {
       if(gun != null)
        {
            gun.TryReload();
        }
    }

    void OnPickUp()
    {
        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>();
        }

        if (playerCamera != null && Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, 3f))
        {
            PickUp pickup = hit.collider.GetComponentInParent<PickUp>();
            if (pickup != null)
            {
                pickup.OnPickUp();
            }
        }
    }

    void Update()
    {
        if(isHoldingShoot && gun != null)
        {
            gun.Shoot();
        }
    }

    public void OnDrop()
    {
        if (gun != null)
        {
            gun.Drop();
            gun = null;
        }
    }
}
