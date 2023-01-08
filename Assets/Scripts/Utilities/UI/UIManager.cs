using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace FirebirdGames.Utilities.UI
{
    public class UIEvents : SingletonObject<UIEvents>
    {
        public event Action<Canvas> onDialogShown;
        public void SendOnDialogShown(Canvas canvas)
        {
            onDialogShown?.Invoke(canvas);
        }
        
        public event Action<Canvas> onDialogHidden;
        public void SendOnDialogHidden(Canvas canvas)
        {
            onDialogHidden?.Invoke(canvas);
        }
    }
    
    [DisallowMultipleComponent]
    public class UIManager : SingletonComponent<UIManager>, IInitializable
    {
        public Canvas BackgroundCanvas;
        public Canvas BaseCanvas;
        public Canvas OverlayCanvas;
        public Canvas ForegroundCanvas;

        private Dialog currBackgroundDialog;
        private Dialog currForegroundDialog;
        private List<Dialog> baseDialogStack = new List<Dialog>();
        private List<Dialog> overlayDialogStack = new List<Dialog>();

        private int transitionCount;
        public bool IsTransitioning => transitionCount > 0;

        protected override void Awake()
        {
            base.Awake();
            IsLoaded = true;
        }
        public void Initialize()
        {
            IsReady = true;
        }

        //-------------------------------------------------------------------------------------
        /// <summary>
        /// Spawn the passed in dialog, either adding it to the top of the stack or replacing the topmost dialog
        /// </summary>
        public Dialog ShowBaseDialog(Dialog _dialog, bool replace = false, object config = null, Action onClosedCallback=null)
        {
            var d = ShowDialog(_dialog, BaseCanvas, baseDialogStack, replace, config, onClosedCallback);
            UIEvents.Instance.SendOnDialogShown(BaseCanvas);
            return d;
        }
        /// <summary>
        /// Spawn the passed in dialog, either adding it to the top of the stack or replacing the topmost dialog
        /// </summary>
        public Dialog ShowOverlayDialog(Dialog _dialog, bool replace = false, object config = null, Action onClosedCallback=null)
        {
            var d = ShowDialog(_dialog, OverlayCanvas, overlayDialogStack, replace, config, onClosedCallback);
            UIEvents.Instance.SendOnDialogShown(OverlayCanvas);
            return d;
        }

        public Dialog ShowBackgroundDialog(Dialog _dialog)
        {
            currBackgroundDialog = ShowDialog(_dialog, BackgroundCanvas, null, true);
            return currBackgroundDialog;
        }

        public Dialog ShowForegroundDialog(Dialog _dialog)
        {
            currForegroundDialog = ShowDialog(_dialog, ForegroundCanvas, null, true);
            return currForegroundDialog;
        }

        private Dialog ShowDialog(Dialog _dialog, Canvas targetCanvas, List<Dialog> _dialogStack=null, bool replace = false, object config = null, Action onClosedCallback=null)
        {
            if (_dialogStack != null && _dialogStack.Contains(_dialog))
                return null;

            transitionCount += 1;

            float delay = 0f;
            if (_dialogStack != null && replace && _dialogStack.Count > 0)
            {
                delay += HideDialog(_dialogStack.Last(), _dialogStack);
            }

            GameObject dialogInstanceObj = GameObject.Instantiate(_dialog.gameObject, targetCanvas.transform);
            dialogInstanceObj.transform.SetAsLastSibling();
            dialogInstanceObj.GetComponent<CanvasGroup>().alpha = 0f;

            var dialog = dialogInstanceObj.GetComponent<Dialog>();
            if (_dialogStack != null) _dialogStack.Add(dialog);

            dialog.Initialize(config, onClosedCallback);

            delay -= (delay * dialog.TransitionOverlap);

            DOTween.Sequence().AppendInterval(delay)
                .AppendCallback(() =>
                {
                    dialogInstanceObj.GetComponent<CanvasGroup>().alpha = 1f;
                    var animInDelay = dialog.AnimateIn();

                    DOTween.Sequence().AppendInterval(animInDelay).AppendCallback(() => transitionCount -= 1);
                }
            );

            return dialog;
        }

        public float HideForegroundDialog(bool animate = true, bool doRemoveFromStack = true)
        {
            return HideDialog(currForegroundDialog, null, animate, doRemoveFromStack);
        }

        public float HideDialog(Dialog _dialog, bool animate = true)
        {
            if (_dialog == null) return 0f;

            if (_dialog == currForegroundDialog)
                return HideDialog(_dialog, null, animate);

            int index = -1;
            List<Dialog> _dialogStack = null;

            if (baseDialogStack != null && baseDialogStack.Count > 0)
            {
                index = baseDialogStack.IndexOf(_dialog);
                if (index > -1) _dialogStack = baseDialogStack;
            }
            if (index == -1 && overlayDialogStack != null && overlayDialogStack.Count > 0)
            {
                index = overlayDialogStack.IndexOf(_dialog);
                if (index > -1) _dialogStack = overlayDialogStack;
            }

            if (_dialogStack != null)
                return HideDialog(_dialog, _dialogStack, animate);

            return 0f;
        }

        private float HideDialog(Dialog _dialog, List<Dialog> _dialogStack = null, bool animate = true, bool doRemoveFromStack = true)
        {
            float delay = 0;
            if (_dialogStack != null && !_dialogStack.Contains(_dialog)) 
                return delay;

            transitionCount += 1;

            if (_dialogStack != null && doRemoveFromStack) 
                _dialogStack.Remove(_dialog);

            if (animate)
                delay = _dialog.AnimateOut();

            DOTween.Sequence().AppendInterval(delay)
                .OnComplete(() => 
                { 
                    GameObject.Destroy(_dialog.gameObject); 
                    transitionCount -= 1; 
                    if (_dialogStack == baseDialogStack) UIEvents.Instance.SendOnDialogHidden(BaseCanvas);
                    else if (_dialogStack == overlayDialogStack) UIEvents.Instance.SendOnDialogHidden(OverlayCanvas);
                }).Play();

            return delay;
        }

        public float HideAllOverlayDialogs(bool animate = true)
        {
            return HideAllDialogs(overlayDialogStack, animate);
        }

        private float HideAllDialogs(List<Dialog> _dialogStack, bool animate = true)
        {
            float delay = 0f;
            if (_dialogStack == null || _dialogStack.Count == 0)
                return delay;

            for (int i = 0; i < _dialogStack.Count; i++)
            {
                var dialog = _dialogStack[i];
                float d = HideDialog(dialog, _dialogStack, animate, false);
                if (d > delay) delay = d;
            }

            _dialogStack.Clear();
            return delay;
        }

        public void BlockAllInput(bool block)
        {
            BackgroundCanvas.GetComponent<GraphicRaycaster>().enabled = !block;
            BaseCanvas.GetComponent<GraphicRaycaster>().enabled = !block;
            OverlayCanvas.GetComponent<GraphicRaycaster>().enabled = !block;
            ForegroundCanvas.GetComponent<GraphicRaycaster>().enabled = !block;
        }

        public void BlockBaseCanvasInput(bool block)
        {
            BaseCanvas.GetComponent<GraphicRaycaster>().enabled = !block;
        }

        public bool IsOverlayDialogShowing => overlayDialogStack != null && overlayDialogStack.Count > 0;
        

        public bool IsLoaded { get; set; }
        public bool IsReady { get; set; }
    }
}
