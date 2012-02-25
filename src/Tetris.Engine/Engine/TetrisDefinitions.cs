/*
 * Author: Bruno Gonçalves - brunodfg@gmail.com
 * Date: 18/02/2012
 * 
 * **/

using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace brunodfg.tetris.engine
{
    public class TetrisDefinitions
    {
        #region Properties

        /// <summary>
        /// The width of the game area
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// The height of the game area
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// The number of pieces the user may preview
        /// </summary>
        public int PreviewPiecesCount { get; set; }
    
        /// <summary>
        /// The key to move the current piece to the left
        /// </summary>
        public Key MoveLeftKey { get; set; }

        /// <summary>
        /// The key to move the current piece to the right
        /// </summary>
        public Key MoveRightKey { get; set; }

        /// <summary>
        /// The key to move the current piece down
        /// </summary>
        public Key MoveDownKey { get; set; }

        /// <summary>
        /// The key to slide the current piece to the bottom
        /// </summary>
        public Key SlideDownKey { get; set; }

        /// <summary>
        /// The key to rotate the current piece clockwise
        /// </summary>
        public Key RotateClockwiseKey { get; set; }

        /// <summary>
        /// The key to rotate the current piece clockwise
        /// </summary>
        public Key RotateCounterClockwiseKey { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public TetrisDefinitions()
        {
            this.Width = 10;
            this.Height = 20;
            this.PreviewPiecesCount = 3;

            this.MoveLeftKey = Key.Left;
            this.MoveRightKey = Key.Right;
            this.MoveDownKey = Key.Down;
            this.SlideDownKey = Key.S;
            this.RotateCounterClockwiseKey = Key.Up;
            this.RotateClockwiseKey = Key.C;
        }

        #endregion
    }
}
