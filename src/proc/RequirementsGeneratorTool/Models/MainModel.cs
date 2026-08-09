using RequirementsDBEntity.Entities.Transactions;
using RequirementsGeneratorTool.Infrastructure.Impl;
using RequirementsGeneratorTool.Infrastructure.Interface;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace RequirementsGeneratorTool.Models
{
    /// <summary>
    /// メイン画面のビジネスロジックを担うモデルクラス
    /// </summary>
    public class MainModel
    {
        /// <summary>
        /// プロジェクトフォルダパスからデータベースフォルダパスを組み立てる
        /// </summary>
        /// <param name="projectFolderPath">プロジェクトフォルダパス</param>
        /// <returns>データベースフォルダパス</returns>
        public string BuildDatabaseFolderPath ( string projectFolderPath )
        {
            return Path.Combine ( projectFolderPath , DATABASE_FOLDER_PATH );
        }

        /// <summary>
        /// プロジェクトフォルダパスから出力フォルダパスを組み立てる
        /// </summary>
        /// <param name="projectFolderPath">プロジェクトフォルダパス</param>
        /// <returns>出力フォルダパス</returns>
        public string BuildOutputFolderPath ( string projectFolderPath )
        {
            return Path.Combine ( projectFolderPath , OUTPUT_FOLDER_PATH );
        }

        /// <summary>
        /// データベースを初期化してオープンする（フォルダが存在しない場合は作成する）
        /// </summary>
        /// <param name="databaseFolderPath">データベースフォルダパス</param>
        /// <returns>初期化成功：<see langword="true"/>、初期化失敗：<see langword="false"/></returns>
        public async Task<bool> InitializeDatabaseAsync ( string databaseFolderPath )
        {
            if ( !Directory.Exists ( databaseFolderPath ) )
            {
                Directory.CreateDirectory ( databaseFolderPath );
                File.SetAttributes ( databaseFolderPath , FileAttributes.Hidden );
            }

            using IDBAccessor dba = DBAccessor.CreateInstance ( databaseFolderPath );
            return await dba.InitializeOpenAsync ();
        }

        /// <summary>
        /// 要求名(英語)が半角英数字のみで構成されているかを検証する
        /// </summary>
        /// <param name="requirementNameEn">要求名(英語)</param>
        /// <returns>妥当な場合：<see langword="true"/>、不正な場合：<see langword="false"/></returns>
        public bool ValidateRequirementNameEn ( string requirementNameEn )
        {
            return Regex.IsMatch ( requirementNameEn , ALPHANUMERIC_CHARACTERS_ONLY_PATTERN );
        }

        /// <summary>
        /// リッチテキストの生テキストをMarkdown向けのテキストへ変換する（改行を&lt;br&gt;タグへ置換する）
        /// </summary>
        /// <param name="rawText">リッチテキストから取得した生テキスト</param>
        /// <returns>Markdown向けに変換されたテキスト</returns>
        public string ConvertToMarkdownText ( string rawText )
        {
            string cleanText = rawText.TrimEnd ( '\r' , '\n' );
            return cleanText.Replace ( "\r" , "<br>" );
        }

        /// <summary>
        /// 要求仕様書のファイル名を組み立てる
        /// </summary>
        /// <param name="requirementId">要求事項ID</param>
        /// <param name="requirementNameEn">要求名(英語)</param>
        /// <returns>要求仕様書のファイル名</returns>
        public string BuildRequirementFileName ( long requirementId , string requirementNameEn )
        {
            return string.Format ( BASE_REQUIREMENT_FILE , requirementId , requirementNameEn );
        }

        /// <summary>
        /// 入力情報のMarkdownテーブルを生成する
        /// </summary>
        /// <param name="items">入力情報の一覧</param>
        /// <returns>Markdown形式のテーブル文字列</returns>
        public string CreateInputInfoMarkdownTable ( ObservableCollection<InputInfoItem> items )
        {
            StringBuilder sb = new StringBuilder ();
            sb.AppendLine ( "| No | 必須 | 項目名 | 説明 |" );
            sb.AppendLine ( "|---|---|---|---|" );

            foreach ( InputInfoItem item in items )
            {
                string requiredText = item.IsRequired ? "必須" : "任意";
                string content = $"{item.No} | {requiredText} | {item.ItemName} | {item.Description}";

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

        /// <summary>
        /// 処理・正常系・異常系・制約・受入条件のMarkdownリストを生成する
        /// </summary>
        /// <param name="items">対象の項目一覧</param>
        /// <param name="isNumber">番号付きリストとする場合：<see langword="true"/>、箇条書きとする場合：<see langword="false"/></param>
        /// <returns>Markdown形式のリスト文字列</returns>
        public string CreateMarkDownInfoTable ( ObservableCollection<DataGridInfoItem> items , bool isNumber = false )
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

        /// <summary>
        /// 要求仕様書のMarkdownコンテンツを生成する
        /// </summary>
        public string BuildMarkdownContent (
            long requirementId ,
            string requirementNameJa ,
            string functionOverviewText ,
            ObservableCollection<InputInfoItem> inputInfoItems ,
            ObservableCollection<DataGridInfoItem> processInfoItems ,
            ObservableCollection<DataGridInfoItem> normalInfoItems ,
            ObservableCollection<DataGridInfoItem> abNormalInfoItems ,
            ObservableCollection<DataGridInfoItem> constraintsInfoItems ,
            ObservableCollection<DataGridInfoItem> acceptanceInfoItems )
        {
            StringBuilder stringBuilder = new StringBuilder ();
            stringBuilder.AppendLine (
                string.Format (
                    MARKDOWN_HEADER ,
                    string.Format ( MARKDOWN_REQUIREMENT_ID , requirementId ) ,
                    requirementNameJa ,
                    functionOverviewText ,
                    CreateInputInfoMarkdownTable ( inputInfoItems ) ,
                    CreateMarkDownInfoTable ( processInfoItems , true ) ,
                    CreateMarkDownInfoTable ( normalInfoItems ) ,
                    CreateMarkDownInfoTable ( abNormalInfoItems ) ,
                    CreateMarkDownInfoTable ( constraintsInfoItems ) ,
                    CreateMarkDownInfoTable ( acceptanceInfoItems ) ) );

            return stringBuilder.ToString ();
        }

        /// <summary>
        /// Markdownコンテンツをファイルへ出力する（Shift-JISで出力する）
        /// </summary>
        /// <param name="filePath">出力先のファイルパス</param>
        /// <param name="content">出力するMarkdownコンテンツ</param>
        public void SaveMarkdownFile ( string filePath , string content )
        {
            Encoding.RegisterProvider ( CodePagesEncodingProvider.Instance );
            Encoding sjis = Encoding.GetEncoding ( "shift_jis" );

            using FileStream fs = new FileStream ( filePath , FileMode.Create , FileAccess.Write );
            fs.Write ( sjis.GetBytes ( content ) );
        }

        /// <summary>
        /// 要求仕様書を出力し、要求事項IDをデータベースへ登録する
        /// </summary>
        /// <param name="databaseFolderPath">データベースフォルダパス</param>
        /// <param name="outputFolderPath">出力フォルダパス</param>
        /// <param name="requirementNameEn">要求名(英語)</param>
        /// <param name="requirementNameJa">要求名(日本語)</param>
        /// <param name="functionOverviewText">機能概要のテキスト</param>
        /// <param name="inputInfoItems">入力情報の一覧</param>
        /// <param name="processInfoItems">処理情報の一覧</param>
        /// <param name="normalInfoItems">正常系情報の一覧</param>
        /// <param name="abNormalInfoItems">異常系情報の一覧</param>
        /// <param name="constraintsInfoItems">制約情報の一覧</param>
        /// <param name="acceptanceInfoItems">受入条件情報の一覧</param>
        /// <returns>出力結果</returns>
        public async Task<OutputRequirementResult> OutputRequirementAsync (
            string databaseFolderPath ,
            string outputFolderPath ,
            string requirementNameEn ,
            string requirementNameJa ,
            string functionOverviewText ,
            ObservableCollection<InputInfoItem> inputInfoItems ,
            ObservableCollection<DataGridInfoItem> processInfoItems ,
            ObservableCollection<DataGridInfoItem> normalInfoItems ,
            ObservableCollection<DataGridInfoItem> abNormalInfoItems ,
            ObservableCollection<DataGridInfoItem> constraintsInfoItems ,
            ObservableCollection<DataGridInfoItem> acceptanceInfoItems )
        {
            using IDBAccessor dba = DBAccessor.CreateInstance ( databaseFolderPath );
            bool isDbOpened = await dba.OpenAsync ();
            if ( !isDbOpened )
            {
                return OutputRequirementResult.Failure ( "データベースのオープンに失敗しました。" );
            }

            long requirementId = await dba.GetLastRequirementIdAsync ();
            long newRequirementId = requirementId + 1;

            if ( !Directory.Exists ( outputFolderPath ) )
            {
                Directory.CreateDirectory ( outputFolderPath );
            }

            if ( !ValidateRequirementNameEn ( requirementNameEn ) )
            {
                return OutputRequirementResult.Failure ( "要求仕様書名は半角英数字のみで入力してください。" );
            }

            string fileName = BuildRequirementFileName ( newRequirementId , requirementNameEn );
            string filePath = Path.Combine ( outputFolderPath , fileName );

            string markdownContent = BuildMarkdownContent (
                newRequirementId ,
                requirementNameJa ,
                functionOverviewText ,
                inputInfoItems ,
                processInfoItems ,
                normalInfoItems ,
                abNormalInfoItems ,
                constraintsInfoItems ,
                acceptanceInfoItems );

            SaveMarkdownFile ( filePath , markdownContent );

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
                return OutputRequirementResult.Failure ( "要求仕様書IDの登録に失敗しました。" );
            }

            bool isSaved = await dba.SaveChangesAsync ( 1 );
            if ( !isSaved )
            {
                return OutputRequirementResult.Failure ( "要求仕様書IDの保存に失敗しました。" );
            }

            return OutputRequirementResult.Success ( filePath );
        }
    }

    /// <summary>
    /// 要求仕様書出力処理の結果を表すクラス
    /// </summary>
    public class OutputRequirementResult
    {
        /// <summary>
        /// 処理が成功したかどうかを示すフラグ
        /// </summary>
        public bool IsSuccess { get; private init; }

        /// <summary>
        /// エラーメッセージ（失敗時のみ設定される）
        /// </summary>
        public string ErrorMessage { get; private init; } = string.Empty;

        /// <summary>
        /// 出力されたファイルパス（成功時のみ設定される）
        /// </summary>
        public string OutputFilePath { get; private init; } = string.Empty;

        /// <summary>
        /// 成功結果を生成する
        /// </summary>
        /// <param name="outputFilePath">出力されたファイルパス</param>
        /// <returns>成功を表す結果</returns>
        public static OutputRequirementResult Success ( string outputFilePath )
        {
            return new OutputRequirementResult { IsSuccess = true , OutputFilePath = outputFilePath };
        }

        /// <summary>
        /// 失敗結果を生成する
        /// </summary>
        /// <param name="errorMessage">エラーメッセージ</param>
        /// <returns>失敗を表す結果</returns>
        public static OutputRequirementResult Failure ( string errorMessage )
        {
            return new OutputRequirementResult { IsSuccess = false , ErrorMessage = errorMessage };
        }
    }
}
