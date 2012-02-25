/*
 * Author: Bruno Gonçalves - brunodfg@gmail.com
 * Date: 18/02/2012
 * 
 * **/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace brunodfg.tetris.engine.pieces
{
    /// <summary>
    /// Represents a tetris piece. Pieces are: I, J, L, O, S, T, Z
    /// Each piece defines its shape and specifies which TetrisGame blocks is filling at any given moment.
    /// </summary>
    public abstract class Piece
    {
        #region Static variables

        private static int PieceCount = 0;

        #endregion

        #region Events

        /// <summary>
        /// Occurs when a piece moves down, left or right
        /// </summary>
        public event EventHandler<PieceMovedEventArgs> Moved;

        /// <summary>
        /// Occurs when a piece rotates
        /// </summary>
        public event EventHandler<PieceRotatedEventArgs> Rotated;

        #endregion

        #region Properties

        public string Id { get; set; }

        /// <summary>
        /// A list of points on a 2D matrix containing the coordinates of the parts of this piece.
        /// This list represents the template of this piece
        /// </summary>
        public abstract List<Point> Template { get; }

        /// <summary>
        /// This point represents the location on the piece's template which should be used as the rotation origin.
        /// </summary>
        protected abstract Point TemplateRotationOrigin { get; }

        /// <summary>
        /// The size of the largest axis of this piece
        /// </summary>
        public int Size 
        { 
            get 
            {
                var maxX = this.Template.Max(p => p.X) + 1;
                var maxY = this.Template.Max(p => p.Y) + 1;

                return Math.Max(maxX, maxY);
            } 
        }

        /// <summary>
        /// The background color of this piece
        /// </summary>
        public abstract Color Background { get; }
        
        /// <summary>
        /// The game area to which this block belongs
        /// </summary>
        protected TetrisGame GameArea { get; private set; }

        /// <summary>
        /// The block from the list of currently filled blocks which represents the rotation origin
        /// </summary>
        public Block CurrentRotationOriginBlock { get; set; }

        /// <summary>
        /// The list of blocks which this piece is currently filling
        /// </summary>
        public List<Block> FilledBlocks
        {
            get
            {
                var blocks = new List<Block>();

                if (this.GameArea != null)
                {
                    blocks = this.GameArea.Blocks.Values.Where(x => x.Piece == this).ToList();
                }

                return blocks;
            }
        }

        /// <summary>
        /// Whether the current piece still has all belonging parts
        /// </summary>
        public bool IsComplete
        {
            get
            {
                return this.FilledBlocks.Count == this.Template.Count;
            }
        }

        /// <summary>
        /// When a piece has been broken in several subpieces (not tied together)
        /// this list returns the blocks which the several sub-pieces are filling
        /// If the piece is complete (not broken in several subpieces) the list returns one position with 
        /// the same content as FilledBlocks
        /// </summary>
        public List<List<Block>> FilledRegions
        {
            get
            {
                var queue = new List<Block>(this.FilledBlocks);
                var regions = new List<List<Block>>();

                while (queue.Count > 0)
                {
                    var region = this.GetRegion(queue[0]);
                    region.ForEach(b => queue.Remove(b));

                    regions.Add(region);
                }

                return regions;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public Piece()
        {
            if (this.Template == null || this.Template.Count == 0)
            {
                throw new ArgumentNullException("DefaultPieceLocations", "The size of DefaultPieceLocations must be greater than 0");
            }

            if (this.TemplateRotationOrigin == null || this.Template.FirstOrDefault(p => p.X == this.TemplateRotationOrigin.X && p.Y == this.TemplateRotationOrigin.Y) == null)
            {
                throw new ArgumentNullException("RotationOrigin", "Please specify a rotation origin for this piece belonging to the template");
            }

            this.Id = (++PieceCount).ToString();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameArea"></param>
        public virtual void JoinGame(TetrisGame gameArea)
        {
            this.GameArea = gameArea;

            #region Validations

            if (this.GameArea == null)
            {
                throw new ArgumentNullException("gameArea");
            }

            if (this.TemplateRotationOrigin == null)
            {
                throw new ArgumentNullException("RotationCenterLocation");
            }

            if (this.FilledBlocks == null || this.FilledBlocks.Count() == 0)
            {
                throw new Exception("The piece is not present on the game area");
            }

            #endregion

            #region Determine initial rotation origin block based on template

            // Based on the piece's template and on the template's rotation origin,
            // determine which of the currently filled blocks is the origin of rotation
            
            var minXFilledBlock = this.FilledBlocks.Select(b => b.X).Min();
            var minYFilledBlock = this.FilledBlocks.Select(b => b.Y).Min();

            var minXTemplateOrigin = this.Template.Select(l => l.X).Min();
            var minYTemplateOrigin = this.Template.Select(l => l.Y).Min();

            var rotationBlockX = this.TemplateRotationOrigin.X + (minXFilledBlock - minXTemplateOrigin);
            var rotationBlockY = this.TemplateRotationOrigin.Y + (minYFilledBlock - minYTemplateOrigin);

            this.CurrentRotationOriginBlock = this.FilledBlocks.FirstOrDefault(b => b.X == rotationBlockX && b.Y == rotationBlockY);

            #endregion
        }

        /// <summary>
        /// Moves the piece one position in the specified direction
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public bool Move(MoveDirection direction)
        {
            var start = DateTime.Now;
            var pieceMoved = false;

            var pieceRegions = this.FilledRegions;

            foreach (var region in pieceRegions)
            {
                var destinationBlocks = this.GetBlocksToFillOnMove(region, direction);

                if (this.SwapBlocksContent(region, destinationBlocks))
                {
                    pieceMoved = true;
                    this.OnMove(direction);
                }
            }

            return pieceMoved;
        }

        /// <summary>
        /// Moves the piece down until it can no longer move
        /// </summary>
        /// <returns></returns>
        public virtual bool Slide()
        {
            var pieceSlided = false;

            while (this.Move(MoveDirection.Down))
            {
                pieceSlided = true;
            };

            return pieceSlided;
        }

        /// <summary>
        /// Rotates the piece 90º degrees clockwise
        /// </summary>
        /// <param name="clockwise"></param>
        /// <returns></returns>
        public virtual bool Rotate(bool clockwise)
        {
            var pieceRotated = false;

            var destinationBlocks = this.GetBlocksToFillOnRotate(this.FilledBlocks, this.CurrentRotationOriginBlock, clockwise);

            if (pieceRotated = this.SwapBlocksContent(this.FilledBlocks, destinationBlocks))
            {
                this.OnRotate(clockwise);
            }

            return pieceRotated;
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Occurs when a piece is moved
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="direction"></param>
        protected virtual void OnMove(MoveDirection direction)
        {
            if (this.Moved != null)
            {
                this.Moved(this, new PieceMovedEventArgs(this, direction));
            }

            this.UpdateRotationOrigin(direction);
        }

        /// <summary>
        /// Occurs when a piece is rotated
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="clockwise"></param>
        protected virtual void OnRotate(bool clockwise)
        {
            if (this.Rotated != null)
            {
                this.Rotated(this, new PieceRotatedEventArgs(this, clockwise));
            }
        }

        #endregion

        #region Private methods

        private List<Block> GetBlocksToFillOnMove(List<Block> sourceBlocks, MoveDirection direction)
        {
            var adjacentBlocks = new List<Block>();

            if (sourceBlocks != null)
            {
                foreach (var block in sourceBlocks)
                {
                    switch (direction)
                    {
                        case MoveDirection.Left:
                            adjacentBlocks.Add(block.LeftNeighbour);
                            break;

                        case MoveDirection.Right:
                            adjacentBlocks.Add(block.RightNeighbour);
                            break;

                        case MoveDirection.Down:
                            adjacentBlocks.Add(block.BottomNeighbour);
                            break;
                    }
                }
            }

            return adjacentBlocks.Where(b => b != null).ToList();
        }

        private List<Block> GetBlocksToFillOnRotate(List<Block> sourceBlocks, Block rotationOrigin, bool clockwise)
        {
            var blocksToFill = new List<Block>();

            // Validate that the current rotation block is within the source blocks
            if (rotationOrigin != null && sourceBlocks != null && sourceBlocks.Contains(rotationOrigin))
            {
                blocksToFill.Clear();

                // x' = x * cos(PI/2) - y * sin(PI/2)
                // y' = x * sin(PI/2) + y * cos(PI/2)
                foreach (var block in sourceBlocks)
                {
                    // Get translate coordinates as if the piece was centered on its rotation origin block
                    var translatedX = block.X - rotationOrigin.X;
                    var translatedY = block.Y - rotationOrigin.Y;
                    var angleDirection = clockwise ? -1 : 1;

                    // Rotate and translate to initial position
                    var newX = (int)Math.Round(translatedX * Math.Cos(angleDirection * Math.PI / 2) - translatedY * Math.Sin(angleDirection * Math.PI / 2)) + rotationOrigin.X;
                    var newY = (int)Math.Round(translatedX * Math.Sin(angleDirection * Math.PI / 2) + translatedY * Math.Cos(angleDirection * Math.PI / 2)) + rotationOrigin.Y;

                    blocksToFill.Add(this.GameArea.GetBlock(newX, newY));
                }
            }

            return blocksToFill.Where(b => b != null).ToList();
        }

        private bool CanSwapBlocksContent(List<Block> sourceBlocks, List<Block> destinationBlocks)
        {
            var canMove = false;

            // Source and destination share the same ammount of blocks
            if (sourceBlocks != null && destinationBlocks != null && sourceBlocks.Count() > 0 && sourceBlocks.Count() == destinationBlocks.Count())
            {
                var piece = sourceBlocks[0].Piece;

                // Source blocks contain all the same piece
                if (sourceBlocks.All(b => b.IsFilled) && sourceBlocks.Select(b => b.Piece).Distinct().Count() == 1)
                {
                    // Destination blocks are empty or if not they are one of the source blocks
                    canMove = destinationBlocks.All(b => !b.IsFilled || sourceBlocks.Contains(b));
                }
            }

            return canMove;
        }

        private bool SwapBlocksContent(List<Block> sourceBlocks, List<Block> destinationBlocks)
        {
            var blocksMoved = false;

            if (this.CanSwapBlocksContent(sourceBlocks, destinationBlocks))
            {
                var piece = sourceBlocks[0].Piece;

                sourceBlocks.ForEach(b => b.Empty());
                destinationBlocks.ForEach(b => b.Fill(piece));

                blocksMoved = true;
            }

            return blocksMoved;
        }

        private void UpdateRotationOrigin(MoveDirection direction)
        {
            if (this.CurrentRotationOriginBlock != null)
            {
                switch (direction)
                {
                    case MoveDirection.Left:
                        this.CurrentRotationOriginBlock = this.CurrentRotationOriginBlock.LeftNeighbour;
                        break;

                    case MoveDirection.Right:
                        this.CurrentRotationOriginBlock = this.CurrentRotationOriginBlock.RightNeighbour;
                        break;

                    case MoveDirection.Down:
                        this.CurrentRotationOriginBlock = this.CurrentRotationOriginBlock.BottomNeighbour;
                        break;
                }
            }
        }

        /// <summary>
        /// Uses a flood fill algorithm to determine the region contained by the specified block
        /// A region is all the adjacent blocks which are filled by a single piece.
        /// A piece when complete (not yet split) only occupies one region. After being split it may occupy more than one region
        /// </summary>
        /// <param name="block"></param>
        /// <param name="region"></param>
        private List<Block> GetRegion(Block block, List<Block> region = null)
        {
            if (region == null)
            {
                region = new List<Block>();
            }

            if (block != null && !region.Contains(block))
            {
                region.Add(block);

                if (block.BottomNeighbour != null && block.BottomNeighbour.Piece == this)
                {
                    this.GetRegion(block.BottomNeighbour, region);
                }

                if (block.TopNeighbour != null && block.TopNeighbour.Piece == this)
                {
                    this.GetRegion(block.TopNeighbour, region);
                }

                if (block.LeftNeighbour != null && block.LeftNeighbour.Piece == this)
                {
                    this.GetRegion(block.LeftNeighbour, region);
                }

                if (block.RightNeighbour != null && block.RightNeighbour.Piece == this)
                {
                    this.GetRegion(block.RightNeighbour, region);
                }
            }

            return region;
        }

        #endregion
    }
}
