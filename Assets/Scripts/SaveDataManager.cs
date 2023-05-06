using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

//PlayerPrefs for options, JSON for level completion
public class SaveDataManager : MonoBehaviour
{
    //RESETS DATA ON START IF TRUE
    [SerializeField] private bool reset = false;

    private string saveFile;
    [SerializeField] SaveData saveData = new SaveData();

    void Awake()
    {
        saveFile = Application.persistentDataPath + "/saveData.json";
        //Debug.Log(saveFile);

        if (reset) 
        {
            File.Delete(saveFile);
            PlayerPrefs.DeleteAll();
        }
        else
        {
            readFile();
            getPlayerPrefs();
        }
        
    }

    /// <summary>
    /// reads the file and dumps into saveData
    /// </summary>
    public void readFile()
    {
        if (File.Exists(saveFile))
        {
            string fileContents = File.ReadAllText(saveFile);
            saveData = JsonUtility.FromJson<SaveData>(fileContents);

            ManageGame.finishedLevels = saveData.finishedLevels;
        }
    }
    /// <summary>
    /// writes the file to saveData
    /// </summary>
    public void writeFile()
    {
        saveData.finishedLevels = ManageGame.finishedLevels;

        string jsonString = JsonUtility.ToJson(saveData);
        File.WriteAllText(saveFile, jsonString);
    }

    public static void setPlayerPrefs() 
    {
        PlayerPrefs.SetFloat("Transparency", SliderOptions.sliderControls["Transparency"]);
        PlayerPrefs.SetFloat("Scale", SliderOptions.sliderControls["Scale"]);
        PlayerPrefs.SetFloat("Music", AudioSliderController.volume["Music"]);
        PlayerPrefs.SetFloat("SFX", AudioSliderController.volume["SFX"]);
        PlayerPrefs.SetString("FPS",ButtonOptions.buttonControls["FPS"].ToString());
        PlayerPrefs.SetString("Side", ButtonOptions.buttonControls["Side"].ToString());
        PlayerPrefs.Save();
    }

    public static void getPlayerPrefs()
    {
        if (PlayerPrefs.HasKey("Scale"))
        {
            SliderOptions.sliderControls["Transparency"] = PlayerPrefs.GetFloat("TranTransparency");
            SliderOptions.sliderControls["Scale"] = PlayerPrefs.GetFloat("Scale");
            AudioSliderController.volume["Music"] = PlayerPrefs.GetFloat("Music");
            AudioSliderController.volume["SFX"] = PlayerPrefs.GetFloat("SFX");
            ButtonOptions.buttonControls["FPS"] = bool.Parse(PlayerPrefs.GetString("FPS"));
            ButtonOptions.buttonControls["Side"] = bool.Parse(PlayerPrefs.GetString("Side"));
        }
    }

}

[System.Serializable]
public class SaveData
{
    public List<string> finishedLevels = new List<string>();
}
