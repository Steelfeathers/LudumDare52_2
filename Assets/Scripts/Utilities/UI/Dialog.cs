using System;
using System.Collections;
using UnityEngine;

namespace FirebirdGames.Utilities.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Dialog : MonoBehaviour
    {
        [Header("Animation")]
        private float animateInTime;
        private float animateOutTime;
        [Range(0f, 1f)] public float TransitionOverlap;

        protected Action onClosedCallback;

        public virtual void Initialize(object config = null, Action onClosedCallback=null)
        {
            this.onClosedCallback = onClosedCallback;
        }

        public virtual float AnimateIn()
        {
            StartCoroutine(DoAnimateIn());
            return animateInTime;
        }

        private IEnumerator DoAnimateIn()
        {
            animateInTime = AnimUtils.PlayAllChildTweensWithID(gameObject, "AnimateIn");

            yield return new WaitForSeconds(animateInTime);
            AnimateInComplete();
        }

        public virtual void AnimateInComplete()
        {
        }

        public virtual float AnimateOut()
        {
            StartCoroutine(DoAnimateOut());
            return animateOutTime;
        }

        private IEnumerator DoAnimateOut()
        {
            animateOutTime = AnimUtils.PlayAllChildTweensWithID(gameObject, "AnimateOut");

            yield return new WaitForSeconds(animateOutTime);
            AnimateOutComplete();
        }

        public virtual void AnimateOutComplete()
        {
        }

        public virtual void OnCloseClicked()
        {
            onClosedCallback?.Invoke();
            UIManager.Instance.HideDialog(this);
        }
    }
}
