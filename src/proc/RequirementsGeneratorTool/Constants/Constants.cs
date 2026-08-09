namespace RequirementsGeneratorTool.Constants
{
    public static class Constants
    {
        /// <summary>
        /// 標準のタイムアウト時間（秒）
        /// </summary>
        public const int DEFAULT_TIMEOUT_SECONDS = 30;

        /// <summary>
        /// データベースファイル名
        /// </summary>
        public const string DATABASE_FILE = "requirements.db";

        /// <summary>
        /// データベースフォルダのパス（プロジェクトフォルダ内のDocuments\Database）
        /// </summary>
        public const string DATABASE_FOLDER_PATH = @"Documents\Database";

        /// <summary>
        /// 出力フォルダのパス（プロジェクトフォルダ内のDocuments\Requirements）
        /// </summary>
        public const string OUTPUT_FOLDER_PATH = @"Documents\Requirements";

        /// <summary>
        /// 出力ファイルのベース名（REQ-0001-○○.md のような形式）
        /// </summary>
        public const string BASE_REQUIREMENT_FILE = "REQ-{0:D4}-{1}.md";

        /// <summary>
        /// 英数字のみの正規表現パターン
        /// </summary>
        public const string ALPHANUMERIC_CHARACTERS_ONLY_PATTERN = @"^[a-zA-Z0-9]+$";
    }

    public static class MarkDownContents
    {
        /// <summary>
        /// MarkDownの区切り文字（YAML Front Matterの区切りとして使用）
        /// </summary>
        public const string MARKDOWN_DELIMITER = "---";

        /// <summary>
        /// 要求IDのフォーマット（REQ-0001 のような形式）
        /// </summary>
        public const string MARKDOWN_REQUIREMENT_ID = "REQ-{0:D4}";

        /// <summary>
        /// MarkDownのヘッダー部分（YAML Front Matter）の内容
        /// </summary>
        public const string MARKDOWN_HEADER = "# 機能要求\n\n" +
            "## 基本情報\n\n" +
            "- 要求ID: {0}\n" +
            "- 要求名: {1}\n\n" +
            "## 1. 機能概要\n\n" +
            "{2}\n\n" +
            "## 2. 入力\n\n" +
            "{3}\n\n" +
            "## 3. 処理\n\n" +
            "{4}\n\n" +
            "## 4. 正常系\n\n" +
            "{5}\n\n" +
            "## 5. 異常系\n\n" +
            "{6}\n\n" +
            "## 6. 制約\n\n" +
            "{7}\n\n" +
            "## 7. 受入条件\n\n" +
            "{8}";
    }
}
