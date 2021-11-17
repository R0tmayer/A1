using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager    
{
    //string filePath = Application.persistentDataPath + "/save.gamesave";

    public static void SaveData(MainPlayer mainPlayer, TargetsManager targetsManager,  string saveName)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string filePath = Application.persistentDataPath + "/"+ saveName;
        FileStream stream = new FileStream(filePath, FileMode.Create);

        SaveGameData data = new SaveGameData(mainPlayer, targetsManager);
        formatter.Serialize(stream, data);
        stream.Close();
    }    
    
    public static void DeleteSave()
    {
        string[] files = Directory.GetFiles(Application.persistentDataPath, "*.gamesave");
        foreach (var item in files)
        {
            File.Delete(item);
        }
        
    }

    public static SaveGameData LoadData(string saveName) {
        string filePath = Application.persistentDataPath + "/" + saveName;
        if (File.Exists(filePath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(filePath, FileMode.Open);

            SaveGameData data = formatter.Deserialize(stream) as SaveGameData;
            stream.Close();

            return data;
        }
        else {
            Debug.LogWarning("Нет файла для следующего пути пока не существует: " + filePath);
            return null;
        }
    }

}
