//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Birzha.Data_base
{
    using System;
    using System.Collections.Generic;
    
    public partial class Deal
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Deal()
        {
            this.Stocks = new HashSet<Stock>();
            this.Stock_exchange = new HashSet<Stock_exchange>();
        }
    
        public int Deal_ID { get; set; }
        public int Type { get; set; }
        public System.DateTime Deal_date { get; set; }
        public Nullable<int> Broker { get; set; }
    
        public virtual Broker Broker1 { get; set; }
        public virtual Deal_type Deal_type { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Stock> Stocks { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Stock_exchange> Stock_exchange { get; set; }
    }
}