using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FirebirdGames.Utilities;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace LudumDare52_2
{
    public class Obstacle : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI myWordText;
        [SerializeField] private Color progressColor;
        [SerializeField] private ParticleSystem sparksFX;
        [SerializeField] private ParticleSystem sparksFX2;
        [SerializeField] private ParticleSystem deathFX;
        [SerializeField] private ParticleSystem ouchFX;

        public string MyWord => myWord;
        private string myWord;
        private string progressColorHexCode;

        [HideInInspector] public bool WasRemoved;

        public void Setup(string word)
        {
            myWord = word;
            progressColorHexCode = progressColor.ToHexString();
            myWordText.SetText(myWord);
        }

        public void UpdateProgress(int charCount)
        {
            var charArray = myWord.ToCharArray();
            string textStr = string.Empty;

            for (int i = 0; i < charArray.Length; i++)
            {
                if (i < charCount)
                {
                    textStr += $"<color=#{progressColorHexCode}>" + charArray[i] + "</color>";
                }
                else
                {
                    textStr += charArray[i];
                }
            }
            //Debug.Log("Updating text for: " + myWord);
            myWordText.SetText(textStr);
            AnimUtils.PlayAllChildTweensWithID(gameObject, "Damaged");
        }

        public void ClearProgress()
        {
            myWordText.SetText(myWord);
        }

        public void PlayRemoveFX()
        {
            sparksFX.gameObject.SetActive(true);
            deathFX.gameObject.SetActive(true);
            sparksFX.Play();
            deathFX.Play();
            AnimUtils.PlayAllChildTweensWithID(gameObject, "Removed");
            DOTween.Sequence().AppendInterval(3).AppendCallback(() =>
            {
                if (this != null && gameObject != null)
                {
                    gameObject.SetActive(false);
                }
            });
        }

        public void PlayDestroyedFX()
        {
            sparksFX2.gameObject.SetActive(true);
            ouchFX.gameObject.SetActive(true);
            sparksFX2.Play();
            ouchFX.Play();
            AnimUtils.PlayAllChildTweensWithID(gameObject, "Removed");
            DOTween.Sequence().AppendInterval(3).AppendCallback(() =>
            {
                if (this != null && gameObject != null)
                {
                    gameObject.SetActive(false);
                }
            });
            
        }
    }
}
