using Microsoft.EntityFrameworkCore;
using RequirementsDBEntity.Entities.Masters;
using RequirementsDBEntity.Entities.Transactions;
using System;

namespace RequirementsDBEntity
{
    /// <summary>
    /// DBコンテキストクラス
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        private readonly string _projectPath;

        /// <summary>
        /// 要求事項定義マスタのDbSet
        /// </summary>
        public DbSet<RequirementsDefinition> RequirementsDefinitions { get; set; }
        /// <summary>
        /// 要求事項IDテーブルのDbSet
        /// </summary>
        public DbSet<RequirementId> RequirementIds { get; set; }

        public ApplicationDbContext ()
        {
            _projectPath = AppDomain.CurrentDomain.BaseDirectory;
        }

        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        /// <param name="projectPath">プロジェクトのパス</param>
        public ApplicationDbContext ( string projectPath )
        {
            _projectPath = projectPath;
        }

        /// <summary>
        /// DB接続の設定を行う
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring ( DbContextOptionsBuilder optionsBuilder )
        {
            const string DATABASE_NAME = "requirements.db";
            string databasePath = Path.Combine ( _projectPath , DATABASE_NAME );

            optionsBuilder.UseSqlite ( $"DataSource={databasePath}" );
        }

        /// <summary>
        /// モデルの作成時に呼び出されるメソッド。エンティティの構成を行う。
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating ( ModelBuilder modelBuilder )
        {
            modelBuilder.Entity<RequirementsDefinition> ( entity =>
            {
                entity.HasKey ( x => x.Id ).HasName ( "m_requirements_definition_pkc" );
            } );

            modelBuilder.Entity<RequirementId> ( entity =>
            {
                entity.HasKey ( x => x.Id ).HasName ( "t_requirement_id_pkc" );
            } );
        }

        /// <summary>
        /// 初期化処理を実行する。必要に応じてデータベースの作成やマイグレーションを行う。
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task InitializeAsync ( CancellationToken cancellationToken = default )
        {
            bool isNew = !File.Exists ( Path.Combine ( _projectPath , "requirements.db" ) );

            string databasePath = Path.Combine ( _projectPath , "requirements.db" );
            DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext> ( )
                .UseSqlite ( $"DataSource={databasePath}" )
                .Options;

            using ApplicationDbContext context = new ApplicationDbContext ( _projectPath );
            await context.Database.MigrateAsync ( cancellationToken );

            if ( !isNew )
            {
                return;
            }

            DateTime dateTime = DateTime.UtcNow;
            // 初期データの投入
            List<RequirementsDefinition> initialData = new List<RequirementsDefinition>
            {
                new RequirementsDefinition
                {
                    Id = 1,
                    RequirementDefinition = "F" ,
                    CreatedAt = dateTime ,
                    CreateUser = "System" ,
                    CreateProgram = "ApplicationDbContext" ,
                    UpdatedAt = dateTime ,
                    UpdateUser = "System" ,
                    UpdateProgram = "ApplicationDbContext"
                } ,
                new RequirementsDefinition
                {
                    Id = 2,
                    RequirementDefinition = "NF" ,
                    CreatedAt = dateTime ,
                    CreateUser = "System" ,
                    CreateProgram = "ApplicationDbContext" ,
                    UpdatedAt = dateTime ,
                    UpdateUser = "System" ,
                    UpdateProgram = "ApplicationDbContext"
                }
            };

            await context.RequirementsDefinitions.AddRangeAsync ( initialData , cancellationToken );
            await context.SaveChangesAsync ( cancellationToken );
        }
    }
}
