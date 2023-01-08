using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace FirebirdGames.Utilities
{
    [DataContract]
    public class Saveable<T>
        where T : new()
    {
        internal bool IsDirty;

        public virtual void Initialize() {}
        
        protected bool SetDirty<T>(ref T input, T value)
        {
            if (!EqualityComparer<T>.Default.Equals(input, value))
            {
                input = value;
                IsDirty = true;
                return true;
            }
            return false;
        }
    }
    
    public class SaveEvents : SingletonObject<SaveEvents>
    {
        public event Action<string> onRequestSave;
        public void RequestSave(string folderName)
        {
            onRequestSave?.Invoke(folderName);
        }
        
        public event Action<string> onRequestLoad;
        public void RequestLoad(string folderName)
        {
            onRequestLoad?.Invoke(folderName);
        }

    }
}
