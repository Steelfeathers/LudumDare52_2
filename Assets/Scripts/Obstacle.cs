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

        public string MyWord => myWord;
        private string myWord;
        private string progressColorHexCode;

        public bool WasRemoved;

        public void Setup(string word)
        {
            myWord = word;
            progressColorHexCode = progressColor.ToHexString();
            myWordText.text = myWord;
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
            myWordText.text = textStr;
        }

        public void ClearProgress()
        {
            myWordText.text = myWord;
        }

        public void PlayRemoveFX()
        {
            DOTween.Sequence().AppendInterval(AnimUtils.PlayAllChildTweensWithID(gameObject, "Removed")).AppendCallback(() =>
            {
                if (this != null && gameObject != null) gameObject.SetActive(false);
            });
            //TODO FX
        }

        public void PlayDestroyedFX()
        {
            DOTween.Sequence().AppendInterval(AnimUtils.PlayAllChildTweensWithID(gameObject, "Destroyed")).AppendCallback(() =>
            {
                if (this != null && gameObject != null) gameObject.SetActive(false);
            });
            //TODO: FX
        }
    }
}
