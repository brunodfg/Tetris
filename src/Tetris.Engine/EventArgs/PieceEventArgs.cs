/*
 * Author: Bruno Gonçalves - brunodfg@gmail.com
 * Date: 18/02/2012
 * 
 * **/

using System;
using brunodfg.tetris.engine;
using brunodfg.tetris.engine.pieces;

namespace brunodfg.tetris
{
    public class PieceEventArgs : EventArgs
    {
        /// <summary>
        /// The piece related to the event
        /// </summary>
        public Piece Piece{ get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="piece"></param>
        public PieceEventArgs(Piece piece)
        {
            this.Piece = piece;
        }
    }

    public class PieceMovedEventArgs : PieceEventArgs
    {
        /// <summary>
        /// The direction to which the piece has moved
        /// </summary>
        public MoveDirection Direction { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="direction"></param>
        public PieceMovedEventArgs(Piece piece, MoveDirection direction)
            : base(piece)
        {
            this.Direction = direction;
        }
    }

    public class PieceRotatedEventArgs : PieceEventArgs
    {
        /// <summary>
        /// Whether the piece was rotated clockwise or counter-clockwise
        /// </summary>
        public bool Clockwise { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="clockwise"></param>
        public PieceRotatedEventArgs(Piece piece, bool clockwise)
            : base(piece)
        {
            this.Clockwise = clockwise;
        }
    }
}
