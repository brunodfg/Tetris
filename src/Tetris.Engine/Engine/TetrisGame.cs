/*h
 * Author: Bruno Gonçalves - brunodfg@gmail.com
 * Date: 18/02/2012
 * 
 * **/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;
using brunodfg.tetris.engine.controllers;
using brunodfg.tetris.engine.pieces;

namespace brunodfg.tetris.engine
{
    /// <summary>
    /// This class represents a game of tetris.
    /// </summary>
    public class TetrisGame
    {
        #region Private variables

        private DispatcherTimer gameLoopTimer;

        private TimeSpan moveSidewaysMinInterval = TimeSpan.FromMilliseconds(100);
        private TimeSpan moveDownMinInterval = TimeSpan.FromMilliseconds(50);
        private TimeSpan rotateMinInterval = TimeSpan.FromMilliseconds(150);

        private long lastRotation = DateTime.Now.Ticks;
        private long lastMoveSideways = DateTime.Now.Ticks;
        private long lastMoveDown = DateTime.Now.Ticks;
        private long lastDescend = DateTime.Now.Ticks;
        
        #endregion

        #region Events

        /// <summary>
        /// Occurs right after a game loop has ended
        /// </summary>
        public event EventHandler<EventArgs> Loop;

        /// <summary>
        /// Occurs when a row cis cleared
        /// </summary>
        public event EventHandler<RowClearedEventArgs> RowClear;

        #endregion

        #region Properties

        /// <summary>
        /// The definitions for the current game
        /// </summary>
        public TetrisDefinitions Definitions { get; private set; }

        /// <summary>
        /// The blocks which constitute the game area and which will be filled by the game pieces.
        /// Indexed by x,y position.
        /// </summary>
        public IDictionary<string, Block> Blocks { get; private set; }

        /// <summary>
        /// The blocks which constitute the game area and which will be filled by the game pieces.
        /// Indexed by row
        /// </summary>
        public IDictionary<int, List<Block>> Rows { get; private set; }

        /// <summary>
        /// The list of piece types available to this game
        /// </summary>
        public List<Type> AvailablePieceTypes { get; private set; }

        /// <summary>
        /// Returns the list of pieces which will be dropping next
        /// </summary>
        public ObservableCollection<Piece> NextPieces { get; private set; }

        /// <summary>
        /// Represents the piece currently being dropped.
        /// </summary>
        public Piece CurrentPiece { get; private set; }

        /// <summary>
        /// Whether the game is currently running, paused or stopped
        /// </summary>
        public GameState State { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int ClearedRows { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int Score { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int Level { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        private TimeSpan DescendInterval { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="definitions"></param>
        public TetrisGame(TetrisDefinitions definitions)
        {
            #region Validations

            if (definitions.Width <= 0)
            {
                throw new ArgumentException("width", "width must be greater than 0");
            }

            if (definitions.Height <= 0)
            {
                throw new ArgumentException("height", "width must be greater than 0");
            }

            #endregion

            this.Definitions = definitions;

            this.AvailablePieceTypes = new List<Type>() { typeof(IPiece), typeof(JPiece), typeof(LPiece), typeof(OPiece), typeof(SPiece), typeof(TPiece), typeof(ZPiece) };
            this.NextPieces = new ObservableCollection<Piece>();
            this.CreateBlocks();
            this.ResetGame();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Starts a new game
        /// </summary>
        public void Start()
        {
            if (this.State == GameState.Stopped)
            {
                this.ResetGame();
                this.GenerateRandomPieces(this.Definitions.PreviewPiecesCount).ForEach(x => this.NextPieces.Add(x));
                this.State = GameState.Running;

                this.gameLoopTimer = new DispatcherTimer() { Interval = TimeSpan.Zero };
                this.gameLoopTimer.Tick += new EventHandler(OnGameLoop);
                this.gameLoopTimer.Start();
            }
        }

        /// <summary>
        /// Ends the current game
        /// </summary>
        public void Stop()
        {
            this.State = GameState.Stopped;
            this.gameLoopTimer.Stop();
            this.ResetGame();
        }

        /// <summary>
        /// Pauses the current game
        /// </summary>
        public void Pause()
        {
            if (this.State == GameState.Running)
            {
                this.State = GameState.Paused;
            }
        }

        /// <summary>
        /// Resumes the current game
        /// </summary>
        public void Resume()
        { 
            if (this.State == GameState.Paused)
            {
                this.State = GameState.Running;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Block GetBlock(int x, int y)
        {
            var key = this.GetCoordinateKey(x, y);

            if (this.Blocks.ContainsKey(key))
            {
                return this.Blocks[key];
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Protected methods
        
        /// <summary>
        /// Occurs after a game loop ends
        /// </summary>
        protected virtual void OnLoopEnd()
        {
            if (this.Loop != null)
            {
                this.Loop(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Occurs when a row is cleared
        /// </summary>
        /// <param name="rowNumber"></param>
        protected virtual void OnRowClear(int rowNumber)
        {
            if (this.RowClear != null)
            {
                this.RowClear(this, new RowClearedEventArgs(rowNumber));
            }
        }        

        #endregion

        #region Private methods

        private void OnGameLoop(object sender, EventArgs e)
        {
            this.HandleKeyboard();

            if (this.State == GameState.Running)
            {
                // Can only drop a new piece if there isn't any piece falling down and if there are no more rows to clear
                var canDropNewPiece = this.CurrentPiece == null && !this.ShiftRemainingPiecesDown() && !this.ClearFilledRows();
                var canDescend = (DateTime.Now.Ticks - this.lastDescend) > this.DescendInterval.Ticks;

                #region Move current piece down or drop a new one

                if (canDescend)
                {
                    lastDescend = DateTime.Now.Ticks;

                    // Drop new piece if necessary
                    if (this.CurrentPiece == null)
                    {
                        if (canDropNewPiece)
                        {
                            if (!this.DropNewPiece())
                            {
                                this.State = GameState.GameOver;
                                this.gameLoopTimer.Stop();
                            }
                        }
                    }
                    else
                    {
                        // Descend piece one block down
                        if (!this.CurrentPiece.Move(MoveDirection.Down))
                        {
                            this.CurrentPiece = null;
                        }
                    }
                }

                #endregion

                this.OnLoopEnd();
            }
        }

        private void HandleKeyboard()
        {
            if (this.CurrentPiece != null && this.State == GameState.Running)
            {
                #region Move / Rotate

                var canMoveSideways = (DateTime.Now.Ticks - this.lastMoveSideways) > this.moveSidewaysMinInterval.Ticks;
                var canMoveDown= (DateTime.Now.Ticks - this.lastMoveDown) > this.moveDownMinInterval.Ticks;
                var canRotate = (DateTime.Now.Ticks - this.lastRotation) > this.rotateMinInterval.Ticks;

                if (KeyboardState.IsKeyPressed(this.Definitions.MoveLeftKey) == true && canMoveSideways)
                {
                    this.lastMoveSideways = DateTime.Now.Ticks;
                    this.CurrentPiece.Move(MoveDirection.Left);
                }

                if (KeyboardState.IsKeyPressed(this.Definitions.MoveRightKey) == true && canMoveSideways)
                {
                    this.lastMoveSideways = DateTime.Now.Ticks;
                    this.CurrentPiece.Move(MoveDirection.Right);
                }

                if (KeyboardState.IsKeyPressed(this.Definitions.MoveDownKey) == true && canMoveDown)
                {
                    this.lastMoveDown = DateTime.Now.Ticks;
                    this.CurrentPiece.Move(MoveDirection.Down);
                    
                }

                if (KeyboardState.IsKeyPressed(this.Definitions.SlideDownKey) == true && canMoveDown)
                {
                    this.lastMoveDown = DateTime.Now.Ticks;
                    this.CurrentPiece.Slide();
                }

                if (KeyboardState.IsKeyPressed(this.Definitions.RotateClockwiseKey) == true && canRotate)
                {
                    this.lastRotation = DateTime.Now.Ticks;
                    this.CurrentPiece.Rotate(true);
                }

                if (KeyboardState.IsKeyPressed(this.Definitions.RotateCounterClockwiseKey) == true && canRotate)
                {
                    this.lastRotation = DateTime.Now.Ticks;
                    this.CurrentPiece.Rotate(false);
                }

                #endregion
            }
        }

        private bool DropNewPiece()
        {
            var pieceDropped = false;

            if (this.NextPieces != null && this.NextPieces.Count > 0)
            {
                // Deque
                this.CurrentPiece = this.NextPieces[0];

                var yIncrement = this.Definitions.Height - 1 - this.CurrentPiece.Template.Select(l => (int)l.Y).Max();
                var xIncrement = (this.Definitions.Width - this.CurrentPiece.Size) / 2;
                var blocksToFill = this.CurrentPiece.Template.Select(point => this.GetBlock((int)point.X + xIncrement, (int)point.Y + yIncrement)).ToList();

                // Do not allow droping a new piece if the drop location is filled
                if (blocksToFill != null && blocksToFill.All(b => !b.IsFilled))
                {
                    blocksToFill.ForEach(b => b.Fill(this.CurrentPiece));
                    this.CurrentPiece.JoinGame(this);

                    // Generate a new random piece and add it to the queue
                    this.NextPieces.Add(this.GenerateRandomPieces(1)[0]);
                    this.NextPieces.RemoveAt(0);

                    pieceDropped = true;
                }
            }

            return pieceDropped;
        }

        private bool ClearFilledRows()
        {
            var clearedRows = 0;

            // Clears all rows which have all blocks currently filled
            for (int y = this.Definitions.Height - 1; y >= 0; y--)
            {
                var rowBlocks = this.Rows[y];
                var isRowFilled = rowBlocks.All(x => x.IsFilled);

                if (isRowFilled)
                {
                    clearedRows++;
                    rowBlocks.ForEach(b => b.Empty());

                    this.OnRowClear(y);
                }
            }

            this.UpdateScoreAndLevelUp(clearedRows);

            return clearedRows > 0;
        }

        private bool ShiftRemainingPiecesDown()
        {
            var piecesHaveMoved = false;
            var remainingPieces = this.Blocks.Values.Where(x => x.IsFilled && x.Piece != this.CurrentPiece).OrderBy(b => b.Y).Select(b => b.Piece).Distinct().ToList();

            foreach (var piece in remainingPieces)
            {
                if (piece.Slide())
                {
                    piecesHaveMoved = true;
                }
            }

            return piecesHaveMoved;
        }

        private void UpdateScoreAndLevelUp(int clearedRows)
        {
            if (clearedRows > 0)
            {
                // Update score
                var pointsPerLine = 100;
                var difficultyFactor = clearedRows < 4 ? 1 : (clearedRows < 8 ? 2 : 3);

                this.Score += difficultyFactor * clearedRows * pointsPerLine;

                // Level up
                for (int i = 0; i < clearedRows; i++)
                {
                    this.ClearedRows++;

                    // Every 10 rows the game levels up
                    if (this.ClearedRows % 10 == 0)
                    {
                        this.Level++;

                        // Increase the speed every level until the time between descends is less than 250 ms
                        var tmp = this.DescendInterval.Subtract(TimeSpan.FromMilliseconds(40));

                        if (tmp.TotalMilliseconds > 250)
                        {
                            this.DescendInterval = tmp;
                        }
                    }
                }
            }
        }       

        private List<Piece> GenerateRandomPieces(int numberOfPieces)
        {
            var randomPieces = new List<Piece>();
            var random = new Random((int)DateTime.UtcNow.Ticks);

            for (int i = 0; i < numberOfPieces; i++)
            {
                var pieceTypeIndex = random.Next(this.AvailablePieceTypes.Count);
                var piece = Activator.CreateInstance(this.AvailablePieceTypes[pieceTypeIndex]) as Piece;

                randomPieces.Add(piece);
            }

            return randomPieces;
        }

        private string GetCoordinateKey(int x, int y)
        {
            return string.Format("{0}:{1}", x, y);
        }

        private void CreateBlocks()
        {
            this.Blocks = new Dictionary<string, Block>();
            this.Rows = new Dictionary<int, List<Block>>();

            for (int y = 0; y < this.Definitions.Height; y++)
            {
                var row = new List<Block>();

                for (int x = 0; x < this.Definitions.Width; x++)
                {
                    var block = new Block(this, x, y);

                    // Index blocks by x,y position
                    this.Blocks.Add(this.GetCoordinateKey(x, y), block);
                    row.Add(block);
                }

                // Index blocks by row
                this.Rows.Add(y, row);
            }
        }

        private void ResetGame()
        {
            this.Blocks.Values.ToList().ForEach(x => x.Empty());
            this.NextPieces.Clear();
            
            this.CurrentPiece = null;
            this.ClearedRows = 0;
            this.Level = 1;
            this.Score = 0;
            this.DescendInterval = TimeSpan.FromSeconds(1);
        }

        #endregion
    }
}
