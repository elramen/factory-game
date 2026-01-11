using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Analytics;
using System;

public class Base : MonoBehaviour 
{
    // Animation for when the progress bar is updated
    [SerializeField]
    private AnimationCurve progressAnimation;
    // Animation for progress bar when goal is reached
    [SerializeField]
    private AnimationCurve completedAnimation;
    public Goal[] goals = new Goal[15];
    public int currentGoal;
    public int collectedAmount;
    public GameObject goalPrefab;

    public delegate void UpdateTutorial();
    public static event UpdateTutorial accomplishedGoal;

    private bool hasMovedIntoBase = false;

    public void Init() {
        progressAnimation = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
        progressAnimation.preWrapMode = WrapMode.PingPong;
        progressAnimation.postWrapMode = WrapMode.PingPong;

        completedAnimation = new AnimationCurve(
            new Keyframe(0, 0), 
            new Keyframe(0.5f, 1), 
            new Keyframe(1, 0)
        );
        completedAnimation.preWrapMode = WrapMode.PingPong;
        completedAnimation.postWrapMode = WrapMode.PingPong;

        CreateGoals();
    }

    //Kan inte hanteras i init då det skulle ge felaktiga värden från Data
    public void Start(){
        if (Data.Instance.currentGoal >= 14) {
            goals[14] = new Goal(
                Data.Instance.currentGoal - 3, 
                AutoGenerateItem(true), 
                Data.Instance.goalAmount
            );
        }
        goals[Math.Min(Data.Instance.currentGoal, 14)].item.gameObject.SetActive(true);
    }

    private void CreateGoals() {
        int length;

        GameObject wantedPrefab1 = Instantiate(goalPrefab);
        Item wantedItem1 = wantedPrefab1.AddComponent<Item>();
        wantedItem1.Init(CoolColor.Yellow);
        wantedItem1.transform.SetParent(this.transform);

        //Dummy goals används för att sammanfoga basens mål med tutorialens mål
        Goal dummy1 = new Goal(1, wantedItem1, 1);
        Goal dummy2 = new Goal(1, wantedItem1, 1);
        Goal dummy3 = new Goal(1, wantedItem1, 1);
        Goal dummy4 = new Goal(1, wantedItem1, 1);


        Goal goal1 = new Goal(1, wantedItem1, 10);

        GameObject wantedPrefab2 = Instantiate(goalPrefab);
        Item wantedItem2 = wantedPrefab2.AddComponent<Item>();
        wantedItem2.Init(CoolColor.Yellow);
        wantedItem2.transform.SetParent(this.transform);
        length = wantedItem2.pixels.GetLength(0);
        for (int i = length / 2; i < length; i++)
        {
            for (int j = 0; j < length; j++)
            {
                wantedItem2.pixels[i, j] = CoolColor.Null;
            }
        }
        wantedItem2.UpdateSprite();
        Goal goal2 = new Goal(2, wantedItem2, 20);

        GameObject wantedPrefab3 = Instantiate(goalPrefab);
        Item wantedItem3 = wantedPrefab3.AddComponent<Item>();
        wantedItem3.Init(CoolColor.Yellow);
        wantedItem3.transform.SetParent(this.transform);
        length = wantedItem3.pixels.GetLength(0);
        for (int i = 0; i < length/2; i++)
        {
            for (int j = 0; j < length; j++)
            {
                wantedItem3.pixels[i, j] = CoolColor.Null;
            }
        }
        wantedItem3.UpdateSprite();
        Goal goal3 = new Goal(3, wantedItem3, 30);

        GameObject wantedPrefab4 = Instantiate(goalPrefab);
        Item wantedItem4 = wantedPrefab4.AddComponent<Item>();
        wantedItem4.Init(CoolColor.Yellow);
        wantedItem4.transform.SetParent(this.transform);
        length = wantedItem4.pixels.GetLength(0);
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < length/2; j++)
            {
                wantedItem4.pixels[i, j] = CoolColor.Green;
            }
        }
        wantedItem4.UpdateSprite();
        Goal goal4 = new Goal(4, wantedItem4, 40);

        GameObject wantedPrefab5= Instantiate(goalPrefab);
        Item wantedItem5 = wantedPrefab5.AddComponent<Item>();
        wantedItem5.Init(CoolColor.Null);
        wantedItem5.transform.SetParent(this.transform);
        length = wantedItem5.pixels.GetLength(0);
        for (int i = 0; i < length/2; i++)
        {
            for (int j = 0; j < length/2; j++)
            {
                wantedItem5.pixels[i, j] = CoolColor.Yellow;
            }
        }
        for (int i = length/2; i < length; i++)
        {
            for (int j = length/2; j < length; j++)
            {
                wantedItem5.pixels[i, j] = CoolColor.Green;
            }
        }
        wantedItem5.UpdateSprite();
        Goal goal5 = new Goal(5, wantedItem5, 50);

        GameObject wantedPrefab6 = Instantiate(goalPrefab);
        Item wantedItem6 = wantedPrefab6.AddComponent<Item>();
        wantedItem6.Init(CoolColor.Yellow);
        wantedItem6.transform.SetParent(this.transform);
        length = wantedItem6.pixels.GetLength(0);
        for (int i = 0; i < length/2; i++)
        {
            for (int j = length/2; j < length; j++)
            {
                wantedItem6.pixels[i, j] = CoolColor.Green;
            }
        }
        for (int i = 0; i < length/2; i++)
        {
            for (int j = 0; j < length/2; j++)
            {
                wantedItem6.pixels[i, j] = CoolColor.Red;
            }
        }
        wantedItem6.UpdateSprite();
        Goal goal6 = new Goal(6, wantedItem6, 60);

        GameObject wantedPrefab7 = Instantiate(goalPrefab);
        Item wantedItem7 = wantedPrefab7.AddComponent<Item>();
        wantedItem7.Init(CoolColor.Yellow);
        wantedItem7.transform.SetParent(this.transform);
        length = wantedItem7.pixels.GetLength(0);
        for (int i = 0; i < length/2; i++)
        {
            for (int j = 0; j < length/2; j++)
            {
                wantedItem7.pixels[i, j] = CoolColor.Red;
            }
        }
        for (int i = length/2; i < length; i++)
        {
            for (int j = 0; j < length/2; j++)
            {
                wantedItem7.pixels[i, j] = CoolColor.Green;
            }
        }
        for (int i = length/2; i < length; i++)
        {
            for (int j = length/2; j < length; j++)
            {
                wantedItem7.pixels[i, j] = CoolColor.Blue;
            }
        }
        wantedItem7.UpdateSprite();
        Goal goal7 = new Goal(7, wantedItem7, 70);

        GameObject wantedPrefab8 = Instantiate(goalPrefab);
        Item wantedItem8 = wantedPrefab8.AddComponent<Item>();
        wantedItem8.Init(CoolColor.Yellow);
        wantedItem8.transform.SetParent(this.transform);
        length = wantedItem8.pixels.GetLength(0);
        for (int i = 0; i < length/2; i++)
        {
            for (int j = 0; j < length/2; j++)
            {
                wantedItem8.pixels[i, j] = CoolColor.Green;
            }
        }
        for (int i = length/2; i < length; i++)
        {
            for (int j = 0; j < length/2; j++)
            {
                wantedItem8.pixels[i, j] = CoolColor.Blue;
            }
        }
        for (int i = 0; i < length/2; i++)
        {
            for (int j = length/2; j < length; j++)
            {
                wantedItem8.pixels[i, j] = CoolColor.Red;
            }
        }
        wantedItem8.UpdateSprite();
        Goal goal8 = new Goal(8, wantedItem8, 80);

        GameObject wantedPrefab9 = Instantiate(goalPrefab);
        Item wantedItem9 = wantedPrefab9.AddComponent<Item>();
        wantedItem9.Init(CoolColor.Blue);
        wantedItem9.transform.SetParent(this.transform);
        length = wantedItem9.pixels.GetLength(0);
        for (int i = 0; i < length/2; i++)
        {
            for (int j = 0; j < length/2; j++)
            {
                wantedItem9.pixels[i, j] = CoolColor.Red;
            }
        }
        for (int i = length/2; i < length; i++)
        {
            for (int j = length/2; j < length; j++)
            {
                wantedItem9.pixels[i, j] = CoolColor.Red;
            }
        }
        wantedItem9.UpdateSprite();
        Goal goal9 = new Goal(9, wantedItem9, 90);

        GameObject wantedPrefab10 = Instantiate(goalPrefab);
        Item wantedItem10 = wantedPrefab10.AddComponent<Item>();
        wantedItem10.Init(CoolColor.Green);
        wantedItem10.transform.SetParent(this.transform);
        wantedItem10.TransformHeart();
        wantedItem10.TransformSplitVertically();
        wantedItem10.UpdateSprite();
        Goal goal10 = new Goal(10, wantedItem10, 100);

        goals[0] = dummy1;
        goals[1] = dummy2;
        goals[2] = dummy3;
        goals[3] = dummy4;
        goals[4] = goal1;
        goals[5] = goal2;
        goals[6] = goal3;
        goals[7] = goal4;
        goals[8] = goal5;
        goals[9] = goal6;
        goals[10] = goal7;
        goals[11] = goal8;
        goals[12] = goal9;
        goals[13] = goal10;

        foreach (Goal goal in goals) {
            if (goal != null) {
                goal.item.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 9;
            }
        }

        for(int i = 0; i<goals.Length; i++) {
            if (goals[i] != null) {
                goals[i].item.gameObject.SetActive(false);
            }
        }
        
    }

    public void Receive(Item receivedItem) {
        // Do stuff if the correct item was received
        if (receivedItem != null && goals[Math.Min(Data.Instance.currentGoal, 14)] != null) {
            if (receivedItem.Equals(goals[Math.Min(Data.Instance.currentGoal, 14)].item)) {
                collectedAmount++;

                // Update progress bar
                float progress = (float) collectedAmount / (float) goals[Math.Min(Data.Instance.currentGoal, 14)].amount;
                StartCoroutine(MoveProgressMeter(progress));

                if (collectedAmount == goals[Math.Min(Data.Instance.currentGoal, 14)].amount) {
                    GoalCompleted();
                } 
            }
        }
        Resources.UnloadUnusedAssets();
    }

    private Item AutoGenerateItem(bool loadLastGoal) {
        Debug.Log("Autogenerating item");
        Item[] quadrants = new Item[4];
        CoolColor[] colors = new CoolColor[]{CoolColor.Blue, CoolColor.Green, CoolColor.Yellow, CoolColor.Red};
         System.Random rand;

        if (loadLastGoal == false) {
            Data.Instance.goalSeed = Environment.TickCount & Int32.MaxValue;
            Debug.Log("GoalSeed saved: " + Data.Instance.goalSeed);
        }
        else {
            Debug.Log("GoalSeed loaded: " + Data.Instance.goalSeed);
        }
        rand = new System.Random(Data.Instance.goalSeed);
        
        // Skapa fyra kvadranter
        for (int i = 0; i<4; i++) {
            GameObject testprefab = Instantiate(goalPrefab);
            testprefab.name = "CurrentGoalItem";
            Item quadrant = testprefab.AddComponent<Item>();
            quadrant.transform.SetParent(this.transform);

            quadrant.Init(colors[rand.Next(4)]);
            quadrant.TransformSplitVertically();
            quadrant.TransformRotateClockwise();
            quadrant.TransformSplitVertically();
            // varje kvadrant ska ligga i sin egen kvadrant (en uppe till vänster, en nere till höger osv...)
            // när vi gör hjärtsplitten
            for (int j=0; j<i; j++) {
                quadrant.TransformRotateClockwise();
            }
            // 50% chans att kvadranten blir hjärtsplittad
            if (rand.Next(2) == 1) {
                quadrant.TransformHeart();
            }
            
            // Rotera kvadranten 0-3 gånger
            int rotations = rand.Next(4);
            for (int j = 0; j<rotations; j++) {
                quadrant.TransformRotateClockwise();
            }

            quadrant.UpdateSprite();
            quadrants[i] = quadrant;
            
        }
        //Merga ihop kvadranterna
        quadrants[0].TransformMerge(quadrants[1]);
        quadrants[0].TransformMerge(quadrants[2]);
        quadrants[0].TransformMerge(quadrants[3]);
        quadrants[0].UpdateSprite();
        quadrants[0].GetComponent<SpriteRenderer>().sortingOrder = 9;

        // Förstör de 3 andra hoppas att detta räcker och att ingen texture behöver tas bort manuellt
        for (int i = 1; i<4; i++) {
            Destroy(quadrants[i].gameObject);
        }

        return quadrants[0];
    }

    // Called when the goal has been reached
    // Do something cool. Load next goal?
    private void GoalCompleted()
    {
        goals[Math.Min(Data.Instance.currentGoal, 14)].item.gameObject.SetActive(false);
        //Mål 0-12 avklarat
        if (Data.Instance.currentGoal < goals.Length - 2)
        {
            Data.Instance.currentGoal++; 
        }
        else {
            int currentAmount = goals[Math.Min(Data.Instance.currentGoal, 14)].amount;
            if (Data.Instance.currentGoal == goals.Length - 2) {
                Data.Instance.currentGoal++;
            }
            else {
                Destroy(goals[Math.Min(Data.Instance.currentGoal, 14)].item.gameObject);
            }
            
            goals[Math.Min(Data.Instance.currentGoal, 14)] = new Goal(Data.Instance.currentGoal - 3, AutoGenerateItem(false), currentAmount);
            Data.Instance.goalAmount = currentAmount;
            PlayerPrefs.SetInt("goalAmount", Data.Instance.goalAmount);
            PlayerPrefs.SetInt("goalSeed", Data.Instance.goalSeed);
        }
        

        Debug.Log("Base current goal: " + Data.Instance.currentGoal);
        goals[Math.Min(Data.Instance.currentGoal, 14)].item.gameObject.SetActive(true);
        collectedAmount = 0;
        accomplishedGoal();
        StartCoroutine(GoalAnimation());
        Debug.Log("Goal completed");

        int completed = Data.Instance.currentGoal-1;

        AnalyticsResult res = Analytics.CustomEvent("nivåAvklarad", new Dictionary<string, object>
        {
        {"nivå", completed}
        });
        Debug.Log("Event status: " + res);
        PlayerPrefs.SetInt("currentGoal", Data.Instance.currentGoal);

        GameObject progressBarContainer = GameObject.Find("ProgressBarContainer");
        ProgressBar progressBarScript = progressBarContainer.GetComponent<ProgressBar>();
        if (Data.Instance.currentGoal > 4) {
            Data.Instance.levelNr++;
        }
        PlayerPrefs.SetInt("levelNr", Data.Instance.levelNr);
        progressBarScript.UpdateGoal();

        // Rewarded ads after completing 5 goals
        if (Data.Instance.levelNr % 5 == 1 && Data.Instance.levelNr >= 10)
        {
            //&& Data.Instance.levelNr >= 10
            RewardedAdsButton.rewardedAdsPopUp.SetActive(true);
            TMPro.TextMeshProUGUI text = RewardedAdsButton.rewardedAdsPopUp.gameObject.transform.GetChild(1).gameObject.GetComponent<TMPro.TextMeshProUGUI>();
            text.SetText("Congratulations, you have completed " + (Data.Instance.levelNr -1) + " levels! Watch this ad and get 2x speed for 2 minutes");
    
        }
    }

    private IEnumerator GoalAnimation() {
        float animationTime = 1f;

        // Find the progress bar image color
        GameObject progressBar = transform.parent.gameObject.transform.Find("ProgressBar").gameObject;
        Image progressBarImage = progressBar.GetComponent<Image>();

        Color startColor = new Color(
            progressBarImage.color.r,
            progressBarImage.color.g,
            progressBarImage.color.b,
            progressBarImage.color.a   
        );

        Color animationColor = new Color(0, 0, 255);

        float timeElapsed = 0f;

        while (timeElapsed < animationTime)
        {
            progressBarImage.color = 
                Color.Lerp(startColor, animationColor, completedAnimation.Evaluate(timeElapsed / animationTime));

            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        progressBarImage.color = startColor;
        progressBarImage.fillAmount = 0;

        yield return null;
    }

    

    // Updates the progress meter to a new value with a small, smooth, animation.
    // float progress - How much of the progress meter that should be filled (0-1)
    private IEnumerator MoveProgressMeter(float progress) {
        float movementTime = 0.5f;

        // Find the progress bar image component
        GameObject progressBar = transform.parent.gameObject.transform.Find("ProgressBar").gameObject;
        Image progressBarImage = progressBar.GetComponent<Image>();

        // Calculate change to be made
        float prevProgress = progressBarImage.fillAmount;
        float change = progress - prevProgress;

        float timeElapsed = 0f;

        while (timeElapsed < movementTime)
        {
            progressBarImage.fillAmount =
                prevProgress + change * progressAnimation.Evaluate(timeElapsed / movementTime);

            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        // Ensure that the progress meter is set to exactly the desired value
        progressBarImage.fillAmount = progress;

        yield return null;
    }
    public float GetProgress() {
        return (float) collectedAmount / (float) goals[Math.Min(Data.Instance.currentGoal, 14)].amount;
    }
}
