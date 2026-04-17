using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace MusiShop
{
    public static class Menu
    {
        public static readonly string[] mainMenu =
        {
            "Check Backlog",
            "Add new product",
            "Edit products",
            "Delete products",
            "Validate files",
            "Open command line",
            "Exit program"
        };
        private static readonly List<string> usedCommands = new();
        // private static readonly Action[] mainMenuFuncs =
        // {
        //     CheckBacklog,
        //     AddNewProduct,
        //     EditProducts,
        //     DeleteProducts,
        //     ValidateFiles,
        //     CommandLineInterpreter,
        //     ExitProgram
        // };
        private static readonly string[] currMenu = mainMenu;
        // private static readonly Action[] currMenuFuncs = mainMenuFuncs;
        private static int currItem = 0;
        private static void PrintMenu(string[] menu)
        {
            int index = 0;
            foreach (var item in menu)
            {
                if (item.StartsWith("["))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(item);
                    Console.ForegroundColor = ConsoleColor.White;
                    index++;
                    continue;
                }
                if (index++ == currItem)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                Console.Write(item);
                Console.BackgroundColor = ConsoleColor.Black; // dumb ahh thing ngl
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine();
            }
        }
        public static void MainLoop()
        {
            while (true)
            {
                Console.Clear();
                GetMenuItem(currMenu);
                // currMenuFuncs[currItem]();
            }
        }
        private static void GetMenuItem(string[] menu)
        {
            ConsoleKeyInfo key;
            do
            {
                PrintMenu(menu);
                key = Console.ReadKey();
                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (0 < currItem) currItem--;
                        else currItem = menu.Length - 1;
                        break;
                    case ConsoleKey.DownArrow:
                        if (currItem < menu.Length - 1) currItem++;
                        else currItem = 1;
                        break;
                    default:
                        int found = Array.FindIndex(menu, x => x[0] == key.KeyChar);
                        if (found != -1) currItem = found;
                        break;
                }
                Console.Clear();
            }
            while (key.Key != ConsoleKey.Enter);
        }
    }
}