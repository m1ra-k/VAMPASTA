// BURGER
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

// should be on an object like gamedatamanager that is present on every screen
public class GameDataManager : MonoBehaviour
{
    [SerializeField]
    public GameData gameData;

    public GameObject DialogueSystemManager;

    // hook to the save button in menu
    // going for 3 save files

    void Update() 
    {
        if (Input.GetKeyDown(KeyCode.Alpha0)) 
        {
            SaveGameData(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1)) 
        {
            SaveGameData(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) 
        {
            SaveGameData(2);
        }
        if (Input.GetKeyDown(KeyCode.L)) 
        {
            LoadGameData();
        }
        // very temp!!
    }

    #region Save/Load
    public void SaveGameData(int saveSlot) 
    {
        // get scene
        gameData.scene = SceneManager.GetActiveScene().name;

        // get dialogue index, if exists
        gameData.dialougeIndex = DialogueSystemManager.GetComponent<DialogueSystemManager>().GetDialogueIndex();

        // construct data and path, then save
        string filePath = Application.persistentDataPath + "/burger.txt"; // make more hidden, just testing
        
        // convert class to json
        string gameDataJson = JsonUtility.ToJson(gameData, true);

        // convert json to bytes
        byte[] gameDataBytes = Encoding.ASCII.GetBytes(gameDataJson);

        string gameDataShifted = ShiftBytes(gameDataBytes, 9);
        
        // shift bytes

        // write to txt file in certain slot
        string[] saveFileGameData = File.ReadAllLines(filePath);
        Debug.Log(saveFileGameData.Length);
        saveFileGameData[saveSlot] = gameDataShifted;
        File.WriteAllLines(filePath, saveFileGameData);

        Debug.Log("saved data to here: " + filePath);
    }

    public void LoadGameData() 
    {
        // JsonUtility.FromJson<GameData>()
    }
    #endregion

    #region Adjust Data
    // increment/decrement stuff in gamedata
    
    #endregion

    #region Helper Methods
    private string ShiftBytes(byte[] gameDataBytes, int shift)
    {
        // shift bytes
        for (int i = 0; i < gameDataBytes.Length; i++)
        {
            if (i % 4 != 0) {
                gameDataBytes[i] = (byte) ((gameDataBytes[i] + shift + 256) % 256);
            }
            else
            {
                gameDataBytes[i] = (byte) ((gameDataBytes[i] - shift + 256) % 256);
            }
        }

        // replace all dashes with random numbers
        char[] gameDataBytesShiftedChars = BitConverter.ToString(gameDataBytes).ToCharArray();

        for (int i = 2; i < gameDataBytesShiftedChars.Length; i += 3) 
        {
            gameDataBytesShiftedChars[i] = (char) UnityEngine.Random.Range(48, 57);
        }

        return new string(gameDataBytesShiftedChars);
    }
    #endregion
}