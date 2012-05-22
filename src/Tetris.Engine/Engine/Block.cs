/*
 * Author: Bruno Gonçalves - brunodfg@gmail.com
 * Date: 18/02/2012
 * 
 * **/

using brunodfg.tetris.engine.pieces;
using System;
using System.Windows;

namespace brunodfg.tetris.engine
{
    public class Block
    {
        #region Events
        
        /// <summary>
        /// Occurs when this block becomes filled by a piece
        /// </summary>
        public event EventHandler<EventArgs> Filled;

        /// <summary>
        /// Occurs when this block becomes free
        /// </summary>
        public event EventHandler<EventArgs> Emptied;

        #endregion

        #region Properties

        /// <summary>
        /// The game area to which this block belongs
        /// </summary>
        public TetrisGame GameArea { get; set; }

        /// <summary>
        /// The x coordinate of the block in the game area
        /// </summary>
        public Point Point { get; private set; }

        /// <summary>
        /// The x coordinate of this block's point
        /// </summary>
        public int X { get { return (int)this.Point.X; } }

        /// <summary>
        /// The y coordinate of this block's point
        /// </summary>
        public int Y { get { return (int)this.Point.Y; } }

        /// <summary>
        /// Gets the shape which is filling this specific block. Null if this block is empty
        /// </summary>
        public Piece Piece { get; private set; }

        /// <summary>
        /// Whether this block is currently filled by a piece or a remainder of a piece
        /// </summary>
        public bool IsFilled { get { return this.Piece != null; } }

        /// <summary>
        /// Returns the block which is the left neighbour to this one
        /// </summary>
        public Block LeftNeighbour { get { return this.GameArea.GetBlock(this.X - 1, this.Y); } }

        /// <summary>
        /// Returns the block which is the right neighbour to this one
        /// </summary>
        public Block RightNeighbour { get { return this.GameArea.GetBlock(this.X + 1, this.Y); } }

        /// <summary>
        /// Returns the block which is the top neighbour to this one
        /// </summary>
        public Block TopNeighbour { get { return this.GameArea.GetBlock(this.X, this.Y + 1); } } 

        /// <summary>
        /// Returns the block which is the bottom neighbour to this one
        /// </summary>
        public Block BottomNeighbour { get { return this.GameArea.GetBlock(this.X, this.Y - 1); } }

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameArea"></param>
        /// <param name="x">The x coordinate of the block in the game area</param>
        /// <param name="y">The y coordinate of the block in the game area</param>
        public Block(TetrisGame gameArea, int x, int y)
        {
            this.GameArea = gameArea;
            this.Point = new Point(x, y);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Fills this block with the specified piece
        /// </summary>
        /// <param name="piece"></param>
        public void Fill(Piece piece)
        {
            this.Piece = piece;

            if (this.Filled != null)
            {
                this.Filled(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Releases this block from the filling piece (if any)
        /// </summary>
        public void Empty()
        {
            this.Piece = null;

            if (this.Emptied != null)
            {
                this.Emptied(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}
