using System;
using System.Windows;

namespace WpfMemoryTestApp
{
	public partial class SecondWindow : Window
	{
		private double[] _data = new double[126214];

		public SecondWindow()
		{
			InitializeComponent();

			Application.Current.Deactivated += AppDeactivated;
		}

		void AppDeactivated(object sender, EventArgs e)
		{
			// do something
		}
	}
}


