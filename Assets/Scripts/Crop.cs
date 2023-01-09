using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FirebirdGames.Utilities;
using TMPro;
using UnityEngine;

namespace LudumDare52_2
{
    public class Crop : MonoBehaviour
    {
        [SerializeField] private int pointValue = 1;
        [SerializeField] private TextMeshProUGUI harvestFXTest;
        public int PointValue => pointValue;

        [HideInInspector] public bool WasHarvested;

        public void PlayHarvestFX()
        {
            harvestFXTest.text = $"+{pointValue}";
            harvestFXTest.gameObject.SetActive(true);
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
