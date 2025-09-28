using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saper.Models
{
    public class GameSettings
    {
        public int Rows { get; set; }
        public int Columns { get; set; }
        public int Mines { get; set; }
        public double MinePercentage { get; set; }

        public GameSettings(int rows, int columns, int mines)
        {
            Rows = rows;
            Columns = columns;
            Mines = mines;
            MinePercentage = (double)mines / (rows * columns) * 100;
        }
    }
}
