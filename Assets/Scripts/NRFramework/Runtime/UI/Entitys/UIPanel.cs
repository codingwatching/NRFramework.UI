﻿using System;
using UnityEngine;
using UnityEngine.UI;

namespace NRFramework
{
    public enum UIPanelShowState { Initing, Idle, Refreshing, Closed }

    public enum UIPanelAnimState { Opening, Idle, Closing, Closed }

    public abstract class UIPanel : UIView
    {
        public string panelId { get { return viewId; } }
        public UIPanelBehaviour panelBehaviour { get { return (UIPanelBehaviour)viewBehaviour; } }

        public UIRoot parentUIRoot;
        public Canvas canvas;
        public GraphicRaycaster gaphicRaycaster;

        public UIPanelShowState showState { protected set; get; }

        public UIPanelAnimState animState { protected set; get; }

        internal void Create(string panelId, UIRoot uiRoot, string prefabPath)
        {
            this.parentUIRoot = uiRoot;
            base.Create(panelId, UIManager.Instance.uiCanvas.GetComponent<RectTransform>(), prefabPath);

            PlayOpenAnim(null);
        }

        internal void Create(string panelId, UIRoot uiRoot, UIPanelBehaviour panelBehaviour)
        {
            this.parentUIRoot = uiRoot;
            base.Create(panelId, UIManager.Instance.uiCanvas.GetComponent<RectTransform>(), panelBehaviour);

            PlayOpenAnim(null);
        }

        internal void Close(Action onFinish = null)
        {
            PlayCloseAnim(() => 
            { 
                base.Close(); 
                onFinish?.Invoke(); 
            });
        }

        internal void CloseWithoutAnim()
        {
            base.Close();
        }

        protected void CloseSelf(Action onFinish = null)
        {
            parentUIRoot.ClosePanel(panelId);
        }

        protected void CloseSelfWithoutAnim()
        {
            parentUIRoot.ClosePanelWithoutAnim(panelId);
        }

        internal void SetSortingOrder(int sortingOrder)
        {
            canvas.sortingOrder = sortingOrder;
        }

        #region 打开关闭动画接口
        protected virtual void PlayOpenAnim(Action onFinish = null)
        {
            if (panelBehaviour.ExistValidAnimator() && panelBehaviour.openAnimPlayMode == UIPanelOpenAnimPlayMode.AutoPlay)
            {
                Debug.Assert(animState != UIPanelAnimState.Opening && animState != UIPanelAnimState.Closing);

                animState = UIPanelAnimState.Opening;
                panelBehaviour.PlayOpenAnim(() => { animState = UIPanelAnimState.Idle; onFinish?.Invoke(); });
            }
            else
            {
                animState = UIPanelAnimState.Idle;
                onFinish?.Invoke();
            }
        }

        protected virtual void PlayCloseAnim(Action onFinish = null)
        {
            if (panelBehaviour.ExistValidAnimator() && panelBehaviour.openAnimPlayMode == UIPanelOpenAnimPlayMode.AutoPlay)
            {
                Debug.Assert(animState != UIPanelAnimState.Opening && animState != UIPanelAnimState.Closing);

                animState = UIPanelAnimState.Closing;
                panelBehaviour.PlayOpenAnim(() => { animState = UIPanelAnimState.Closed; onFinish?.Invoke(); });
            }
            else
            {
                animState = UIPanelAnimState.Closed;
                onFinish?.Invoke();
            }
        }
        #endregion

        internal void ChangeFocus(bool got)
        {
            OnFocusChanged(got);
        }

        protected internal override void OnInternalCreating()
        {
            base.OnInternalCreating();

            canvas = panelBehaviour.gameObject.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            gaphicRaycaster = panelBehaviour.gameObject.AddComponent<GraphicRaycaster>();
        }

        protected internal override void OnInternalCreated() 
        {
            showState = UIPanelShowState.Idle;
            animState = UIPanelAnimState.Idle;
        }

        protected internal override void OnInternalClosing()
        {
            //组件引用解除即可, 实例会随gameObject销毁
            gaphicRaycaster = null;
            canvas = null;
            parentUIRoot = null;

            base.OnInternalClosing();
        }

        protected internal override void OnInternalClosed() 
        {
            showState = UIPanelShowState.Closed;
        }

        #region 子类生命周期
        protected virtual void OnFocusChanged(bool got) { }

        protected virtual void OnEscButtonClicked() { }

        protected virtual void OnWindowBgClicked() { }

        #endregion
    }
}


