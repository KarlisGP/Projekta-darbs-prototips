using UnityEngine;

public class WaterManager : MonoBehaviour
{
    public float riseSpeed = 0.5f;
    public static float WaterY;

    void Update()
    {
        transform.position += Vector3.up * riseSpeed * Time.deltaTime;
        WaterY = transform.position.y;
    }
}
