using System;
using UnityEngine;

namespace FirebirdGames.Utilities
{
    public class Module<T, U> : SingletonObject<T>
        where T : new()
        where U : Saveable<U>, new()
    {
        public U Data { get; protected set; }
        public bool IsLoaded { get; set; }

        public virtual void Initialize()
        {
            SaveEvents.Instance.onRequestSave += SaveData;
            SaveEvents.Instance.onRequestLoad += LoadData;
            Data = new U();
        }

        public virtual void LoadData(string folderName="")
        {
            Data = SerializationManager.Load<U>(folderName, typeof(U).Name);
            if (Data == null)
            {
                Data = new U();
                Data.Initialize();
                Debug.LogWarning($"{typeof(U).Name} loading failed - no data found to load, initializing new data container");
            }
            IsLoaded = true;
        }

        public virtual void SaveData(string folderName="")
        {
            if (Data == null)
            {
                Debug.LogError($"typeof(U).Name saving failed - class is null!");
                return;
            }
            
            if (!Data.IsDirty) return;
            Data.IsDirty = false;
            
            string fileName = typeof(U).Name;
            bool saved = SerializationManager.Save(Data, folderName, fileName);
            
            if (saved)   Debug.Log($"{fileName} successfully saved!");
            else         Debug.LogError($"{fileName} saving failed!");
        }
    }
}
