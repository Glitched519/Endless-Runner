using System.Collections;
using UnityEngine;

[RequireComponent (typeof(Animator))]
public class PlayerController : Singleton<PlayerController>
{
    // Input variables
    float hPrev = 0f;
    int dirBuffer = 0;
    [SerializeField]
    float gravity = -9.8f;
    [SerializeField]
    float vi = 5f;
    Animator anim;
    int JumpParam;
    int SlideParam;
    

    // Lane variables
    int currentLane = 0;
    int prevLane = 0;
    public float laneWidth;
    Coroutine currentLaneChange;
    Coroutine currentJump;
    int laneChangeStack = 0;

    // Inspector parameters
    public int numLanes = 3;

    [SerializeField]
    float strafeSpeed = 5f; // 1/strafeSpeed = amount of time for lane change (seconds) 

    public void Reset()
    {
        gameObject.SetActive(true);
        currentLane = 0;
        transform.position = Vector3.zero;
        anim.SetBool(JumpParam, false);
        if (currentJump != null)
        {

            StopCoroutine(currentJump);
            currentJump = null; 
        }
        currentLaneChange = null; 

    }

    private void OnDisable()
    {
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        currentJump = null;
        currentLaneChange = null;

    }

    // Use this for initialization
    void Awake()
    {
        transform.position = Vector3.zero; // middle lane is always at origin
        laneWidth = 7.5f / numLanes;
        anim = GetComponent<Animator>();
        JumpParam = Animator.StringToHash("Jump");
        SlideParam = Animator.StringToHash("Slide");
    }

    // Update is called once per frame
    void Update()
    {
        float hNew = Input.GetAxisRaw(InputNames.horizontalAxis); // returns -1, 0 or 1 with no smoothing
        float hDelta = hNew - hPrev;

        if (Mathf.Abs(hDelta) > 0f && Mathf.Abs(hNew) > 0f)
        {
            MovePlayer((int)hNew);
        }

        hPrev = hNew;

        // Jumping
        if (Input.GetButtonDown(InputNames.jumpButton))
        {
            if (currentJump == null)
            {

                currentJump = StartCoroutine(Jump());
            }
        }

        // Sliding
        if (Input.GetButtonDown(InputNames.slideButton))
        {
            anim.SetTrigger(SlideParam);
        }
	}

    void MovePlayer(int dir)
    {
        if (currentLaneChange != null)
        {
            if (currentLane + dir != prevLane)
            {
                dirBuffer = dir;
                return;
            }

            // override previous movement
            StopCoroutine(currentLaneChange);
            dirBuffer = 0;
        }

        prevLane = currentLane;
        currentLane = Mathf.Clamp(currentLane + dir, numLanes / -2, numLanes / 2);

        currentLaneChange = StartCoroutine(LaneChange());
    }

    IEnumerator Jump()
    {
        float timeFinal = 2 * vi / -gravity;
        anim.SetBool(JumpParam, true);

        for (float t = Time.deltaTime; t < timeFinal; t += Time.deltaTime)
        {
            float y = (gravity * t * t / 2) + (vi * t);
            Helpers.SetPositionY(transform, y);
            yield return null;
        }
        Helpers.SetPositionY(transform, 0);
        anim.SetBool(JumpParam, false);
        currentJump = null;
    }

    // Strafe movement coroutine
    IEnumerator LaneChange()
    {
        Vector3 From = Vector3.right * prevLane * laneWidth;
        Vector3 To = Vector3.right * currentLane * laneWidth;

        float t = (laneWidth - Vector3.Distance(transform.position.x*Vector3.right, To)) / laneWidth;
        for (; t < 1f; t += strafeSpeed * Time.deltaTime / laneWidth)
        {
            transform.position = Vector3.Lerp(From+ Vector3.up*transform.position.y, To+Vector3.up*transform.position.y, t);

            yield return null;
        }

        transform.position = To + Vector3.up * transform.position.y ;
        currentLaneChange = null;

        if (dirBuffer != 0 && ++laneChangeStack < 2)
        {
            MovePlayer(dirBuffer);
            dirBuffer = 0;
        }

        laneChangeStack = 0;
    }
} 
