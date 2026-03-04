using UnityEngine;

public class Hachisha_AudioManager : MonoBehaviour
{
    public AudioSource poPoPoSource;
    public Transform player;
    public float maxDistance = 30f;
    public float minDistance = 5f;

    void Start()
    {
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;
        
        if (poPoPoSource != null)
        {
            poPoPoSource.loop = true;
            poPoPoSource.spatialBlend = 1.0f; // 3D Spatial Sound
            poPoPoSource.rolloffMode = AudioRolloffMode.Logarithmic;
            poPoPoSource.maxDistance = maxDistance;
            poPoPoSource.minDistance = minDistance;
            poPoPoSource.Play();
        }
    }

    void Update()
    {
        if (player == null || poPoPoSource == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        
        // Dynamic volume or pitch modulation could be added here
        // For example, increasing pitch as she gets closer
        if (distance < maxDistance)
        {
            poPoPoSource.pitch = Mathf.Lerp(1.2f, 0.8f, distance / maxDistance);
        }
    }
}
