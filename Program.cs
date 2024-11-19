using System;
using System.Threading;

class Program1
{
    static void Main()
    {
        // Ensure the login page runs first
        if (!LoginPage())
        {
            Console.WriteLine("Login failed. Exiting program...");
            Thread.Sleep(2000); // Wait for 2 seconds before exiting
            return;
        }

        // Weapon options
        string[] weapons = { "AK-47", "LR300", "MP5A4", "SMG", "M249" };
        bool[] selectedWeapons = new bool[weapons.Length];
        int selectedWeaponIndex = -1; // No weapon selected by default

        // Settings options
        string[] settings = {
            "Randomizer X: 0  Y: 0",
            "Recoil-Ctrl X: 100  Y: 100",
            "AutoGunDetect: False",
            "AutoModDetect: False",
            "Burst-F Mode: False",
            "S-RUN: False",
            "RAPID: False",
            "HIP-F: False",
            "A-AFK: False"
        };
        bool[] toggledSettings = new bool[settings.Length]; // Track toggle state for settings

        int currentIndex = 0;
        bool isSettingsMenu = false; // Tracks whether the user is in the settings menu
        bool recoilThreadRunning = false;
        Thread recoilThread = null;

        while (true)
        {
            Console.Clear();
            if (isSettingsMenu)
                DrawSettingsMenu(settings, toggledSettings, currentIndex);
            else
                DrawInterface(weapons, selectedWeapons, selectedWeaponIndex, currentIndex);

            var key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.UpArrow:
                    currentIndex = Math.Max(currentIndex - 1, 0);
                    break;

                case ConsoleKey.DownArrow:
                    currentIndex = isSettingsMenu
                        ? Math.Min(currentIndex + 1, settings.Length - 1)
                        : Math.Min(currentIndex + 1, weapons.Length - 1);
                    break;

                case ConsoleKey.Spacebar:
                    if (isSettingsMenu)
                    {
                        // Toggle the current setting
                        toggledSettings[currentIndex] = !toggledSettings[currentIndex];
                        settings[currentIndex] = toggledSettings[currentIndex]
                            ? settings[currentIndex].Replace("False", "True")
                            : settings[currentIndex].Replace("True", "False");
                    }
                    else
                    {
                        // Select or deselect weapon
                        for (int i = 0; i < selectedWeapons.Length; i++)
                            selectedWeapons[i] = false; // Deselect all weapons

                        selectedWeapons[currentIndex] = true;
                        selectedWeaponIndex = currentIndex;
                    }
                    break;

                case ConsoleKey.Tab:
                    isSettingsMenu = !isSettingsMenu; // Toggle between weapons and settings menu
                    currentIndex = 0; // Reset index when switching menus
                    break;

                case ConsoleKey.Escape:
                    // Ensure any running recoil thread is terminated before exiting
                    if (recoilThreadRunning && recoilThread != null && recoilThread.IsAlive)
                    {
                        recoilThread.Abort();
                    }
                    ExitProgram();
                    return;

                case ConsoleKey.R when !isSettingsMenu:
                    if (selectedWeaponIndex >= 0)
                    {
                        if (!recoilThreadRunning)
                        {
                            recoilThread = new Thread(() =>
                            {
                                RecoilManager.SimulateRecoil(selectedWeaponIndex);
                            });
                            recoilThread.IsBackground = true;
                            recoilThread.Start();
                            recoilThreadRunning = true;
                        }
                    }
                    else
                    {
                        Console.WriteLine("\nPlease select a weapon first!");
                        Console.ReadKey();
                    }
                    break;

                default:
                    break;
            }

            // Check if the right mouse button is released to stop the recoil thread
            if (recoilThreadRunning && (RecoilManager.IsRecoilActive() == false))
            {
                recoilThreadRunning = false;
                recoilThread = null;
            }
        }
    }

    private static void DrawSettingsMenu(string[] settings, bool[] toggledSettings, int currentIndex)
    {
        throw new NotImplementedException();
    }

    static bool LoginPage()
    {
        string correctUsername = "admin";
        string correctPassword = "password123";

        while (true)
        {
            Console.Clear();
            Console.WriteLine("LOGIN PAGE");
            Console.WriteLine(new string('═', 30));

            Console.Write("Username: ");
            string username = Console.ReadLine();

            Console.Write("Password: ");
            string password = MaskPassword();

            if (username == correctUsername && password == correctPassword)
            {
                Console.Clear();
                Console.WriteLine("Login successful! Press any key to continue...");
                Console.ReadKey();
                return true;
            }
            else
            {
                Console.WriteLine("\nInvalid credentials. Press ESC to exit or any other key to retry.");
                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Escape)
                {
                    return false;
                }
            }
        }
    }

    static string MaskPassword()
    {
        string password = "";
        ConsoleKeyInfo key;

        do
        {
            key = Console.ReadKey(true);

            if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
            {
                password += key.KeyChar;
                Console.Write("*");
            }
            else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password = password[..^1];
                Console.Write("\b \b");
            }
        } while (key.Key != ConsoleKey.Enter);

        Console.WriteLine(); // Move to next line after Enter
        return password;
    }

    static void DrawInterface(string[] weapons, bool[] selectedWeapons, int selectedWeaponIndex, int currentIndex)
    {
        Console.Title = "Rust No-Recoil Macro - Weapon Menu";
        Console.ForegroundColor = ConsoleColor.Red;

        // Banner
        PrintBanner();

        // Separator
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n" + new string('═', 70));
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("SELECT A WEAPON TO SIMULATE RECOIL".PadLeft(45));
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(new string('═', 70));

        // Weapon List
        for (int i = 0; i < weapons.Length; i++)
        {
            if (i == currentIndex) Console.BackgroundColor = ConsoleColor.DarkGray;

            Console.WriteLine($"[{(selectedWeapons[i] ? "x" : " ")}] {weapons[i]}");
            Console.ResetColor();
        }

        Console.WriteLine("\n" + new string('═', 70));
        Console.WriteLine("\nUse arrow keys to navigate, SPACE to select a weapon.");
        Console.WriteLine("Press R to simulate recoil, TAB to switch to Settings menu, ESC to exit.");
    }

    static void ExitProgram()
    {
        Console.Clear();
        Console.WriteLine("Thank you for using the Rust No-Recoil Macro! Press any key to exit...");
        Console.ReadKey();
    }

    static void PrintBanner()
    {
        Console.WriteLine(" ██████╗ ██╗   ██╗███████╗██████╗     ███╗   ███╗███████╗███╗   ██╗");
        Console.WriteLine("██╔════╝ ██║   ██║██╔════╝██╔══██╗    ████╗ ████║██╔════╝████╗  ██║");
        Console.WriteLine("██║  ███╗██║   ██║█████╗  ██████╔╝    ██╔████╔██║█████╗  ██╔██╗ ██║");
        Console.WriteLine("██║   ██║██║   ██║██╔══╝  ██╔═══╝     ██║╚██╔╝██║██╔══╝  ██║╚██╗██║");
        Console.WriteLine("╚██████╔╝╚██████╔╝███████╗██║         ██║ ╚═╝ ██║███████╗██║ ╚████║");
        Console.WriteLine(" ╚═════╝  ╚═════╝ ╚══════╝╚═╝         ╚═╝     ╚═╝╚══════╝╚═╝  ╚═══╝");
    }
}
