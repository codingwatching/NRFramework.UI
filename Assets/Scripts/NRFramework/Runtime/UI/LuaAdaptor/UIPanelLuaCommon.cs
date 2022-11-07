﻿#if USE_LUA
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace NRFramework
{
    public class UIPanelLuaCommon : UIPanel
    {
        public LuaTable @this;

        private Action<LuaTable> m_LuaOnCreating;
        private Action<LuaTable> m_LuaOnCreated;
        private Action<LuaTable> m_LuaOnClosing;
        private Action<LuaTable> m_LuaOnClosed;

        public void Create(string panelId, Canvas parentCanvas, string prefabPath, LuaTable luaTable)
        {
            @this = luaTable;
            base.Create(panelId, parentCanvas.GetComponent<RectTransform>(), prefabPath);
        }

        public void Create(string panelId, Canvas parentCanvas, UIPanelBehaviour panelBehaviour, LuaTable luaTable)
        {
            @this = luaTable;
            base.Create(panelId, parentCanvas.GetComponent<RectTransform>(), panelBehaviour);
        }

        protected override void OnCreating()
        {
            base.OnCreating();

            SetMember("csPanel", this);
            SetMember("panelId", panelId);
            SetMember("behaviour", panelBehaviour);
            SetMember("rectTransform", rectTransform);
            SetMember("gameObject", gameObject);
            SetMember("parentRectTransform", parentRectTransform);
            SetMember("parentUIRoot", parentUIRoot);
            SetMember("canvas", canvas);
            SetMember("gaphicRaycaster", gaphicRaycaster);

            GetMember("OnCreating", out m_LuaOnCreating);
            GetMember("OnCreated", out m_LuaOnCreated);
            GetMember("OnClosing", out m_LuaOnClosing);
            GetMember("OnClosed", out m_LuaOnClosed);

            m_LuaOnCreating?.Invoke(@this);

        }

        protected override void OnCreated()
        {
            base.OnCreated();
            m_LuaOnCreated?.Invoke(@this);
        }

        protected override void OnClosing()
        {
            m_LuaOnClosing?.Invoke(@this);

            m_LuaOnCreating = null;
            m_LuaOnCreated = null;
            m_LuaOnClosing = null;

            base.OnClosing();
        }

        protected override void OnClosed()
        {
            base.OnClosed();
            m_LuaOnClosed?.Invoke(@this);

            m_LuaOnClosed = null;
        }

        //设置成员 供Lua调C#
        private void SetMember<TKey, TValue>(TKey key, TValue value)
        {
            @this.Set(key, value);
        }

        //获取成员 供C#调Lua
        private void GetMember<TKey, TValue>(TKey key, out TValue value)
        {
            @this.Get(key, out value);
        }
    }
}
#endif