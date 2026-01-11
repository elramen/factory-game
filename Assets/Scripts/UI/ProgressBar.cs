using UnityEngine;
using System;
using System.Collections;
using TMPro;

public class ProgressBar : MonoBehaviour
{
    // References
    private Base mainBase;
    private GameObject container;
    private GameObject border;
    private GameObject background;
    private GameObject fillBar;
    private GameObject item;
    private GameObject text;


    // Visual attributes
    [SerializeField]
    private Color backgroundColor, borderColor, barColor;

    [SerializeField]
    [Range(2, 50)]
    private float width, height; 

    [SerializeField]
    private float borderWidth;

    [SerializeField]
    private AnimationCurve progressAnimation;


    // Variables?
    private float prevProgress;
    private int prevGoal;

    void Start()
    {
        /*  
        Layers:
        10 - Border
        11 - Background
        12 - FillBar
        13 - Text & Item
        */

        progressAnimation = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
        progressAnimation.preWrapMode = WrapMode.PingPong;
        progressAnimation.postWrapMode = WrapMode.PingPong;


        Data.Instance.Init();
        mainBase = GameObject.Find("Base").GetComponent<Base>();

        // Setup container
        transform.gameObject.layer = 5;
        transform.localScale = new Vector3(45f, 45f, 1f);

        RectTransform rt = gameObject.GetComponent<RectTransform>();

        rt.anchoredPosition = new Vector2(0, -200);

        // Setup border
        border = transform.Find("Border").gameObject;
        border.layer = 5;

        SpriteRenderer spr = border.GetComponent<SpriteRenderer>();
        spr.sortingOrder = 10;
        spr.drawMode = SpriteDrawMode.Sliced;
        spr.size = new Vector2(width + borderWidth, height + borderWidth);
        spr.color = borderColor;

        border.transform.localScale = Vector3.one;

        // Setup background
        background = transform.Find("Background").gameObject;
        background.layer = 5;

        spr = background.GetComponent<SpriteRenderer>();
        spr.sortingOrder = 11;
        spr.drawMode = SpriteDrawMode.Sliced;
        spr.size = new Vector2(width, height);
        spr.color = backgroundColor;

        background.transform.localScale = Vector3.one;

        // Setup fillbar
        fillBar = transform.Find("FillBar").gameObject;
        fillBar.layer = 5;

        spr = fillBar.GetComponent<SpriteRenderer>();
        spr.sortingOrder = 12;
        spr.drawMode = SpriteDrawMode.Sliced;
        spr.size = new Vector2(2f / width, height);
        spr.transform.position = new Vector3(
            -width,
            0f,
            0f
        );
        spr.color = barColor;

        fillBar.transform.localScale = Vector3.one;

        // Manipulate text
        text = transform.Find("GoalText").gameObject;
        text.transform.parent = transform;

        // Place it right
        RectTransform rect = text.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector3(
            -4f,
            0f,
            0f
        );

        rect.localScale = new Vector3(
            0.4f,
            0.4f,
            1f            
        );

        rect.sizeDelta = new Vector2(34, 10);

        // Text stuff
        text.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Right;


        UpdateText();
        UpdateGoal();
        prevGoal = Data.Instance.currentGoal;
    }

    void Update() {
        UpdateProgress();
    }

    private void UpdateProgress() {
        // fillBar måste vara minst 2f bred för att vara snygg
        // Därför fadeas fillbar in fram till att progress når ett värde som motsvarar 2f
        // Efter den punkten börjar fillbar växa horisontellt istället
        float progress = mainBase.GetProgress();

        if (progress == prevProgress) return; 

        if (progress == 0 && prevProgress != 0) {
            // Goal achieved
            StartCoroutine(GoalComplete());
        } else {
            StartCoroutine(MoveProgressMeter(progress));
        }

        // Oklart varför detta behövs, men nånting ändrar på scale annars
        fillBar.transform.localScale = Vector3.one;

        prevProgress = progress;
    }

    private IEnumerator MoveProgressMeter(float progress) {
        float movementTime = 0.5f;

        float shownProgress = Mathf.Clamp(
            progress,
            2f / width,
            1f
        );

        SpriteRenderer spr = fillBar.GetComponent<SpriteRenderer>();

        float prevAlpha = spr.color.a;
        float prevWidth = spr.size.x;

        float alphaChange = Mathf.Clamp(progress / (2f / width), 0f, 1f) - prevAlpha;
        float widthChange = shownProgress * width - prevWidth;


        float timeElapsed = 0f;
        while (timeElapsed < movementTime)
        {
            float eval = progressAnimation.Evaluate(timeElapsed / movementTime);

            spr.color = new Color (
                spr.color.r,
                spr.color.g,
                spr.color.b,
                Mathf.Clamp(prevAlpha + eval * alphaChange, 0f, 1f)
            );

            float newWidth = prevWidth + eval * widthChange;

            spr.size = new Vector2(newWidth, height);
            fillBar.transform.localPosition = new Vector3(
                (newWidth - width) / 2,
                0f,
                0f
            );

            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator GoalComplete() {
        // Disable goal symbol and text
        text.SetActive(false);
        transform.Find("WantedItem").gameObject.SetActive(false);
        StartCoroutine(FlashColor());
        // Move progress meter all the way
        yield return StartCoroutine(MoveProgressMeter(1));
        yield return StartCoroutine(MoveProgressMeter(0));
        
        text.SetActive(true);
        transform.Find("WantedItem").gameObject.SetActive(true);
    }

    private IEnumerator FlashColor() {
        float animationTime = 1f;

        SpriteRenderer spr = fillBar.GetComponent<SpriteRenderer>();
        
        Color startColor = new Color(
            spr.color.r,
            spr.color.g,
            spr.color.b,
            spr.color.a   
        );

        Color animationColor = new Color(0, 110, 110);

        float timeElapsed = 0f;

        while (timeElapsed < animationTime)
        {
            spr.color = 
                Color.Lerp(startColor, animationColor, progressAnimation.Evaluate(timeElapsed / animationTime));

            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        spr.color = startColor;

        yield return null;
    }

    public void UpdateGoal() {
        Debug.Log("Updating progressbar");
        Destroy(item);

        item = Instantiate(mainBase.goals[Math.Min(14, Data.Instance.currentGoal)].item.gameObject);
        item.name = "WantedItem";

        item.transform.parent = transform;
        item.transform.localPosition = new Vector3(5.5f, 0, 0);
        item.transform.localScale = new Vector3(2, 2, 1);

        item.layer = 5;
        item.GetComponent<SpriteRenderer>().sortingOrder = 13;

        item.SetActive(true);

        UpdateText();
    }

    private void UpdateText() {
        text.GetComponent<TextMeshProUGUI>().text = "Level " + Data.Instance.levelNr;
    }
}
