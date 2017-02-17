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
using CoreTweet;

namespace MyTwitter {
	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow : Window {
		Tokens tokens = null;
		// コンストラクタ
		public MainWindow() {
			InitializeComponent();
		}
		// メニュー操作
		private void LoginMenu_Click(object sender, RoutedEventArgs e) {
			var session = OAuth.Authorize(Consumer.Key, Consumer.Secret);
			System.Diagnostics.Process.Start(session.AuthorizeUri.AbsoluteUri);
			string pinCode = Microsoft.VisualBasic.Interaction.InputBox("PIN codeを入力して下さい。", "MyTwitter");
			try {
				tokens = OAuth.GetTokens(session, pinCode);
			}catch(TwitterException) {
				MessageBox.Show("PIN codeが正しくありません。", "MyTwitter");
				return;
			}
			RedrawTimeline();
		}
		private void ExitMenu_Click(object sender, RoutedEventArgs e) {
			Close();
		}
		private void TweetMenu_Click(object sender, RoutedEventArgs e) {
		}
		private void AboutMenu_Click(object sender, RoutedEventArgs e) {
		}
		// 画面操作
		void RedrawTimeline() {
			if(tokens == null)
				return;
			// タイムラインのツイートを取得
			var homeStatus = tokens.Statuses.HomeTimeline();
			foreach(var tweet in homeStatus) {
				
			}
		}
	}
}
