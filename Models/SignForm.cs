//------------------------------------------------------------------------------
// <auto-generated>
//    這個程式碼是由範本產生。
//
//    對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//    如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace AppWebServer.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class SignForm
    {
        public SignForm()
        {
            this.SignRecords = new HashSet<SignRecords>();
        }
    
        public int sId { get; set; }
        public int eId { get; set; }
        public Nullable<int> userLimit { get; set; }
        public Nullable<System.DateTime> dateStart { get; set; }
        public Nullable<System.DateTime> dateEnd { get; set; }
        public string field { get; set; }
        public Nullable<bool> isEnable { get; set; }
    
        public virtual EventPage EventPage { get; set; }
        public virtual ICollection<SignRecords> SignRecords { get; set; }
    }
}
