using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirebirdGames.Utilities
{
    public interface IInitializable
    {
        void Initialize();
        bool IsLoaded { get; set; }
        bool IsReady { get; set; }
    }
}
