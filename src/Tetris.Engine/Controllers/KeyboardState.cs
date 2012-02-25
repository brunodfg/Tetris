/*
 * Author: Bruno Gonçalves - brunodfg@gmail.com
 * Date: 18/02/2012
 * 
 * Credit to Dave Wheeler - http://www.developerfusion.com/article/84387/game-development-using-silverlight-2/
 * 
 * **/

using System.Windows;
using System.Windows.Input;

namespace brunodfg.tetris.engine.controllers
{
    /// <summary>
    /// Allows to query whether a specific key is being pressed or not
    /// </summary>
    public static class KeyboardState
    {
        private const int MAX_KEYS = 256;
        private static bool[] keysState = new bool[MAX_KEYS];

        #region Public methods

        /// <summary>
        /// Whether the specified key is pressed
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsKeyPressed(Key key)
        {
            int index = (int)key;

            if (index < 0 || index >= MAX_KEYS)
            {
                return false;
            }

            return keysState[index];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uiElement"></param>
        public static void HookEvents(UIElement uiElement)
        {
            if (uiElement != null)
            {
                uiElement.KeyDown += OnKeyDown;
                uiElement.KeyUp += OnKeyUp;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uiElement"></param>
        public static void UnhookEvents(UIElement uiElement)
        {
            if (uiElement != null)
            {
                uiElement.KeyDown -= OnKeyDown;
                uiElement.KeyUp -= OnKeyUp;
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Sets whether the specified key is pressed or not
        /// </summary>
        /// <param name="key"></param>
        /// <param name="pressed"></param>
        private static void SetUnsetKeyState(Key key, bool pressed)
        {
            int index = (int)key;

            if (index < 0 || index >= MAX_KEYS)
            {
                return;
            }

            keysState[index] = pressed;
        }

        #endregion

        #region Event handlers

        private static void OnKeyUp(object sender, KeyEventArgs e)
        {
            SetUnsetKeyState(e.Key, false);
        }

        private static void OnKeyDown(object sender, KeyEventArgs e)
        {
            SetUnsetKeyState(e.Key, true);
        }

        #endregion
    }
}
