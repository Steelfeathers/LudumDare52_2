using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using UnityEngine;

namespace FirebirdGames.Utilities
{
    public class SerializationManager
    {
        public static bool Save<T>(T saveData, string folderName = "", string fileName = "game")
        {
            string folderPath = GetSaveFolderPath(folderName);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string filePath = GetSaveFilePath(folderName, fileName);

            try
            {
                var stream = new MemoryStream();
                var ser = new DataContractJsonSerializer(typeof(T));
                ser.WriteObject(stream, saveData);
                byte[] json = stream.ToArray();
                stream.Close();
            
                File.WriteAllText(filePath, Encoding.UTF8.GetString(json, 0, json.Length));
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("Error saving file at {0}", filePath);
                return false;
            }

            return true;
        }

        public static T Load<T>(string folderName = "", string fileName = "game")
        {
            string folderPath = GetSaveFolderPath(folderName);

            if (!Directory.Exists(folderPath))
                return default(T);

            string filePath = GetSaveFilePath(folderName, fileName);

            if (!File.Exists(filePath))
                return default(T);

            try
            {
                string text = File.ReadAllText(filePath);
                var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
                var ser = new DataContractJsonSerializer(typeof(T));
                var loadedData = (T)ser.ReadObject(stream);
                stream.Close();

                return loadedData;
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("Error loading file at {0}", filePath);
                return default(T);
            }
            
        }

        public static bool Delete(string folderName = "")
        {
            string folderPath = GetSaveFolderPath(folderName);

            if (!Directory.Exists(folderPath))
                return false;

            var files = Directory.GetFiles(folderPath);
            foreach (var file in files)
            {
                File.Delete(file);
            }

            Directory.Delete(folderPath);

            return true;
        }

        private static string GetSaveFolderPath(string folderName)
        {
            string folderPath = Application.persistentDataPath + "/data";
            if (!string.IsNullOrEmpty(folderName))
                folderPath += "/" + folderName;

            return folderPath;
        }

        private static string GetSaveFilePath(string folderName, string fileName)
        {
            string filePath = GetSaveFolderPath(folderName);
            filePath += "/" + fileName + ".dat";

            return filePath;
        }

        public static bool HasSaveData(string folderName)
        {
            string folderPath = GetSaveFolderPath(folderName);
            if (!Directory.Exists(folderPath))
                return false;

            if (Directory.GetFiles(folderPath, "*.dat").Length == 0)
                return false;

            return true;

        }

    }
}
