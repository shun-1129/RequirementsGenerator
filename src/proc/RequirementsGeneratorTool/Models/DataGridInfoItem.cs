namespace RequirementsGeneratorTool.Models
{
    // クラス名は仮
    public class DataGridInfoItem
    {
        /// <summary>
        /// No（処理項目の番号）
        /// </summary>
        public int No { get; set; }
        /// <summary>
        /// 説明（処理項目の詳細な説明）
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }
}
