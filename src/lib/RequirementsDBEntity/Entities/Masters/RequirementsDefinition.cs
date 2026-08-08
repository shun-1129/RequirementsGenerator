using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RequirementsDBEntity.Entities.Masters
{
    [Table ( "m_requirements_definition" )]
    [Comment ( "要求事項定義マスタ" )]
    public class RequirementsDefinition : BaseEntityColumn
    {
        [Key]
        [Required]
        [Column ( "id" )]
        [Comment ( "要求事項定義ID" )]
        [DatabaseGenerated ( DatabaseGeneratedOption.None )]
        public int Id { get; set; }

        [Required]
        [Column ( "requirement_definition" )]
        [Comment ( "要求事項定義:【値例】F：機能要求 , NF：非機能要求" )]
        public string RequirementDefinition { get; set; } = string.Empty;
    }
}
