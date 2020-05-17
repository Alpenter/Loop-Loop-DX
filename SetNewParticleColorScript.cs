using UnityEngine;

public class SetNewParticleColorScript : MonoBehaviour
{

    public enum ColorToMatch
    {
        Front,
        Bullet,
        Back,
    }
    public ColorToMatch colorType;

    public enum ApplyType
    {
        OnAwake,
        OnStart,
        OnUpdate,
    }
    public ApplyType apply;
    Color targetColor;

    private void Awake()
    {
        if (apply == ApplyType.OnAwake)
        {
            Apply();
        }
    }

    private void Start()
    {
        if (apply == ApplyType.OnStart)
        {
            Apply();
        }
    }

    private void Update()
    {
        if (apply == ApplyType.OnUpdate)
        {
            Apply();
        }
    }

    public void Apply()
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();
        var main = ps.main;
        if (colorType == ColorToMatch.Front)
        {
            targetColor = Game.frontColor;
        }
        else if (colorType == ColorToMatch.Bullet)
        {
            targetColor = Game.bulletColor;
        }
        else if (colorType == ColorToMatch.Back)
        {
            targetColor = Game.backColor;
        }
        main.startColor = targetColor;
    }
}
