using Microsoft.Win32;
using RequirementsGeneratorTool.Models;
using RequirementsGeneratorTool.ViewModels;
using System.ComponentModel;
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
        /// <summary>
        /// メイン画面のViewModel
        /// </summary>
        private readonly MainViewModel _viewModel;

        public MainView ()
        {
            InitializeComponent ();

            _viewModel = new MainViewModel ();
            DataContext = _viewModel;

            InputDataGrid.ItemsSource = _viewModel.InputInfoItems;
            ProcessDataGrid.ItemsSource = _viewModel.ProcessInfoItems;
            NormalDataGrid.ItemsSource = _viewModel.NormalInfoItems;
            AbNormalDataGrid.ItemsSource = _viewModel.AbNormalInfoItems;
            ConstraintsDataGrid.ItemsSource = _viewModel.ConstraintsInfoItems;
            AcceptanceDataGrid.ItemsSource = _viewModel.AcceptanceInfoItems;
        }

        private void Window_Loaded ( object sender , RoutedEventArgs e )
        {
            _viewModel.ProjectFolderPath = Properties.Settings.Default.SelectedProjectPath;
            _viewModel.IsDbInitialized = Properties.Settings.Default.IsDbInitialized;
        }

        private void Window_Closing ( object sender , CancelEventArgs e )
        {
            Properties.Settings.Default.SelectedProjectPath = _viewModel.ProjectFolderPath;
            Properties.Settings.Default.IsDbInitialized = _viewModel.IsDbInitialized;
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

            _viewModel.ProjectFolderPath = dialog.FolderName;

            string databaseFolderPath = _viewModel.BuildDatabaseFolderPath ();

            bool isDbInitialized = await _viewModel.InitializeDatabaseAsync ( databaseFolderPath );
            if ( !isDbInitialized )
            {
                MessageBox.Show ( this , "データベースのオープンに失敗しました。" , "データベースエラー" , MessageBoxButton.OK , MessageBoxImage.Error );
                return;
            }
        }

        private void InputClearBtn_Click ( object sender , RoutedEventArgs e )
        {
            FunctionOverviewRichTextBox.Document.Blocks.Clear ();
            _viewModel.ClearInputs ();
        }

        private async void OutputBtn_Click ( object sender , RoutedEventArgs e )
        {
            if ( !_viewModel.IsDbInitialized )
            {
                MessageBox.Show (
                    this ,
                    "データベースが初期化されていません。フォルダ選択ボタンをクリックして、プロジェクトフォルダを選択してください。" ,
                    "データベース未初期化" ,
                    MessageBoxButton.OK ,
                    MessageBoxImage.Warning );
                return;
            }

            // RichTextBox全体を指すTextRangeを作成
            TextRange textRange = new TextRange(
                FunctionOverviewRichTextBox.Document.ContentStart,
                FunctionOverviewRichTextBox.Document.ContentEnd
            );

            // プレーンテキストとして文字列を取得
            string tempText = textRange.Text;
            string functionOverviewText = _viewModel.ConvertToMarkdownText ( tempText );

            OutputRequirementResult result = await _viewModel.OutputRequirementAsync ( functionOverviewText );
            if ( !result.IsSuccess )
            {
                MessageBox.Show (
                    this ,
                    result.ErrorMessage ,
                    "データベースエラー" ,
                    MessageBoxButton.OK ,
                    MessageBoxImage.Error );
                return;
            }

            MessageBox.Show (
                this ,
                $"要求仕様書を出力しました。\n\n{result.OutputFilePath}" ,
                "出力完了" ,
                MessageBoxButton.OK ,
                MessageBoxImage.Information );
        }

        private void InputDataGrid_AddingNewItem ( object sender , AddingNewItemEventArgs e )
        {
            e.NewItem = _viewModel.CreateNewInputInfoItem ();
        }

        private void ProcessDataGrid_AddingNewItem ( object sender , AddingNewItemEventArgs e )
        {
            e.NewItem = _viewModel.CreateNewDataGridInfoItem ( _viewModel.ProcessInfoItems );
        }

        private void NomalDataGrid_AddingNewItem ( object sender , AddingNewItemEventArgs e )
        {
            e.NewItem = _viewModel.CreateNewDataGridInfoItem ( _viewModel.NormalInfoItems );
        }

        private void AbNomalDataGrid_AddingNewItem ( object sender , AddingNewItemEventArgs e )
        {
            e.NewItem = _viewModel.CreateNewDataGridInfoItem ( _viewModel.AbNormalInfoItems );
        }

        private void ConstraintsDataGrid_AddingNewItem ( object sender , AddingNewItemEventArgs e )
        {
            e.NewItem = _viewModel.CreateNewDataGridInfoItem ( _viewModel.ConstraintsInfoItems );
        }

        private void AcceptanceDataGrid_AddingNewItem ( object sender , AddingNewItemEventArgs e )
        {
            e.NewItem = _viewModel.CreateNewDataGridInfoItem ( _viewModel.AcceptanceInfoItems );
        }

        private void SampleBtn_Click ( object sender , RoutedEventArgs e )
        {
            FunctionOverviewRichTextBox.Document.Blocks.Clear ();

            _viewModel.LoadSampleData ();

            FunctionOverviewRichTextBox.AppendText ( "テスト用" );
            FunctionOverviewRichTextBox.AppendText ( Environment.NewLine );
            FunctionOverviewRichTextBox.AppendText ( "サンプル" );
            FunctionOverviewRichTextBox.AppendText ( Environment.NewLine );
            FunctionOverviewRichTextBox.AppendText ( "ユーザーを認証し、システムへのアクセスを許可する機能" );

            InputDataGrid.ItemsSource = null;
            ProcessDataGrid.ItemsSource = null;
            NormalDataGrid.ItemsSource = null;
            AbNormalDataGrid.ItemsSource = null;
            ConstraintsDataGrid.ItemsSource = null;
            AcceptanceDataGrid.ItemsSource = null;

            InputDataGrid.ItemsSource = _viewModel.InputInfoItems;
            ProcessDataGrid.ItemsSource = _viewModel.ProcessInfoItems;
            NormalDataGrid.ItemsSource = _viewModel.NormalInfoItems;
            AbNormalDataGrid.ItemsSource = _viewModel.AbNormalInfoItems;
            ConstraintsDataGrid.ItemsSource = _viewModel.ConstraintsInfoItems;
            AcceptanceDataGrid.ItemsSource = _viewModel.AcceptanceInfoItems;
        }
    }
}
