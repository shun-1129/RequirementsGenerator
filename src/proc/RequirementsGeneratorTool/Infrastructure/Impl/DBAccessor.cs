using Microsoft.EntityFrameworkCore;
using RequirementsDBEntity;
using RequirementsDBEntity.Entities.Masters;
using RequirementsDBEntity.Entities.Transactions;
using RequirementsGeneratorTool.Infrastructure.Interface;
using System.IO;

namespace RequirementsGeneratorTool.Infrastructure.Impl
{
    public class DBAccessor : IDBAccessor
    {
        #region メンバ変数
        /// <summary>
        /// 接続情報
        /// </summary>
        private readonly string _connectionString;
        /// <summary>
        /// タイムアウト時間（秒）
        /// </summary>
        private readonly int _timeoutSeconds;
        /// <summary>
        /// DBコンテキストのインスタンス
        /// </summary>
        private ApplicationDbContext? _dbContext;
        #endregion

        #region プロパティ
        /// <summary>
        /// インスタンスを生成する
        /// </summary>
        /// <param name="connectionString">接続情報</param>
        /// <param name="timeoutSeconds">タイムアウト時間（秒）</param>
        /// <returns>DBAccessorのインスタンス</returns>
        public static IDBAccessor CreateInstance ( string connectionString , int timeoutSeconds = DEFAULT_TIMEOUT_SECONDS )
        {
            return new DBAccessor ( connectionString , timeoutSeconds );
        }
        #endregion

        #region コンストラクタ／デストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="connectionString">接続情報</param>
        /// <param name="timeoutSeconds">タイムアウト時間（秒）</param>
        private DBAccessor ( string connectionString , int timeoutSeconds )
        {
            _connectionString = connectionString;
            _timeoutSeconds = timeoutSeconds;
        }

        /// <summary>
        /// 破棄処理を行う
        /// </summary>
        public void Dispose ()
        {
            CloseAsync ().GetAwaiter ().GetResult ();
        }
        #endregion

        #region メソッド
        #region DB操作関連
        /// <inheritdoc/>
        public async Task<bool> OpenAsync ( CancellationToken cancellationToken = default )
        {
            if ( !File.Exists ( Path.Combine ( _connectionString , DATABASE_FILE ) ) )
            {
                return false;
            }

            return await InitializeOpenAsync ( cancellationToken );
        }

        /// <inheritdoc/>
        public async Task<bool> InitializeOpenAsync ( CancellationToken cancellationToken = default )
        {
            try
            {
                if ( IsDbContextValid () )
                {
                    await CloseAsync ( cancellationToken );
                }

                _dbContext = new ApplicationDbContext ( _connectionString );
                await _dbContext.InitializeAsync ( cancellationToken );

                bool isConnected = await _dbContext.Database.CanConnectAsync ( cancellationToken );

                return isConnected;
            }
            catch
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> CloseAsync ( CancellationToken cancellationToken = default )
        {
            try
            {
                if ( !IsDbContextValid () )
                {
                    return true;
                }

                await _dbContext!.DisposeAsync ();
                _dbContext = null;

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task<int> SaveChangesAsync ( CancellationToken cancellationToken = default )
        {
            try
            {
                if ( !IsDbContextValid () )
                {
                    return 0;
                }

                return await _dbContext!.SaveChangesAsync ( cancellationToken );
            }
            catch
            {
                return 0;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> SaveChangesAsync ( int expectedChanges , CancellationToken cancellationToken = default )
        {
            try
            {
                if ( !IsDbContextValid () )
                {
                    return false;
                }

                int changes = await _dbContext!.SaveChangesAsync ( cancellationToken );
                return changes == expectedChanges;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 共通処理
        /// <summary>
        /// DBコンテキストが有効かどうかを確認する
        /// </summary>
        /// <returns>
        /// 有効：<see langword="true"/><br/>
        /// 無効：<see langword="false"/>
        /// </returns>
        private bool IsDbContextValid ()
        {
            return _dbContext is not null;
        }
        #endregion

        #region 取得
        /// <inheritdoc/>
        public async Task<List<RequirementsDefinition>> GetRequirementsDefinitionsAsync ( CancellationToken cancellationToken = default )
        {
            if ( !IsDbContextValid () )
            {
                throw new InvalidOperationException ( "DBコンテキストが有効ではありません。OpenAsyncメソッドを呼び出してDBに接続してください。" );
            }

            return await _dbContext!.RequirementsDefinitions
                .OrderBy ( x => x.Id )
                .AsNoTracking ()
                .ToListAsync ( cancellationToken );
        }

        /// <inheritdoc/>
        public async Task<long> GetLastRequirementIdAsync ( CancellationToken cancellationToken = default )
        {
            if ( !IsDbContextValid () )
            {
                throw new InvalidOperationException ( "DBコンテキストが有効ではありません。OpenAsyncメソッドを呼び出してDBに接続してください。" );
            }

            return await _dbContext!.RequirementIds
                .OrderByDescending ( x => x.Id )
                .AsNoTracking ()
                .Select ( x => x.Id )
                .FirstOrDefaultAsync ( cancellationToken );
        }
        #endregion

        #region 挿入
        /// <inheritdoc/>
        public async Task<bool> InsertRequirementIdAsync ( RequirementId requirementId , CancellationToken cancellationToken = default )
        {
            if ( !IsDbContextValid () )
            {
                throw new InvalidOperationException ( "DBコンテキストが有効ではありません。OpenAsyncメソッドを呼び出してDBに接続してください。" );
            }

            try
            {
                await _dbContext!.RequirementIds.AddAsync ( requirementId , cancellationToken );
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
        #endregion
    }
}
