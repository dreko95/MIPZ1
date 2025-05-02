using System;
using System.IO;

namespace MIPZ1
{
    internal class Program
    {
        private const int BoardSize = 19;

        /// <summary>
        /// Основний метод програми. Запускається при запуску консолі програми.
        /// </summary>
        static void Main(string[] args)
        {
            try
            {
                string inputFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tables", "Test2.txt");
                if (!File.Exists(inputFilePath))
                {
                    Console.WriteLine("Input file not found.");
                    Console.ReadKey();
                    return;
                }

                string[] lines = File.ReadAllLines(inputFilePath);
                int testCases = int.Parse(lines[0]);
                int lineIndex = 1;

                for (int t = 0; t < testCases; t++)
                {
                    int[,] board = new int[BoardSize, BoardSize];
                    for (int i = 0; i < BoardSize; i++)
                    {
                        string[] row = lines[lineIndex++].Split(' ');
                        for (int j = 0; j < BoardSize; j++)
                        {
                            board[i, j] = int.Parse(row[j]);
                        }
                    }

                    var result = CheckWinner(board);
                    Console.WriteLine(result.Item1);
                    if (result.Item1 != 0)
                    {
                        Console.WriteLine($"{result.Item2} {result.Item3}");
                    }

                    lineIndex++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\r\n\tStackTrace:{ex.StackTrace}");
            }
            

            Console.ReadKey();
        }

        /// <summary>
        /// Перевіряє стан дошки на наявність переможця в грі Renju.
        /// </summary>
        /// <param name="board">Двовимірний масив 19x19, що представляє стан дошки. 
        /// Значення: 0 - порожня клітинка, 1 - чорний камінь, 2 - білий камінь.</param>
        /// <returns>
        /// Кортеж (int, int, int):
        /// - Перший елемент: 1, якщо виграв чорний; 2, якщо виграв білий; 0, якщо ніхто не виграв.
        /// - Другий елемент: номер рядка (1-based) лівого верхнього каменя з п'яти поспіль.
        /// - Третій елемент: номер стовпця (1-based) лівого верхнього каменя з п'яти поспіль.
        /// </returns>
        static (int, int, int) CheckWinner(int[,] board)
        {
            int[] dx = { 0, 1, 1, -1 }; 
            int[] dy = { 1, 0, 1, 1 };

            for (int x = 0; x < BoardSize; x++)
            {
                for (int y = 0; y < BoardSize; y++)
                {
                    if (board[x, y] == 0) 
                        continue;

                    int player = board[x, y];
                    for (int d = 0; d < 4; d++)
                    {
                        int count = 1;
                        int nx = x, ny = y;

                        // Перевірка на 5 каменів поспіль
                        while (true)
                        {
                            nx += dx[d];
                            ny += dy[d];
                            if (nx < 0 || nx >= BoardSize || ny < 0 || ny >= BoardSize || board[nx, ny] != player)
                                break;
                            count++;
                        }

                        // Перевірка на більше ніж 5 каменів поспіль
                        if (count == 5)
                        {
                            int prevX = x - dx[d];
                            int prevY = y - dy[d];
                            if (prevX >= 0 && prevX < BoardSize && prevY >= 0 && prevY < BoardSize && board[prevX, prevY] == player)
                                continue;

                            int nextX = nx;
                            int nextY = ny;
                            if (nextX >= 0 && nextX < BoardSize && nextY >= 0 && nextY < BoardSize && board[nextX, nextY] == player)
                                continue;

                            // Виграв player
                            return (player, x + 1, y + 1); 
                        }
                    }
                }
            }
            
            // Ніхто не виграв
            return (0, 0, 0); 
        }
    }
}
