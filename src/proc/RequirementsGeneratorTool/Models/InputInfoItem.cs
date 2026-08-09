namespace RequirementsGeneratorTool.Models
{
    /// <summary>
    /// 入力情報の項目を表すクラス
    /// </summary>
    public class InputInfoItem
    {
        /// <summary>
        /// No（入力項目の番号）
        /// </summary>
        public int No { get; set; }
        /// <summary>
        /// 必須項目かどうかを示すフラグ（trueの場合は必須、falseの場合は任意）
        /// </summary>
        public bool IsRequired { get; set; }
        /// <summary>
        /// 項目名（入力項目の名前）
        /// </summary>
        public string ItemName { get; set; } = string.Empty;
        /// <summary>
        /// 説明（入力項目の詳細な説明）
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }
}
