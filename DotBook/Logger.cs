using System;
using System.Drawing;
using Console = Colorful.Console;

namespace DotBook
{
    public static class Logger
    {
        /// <summary>
        /// Writes a lime-colored success message
        /// </summary>
        /// <param name="message"></param>
        public static void Success(string message) =>
            Console.WriteLine($"[GOOD] {message}", Color.Lime);

        /// <summary>
        /// Writes a white-colored information message
        /// </summary>
        /// <param name="message"></param>
        public static void Info(string message) =>
            Console.WriteLine($"[INFO] {message}", Color.White);

        /// <summary>
        /// Writes a message with default color
        /// </summary>
        /// <param name="message"></param>
        public static void Log(string message) =>
            Console.WriteLine($"[INFO] {message}");

        /// <summary>
        /// Writes a yellow-colored warning message
        /// </summary>
        /// <param name="message"></param>
        public static void Warning(string message) =>
            Console.WriteLine($"[WARN] {message}", Color.Yellow);

        /// <summary>
        /// Writes a red-colored error message and exits the application
        /// </summary>
        /// <param name="message"></param>
        /// <param name="errorCode"></param>
        public static void Fatal(string message, int errorCode = -1)
        {
            Console.WriteLine($"[ERROR] {message}", Color.Red);
            Environment.Exit(errorCode);
        }
    }
}
