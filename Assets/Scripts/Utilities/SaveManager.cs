using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirebirdGames.Utilities
{
    public class SaveManager : SingletonComponent<SaveManager>, IInitializable
    {
        [SerializeField] private bool useAutosave;
        public bool UseAutosave => useAutosave;

        private int curSaveSlotIndex = 0;
        public int CurSaveSlotIndex => curSaveSlotIndex;

        protected override void Awake()
        {
            base.Awake();
            IsLoaded = true;
        }
        
        public void Initialize()
        {
            IsReady = true;
        }

        public void SaveGame()
        {
            SaveEvents.Instance.RequestSave(curSaveSlotIndex.ToString());
        }

        public void LoadGame(int targetSaveSlotIndex=0)
        {
            curSaveSlotIndex = targetSaveSlotIndex;
            SaveEvents.Instance.RequestLoad(curSaveSlotIndex.ToString());
        }

        private void LateUpdate()
        {
            //Autosave game data
            if (!useAutosave) return;
            SaveEvents.Instance.RequestSave(curSaveSlotIndex.ToString());
        }
        
        public bool IsLoaded { get; set; }
        public bool IsReady { get; set; }
    }
}
