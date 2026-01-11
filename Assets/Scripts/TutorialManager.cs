using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialManager : MonoBehaviour
{

    public GameObject textManager;
    //public GameObject cb;

    public RectTransform rectTransform;

    public Sprite [] sprites;

    
    
    public GameObject checkBox;
    public Sprite tickedCheckbox;
    public Sprite untickedCheckbox;
    private string currentMessage;

    public Sprite[] tutorialImages;
    public int displayState;
    private string displayMessage;

    public GameObject okButton;

    public string[] messages;

    RectTransform imgRect;
    RectTransform checkBoxRect;

    private int lastState;

    public GameObject icon;
    public GameObject leftButton;
    public GameObject rightButton;

    public float[] scroll = {0f, 0f, -186.75f, 0f, 0f, 0f, -103.9f, -124.6f, -124.6f, -124.6f, 0f, -124.6f, -124.6f, 0f, 0f};


    //-113
    //-121.3f
    public (float X, float Y)[] iconCoords = new [] {(0f, 0f),
                                                    (278f, -74f),
                                                     (295, -155f),
                                                     (228.22f, -121.3f),
                                                     (228.22f, -166f),
                                                     (196.71f, -154.7f),
                                                     (196.71f, -154.7f),
                                                     (198f, -113f)}; 
    public (float X, float Y)[] iconSize = new [] {(0f, 0f),
                                                   (30f, 30f),
                                                   (30f, 30f),
                                                   (60f, 60f),
                                                   (60f, 60f),
                                                   (30f, 30f),
                                                   (30f, 30f),
                                                   (30f, 30f)};

    public (float X, float Y)[] checkBoxCoord = new [] {(0f, 0f),
                                                   (0f, 668f),
                                                   (0f, 370),
                                                   (0f, 550f),
                                                   (0f, 550f),
                                                   (0f, 580f),
                                                   (0f, 475f),
                                                   (0f, 450f),
                                                   (0f, 450f),
                                                   (0f, 450f)};
    void Start()
    {
        GameObject scroller = GameObject.Find("/UI/Safe Area/tutorialBox/textScroller/content/Text");
        rectTransform = scroller.GetComponent<RectTransform>();

        messages = new string[] {
        "Welcome to Factory Game! \n\nA game where you mine, transform and then collect resources. \n\nLet's start off with a tutorial",
        "Goal #1 \n\nPlace a factory over a yellow resource tile \n\nFactory icon: ",
        "Goal #2 \n\nGreat! Now place a few conveyorbelts going from the factory \n\nConveyorbelt: \n\nTip: When the conveyorbelt is selected. You can drag on the screen to create a whole bunch of them",
        "Goal #3 \n\nGreat! Now connect the factory with conveyorbelts to the base \n\nBase: ",
        "Goal #4 \n\nAwesome! Now move ten more yellow squares to the base \n\n\nBase: ",
        "Goal #5 \n\nSuper! Now cut the yellow squares using the splitter and move them to the base \n\nSplitter: ",
        "Goal #6 \n\nSuberb! Now rotate the shapes 180 degrees using the rotator. \n\nRotator:\n\nTip: You might have to rotate them twice",
        "Goal #7 \n\nGreat! Now use the merger to create the item in the base. \n\nMerger: \n\nTip: Split the squares first and then use the merger to combine the shapes",
        "Goal #8 \n\nKeep it up! Now create 50 squares of the type outlined in the base. \n\nTip: You can split the same shape several times and then put them together in the merger",
        "Nice work! You finished the tutorial but you can bring it up again if you need any assistance!"};

        //Debug.Log("Current state i början: " + Data.Instance.currentGoal);
        if(Data.Instance.currentGoal < messages.Length) {
            displayState = Data.Instance.currentGoal;
        }
        else {
            displayState = messages.Length-1;
        }
        currentMessage = messages[displayState];
        displayMessage = messages[displayState];
        lastState = messages.Length - 1;
        checkBoxRect = checkBox.GetComponent<RectTransform>();
        imgRect = icon.GetComponent<RectTransform>();
        imgRect.anchoredPosition = new Vector2(iconCoords[0].Item1, iconCoords[0].Item2);
        DisplayText();
        ResetScrollbar();
    }

    void Awake(){
        Base.accomplishedGoal += SwitchCurrentState;
        Game.accomplishedGoal  += SwitchCurrentState;
    }
    void OnDestroy() {
        Base.accomplishedGoal -= SwitchCurrentState;
        Game.accomplishedGoal -= SwitchCurrentState;
    }

     //Trycker vi åt höger när vi på första sliden kommer vi till den sista
    void SwitchRight(){
        if(displayState != messages.Length-1 && displayState < Data.Instance.currentGoal)
        {
            displayState++;
            displayMessage = messages[displayState];
        }
    }

    //Trycker vi åt vänster när vi på första sliden kommer vi till den sista
    void SwitchLeft(){
        if (displayState != 0){
            displayState--;
            displayMessage = messages[displayState];
        }
    }

    void SwitchCurrentState(){
        Debug.Log("SwitchCurrentState");
        if(Data.Instance.currentGoal < messages.Length){

            //Specialfall för de första målen i tutorialen
            if(Data.Instance.currentGoal == 1 || Data.Instance.currentGoal == 2){
                //Data.Instance.currentGoal++;
            }
            //Data.Instance.currentGoal++;
            DisplayCurrent();

            //Definitionen av cancer. PopupBox och TutorialBox borde separeras
            if(gameObject.name == "tutorialBox"){
                gameObject.SetActive(true);
                UpdateImages();
            }
        }
    }

    public void ResetScrollbar(){
        Vector3 pos = rectTransform.transform.localPosition;
        Debug.Log("Display state" + displayState);
        Debug.Log("Value " + scroll[displayState]);
        pos.y = scroll[displayState];
        rectTransform.transform.localPosition = pos;
    }

    public void DisplayCurrent(){
        displayState = Data.Instance.currentGoal;
        //Debug.Log("Display state: " + displayState);
        displayMessage = messages[displayState];
        DisplayText();
        ResetScrollbar();
    }

    public void DisplayNext() {
        SwitchRight();
        DisplayText();
        ResetScrollbar();
    }

    public void DisplayPrev() {
        SwitchLeft();
        DisplayText();
        ResetScrollbar();
    }

    void DisplayText() {
        TextMeshProUGUI textComp = textManager.GetComponent<TextMeshProUGUI>();
        textComp.text = displayMessage; 

        if (textComp.preferredHeight > 450) {
            textManager.GetComponent<RectTransform>().sizeDelta = new Vector2 (
                339.79f, 
                textComp.preferredHeight - 50
            );
            textManager.transform.parent.GetComponent<RectTransform>().anchoredPosition = new Vector2 (
                0f, 
                0f
            );
        }
        else {
            textManager.GetComponent<RectTransform>().sizeDelta = new Vector2 (
                339.79f, 
                0
            );
            textManager.transform.parent.GetComponent<RectTransform>().anchoredPosition = new Vector2 (
                0f, 
                60f
            );
        }
        


        UpdateImages();
    }

    private void HandleButtons () {
        if (displayState == Data.Instance.currentGoal) {
            rightButton.SetActive(false);
        } else {
            rightButton.SetActive(true);
        }

        if (displayState == 0 || displayState == 1) {
            leftButton.SetActive(false);
        } else {
            leftButton.SetActive(true);
        }

        if(displayState > 0){
            okButton.SetActive(false);
        }
    }

    private void UpdateImages(){
        HandleButtons();
        //Debug.Log("displayState: " + displayState + ". tutorialImages.Length: " + tutorialImages.Length);

        if(Data.Instance.currentGoal > displayState){
            //Checkboxen används inte då den är ful
            //checkBox.SetActive(true);
            checkBox.GetComponent<Image>().sprite = tickedCheckbox;
            checkBoxRect.anchoredPosition = new Vector2(checkBoxCoord[displayState].Item1, checkBoxCoord[displayState].Item2);
        }
        else{
            checkBox.SetActive(false);
        }
        if(displayState < tutorialImages.Length){
        icon.SetActive(true);
        imgRect.anchoredPosition = new Vector2(iconCoords[displayState].Item1, iconCoords[displayState].Item2);
        imgRect.sizeDelta = new Vector2(iconSize[displayState].Item1, iconSize[displayState].Item2);
        icon.GetComponent<Image>().sprite = tutorialImages[displayState];
        }
        else{
            icon.SetActive(false);
        }
        if(displayState == lastState){
            checkBox.SetActive(false);
        }
    }

        public void StartTutorial(){
        if(Data.Instance.currentGoal == 0){
            Data.Instance.currentGoal++;
            DisplayNext();
        }

    }
}   

