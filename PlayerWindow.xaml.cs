using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace Player
{
	/// <summary>
	/// Interaction logic for PlayerWindow.xaml
	/// </summary>
	public partial class PlayerWindow : Window
	{
		public event EventHandler OnMediaOpended;

		public Duration NaturalDuration
		{
			get { return mediaElement.NaturalDuration; }
		}

		public TimeSpan Position
		{
			get { return mediaElement.Position; }
			set { mediaElement.Position = value; }
		}

		public double Volume
		{
			get { return mediaElement.Volume; }
			set { mediaElement.Volume = value; }

		}

		public bool IsClosed { get; set; }
		public PlayerWindow()
		{
			var screens = Screen.AllScreens;
			
			var primary = screens.FirstOrDefault(s => s.Primary);

			var secundary = screens.DefaultIfEmpty(primary).FirstOrDefault(s => !s.Primary);
			this.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;

			this.Top = secundary.WorkingArea.Top;
			this.Left = secundary.WorkingArea.Left;

			//this.Width = secundary.WorkingArea.Width;
			//this.Height = secundary.WorkingArea.Height;

			this.IsClosed = false;
			InitializeComponent();
		}


		internal void InicializarMedia(string filename)
		{
			mediaElement.Source = new Uri(filename);
		}

		internal void Play()
		{
			mediaElement.Play();
			mediaElement.Visibility = System.Windows.Visibility.Visible;
		}

		internal void Pause()
		{
			mediaElement.Pause();
		}

		internal void Stop()
		{
			mediaElement.Stop();
			mediaElement.Visibility = System.Windows.Visibility.Collapsed;
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			this.IsClosed = true;
			mediaElement.Stop();
		}

		private void mediaElement_MediaEnded(object sender, RoutedEventArgs e)
		{
			//this.Close();
			mediaElement.Visibility = System.Windows.Visibility.Collapsed;
		}

		private void mediaElement_MediaOpened(object sender, RoutedEventArgs e)
		{
			if (OnMediaOpended != null)
			{
				OnMediaOpended(this, e);
			}
		}
	}
}
