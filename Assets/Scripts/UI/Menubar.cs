using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

class ImageComponent
{
    public GameObject gameObject;
    public Image image;
    public RectTransform rt;
    Vector2 position;
    Vector2 size;
    string sprite;
    string color;
    RectTransform parent;

    public void Create(
        Vector2 _position,
        Vector2 _size,
        string _sprite,
        string _color,
        RectTransform _parent
    )
    {
        this.gameObject = new GameObject("ImageComponent");
        this.image = gameObject.AddComponent<Image>();
        this.rt = gameObject.GetComponent<RectTransform>();

        this.position = _position;
        this.size = _size;
        this.sprite = _sprite;
        this.color = _color;
        this.parent = _parent;

        rt.SetParent(parent);

        gameObject.transform.localScale = Vector3.one;

        image.sprite = Data.Instance.sprites[sprite];
        image.color = Data.Instance.uiColors[color];
        image.raycastTarget = false;
        rt.anchorMin = new Vector2(0.5f, 0);
        rt.anchorMax = new Vector2(0.5f, 0);
        rt.anchoredPosition = this.position;
        rt.sizeDelta = new Vector2(size.x, size.y);
    }

    public void Resize(int newWidth, int newHeight)
    {
        size.x = newWidth;
        size.y = newHeight;
        rt.sizeDelta = new Vector2(size.x, size.y);
    }

    public void Move(Vector2 newPosition)
    {
        this.position = newPosition;
        rt.anchoredPosition = this.position;
    }
}

class ButtonComponent
{
    GameObject gameObject;
    Image image;
    Button button;
    RectTransform rt;
    Vector2 position;
    Vector2 size;
    string sprite;
    string color;
    RectTransform parent;
    UnityEngine.Events.UnityAction fn;

    public void Create(
        Vector2 _position,
        Vector2 _size,
        string _sprite,
        string _color,
        RectTransform _parent,
        UnityEngine.Events.UnityAction _fn
    )
    {
        this.gameObject = new GameObject("Button");
        this.image = gameObject.AddComponent<Image>();
        this.rt = gameObject.GetComponent<RectTransform>();
        this.button = gameObject.AddComponent<Button>();

        this.position = _position;
        this.size = _size;

        this.sprite = _sprite;
        this.color = _color;
        this.parent = _parent;
        this.fn = _fn;

        rt.SetParent(parent);

        gameObject.transform.localScale = Vector3.one;

        image.sprite = Data.Instance.sprites[sprite];
        image.color = Data.Instance.uiColors[color];

        rt.anchorMin = new Vector2(0.5f, 0);
        rt.anchorMax = new Vector2(0.5f, 0);
        rt.anchoredPosition = this.position;
        rt.sizeDelta = new Vector2(_size.x, _size.y);

        button.onClick.AddListener(fn);
        button.transition = Selectable.Transition.None;
    }
}

class MachineButtonComponent
{
    public GameObject gameObject;
    public Animator animator;
    public RectTransform rt;
    public Vector2 position;
    public Vector2 size;
    public Image buttonImage;
    public Image iconImage;
    public LongPressEventTrigger longPressEventTrigger;
    string sprite;
    string color;
    RectTransform parent;

    public void Create(
        Vector2 _position,
        Vector2 _size,
        string _sprite,
        string _color,
        RectTransform _parent
    )
    {
        this.gameObject = GameObject.Instantiate(Data.Instance.prefabs["Building Button"]);
        this.rt = gameObject.GetComponent<RectTransform>();
        this.animator = gameObject.transform.GetComponent<Animator>();
        Transform button = gameObject.transform.Find("Button");
        this.buttonImage = button.GetComponent<Image>();
        this.iconImage = button.Find("Icon").GetComponent<Image>();
        this.longPressEventTrigger = button.GetComponent<LongPressEventTrigger>();

        this.position = _position;
        this.size = _size;
        this.sprite = _sprite;
        this.color = _color;
        this.parent = _parent;

        rt.SetParent(parent);

        buttonImage.color = Data.Instance.uiColors[this.color];
        iconImage.sprite = Data.Instance.sprites[sprite];

        gameObject.transform.localScale = Vector3.one;

        rt.anchorMin = new Vector2(0.5f, 0);
        rt.anchorMax = new Vector2(0.5f, 0);
        rt.anchoredPosition = this.position;
        rt.sizeDelta = new Vector2(_size.x, _size.y);
    }
}

class SpacerComponent
{
    public GameObject gameObject;
    public Animator animator;
    public RectTransform rt;
    public RectTransform buttonRt;
    public Vector2 position;
    public Vector2 size;
    public Image buttonImage;
    string sprite;
    string color;
    RectTransform parent;

    public void Create(
        Vector2 _position,
        Vector2 _size,
        string _sprite,
        string _color,
        RectTransform _parent
    )
    {
        this.gameObject = GameObject.Instantiate(Data.Instance.prefabs["Building Button Spacer"]);
        this.animator = gameObject.transform.GetComponent<Animator>();
        this.rt = gameObject.GetComponent<RectTransform>();
        Transform button = gameObject.transform.Find("Button");
        this.buttonRt = button.GetComponent<RectTransform>();
        this.buttonImage = button.GetComponent<Image>();

        this.position = _position;
        this.size = _size;
        this.sprite = _sprite;
        this.color = _color;
        this.parent = _parent;

        rt.SetParent(parent);

        buttonImage.color = Data.Instance.uiColors[this.color];

        gameObject.transform.localScale = Vector3.one;

        rt.anchorMin = new Vector2(0.5f, 0);
        rt.anchorMax = new Vector2(0.5f, 0);
        rt.anchoredPosition = this.position;
        rt.sizeDelta = new Vector2(_size.x, _size.y);
        buttonRt.sizeDelta = new Vector2(_size.x, _size.y);
    }
}

public class Menubar : MonoBehaviour
{
    [Header("References")]
    public InputManager inputManager;
    public VisualGrid visualGrid;

    [Header("Variables")]
    public Vector2Int position;
    public Vector2Int selectedPosition;
    public Vector2Int deselectedPosition;
    public Vector2Int size;

    public int selectorWidth;
    public int iconWidth;
    public int buildingCount;
    public float animationTime;

    private float menuStartX;
    private float menuSpaceBetween;
    private float buildingStartX;
    public float buildingSpaceBetween;
    public float buildingSpaceOffset;

    public int[] spacers;
    public Vector2 spacerSize;
    public float spacerPadding;

    public string[] menuIcons;
    public string[] buildingIcons;

    public AnimationCurve movement;
    public AnimationCurve scale_x;
    public AnimationCurve scale_y;
    public AnimationCurve colorChange;

    private ImageComponent selector;

    private RectTransform mainGroup;
    private RectTransform buildingIconGroup;

    private List<ImageComponent> icons;
    private List<MachineButtonComponent> buildingButtons;
    private List<SpacerComponent> buildingSpacers;

    // Start is called before the first frame update
    void Start()
    {
        // Variables
        Data.Instance.Init();
        inputManager.mode = 0;
        CreateMainGroup();
        CreateBuilingIcons();
        CreateMenu();

        mainGroup.localPosition = new Vector3(mainGroup.localPosition.x, mainGroup.localPosition.y, 0);
    }

    void CreateMainGroup()
    {
        GameObject _mainGroup = new GameObject("Main");
        mainGroup = _mainGroup.AddComponent<RectTransform>();

        mainGroup.SetParent(transform);
        mainGroup.localScale = Vector3.one;
        mainGroup.position = Vector3.zero;
        mainGroup.anchoredPosition = Vector3.zero;

        mainGroup.anchorMin = new Vector2(0.5f, 0);
        mainGroup.anchorMax = new Vector2(0.5f, 0);

        mainGroup.sizeDelta = Vector2.zero;
    }

    void CreateBuilingIcons()
    {
        GameObject _buildingIconGroup = new GameObject("BuildingIcons");
        buildingIconGroup = _buildingIconGroup.AddComponent<RectTransform>();

        buildingIconGroup.anchorMin = new Vector2(0.5f, 0);
        buildingIconGroup.anchorMax = new Vector2(0.5f, 0);

        buildingIconGroup.SetParent(mainGroup);
        buildingIconGroup.anchoredPosition = new Vector2Int(0, 125);

        buildingIconGroup.localScale = Vector3.one;

        buildingButtons = new List<MachineButtonComponent>();
        buildingSpacers = new List<SpacerComponent>();

        buildingStartX =
            buildingSpaceOffset
            - ((((float)(buildingIcons.Length - 1)) / 2) * buildingSpaceBetween)
            - (spacers.Length * spacerPadding);

        float currentX = buildingStartX;

        for (int i = 0; i < buildingIcons.Length; i++)
        {
            buildingButtons.Add(new MachineButtonComponent());
            int current = i;
            buildingButtons[i].Create(
                new Vector2(currentX, position.y),
                new Vector2(selectorWidth, selectorWidth),
                buildingIcons[i],
                "Color 1",
                buildingIconGroup
            );
            buildingButtons[current].rt.sizeDelta = Vector2.zero;
            buildingButtons[current].longPressEventTrigger.onLongPress.AddListener(
                () =>
                {
                    inputManager.building = current;
                }
            );
            buildingButtons[current].longPressEventTrigger.onLongPress.AddListener(
                () => inputManager.StartDrag()
            );

            currentX += buildingSpaceBetween / 2;
            if (spacers.Contains(i))
            {
                currentX += spacerPadding;
                SpacerComponent spacer = new SpacerComponent();
                spacer.Create(
                    new Vector2(currentX, position.y),
                    spacerSize,
                    "Square",
                    "Grey 1",
                    buildingIconGroup
                );
                buildingSpacers.Add(spacer);
                currentX += spacerPadding;
            }
            currentX += buildingSpaceBetween / 2;
        }
    }

    void CreateMenu()
    {
        menuStartX = position.x + (size.y / 2) - (size.x / 2);
        menuSpaceBetween = (size.x - size.y) / (menuIcons.Length - 1);

        List<ImageComponent> background = new List<ImageComponent>();
        background.Add(new ImageComponent());
        background.Add(new ImageComponent());
        background.Add(new ImageComponent());

        // Background
        background[0].Create(
            position,
            new Vector2Int((size.x - size.y), size.y),
            "Square",
            "Black",
            mainGroup
        );
        background[1].Create(
            new Vector2Int(position.x - (size.y / 2) + (size.x / 2), position.y),
            new Vector2Int(size.y, size.y),
            "Circle",
            "Black",
            mainGroup
        );
        background[2].Create(
            new Vector2Int(position.x + (size.y / 2) - (size.x / 2), position.y),
            new Vector2Int(size.y, size.y),
            "Circle",
            "Black",
            mainGroup
        );

        // Buttons
        List<ButtonComponent> buttons = new List<ButtonComponent>();

        for (int i = 0; i < menuIcons.Length; i++)
        {
            buttons.Add(new ButtonComponent());
            int current = i;
            buttons[i].Create(
                new Vector2Int((int)(menuStartX + menuSpaceBetween * i), position.y),
                new Vector2Int(size.y, size.y),
                "Circle",
                "Black",
                mainGroup,
                delegate
                {
                    SelectMenu(current);
                }
            );
        }

        // Selector
        selector = new ImageComponent();
        selector.Create(
            new Vector2Int(position.x + (size.y / 2) - (size.x / 2), position.y),
            new Vector2Int(selectorWidth, selectorWidth),
            "Circle",
            "White",
            mainGroup
        );

        // Icons
        icons = new List<ImageComponent>();

        for (int i = 0; i < menuIcons.Length; i++)
        {
            icons.Add(new ImageComponent());
            icons[i].Create(
                new Vector2Int((int)(menuStartX + menuSpaceBetween * i), position.y),
                new Vector2Int(iconWidth, iconWidth),
                menuIcons[i],
                "White",
                mainGroup
            );
        }

        icons[inputManager.mode].image.color = Data.Instance.uiColors["Black"];
    }

    void SelectMenu(int newSelected)
    {
        StartCoroutine(
            MoveAndBlob(
                selector.rt,
                new Vector2((int)(menuStartX + menuSpaceBetween * newSelected), position.y)
            )
        );

        if (newSelected != inputManager.mode)
        {
            StartCoroutine(
                RecolorIcon(
                    inputManager.mode,
                    Data.Instance.uiColors["Black"],
                    Data.Instance.uiColors["White"]
                )
            );
            StartCoroutine(
                RecolorIcon(
                    newSelected,
                    Data.Instance.uiColors["White"],
                    Data.Instance.uiColors["Black"]
                )
            );

            switch (inputManager.mode)
            {
                case 0:
                    visualGrid.Show();
                    break;
                case 1:
                    break;
                case 2:
                    StartCoroutine(Move(mainGroup, selectedPosition));
                    for (int i = 0; i < (buildingButtons.Count); i++)
                    {
                        StartCoroutine(
                            Delay(
                                0.02f * i,
                                SetAnimatorBool(buildingButtons[i].animator, "Active", false)
                            )
                        );
                        StartCoroutine(
                            Delay(
                                0.02f * i,
                                MoveAndBlob(
                                    buildingButtons[i].rt,
                                    buildingButtons[i].rt.anchoredPosition / 1.2f
                                )
                            )
                        );
                    }
                    for (int i = 0; i < (buildingSpacers.Count); i++)
                    {
                        StartCoroutine(
                            SetAnimatorBool(buildingSpacers[i].animator, "Active", false)
                        );

                        StartCoroutine(
                            Delay(
                                0.02f * i,
                                MoveAndBlob(
                                    buildingSpacers[i].rt,
                                    buildingSpacers[i].rt.anchoredPosition / 1.2f
                                )
                            )
                        );
                    }
                    break;
                case 3:
                    visualGrid.RegularColor();
                    break;
            }

            switch (newSelected)
            {
                case 0:
                    visualGrid.Hide();
                    break;
                case 1:
                    break;
                case 2:
                    StartCoroutine(Move(mainGroup, deselectedPosition));
                    for (int i = 0; i < (buildingButtons.Count); i++)
                    {
                        StartCoroutine(
                            Delay(
                                0.05f * i,
                                SetAnimatorBool(buildingButtons[i].animator, "Active", true)
                            )
                        );
                        StartCoroutine(
                            Delay(
                                0.05f * i,
                                MoveAndBlob(
                                    buildingButtons[i].rt,
                                    buildingButtons[i].rt.anchoredPosition * 1.2f
                                )
                            )
                        );
                    }
                    for (int i = 0; i < (buildingSpacers.Count); i++)
                    {
                        StartCoroutine(
                            SetAnimatorBool(buildingSpacers[i].animator, "Active", true)
                        );
                        StartCoroutine(
                            Delay(
                                0.05f * i,
                                MoveAndBlob(
                                    buildingSpacers[i].rt,
                                    buildingSpacers[i].rt.anchoredPosition * 1.2f
                                )
                            )
                        );
                    }
                    break;
                case 3:
                    visualGrid.DangerColor();
                    break;
            }

            inputManager.SetPreviousMode();
            inputManager.SetMode(newSelected);
        }
        else
        {
            if (newSelected == 2)
            {
                for (int i = 0; i < (buildingButtons.Count); i++)
                {
                    StartCoroutine(
                        Delay(
                            0.02f * i,
                            SetAnimatorBool(buildingButtons[i].animator, "Bounce", true)
                        )
                    );
                }
            }
        }
    }

    IEnumerator Delay(float delay, IEnumerator func)
    {
        float timeElapsed = 0f;

        while (timeElapsed < delay)
        {
            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        yield return func;
    }

    IEnumerator SetAnimatorBool(Animator animator, string key, bool value)
    {
        animator.SetBool(key, value);
        yield return null;
    }

    IEnumerator SetAnimatorTrigger(Animator animator, string key)
    {
        animator.SetTrigger(key);
        yield return null;
    }

    IEnumerator Move(RectTransform rt, Vector2 endPos)
    {
        Vector2 startPos = rt.anchoredPosition;
        Vector2 change = endPos - startPos;

        float timeElapsed = 0f;

        while (timeElapsed < animationTime)
        {
            rt.anchoredPosition =
                startPos + change * movement.Evaluate(timeElapsed / animationTime);
            timeElapsed += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }
        rt.anchoredPosition = endPos;

        yield return null;
    }

    IEnumerator MoveAndBlob(RectTransform rt, Vector2 endPos)
    {
        Vector2 startPos = rt.anchoredPosition;
        Vector2 change = endPos - startPos;

        float timeElapsed = 0f;
        float prevTimeElapsed = 0f;
        float derivative = 0f;

        while (timeElapsed < animationTime)
        {
            rt.anchoredPosition =
                startPos + change * movement.Evaluate(timeElapsed / animationTime);

            if (timeElapsed > 0)
            {
                derivative =
                    (
                        movement.Evaluate(timeElapsed / animationTime)
                        - movement.Evaluate(prevTimeElapsed / animationTime)
                    ) / (timeElapsed - prevTimeElapsed);
            }

            rt.localScale = new Vector2(
                scale_x.Evaluate(derivative / 10),
                scale_y.Evaluate(derivative / 10)
            );

            prevTimeElapsed = timeElapsed;
            timeElapsed += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }
        rt.anchoredPosition = endPos;
        rt.localScale = new Vector2(1, 1);

        yield return null;
    }

    IEnumerator RecolorIcon(int id, Color start, Color stop)
    {
        Image image = icons[id].image;

        float timeElapsed = 0f;

        Color change = stop - start;

        while (timeElapsed < animationTime)
        {
            image.color = start + change * movement.Evaluate(timeElapsed / animationTime);

            timeElapsed += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }
        image.color = stop;

        yield return null;
    }
}
