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
using System.Collections.ObjectModel;

namespace MyTwitter {
	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow : Window {
		Tokens tokens = null;
		ObservableCollection<Status> homeTweets;
		// コンストラクタ
		public MainWindow() {
			InitializeComponent();
			homeTweets = new ObservableCollection<Status>();
		}

		// メニュー操作
		private void LoginMenu_Click(object sender, RoutedEventArgs e) {
			// セッションを確立させるため、PIN codeの入力を求める
			var session = OAuth.Authorize(Consumer.Key, Consumer.Secret);
			System.Diagnostics.Process.Start(session.AuthorizeUri.AbsoluteUri);
			string pinCode = Microsoft.VisualBasic.Interaction.InputBox("PIN codeを入力して下さい。", "MyTwitter");
			// 正しく入力された際は、トークンを取得するようにする
			try {
				tokens = OAuth.GetTokens(session, pinCode);
			}catch(TwitterException) {
				MessageBox.Show("PIN codeが正しくありません。", "MyTwitter");
				return;
			}
			// タイムラインを読み込む
			RedrawTimeline();
		}
		private void ExitMenu_Click(object sender, RoutedEventArgs e) {
			Close();
		}
		private void TweetMenu_Click(object sender, RoutedEventArgs e) {
		}
		private void AboutMenu_Click(object sender, RoutedEventArgs e) {
		}
		private void CopyTweetUrlMenu_Click(object sender, RoutedEventArgs e) {
			var tabItem = (ListBoxItem)GetOwnerControlFromMenuItem((MenuItem)sender, typeof(ListBoxItem));
			var clickedItem = (Status)tabItem.Content;
			var url = $"https://twitter.com/{clickedItem.User.ScreenName}/status/{clickedItem.Id}";
			Clipboard.SetText(url);
		}

		// 画面操作
		// タイムラインを読み込んで画面に反映する
		void RedrawTimeline() {
			if(tokens == null)
				return;
			// タイムラインのツイートを取得
			var homeStatus = tokens.Statuses.HomeTimeline();
			homeTweets.Clear();
			foreach(var tweet in homeStatus) {
				homeTweets.Add(tweet);
			}
			this.DataContext = homeTweets;
		}

		// 右クリックしたオブジェクトが依存するオブジェクトを検索する
		// http://pieceofnostalgy.blogspot.jp/2012/03/wpf-contextmenuitem.html
		DependencyObject GetOwnerControlFromMenuItem(MenuItem menuItem, Type controlType) {
			DependencyObject obj = menuItem;
			while(!(obj is ContextMenu))
				obj = LogicalTreeHelper.GetParent(obj);
			var elem =　(FrameworkElement)((ContextMenu)obj).PlacementTarget;
			obj = elem.TemplatedParent;
			while(!(obj.GetType() == controlType))
				obj = VisualTreeHelper.GetParent(obj);
			return obj;
		}
	}
}
