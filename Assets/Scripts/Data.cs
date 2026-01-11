using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public enum CoolColor
{
    Null,
    Green,
    Yellow,
    Blue,
    Red
}

[CreateAssetMenu(fileName = "Data", menuName = "Custom/Data", order = 0)]
public class Data : SingletonScriptableObject<Data>
{
    private bool initialized = false;

    public int gridWidth;
    public int gridHeight;
    public float gridSize;

    public int currentGoal;

    public int currentTime;

    public int totalStars;
    public int starsSubGoal;    
    public int goalSeed;
    public int goalAmount;
    public int levelNr = 1;

    [System.Serializable]
    public struct SpritePair
    {
        public string name;
        public Sprite sprite;
    }

    [System.Serializable]
    public struct Texture2DPair
    {
        public string name;
        public Texture2D texture;
    }

    [System.Serializable]
    public struct GameObjectPair
    {
        public string name;
        public GameObject gameObject;
    }

    [System.Serializable]
    public struct ColorPair
    {
        public string name;
        public Color color;
    }

    public Color[] colors;

    public ColorPair[] _uiColors;
    public Dictionary<string, Color> uiColors;

    public SpritePair[] _sprites;
    public Dictionary<string, Sprite> sprites;
    public Dictionary<string, Sprite> generatedSprites;

    public GameObjectPair[] _prefabs;
    public Dictionary<string, GameObject> prefabs;

    public GameObjectPair[] _buildings;
    public Dictionary<string, GameObject> buildings;

    public GameObjectPair[] _resources;
    public Dictionary<string, GameObject> resources;

    public GameObjectPair[] _resourcesInGround;
    public Dictionary<string, GameObject> resourcesInGround;

    // converts the regular array to a dict. (This is done to make the dictionary visible from the editor).
    public void Init()
    {
        uiColors = new Dictionary<string, Color>();
        foreach (ColorPair pair in _uiColors)
        {
            uiColors[pair.name] = pair.color;
        }

        sprites = new Dictionary<string, Sprite>();
        foreach (SpritePair pair in _sprites)
        {
            sprites[pair.name] = pair.sprite;
        }

        generatedSprites = new Dictionary<string, Sprite>();

        prefabs = new Dictionary<string, GameObject>();
        foreach (GameObjectPair pair in _prefabs)
        {
            prefabs[pair.name] = pair.gameObject;
        }

        buildings = new Dictionary<string, GameObject>();
        foreach (GameObjectPair pair in _buildings)
        {
            buildings[pair.name] = pair.gameObject;
        }

        resources = new Dictionary<string, GameObject>();
        foreach (GameObjectPair pair in _resources)
        {
            resources[pair.name] = pair.gameObject;
        }

        resourcesInGround = new Dictionary<string, GameObject>();
        foreach (GameObjectPair pair in _resourcesInGround)
        {
            resourcesInGround[pair.name] = pair.gameObject;
        }
    }
}

/// <summary> 
/// Skapar en instans av 
/// </summary>
public class SingletonScriptableObject<T> : ScriptableObject where T : SingletonScriptableObject<T>
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load(typeof(T).Name) as T;
            }
            return instance;
        }
    }
}
