/*
 * Author: Bruno Gonçalves - brunodfg@gmail.com
 * Date: 18/02/2012
 * 
 * **/

using System.Collections.Generic;
using System.Windows.Media;
using System.Windows;

namespace brunodfg.tetris.engine.pieces
{
    /// <summary>
    /// Represents the Z piece
    ///     **
    ///      **
    /// </summary>
    public class ZPiece : Piece
    {
        #region Public properties

        /// <summary>
        /// A list of points on a 2D matrix containing the coordinates of the parts of this piece.
        /// This list represents the template of this piece
        /// </summary>
        public override List<Point> Template
        {
            get 
            {
                return new List<Point>() { new Point(0, 1), new Point(1, 1), new Point(1, 0), new Point(2, 0) };
            }
        }

        /// <summary>
        /// This point represents the location on the piece's template which should be used as the rotation origin.
        /// </summary>
        protected override Point TemplateRotationOrigin
        {
            get { return new Point(1, 0); }
        }

        /// <summary>
        /// The background color of this piece
        /// </summary>
        public override Color Background
        {
            get { return Colors.Green; }
        }

        #endregion

        #region Constructors
        
        #endregion
    }
}
