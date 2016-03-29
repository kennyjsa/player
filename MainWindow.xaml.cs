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
using Microsoft.Win32;
using System.Windows.Forms;
using System.ComponentModel;
using System.Windows.Threading;



namespace Player
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{

		private bool _isVideoPronto;
		public bool IsVideoPronto
		{
			get { return _isVideoPronto; }
			set
			{
				_isVideoPronto = value;
				btnPlay.IsEnabled = value;
				btnPause.IsEnabled = value;
				btnStop.IsEnabled = value;
			}
		}
		public DispatcherTimer timer;

		public MainWindow()
		{
			InitializeComponent();
			InitializePlayerWindow();

			this.IsVideoPronto = false;

			timer = new DispatcherTimer();
			timer.Interval = TimeSpan.FromMilliseconds(300);
			timer.Tick += timer_Tick;
		}

		void timer_Tick(object sender, EventArgs e)
		{
			if (!isDragging)
			{
				sliderTempo.Value = player.Position.TotalSeconds;

				if (player.NaturalDuration.HasTimeSpan)
					tempoVideo.Content = player.Position.ToString(@"hh\:mm\:ss") + " / " + player.NaturalDuration.TimeSpan.ToString(@"hh\:mm\:ss");
			}
		}

		private void InitializePlayerWindow()
		{
			player = new PlayerWindow();
			player.OnMediaOpended += player_OnMediaOpended;

			if (!String.IsNullOrWhiteSpace(caminhoArquivo))
				player.InicializarMedia(caminhoArquivo);
		}

		void player_OnMediaOpended(object sender, EventArgs e)
		{
			lblArquivo.Content = System.IO.Path.GetFileName(caminhoArquivo);

			if (player.NaturalDuration.HasTimeSpan)
			{
				sliderTempo.Value = 0;
				sliderTempo.Maximum = player.NaturalDuration.TimeSpan.TotalSeconds;
				sliderTempo.SmallChange = 1;
				sliderTempo.LargeChange = Math.Min(10, player.NaturalDuration.TimeSpan.Seconds / 10);
			}
			this.IsVideoPronto = true;

			player.Volume = sliderVolume.Value / 100d;
			timer.Start();

			player.Show();
			player.WindowState = System.Windows.WindowState.Maximized;
		}

		private void InicializeMedia()
		{
			Stop();
			player.InicializarMedia(caminhoArquivo);
		}

		private void Stop()
		{
			if (!String.IsNullOrWhiteSpace(caminhoArquivo))
			{
				player.Stop();

				timer.Stop();
				sliderTempo.Value = 0;
			}
		}

		string caminhoArquivo = string.Empty;
		PlayerWindow player = null;
		private bool isDragging;

		private void btnPlay_Click(object sender, RoutedEventArgs e)
		{
			if (!String.IsNullOrWhiteSpace(caminhoArquivo))
			{
				if (player.IsClosed)
				{
					InitializePlayerWindow();
				}

				timer.Start();
				
				player.Play();
			}
		}

		private void btnPause_Click(object sender, RoutedEventArgs e)
		{
			if (!String.IsNullOrWhiteSpace(caminhoArquivo))
			{
				player.Pause();
			}
		}

		private void btnStop_Click(object sender, RoutedEventArgs e)
		{
			Stop();
		}

		private void btnAddMedia_Click(object sender, RoutedEventArgs e)
		{
			Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
			ofd.AddExtension = true;
			ofd.DefaultExt = "*.*";
			ofd.Filter = "Media(*.*)|*.*";

			if (ofd.ShowDialog().GetValueOrDefault(false))
			{
				Stop()

				this.caminhoArquivo = ofd.FileName;
				InicializeMedia();
			}
		}

		private void Window_Drop(object sender, System.Windows.DragEventArgs e)
		{
			if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
			{
				string[] files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);

				this.caminhoArquivo = files.DefaultIfEmpty(string.Empty).FirstOrDefault();
				InicializeMedia();
			}
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			App.Current.Shutdown();
		}

		private void sliderTempo_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
		{
			isDragging = true;
		}

		private void sliderTempo_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
		{
			isDragging = false;
			player.Position = TimeSpan.FromSeconds(sliderTempo.Value);
		}

		private void sliderVolume_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
		{
			player.Volume = sliderVolume.Value / 100d;
		}
	}
}