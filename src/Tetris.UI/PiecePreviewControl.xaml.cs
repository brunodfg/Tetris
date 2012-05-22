/*
 * Author: Bruno Gonçalves - brunodfg@gmail.com
 * Date: 18/02/2012
 * 
 * **/

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using brunodfg.tetris.engine.pieces;

namespace brunodfg.tetris.ui
{
    public partial class PiecePreviewControl : UserControl
    {
        #region Properties

        /// <summary>
        /// The piece being represented by this preview control
        /// </summary>
        public Piece Piece
        {
            get { return (Piece)GetValue(PieceProperty); }
            set { SetValue(PieceProperty, value); }
        }

        /// <summary>
        /// The piece being represented by this preview control
        /// </summary>
        public static readonly DependencyProperty PieceProperty = DependencyProperty.Register(
            "Piece",
            typeof(Piece),
            typeof(PiecePreviewControl),
            new PropertyMetadata((sender, e) =>
            {
                if (sender is PiecePreviewControl)
                {
                    (sender as PiecePreviewControl).OnPieceChanged();
                }
            }));

        #endregion

        #region Constructors

        /// <summary>
        /// A UserControl to represent the next pieces to be dropped in the game area
        /// </summary>
        public PiecePreviewControl()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Draws the piece represented by this UserControl
        /// </summary>
        private void OnPieceChanged()
        {
            this.PiecePreviewLayoutRoot.Children.Clear();
            this.PiecePreviewLayoutRoot.RowDefinitions.Clear();
            this.PiecePreviewLayoutRoot.ColumnDefinitions.Clear();

            if (this.Piece != null)
            {
                var size = this.Piece.Size;
                var maxX = this.Piece.Template.Max(p => (int)p.X);
                var maxY = this.Piece.Template.Max(p => (int)p.Y);

                #region Create rows and columns for the piece's parts

                for (int i = 0; i <= maxY; i++)
                {
                    this.PiecePreviewLayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(GameArea.BlockSize, GridUnitType.Pixel) });
                }

                for (int i = 0; i <= maxX; i++)
                {
                    this.PiecePreviewLayoutRoot.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(GameArea.BlockSize, GridUnitType.Pixel) });
                }

                #endregion

                #region Represent the piece with filled borders

                this.Piece.Template.ForEach(p =>
                {
                    var border = new Border()
                    { 
                        Margin = new Thickness(1), 
                        CornerRadius = new CornerRadius(2),
                        Background = new SolidColorBrush(this.Piece.Background)
                    };

                    border.SetValue(Grid.ColumnProperty, (int)p.X);
                    border.SetValue(Grid.RowProperty, maxY - (int)p.Y);

                    this.PiecePreviewLayoutRoot.Children.Add(border);
                });

                #endregion
            }
        }

        #endregion
    }
}
