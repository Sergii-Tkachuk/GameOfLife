using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife
{
    public class GameEngine
    {
        public uint currentGeneration { get; private set; }
        private bool[,] field;       //поле станів клітинок
        private readonly int rows;   //кількість рядків поля
        private readonly int columns;//кількість колонок поля
        private Random random = new Random();


        public GameEngine(int rows, int columns, int density)
        {
            this.rows = rows;
            this.columns = columns;
            field = new bool [columns, rows];

            //генерація першого покоління клітинок
            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    field[x, y] = random.Next(density) == 0;
                }
            }
        }

        public void nextGeneration()
        {
            var newField = new bool[columns, rows];

            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    var neighboursCount = CountNeighbours(x, y); //підрахунок живих сусідів даної клітинки
                    var hasLife = field[x, y];                   //перевірка, чи жива дана клітинка

                    // сусідів 3 - клітинка народжується
                    if (!hasLife && neighboursCount == 3)
                        newField[x, y] = true;

                    //сусідів менше 2, або більше 3 - клітинка померає
                    else if (hasLife && (neighboursCount < 2 || neighboursCount > 3))
                        newField[x, y] = false;

                    //сусідів 2, 3 - клітинка лишається
                    else
                        newField[x, y] = field[x, y];
                }
            }
            field = newField;
            currentGeneration++;
        }

        public bool[,] GetCurrentGeneration()
        {
            bool[,] result = new bool[columns, rows];
            for(int x = 0; x < columns; x++)
            {
                for(int y = 0; y < rows; y++)
                {
                    result[x, y] = field[x, y];
                }
            }
            return result;
        }

        private int CountNeighbours(int x, int y)
        {
            int count = 0;

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    //поле малювання вважається як карта світу (тобто без рамок)
                    var col = (x + i + columns) % columns;
                    var row = (y + j + rows) % rows;

                    var isSelfChecking = col == x && row == y;
                    var hasLife = field[col, row];

                    if (hasLife && !isSelfChecking)//перевіряюча клітинка, сусідом не вважається
                        count++;
                }
            }
            return count;
        }
    }
}
