using System.Collections;
using System.Collections.Generic;
using FirebirdGames.Utilities;
using FirebirdGames.Utilities.UI;
using UnityEngine;

namespace LudumDare52_2
{
    public class MyGameRoot : GameRoot<MyGameRoot>
    {
        [SerializeField] private Dialog titleDialogPrefab;
        [SerializeField] private Dialog gameDialogPrefab;
        [SerializeField] private Dialog gameVictoryDialogPrefab;
        [SerializeField] private Dialog gameLossDialogPrefab;
        [SerializeField] private Dialog tutorialPrefab;

        public Dialog GameVictoryDialogPrefab => gameVictoryDialogPrefab;
        public Dialog GameLossDialogPrefab => gameLossDialogPrefab;

        public int CurLevel;
        
        private Dialog gameDialog;

        protected override void OnBoostrapLoadComplete()
        {
            base.OnBoostrapLoadComplete();
            LoadMenu();
        }

        public void LoadMenu()
        {
            if (gameDialog != null) UIManager.Instance.HideDialog(gameDialog);
            gameDialog = null;
            
            UIManager.Instance.ShowBaseDialog(titleDialogPrefab, true);
        }

        public void LoadGame()
        {
            if (gameDialog != null) UIManager.Instance.HideDialog(gameDialog);
            gameDialog = null;
            
            UIManager.Instance.ShowBaseDialog(gameDialogPrefab, true);
        }

    }
}
