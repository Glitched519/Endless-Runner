using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour {

    int playerMask;
    Vector3[] collisionSphereSlidePositions;
    GameObject player;
    CollisionSphere[] CollisionSpheres;
    CollisionSphere feet, head;
    SkinnedMeshRenderer renderer;
    bool invincible = false;
    [SerializeField]
    float blinkRate = 0.1f;
    [SerializeField] 
    float blinkTime = 1f;
    Animator playerAnim;
    int slideParam;

    bool onRamp = false;
    public delegate void obstacleCollisionHandler();
    public event obstacleCollisionHandler onObstacleCollision;


    public class CollisionSphere
    {
        public Vector3 offset;
        public float radius;
        public CollisionSphere(string name, Vector3 offset, float radius)
        {
            this.offset = offset;
            this.radius = radius;


        }

        public static bool operator >(CollisionSphere LHS, CollisionSphere RHS)
        {
            return LHS.offset.y > RHS.offset.y;
        }

        public static bool operator <(CollisionSphere LHS, CollisionSphere RHS)
        {
            return LHS.offset.y < RHS.offset.y;
        }

        public class CollisionSphereComparer : IComparer
        {
            public int Compare(object a, object b)
            {
                if (!(a is CollisionSphere) || !(b is CollisionSphere))
                {
                    Debug.LogError(Environment.StackTrace);
                    throw new ArgumentException("Can't compare collision spheres to non-collision spheres.");

                }
                CollisionSphere LHS = (CollisionSphere)a;
                CollisionSphere RHS = (CollisionSphere)b;
                if (LHS.offset.y < RHS.offset.y)
                    return -1;
                else if (LHS.offset.y > RHS.offset.y)
                    return 1;
                else
                    return 0;
            }
        }
    }

    public void obstacleCollision()
    {
        if (!invincible)
        {
            invincible = true;
            StartCoroutine(BlinkPlayer());
        }
    }

    IEnumerator BlinkPlayer() {
        float startTime = Time.time;
        while (true){
            renderer.enabled = !renderer.enabled;
            if (Time.time >= startTime + blinkTime)
            {
                renderer.enabled = true;
                invincible = false;
                break;
            }
            yield return new WaitForSeconds(blinkRate);
        }
    }

    public int getLayerMask(params int[] Indices)
    {
        int mask = 0;
        for (int i = 0; i < Indices.Length; i++)
        {
            mask |= 1 << Indices[i];
        }
        return mask;

    }

    public int getLayerIgnoreMask(params int[] Indices)
    {
        return ~getLayerMask(Indices);

    }

    public void addLayers(ref int mask, params int[] Indices)
    {
        mask |= getLayerMask(Indices);

    }
    public void removeLayers(ref int mask, params int[] Indices) {
        mask &= ~getLayerMask(Indices);
    }

   

   


    // Use this for initialization
    void Start()
    {
        onObstacleCollision += obstacleCollision;
        player = GameObject.Find("Robot");
        if (!player) {
            Debug.Log("Player not detected.");
            Destroy(this);
        }
        playerAnim = player.GetComponent<Animator>();
        if (!playerAnim)
        {
            Debug.Log("Animator not responding.");
            Destroy(this);
        }
        slideParam = Animator.StringToHash("SlideCurve");
        renderer = player.GetComponentInChildren<SkinnedMeshRenderer>();
        if (!renderer)
        {
            Debug.Log("Not rendering.");
            Destroy(this);
        }
        playerMask =  getLayerMask((int)Layer.Obstacle);
        SphereCollider[] Colliders = player.GetComponents<SphereCollider>();
        CollisionSpheres = new CollisionSphere[Colliders.Length];
        for (int i = 0; i < Colliders.Length; i++)
        {
            CollisionSpheres[i] = new CollisionSphere("Crash!", Colliders[i].center, Colliders[i].radius);
        }
        Array.Sort(CollisionSpheres, new CollisionSphere.CollisionSphereComparer());
        feet = CollisionSpheres[0];
        head = CollisionSpheres[CollisionSpheres.Length - 1];
        collisionSphereSlidePositions = new Vector3[4];
        collisionSphereSlidePositions[0] = new Vector3(0.3f, 0.88f, -0.23f);
        collisionSphereSlidePositions[1] = new Vector3(-0.14f, 0.46f, 0.15f);
        collisionSphereSlidePositions[2] = new Vector3(0.01f, 0.39f, 0.56f);
        collisionSphereSlidePositions[3] = new Vector3(0.05f, 0.68f, -0.03f);

        
	}

    // Update is called once per frame
    void LateUpdate()
    {
        List<Collider> collisions = new List<Collider>();
        for(int i = 0; i < CollisionSpheres.Length; i++)
        {
            Vector3 slideDisplacement = collisionSphereSlidePositions[i]-CollisionSpheres[i].offset;
            slideDisplacement *= playerAnim.GetFloat(slideParam);
            Vector3 offset = CollisionSpheres[i].offset + slideDisplacement;
            foreach(Collider c in Physics.OverlapSphere(player.transform.position + offset, CollisionSpheres[i].radius, playerMask))
            {
                collisions.Add(c);
            }
        }

        if (collisions.Count > 0)
        {
            if (collisions[0].gameObject.name.Contains("ramp"))
            {
                if (collisions[0].gameObject.transform.position.z > -2.5)
                {
                    float diff = Mathf.Abs(collisions[0].gameObject.transform.position.z) / 2.5f;

                    player.transform.SetPositionAndRotation(new Vector3(player.transform.position.x, 1.8f * diff, player.transform.position.z), player.transform.rotation);
                    onRamp = true;
                }
            }
            else
            {
                onObstacleCollision();
            }
        }
        else
        {
            if (onRamp)
            {
                if (player.transform.position.y < 9.8f * Time.deltaTime)
                {
                    onRamp = false;
                    player.transform.SetPositionAndRotation(new Vector3(player.transform.position.x, 0, player.transform.position.z), player.transform.rotation);
                }
                else
                {
                    player.transform.Translate(0, -9.8f * Time.deltaTime, 0);
                }
                
                
            }
        }
	}
}
