using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using University.ConsoleApp.DataAccess;
using University.ConsoleApp.Services;

namespace University.ConsoleApp;

public class Program
{

    static void Main(string[] args)
    {

        var _context = new ApplicationDbContext();
        if (_context.Database.EnsureCreated())
            _context.Database.Migrate();


        while (true)
        {
            var _program = new ProgramApi();
            _program.LoadAllPathForSearch();
            Console.Clear();
            Console.Title = "Menu";
            Console.WriteLine("==================Menu==================");
            Console.WriteLine(">Enter a number ");
            Console.WriteLine("1. Search");
            Console.WriteLine("2. Settings");
            Console.WriteLine("0. Exit");
            var userSelection = Console.ReadLine();
            switch (userSelection)
            {
                case "2":
                    SettingsApp();
                    break;
                case "1":
                    SearchApp();
                    break;
                case "0":
                    return;
            }
        }
    }


    private static void SearchApp()
    {
        while (true)
        {
            Console.Title = "Search";
            var _program = new ProgramApi();
            Console.Clear();
            Console.WriteLine("==================Search==================");
            Console.WriteLine(">Enter a text for search or 0 to exit");
            Console.WriteLine("Search for:");
            var userSelection = Console.ReadLine();

            if (userSelection == "0")
                return;

            if (string.IsNullOrEmpty(userSelection)) continue;

            var result = _program.Search(userSelection);
            Console.Title = $"Result: {result.Count}";
            int n = 1;

            Console.WriteLine("Outputs:");
            if (result.Count == 0)
            {
                Console.WriteLine("no matched file found");
                Console.Read();
                userSelection = Console.ReadLine();

                if (userSelection == "0") return;
            }
            else
            {
                foreach (var item in result)
                {
                    Console.WriteLine($"{n}. {item.Path}\t{item.Score}");
                    n++;
                };

                Console.WriteLine(">Enter number of desired file to be open or 0 to exit");
                userSelection = Console.ReadLine();

                if (userSelection == "0") return;

                int userInputToInt;
                int.TryParse(userSelection, out userInputToInt);

                if (0 <= userInputToInt - 1 && userInputToInt - 1 < result.Count)
                {
                    var thread = new Thread(() => { Process.Start("explorer.exe", result[userInputToInt - 1].Path); }) { IsBackground = true };

                    thread.Start();
                }
                else
                    Console.WriteLine("bad number input");
            }
        }
    }

    private static void SettingsApp()
    {
        while (true)
        {
            Console.Title = "Settings";
            var _program = new ProgramApi();
            Console.Clear();
            Console.WriteLine("==================Settings==================");
            Console.WriteLine(">Enter a number ");
            Console.WriteLine("1. See/Add directory");
            Console.WriteLine("2. Index Files");
            Console.WriteLine("0. Exit");
            var userSelection = Console.ReadLine();
            switch (userSelection)
            {
                case "2":
                    _program.IndexFiles();
                    Console.Read();
                    break;
                case "1":
                    ManageDirectoriesApp();
                    break;
                case "0":
                    return;
            }
        }
    }

    private static void ManageDirectoriesApp()
    {
        while (true)
        {
            Console.Title = "Dirs";
            var _program = new ProgramApi();
            var dirs = _program.ReadAllPathForSearch();
            Console.Clear();
            Console.WriteLine("==================Directories==================");
            Console.WriteLine("0. Exit");
            var n = 1;
            foreach (var dir in dirs)
            {
                Console.WriteLine($"{n}.{dir.Address}");
                n++;
            }

            Console.WriteLine(">>To add new directory enter the address or directory number to be removed or enter 0 to exit");
            var userSelection = Console.ReadLine();
            int userSelectionInt;
            var isNum = int.TryParse(userSelection, out userSelectionInt);

            if (!isNum)
            {

                _program.AddToPathForSearch(userSelection.ToLower());
                continue;
            }

            if (userSelectionInt == 0)
                return;

            _program.RemoveFromPathForSearch(userSelectionInt);
        }
    }
}