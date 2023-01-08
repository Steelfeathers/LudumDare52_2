using System.Collections;
using System.Collections.Generic;
using FirebirdGames.Utilities;
using FirebirdGames.Utilities.UI;
using UnityEngine;

namespace LudumDare52_2
{
    public class TitleDialog : Dialog
    {
        public void OnStartGame()
        {
            MyGameRoot.Instance.LoadGame();
            OnCloseClicked();
        }

        public void OnExitGame()
        {
            Utils.QuitApplication();
        }
    }
}
