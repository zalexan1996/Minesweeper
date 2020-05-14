using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Minesweeper
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow(int sizeX, int sizeY, int numMines, int cellSize)
        {
            InitializeComponent();

            SizeX = sizeX;
            SizeY = sizeY;
            NumMines = numMines;
            CellSize = cellSize;
        }


        public int SizeX
        {
            get
            {
                int i = 0;
                if (Int32.TryParse(TB_SizeX.Text, out i))
                {
                    return i;
                }
                return 32;

            }
            set
            {
                TB_SizeX.Text = value.ToString();
            }
        }
        public int SizeY
        {
            get
            {
                int i = 0;
                if (Int32.TryParse(TB_SizeY.Text, out i))
                {
                    return i;
                }
                return 32;
            }
            set
            {
                TB_SizeY.Text = value.ToString();
            }
        }
        public int NumMines
        {
            get
            {
                int i = 0;
                if (Int32.TryParse(TB_NumMines.Text, out i))
                {
                    return i;
                }
                return 100;
            }
            set
            {
                TB_NumMines.Text = value.ToString();
            }
        }
        public int CellSize
        {
            get
            {
                int i = 0;
                if (Int32.TryParse(TB_CellSize.Text, out i))
                {
                    return i;
                }
                return 16;
            }
            set
            {
                TB_CellSize.Text = value.ToString();
            }
        }
    }
}
