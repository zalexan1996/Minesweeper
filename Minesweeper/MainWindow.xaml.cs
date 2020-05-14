using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        BitmapImage Smiley;
        BitmapImage Nervous;
        BitmapImage Frowny;
        BitmapImage Winner;


        Solver s = new Solver();

        public enum Faces
        {
            Smiley,
            Nervous,
            Frowny,
            Winner
        }

        public MainWindow()
        {

            InitializeComponent();
            board.winRef = this;
            Smiley = new BitmapImage(new Uri("Assets/Smiley.png", UriKind.Relative));
            Nervous = new BitmapImage(new Uri("Assets/Nervous.png", UriKind.Relative));
            Frowny = new BitmapImage(new Uri("Assets/Frowny.png", UriKind.Relative));
            Winner = new BitmapImage(new Uri("Assets/Winner.png", UriKind.Relative));
        }

        public void SetFace(Faces Face)
        {
            Image i = new Image();
            switch(Face)
            {
                case Faces.Frowny:
                    i.Source = Frowny;
                    BTN_Face.Content = i;
                    break;
                case Faces.Nervous:
                    i.Source = Nervous;
                    BTN_Face.Content = i;
                    break;
                case Faces.Smiley:
                    i.Source = Smiley;
                    BTN_Face.Content = i;
                    break;
                case Faces.Winner:
                    i.Source = Winner;
                    BTN_Face.Content = i;
                    break;
            }
        }

        private void ResetMenu_Click(object sender, RoutedEventArgs e)
        {
            board.InitGrid();
        }
        private void SettingsMenu_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settings = new SettingsWindow(board.SizeX, board.SizeY, board.NumMines, board.CellSize);
            settings.ShowDialog();

            if (settings.SizeX > 0 && settings.SizeY > 0 && settings.NumMines <= settings.SizeX * settings.SizeY)
            {
                board.SizeX = settings.SizeX;
                board.SizeY = settings.SizeY;
                board.NumMines = settings.NumMines;
                board.CellSize = settings.CellSize;

                //board.InitGrid();
            }
        }

        private void BTN_Face_Click(object sender, RoutedEventArgs e)
        {
            if (!board.GameRunning)
            {
                board.InitGrid();
                
            }
        }

        private void BTN_Solver_Click(object sender, RoutedEventArgs e)
        {
            if (!s.InProgress)
            {
                s.BeginSolve(board);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (s.InProgress)
            {
                s.StopSolve();
            }
        }
    }
}