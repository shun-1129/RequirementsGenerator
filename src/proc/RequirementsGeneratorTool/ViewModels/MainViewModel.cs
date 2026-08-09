using RequirementsGeneratorTool.Models;
using System.Collections.ObjectModel;

namespace RequirementsGeneratorTool.ViewModels
{
    /// <summary>
    /// メイン画面のViewModelクラス
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// ビジネスロジックを担うモデル
        /// </summary>
        private readonly MainModel _mainModel;

        private string _projectFolderPath = string.Empty;
        /// <summary>
        /// プロジェクトフォルダパス
        /// </summary>
        public string ProjectFolderPath
        {
            get => _projectFolderPath;
            set
            {
                _projectFolderPath = value;
                RaisePropertyChanged ( nameof ( ProjectFolderPath ) );
            }
        }

        private string _requirementNameEn = string.Empty;
        /// <summary>
        /// 要求名(英語)
        /// </summary>
        public string RequirementNameEn
        {
            get => _requirementNameEn;
            set
            {
                _requirementNameEn = value;
                RaisePropertyChanged ( nameof ( RequirementNameEn ) );
            }
        }

        private string _requirementNameJa = string.Empty;
        /// <summary>
        /// 要求名(日本語)
        /// </summary>
        public string RequirementNameJa
        {
            get => _requirementNameJa;
            set
            {
                _requirementNameJa = value;
                RaisePropertyChanged ( nameof ( RequirementNameJa ) );
            }
        }

        /// <summary>
        /// データベースが初期化済みかどうかを示すフラグ
        /// </summary>
        public bool IsDbInitialized { get; set; }

        /// <summary>
        /// 入力情報の一覧
        /// </summary>
        public ObservableCollection<InputInfoItem> InputInfoItems { get; private set; }
        /// <summary>
        /// 処理情報の一覧
        /// </summary>
        public ObservableCollection<DataGridInfoItem> ProcessInfoItems { get; private set; }
        /// <summary>
        /// 正常系情報の一覧
        /// </summary>
        public ObservableCollection<DataGridInfoItem> NormalInfoItems { get; private set; }
        /// <summary>
        /// 異常系情報の一覧
        /// </summary>
        public ObservableCollection<DataGridInfoItem> AbNormalInfoItems { get; private set; }
        /// <summary>
        /// 制約情報の一覧
        /// </summary>
        public ObservableCollection<DataGridInfoItem> ConstraintsInfoItems { get; private set; }
        /// <summary>
        /// 受入条件情報の一覧
        /// </summary>
        public ObservableCollection<DataGridInfoItem> AcceptanceInfoItems { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainViewModel ()
        {
            _mainModel = new MainModel ();

            InputInfoItems = new ObservableCollection<InputInfoItem> ();
            ProcessInfoItems = new ObservableCollection<DataGridInfoItem> ();
            NormalInfoItems = new ObservableCollection<DataGridInfoItem> ();
            AbNormalInfoItems = new ObservableCollection<DataGridInfoItem> ();
            ConstraintsInfoItems = new ObservableCollection<DataGridInfoItem> ();
            AcceptanceInfoItems = new ObservableCollection<DataGridInfoItem> ();
        }

        /// <summary>
        /// データベースフォルダパスを組み立てる
        /// </summary>
        /// <returns>データベースフォルダパス</returns>
        public string BuildDatabaseFolderPath ()
        {
            return _mainModel.BuildDatabaseFolderPath ( ProjectFolderPath );
        }

        /// <summary>
        /// 出力フォルダパスを組み立てる
        /// </summary>
        /// <returns>出力フォルダパス</returns>
        public string BuildOutputFolderPath ()
        {
            return _mainModel.BuildOutputFolderPath ( ProjectFolderPath );
        }

        /// <summary>
        /// データベースを初期化してオープンする
        /// </summary>
        /// <param name="databaseFolderPath">データベースフォルダパス</param>
        /// <returns>初期化成功：<see langword="true"/>、初期化失敗：<see langword="false"/></returns>
        public async Task<bool> InitializeDatabaseAsync ( string databaseFolderPath )
        {
            bool isInitialized = await _mainModel.InitializeDatabaseAsync ( databaseFolderPath );
            IsDbInitialized = isInitialized;
            return isInitialized;
        }

        /// <summary>
        /// リッチテキストの生テキストをMarkdown向けのテキストへ変換する
        /// </summary>
        /// <param name="rawText">リッチテキストから取得した生テキスト</param>
        /// <returns>Markdown向けに変換されたテキスト</returns>
        public string ConvertToMarkdownText ( string rawText )
        {
            return _mainModel.ConvertToMarkdownText ( rawText );
        }

        /// <summary>
        /// 要求仕様書を出力し、要求事項IDをデータベースへ登録する
        /// </summary>
        /// <param name="functionOverviewText">機能概要のMarkdownテキスト</param>
        /// <returns>出力結果</returns>
        public async Task<OutputRequirementResult> OutputRequirementAsync ( string functionOverviewText )
        {
            return await _mainModel.OutputRequirementAsync (
                BuildDatabaseFolderPath () ,
                BuildOutputFolderPath () ,
                RequirementNameEn ,
                RequirementNameJa ,
                functionOverviewText ,
                InputInfoItems ,
                ProcessInfoItems ,
                NormalInfoItems ,
                AbNormalInfoItems ,
                ConstraintsInfoItems ,
                AcceptanceInfoItems );
        }

        /// <summary>
        /// 入力内容をクリアする
        /// </summary>
        public void ClearInputs ()
        {
            RequirementNameEn = string.Empty;
            RequirementNameJa = string.Empty;
            InputInfoItems.Clear ();
            ProcessInfoItems.Clear ();
            NormalInfoItems.Clear ();
            AbNormalInfoItems.Clear ();
            ConstraintsInfoItems.Clear ();
            AcceptanceInfoItems.Clear ();
        }

        /// <summary>
        /// サンプルデータを読み込む
        /// </summary>
        public void LoadSampleData ()
        {
            ClearInputs ();

            RequirementNameEn = "login";
            RequirementNameJa = "ログイン";

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

            RaisePropertyChanged ( nameof ( InputInfoItems ) );
            RaisePropertyChanged ( nameof ( ProcessInfoItems ) );
            RaisePropertyChanged ( nameof ( NormalInfoItems ) );
            RaisePropertyChanged ( nameof ( AbNormalInfoItems ) );
            RaisePropertyChanged ( nameof ( ConstraintsInfoItems ) );
            RaisePropertyChanged ( nameof ( AcceptanceInfoItems ) );
        }

        /// <summary>
        /// 入力情報グリッドへ新規行を追加する際の初期値を生成する
        /// </summary>
        /// <returns>新規行の初期値</returns>
        public InputInfoItem CreateNewInputInfoItem ()
        {
            int nextNo = InputInfoItems.Any () ? InputInfoItems.Max ( x => x.No ) + 1 : 1;
            return new InputInfoItem
            {
                No = nextNo,
                IsRequired = false,
                ItemName = string.Empty,
                Description = string.Empty
            };
        }

        /// <summary>
        /// データグリッドへ新規行を追加する際の初期値を生成する
        /// </summary>
        /// <param name="items">対象の項目一覧</param>
        /// <returns>新規行の初期値</returns>
        public DataGridInfoItem CreateNewDataGridInfoItem ( ObservableCollection<DataGridInfoItem> items )
        {
            int nextNo = items.Any () ? items.Max ( x => x.No ) + 1 : 1;
            return new DataGridInfoItem
            {
                No = nextNo,
                Description = string.Empty
            };
        }
    }
}
