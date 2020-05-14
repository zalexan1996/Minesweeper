using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Xml.Serialization;

namespace Minesweeper
{
    /// <summary>
    /// Interaction logic for MineButton.xaml
    /// </summary>
    public partial class MineButton : UserControl
    {

        public static List<SolidColorBrush> Colors = new List<SolidColorBrush>();


        public enum RelativePositions
        {
            TopLeft,
            Top,
            TopRight,
            Right,
            BottomRight,
            Bottom,
            BottomLeft,
            Left
        }

        public bool IsMine { get; set; }
        public bool IsRevealed { get; set; }
        public bool IsFlagged { get; set; }
        public List<Button> Neighbors { get; set; }
        public int X { get; set; }
        public int Y { get; set; }


        private Board board;

        public MineButton()
        {
            IsMine = false;
            IsRevealed = false;
            InitializeComponent();

        }
        public MineButton(Board b, int x, int y, bool isMine)
        {
            X = x;
            Y = y;
            board = b;
            IsMine = isMine;
            IsRevealed = false;
            InitializeComponent();
            BTN.FontSize = board.CellSize / 2;
            BTN.Width = board.CellSize;
            BTN.Height = board.CellSize;
            Width = board.CellSize;
            Height = board.CellSize;
        }

        public void Activate()
        {
            if (!IsRevealed && !IsFlagged)
            {
                IsRevealed = true;
                BTN.IsEnabled = false;
                board.UnrevealedButtons.Remove(this);
                if (IsMine)
                {
                    BTN.Background = Brushes.Red;
                    Image i = new Image();
                    i.Source = new BitmapImage(new Uri("Assets/Mine.png", UriKind.Relative));
                    BTN.Content = i;
                    board.GameOver();
                }
                else
                {
                    // Set Text to number of neighbors.
                    int Count = board.CountMineNeighbors(X, Y);
                    if (Count > 0)
                    {
                        BTN.Content = Count;
                        BTN.Foreground = Colors[Count - 1];
                        board.RevealedButtons.Add(this);
                    }
                    else
                    {
                        // 0 neighbors that are mines, so activate each one.
                        foreach (RelativePositions pos in Enum.GetValues(typeof(RelativePositions)))
                        {
                            MineButton b = GetNeighbor(pos);
                            if (b != null && b.IsRevealed == false )
                                b.Activate();
                        }
                    }
                }
            }
        }

        public void Flag()
        {
            if (board.GameRunning)
            {
                if (!IsRevealed)
                {
                    board.UnrevealedButtons.Remove(this);
                    if (IsFlagged)
                    {
                        BTN.Content = "";
                        IsFlagged = false;
                        board.winRef.TB_FlagsLeft.Text = (Int32.Parse(board.winRef.TB_FlagsLeft.Text.ToString()) + 1).ToString();
                        board.FlaggedButtons.Remove(this);
                    }
                    else
                    {

                        Image i = new Image();
                        i.Source = new BitmapImage(new Uri("Assets/Flag.png", UriKind.Relative));
                        BTN.Content = i;
                        IsFlagged = true;
                        board.winRef.SetFace(MainWindow.Faces.Smiley);
                        board.VerifyWinCondition();
                        board.winRef.TB_FlagsLeft.Text = (Int32.Parse(board.winRef.TB_FlagsLeft.Text.ToString()) - 1).ToString();
                        board.FlaggedButtons.Add(this);
                    }
                }
            }
        }


        public void Reveal()
        {
            IsEnabled = false;
            board.UnrevealedButtons.Remove(this);
            if (IsMine)
            {
                Image i = new Image();
                i.Source = new BitmapImage(new Uri("Assets/Mine.png", UriKind.Relative));
                BTN.Content = i;
                return;
            }

            int neighbors = CountMineNeighbors();
            if (neighbors == 0)
            {
                BTN.Content = "";
            }
            else
            {
                BTN.Content = neighbors;
                BTN.Foreground = Colors[neighbors - 1];
            }
            
        }

        public MineButton GetNeighbor(RelativePositions position)
        {
            MineButton neighbor = null;
            switch (position)
            {
                case RelativePositions.Bottom:
                    neighbor = board.GetButton(X, Y + 1);
                    break;
                case RelativePositions.BottomLeft:
                    neighbor = board.GetButton(X - 1, Y + 1);
                    break;
                case RelativePositions.BottomRight:
                    neighbor = board.GetButton(X + 1, Y + 1);
                    break;
                case RelativePositions.Left:
                    neighbor = board.GetButton(X - 1, Y);
                    break;
                case RelativePositions.Right:
                    neighbor = board.GetButton(X + 1, Y);
                    break;
                case RelativePositions.Top:
                    neighbor = board.GetButton(X, Y - 1);
                    break;
                case RelativePositions.TopLeft:
                    neighbor = board.GetButton(X - 1, Y - 1);
                    break;
                case RelativePositions.TopRight:
                    neighbor = board.GetButton(X + 1, Y - 1);
                    break;
            }

            return neighbor;
        }



        public int CountTotalNeighbors()
        {
            int acc = 0;
            foreach (RelativePositions pos in Enum.GetValues(typeof(RelativePositions)))
            {
                if (GetNeighbor(pos) != null)
                    acc++;
            }

            return acc;
        }
        public int CountMineNeighbors()
        {
            int acc = 0;
            foreach (RelativePositions pos in Enum.GetValues(typeof(RelativePositions)))
            {
                if (GetNeighbor(pos) != null && GetNeighbor(pos).IsMine)
                    acc++;
            }

            return acc;
        }

        public int CountRevealedNeighbors()
        {
            int acc = 0;
            foreach (RelativePositions pos in Enum.GetValues(typeof(RelativePositions)))
            {
                if (GetNeighbor(pos) != null && GetNeighbor(pos).IsRevealed)
                    acc++;
            }

            return acc;
        }
        public int CountFlaggedNeighbors()
        {
            int acc = 0;
            foreach (RelativePositions pos in Enum.GetValues(typeof(RelativePositions)))
            {
                if (GetNeighbor(pos) != null && GetNeighbor(pos).IsFlagged)
                    acc++;
            }

            return acc;
        }



        


        private void BTN_MouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Flag();
        }

        private void BTN_Click(object sender, RoutedEventArgs e)
        {
            Activate();
            board.VerifyWinCondition();
        }

        private void BTN_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            board.winRef.SetFace(MainWindow.Faces.Nervous);
        }

        private void BTN_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (board.GameRunning)
            {
                board.winRef.SetFace(MainWindow.Faces.Smiley);
            }
        }
    }
}
