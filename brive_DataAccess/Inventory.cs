
namespace brive_DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Inventory
    {
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int BranchId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "¡No se puede tener unidades negativas!")]
        public int BranchUnits { get; set; }
    
        public virtual Product Product { get; set; }
    }
}
