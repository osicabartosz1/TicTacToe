class TicTacToe
{

    char[,] board =
    {
        { '1', '2', '3' },
        { '4', '5', '6' },
        { '7', '8', '9' }
    };

    char currentPlayer = 'X'; // Gracz zaczyna jako 'X'

    int decisionCounter = 0;

    bool console = false;

    public TicTacToe(bool console = false)
    {
        this.console = console;
    }

    static void Main(string[] args)
    {
        Dictionary<string, int> results = new Dictionary<string, int>();
        bool console = false;
        if (console) 
        {
            int[] decision = { 0, 0, 0, 0, 0 };
            TicTacToe ticTacToe = new TicTacToe(console);
            ticTacToe.Play(decision);
        }
        else
        {
            int[] decision = { 0, 0, 0, 0, 0 };
            for (int a = 0; a < 9; a++)
            {
                for (int b = 0; b < 7; b++)
                {
                    for (int c = 0; c < 5; c++)
                    {
                        for (int d = 0; d < 3; d++)
                        {
                            decision[0] = a;
                            decision[1] = b;
                            decision[2] = c;
                            decision[3] = d;
                            TicTacToe ticTacToe = new TicTacToe();
                            string result = ticTacToe.Play(decision);
                            if(results.ContainsKey(result)) results[result]++;
                            else results.Add(result, 1);
                        }
                    }
                }
            }
            foreach (var resul in results) 
            {
                Console.WriteLine($"{resul.Key} = {resul.Value}");
            }
        }
    }

    string Play(int[] decision)
    {
        int moves = 0; // Licznik ruchów

        while (true)
        {
            if(console){
                Console.Clear();
                PrintBoard(); // Wyświetl planszę
            }

            if (currentPlayer == 'X')
            {
                PlayerInput(decision); // Ruch gracza
            }
            else
            {
                ComputerMove(); // Ruch komputera
            }

            if (CheckWin())
            {
                string result = $"Gracz {currentPlayer} wygrywa!";
                if (console)
                {
                    Console.Clear();
                    PrintBoard();
                    Console.WriteLine(result);
                }
                if (result.Contains("X"))
                {
                    PrintBoard();
                    decision.ToList().ForEach(x => Console.Write(x));
                    Console.WriteLine();
                }

                return result;
            }

            if (++moves == 9)
            {
                if (console)
                {
                    Console.Clear();
                    PrintBoard();
                    Console.WriteLine();
                }
                return "Remis!";
            }

            SwitchPlayer(); // Zmiana gracza
        }
    }

    void PrintBoard()
    {
        Console.WriteLine("     |     |     ");
        Console.WriteLine($"  {board[0, 0]}  |  {board[0, 1]}  |  {board[0, 2]}  ");
        Console.WriteLine("_____|_____|_____");
        Console.WriteLine("     |     |     ");
        Console.WriteLine($"  {board[1, 0]}  |  {board[1, 1]}  |  {board[1, 2]}  ");
        Console.WriteLine("_____|_____|_____");
        Console.WriteLine("     |     |     ");
        Console.WriteLine($"  {board[2, 0]}  |  {board[2, 1]}  |  {board[2, 2]}  ");
        Console.WriteLine("     |     |     ");
    }

    char[] GetAvailableOptions()
    {
        List<char> temp = new List<char>();
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                if (board[x, y] != 'X' && board[x, y] != 'O')
                {
                    temp.Add(board[x, y]);
                }
            }
        }
        return temp.ToArray();
    }

    void PlayerInput(int[] decision)
    {
        int choice;
        bool validInput = false;

        do
        {
            string input;
            if (console)
            {
                Console.WriteLine($"\nGracz {currentPlayer}, wybierz numer pola:");
                input = Console.ReadLine();
            }
            else
            {
                char[] arraydecision = GetAvailableOptions();
                input = arraydecision[decision[decisionCounter]].ToString();
                decisionCounter++;
            }

            if (int.TryParse(input, out choice) && choice >= 1 && choice <= 9)
            {
                int row = (choice - 1) / 3;
                int col = (choice - 1) % 3;

                if (board[row, col] != 'X' && board[row, col] != 'O')
                {
                    board[row, col] = currentPlayer;
                    validInput = true;
                }
                else
                {
                    Console.WriteLine("To pole jest już zajęte, wybierz inne.");
                }
            }
            else
            {
                Console.WriteLine("Nieprawidłowy wybór, spróbuj ponownie.");
            }
        } while (!validInput);
    }

    // Ruch komputera (losowy wybór wolnego pola)
    // Ruch komputera (oparty na analizie planszy)
    void ComputerMove()
    {
        if (console) Console.WriteLine("\nRuch komputera...");
        if (console) System.Threading.Thread.Sleep(1000); // Dodanie krótkiego opóźnienia dla lepszego efektu

        // Sprawdzenie, czy komputer może wygrać
        if (TryMakeWinningMove())
        {
            return;
        }

        // Sprawdzenie, czy komputer może zablokować gracza
        if (TryBlockPlayer())
        {
            return;
        }

        // Zajmowanie środka planszy, jeśli wolny
        if (board[1, 1] != 'X' && board[1, 1] != 'O')
        {
            board[1, 1] = currentPlayer;
            return;
        }

        // Zajmowanie rogu, jeśli wolny
        if (TryTakeCornerUpgrade())
        {
            return;
        }

        // Zajęcie dowolnego wolnego pola
        TakeRandomFreeSpace();
    }

    // Funkcja sprawdzająca, czy komputer może wygrać
    bool TryMakeWinningMove()
    {
        return TryCompleteLine(currentPlayer); // Szukanie, czy można wygrać w jednym z wierszy, kolumn lub przekątnych
    }

    // Funkcja sprawdzająca, czy komputer może zablokować gracza
    bool TryBlockPlayer()
    {
        char opponent = (currentPlayer == 'X') ? 'O' : 'X';
        return TryCompleteLine(opponent); // Szukanie, czy gracz może wygrać i zablokowanie jego ruchu
    }

    // Funkcja próbująca uzupełnić linię dla danego gracza (np. wygranie lub blokowanie przeciwnika)
    bool TryCompleteLine(char player)
    {
        // Sprawdzanie wierszy
        for (int row = 0; row < 3; row++)
        {
            if (CheckLine(player, row, 0, row, 1, row, 2))
            {
                return true;
            }
        }

        // Sprawdzanie kolumn
        for (int col = 0; col < 3; col++)
        {
            if (CheckLine(player, 0, col, 1, col, 2, col))
            {
                return true;
            }
        }

        // Sprawdzanie przekątnych
        if (CheckLine(player, 0, 0, 1, 1, 2, 2))
        {
            return true;
        }

        if (CheckLine(player, 0, 2, 1, 1, 2, 0))
        {
            return true;
        }

        return false;
    }

    // Funkcja sprawdzająca, czy można uzupełnić linię (np. wypełnić wiersz, kolumnę lub przekątną)
    bool CheckLine(char player, int r1, int c1, int r2, int c2, int r3, int c3)
    {
        // Sprawdzamy, czy dwie komórki są zajęte przez gracza, a trzecia jest wolna
        if (board[r1, c1] == player && board[r2, c2] == player && board[r3, c3] != 'X' && board[r3, c3] != 'O')
        {
            board[r3, c3] = currentPlayer;
            return true;
        }

        if (board[r1, c1] == player && board[r3, c3] == player && board[r2, c2] != 'X' && board[r2, c2] != 'O')
        {
            board[r2, c2] = currentPlayer;
            return true;
        }

        if (board[r2, c2] == player && board[r3, c3] == player && board[r1, c1] != 'X' && board[r1, c1] != 'O')
        {
            board[r1, c1] = currentPlayer;
            return true;
        }

        return false;
    }

    // Funkcja próbująca zajęć róg, jeśli wolny
    bool TryTakeCorner()
    {
        // Sprawdzamy rogi w kolejności: lewy górny, prawy górny, lewy dolny, prawy dolny
        if (board[0, 0] != 'X' && board[0, 0] != 'O')
        {
            board[0, 0] = currentPlayer;
            return true;
        }

        if (board[0, 2] != 'X' && board[0, 2] != 'O')
        {
            board[0, 2] = currentPlayer;
            return true;
        }

        if (board[2, 0] != 'X' && board[2, 0] != 'O')
        {
            board[2, 0] = currentPlayer;
            return true;
        }

        if (board[2, 2] != 'X' && board[2, 2] != 'O')
        {
            board[2, 2] = currentPlayer;
            return true;
        }

        return false;
    }
    // Funkcja próbująca zajęć bok, jeśli wolny
    bool TryTakeSide()
    {
        // Sprawdzamy soki w kolejności: takie a takiej
        if (board[0, 1] != 'X' && board[0, 1] != 'O')
        {
            board[0, 1] = currentPlayer;
            return true;
        }

        if (board[1, 0] != 'X' && board[1, 0] != 'O')
        {
            board[1, 0] = currentPlayer;
            return true;
        }

        if (board[2, 1] != 'X' && board[2, 1] != 'O')
        {
            board[2, 1] = currentPlayer;
            return true;
        }

        if (board[1, 2] != 'X' && board[1, 2] != 'O')
        {
            board[1, 2] = currentPlayer;
            return true;
        }

        return false;
    }
    // Funkcja próbująca zajęć róg, jeśli wolny
    bool TryTakeCornerUpgrade()
    {
        Dictionary<int,int> map = new Dictionary<int,int>();
        // Sprawdzamy rogi w kolejności: lewy górny, prawy górny, lewy dolny, prawy dolny
        if (board[0, 0] != 'X' && board[0, 0] != 'O')
        {
            map.Add(0, (board[0, 1] == 'X' ? 1 : 0 )+( board[0, 2] == 'X' ? 1 : 0) + (board[1, 0] == 'X' ? 1 : 0 )+( board[2, 0] == 'X' ? 1 : 0));
        }

        if (board[0, 2] != 'X' && board[0, 2] != 'O')
        {
            map.Add(1, (board[0, 0] == 'X' ? 1 : 0) +( board[0, 1] == 'X' ? 1 : 0) + (board[1, 2] == 'X' ? 1 : 0) + (board[2, 2] == 'X' ? 1 : 0));
        }

        if (board[2, 0] != 'X' && board[2, 0] != 'O')
        {
            map.Add(2, (board[2, 1] == 'X' ? 1 : 0) + (board[2, 2] == 'X' ? 1 : 0) + (board[0, 0] == 'X' ? 1 : 0 )+ (board[1, 0] == 'X' ? 1 : 0));
        }

        if (board[2, 2] != 'X' && board[2, 2] != 'O')
        {
            map.Add(3,(board[2, 0] == 'X' ? 1 : 0 )+ (board[2, 1] == 'X' ? 1 : 0 )+ (board[0, 2] == 'X' ? 1 : 0) + (board[1, 2] == 'X' ? 1 : 0));
        }
        if (map.Count <= 1) return TryTakeCorner();
        if (map.Count == 2 && map[map.Keys.First()] == 2 && map[map.Keys.Last()] == 2)
        {
            return TryTakeSide();
        }

        //get corner with higest value
        int keyOfMaxValue = map.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
        switch (keyOfMaxValue)
        {
            case 0:
                board[0, 0] = currentPlayer;
                return true;
            case 1:
                board[0, 2] = currentPlayer;
                return true;
            case 2:
                board[2, 0] = currentPlayer;
                return true;
            case 3:
                board[2, 2] = currentPlayer;
                return true;
            default:
                break;
        }
        return false;
    }

    // Funkcja zajmująca dowolne wolne pole, jeśli inne strategie nie zadziałały
    void TakeRandomFreeSpace()
    {
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                if (board[row, col] != 'X' && board[row, col] != 'O')
                {
                    board[row, col] = currentPlayer;
                    return;
                }
            }
        }
    }

    void SwitchPlayer()
    {
        currentPlayer = (currentPlayer == 'X') ? 'O' : 'X';
    }

    bool CheckWin()
    {
        return
            (board[0, 0] == currentPlayer && board[0, 1] == currentPlayer && board[0, 2] == currentPlayer) ||
            (board[1, 0] == currentPlayer && board[1, 1] == currentPlayer && board[1, 2] == currentPlayer) ||
            (board[2, 0] == currentPlayer && board[2, 1] == currentPlayer && board[2, 2] == currentPlayer) ||
            (board[0, 0] == currentPlayer && board[1, 0] == currentPlayer && board[2, 0] == currentPlayer) ||
            (board[0, 1] == currentPlayer && board[1, 1] == currentPlayer && board[2, 1] == currentPlayer) ||
            (board[0, 2] == currentPlayer && board[1, 2] == currentPlayer && board[2, 2] == currentPlayer) ||
            (board[0, 0] == currentPlayer && board[1, 1] == currentPlayer && board[2, 2] == currentPlayer) ||
            (board[0, 2] == currentPlayer && board[1, 1] == currentPlayer && board[2, 0] == currentPlayer);
    }
}
