using UnityEngine;

public class CameraShake : MonoBehaviour {

    Vector3 originPosition;
    Quaternion originRotation;
    [SerializeField]
    float shakeIntensity = 0.001f;
    int counter = 0;

	// Use this for initialization
	void Start () {
        originPosition = transform.position;
        originRotation = transform.rotation;
	}

    // Update is called once per frame
    void Update() {
        if(counter == 4)
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
