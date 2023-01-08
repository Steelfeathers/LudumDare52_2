using System;
using System.Collections;
using System.Collections.Generic;
using FirebirdGames.Utilities.UI;
using TMPro;
using UnityEngine;

namespace LudumDare52_2
{
    public class GameOverDialog : DialogPopup
    {
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private GameObject nextLevelButton;

        public override void Initialize(object config = null, Action onClosedCallback = null)
        {
            base.Initialize(config, onClosedCallback);
            scoreText.text = $"Final Score: {MyGameRoot.Instance.CurScore}";
            levelText.text = $"Level {MyGameRoot.Instance.CurLevel}";
            
            if (nextLevelButton != null && MyGameRoot.Instance.CurLevel >= MyGameRoot.Instance.MaxLevel)
                nextLevelButton.SetActive(false);
        }

        public void OnReturnToMenu()
        {
            MyGameRoot.Instance.LoadMenu();
        }

        public void OnRetry()
        {
            MyGameRoot.Instance.LoadGame();
        }

        public void OnNextLevel()
        {
            MyGameRoot.Instance.CurLevel += 1;
            MyGameRoot.Instance.LoadGame();
        }
    }
}
