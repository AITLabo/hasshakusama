using UnityEngine;

public class Hachisha_SafeZone : MonoBehaviour
{
    public Hachisha_StalkingAI hachishaAI;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (hachishaAI != null) hachishaAI.SetSafeStatus(true);
            Debug.Log("Entered Safe Zone");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (hachishaAI != null) hachishaAI.SetSafeStatus(false);
            Debug.Log("Exited Safe Zone");
        }
    }
}
