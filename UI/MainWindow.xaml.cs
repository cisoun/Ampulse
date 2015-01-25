using CSCore.CoreAudioAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ampulse.UI
{
    /// <summary>
    /// Main window.
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

			// Change the title.
            this.Title = App.NAME + " " + App.VERSION + " by Cyriaque Skrapits";

			// Load availables audio devices.
			List<MMDevice> devices = Audio.GetDevices();
			for (int i = 0; i < devices.Count(); i++)
				AddDevice(devices.ElementAt(i).FriendlyName);

			// Load availables impulse responses.
			List<string> impulseResponses = Audio.GetImpulseResponses();
			foreach (string impulseResponse in impulseResponses)
				AddImpulseResponse(impulseResponse);
        }
		 
		public void AddDevice(string name)
		{
			ComboBoxItem item = new ComboBoxItem();
			item.Content = name;
			devicesList.Items.Add(item);
		}

		public void AddImpulseResponse(string name)
		{
			ListBoxItem item = new ListBoxItem();
			item.Content = name.Substring(3, name.Length - 7);
			irList.Items.Add(item);
		}

		public void BindImpulseResponses(List<string> impulseResponses)
		{
			irList.ItemsSource = impulseResponses;
		}

		private void OnDeviceChanged(object sender, SelectionChangedEventArgs e)
		{
			Audio.SelectDevice(devicesList.SelectedIndex);
		}

		private void PowerChanged(object sender, EventArgs e)
		{
			Audio.SetActive(amplifier.IsActive());
		}

		private void DistortionChanged(object sender, EventArgs e)
		{
			Audio.SetDistortion(amplifier.GetDistortionValue() * 100.0f);
		}

		private void EqualizerChanged(object sender, EventArgs e)
		{
			for (int i = 0; i < 10; i++)
				Audio.SetEqualizer(i, amplifier.GetEqualizerValue(i) * 12.0f);
		}

		private void PregainChanged(object sender, EventArgs e)
		{
			Audio.SetPregain(amplifier.GetPregainValue());
		}

		private void MasterChanged(object sender, EventArgs e)
		{
			Audio.SetMaster(amplifier.GetMasterValue());
		}

		private void ReverbChanged(object sender, EventArgs e)
		{
			Audio.SetReverb(amplifier.GetReverbValue());
		}

		private void window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			Audio.Stop();
		}
    }
}
