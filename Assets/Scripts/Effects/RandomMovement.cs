using UnityEngine;

public class RandomMovement : MonoBehaviour
{
    [SerializeField] private float radius = 0.5f;        // how far it can drift
    [SerializeField] private float speed = 1.5f;        // movement speed
    [SerializeField] private float noiseScale = 1f;     // smooth randomness

    private Vector2 origin;
    private float seedX;
    private float seedY;

    private void Start()
    {
        origin = transform.position;

        seedX = Random.Range(0f, 100f);
        seedY = Random.Range(0f, 100f);
    }

    private void Update()
    {
        float time = Time.time * noiseScale;

        // Smooth random movement using Perlin noise
        float x = Mathf.PerlinNoise(seedX, time) * 2f - 1f;
        float y = Mathf.PerlinNoise(seedY, time) * 2f - 1f;

        Vector2 offset = new Vector2(x, y) * radius;

        // Smoothly move toward target offset
        Vector2 targetPos = origin + offset;

        transform.position = Vector2.Lerp(
            transform.position,
            targetPos,
            speed * Time.deltaTime
        );
    }
}
