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
    
    public partial class Stock_exchange
    {
        public int Investigation_ID { get; set; }
        public Nullable<int> Deal { get; set; }
        public Nullable<int> Person { get; set; }
    
        public virtual Deal Deal1 { get; set; }
        public virtual User User { get; set; }
    }
}
