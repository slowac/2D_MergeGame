using System.IO;
using System.Runtime.Serialization.Formatters.Binary; 
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[System.Serializable]
public class SaveSystem
{
    public static SaveSystem Instance;

    public List<int> ShopSkinKeyValues = new List<int>();
    public int ShopLastSelectedSkin = 0;

    public int LastPlayedLevelIndex;
    public int BestScore = 0;
    public List<int> UnlockedLevels = new List<int>();

    public int Coins = 0;

    public static void Save()
    {
        string path = Application.persistentDataPath + "/save.dat";
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(path, FileMode.OpenOrCreate);
        bf.Serialize(file, Instance);
        file.Close();
    }

    public static void Load()
    {
        Instance = new SaveSystem();

        string path = Application.persistentDataPath + "/save.dat";
        if (File.Exists(path))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(path, FileMode.Open);
            SaveSystem data = (SaveSystem)bf.Deserialize(file);

            file.Close();
            Instance = data;
        }
        else
        {
            Instance = new SaveSystem();
          
            Save();
        }
    }

#if UNITY_EDITOR

    [MenuItem("SaveSystem/Clear Save File")]
    private static void ClearSave()
    {
        string path = Application.persistentDataPath + "/save.dat";
        if (File.Exists(path))
        {
            File.Delete(path);

        }
    }
     
#endif   
    
}