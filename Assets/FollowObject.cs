using UnityEngine;

public class FollowObject : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeReference] GameObject TargetObject;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetCamPos = new Vector3(TargetObject.transform.position.x, TargetObject.transform.position.y, -100);
        transform.position = Vector3.Lerp(transform.position, targetCamPos, Time.deltaTime * 3f);
    }
}
