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

namespace Ampulse.UI.Components
{
    /// <summary>
    /// Amplifier user control.
    /// </summary>
    public partial class Amplifier : UserControl
    {
		private bool power;

		private ImageSource backgroundOn;
		private ImageSource backgroundOff;

		public event EventHandler PowerChanged;
		public event EventHandler DistortionChanged;
		public event EventHandler EqualizerChanged;
		public event EventHandler MasterChanged;
		public event EventHandler PregainChanged;
		public event EventHandler ReverbChanged;

        public Amplifier()
        {
            InitializeComponent();

			// Load on/off backgrounds.
			backgroundOn = new BitmapImage(new Uri("pack://application:,,,/Assets/amp_on.png"));
			backgroundOff = new BitmapImage(new Uri("pack://application:,,,/Assets/amp_off.png"));

			power = true;
        }

		public float GetDistortionValue()
		{
			return distortion.Value;
		}

		public float GetEqualizerValue(int index)
		{
			Slider slider = (Slider)this.FindName("eq" + index.ToString());
			if (slider != null)
				return slider.Value; 
			return 0.5f;
		}

		public float GetMasterValue()
		{
			return master.Value;
		}

		public float GetPregainValue()
		{
			return pregain.Value;
		}

		public float GetReverbValue()
		{
			return reverb.Value;
		}

		public bool IsActive()
		{
			return power;
		}

		private void SwitchPower(object sender, RoutedEventArgs e)
		{
			power = !power;
			if (power)
				background.ImageSource = backgroundOn;
			else
				background.ImageSource = backgroundOff;

			if (PowerChanged != null)
			{
				PowerChanged(this, new EventArgs());
			}
		}

		//
		// Events
		//

		private void DistortionChangedEvent(object sender, EventArgs e)
		{
			if (DistortionChanged != null)
			{
				DistortionChanged(this, new EventArgs());
			}
		}
		private void EqualizerChangedEvent(object sender, EventArgs e)
		{
			if (EqualizerChanged != null)
			{
				EqualizerChanged(this, new EventArgs());
			}
		}

		private void MasterChangedEvent(object sender, EventArgs e)
		{
			if (MasterChanged != null)
			{
				MasterChanged(this, new EventArgs());
			}
		}

		private void PregainChangedEvent(object sender, EventArgs e)
		{
			if (PregainChanged != null)
			{
				PregainChanged(this, new EventArgs());
			}
		}

		private void ReverbChangedEvent(object sender, EventArgs e)
		{
			if (ReverbChanged != null)
			{
				ReverbChanged(this, new EventArgs());
			}
		}
    }
}
