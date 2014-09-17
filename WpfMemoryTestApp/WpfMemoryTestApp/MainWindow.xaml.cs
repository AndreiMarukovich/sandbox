using System.Windows;

namespace WpfMemoryTestApp
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			var window = new SecondWindow();
			window.ShowDialog();
		}

	}
}
