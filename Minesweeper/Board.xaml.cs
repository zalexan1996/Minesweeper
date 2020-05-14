using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Minesweeper
{
    /// <summary>
    /// Interaction logic for Board.xaml
    /// </summary>
    public partial class Board : UserControl
    {

        private List<List<MineButton>> Buttons;
        private List<MineButton> MineButtons;




        public List<MineButton> RevealedButtons { get; protected set; }
        public List<MineButton> FlaggedButtons { get; protected set; }
        public List<MineButton> UnrevealedButtons { get; protected set; }

        public MainWindow winRef { get; set; }



        public bool GameRunning { get; set; }
        public int SizeX { get; set; } = 40;
        public int SizeY { get; set; } = 20;
        public int CellSize { get; set; } = 16;
        public int NumMines { get; set; } = 100;
        public int NumMinesLeft
        {
            get
            {
                return MineButtons.Count(b => !(b.IsFlagged));
            }
        }




        public Board()
        {
            Buttons = new List<List<MineButton>>();
            MineButtons = new List<MineButton>();
            
            // Used to optimize the AI
            RevealedButtons = new List<MineButton>();
            UnrevealedButtons = new List<MineButton>();
            FlaggedButtons = new List<MineButton>();

            

            MineButton.Colors.Add(Brushes.Blue);
            MineButton.Colors.Add(Brushes.DarkGreen);
            MineButton.Colors.Add(Brushes.Red);
            MineButton.Colors.Add(Brushes.Purple);
            MineButton.Colors.Add(Brushes.DarkMagenta);
            MineButton.Colors.Add(Brushes.DarkRed);
            MineButton.Colors.Add(Brushes.SandyBrown);
            MineButton.Colors.Add(Brushes.Black);


            InitializeComponent();

            InitGrid();

        }


        public void InitGrid()
        {
            GameRunning = true;

            // Reset Arrays
            Buttons.Clear();
            MineButtons.Clear();
            RevealedButtons.Clear();
            FlaggedButtons.Clear();

            for (int i = 0; i < SizeY; i++)
            {
                Buttons.Add(new List<MineButton>());
            }



            if (PlayGrid != null)
            {
                if (PlayGrid.Children != null)
                {
                    PlayGrid.Children.Clear();
                }

                PlayGrid.ColumnDefinitions.Clear();
                PlayGrid.RowDefinitions.Clear();

                for (int x = 0; x < SizeX; x++)
                {
                    PlayGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(CellSize) });
                }
                for (int y = 0; y < SizeY; y++)
                {
                    PlayGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(CellSize) });
                }

                for (int x = 0; x < SizeX; x++)
                {
                    for (int y = 0; y < SizeY; y++)
                    {
                        MineButton b = new MineButton(this, x, y, false);

                        Grid.SetColumn(b, x);

                        Grid.SetRow(b, y);

                        PlayGrid.Children.Add(b);
                        
                        Buttons[y].Add(b);
                        UnrevealedButtons.Add(b);

                    }
                }


                Console.WriteLine(Buttons.Count);
                Console.WriteLine(Buttons[0].Count);
                for (int i = 0; i < NumMines; i++)
                {
                    int x, y;
                    GetRandomCell(out x, out y);
                    Buttons[y][x].IsMine = true;
                    MineButtons.Add(Buttons[y][x]);
                }

                if (winRef != null)
                winRef.TB_FlagsLeft.Text = NumMines.ToString();
            }
            else
            {
                Console.WriteLine("NULL!");
            }

        }

        public void GetRandomCell(out int x, out int y, bool ExcludeMines = true)
        {

            Random r = new Random();
            x = r.Next(0, SizeX);
            y = r.Next(0, SizeY);

            if (ExcludeMines)
            {
                while (Buttons[y][x].IsMine && !Buttons[y][x].IsRevealed)
                {
                    x = r.Next(0, SizeX);
                    y = r.Next(0, SizeY);
                }
            }
        }


        public void VerifyWinCondition()
        {
            if (HasWon())
            {
                GameRunning = false;
                winRef.SetFace(MainWindow.Faces.Winner);
            }
        }

        public bool HasWon()
        {
            foreach (MineButton b in MineButtons)
            {
                if (b.IsFlagged == false)
                {
                    return false;
                }
            }
            return true;
        }

        public MineButton GetButton(int x, int y)
        {
            if (x >= 0 && x < SizeX && y >= 0 && y < SizeY)
            {
                return Buttons[y][x];
            }
            else
            {
                return null;
            }
        }



        public void RevealAll()
        {
            foreach (List<MineButton> l in Buttons)
            {
                foreach (MineButton b in l)
                {
                    b.Reveal();

                }
            }
        }



        public List<MineButton> GetNeighbors(int x, int y)
        {
            List<MineButton> btns = new List<MineButton>();


            for (int x1 = 0; x1 < 3; x1++)
            {

                for (int y1 = 0; y1 < 3; y1++)
                {
                    // Neighbor X
                    int nx = x1 + x;
                    // Neighbor Y
                    int ny = y1 + y;

                    // If this neighbor cell is out of bounds, add null
                    if ((nx < 0 || nx >= SizeX) || (ny < 0 || ny >= SizeY))
                    {
                        btns.Add(null);
                    }
                    // If this neighbor cell is valid, add the cell
                    else
                    {
                        btns.Add(Buttons[ny][nx]);
                    }
                }
            }

            return btns;
        }

        public int CountMineNeighbors(int x, int y)
        {
            int acc = 0;
            for (int x1 = -1; x1 < 2; x1++)
            {

                for (int y1 = -1; y1 < 2; y1++)
                {
                    // Neighbor X
                    int nx = x1 + x;
                    // Neighbor Y
                    int ny = y1 + y;

                    // If this neighbor is in range and isn't a mine
                    if (!((nx < 0 || nx >= SizeX) || (ny < 0 || ny >= SizeY)))
                    {
                        if (Buttons[ny][nx].IsMine)
                        {
                            acc++;
                        }
                    }
                }
            }

            return acc;
        }

        public void GameOver()
        {
            GameRunning = false;
            RevealAll();
            winRef.SetFace(MainWindow.Faces.Frowny);
        }
    }
}
