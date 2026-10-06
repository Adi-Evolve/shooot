using UnityEngine;
using UnityEngine.InputSystem;
public class PickUp : MonoBehaviour
{
    public Material highlightMaterial;
    private Material[] originalMaterial;
    private MeshRenderer[] meshRenderer;

    public GameObject weaponPrefab;
    public float lookRange =3f;
    private bool isLookedAt = false;
    private Camera playerCam;
    private PlayerShooting player;
    void Start()
    {
        meshRenderer = GetComponentsInChildren<MeshRenderer>();
        originalMaterial = new Material[meshRenderer.Length];
        for (int i = 0; i < meshRenderer.Length; i++)
        {
            originalMaterial[i] = meshRenderer[i].material;
        }
        player = FindAnyObjectByType<PlayerShooting>();
        playerCam = player.GetComponentInChildren<Camera>();
    }

    void Update()
    {
        Ray ray = new Ray(playerCam.transform.position, playerCam.transform.forward);
        if (Physics.Raycast(ray , out RaycastHit hit, lookRange))
        {
            if (hit.collider.GetComponentInParent<PickUp>()  == this)
            {
                if (!isLookedAt)
                {
                    SetLookedAt(true);
                }

                return;
            }
            
        }
        if (isLookedAt)
        {
            SetLookedAt(false);
        }
    }

    void SetLookedAt(bool lookedAt)
    {
        isLookedAt = lookedAt;
        if (isLookedAt)
        {
            foreach(MeshRenderer renderer in meshRenderer)
            {
                renderer.material = highlightMaterial;
            }
        }
        else
        {
            for( int i=0; i<meshRenderer.Length; i++)
            {
                meshRenderer[i].material = originalMaterial[i];
            }
        }
    }

    public void OnPickUp()
    {
        if (!isLookedAt) return;
        player.OnDrop();
        GameObject newWeapon = Instantiate(weaponPrefab, player.gunHolder);
        newWeapon.transform.localPosition = Vector3.zero;
        newWeapon.transform.localRotation = Quaternion.identity;
        player.gun = newWeapon.GetComponent<Gun>();
        Destroy(gameObject);
    }
}
