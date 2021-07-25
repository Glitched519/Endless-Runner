using UnityEngine;

public class ObstacleSpawner : MonoBehaviour {
    GameObject[] loadedObstacles;
    // unused distance from previous place
    float leftover = 0;

    // Inspector Parameters
    [SerializeField]
    float distInterval = 30f;

	// Use this for initialization
	void Start () {
       
        loadedObstacles = Resources.LoadAll<GameObject>("Obstacles");
        RoadManager.Instance.OnAddPiece += PlaceObstacles;
	}
	
	// Update is called once per frame
	void PlaceObstacles (GameObject Piece) {
        Transform BeginLeft = Piece.transform.Find("BeginLeft");
        Transform BeginRight = Piece.transform.Find("BeginRight");
        Transform EndLeft = Piece.transform.Find("EndLeft");
        Transform EndRight = Piece.transform.Find("EndRight");

        // Get new piece length
        float length;
        Vector3 RotationPoint = Vector3.zero;
        float radius = 0;

        if (Piece.tag == Tags.straightPiece)
        {
            length = Vector3.Distance(BeginLeft.position, BeginRight.position);
        }
        else { 
            // Get radius
            RotationPoint = RoadManager.Instance.GetRotationPoint(BeginLeft, BeginRight, EndLeft, EndRight);
            radius = Vector3.Distance(Piece.transform.position, RotationPoint);

            // Get angle
            float angle = Vector3.Angle(BeginLeft.position - BeginRight.position, EndLeft.position - EndRight.position);

            length = radius * angle * Mathf.Deg2Rad;
        }

        float halflength = length / 2f;
        float curdist = distInterval - halflength - leftover;
        if (curdist >= halflength)
        {
            leftover += length;
        }

        for(; curdist < halflength; curdist += distInterval)
        {
            //Obstacle container
            GameObject ObstacleRow = new GameObject("ObstacleRow");
            ObstacleRow.transform.position = Piece.transform.position;
            ObstacleRow.transform.rotation = Piece.transform.rotation;
            ObstacleRow.transform.Rotate(90f, 0, 0); // compensate for road piece rotation
            ObstacleRow.transform.parent = Piece.transform;
            bool consec = false;
            int prevIndex = -1;
            for (int i = PlayerController.Instance.numLanes / -2; i <= PlayerController.Instance.numLanes / 2; i++)
            {
                int randomObstacle = Random.Range(0, 4);
                if (randomObstacle == prevIndex)
                {
                    if (!consec)
                    {
                        consec = true;
                    }
                    else
                    {
                        randomObstacle = (randomObstacle+1) % loadedObstacles.Length;
                    }

                }
                else
                {
                    consec = false;
                }
                prevIndex = randomObstacle;
                // Instantiate obstacle prefab

                GameObject Obstacle = Instantiate<GameObject>(loadedObstacles[randomObstacle], ObstacleRow.transform.position, ObstacleRow.transform.rotation, ObstacleRow.transform);
                Obstacle.transform.Translate(Vector3.right * i * PlayerController.Instance.laneWidth, Space.Self);
                if (Obstacle.name.Contains("ramp"))
                {
                    Obstacle.transform.Rotate(90, 0, 90);
                    Obstacle.transform.Translate(0, 0.9f, 0, Space.World);
                }
            }
            if (Piece.tag == Tags.straightPiece)
            {
                ObstacleRow.transform.Translate(0, 0, curdist);
            }
            else
            {
                float angle = curdist / radius;
                ObstacleRow.transform.RotateAround(RotationPoint, Vector3.up, angle * Mathf.Rad2Deg * -Mathf.Sign(Piece.transform.localScale.x));
            }
        }
	}

    public override string ToString()
    {
        return base.ToString();
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override bool Equals(object other)
    {
        return base.Equals(other);
    }
}
