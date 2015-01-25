using CSCore;
using CSCore.CoreAudioAPI;
using CSCore.SoundIn;
using CSCore.SoundOut;
using CSCore.Streams;
using CSCore.Streams.Effects;
using CSCore.Streams.SampleConverter;
using CSCore.MediaFoundation;
using CSCore.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Ampulse
{
	/// <summary>
	/// Capture, modify and read the input audio signal.
	/// 
	/// TODO :
	///		- Add convolver (I did not have the time to do that).
	///		- Optimize latency.
	///		- See why the equalizer doesn't seem to work.
	///		- See why the audio thread sometimes crashes (error 259).
	/// </summary>
	class Audio
	{
		private static int selectedDevice;
		private static List<MMDevice> devices;
		private static List<string> impulseResponses;

		private static WasapiCapture capture;
		private static ISoundOut output;
		private static SoundInSource sourceIn;

		private static GainSource pregain;
		private static Equalizer equalizer;
		private static DmoDistortionEffect distortionEffect;
		private static DmoWavesReverbEffect reverbEffect;
		private static VolumeSource master;

		/// <summary>
		/// Initialize the audio interface.
		/// </summary>
		public static void Initialize()
		{
			LoadDevices();
			LoadImpulseResponses();
		}

		/// <summary>
		/// Get loaded devices.
		/// </summary>
		/// <returns>Devices</returns>
		public static List<MMDevice> GetDevices()
		{
			return devices;
		}

		/// <summary>
		/// Get the available impulse responses.
		/// </summary>
		/// <returns>Impulse responses</returns>
		public static List<string> GetImpulseResponses()
		{
			return impulseResponses;
		}

		/// <summary>
		/// Check which devices are availables.
		/// </summary>
		private static void LoadDevices()
		{
			devices = new List<MMDevice>();

			using (var deviceEnumerator = new MMDeviceEnumerator())
			using (var deviceCollection = deviceEnumerator.EnumAudioEndpoints(DataFlow.Capture, DeviceState.Active))
			{
				foreach (var device in deviceCollection)
				{
					var deviceFormat = WaveFormatFromBlob(device.PropertyStore[
						new PropertyKey(new Guid(0xf19f064d, 0x82c, 0x4e27, 0xbc, 0x73, 0x68, 0x82, 0xa1, 0xbb, 0x8e, 0x4c), 0)].BlobValue);
					devices.Add(device);
				}
			}
		}

		/// <summary>
		/// Check if some impulses responses are availables.
		/// </summary>
		private static void LoadImpulseResponses()
		{
			impulseResponses = new List<string>();

			string[] filePaths = Directory.GetFiles(@"IR");
			foreach(string file in filePaths)
			{
				impulseResponses.Add(file);
			}
		}

		public static void SetDistortion(float edge)
		{
			if (edge < DmoDistortionEffect.EdgeMin)
				edge = DmoDistortionEffect.EdgeMin;
			if (edge > DmoDistortionEffect.EdgeMax)
				edge = DmoDistortionEffect.EdgeMax;

			distortionEffect.Edge = edge;
		}
		public static void SetEqualizer(int index, float gainDB)
		{
			equalizer.SampleFilters[index].SetGain(gainDB);
		}

		public static void SetMaster(float value)
		{
			if (value > 1.0f)
				value = 1.0f;
			if (value < 0.0f)
				value = 0.0f;

			master.Volume = value;
		}

		public static void SetPregain(float value)
		{
			if (value > 1.0f)
				value = 1.0f;
			if (value < 0.0f)
				value = 0.0f;

			pregain.Gain = value * 2.0f;
		}

		public static void SetReverb(float value)
		{
			if (value > 1.0f)
				value = 1.0f;
			if (value < 0.001f)
				value = 0.001f;

			value *= DmoWavesReverbEffect.ReverbTimeMax;

			// By pass if reverb is too short.
			reverbEffect.IsEnabled = value > 50f;
		}

		/// <summary>
		/// Define if the amplifier is active.
		/// </summary>
		/// <param name="active"></param>
		public static void SetActive(bool active)
		{
			if (active)
				Start();
			else
				Stop();
		}

		/// <summary>
		/// Audio routine.
		/// </summary>
		public static void Start()
		{
			// Capture
			capture = new WasapiCapture();
			capture.Device = devices.ElementAt(selectedDevice);
			capture.Initialize();
			
			// Source In
			sourceIn = new SoundInSource(capture);


			//
			// Effects chain
			//

			// Pregain
			pregain = new GainSource(sourceIn.ToMono());

			// EQ
			equalizer = Equalizer.Create10BandEqualizer(pregain.ToWaveSource(16)); // 16 bits (good enough)

			// Distortion
			distortionEffect = new DmoDistortionEffect(equalizer.ToWaveSource(16)); // 16 bits (good enough)

			// Reverb
			reverbEffect = new DmoWavesReverbEffect(distortionEffect.ToMono());

			// Master
			master = new VolumeSource(reverbEffect.ToMono());


			// Output
			if (WasapiOut.IsSupportedOnCurrentPlatform)
				output = new WasapiOut();
			else
				output = new DirectSoundOut();
			output.Initialize(master.ToWaveSource(16)); // 16 bits (good enough)

			// Start the capture/output.
			capture.Start();
			output.Play();
		}

		/// <summary>
		/// Stop the audio routine.
		/// </summary>
		public static void Stop()
		{
			if (capture != null)
			{
				capture.Stop();
				capture.Dispose();
				sourceIn.Dispose();
				pregain.Dispose();
				equalizer.Dispose();
				distortionEffect.Dispose();
				reverbEffect.Dispose();
				master.Dispose();
				output.Stop();
				output.Dispose();
			}
		}

		/// <summary>
		/// Select a device by its index in "devices".
		/// </summary>
		/// <param name="index"></param>
		public static void SelectDevice(int index)
		{
			if (devices == null)
				return;

			selectedDevice = index;

			// Restart the audio routine.
			Stop();
			Start();
		}

		/// <summary>
		/// Uuuh...
		/// See this : https://cscore.codeplex.com/SourceControl/latest#Samples/Recorder/MainWindow.cs
		/// </summary>
		/// <param name="blob"></param>
		/// <returns>Something.</returns>
		private static WaveFormat WaveFormatFromBlob(Blob blob)
		{
			if (blob.Length == 40)
				return (WaveFormat)Marshal.PtrToStructure(blob.Data, typeof(WaveFormatExtensible));
			return (WaveFormat)Marshal.PtrToStructure(blob.Data, typeof(WaveFormat));
		}
	}
}
