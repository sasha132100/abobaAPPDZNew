//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace abobaAPP
{
    using System;
    using System.Collections.Generic;
    
    public partial class OrderStatus
    {
        public OrderStatus()
        {
            this.Order = new HashSet<Order>();
        }
    
        public int OrderStatusID { get; set; }
        public string OrderStatusName { get; set; }
    
        public virtual ICollection<Order> Order { get; set; }
    }
}