using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    public float speed;

    /// <summary>
    /// Keeps moving the fireball to the left
    /// </summary>
    private void Update()
    {
        transform.position -= transform.right * Time.deltaTime * speed;
    }
}