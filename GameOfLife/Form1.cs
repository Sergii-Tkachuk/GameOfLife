using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace GameOfLife
{
    public partial class Form1 : Form
    {
        private Graphics graphics;
        private int resolution;   //розширення клітинки
        private bool[,] field;    //поле станів клітинок
        private int rows;         //кількість рядків поля
        private int columns;      //кількість колонок поля
        private int currentGeneration = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void StartGame()
        {
            //якщо таймер запущений, ми нічо не можемо змінити
            if (timer1.Enabled) 
                return;

            currentGeneration     = 0;
            Text                  = $"Generation {currentGeneration}";

            nudResolution.Enabled = false;
            nudDensity.Enabled    = false;

            //ініціалізація даних
            resolution            = (int)nudResolution.Value;
            rows                  = pictureBox1.Height / resolution;
            columns               = pictureBox1.Width / resolution;
            field                 = new bool[columns, rows];

            //генерація першого покоління клітинок
            Random random = new Random();
            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    field[x, y] = random.Next((int)nudDensity.Value) == 0;
                }
            }

            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            graphics          = Graphics.FromImage(pictureBox1.Image);

            timer1.Start();
        }

        private void nextGeneration()
        {
            graphics.Clear(Color.Black);

            var newField = new bool[columns, rows];

            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    var neighboursCount = CountNeighbours(x, y);
                    var hasLife = field[x, y];

                    if(!hasLife && neighboursCount == 3)
                        newField[x, y] = true;
                    else if(hasLife && (neighboursCount < 2 || neighboursCount > 3))
                        newField[x, y] = false;
                    else
                        newField[x, y] = field[x, y];

                    if(hasLife)
                        graphics.FillRectangle(Brushes.Crimson, x * resolution, y * resolution, resolution, resolution);
                }
            }
            field = newField;
            pictureBox1.Refresh();
            Text = $"Generation {++currentGeneration}";
        }

        private int CountNeighbours(int x, int y)
        {
            int count = 0;

            for (int i = -1; i < 2; i++)
            {
                for(int j = -1; j < 2; j++)
                {
                    var col            = (x + i + columns) % columns;
                    var row            = (y + j + rows) % rows;
                    var isSelfChecking = col == x && row == y;
                    var hasLife        = field[col, row];

                    if(hasLife && !isSelfChecking)
                        count++;
                }
            }
            return count;
        }

        private void StopGame()
        {
            if (!timer1.Enabled)
                return;
            timer1.Stop();
            nudResolution.Enabled = true;
            nudDensity.Enabled = true;
        }



        private void timer1_Tick(object sender, EventArgs e)
        {
            nextGeneration();
        }

        private void bStart_Click(object sender, EventArgs e)
        {
            StartGame();
            //graphics.FillRectangle(Brushes.Crimson, 0, 0, resolution, resolution);
        }

        private void bStop_Click(object sender, EventArgs e)
        {
            StopGame();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!timer1.Enabled)
                return;

            if(e.Button == MouseButtons.Left)
            {
                var x = e.Location.X / resolution;
                var y = e.Location.Y / resolution;
                if(ValidateMousePosition(x, y))
                    field[x, y] = true;
            }

            if (e.Button == MouseButtons.Right)
            {
                var x = e.Location.X / resolution;
                var y = e.Location.Y / resolution;
                if (ValidateMousePosition(x, y))
                    field[x, y] = false;
            }
        }
        private bool ValidateMousePosition(int x, int y)
        {
            return x >= 0 && y >= 0 && x < columns && y < rows;
        }
    }
}
