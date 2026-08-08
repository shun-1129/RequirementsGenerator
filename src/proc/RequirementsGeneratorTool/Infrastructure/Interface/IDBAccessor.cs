using RequirementsDBEntity.Entities.Masters;
using RequirementsDBEntity.Entities.Transactions;

namespace RequirementsGeneratorTool.Infrastructure.Interface
{
    public interface IDBAccessor : IDisposable
    {
        #region DB操作関連
        /// <summary>
        /// DBへ接続する
        /// </summary>
        /// <param name="cancellationToken">キャンセルトークン</param>
        /// <returns>
        /// 接続成功：<see langword="true"/><br/>
        /// 接続失敗：<see langword="false"/>
        /// </returns>
        Task<bool> OpenAsync ( CancellationToken cancellationToken = default );

        /// <summary>
        /// DBへの接続を初期化する（DBが存在しない場合は作成する）
        /// </summary>
        /// <param name="cancellationToken">キャンセルトークン</param>
        /// <returns>
        /// 初期化成功：<see langword="true"/><br/>
        /// 初期化失敗：<see langword="false"/>
        /// </returns>
        Task<bool> InitializeOpenAsync ( CancellationToken cancellationToken = default );

        /// <summary>
        /// DBから切断する
        /// </summary>
        /// <param name="cancellationToken">キャンセルトークン</param>
        /// <returns>
        /// 切断成功：<see langword="true"/><br/>
        /// 切断失敗：<see langword="false"/>
        /// </returns>
        Task<bool> CloseAsync ( CancellationToken cancellationToken = default );

        /// <summary>
        /// 変更を保存する
        /// </summary>
        /// <param name="cancellationToken">キャンセルトークン</param>
        /// <returns>
        /// 保存成功：変更されたエントリの数<br/>
        /// 保存失敗：0
        /// </returns>
        Task<int> SaveChangesAsync ( CancellationToken cancellationToken = default );

        /// <summary>
        /// 変更を保存する（期待される変更数を指定）
        /// </summary>
        /// <param name="expectedChanges">期待される変更数</param>
        /// <param name="cancellationToken">キャンセルトークン</param>
        /// <returns>
        /// 保存成功：<see langword="true"/><br/>
        /// 保存失敗：<see langword="false"/>
        /// </returns>
        Task<bool> SaveChangesAsync ( int expectedChanges, CancellationToken cancellationToken = default );
        #endregion

        #region 取得
        /// <summary>
        /// 要求事項定義マスタの一覧を取得する
        /// </summary>
        /// <param name="cancellationToken">キャンセルトークン</param>
        /// <returns>要求事項定義マスタの一覧</returns>
        Task<List<RequirementsDefinition>> GetRequirementsDefinitionsAsync ( CancellationToken cancellationToken = default );

        /// <summary>
        /// 要求事項IDテーブルの最後のIDを取得する
        /// </summary>
        /// <param name="cancellationToken">キャンセルトークン</param>
        /// <returns>要求事項ID</returns>
        Task<long> GetLastRequirementIdAsync ( CancellationToken cancellationToken = default );
        #endregion

        #region 挿入
        /// <summary>
        /// 要求事項IDテーブルに新しいレコードを挿入する
        /// </summary>
        /// <param name="requirementId">挿入する要求事項IDの情報</param>
        /// <param name="cancellationToken">キャンセルトークン</param>
        /// <returns>
        /// 挿入成功：<see langword="true"/><br/>
        /// 挿入失敗：<see langword="false"/>
        /// </returns>
        Task<bool> InsertRequirementIdAsync ( RequirementId requirementId, CancellationToken cancellationToken = default );
        #endregion
    }
}
