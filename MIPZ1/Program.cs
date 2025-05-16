using System;
using System.Collections.Generic;
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

                if (!TryGetBoards(lines, out List<int[,]> boards, out string errorMessage))
                {
                    Console.WriteLine($"Error: {errorMessage}");
                    Console.ReadKey();
                    return;
                }

                foreach (var board in boards)
                {
                    var result = CheckWinner(board);
                    Console.WriteLine(result.Item1);
                    if (result.Item1 != 0)
                    {
                        Console.WriteLine($"{result.Item2} {result.Item3}");
                    }

                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\r\n\tStackTrace:{ex.StackTrace}");
            }
            

            Console.ReadKey();
        }

        /// <summary>
        /// Спроба отримати список дошок з вхідних даних.
        /// </summary>
        /// <param name="lines">Масив рядків з файлу.</param>
        /// <param name="boards">Список дошок, якщо дані валідні.</param>
        /// <param name="errorMessage">Повідомлення про помилку, якщо перевірка не пройдена.</param>
        /// <returns>True, якщо дані валідні, інакше False.</returns>
        static bool TryGetBoards(string[] lines, out List<int[,]> boards, out string errorMessage)
        {
            boards = new List<int[,]>();
            errorMessage = string.Empty;

            if (lines.Length < 1)
            {
                errorMessage = "Input file is empty or does not contain the number of test cases.";
                return false;
            }

            if (!int.TryParse(lines[0], out int testCases) || testCases <= 0)
            {
                errorMessage = "The first line must contain a positive integer representing the number of test cases.";
                return false;
            }

            int lineIndex = 1;
            for (int t = 0; t < testCases; t++)
            {
                if (lineIndex + BoardSize > lines.Length)
                {
                    errorMessage = $"Test case {t + 1} does not contain exactly {BoardSize} rows.";
                    return false;
                }

                int[,] board = new int[BoardSize, BoardSize];
                for (int i = 0; i < BoardSize; i++)
                {
                    string[] row = lines[lineIndex++].Split(' ');
                    if (row.Length != BoardSize)
                    {
                        errorMessage = $"Row {i + 1} in test case {t + 1} does not contain exactly {BoardSize} columns.";
                        return false;
                    }

                    for (int j = 0; j < BoardSize; j++)
                    {
                        if (!int.TryParse(row[j], out int value) || (value != 0 && value != 1 && value != 2))
                        {
                            errorMessage = $"Invalid value '{row[j]}' at row {i + 1}, column {j + 1} in test case {t + 1}. Allowed values are 0, 1, 2.";
                            return false;
                        }

                        board[i, j] = value;
                    }
                }

                boards.Add(board);
                lineIndex++;
            }

            return true;
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
                            if (!IsValidCoordinateAndPlayer(nx, ny, board, player))
                                break;
                            count++;
                        }

                        // Перевірка на більше ніж 5 каменів поспіль
                        if (count == 5)
                        {
                            int prevX = x - dx[d];
                            int prevY = y - dy[d];
                            if (IsValidCoordinateAndPlayer(prevX, prevY, board, player))
                                continue;

                            int nextX = nx;
                            int nextY = ny;
                            if (IsValidCoordinateAndPlayer(nextX, nextY, board, player))
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

        /// <summary>
        /// Перевіряє, чи координата знаходиться в межах дошки і чи камінь належить певному гравцю
        /// </summary>
        /// <param name="x">Координата X</param>
        /// <param name="y">Координата Y</param>
        /// <param name="board">Дошка</param>
        /// <param name="player">Гравець (1 або 2)</param>
        /// <returns>True, якщо координата валідна і камінь належить гравцю, інакше False</returns>
        static bool IsValidCoordinateAndPlayer(int x, int y, int[,] board, int player)
        {
            return x >= 0 && x < BoardSize && y >= 0 && y < BoardSize && board[x, y] == player;
        }
    }
}
