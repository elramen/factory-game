using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;
using System.Timers;
using UnityEngine.UI;
using TMPro;

public class Stars {

    public List<int[]> realTimes;
    public List<List<int>> times;
    public int numGoals;

    public int numStars;
    //public List<int> currentGoal;
    public int[] currentGoal;

    public int currentSubGoal;
    
    public bool hasStarted = false;

    public bool second = true;
    
    
    // UI
    private TextMeshProUGUI timeText;

    private TextMeshProUGUI counterText;
    private List<Image> stars;
    
    public Stars(Game game){
        Debug.Log("currentGoal : " + Data.Instance.currentGoal);
        Debug.Log("starsSubGoal : " + Data.Instance.starsSubGoal);
        Debug.Log("currentTime : " + Data.Instance.currentTime);

        times = new List<List<int>>();
        realTimes = new List<int[]>();
        numGoals = 14;
        //GenerateTimes();
        realGenerateTimes();
        currentGoal = realTimes[Math.Min(14, Data.Instance.currentGoal)];
        currentSubGoal = currentGoal[Data.Instance.starsSubGoal];
        numStars = 3 - Data.Instance.starsSubGoal;
        //Man borde nog ta bort den här listenern när programmet avslutas
        Base.accomplishedGoal += AccomplishedGoal;
        if(Data.Instance.currentGoal > 3){
            hasStarted = true;
        }

        // UI
        GameObject uiParent = GameObject.Find("/UI/Safe Area/Stars");
        timeText = uiParent.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        stars = new List<Image>();
        stars.Add(uiParent.transform.GetChild(1).GetComponent<Image>());
        stars.Add(uiParent.transform.GetChild(2).GetComponent<Image>());
        stars.Add(uiParent.transform.GetChild(3).GetComponent<Image>());

        GameObject uiParent2 = GameObject.Find("/UI/Safe Area/StarCounter");
        counterText = uiParent2.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        WriteTime(Data.Instance.currentTime);
        StarCounter(Data.Instance.totalStars);
    }

    public void realGenerateTimes(){
        int[] first = {30, 60, 120, 0};
        realTimes.Add(first);
        realTimes.Add(first);
        realTimes.Add(first);
        realTimes.Add(first);
        realTimes.Add(first);

        int[] second = {60, 120, 240, 0};
        realTimes.Add(second);

        int[] third = {120, 240, 480, 0};
        realTimes.Add(third);

        int[] fourth = {240, 480, 960, 0};
        realTimes.Add(fourth);
        realTimes.Add(fourth);

        int[] fifth = {180, 360, 540, 0};
        realTimes.Add(fifth);

        //Ej gjorda
        realTimes.Add(fifth);
        realTimes.Add(fifth);
        realTimes.Add(fifth);
        realTimes.Add(fifth);
        realTimes.Add(fifth);

        for (int i = realTimes.Count; i <= Data.Instance.currentGoal; i++){
            int[] sixth = {180, 360, 540, 0};
            realTimes.Add(sixth);
        }
    }
    public void AccomplishedGoal(){
        if(Data.Instance.currentGoal >= realTimes.Count){
            int[] sixth = {180, 360, 540, 0};
            realTimes.Add(sixth);
        }

        if(hasStarted == false){
            hasStarted = true;
        }
        else if(Data.Instance.currentGoal > 3 && hasStarted){
            hasStarted = true;
            Data.Instance.starsSubGoal = 0;        
            Data.Instance.currentTime = 0;
            Data.Instance.totalStars += numStars;
            currentGoal = realTimes[Data.Instance.currentGoal];
            //currentGoal = times[Data.Instance.currentGoal-1];
            currentSubGoal = currentGoal[Data.Instance.starsSubGoal];
            numStars = 3;
        } 
    }

    public void Update(double deltaTime){
        if(Data.Instance.currentGoal > 3 && second){
            Data.Instance.currentTime += Convert.ToInt32(deltaTime);
            if(Data.Instance.currentTime > currentSubGoal && Data.Instance.starsSubGoal < 3){
                Data.Instance.starsSubGoal++;
                currentSubGoal = currentGoal[Data.Instance.starsSubGoal];
                numStars = 3 - Data.Instance.starsSubGoal;
            }
            SetPlayerPrefs();
            second = false;
        }
        else{
            second = true;
        }
        //Debug.Log("Length : " + times.Count);
        //Debug.Log("Goal Index : " + Data.Instance.currentGoal + ". SubGoalIndex: " + Data.Instance.starsSubGoal + ". Time : " + currentGoal[0] + " " + currentGoal[1] + " " + currentGoal[2] + "Real time" + Data.Instance.currentTime + " Num stars: " + numStars);
        WriteTime(currentSubGoal - Data.Instance.currentTime);
        DrawStars(numStars);
        StarCounter(Data.Instance.totalStars);
    }

    public void SetPlayerPrefs(){
        PlayerPrefs.SetInt("currentTime", Data.Instance.currentTime);
        PlayerPrefs.SetInt("starsSubGoal", Data.Instance.starsSubGoal);
        PlayerPrefs.SetInt("totalStars", Data.Instance.totalStars);
    }


    // UI
    void DrawStars(int starCount) {
        for (int i = 0; i < starCount; i++)
        {
            stars[i].color = Data.Instance.uiColors["Star 1"];
        }
        for (int i = starCount; i < stars.Count; i++)
        {
            stars[i].color = Data.Instance.uiColors["Star 2"];
        }     
    }

    void WriteTime(int time) {
        if (Data.Instance.starsSubGoal == 3){
            time = 0;
        }
        int minutes = (int)Math.Floor(time / 60.0);
        minutes = minutes < 60 ? minutes : 59; 
        
        int seconds = time - (minutes * 60);
        seconds = seconds < 60 ? seconds : 59; 

        timeText.text = (minutes < 10 ? "0" : "") + minutes + ":" + (seconds < 10 ? "0" : "") + seconds; 
    }

    void StarCounter(int count){
        counterText.text = "×" + count;
    }

}