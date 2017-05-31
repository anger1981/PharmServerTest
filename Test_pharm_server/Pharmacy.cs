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
    
    public partial class Pharmacy
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Pharmacy()
        {
            this.AbsentProducts = new HashSet<AbsentProduct>();
            this.Announcements = new HashSet<Announcement>();
            this.DatesOfTransfers = new HashSet<DatesOfTransfer>();
            this.HistoryOfChangesOfPrices = new HashSet<HistoryOfChangesOfPrice>();
            this.HistoryOfReceptions = new HashSet<HistoryOfReception>();
            this.InformationOfSettings = new HashSet<InformationOfSetting>();
            this.ListOfSettings = new HashSet<ListOfSetting>();
            this.LogsOfDrugstores = new HashSet<LogsOfDrugstore>();
            this.price_list = new HashSet<price_list>();
            this.RegistrationOfDrugstores = new HashSet<RegistrationOfDrugstore>();
            this.ReportsOfImportingOfPriceLists = new HashSet<ReportsOfImportingOfPriceList>();
        }
    
        public int Id_Pharmacy { get; set; }
        public string Name_full { get; set; }
        public string Addr { get; set; }
        public string Phone { get; set; }
        public string Mail { get; set; }
        public string Web { get; set; }
        public string Hours { get; set; }
        public string Trans { get; set; }
        public Nullable<System.DateTime> Date_upd { get; set; }
        public int Id_District { get; set; }
        public bool Is_deleted { get; set; }
        public string Notes { get; set; }
        public Nullable<bool> VisibleToOperators { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AbsentProduct> AbsentProducts { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Announcement> Announcements { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DatesOfTransfer> DatesOfTransfers { get; set; }
        public virtual District District { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HistoryOfChangesOfPrice> HistoryOfChangesOfPrices { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HistoryOfReception> HistoryOfReceptions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InformationOfSetting> InformationOfSettings { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ListOfSetting> ListOfSettings { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LogsOfDrugstore> LogsOfDrugstores { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<price_list> price_list { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RegistrationOfDrugstore> RegistrationOfDrugstores { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReportsOfImportingOfPriceList> ReportsOfImportingOfPriceLists { get; set; }
    }
}
