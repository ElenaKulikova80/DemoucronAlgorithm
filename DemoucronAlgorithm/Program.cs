namespace DemoucronAlgorithm
{
    class Program
    {
        // "нет ребра". 
        const int NO_EDGE = -1;

        static void Main()
        {
            const int N = 6;
            const int Smax = 3;
            int[][] A = new int[N][]
            {
                new int[] { 1, 2, NO_EDGE }, // 0 -> 1, 2
                new int[] { 3, NO_EDGE, NO_EDGE }, // 1 -> 3
                new int[] { 3, 4, NO_EDGE }, // 2 -> 3, 4
                new int[] { 5, NO_EDGE, NO_EDGE }, // 3 -> 5
                new int[] { 5, NO_EDGE, NO_EDGE }, // 4 -> 5
                new int[] { NO_EDGE, NO_EDGE, NO_EDGE } // 5 -> нет выходящих
            };

            try
            {
                int[][] level = Demoucron(A);
                PrintLevels(level);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Ошибка: {ex.Message}");
                Console.ResetColor();
            }

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        /// <summary>
        /// Алгоритм Демукрона: послойная топологическая сортировка DAG.
        /// Вход: вектор смежности A[N][Smax], где пустые ячейки помечены NO_EDGE.
        /// Выход: int[][] level, level[k] - вершины k-го уровня.
        /// </summary>
        public static int[][] Demoucron(int[][] A)
        {
            int N = A.Length;
            if (N == 0) return new int[0][];

            int Smax = A[0].Length;
            int[] inDegree = new int[N];
            int processedCount = 0;
            List<int[]> levelsList = new List<int[]>();

            //Подсчёт входящих степеней
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < Smax; j++)
                {
                    int v = A[i][j];
                    if (v != NO_EDGE && v >= 0 && v < N)
                    {
                        inDegree[v]++;
                    }
                }
            }

            // Находим начальный уровень (вершины с inDegree == 0)
            List<int> currentLevel = new List<int>();
            for (int i = 0; i < N; i++)
            {
                if (inDegree[i] == 0)
                    currentLevel.Add(i);
            }

            // Итеративно снимаем уровни
            while (currentLevel.Count > 0)
            {
                levelsList.Add(currentLevel.ToArray());
                processedCount += currentLevel.Count;

                List<int> nextLevel = new List<int>();

                foreach (int u in currentLevel)
                {
                    // Проходим по всем возможным соседям вершины
                    for (int j = 0; j < Smax; j++)
                    {
                        int v = A[u][j];
                        if (v != NO_EDGE && v >= 0 && v < N)
                        {
                            inDegree[v]--;
                            if (inDegree[v] == 0)
                            {
                                nextLevel.Add(v);
                            }
                        }
                    }
                }

                currentLevel = nextLevel;
            }

            // Проверка на наличие цикла
            if (processedCount != N)
            {
                throw new InvalidOperationException(
                    "Граф содержит цикл. Алгоритм Демукрона применим только к ациклическим графам (DAG).");
            }

            return levelsList.ToArray();
        }

        private static void PrintLevels(int[][] level)
        {
            Console.WriteLine("Результат расстановки по уровням:");
            for (int i = 0; i < level.Length; i++)
            {
                Console.Write($"Уровень {i}: [");
                Console.Write(string.Join(", ", level[i]));
                Console.WriteLine("]");
            }
        }
    }
}