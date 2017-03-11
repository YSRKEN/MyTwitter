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
using System.ComponentModel;

namespace MyTwitter {
	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow : Window {
		Tokens tokens = null;
		List<string> twitterList = null;
		// コンストラクタ
		public MainWindow() {
			InitializeComponent();
			this.DataContext = new MainWindowDC();
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
			// ログインした人のリストを取得する
			LoadTwitterListList();
		}
		private void ExitMenu_Click(object sender, RoutedEventArgs e) {
			Close();
		}
		private void TweetMenu_Click(object sender, RoutedEventArgs e) {
		}
		private void SearchTweetMenu_Click(object sender, RoutedEventArgs e) {
			if(SearchTweetTextBox.Text.Length == 0)
				return;
			if(tokens == null)
				return;
			
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
			try {
				var homeStatus = tokens.Statuses.HomeTimeline();
				var homeTweets = new ObservableCollection<Status>();
				foreach(var tweet in homeStatus) {
					homeTweets.Add(tweet);
				}
				var bindData = this.DataContext as MainWindowDC;
				bindData.HomeTweets = homeTweets;
			} catch {
				MessageBox.Show("タイムラインを取得できませんでした。", "MyTwitter");
			}
		}
		// リストを読み込んで画面に反映する
		void RedrawTwitterList(string name) {
			if(tokens == null)
				return;
			try {
				// リストのツイートを取得
				var selectList =
					from p in tokens.Lists.List()
					where p.Name == name
					select p;
				var listStatus = tokens.Lists.Statuses(selectList.First().Id);
				var listTweets = new ObservableCollection<Status>();
				foreach(var tweet in listStatus) {
					listTweets.Add(tweet);
				}
				var bindData = this.DataContext as MainWindowDC;
				bindData.ListTweets = listTweets;
			}catch {
				MessageBox.Show("リストのツイートを取得できませんでした。", "MyTwitter");
			}
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
		// Twitterのリストの一覧を取得する
		void LoadTwitterListList() {
			if(tokens == null)
				return;
			try {
				twitterList = tokens.Lists.List().Select(e => e.Name).ToList();
				TweetListComboBox.Items.Clear();
				foreach(var name in twitterList) {
					TweetListComboBox.Items.Add(name);
				}
			}catch {
				MessageBox.Show("ログインユーザーのリストを取得できませんでした。", "MyTwitter");
			}
		}
		// 表示するリストを切り替える
		private void TweetListComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			if(TweetListComboBox.SelectedIndex != -1) {
				RedrawTwitterList(twitterList[TweetListComboBox.SelectedIndex]);
			}
		}
	}
	public class MainWindowDC : INotifyPropertyChanged {
		// タイムライン
		ObservableCollection<Status> homeTweets;
		public ObservableCollection<Status> HomeTweets {
			get { return homeTweets; }
			set { homeTweets = value; NotifyPropertyChanged("HomeTweets"); }
		}
		// リスト
		ObservableCollection<Status> listTweets;
		public ObservableCollection<Status> ListTweets {
			get { return listTweets; }
			set { listTweets = value; NotifyPropertyChanged("ListTweets"); }
		}
		public event PropertyChangedEventHandler PropertyChanged = (s, e) => { };
		public void NotifyPropertyChanged(string parameter) {
			PropertyChanged(this, new PropertyChangedEventArgs(parameter));
		}
	}
}
