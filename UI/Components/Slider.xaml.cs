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
	/// Logique d'interaction pour Slider.xaml
	/// </summary>
	public partial class Slider : UserControl
	{
		public event EventHandler ValueChanged;

		private bool dragged;
		private float value = 0.5f;

		public float Value
		{
			get { return value; }
			set { SetValue(value); }
		}

		public Slider()
		{
			InitializeComponent();

			dragged = false;
		}

		private void OnMouseDown(object sender, MouseButtonEventArgs e)
		{
			dragged = true;
			Mouse.Capture(slider);
		}

		private void OnMouseMove(object sender, MouseEventArgs e)
		{
			if (dragged)
			{
				double max = this.ActualHeight - slider.ActualHeight;
				double position = e.GetPosition(this).Y;
				if (position < 0) position = 0.0;
				if (position + slider.ActualHeight >= this.ActualHeight) position = max;
				Canvas.SetTop(slider, position);

				value = (float)(position / max);
			}
		}

		private void OnMouseUp(object sender, MouseButtonEventArgs e)
		{
			dragged = false;
			Mouse.Capture(null);
		}

		private void SetValue(float value)
		{
			if (value < 0.0f) value = 0.0f;
			if (value > 1.0f) value = 1.0f;
			this.value = value;

			Canvas.SetTop(slider, this.value * (this.ActualHeight - slider.ActualHeight));

			if (ValueChanged != null)
			{
				ValueChanged(this, EventArgs.Empty);
			}
		}

		private void OnInitialized(object sender, EventArgs e)
		{
			SetValue(value);
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			SetValue(value);
		}
	}
}
