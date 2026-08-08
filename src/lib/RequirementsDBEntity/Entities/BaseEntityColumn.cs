using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RequirementsDBEntity.Entities
{
    public class BaseEntityColumn
    {
        /// <summary>
        /// 作成日時
        /// </summary>
        [Column ( "created_at" )]
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// 作成者
        /// </summary>
        [Column ( "create_user" )]
        [StringLength ( 128 )]
        public string CreateUser { get; set; } = string.Empty;
        /// <summary>
        /// 作成プログラム
        /// </summary>
        [Column ( "create_program" )]
        [StringLength ( 128 )]
        public string CreateProgram { get; set; } = string.Empty;
        /// <summary>
        /// 更新日時
        /// </summary>
        [Column ( "updated_at" )]
        [StringLength ( 128 )]
        public DateTime UpdatedAt { get; set; }
        /// <summary>
        /// 更新者
        /// </summary>
        [Column ( "update_user" )]
        [StringLength ( 128 )]
        public string UpdateUser { get; set; } = string.Empty;
        /// <summary>
        /// 更新プログラム
        /// </summary>
        [Column ( "update_program" )]
        [StringLength ( 128 )]
        public string UpdateProgram { get; set; } = string.Empty;
    }
}
