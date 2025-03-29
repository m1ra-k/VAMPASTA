using UnityEngine;

public class FloatEffect : MonoBehaviour
{
    [SerializeField]
    private float speed = 30f;
    private float amplitudeUp = 5f;
    private float amplitudeDown = 10f;

    private float startY;

    void Start()
    {
        startY = transform.position.y;
    }

    void Update()
    {
        float offset = Mathf.PingPong(Time.time * speed, amplitudeUp + amplitudeDown) - amplitudeDown;
        transform.position = new Vector3(transform.position.x, startY + offset, transform.position.z);
    }
}
