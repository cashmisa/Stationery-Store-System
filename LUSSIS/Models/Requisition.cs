namespace LUSSIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Requisition")]
    public partial class Requisition
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Requisition()
        {
            RequisitionDetails = new HashSet<RequisitionDetail>();
        }

        public int RequisitionId { get; set; }

        public int? RequisitionEmpNum { get; set; }

        [Column(TypeName = "date")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? RequisitionDate { get; set; }

        public int? ApprovalEmpNum { get; set; }

        public string ApprovalRemarks { get; set; }

        public string RequestRemarks { get; set; }

        [Column(TypeName = "date")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? ApprovalDate { get; set; }

        [StringLength(20)]
        public string Status { get; set; }

        public virtual Employee ApprovalEmployee { get; set; }

        public virtual Employee RequisitionEmployee { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RequisitionDetail> RequisitionDetails { get; set; }
    }
}
