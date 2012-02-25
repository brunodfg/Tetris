/*
 * Author: Bruno Gonçalves - brunodfg@gmail.com
 * Date: 18/02/2012
 * 
 * **/

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using brunodfg.tetris.engine;

namespace brunodfg.tetris.ui
{
    public partial class GameArea : UserControl
    {
        #region Constants

        /// <summary>
        /// The size in pixels of each game area block
        /// </summary>
        public const int BlockSize = 23;

        #endregion

        #region Private variables

        private TetrisGame game;
        private Dictionary<Block, BlockControl> blockControls;

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public GameArea()
        {
            this.InitializeComponent();

            this.game = new TetrisGame(new TetrisDefinitions());
            this.game.Loop += new EventHandler<EventArgs>(Game_Loop);
            this.game.RowClear += new EventHandler<RowClearedEventArgs>(Game_RowClear);

            this.blockControls = new Dictionary<Block, BlockControl>();

            this.CreateDrawArea();
            this.DrawGameArea();
        }

        #endregion

        #region Private methods

        private void CreateDrawArea()
        {
            #region Create columns and rows for the blocks

            for (int x = 0; x < this.game.Definitions.Width; x++)
            {
                this.TetrisLayout.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(GameArea.BlockSize, GridUnitType.Pixel) });
            }

            for (int y = 0; y < this.game.Definitions.Height; y++)
            {
                this.TetrisLayout.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(GameArea.BlockSize, GridUnitType.Pixel) });
            }

            #endregion

            #region Create game area blocks

            foreach (var block in this.game.Blocks.Values)
            {
                var blockControl = new BlockControl(block);

                Grid.SetColumn(blockControl, block.X);
                Grid.SetRow(blockControl, this.game.Definitions.Height - 1 - block.Y);

                // Index the block controls by their represented block for faster access
                this.blockControls.Add(block, blockControl);

                this.TetrisLayout.Children.Add(blockControl);
            }

            #endregion
        }

        private void UpdateInformationHeader()
        {
            this.txbLevel.Text = "Level: " + this.game.Level.ToString();
            this.txbScore.Text = "Score: " + this.game.Score.ToString();
            this.txbRows.Text = "Rows: " + this.game.ClearedRows.ToString();
        }

        private void DrawGameArea()
        {
            this.UpdateInformationHeader();

            foreach (var kvp in this.blockControls)
            {
                kvp.Value.Draw();
            }

            this.preview1.Piece = this.game.NextPieces.Count > 0 ? this.game.NextPieces[0] : null;
            this.preview2.Piece = this.game.NextPieces.Count > 1 ? this.game.NextPieces[1] : null;
            this.preview3.Piece = this.game.NextPieces.Count > 2 ? this.game.NextPieces[2] : null;
        }

        #endregion

        #region Event handlers

        private void Game_Loop(object sender, EventArgs e)
        {
            this.DrawGameArea();
        }

        private void Game_RowClear(object sender, RowClearedEventArgs e)
        {
            this.game.Pause();
            
            this.game.Rows[e.RowNumber].ForEach(x => this.blockControls[x].Highlight((sender1, e1) =>
            {
                this.game.Resume();
            }));
        }

        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            #region Start / Pause / Resume

            if (e.Key == Key.P)
            {
                if (this.game.State == GameState.Running)
                {
                    this.game.Pause();
                }
                else if (this.game.State == GameState.Paused)
                {
                    this.game.Resume();
                }
                else if (this.game.State == GameState.Stopped)
                {
                    this.game.Start();
                }
            }

            #endregion

            #region End game

            if (e.Key == Key.E)
            {
                this.game.Stop();
                this.DrawGameArea();
            }

            #endregion
        }

        #endregion
    }
}
