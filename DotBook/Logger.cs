using System;
using System.Drawing;
using Console = Colorful.Console;

namespace DotBook
{
    public static class Logger
    {
        /// <summary>
        /// Writes a lime-colored message
        /// </summary>
        /// <param name="message"></param>
        public static void Success(string message) =>
            Console.WriteLine(message, Color.Lime);

        /// <summary>
        /// Writes a white-colored message
        /// </summary>
        /// <param name="message"></param>
        public static void Info(string message) =>
            Console.WriteLine(message, Color.White);

        /// <summary>
        /// Writes a message with default color
        /// </summary>
        /// <param name="message"></param>
        public static void Log(string message) =>
            Console.WriteLine(message);

        /// <summary>
        /// Writes a red-colored message and exits the application
        /// </summary>
        /// <param name="message"></param>
        /// <param name="errorCode"></param>
        public static void Fatal(string message, int errorCode = -1)
        {
            Console.WriteLine(message, Color.Red);
            Environment.Exit(errorCode);
        }
    }
}
