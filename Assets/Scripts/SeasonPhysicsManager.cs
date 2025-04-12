using UnityEngine;

public enum Season
{
    Summer,
    Winter,
    Autumn
}

[CreateAssetMenu(fileName = "SeasonPhysics", menuName = "Game/Season Physics")]
public class SeasonPhysics : ScriptableObject
{
    public float gravity = 1.0f;
    public float moveSpeed = 2.0f;
    public float moveDistance = 3.0f;
    public float blockFriction = 0.4f;
    public float blockBounciness = 0.0f;
    public float windStrength = 0.0f;
    public float windFrequency = 1.0f;
    public bool hasRandomEvents = false;
}