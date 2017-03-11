using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace MyTwitter {
	/// <summary>
	/// TweetWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class TweetWindow : Window {
		public TweetWindow() {
			InitializeComponent();
			this.DataContext = new TweetWindowDC() {
				TweetText = "",
			};
		}
		// ツイートを送信する
		private void SendTweetButton_Click(object sender, RoutedEventArgs e) {
			
		}

		private void TweetTextBox_TextChanged(object sender, TextChangedEventArgs e) {
			var bindData = this.DataContext as TweetWindowDC;
			var a = TweetTextBox.Text;
			bindData.TweetText = a;
		}
	}
	public class TweetWindowDC : INotifyPropertyChanged {
		// ツイート
		string tweetText;
		public string TweetText {
			get { return tweetText; }
			set {
				tweetText = value;
				TweetLength = "";
			}
		}
		// ツイートの文字数
		public string TweetLength {
			get { return $"{tweetText.Replace("\r\n", "\r").Length}/140文字"; }
			set { NotifyPropertyChanged("TweetLength"); }
		}
		public event PropertyChangedEventHandler PropertyChanged = (s, e) => { };
		public void NotifyPropertyChanged(string parameter) {
			PropertyChanged(this, new PropertyChangedEventArgs(parameter));
		}
	}
}
