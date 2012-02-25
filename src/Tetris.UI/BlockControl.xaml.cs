/*
 * Author: Bruno Gonçalves - brunodfg@gmail.com
 * Date: 18/02/2012
 * 
 * **/

using System;
using System.Windows.Media;
using System.Windows.Media.Animation;
using brunodfg.tetris.engine;

namespace brunodfg.tetris.ui
{
    public partial class BlockControl
    {
        #region Properties

        /// <summary>
        /// The Tetris block being represented by this UI block
        /// </summary>
        public Block Block { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="block"></param>
        public BlockControl(Block block)
        {
            this.InitializeComponent();

            this.Block = block;
        }

        #endregion

        #region Event handlers

        /// <summary>
        /// Updates the current UserControl with the colors of the block being represented
        /// </summary>
        public void Draw()
        {
            if (this.Block.IsFilled)
            {
                this.RowClearedHighlight.Background = new SolidColorBrush(this.Block.Piece.Background);
                this.PieceBorder.Background = new SolidColorBrush(this.Block.Piece.Background);
            }
            else
            {
                this.PieceBorder.Background = new SolidColorBrush(Colors.Transparent);
            }
        }

        /// <summary>
        /// Begins the highlight animation when the row of this block is cleared
        /// </summary>
        /// <param name="animationCompleted"></param>
        public void Highlight(EventHandler<EventArgs> animationCompleted)
        {
            var storyboard = this.Resources["RowClearedStoryboard"] as Storyboard;

            storyboard.Completed += new EventHandler(animationCompleted);
            storyboard.Begin();
        }

        #endregion
    }
}
