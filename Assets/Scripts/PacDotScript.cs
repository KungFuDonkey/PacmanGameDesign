using UnityEngine;

public class PacDotScript : MonoBehaviour
{
    private Vector3[] locations = {
        new Vector3(22, 11, 0),
        new Vector3(10, 11, 0),
        new Vector3(19, 23, 0),
        new Vector3(10, 23, 0),
        new Vector3(10, 11, 0),
        new Vector3(19, 11, 0)
    };
    void OnTriggerEnter2D(Collider2D co)
    {
        if (co.name == "PacMan")
        {
            
            if(Random.Range(0,1) == 0){ 
                Destroy(gameObject);
            }
            else{
                int randomIndex = Random.Range(0,6);
                Vector3 newPosition = locations[randomIndex];
                transform.position = newPosition;
            }

        }
    
    }
}
