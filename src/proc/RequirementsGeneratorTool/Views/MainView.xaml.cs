using Microsoft.Win32;
using RequirementsDBEntity.Entities.Transactions;
using RequirementsGeneratorTool.Infrastructure.Impl;
using RequirementsGeneratorTool.Infrastructure.Interface;
using RequirementsGeneratorTool.Models;
using RequirementsGeneratorTool.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace RequirementsGeneratorTool.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        private bool _isDbInitialized;
        private string _outputPath;

        public ObservableCollection<InputInfoItem> InputInfoItems { get; set; }
        public ObservableCollection<DataGridInfoItem> ProcessInfoItems { get; set; }
        public ObservableCollection<DataGridInfoItem> NormalInfoItems { get; set; }
        public ObservableCollection<DataGridInfoItem> AbNormalInfoItems { get; set; }
        public ObservableCollection<DataGridInfoItem> ConstraintsInfoItems { get; set; }
        public ObservableCollection<DataGridInfoItem> AcceptanceInfoItems { get; set; }

        public MainView ()
        {
            _isDbInitialized = false;
            _outputPath = string.Empty;
            InitializeComponent ();

            InputInfoItems = new ObservableCollection<InputInfoItem> ();
            ProcessInfoItems = new ObservableCollection<DataGridInfoItem> ();
            NormalInfoItems = new ObservableCollection<DataGridInfoItem> ();
            AbNormalInfoItems = new ObservableCollection<DataGridInfoItem> ();
            ConstraintsInfoItems = new ObservableCollection<DataGridInfoItem> ();
            AcceptanceInfoItems = new ObservableCollection<DataGridInfoItem> ();

            InputDataGrid.ItemsSource = InputInfoItems;
            ProcessDataGrid.ItemsSource = ProcessInfoItems;
            NormalDataGrid.ItemsSource = NormalInfoItems;
            AbNormalDataGrid.ItemsSource = AbNormalInfoItems;
            ConstraintsDataGrid.ItemsSource = ConstraintsInfoItems;
            AcceptanceDataGrid.ItemsSource = AcceptanceInfoItems;
        }

        private void Window_Loaded ( object sender , RoutedEventArgs e )
        {
            ProjectFolderPathTextBox.Text = Properties.Settings.Default.SelectedProjectPath;
            _isDbInitialized = Properties.Settings.Default.IsDbInitialized;
            _outputPath = Path.Combine ( ProjectFolderPathTextBox.Text , DATABASE_FOLDER_PATH );
        }

        private void Window_Closing ( object sender , CancelEventArgs e )
        {
            Properties.Settings.Default.SelectedProjectPath = ProjectFolderPathTextBox.Text;
            Properties.Settings.Default.IsDbInitialized = _isDbInitialized;
            Properties.Settings.Default.Save ();
        }

        private async void FolderSelectBtn_Click ( object sender , RoutedEventArgs e )
        {
            OpenFolderDialog dialog = new OpenFolderDialog ()
            {
                Title = "プロジェクトフォルダを選択してください"
            };

            bool isResult = dialog.ShowDialog () ?? false;

            if ( !isResult )
            {
                MessageBox.Show ( this , "プロジェクトフォルダ選択をキャンセルしました。" , "フォルダ選択" , MessageBoxButton.OK , MessageBoxImage.Information );
                return;
            }

            ProjectFolderPathTextBox.Text = dialog.FolderName;

            _outputPath = Path.Combine ( ProjectFolderPathTextBox.Text , DATABASE_FOLDER_PATH );

            if ( !Directory.Exists ( _outputPath ) )
            {
                Directory.CreateDirectory ( _outputPath );
                File.SetAttributes ( _outputPath , FileAttributes.Hidden );
            }

            using ( IDBAccessor dba = DBAccessor.CreateInstance ( _outputPath ) )
            {
                bool isDbOpened = await dba.InitializeOpenAsync ();
                if ( !isDbOpened )
                {
                    MessageBox.Show ( this , "データベースのオープンに失敗しました。" , "データベースエラー" , MessageBoxButton.OK , MessageBoxImage.Error );
                    return;
                }
            }
        }

        private void InputClearBtn_Click ( object sender , RoutedEventArgs e )
        {
            RequirementNameEnTextBox.Clear ();
            RequirementNameJaTextBox.Clear ();
            FunctionOverviewRichTextBox.Document.Blocks.Clear ();
            InputInfoItems.Clear ();
            ProcessInfoItems.Clear ();
            NormalInfoItems.Clear ();
            AbNormalInfoItems.Clear ();
            ConstraintsInfoItems.Clear ();
            AcceptanceInfoItems.Clear ();
        }

        private async void OutputBtn_Click ( object sender , RoutedEventArgs e )
        {
            if ( !_isDbInitialized )
            {
                MessageBox.Show (
                    this ,
                    "データベースが初期化されていません。フォルダ選択ボタンをクリックして、プロジェクトフォルダを選択してください。" ,
                    "データベース未初期化" ,
                    MessageBoxButton.OK ,
                    MessageBoxImage.Warning );
                return;
            }

            using IDBAccessor dba = DBAccessor.CreateInstance ( _outputPath );
            bool isDbOpened = await dba.OpenAsync ();
            if ( !isDbOpened )
            {
                MessageBox.Show ( this , "データベースのオープンに失敗しました。" , "データベースエラー" , MessageBoxButton.OK , MessageBoxImage.Error );
                return;
            }

            long requirementId = await dba.GetLastRequirementIdAsync ();
            long newRequirementId = requirementId + 1;

            string outputFolderPath = Path.Combine ( ProjectFolderPathTextBox.Text , OUTPUT_FOLDER_PATH );
            if ( !Directory.Exists ( outputFolderPath ) )
            {
                Directory.CreateDirectory ( outputFolderPath );
            }

            if ( !Regex.IsMatch ( RequirementNameEnTextBox.Text , ALPHANUMERIC_CHARACTERS_ONLY_PATTERN ) )
            {
                MessageBox.Show (
                    this ,
                    "要求仕様書名は半角英数字のみで入力してください。" ,
                    "入力エラー" ,
                    MessageBoxButton.OK ,
                    MessageBoxImage.Warning );

                return;
            }

            string fileName = string.Format ( BASE_REQUIREMENT_FILE , newRequirementId , RequirementNameEnTextBox.Text );

            // RichTextBox全体を指すTextRangeを作成
            TextRange textRange = new TextRange(
                FunctionOverviewRichTextBox.Document.ContentStart,
                FunctionOverviewRichTextBox.Document.ContentEnd
            );

            // プレーンテキストとして文字列を取得
            string tempText = textRange.Text;
            // 末尾の改行を削除
            string cleanText = tempText.TrimEnd ('\r' , '\n');
            // 改行コードを<br>タグに置換
            string functionOverviewText = cleanText.Replace ( "\r" , "<br>");

            Encoding.RegisterProvider ( CodePagesEncodingProvider.Instance );
            Encoding sjis = Encoding.GetEncoding ( "shift_jis" );

            using FileStream fs = new FileStream ( Path.Combine ( outputFolderPath , fileName ) , FileMode.Create , FileAccess.Write );

            StringBuilder stringBuilder = new StringBuilder ();
            stringBuilder.AppendLine (
                string.Format (
                    MARKDOWN_HEADER ,

                    string.Format ( MARKDOWN_REQUIREMENT_ID , newRequirementId ) ,
                    RequirementNameJaTextBox.Text ,
                    functionOverviewText ,
                    CreateInputInfoMarkdownTable () ,
                    CreateMarkDownInfoTable ( ProcessInfoItems , true ) ,
                    CreateMarkDownInfoTable ( NormalInfoItems ) ,
                    CreateMarkDownInfoTable ( AbNormalInfoItems ) ,
                    CreateMarkDownInfoTable ( ConstraintsInfoItems ) ,
                    CreateMarkDownInfoTable ( AcceptanceInfoItems ) ) );

            fs.Write ( sjis.GetBytes ( stringBuilder.ToString () ) );
            fs.Close ();
            fs.Dispose ();

            DateTime dateTime = DateTime.UtcNow;
            RequirementId requirementIdEntity = new RequirementId
            {
                RequirementDefinitionId = 1 ,
                CreatedAt = dateTime ,
                CreateUser = "System" ,
                CreateProgram = "RequirementsGeneratorTool" ,
                UpdatedAt = dateTime ,
                UpdateUser = "System" ,
                UpdateProgram = "RequirementsGeneratorTool"
            };

            bool isInserted = await dba.InsertRequirementIdAsync ( requirementIdEntity );
            if ( !isInserted )
            {
                MessageBox.Show (
                    this ,
                    "要求仕様書IDの登録に失敗しました。" ,
                    "データベースエラー" ,
                    MessageBoxButton.OK ,
                    MessageBoxImage.Error );
                return;
            }

            bool isSaved = await dba.SaveChangesAsync ( 1 );
            if ( !isSaved )
            {
                MessageBox.Show (
                    this ,
                    "要求仕様書IDの保存に失敗しました。" ,
                    "データベースエラー" ,
                    MessageBoxButton.OK ,
                    MessageBoxImage.Error );
                return;
            }

            MessageBox.Show (
                this ,
                $"要求仕様書を出力しました。\n\n{Path.Combine ( outputFolderPath , fileName )}" ,
                "出力完了" ,
                MessageBoxButton.OK ,
                MessageBoxImage.Information );
        }

        private string CreateInputInfoMarkdownTable ()
        {
            StringBuilder sb = new StringBuilder ();
            sb.AppendLine ( "| No | 必須 | 項目名 | 説明 |" );
            sb.AppendLine ( "|---|---|---|---|" );

            foreach ( InputInfoItem item in InputInfoItems )
            {
                string requiredText = item.IsRequired ? "必須" : "任意";
                string content = $"{item.No} | {requiredText} | {item.ItemName} | {item.Description}";

                if ( InputInfoItems.Last ().Equals ( item ) )
                {
                    sb.Append ( content );
                }
                else
                {
                    sb.AppendLine ( content );
                }
            }

            return sb.ToString ();
        }

        private string CreateMarkDownInfoTable ( ObservableCollection<DataGridInfoItem> items , bool isNumber = false )
        {
            StringBuilder sb = new StringBuilder ();
            foreach ( DataGridInfoItem item in items )
            {
                string heder = isNumber ? $"{item.No}. " : "- ";
                string content = $"{heder}{item.Description}";
                if ( items.Last ().Equals ( item ) )
                {
                    sb.Append ( content );
                }
                else
                {
                    sb.AppendLine ( content );
                }
            }
            return sb.ToString ();
        }

        private void InputDataGrid_AddingNewItem ( object sender , AddingNewItemEventArgs e )
        {
            int nextNo = InputInfoItems.Any () ? InputInfoItems.Max ( x => x.No ) + 1 : 1;

            e.NewItem = new InputInfoItem
            {
                No = nextNo,
                IsRequired = false,
                ItemName = string.Empty,
                Description = string.Empty
            };
        }

        private void ProcessDataGrid_AddingNewItem ( object sender , AddingNewItemEventArgs e )
        {
            int nextNo = ProcessInfoItems.Any () ? ProcessInfoItems.Max ( x => x.No ) + 1 : 1;
            e.NewItem = new DataGridInfoItem
            {
                No = nextNo,
                Description = string.Empty
            };
        }

        private void NomalDataGrid_AddingNewItem ( object sender , AddingNewItemEventArgs e )
        {
            int nextNo = NormalInfoItems.Any () ? NormalInfoItems.Max ( x => x.No ) + 1 : 1;
            e.NewItem = new DataGridInfoItem
            {
                No = nextNo,
                Description = string.Empty
            };
        }

        private void AbNomalDataGrid_AddingNewItem ( object sender , AddingNewItemEventArgs e )
        {
            int nextNo = AbNormalInfoItems.Any () ? AbNormalInfoItems.Max ( x => x.No ) + 1 : 1;
            e.NewItem = new DataGridInfoItem
            {
                No = nextNo,
                Description = string.Empty
            };
        }

        private void ConstraintsDataGrid_AddingNewItem ( object sender , AddingNewItemEventArgs e )
        {
            int nextNo = ConstraintsInfoItems.Any () ? ConstraintsInfoItems.Max ( x => x.No ) + 1 : 1;
            e.NewItem = new DataGridInfoItem
            {
                No = nextNo,
                Description = string.Empty
            };
        }

        private void AcceptanceDataGrid_AddingNewItem ( object sender , AddingNewItemEventArgs e )
        {
            int nextNo = AcceptanceInfoItems.Any () ? AcceptanceInfoItems.Max ( x => x.No ) + 1 : 1;
            e.NewItem = new DataGridInfoItem
            {
                No = nextNo,
                Description = string.Empty
            };
        }

        private void SampleBtn_Click ( object sender , RoutedEventArgs e )
        {
            InputClearBtn_Click ( sender , e );

            RequirementNameEnTextBox.Text = "login";
            RequirementNameJaTextBox.Text = "ログイン";

            FunctionOverviewRichTextBox.AppendText ( "テスト用" );
            FunctionOverviewRichTextBox.AppendText ( Environment.NewLine );
            FunctionOverviewRichTextBox.AppendText ( "サンプル" );
            FunctionOverviewRichTextBox.AppendText ( Environment.NewLine );
            FunctionOverviewRichTextBox.AppendText ( "ユーザーを認証し、システムへのアクセスを許可する機能" );

            InputInfoItems = new ObservableCollection<InputInfoItem> ()
            {
                new InputInfoItem ()
                {
                    No = 1,
                    IsRequired = true,
                    ItemName = "ユーザーID",
                    Description = "ログインするユーザーのID"
                } ,
                new InputInfoItem ()
                {
                    No = 2,
                    IsRequired = true,
                    ItemName = "パスワード",
                    Description = "ユーザーのパスワード"
                }
            };
            ProcessInfoItems = new ObservableCollection<DataGridInfoItem> ()
            {
                new DataGridInfoItem () { No = 1, Description = "ユーザーIDの入力値を確認する。" } ,
                new DataGridInfoItem () { No = 2, Description = "パスワードの入力値を確認する。" } ,
                new DataGridInfoItem () { No = 3, Description = "ユーザー情報を検索する。" } ,
                new DataGridInfoItem () { No = 4, Description = "パスワードを検証する。" } ,
                new DataGridInfoItem () { No = 5, Description = "認証結果に応じて画面を遷移する。" }
            };
            NormalInfoItems = new ObservableCollection<DataGridInfoItem> ()
            {
                new DataGridInfoItem () { No = 1, Description = "認証に成功した場合、メイン画面を表示する。" }
            };
            AbNormalInfoItems = new ObservableCollection<DataGridInfoItem> ()
            {
                new DataGridInfoItem () { No = 1, Description = "ユーザーIDが存在しない場合、エラーを表示する。" } ,
                new DataGridInfoItem () { No = 2, Description = "パスワードが一致しない場合、エラーを表示する。" }
            };
            ConstraintsInfoItems = new ObservableCollection<DataGridInfoItem> ()
            {
                new DataGridInfoItem () { No = 1, Description = "パスワードをログへ出力してはならない。" }
            };
            AcceptanceInfoItems = new ObservableCollection<DataGridInfoItem> ()
            {
                new DataGridInfoItem () { No = 1, Description = "正しい認証情報でログインできること。" } ,
                new DataGridInfoItem () { No = 2, Description = "誤った認証情報ではログインできないこと。" }
            };

            InputDataGrid.ItemsSource = null;
            ProcessDataGrid.ItemsSource = null;
            NormalDataGrid.ItemsSource = null;
            AbNormalDataGrid.ItemsSource = null;
            ConstraintsDataGrid.ItemsSource = null;
            AcceptanceDataGrid.ItemsSource = null;

            InputDataGrid.ItemsSource = InputInfoItems;
            ProcessDataGrid.ItemsSource = ProcessInfoItems;
            NormalDataGrid.ItemsSource = NormalInfoItems;
            AbNormalDataGrid.ItemsSource = AbNormalInfoItems;
            ConstraintsDataGrid.ItemsSource = ConstraintsInfoItems;
            AcceptanceDataGrid.ItemsSource = AcceptanceInfoItems;
        }
    }
}