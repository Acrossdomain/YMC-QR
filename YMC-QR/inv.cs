using System;
using System.Collections.Generic;

namespace VELOCIS_EINV
{
    public class inv
    {
        public object transaction_details { get; set; }
        public object document_details { get; set; }
        //public object export_details { get; set; }
       // public object extra_Information { get; set; }
        public object billing_Information { get; set; }
        public object shipping_Information { get; set; }
        public object delivery_Information { get; set; }
       // public object payee_Information { get; set; }
       // public object ewaybill_information { get; set; }
        public object document_Total { get; set; }
        public List<items> items;
    }
}