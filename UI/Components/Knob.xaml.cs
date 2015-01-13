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
	/// Knob interaction logic.
	/// </summary>
	public partial class Knob : UserControl
	{
		private readonly double MAX = 135.0; // Maximum angle

		private bool rotating;
		private float sensivity;
		private float value;
		private RotateTransform transform;

		public float Sensivity
		{
			get { return sensivity; }
			set { this.sensivity = value; }
		}
		public float Value
		{
			get	{ return value;	}
			set	{ this.value = value; }
		}

		public Knob()
		{
			InitializeComponent();

			rotating = false;
			sensivity = 0.2f;
			value = 0.5f;

			transform = new RotateTransform();
			transform.CenterX = 0.5;
			transform.CenterY = 0.5;
			transform.Angle = 0.0;

			knob.RelativeTransform = transform;
		}

		private void DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
		{
			rotating = true;
		}

		private void DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
		{
			rotating = false;
		}

		private void DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
		{
			double newAngle = transform.Angle + (e.VerticalChange * sensivity);
			bool atBounds = Math.Abs(newAngle) > MAX;

			// Rotate unless while we're in the bounds.
			if (rotating && !atBounds)
				transform.Angle = newAngle;

			// Set the value.
			value = (float)((transform.Angle + MAX) / (MAX * 2));
		}
	}
}
