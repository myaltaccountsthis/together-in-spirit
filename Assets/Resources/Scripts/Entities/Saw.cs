using UnityEngine;
public class Saw : Killbrick
{
    public Transform leftPos;
    public Transform rightPos;
    public bool startRight = false;
    public float oscillationTime;
    public float rotationSize;
    
    protected override void Awake()
    {
        base.Awake();
        transform.position = startRight ? rightPos.position : leftPos.position;
        LeanTween
            .move(gameObject, startRight ? leftPos : rightPos, oscillationTime)
            .setEaseInOutSine()
            .setLoopPingPong();

        LeanTween.rotate(gameObject, rotationSize * (startRight ? Vector3.back : Vector3.forward), oscillationTime)
            .setEaseInOutSine()
            .setLoopPingPong();
    }
}