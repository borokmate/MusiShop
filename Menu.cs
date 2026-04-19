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
        private static int GetMenuItemNoClear(string[] menu)
        {
            var pos = Console.GetCursorPosition();
            ConsoleKeyInfo key;
            do
            {
                Console.SetCursorPosition(pos.Left, pos.Top);
                PrintMenu(menu);
                key = Console.ReadKey();
                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        do
                        {
                            if (0 < currItem) currItem--;
                            else currItem = menu.Length - 1;
                        } while (menu[currItem].StartsWith('['));
                        break;
                    case ConsoleKey.DownArrow:
                        do
                        {
                            if (currItem < menu.Length - 1) currItem++;
                            else currItem = 0;
                        } while (menu[currItem].StartsWith('['));
                        break;
                    case ConsoleKey.Spacebar:
                        currItem += 10;
                        currItem = Math.Clamp(currItem, 0, menu.Length - 1);
                        break;
                    case ConsoleKey.Escape:
                        currItem = -1;
                        return 0;
                    default:
                        int found = Array.FindIndex(menu, x => x[0] == key.KeyChar);
                        if (found != -1) currItem = found;
                        break;
                }
            }
            while (key.Key != ConsoleKey.Enter);
            int offset = 0;
            for (int i = 0; i < currItem; i++)
            {
                if (menu[i].StartsWith('[')) offset++;
            }
            currItem -= offset;
            return offset;
        }

        private static int GetMenuItemNoClear(string[] menu, string endingText)
        {
            var pos = Console.GetCursorPosition();
            ConsoleKeyInfo key;
            do
            {
                Console.SetCursorPosition(pos.Left, pos.Top);
                PrintMenu(menu);
                Console.WriteLine(endingText);
                key = Console.ReadKey();
                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        do
                        {
                            if (0 < currItem) currItem--;
                            else currItem = menu.Length - 1;
                        } while (menu[currItem].StartsWith('['));
                    break;
                    case ConsoleKey.DownArrow:
                        do
                        {
                            if (currItem < menu.Length - 1) currItem++;
                            else currItem = 0;
                        } while (menu[currItem].StartsWith('['));
                        break;
                    case ConsoleKey.Spacebar:
                        currItem += 10;
                        currItem = Math.Clamp(currItem, 0, menu.Length - 1);
                        break;
                    case ConsoleKey.Escape:
                        currItem = -1;
                        return 0;
                    default:
                        int found;
                        if (key.KeyChar == ':')
                        {
                            char nextChar = Console.ReadKey().KeyChar;
                            found = Array.FindIndex(menu, x => x[1] == nextChar && x[0] == '[');
                            if (found != -1) found++;
                        }
                        else
                            found = Array.FindIndex(menu, x => x[2] == key.KeyChar);
                        if (found != -1) currItem = found;
                        break;
                }
            }
            while (key.Key != ConsoleKey.Enter);
            int offset = 0;
            for (int i = 0; i < currItem; i++)
            {
                if (menu[i].StartsWith('[')) offset++;
            }
            currItem -= offset;
            return offset;
        }
        private static void CheckBacklog()
        {
            
            currItem = 1;
            while (true)
            {
                Console.Clear();
                var products = Parser.ReadProducts();
                var grouped = products.GroupBy(x => x.Type).OrderBy(g => g.Key);

                var menuItems = new List<string>();

                foreach (var group in grouped)
                {
                    menuItems.Add($"[{group.Key}]"); // header
                    foreach (var product in group.OrderBy(x => x.Name))
                    {
                        menuItems.Add("  " + product.Name);
                    }
                }
                int offset = GetMenuItemNoClear(menuItems.ToArray(), "Press escape to exit");
                if (currItem == -1)
                {
                    currItem = 0;
                    return;
                }

                Console.Clear();
                Console.WriteLine("Chosen product");
                PrintProduct(products[currItem]);
                currItem += offset;
            }
        }
        private static void PrintProduct(Product product, bool space = true)
        {
            Console.WriteLine("Date: " + product.Date);
            Console.WriteLine("Name: " + product.Name);
            Console.WriteLine("Price: " + product.Price);
            Console.WriteLine("Quantity: " + product.Quantity);
            Console.WriteLine("Type: " + product.Type);
            if (space) WaitForSpace();
        }
        private static void AddNewProduct()
        {
            currItem = 0;
            Console.Clear();
            Console.WriteLine("What should the type be?");
            var menu = new List<string>();
            menu.AddRange(Parser.GetTypes());
            menu.Add("New type");
            GetMenuItemNoClear(menu.ToArray());
            if (currItem == -1)
            {
                currItem = 0;
                return;
            }
            string type;
            if (currItem == menu.Count - 1)
            {
                type = GetString("new type: ");
            }
            else
            {
                type = menu[currItem];
            }
            var name = GetString("product name: ");
            if (Parser.ReadProducts().Where(x => x.Name == name && x.Type == type).Count() > 0)
            {
                Console.WriteLine("This product already exists!");
                Console.Write("Add to quantity? (y/n): ");
                if (Console.ReadLine() == "n")
                {
                    return;
                }
                else
                {
                    foreach (var prod in Parser.ReadProducts().Where(x => x.Name == name && x.Type == type))
                    {
                        prod.Quantity++;
                        Parser.ReplaceProduct(prod.Name, prod.Type, prod);
                    }
                    WaitForSpace();
                    return;
                }
            }
            int price = GetANumber("Price", 0);
            int quantity = GetANumber("Quantity", 1);
            DateTime date = GetADate("Release date", 0);
            Product product = new Product(date, name, price, quantity, type);
            Parser.WriteProduct(product);
            Console.WriteLine("Successfully added new product;");
            WaitForSpace();
        }
        private static void EditProducts()
        {
            string[] productMenu;
            while (true)
            {
                Console.Clear();
                var products = Parser.ReadProducts();
                var grouped = products.GroupBy(x => x.Type).OrderBy(g => g.Key);

                var menuItems = new List<string>();

                foreach (var group in grouped)
                {
                    menuItems.Add($"[{group.Key}]"); // header
                    foreach (var product in group.OrderBy(x => x.Name))
                    {
                        menuItems.Add("  " + product.Name);
                    }
                }
                int originalOffset = GetMenuItemNoClear(menuItems.ToArray(), "Press escape to exit");
                if (currItem == -1)
                {
                    currItem = 0;
                    return;
                }
                var itemIndex = currItem;
                var originalName = products[itemIndex].Name;
                var originalType = products[itemIndex].Type;
                currItem = 0;
                while (true)
                {
                    Console.Clear();
                    productMenu = new string[]
                    {
                        $"Date: {products[itemIndex].Date}",
                        $"Name: {products[itemIndex].Name}",
                        $"Price: {products[itemIndex].Price}",
                        $"Quantity: {products[itemIndex].Quantity}",
                        $"Type: {products[itemIndex].Type}"
                    };
                    Console.WriteLine("Chosen product");
                    GetMenuItemNoClear(productMenu, "Press escape to exit");
                    if (currItem == -1)
                    {
                        currItem = 0;
                        break;
                    }
                    var currPos = Console.GetCursorPosition();
                    
                    Console.SetCursorPosition(currPos.Left, currPos.Top - (productMenu.Length - currItem + 1));
                    Console.Write("\x1b[K");
                    Console.Write(productMenu[currItem].Substring(0, productMenu[currItem].IndexOf(':')));
                    switch (currItem)
                    {
                        case 0: products[itemIndex].Date = GetADate("", 0, false); break;
                        case 1: originalName = products[itemIndex].Name; products[itemIndex].Name = GetString("", false);  break;
                        case 2: products[itemIndex].Price = GetANumber("", 0, false); break;
                        case 3: products[itemIndex].Quantity = GetANumber("", 0, false); break;
                        case 4: originalType = products[itemIndex].Type; products[itemIndex].Type = GetString("", false); break;
                    }
                    Parser.ReplaceProduct(originalName, originalType, products[itemIndex]);
                }
                currItem = itemIndex + originalOffset;
            }
        }
        private static void DeleteProducts()
        {
            Console.Clear();
            currItem = 1;
            while (true)
            {
                Console.Clear();
                var products = Parser.ReadProducts();
                var grouped = products.GroupBy(x => x.Type).OrderBy(g => g.Key);

                var menuItems = new List<string>();

                foreach (var group in grouped)
                {
                    menuItems.Add($"[{group.Key}]"); // header
                    foreach (var product in group.OrderBy(x => x.Name))
                    {
                        menuItems.Add("  " + product.Name);
                    }
                }
                int offset = GetMenuItemNoClear(menuItems.ToArray(), "Press escape to exit");
                if (currItem == -1)
                {
                    currItem = 0;
                    return;
                }

                Console.Clear();
                Console.WriteLine($"Delete product {products[currItem].Name} from {products[currItem].Type}");
                Console.Write("(y/n): ");
                var inp = Console.ReadLine();
                if (inp.ToLower() != "no" && inp.ToLower() != "n")
                    Parser.DeleteProduct(products[currItem]);
                currItem += offset;
            }
        }
        private static void ExitProgram()
        {
            Console.Clear();
            Console.WriteLine("Thank you for using our program!");
            Environment.Exit(0);
        }
        private static void WaitForSpace()
        {
            Console.Write("Press space to exit");
            while (Console.ReadKey().Key != ConsoleKey.Spacebar) ;
        }
        private static void WaitForEsc()
        {
            Console.Write("Press escape to exit");
            while (Console.ReadKey().Key != ConsoleKey.Escape) ;
        }
        private static string ReadLineSameLine()
        {
            var sb = new StringBuilder();
            ConsoleKeyInfo key;
            while ((key = Console.ReadKey(intercept: true)).Key != ConsoleKey.Enter)
            {
                if (key.Key == ConsoleKey.Backspace)
                {
                    if (sb.Length > 0)
                    {
                        sb.Remove(sb.Length - 1, 1);
                        Console.Write("\b \b"); // erase the character visually
                    }
                }
                else
                {
                    sb.Append(key.KeyChar);
                    Console.Write(key.KeyChar);
                }
            }
            return sb.ToString();
        }
        private static int GetANumber(string name, int atLeast, bool errorMessage = true)
        {
            var pos = Console.GetCursorPosition();

            int num = -1;
            while (num < atLeast)
            {
                try
                {
                    Console.Write(name == "" ? ": " : $"{name}: ");
                    num = int.Parse(!errorMessage ? ReadLineSameLine() : Console.ReadLine());
                }
                catch
                {
                    if (errorMessage)
                        Console.WriteLine("The input wasn't a legit number!");
                    else
                    {
                        Console.SetCursorPosition(pos.Left, pos.Top);
                        Console.Write("\x1b[K");
                    }
                    num = -1;
                }
            }
            return num;
        }
        private static DateTime GetADate(string name, int atLeast, bool errorMessage = true)
        {
            var pos = Console.GetCursorPosition();

            while (true)
            {
                try
                {
                    Console.Write(name == "" ? ": " : $"{name} (yyyy-MM-dd): ");

                    DateTime date = DateTime.Parse(!errorMessage ? ReadLineSameLine() : Console.ReadLine());
                    if (date.Year >= atLeast)
                        return date;
                    Console.WriteLine($"Date must be from year {atLeast} or later!");
                }
                catch
                {
                    if (errorMessage)
                        Console.WriteLine("The input wasn't a legit date!");
                    else
                    {
                        Console.SetCursorPosition(pos.Left, pos.Top);
                        Console.Write("\x1b[K");
                    }
                }
            } 
        }
        private static string GetString(string name, bool errorMessage = true)
        {
            string? inp = "";
            var pos = Console.GetCursorPosition();
            while (inp == null || inp == "")
            {
                Console.Write(name == "" ? ": " : $"Enter the {name}");
                inp = !errorMessage ? ReadLineSameLine() : Console.ReadLine();
                if (inp == null || inp == "")
                {
                    if (errorMessage)
                        Console.WriteLine("The input wasn't correct!");
                    else
                    {
                        Console.SetCursorPosition(pos.Left, pos.Top);
                        Console.Write("\x1b[K");
                    }
                }
            }
            return inp;
        }
    }
}