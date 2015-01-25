using Ampulse.UI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;

namespace Ampulse
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        public const string NAME = "Ampulse";
		public const string VERSION = "0.0.1";
		public const string AUTHOR = "Cyriaque Skrapits";

		private MainWindow window;


		public App()
		{
			Audio.Initialize();
			Audio.Start();

			window = new MainWindow();
			window.Show();
		}

		public static void Debug(object sender, string message)
		{
			System.Console.WriteLine("[" + sender.GetType().Name + "] " + message);
		}
    }
}
