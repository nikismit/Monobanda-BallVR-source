using UnityEngine;

public class DemoUI : MonoBehaviour
{
    [SerializeField] private RaceCountdown countDown;
    [SerializeField] private TutorialHandler tutHandler;
    [SerializeField] private GameObject transitionUI;
    [SerializeField] private GameObject demoCanvas;
    [HideInInspector] private RectTransform uiTransform;
    [HideInInspector] private CanvasGroup uiFade;

    private WinState winState;
    private float scale = 0;
    private bool startTut = false;
    private bool WinTransition = false;

    void Start()
    {
        winState = GetComponent<WinState>();

        uiTransform = transitionUI.GetComponent<RectTransform>();
        uiFade = transitionUI.GetComponent<CanvasGroup>();

        uiTransform.sizeDelta = new Vector2(scale, scale);
    }

    public void RemoveDemoUIEvent()
    {
        startTut = true;
        transitionUI.SetActive(true);
    }    

    void Update()
    {
        if (uiFade.alpha <= 0)
        {
            startTut = false;
            WinTransition = true;
            transitionUI.SetActive(false);
            uiFade.alpha = 1;
            scale = 0;
            uiTransform.sizeDelta = new Vector2(scale, scale);
        }

        if (startTut && scale <= 50)
        {
            scale += 60 * Time.unscaledDeltaTime;
            uiTransform.sizeDelta = new Vector2(scale * 55, scale * 55);
            uiTransform.Rotate(Vector3.forward, scale * 5);
        }
        else if (scale >= 50 && startTut)
        {
            tutHandler.RemoveRoads();
            if (!WinTransition)
                demoCanvas.SetActive(false);
            else
                StartCoroutine(winState.ResetScene(6));

            uiFade.alpha -= Time.unscaledDeltaTime / 3;
        }
    }
}
