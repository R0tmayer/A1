using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RobberController : MonoBehaviour
{

    #region State
    public StateMachine stateMachine;

    public IdleRobberyState idle;
    public RobberyState robbery;
    public MovingState moving;
    public EscapingState escaping;
    #endregion

    #region Variables

    [SerializeField] MeshRenderer[] meshRenderers;
    public NavMeshAgent navMeshAgent;
    private Transform[] targetMarkers;

    [SerializeField] private float fadeSpeed=0.4f;
    private bool isFadingOut;
    private Color objectColor;

    Coroutine varibleForStopingCouratine;

    public RoberryPathFinder roberryPathFinder;

    public float factorPropertyperSecodn = 10;
    #endregion

    #region Properties

    #endregion

    #region Methods

    private IEnumerator MoveThroughTargets(Transform[] targets)
    {
        yield return null;
    }

  
    public void Escape()
    {
        //TODO: освобождение таргета (дома для кражи)
        StartCoroutine(FadeOutObjects());
    }

    public void StartCoroutineTMP(IEnumerator cur) {
        varibleForStopingCouratine = StartCoroutine(cur);
    }

    public void StopCoroutineTMP() {
        StopCoroutine(varibleForStopingCouratine);
    }

    private IEnumerator FadeOutObjects()
    {
        while (objectColor.a > 0)
        {
            float fadeAmount = objectColor.a - (fadeSpeed * Time.deltaTime);
            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            foreach (var meshRenderer in meshRenderers)
            {
                meshRenderer.material.SetColor("_Color", objectColor);
                meshRenderer.material.SetFloat("_Mode", 3);
                meshRenderer.material.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.SrcAlpha);
                meshRenderer.material.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                meshRenderer.material.EnableKeyword("_ALPHABLEND_ON");
                meshRenderer.material.renderQueue = 3000;
            }
            yield return null;
        }
        TargetsManager.Instance.robberTargetPositions.Add(roberryPathFinder.movePositionHouse);
        TargetsManager.Instance.robbersInLevel.Remove(gameObject);

        Destroy(gameObject);
    }

    #endregion

    #region UnityCallbacks

    void Start()
    {
        stateMachine = new StateMachine();

        idle = new IdleRobberyState(this, stateMachine);
        robbery = new RobberyState(this, stateMachine);
        moving = new MovingState(this, stateMachine);
        escaping = new EscapingState(this, stateMachine);

        stateMachine.Initialize(idle);

        roberryPathFinder = GetComponent<RoberryPathFinder>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        //isFadingOut = true;
        //objectColor = GetComponent<Renderer>().material.color;
        objectColor = meshRenderers[0].GetComponent<Renderer>().material.color;
        //Escape();
    }


    private void FixedUpdate()
    {
        stateMachine.CurrentState.PhysicsUpdate();
    }

    void Update()
    {

        stateMachine.CurrentState.HandleInput();

        stateMachine.CurrentState.LogicUpdate();
        // if (isFadingOut)
        // {
        //     float fadeAmount = objectColor.a - (fadeSpeed * Time.deltaTime);
        //     objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
        //     foreach (var meshRenderer in meshRenderers)
        //     {
        //         meshRenderer.material.SetColor("_Color", objectColor);
        //         meshRenderer.material.SetFloat("_Mode", 3);
        //         meshRenderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        //         meshRenderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        //         meshRenderer.material.EnableKeyword("_ALPHABLEND_ON");
        //         meshRenderer.material.renderQueue = 3000;
        //     }
        //     if(objectColor.a <= 0)
        //         Destroy(gameObject);
        // }

        // if (isFadingOut)
        // {
        //     float fadeAmount = objectColor.a - (fadeSpeed * Time.deltaTime);
        //     objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
        //     mat.SetColor("_Color", objectColor);
        //     mat.SetFloat("_Mode", 3);
        //     mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        //     mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        //     mat.EnableKeyword("_ALPHABLEND_ON");
        //     mat.renderQueue = 3000;
        //     if(objectColor.a <= 0)
        //         Destroy(gameObject); 
        // }


        // if (isFadingOut)
        // {
        //     Color objectColor = GetComponent<Renderer>().material.color;
        //     float fadeAmount = objectColor.a - (fadeSpeed * Time.deltaTime);
        //
        //     objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
        //     GetComponent<Renderer>().material.color = objectColor;
        //
        //     if(objectColor.a <= 0)
        //         Destroy(gameObject); 
        // }
    }
    
    #endregion

}
