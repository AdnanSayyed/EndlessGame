using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace EndlessGame.SaveLoad
{
    public class SaveLoadService : ISaveLoadService
    {
        private const string SaveFileName = "savefile.dat";

        public void Save(SaveData data)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            string path = Application.persistentDataPath + "/" + SaveFileName;

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                formatter.Serialize(stream, data);
            }
        }

        public SaveData Load()
        {
            string path = Application.persistentDataPath + "/" + SaveFileName;

            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();

                using (FileStream stream = new FileStream(path, FileMode.Open))
                {
                    return (SaveData)formatter.Deserialize(stream);
                }
            }
            else
            {
                Debug.LogWarning("Save file not found");
                return new SaveData();
            }
        }
    }
}


namespace EndlessGame.SaveLoad
{
    public interface ISaveLoadService
    {
        void Save(SaveData data);
        SaveData Load();
    }
}
