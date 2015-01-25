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
	/// 
	/// Value goes from 0.0 (left) to 1.0 (right).
	/// 
	/// TODO : Improve the drag system (only works with vertical mouse position).
	/// </summary>
	public partial class Knob : UserControl
	{
		private readonly float MAX = 135.0f; // Maximum angle

		public event EventHandler ValueChanged;

		private bool turned;
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
			set	{ SetValue(value); }
		}

		public Knob()
		{
			InitializeComponent();

			turned = false;
			sensivity = 0.2f;
			value = 0.5f;

			transform = new RotateTransform();
			transform.CenterX = 0.5;
			transform.CenterY = 0.5;
			transform.Angle = 0.0;

			knob.RelativeTransform = transform;
		}

		private void OnDragStart(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
		{
			turned = true;
		}

		private void OnDragComplete(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
		{
			turned = false;
		}

		private void OnDragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
		{
			float newAngle = (float)(transform.Angle + (e.VerticalChange * sensivity));
			bool atBounds = Math.Abs(newAngle) > MAX;

			// Rotate and change the value unless while we're in the bounds.
			if (turned && !atBounds)
				SetValue((newAngle + MAX) / (2.0f * MAX));

			// Event
			if (ValueChanged != null)
			{
				ValueChanged(this, new EventArgs());
			}
		}

		public void SetValue(float value)
		{
			if (value < 0.0f)
				value = 0.0f;
			if (value > 1.0f)
				value = 1.0f;

			this.value = value;

			// Rotate knob
			transform.Angle = (value * MAX * 2) - MAX;
		}
	}
}
