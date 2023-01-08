using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FirebirdGames.Utilities;
using UnityEngine;

namespace LudumDare52_2
{
    public class Crop : MonoBehaviour
    {
        [SerializeField] private int pointValue = 1;
        public int PointValue => pointValue;

        public bool WasHarvested;

        public void PlayHarvestFX()
        {
            DOTween.Sequence().AppendInterval(AnimUtils.PlayAllChildTweensWithID(gameObject, "Harvested")).AppendCallback(() =>
            {
                if (this != null && gameObject != null) gameObject.SetActive(false);
            });
        }

        public void PlayDestroyedFX()
        {
            DOTween.Sequence().AppendInterval(AnimUtils.PlayAllChildTweensWithID(gameObject, "Destroyed")).AppendCallback(() =>
            {
                if (this != null && gameObject != null) gameObject.SetActive(false);
            });
        }
    }
}
