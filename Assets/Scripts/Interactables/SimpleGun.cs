
using UnityEngine;
using UnityEngine.UI;
using Valve.VR.InteractionSystem;

public class SimpleGun : MonoBehaviour
{
    // Start is called before the first frame update

    public float range = 100f;
    public float damage = 20f;
    public float fireRate = 1f;
    public float impactForce = 20f;

    
    
    private NetworkedGun networkedGunInfo;
    [SerializeField]
    private bool AddBulletSpread = true;
    [SerializeField]
    private Vector3 BulletSpread = new Vector3(0f, 0f, 0f);
    [SerializeField]
    private ParticleSystem ShootEffect;
    [SerializeField]
    private ParticleSystem HitEffect;
    [SerializeField]
    private Transform FirePoint;
    [SerializeField]
    private TrailRenderer Tracer;
    [SerializeField]
    private float timeToNext = 0f;
    [SerializeField]
    private LayerMask Mask;

    public bool isFlagging;


    private void Start()
    {
        networkedGunInfo = GetComponent<NetworkedGun>();
    }

    public void Update()
    {
        isFlagging = isFlaggingFriendly();
    }

    public void Shoot()
    {
        if (Time.time >= timeToNext)
        {
            ShootEffect.Play();
            timeToNext = Time.time + 1f / fireRate;
            Vector3 direction = GetDirection();
            Debug.DrawRay(FirePoint.position, direction * 100f, Color.red, 0);

            if (Physics.Raycast(FirePoint.position, direction, out RaycastHit hit, float.MaxValue, Mask))
            {
                Debug.DrawLine(FirePoint.position, hit.transform.position, Color.red);
                //Debug.Log("Hit:" + hit.transform.name);

                if (hit.transform.gameObject.GetComponent<SuppressionSphere>())
                {
                    hit.transform.gameObject.GetComponent<SuppressionSphere>().HitByRay(FirePoint.position, hit);
                }
                    


                //Instantiate a HitEffect facing the user.
                Instantiate(HitEffect, hit.point, Quaternion.LookRotation(hit.normal));

                //updating target trackers
                TrackedShootingTarget target = hit.transform.GetComponent<TrackedShootingTarget>();
                if(target)
                {
                    target.HitTarget();
                }

                //doing damage
                var hitBox = hit.transform.GetComponent<Hitbox>();
                //raycast only happens on server
                if(hitBox && ServerClientFlag.Instance.isServer)
                {
                    hitBox.OnRaycastHit(this);
                }
            }
        }
    }


    private Vector3 GetDirection()
    {
        Vector3 direction = FirePoint.transform.forward;

        if (AddBulletSpread)
        {
            direction += new Vector3(
                Random.Range(-BulletSpread.x, BulletSpread.x),
                Random.Range(-BulletSpread.y, BulletSpread.y),
                Random.Range(-BulletSpread.z, BulletSpread.z)
             );
            direction.Normalize();
        }
        return direction;
    }

    public bool isFlaggingFriendly()
    {

        Vector3 direction = GetDirection();

        // Debug.DrawRay(FirePoint.position, direction * 100f, Color.red, 0);

        // if raycast is on friendly, mark as flagging
        if (Physics.Raycast(FirePoint.position, direction, out RaycastHit hit, float.MaxValue))
        {
            // check for suppression sphere
            if (hit.transform.gameObject.tag == "SuppressionSphere")
            {
                // get the aiagent component in grandparent without nulling
                if (hit.transform.parent != null)
                {
                    if (hit.transform.parent.parent != null)
                    {
                        if (hit.transform.parent.parent.GetComponent<AiAgent>() && hit.transform.parent.parent.GetComponent<AiAgent>().friendly)
                        {
                            return true;
                        }
                    }
                }


            }
        }

        return false;
    }

}
