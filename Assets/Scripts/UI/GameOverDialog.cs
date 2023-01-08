using System.Collections;
using System.Collections.Generic;
using FirebirdGames.Utilities.UI;
using UnityEngine;

namespace LudumDare52_2
{
    public class GameOverDialog : DialogPopup
    {
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
