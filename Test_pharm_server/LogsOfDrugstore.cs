//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Test_pharm_server
{
    using System;
    using System.Collections.Generic;
    
    public partial class LogsOfDrugstore
    {
        public int ID_PH { get; set; }
        public string SystemLog { get; set; }
        public System.DateTime DateOfUpdating { get; set; }
    
        public virtual Pharmacy Pharmacy { get; set; }
    }
}
