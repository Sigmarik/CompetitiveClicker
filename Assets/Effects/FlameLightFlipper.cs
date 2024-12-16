using UnityEngine;

public class FlameLightFlipper : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        light_ = GetComponent<Light>();
        intensity_ = light_.intensity;
    }

    float Noise()
    {
        return
            Mathf.Sin(transform.position.x * 2.0f * noiseScale + Time.time * 2.0f * animSpeed) / 2.0f +
            Mathf.Cos(transform.position.y * 3.0f * noiseScale + Time.time * animSpeed) / 2.0f;
    }

    // Update is called once per frame
    void Update()
    {
        light_.intensity = intensity_ * (1.0f + Noise() * amplitude);
    }

    private Light light_;

    private float intensity_;

    public float noiseScale = 10.0f;
    public float animSpeed = 10.0f;
    public float amplitude = 0.5f;
}
