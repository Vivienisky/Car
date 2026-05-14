using UnityEngine;

public class RoadSteeringHandler : MonoBehaviour
{
    public float SideSpeed = 15f;
    public float MaxOffset = 7f;
    public float Smoothness = 10f;

    private float _targetX = 0f;

    private void Update()
    {
        HandleInput();
        ApplyMovement();
    }

    private void HandleInput()
    {
        float input = Input.GetAxis("Horizontal");
        
       
        _targetX -= input * SideSpeed * Time.deltaTime;

      
        _targetX = Mathf.Clamp(_targetX, -MaxOffset, MaxOffset);
    }

    private void ApplyMovement()
    {
        
        Vector3 newPosition = transform.position;
        newPosition.x = Mathf.Lerp(newPosition.x, _targetX, Time.deltaTime * Smoothness);
        transform.position = newPosition;
    }
}