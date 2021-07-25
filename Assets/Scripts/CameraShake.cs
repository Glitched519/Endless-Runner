using UnityEngine;

public class CameraShake : MonoBehaviour {

    Vector3 originPosition;
    Quaternion originRotation;
    [SerializeField]
    public static float shakeIntensity = 0.001f;
    [SerializeField]
    public static int counter = 0;
    public static bool active;


	// Use this for initialization
	public void Start () {
        originPosition = transform.position;
        originRotation = transform.rotation;
	}

    //
    // Summary:
    //     Is the game running fullscreen?
    public static bool IsShaking { get; set; }

    // Update is called once per frame
    public void Update() {
        if (!active)
        {
            if (counter == 4)
            {
                counter = 0;
                transform.position = originPosition + (Vector3)Random.insideUnitCircle * shakeIntensity;
                transform.rotation = new Quaternion(originRotation.x + Random.Range(-shakeIntensity, shakeIntensity),
                    originRotation.y + Random.Range(-shakeIntensity, shakeIntensity),
        originRotation.z + Random.Range(-shakeIntensity, shakeIntensity),
        originRotation.w + Random.Range(-shakeIntensity, shakeIntensity));
                shakeIntensity += 0.001f * Time.deltaTime;
            }
            counter += 1;
        }
    }
}
