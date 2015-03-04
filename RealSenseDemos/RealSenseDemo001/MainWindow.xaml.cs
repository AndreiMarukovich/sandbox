using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace RealSenseDemo001
{
	public partial class MainWindow : Window
	{
		private readonly PXCMSenseManager _senseManager;
		private readonly PXCMHandConfiguration _handConfig;
		private readonly CancellationTokenSource _cancellationTokenSource;
		private readonly Task _task;

		public MainWindow()
		{
			InitializeComponent();
			
			_senseManager = PXCMSenseManager.CreateInstance();

			_senseManager.EnableHand();

			var handManager = _senseManager.QueryHand();
			_handConfig = handManager.CreateActiveConfiguration();
			_handConfig.EnableGesture("thumb_up");
			_handConfig.EnableGesture("thumb_down");
			_handConfig.EnableAllAlerts();
			_handConfig.ApplyChanges();

			var status = _senseManager.Init();
			if (status >= pxcmStatus.PXCM_STATUS_NO_ERROR)
			{
				_cancellationTokenSource = new CancellationTokenSource();
				_task = Task.Factory.StartNew(x => ProcessInput(_cancellationTokenSource.Token), 
					TaskCreationOptions.LongRunning,
					_cancellationTokenSource.Token);
			}
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);

			if (_cancellationTokenSource != null)
				_cancellationTokenSource.Cancel();
			
			if (_task != null)
				_task.Wait();

			_handConfig.Dispose();
			_senseManager.Dispose();
		}

		private void ProcessInput(CancellationToken token)
		{
			// Wait for available data
			while (!token.IsCancellationRequested &&
				_senseManager.AcquireFrame(true) >= pxcmStatus.PXCM_STATUS_NO_ERROR) 
			{
				try
				{
					var handQuery = _senseManager.QueryHand(); 
					if (handQuery != null)
					{
						var handData = handQuery.CreateOutput(); // Get processing results
						handData.Update();

						PXCMHandData.GestureData gestureData;
						if (handData.IsGestureFired("thumb_down", out gestureData))
						{
							Dispatcher.Invoke(ThumbDown);
						}
						else if (handData.IsGestureFired("thumb_up", out gestureData))
						{
							Dispatcher.Invoke(ThumbUp);
						}
						handData.Dispose();
					}
				}
				finally 
				{
					_senseManager.ReleaseFrame();
				}
			}
		}

		private void ThumbUp()
		{
			ThumbsUp.Visibility = Visibility.Visible;
			ThumbsDown.Visibility = Visibility.Collapsed;
		}

		private void ThumbDown()
		{
			ThumbsUp.Visibility = Visibility.Collapsed;
			ThumbsDown.Visibility = Visibility.Visible;
		}
	}
}
