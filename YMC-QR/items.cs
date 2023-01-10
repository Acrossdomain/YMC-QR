using System;

namespace VELOCIS_EINV
{
    public class items
    {
        public int slno { get; set; }
       // public string item_Description { get; set; }
        public string service { get; set; }
        public string hsn_code { get; set; }
       // public object batch;
       // public string barcode { get; set; }
        public int quantity { get; set; }
       // public int freeQty { get; set; }
        public string uqc { get; set; }
        public Decimal rate { get; set; }
        public Decimal grossAmount { get; set; }
        public Decimal discountAmount { get; set; }
       // public int preTaxAmount { get; set; }
        public Decimal assesseebleValue { get; set; }
        public Decimal igst_rt { get; set; }
        public Decimal cgst_rt { get; set; }
        public Decimal sgst_rt { get; set; }
        public Decimal cess_rt { get; set; }
        public Decimal iamt { get; set; }
        public Decimal camt { get; set; }
        public Decimal samt { get; set; }
        public Decimal csamt { get; set; }
        //public int cessnonadval { get; set; }
        //public int state_cess { get; set; }
        //public int stateCessAmt { get; set; }
        //public int stateCesNonAdvlAmt { get; set; }
        public Decimal otherCharges { get; set; }
        public Decimal itemTotal { get; set; }
       // public string ordLineRef { get; set; }
        public string origin_Country { get; set; }
        //public string prdSlNo { get; set; }
       // public object attribDtls;

    }
}