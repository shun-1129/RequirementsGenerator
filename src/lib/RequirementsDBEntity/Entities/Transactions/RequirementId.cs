using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RequirementsDBEntity.Entities.Transactions
{
    [Table ( "t_requirement_id" )]
    [Comment ( "要求事項IDテーブル" )]
    public class RequirementId : BaseEntityColumn
    {
        [Key]
        [Column ( "id" )]
        [Comment ( "要求事項ID" )]
        [DatabaseGenerated ( DatabaseGeneratedOption.Identity )]
        public long Id { get; set; }

        [Required]
        [Column ( "requirement_definition_id" )]
        [Comment ( "要求事項定義マスタID" )]
        public int RequirementDefinitionId { get; set; }
    }
}
