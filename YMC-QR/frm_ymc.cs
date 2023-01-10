using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VELOCIS_EINV;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Net.Http;
using System.Net;
using System.Dynamic;
using System.IO;

namespace VEL_E_INV
{
    public partial class frm_ymc : Form
    {
        public Dictionary<string, string> errList;
        protected internal string SERVERNAME = null;
        protected internal string USERNAME = null;
        protected internal string PASSWORD = null;
        protected internal string SAGEDB = null;
        protected internal string SAA = null;
        protected internal string SAPSS = null;
        protected internal string SGSTIN = null;
        protected internal string BGSTIN = null;
        protected internal string HSNCODE1 = null;
        protected internal string HSNCODE2 = null;
        protected internal string STATE = null;
        protected internal string PINCODE = null;
        protected internal string INVNUM = null;
        protected internal string APIURL = null;
        public string tabname = "";
        public string CRDNOTEstr = "";
        protected internal string apitype = null;
        string filter_0;
        string Datatype;
		string invoice_type_code_B2C = "";
		dynamic person = new ExpandoObject();
        DataSet dsGenEWB;
        public frm_ymc()
        {
            InitializeComponent();
            CredentialsXml();
            // getAccAPI();

            
            //cmbxInvoice.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            comboBox1.SelectedIndex = 0;
            cmbTransMd.SelectedIndex = 0;
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            ClearControll();

            if (checkINRNOExist(cmbxInvoice.SelectedValue.ToString().Trim()) == "TRUE")
            {
                return;
            }

            //if (cmbTransMd.Text != "")
            //{
            if (cmbxInvoice.SelectedValue.ToString().Trim() != "")
            {

                if (comboBox1.SelectedItem.ToString() == "INV")
                {
                    if (checkGtnAvl(cmbxInvoice.SelectedValue.ToString().Trim()) == "FALSE")
                    {
                        MessageBox.Show("GSTIN not avaible of this Invoice, Please try another invoice!");
                        return;
                    }
                    GetInvDetbyinvNo("");
                }
                if (comboBox1.SelectedItem.ToString() == "CRN")
                {
                    if (checkGtnAvlCRDND(cmbxInvoice.SelectedValue.ToString().Trim()) == "FALSE")
                    {
                        MessageBox.Show("GSTIN not avaible of this Invoice, Please try another invoice!");
                        return;
                    }
                    GetInvDetbyCRD("");
                }
                if (comboBox1.SelectedItem.ToString() == "DBN")
                {
                    if (checkGtnAvlCRDND(cmbxInvoice.SelectedValue.ToString().Trim()) == "FALSE")
                    {
                        MessageBox.Show("GSTIN not avaible of this Invoice, Please try another invoice!");
                        return;
                    }
                    GetInvDetbyCRD("");
                }
            }
            else
                MessageBox.Show("Please Enter Invoice Number!");
            //}
            //else
            //    MessageBox.Show("Please Select Transaction Type!");

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            //and d.QTYSHIPPED<>0
            if (comboBox2.SelectedItem.ToString() == "YES")   //Zero item value
            {
                Datatype = "Z";
                //filter_0 = " and d.QTYSHIPPED <> 0 ";
				filter_0 = " AND D.EXTINVMISC>=0 ";
				//AND D.EXTINVMISC>=0
			}
			else if (comboBox2.SelectedItem.ToString() == "NO")     //Non zero item value and Qauntity
            { filter_0 = " and d.QTYSHIPPED <> 0 AND d.TBASE1<>0 ";
				filter_0 = " AND D.EXTINVMISC>=0 ";
				Datatype = "N"; }
        }
        public void ClearControll()
        {
            txtEwaybillNo.Text = string.Empty;
            txtEwatValTo.Text = string.Empty;
            txtEWayDate.Text = string.Empty;
            SGst.Text = string.Empty;
            SCity.Text = string.Empty;
            sState.Text = string.Empty;
            sname.Text = string.Empty;
            SrichTextBox1.Text = string.Empty;

            Bgst.Text = string.Empty;
            Bname.Text = string.Empty;
            BCity.Text = string.Empty;
            Bstate.Text = string.Empty;
            BrichTextBox2.Text = string.Empty;

            SHCity.Text = string.Empty;
            SHToPin.Text = string.Empty;
            SHPTO.Text = string.Empty;
            SHrichTextBox4.Text = string.Empty;
            SHState.Text = string.Empty;

            txtTotASSVal.Text = string.Empty;
            txtTotGST.Text = string.Empty;
            txtTotInvVal.Text = string.Empty;

            txtTransNumberVehN.Text = string.Empty;
            txtDistance.Text = string.Empty;

        }
        public string checkINRNOExist(string invn)
        {
            DataSet dt;
            String strReturn = "FALSE";
            string connectionstring = "Data Source=" + SERVERNAME + "; Initial Catalog=" + SAGEDB + "; User ID=" + SAA + "; Password=" + SAPSS + ";";
            System.Data.SqlClient.SqlConnection conn;
            System.Data.SqlClient.SqlCommand cmd;
            conn = new System.Data.SqlClient.SqlConnection(connectionstring);
            try
            {
                conn.Open();
                string Querystring = "SELECT * FROM OEIRNO H WHERE H.INVNUMBER='" + invn + "'";
                cmd = new System.Data.SqlClient.SqlCommand(Querystring, conn);
                cmd.CommandTimeout = 180;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                using (System.Data.SqlClient.SqlDataAdapter sda = new System.Data.SqlClient.SqlDataAdapter())
                {
                    cmd.Connection = conn;
                    sda.SelectCommand = cmd;
                    using (dt = new DataSet())
                    {
                        sda.Fill(dt);
                        if (dt.Tables[0].Rows.Count > 0)
                        {
                            MessageBox.Show("IRN Number already Genereted of this Invoice!");
                            txtEwaybillNo.Text = dt.Tables[0].Rows[0]["EWAYBILNO"].ToString();
                            txtEWayDate.Text = dt.Tables[0].Rows[0]["EWBDATE"].ToString();
                            txtEwatValTo.Text = dt.Tables[0].Rows[0]["EWBVLTO"].ToString();
                            string qrcode = dt.Tables[0].Rows[0]["QRCODe"].ToString();
                            string IRNNO = dt.Tables[0].Rows[0]["IRNNO"].ToString();
                            if (File.Exists(@"QR/" + invn.Replace("/", "") + ".png"))
                            {
                            }
                            else
                            {
                                listBox1.Items.Add("QR Thumb not existing, Please wait creating QR Thumb! ");
                                csQRCode objQr = new csQRCode();
                                if (qrcode != "" || qrcode != null)
                                {
									//listBox1.Items.Add(IRNNO);
									if (IRNNO.Trim() != "B2C") {
                                    if (objQr.createQrImage(IRNNO, invn, qrcode.ToString()) == true)
                                    {
                                        listBox1.Items.Add("QR Thumb created!");
                                    }
									}
									else
									{
										if (objQr.createQrImageBS64(IRNNO, invn, qrcode.ToString()) == true)
										{
											listBox1.Items.Add("QR Thumb created!");
										}
									}
								}
                            }
                            strReturn = "TRUE";
                        }
                        else
                            strReturn = "FALSE";
                    }
                }
            }
            catch (Exception ex)
            {
                conn.Close();
                strReturn = "FALSE";
                listBox1.Items.Add(ex.Message);
            }
            conn.Close();
            return strReturn;
        }
        public void GetInvDetbyinvNo(string strInvoice)
        {
           // filter_0 = "  and d.QTYSHIPPED <> 0 AND d.TBASE1<>0";
           // filter_0 = " and d.QTYSHIPPED <> 0  ";
            string connectionstring = "Data Source=" + SERVERNAME + "; Initial Catalog=" + SAGEDB + "; User ID=" + SAA + "; Password=" + SAPSS + ";";
            //MessageBox.Show(connectionstring);
            //constr = "Provider=SQLOLEDB;Data Source=ERP-DATABASE; Initial Catalog=TSTDAT;User ID=sa; Password=Vspl@4321"
            //string connectionstring = "Data Source=ERP-DATABASE; Initial Catalog=TSTDAT; User ID=sa; Password=Vspl@4321;";
            System.Data.SqlClient.SqlConnection conn;
            System.Data.SqlClient.SqlCommand cmd;
            conn = new System.Data.SqlClient.SqlConnection(connectionstring);
            conn.Open();
            string Querystring = "select rtrim((select value from  CSOPTFD where OPTFIELD='GSTNOS' and left(value,2)=left(H.TAXGROUP,2))) supplier_GSTIN, ";
            Querystring += " (select RTRIM(CONAME) from  CSCOM) supplier_Legal_Name,  (Select RTRIM(CITY) from ICLOC a1 where a1.LOCATION=H.LOCATION) supplier_City, ";
            Querystring += " (Select RTRIM(ADDRESS1)+' '+RTRIM(ADDRESS2) from ICLOC a1 where a1.LOCATION=H.LOCATION) supplier_Address1, case when substring(H.TAUTH2 ,3,3)='TCS' then H.TEAMOUNT2 else 	case when substring(H.TAUTH3 ,3,3)='TCS' then H.TEAMOUNT3  else 0 end end OthChrg,  ";
            Querystring += " (Select RTRIM(ADDRESS3)+' '+RTRIM(ADDRESS4) from ICLOC a1 where a1.LOCATION=H.LOCATION) supplier_Address2, ";
            Querystring += " (Select left(h.taxgroup,2) from ICLOC a1 where a1.LOCATION=H.LOCATION) supplier_State, ";
            Querystring += " (Select RTRIM(ZIP) from ICLOC a1 where a1.LOCATION=H.LOCATION) supplier_Pincode, ";
			Querystring += " CASE WHEN H1.VALUE='R' THEN 'B2B' WHEN  H1.VALUE='SEWP' THEN 'SEZWP' WHEN  H1.VALUE='SEWOP' THEN 'SEZWOP' WHEN  H1.VALUE='EXWP' THEN 'EXPWP' WHEN  H1.VALUE='EXWOP' THEN 'EXPWOP' WHEN  H1.VALUE='DE' THEN 'DEXP' ELSE 'B2B' END invoice_type_code ,";
			// Querystring += " ISNULL((select value from  OEINVHO where INVUNIQ=h.INVUNIQ and OPTFIELD='GINVTYPE'),'') invoice_type_code, ";
            Querystring += " CASE when SUBSTRING(h.TAXGROUP,5,1)='R' then 'Y' else 'N' end reversecharge,'INV' invoice_subtype_code,RTRIM(h.INVNUMBER)invoiceNum, ";
            Querystring += " SUBSTRING(CAST(h.INVDATE AS CHAR),7,2)+'-'+SUBSTRING(CAST(h.INVDATE AS CHAR),5,2)+'-'+SUBSTRING(CAST(h.INVDATE AS CHAR),1,4) invoiceDate, ";
            Querystring += " RTRIM(h.BILNAME) billing_Name,rtrim(ISNULL((select value from  OEINVHO where INVUNIQ=h.INVUNIQ and OPTFIELD='GPOS'),'')) billing_POS, ";
            Querystring += " ar.VALUE billing_GSTIN, h.BILCITY billing_City,rtrim(ISNULL((select s.VALUE from ARCUSO s where h.CUSTOMER=s.IDCUST and s.OPTFIELD='GSTCODE'),'')) billing_State, ";
            Querystring += " RTRIM(H.BILADDR1) billing_Address1,RTRIM(H.BILADDR2) billing_Address2,RTRIM(H.BILZIP) billing_Pincode, ";
            Querystring += " H.TBASE1*(H.INRATE) total_assVal,case when substring(H.TAUTH1 ,3,3)='CGN' then H.TEAMOUNT1 else 0 end*(H.INRATE) cgstvalue,case when substring(H.TAUTH2 ,3,3)='SGN' then H.TEAMOUNT2 else 0 end*(H.INRATE) sgstvalue, ";
            Querystring += " case when substring(H.TAUTH1 ,3,3)='IGN' then H.TEAMOUNT1 else 0 end*(H.INRATE) igstvalue, case when substring(H.TAUTH2 ,3,3)='CEN' then H.TEAMOUNT2 else  ";
            Querystring += " case when substring(H.TAUTH3 ,3,3)='CEN' then H.TEAMOUNT3  else 	0 end end*(H.INRATE) cessvalue,0 Discount,case when substring(d.TAUTH2 ,3,3)='TCS' then d.TAMOUNT2 else case when substring(d.TAUTH3 ,3,3)='TCS' then d.TAMOUNT3  else 	0 end end*(H.INRATE) othercharges,0 roundoff,H.INVNETWTX*(H.INRATE) total_Invoice_Value, ";
            Querystring += " 0 val_for_cur,row_number() over(partition by d.INVUNIQ order by d.INVUNIQ) SLno, ";
			Querystring += " CASE  WHEN D.LINETYPE=1 AND  ISNULL((SELECT VALUE FROM  ICITEMO WHERE ITEMNO=I.ITEMNO AND OPTFIELD='GITEMTYPE'),'N')='S' THEN 'Y'  WHEN D.LINETYPE=2 AND ISNULL((SELECT VALUE FROM  OEMISCO  WHERE MISCCHARGE=D.MISCCHARGE AND OPTFIELD='GITEMTYPE' AND CURRENCY=H.INSOURCURR),'N')='S' THEN 'Y' ELSE 'N' END service, "; 
			Querystring += " ISNULL((select SUBSTRING(value,1,8) value from  OEINVDO where INVUNIQ=d.INVUNIQ and LINENUM=d.LINENUM and OPTFIELD='GHSNCODE'),'') hsn_code, ";
            Querystring += " d.QTYSHIPPED quantity, ISNULL((Select RTRIM(VDESC) from CSOPTFD where OPTFIELD='GUOM' and RTRIM([value])=d.INVUNIT),'OTH') uqc,";
            Querystring += " d.unitprice*(H.INRATE) rate,d.EXTINVMISC*(H.INRATE) grossAmount,(d.INVDISC+d.HDRDISC)*(H.INRATE) discountAmount,d.TBASE1*(H.INRATE) assesseebleValue,case when substring(d.TAUTH1 ,3,3)='IGN' then d.TRATE1 else 0  end igst_rt, ";
            Querystring += " case when substring(d.TAUTH1 ,3,3)='CGN' then d.TRATE1 else 0 end cgst_rt,case when substring(d.TAUTH2 ,3,3)='SGN' then d.TRATE2 else 0 end sgst_rt, ";
            Querystring += " case when substring(d.TAUTH2 ,3,3)='CEN' then d.TRATE2 else case when substring(d.TAUTH3 ,3,3)='CEN' then d.TRATE3  else 0 end end cess_rt, ";
            Querystring += " case when substring(d.TAUTH1 ,3,3)='IGN' then d.TAMOUNT1 else 0 end*(H.INRATE) iamt,case when substring(d.TAUTH1 ,3,3)='CGN' then d.TAMOUNT1 else 0 end*(H.INRATE) camt, ";
            Querystring += " case when substring(d.TAUTH2 ,3,3)='SGN' then d.TAMOUNT2 else 0 end*(H.INRATE) samt,case when substring(d.TAUTH2 ,3,3)='CEN' then d.TAMOUNT2 else  ";
            Querystring += " case when substring(d.TAUTH3 ,3,3)='CEN' then d.TAMOUNT3  else 	0 end end*(H.INRATE) csamt,(d.TBASE1+d.TAMOUNT1+d.TAMOUNT2+d.TAMOUNT3)*(H.INRATE) itemTotal ,RTRIM(H.LOCATION) location_code  ";
            Querystring += " ,H.SHPNAME,H.SHIPTO,H.SHPADDR1,H.SHPADDR2,H.SHPCITY,H.SHPCOUNTRY,H.SHPSTATE,H.SHPZIP , h.INVETAXTOT*(H.INRATE) TOTGST,ISNULL((SELECT SUM(D1.EXTINVMISC) EXTINVMISC FROM OEINVD D1 WHERE D.INVUNIQ=D1.INVUNIQ AND D1.MISCCHARGE='ROUND'),0)*-1*(H.INRATE) HDISCOUNT  ";
            Querystring += " from oeinvh h left outer join OEINVD d on h.invuniq=d.INVUNIQ AND D.MISCCHARGE<>'ROUND' left outer join ICITEM i on d.item=i.FMTITEMNO left outer join ICITEMo o on i.ITEMNO=o.ITEMNO and o.OPTFIELD='GHSNCODE' ";
            Querystring += " left outer join ARCUSO ar on h.customer=ar.IDCUST and ar.OPTFIELD='GSTIN' LEFT OUTER JOIN OEINVHO H1 ON H.INVUNIQ=H1.INVUNIQ AND RTRIM(H1.OPTFIELD)='GINVTYPE'   ";
            Querystring += " where substring(h.TAXGROUP,3,3) In('IGN','CGN','IGX')  "+filter_0+"  and  h.INVNUMBER='" + cmbxInvoice.SelectedValue.ToString().Trim() + "' ";
            cmd = new System.Data.SqlClient.SqlCommand(Querystring, conn);
            cmd.CommandTimeout = 180;
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
            dsGenEWB = new DataSet();
			//ISNULL((SELECT SUM(D1.EXTINVMISC) EXTINVMISC FROM OEINVD D1 WHERE D.INVUNIQ=D1.INVUNIQ AND D1.MISCCHARGE='ROUND'),0)*-1*(H.INRATE) HDISCOUNT
			using (System.Data.SqlClient.SqlDataAdapter sda = new System.Data.SqlClient.SqlDataAdapter())
            {
                
                var objerrList = (IDictionary<string, object>)person;
                cmd.Connection = conn;
                sda.SelectCommand = cmd;
                    sda.Fill(dsGenEWB);
                if (dsGenEWB.Tables[0].Rows.Count > 0)
                {
                    if (CheckValidation(dsGenEWB.Tables[0]) == true)
                    {
                        MessageBox.Show("Validation Passed, You can proceed Import/Generate IRN.");

                    }
                    else { MessageBox.Show("Validation Failed.."); }

                    //seller details
                    txtSupplyType.Text = dsGenEWB.Tables[0].Rows[0]["invoice_type_code"].ToString();
                    SGst.Text = dsGenEWB.Tables[0].Rows[0]["supplier_GSTIN"].ToString();
                    string gstin = dsGenEWB.Tables[0].Rows[0]["supplier_GSTIN"].ToString();

                    sname.Text = dsGenEWB.Tables[0].Rows[0]["supplier_Legal_Name"].ToString();
                    sState.Text = dsGenEWB.Tables[0].Rows[0]["supplier_State"].ToString();
                    SrichTextBox1.Text = dsGenEWB.Tables[0].Rows[0]["supplier_Address1"].ToString() + " " + dsGenEWB.Tables[0].Rows[0]["supplier_Address2"].ToString();
                    SCity.Text = dsGenEWB.Tables[0].Rows[0]["supplier_City"].ToString();

                    //Buyer Detail
                    Bgst.Text = dsGenEWB.Tables[0].Rows[0]["billing_GSTIN"].ToString();
                    Bname.Text = dsGenEWB.Tables[0].Rows[0]["billing_Name"].ToString();
                    Bstate.Text = dsGenEWB.Tables[0].Rows[0]["billing_POS"].ToString();
                    BrichTextBox2.Text = dsGenEWB.Tables[0].Rows[0]["billing_Address1"].ToString() + " " + dsGenEWB.Tables[0].Rows[0]["billing_Address2"].ToString();
                    BCity.Text = dsGenEWB.Tables[0].Rows[0]["billing_City"].ToString();

                    //Shipment Detail
                    SHCity.Text = dsGenEWB.Tables[0].Rows[0]["SHPCITY"].ToString();
                    SHToPin.Text = dsGenEWB.Tables[0].Rows[0]["SHPZIP"].ToString();
                    SHState.Text = dsGenEWB.Tables[0].Rows[0]["SHPSTATE"].ToString();
                    SHrichTextBox4.Text = dsGenEWB.Tables[0].Rows[0]["SHPADDR1"].ToString() + " " + dsGenEWB.Tables[0].Rows[0]["SHPADDR2"].ToString();
                    SHPTO.Text = dsGenEWB.Tables[0].Rows[0]["SHPNAME"].ToString();

                    txtTotASSVal.Text = dsGenEWB.Tables[0].Rows[0]["total_assVal"].ToString();
                   txtTotGST.Text = dsGenEWB.Tables[0].Rows[0]["TOTGST"].ToString();
                    txtTotInvVal.Text = dsGenEWB.Tables[0].Rows[0]["total_Invoice_Value"].ToString();


                    //EwayBill Detail
                    //txttransID.Text = dsGenEWB.Tables[0].Rows[0]["TransId"].ToString().Trim();
                }
                else
                   MessageBox.Show("Invoice data not found!");
            }
            conn.Close();
        }
        public Boolean CheckValidation(DataTable dtValidate)
        {
            Boolean val_status = true;
            foreach (DataRow row in dtValidate.Rows)
            {
                if (row["supplier_GSTIN"].ToString().Length == 15)//* mandatory
                { }
                else
                {
                    val_status = false;
                    listBox1.Items.Add("supplier_GSTIN= " + row["supplier_GSTIN"].ToString());
                }
                if (row["supplier_Legal_Name"].ToString().Length >= 3 && row["supplier_Legal_Name"].ToString().Length <= 100)//* mandatory
                { }
                else
                {
                    val_status = false;
                    listBox1.Items.Add("supplier_Legal_Name= " + row["supplier_Legal_Name"].ToString());
                }
                if (row["supplier_City"].ToString().Length >= 3 && row["supplier_City"].ToString().Length <= 50)//* mandatory
                { }
                else
                {
                    val_status = false;
                    listBox1.Items.Add("supplier_City= " + row["supplier_City"].ToString());
                }
                if (row["supplier_Address1"].ToString().Length >= 3 && row["supplier_Address1"].ToString().Length <= 100)//* mandatory
                { }
                else
                {
                    val_status = false;
                    listBox1.Items.Add("supplier_Address1= " + row["supplier_Address1"].ToString());
                }
                if (row["supplier_State"].ToString().Length <= 2)//* mandatory
                { }
                else
                {
                    val_status = false;
                    listBox1.Items.Add("supplier_State= " + row["supplier_State"].ToString());
                }
                if (row["supplier_Pincode"].ToString().Replace(" ", "").Length == 6)//* mandatory
                { }
                else
                {
                    val_status = false;
                    listBox1.Items.Add("supplier_Pincode= " + row["supplier_Pincode"].ToString().Replace(" ", ""));
                }
                //if (row["supplier_Phone"].ToString().Length == 15)
                //{
                //}
                //if (row["supplier_Email"].ToString().Length == 15)
                //{
                //}
                //if (row["supplier_Address2"].ToString().Length<=3)
                //{
                //}
                //if (row["supplier_trading_name"].ToString().Length <= 3)
                //{
                //}
                ///transaction_details
                //if (row["transactionMode"].ToString().Length == 3)//* mandatory
                // { }
                // else
                // {
                //  errList.Add("transactionMode", row["transactionMode"].ToString());
                // }
                // if (row["invoice_type_code"].ToString().Trim().Length >= 3 && row["invoice_type_code"].ToString().Trim().Length <= 10)//* mandatory
                // { }
                //  else
                // {
                //   errList.Add("invoice_type_code", row["invoice_type_code"].ToString());
                //  }
                if (row["reversecharge"].ToString().Length == 1)//* mandatory
                { }
                else
                {
                    val_status = false;
                    listBox1.Items.Add("reversecharge= " + row["reversecharge"].ToString());
                }
                //if (row["ecom_GSTIN"].ToString().Length == 15)//* mandatory
                //{
                //}
                //if (row["IgstOnIntra"].ToString().Length == 15)
                //{
                //}

                //document_details
                if (row["invoice_subtype_code"].ToString().Length == 3)//* mandatory
                { }
                else
                {
                    val_status = false;
                    listBox1.Items.Add("invoice_subtype_code= " + row["invoice_subtype_code"].ToString());
                }
                if (row["invoiceNum"].ToString().Length >= 1 && row["invoiceNum"].ToString().Length <= 16)//* mandatory
                { }
                else
                {
                    val_status = false;
                    listBox1.Items.Add("invoiceNum= " + row["invoiceNum"].ToString());
                }
                if (row["invoiceDate"].ToString().Length == 10)//* mandatory
                { }
                else
                {
                    val_status = false;
                    listBox1.Items.Add("invoiceDate= " + row["invoiceDate"].ToString());
                }
                //if (row["transaction_id"].ToString().Length == 15)
                //{
                //}
                //if (row["plant"].ToString().Length == 15)
                //{
                //}
                //if (row["custom"].ToString().Length == 15)
                //{
                //}

                //export_details

                //if (row["shipping_bill_no"].ToString().Length == 15)
                //{
                //}
                //if (row["shipping_bill_date"].ToString().Length == 15)
                //{
                //}
                //if (row["port_code"].ToString().Length == 15)
                //{
                //}
                //if (row["invoice_currency_code"].ToString().Length == 15)
                //{
                //}
                //if (row["cnt_code"].ToString().Length == 15)
                //{
                //}
                //if (row["RefClm"].ToString().Length == 15)
                //{
                //}
                //if (row["ExpDuty"].ToString().Length == 15)
                //{
                //}

                //extra_Information 

                //if (row["remarks"].ToString().Length == 15)
                //{
                //}
                //if (row["invoice_Period_Start_Date"].ToString().Length == 15)
                //{
                //}
                //if (row["invoice_Period_End_Date"].ToString().Length == 15)
                //{
                //}
                if (tabname == "CREDITNOTE")
                {
                    if (row["preceeding_Invoice_Number"].ToString().Trim().Length <= 20)
                    {
                        //MessageBox.Show("True"+ row["preceeding_Invoice_Number"].ToString());
                    }
                    else
                    {
                        // MessageBox.Show("false" + row["preceeding_Invoice_Number"].ToString());
                        val_status = false;
                        listBox1.Items.Add("preceeding_Invoice_Number= " + row["preceeding_Invoice_Number"].ToString());
                    }
                    if (row["preceeding_Invoice_Date"].ToString().Length == 10)
                    { }
                    else
                    {
                        val_status = false;
                        listBox1.Items.Add("preceeding_Invoice_Date= " + row["preceeding_Invoice_Date"].ToString());
                    }
                }
                //if (row["invoice_Document_Reference"].ToString().Length == 15)
                //{
                //}
                //if (row["receipt_Advice_ReferenceNo"].ToString().Length == 15)
                //{
                //}
                //if (row["receipt_Advice_ReferenceDt"].ToString().Length == 15)
                //{
                //}
                //if (row["tender_or_Lot_Reference"].ToString().Length == 15)
                //{
                //}
                //if (row["contract_Reference"].ToString().Length == 15)
                //{
                //}
                //if (row["external_Reference"].ToString().Length == 15)
                //{
                //}
                //if (row["project_Reference"].ToString().Length == 15)
                //{
                //}
                //if (row["refNum"].ToString().Length == 15)
                //{
                //}
                //if (row["refDate"].ToString().Length == 15)
                //{
                //}
                //if (row["Url"].ToString().Length == 15)
                //{
                //}
                //if (row["Docs"].ToString().Length == 15)
                //{
                //}
                //if (row["Info"].ToString().Length == 15)
                //{
                //}

                //billing_Information/buyer information

                if (row["billing_Name"].ToString().Length >= 3 && row["billing_Name"].ToString().Length <= 100)//* mandatory
                { }
                else
                {
                    val_status = false;
                    listBox1.Items.Add("billing_Name= " + row["billing_Name"].ToString());
                }
                if (row["billing_GSTIN"].ToString().Trim().Length >= 3 && row["billing_GSTIN"].ToString().Trim().Length <= 15)//* mandatory
                { }
                else
                {
                    val_status = false;
                    listBox1.Items.Add("billing_GSTIN= " + row["billing_GSTIN"].ToString().Trim());
                }
                if (row["billing_POS"].ToString().Length >= 1 && row["billing_POS"].ToString().Length <= 2)//* mandatory
                { }
                else
                {
                    val_status = false;
                    listBox1.Items.Add("billing_POS= " + row["billing_POS"].ToString());
                }
                if (row["billing_City"].ToString().Length >= 3 && row["billing_City"].ToString().Length <= 100)//* mandatory
                { }
                else
                {
                    val_status = false;
                    listBox1.Items.Add("billing_City= " + row["billing_City"].ToString());
                }
                if (row["billing_State"].ToString().Length >= 1 && row["billing_State"].ToString().Length <= 2)//* mandatory
                { }
                else
                {
                    val_status = false;
                    listBox1.Items.Add("billing_State= " + row["billing_State"].ToString());
                }
                if (row["billing_Address1"].ToString().Length >= 3 && row["billing_Address1"].ToString().Length <= 100)//* mandatory
                { }
                else
                {
                    val_status = false;
                    listBox1.Items.Add("billing_Address1= " + row["billing_Address1"].ToString());
                }
                if (row["billing_Pincode"].ToString().Replace(" ", "").Length == 6)//* mandatory
                { }
                else
                {
                    val_status = false;
                    listBox1.Items.Add("billing_Pincode= " + row["billing_Pincode"].ToString().Replace(" ", ""));
                }

                //document_Total
                if (Convert.ToDouble(row["total_assVal"].ToString()) <= 99999999999999.99)//* mandatory
                { }
                else
                {
                    val_status = false;
                    listBox1.Items.Add("total_assVal= " + row["total_assVal"].ToString());
                }
                //if (row["roundoff"].ToString().Length == 15)//* mandatory
                //{
                //    errList.Add("roundoff", row["roundoff"].ToString());
                //}
                if (Convert.ToDouble(row["total_Invoice_Value"].ToString()) <= 99999999999999.99)//* mandatory
                { }
                else
                {
                    val_status = false;
                    listBox1.Items.Add("total_Invoice_Value= " + row["total_Invoice_Value"].ToString());
                }

                //if (row["cgstvalue"].ToString().Length == 15)//optional
                //{
                //}
                //if (row["sgstvalue"].ToString().Length == 15)//optional
                //{
                //}
                //if (row["igstvalue"].ToString().Length == 15)//optional
                //{
                //}
                //if (row["cessvalue"].ToString().Length == 15)//optional
                //{
                //}
                //if (row["stateCessValue"].ToString().Length == 15)//optional
                //{
                //}
                //if (row["Discount"].ToString().Length == 15)//optional
                //{
                //}
                //if (row["OthChrg"].ToString().Length == 15)//optional
                //{
                //}
                //if (row["val_for_cur"].ToString().Length == 15)//optional
                //{
                //}

                //items           
                if (row["slno"].ToString().Length >= 1 && row["slno"].ToString().Length <= 6)//* mandatory
                { }
                else
                {
                    val_status = false;
                    listBox1.Items.Add("slno= " + row["slno"].ToString());
                }
                if (row["service"].ToString().Length == 1)//* mandatory
                { }
                else
                {
                    val_status = false;
                    listBox1.Items.Add("service= " + row["service"].ToString());
                }
                if (row["hsn_code"].ToString().Trim().Length >= 6 && row["hsn_code"].ToString().Trim().Length <= 8)//* mandatory
                { }
                else
                {
                    val_status = false;
                    listBox1.Items.Add("hsn_code= " + row["hsn_code"].ToString().Trim());
                }
                if (Convert.ToDouble(row["rate"].ToString()) <= 999999999999.99)//* mandatory
                { }
                else
                {
                    val_status = false;
                    listBox1.Items.Add("rate= " + row["rate"].ToString());
                }
                if (Convert.ToDouble(row["grossAmount"].ToString()) <= 999999999999.99)//* mandatory
                { }
                else
                {
                    val_status = false;
                    listBox1.Items.Add("grossAmount= " + row["grossAmount"].ToString());
                }
                if (Convert.ToDouble(row["assesseebleValue"].ToString()) <= 999999999999.99)//* mandatory
                { }
                else
                {
                    val_status = false;
                    listBox1.Items.Add("assesseebleValue= " + row["assesseebleValue"].ToString());
                }
                if (Convert.ToDouble(row["igst_rt"].ToString()) <= 999.999)//* mandatory
                { }
                else
                {
                    val_status = false;
                    listBox1.Items.Add("igst_rt= " + row["igst_rt"].ToString());
                }
                if (Convert.ToDouble(row["cgst_rt"].ToString()) <= 999.999)//* mandatory
                { }
                else
                {
                    val_status = false;
                    listBox1.Items.Add("cgst_rt= " + row["cgst_rt"].ToString());
                }
                if (Convert.ToDouble(row["sgst_rt"].ToString()) <= 999.999)//* mandatory
                {
                }
                else
                {
                    val_status = false;
                    listBox1.Items.Add("sgst_rt= " + row["sgst_rt"].ToString());
                }
                if (Convert.ToDouble(row["cess_rt"].ToString()) <= 999.999)//* mandatory
                { }
                else
                {
                    val_status = false;
                    listBox1.Items.Add("cess_rt= " + row["cess_rt"].ToString());
                }
                //if (row["otherCharges"].ToString().Length == 15)//* mandatory
                //{
                //    errList.Add("otherCharges", row["otherCharges"].ToString());
                //}
                if (Convert.ToDouble(row["itemTotal"].ToString()) <= 999999999999.99)//* mandatory
                { }
                else
                {
                    val_status = false;
                    listBox1.Items.Add("itemTotal= " + row["itemTotal"].ToString());
                }
            }
            return val_status;
        }
        
        public string checkGtnAvl(string invn)
        {

            DataSet dt;
            string connectionstring = "Data Source=" + SERVERNAME + "; Initial Catalog=" + SAGEDB + "; User ID=" + SAA + "; Password=" + SAPSS + ";";
            //MessageBox.Show(connectionstring);
            //constr = "Provider=SQLOLEDB;Data Source=ERP-DATABASE; Initial Catalog=TSTDAT;User ID=sa; Password=Vspl@4321"
            //string connectionstring = "Data Source=ERP-DATABASE; Initial Catalog=TSTDAT; User ID=sa; Password=Vspl@4321;";
            System.Data.SqlClient.SqlConnection conn;
            System.Data.SqlClient.SqlCommand cmd;

            conn = new System.Data.SqlClient.SqlConnection(connectionstring);
            conn.Open();
            //string Querystring = "select value from arcuso where optfield='gstin' and idcust='"+custno +"'";

            string Querystring = "select ar.value from arcuso ar	inner join oeinvh h on h.CUSTOMER=ar.IDCUST where ar.optfield='gstin' and h.invnumber='" + invn + "'";
            cmd = new System.Data.SqlClient.SqlCommand(Querystring, conn);
            cmd.CommandTimeout = 180;
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
            using (System.Data.SqlClient.SqlDataAdapter sda = new System.Data.SqlClient.SqlDataAdapter())
            {
                cmd.Connection = conn;
                sda.SelectCommand = cmd;
                using (dt = new DataSet())
                {
                    sda.Fill(dt);
                    String ss = dt.Tables[0].Rows[0]["value"].ToString().Trim();
                    if (string.IsNullOrEmpty(ss))
                        return "FALSE";
                    int lntg = ss.Length;
                    if (lntg >= 3 & lntg <= 15)
                    {
                        return "TRUE";
                    }
                    else
                        return "FALSE";
                }
            }
        }
        public void CreateJsonString(DataTable crJsonTb)
        {
            
			if (crJsonTb.Rows.Count > 0)
			{
				int HeadeIndx = 0;
				csInvPost objPt = new csInvPost();
				inv objinv = new inv();
				string hsncode = HSNCODE2;
				foreach (DataRow dr in crJsonTb.Rows)
				{
					if (HeadeIndx == 0)
					{
						string bvv = "";
						if (SGSTIN == "EMPTY")
							bvv = dr["supplier_GSTIN"].ToString().Trim();
						else bvv = SGSTIN;
						string sts = "";
						if (STATE == "EMPTY")
							sts = dr["supplier_State"].ToString().Trim();
						else sts = STATE;
						string pncd = "";
						if (PINCODE == "EMPTY")
							pncd = dr["supplier_Pincode"].ToString().Replace(" ", "");
						else pncd = PINCODE;
						objPt.supplier_GSTIN = bvv;// "29AAFPB7029M000";//dr["supplier_GSTIN"].ToString();
						objPt.supplier_Legal_Name = dr["supplier_Legal_Name"].ToString().Trim();
						// objPt.supplier_trading_name = "VSPL";
						objPt.supplier_City = dr["supplier_City"].ToString().Trim();
						objPt.supplier_Address1 = dr["supplier_Address1"].ToString().Trim();
						objPt.supplier_Address2 = dr["supplier_Address2"].ToString().Trim();
						objPt.supplier_State = sts; //"29";//dr["supplier_State"].ToString();
						objPt.supplier_Pincode = pncd;// "560087";// dr["supplier_Pincode"].ToString();
													  // objPt.supplier_Phone = "9810885187";
													  //  objPt.supplier_Email = "supplier@gmail.com";
						HeadeIndx++;

						if (dr["invoice_type_code"].ToString().Trim() == "SEZWO" || dr["invoice_type_code"].ToString().Trim() == "EXPWO")
						{
							invoice_type_code_B2C = dr["invoice_type_code"].ToString().Trim() + "P";
						}
						else
						{ invoice_type_code_B2C = dr["invoice_type_code"].ToString().Trim(); }
						//string transmode;
						//if (tabname == "CREDITNODE")
						//    transmode = cmbCRDTransMode.Text;
						//else transmode = cmbTransMd.Text;

						var transaction = new
						{
							transactionMode = cmbTransMd.Text,// dr["transactionMode"].ToString().Trim(),
							invoice_type_code = invoice_type_code_B2C,
							reversecharge = dr["reversecharge"].ToString().Trim()
							//ecom_GSTIN = "",
							// IgstOnIntra = "N"
						};
						objinv.transaction_details = transaction;

						string binv = "";
						if (INVNUM == "EMPTY")
							binv = dr["invoiceNum"].ToString();
						else binv = INVNUM;

						var document = new
						{
							invoice_subtype_code = dr["invoice_subtype_code"].ToString().Trim(),
							invoiceNum = binv,
							invoiceDate = dr["invoiceDate"].ToString().Trim()
							// transation_id = "1234",
							// plant = "",  //Mendatory field
							// custom = "123"
						};
						objinv.document_details = document;

						//var export = new
						//{
						//    shipping_bill_no = "1234567",
						//    shipping_bill_date = "02-01-2018",
						//    port_code = "",
						//    invoice_currency_code = "INR",
						//    cnt_code = "IN",
						//    refClm = "N",
						//    ExpDuty = 1
						//};
						//objinv.export_details = export;

						//var extra = new
						//{
						//    invoice_Period_Start_Date = "10-03-2019",
						//    invoice_Period_End_Date = "11-02-2019",
						//  preceeding_Invoice_Number = dr["preceeding_Invoice_Number"].ToString(),//"INV002",
						// preceeding_Invoice_Date = dr["preceeding_Invoice_Date"].ToString(),//"20-09-2019"
						//    invoice_Document_Reference = "KOL01",
						//    receipt_Advice_ReferenceNo = "CREDIT30",
						//    receipt_Advice_ReferenceDt = "20-09-2019",
						//    tender_or_Lot_Reference = "AALPS",
						//    contract_Reference = "CONT23072019",
						//    external_Reference = "EXT23222",
						//    project_Reference = "PJTCODE01",
						//    refNum = "Vendor PO /1",
						//    refDate = "30-01-2020",
						//    remarks = "Invoice remarks",
						//    Url = "http://www.xyz.com/abc",
						//    Docs = "",
						//    Info = "3333"

						// };
						// objinv.extra_Information = extra;

						string bGst = "";
						if (invoice_type_code_B2C != "B2C")
						{
							if (STATE == "EMPTY")
								bGst = dr["billing_GSTIN"].ToString();
							else bGst = BGSTIN;
						}
						else
						{ bGst = "URP"; }
						string bstate = "";
						if (STATE == "EMPTY")
							bstate = dr["billing_State"].ToString();
						else bstate = STATE;

						string bpin = "";
						if (PINCODE == "EMPTY")
							bpin = dr["billing_Pincode"].ToString().Replace(" ", "");
						else bpin = PINCODE;

						var billing = new
						{
							billing_Name = dr["billing_Name"].ToString().Trim(),
							// billing_trade_name = "ADA",
							billing_GSTIN = bGst,//"29AABCS0858G1Z9",//dr["billing_GSTIN"].ToString(),
							billing_POS = dr["billing_POS"].ToString().Trim(),
							billing_City = dr["billing_City"].ToString().Trim(),
							billing_State = bstate,//"29",//dr["billing_State"].ToString(),
							billing_Address1 = dr["billing_Address1"].ToString().Trim(),
							billing_Address2 = dr["billing_Address2"].ToString().Trim(),
							billing_Pincode = bpin.Trim() //"560087",// dr["billing_Pincode"].ToString()//"560087"
														  // billing_Phone = "9999999999",
														  //billing_Email = "billing@go4gst.com"
						};
						objinv.billing_Information = billing;
						if (cmbTransMd.Text.ToString() == "SHP" || cmbTransMd.Text.ToString() == "CMB")
						{
							// Querystring += " ,H.SHPNAME,H.SHIPTO,H.SHPADDR1,H.SHPADDR2,H.SHPCITY,H.SHPCOUNTRY,H.SHPSTATE,H.SHPZIP ";
							var shipping = new
							{
								shippingTo_Name = dr["SHPNAME"].ToString().Trim(),
								//shippingTo_trade_name = dr["SHIPTO"].ToString().Trim(),
								// shippingTo_GSTIN = "",
								shippingTo_Place = dr["SHPCITY"].ToString().Trim(),
								shippingTo_State = bstate,
								shippingTo_Address1 = dr["SHPADDR1"].ToString().Trim(),
								shippingTo_Address2 = dr["SHPADDR2"].ToString().Trim(),
								shippingTo_Pincode = dr["SHPZIP"].ToString().Trim(),

							};
							objinv.shipping_Information = shipping;
						}
						//delivery_Information/Dispatch from information
						//if (cmbTransMd.Text.ToString() != "REG")
						//{
						//	//MessageBox.Show(dr["location_code"].ToString());
						//	//  string gg = "";
						//	if (dr["location_code"].ToString().Substring(3, 2) == "13")
						//	{
						//		var delivery = new
						//		{
						//			company_Name = "VSPL",
						//			city = "Gurgaon",
						//			state = "06",
						//			address1 = "M22, Old DLF,Sector-14",
						//			address2 = "",
						//			pincode = "122002"
						//		};
						//		objinv.delivery_Information = delivery;
						//	}
						//	else if (dr["location_code"].ToString().Substring(3, 2) == "15")
						//	{
						//		var delivery = new
						//		{
						//			company_Name = "VSPL",
						//			city = "Noida",
						//			state = "09",
						//			address1 = "C/O Pro-Connect Supply Chain Solutions Ltd,Khasra No.357",
						//			address2 = ",Village Chhajarsi,Sec.-63",
						//			pincode = "201301"

						//		};
						//		objinv.delivery_Information = delivery;
						//	}
						//	else if (dr["location_code"].ToString().Substring(3, 2) == "30")
						//	{
						//		var delivery = new
						//		{
						//			company_Name = "VSPL",
						//			city = "Bangalore",
						//			state = "29",
						//			address1 = "No.46/2,Ramesh Reddy layout,Behind Nandi Toyota Showroom",
						//			address2 = ",Garvebhavipalya, Hosur Road",
						//			pincode = "560068"

						//		};
						//		objinv.delivery_Information = delivery;
						//	}
						//	else if (dr["location_code"].ToString().Substring(3, 2) == "40")
						//	{
						//		var delivery = new
						//		{
						//			company_Name = "VSPL",
						//			city = "Mumbai",
						//			state = "27",
						//			address1 = "Gala No. 26,1st Floor, Sainath Industrial EstateVishweshwar Road,",
						//			address2 = " Goregaon (East)",
						//			pincode = "400063"

						//		};
						//		objinv.delivery_Information = delivery;
						//	}
						//	else if (dr["location_code"].ToString().Substring(3, 2) == "45")
						//	{

						//		var delivery = new
						//		{
						//			company_Name = "VSPL",
						//			city = "Mumbai",
						//			state = "27",
						//			address1 = "Gala No. 26,1st Floor, Sainath Industrial EstateVishweshwar Road, Goregaon (East)",
						//			address2 = "",
						//			pincode = "400063"

						//		};
						//		objinv.delivery_Information = delivery;
						//	}
						//	else if (dr["location_code"].ToString().Substring(3, 2) == "61")
						//	{

						//		var delivery = new
						//		{
						//			company_Name = "VSPL",
						//			city = "Tamilnadu",
						//			state = "33",
						//			address1 = "C/O DHL Logistics Pvt Ltd,J.Matadee FTZ Pvt Ltd, Survey no 434/A5,434/A6,435/2,437/A",
						//			address2 = ", Mannur Village,Sriperumbudur Taluk,Kanchipuram Dist",
						//			pincode = "602105"

						//		};
						//		objinv.delivery_Information = delivery;
						//	}
						//	else if (dr["location_code"].ToString().Substring(3, 2) == "65")
						//	{
						//		var delivery1 = new
						//		{
						//			company_Name = "VSPL",
						//			city = "Tiruvallur",
						//			state = "33",
						//			address1 = "C/O Pro-Connect Supply Chain Solutions Ltd 79,Kurathanamedu,Panpakkam Village",
						//			address2 = ", Gummudipundi Taluk",
						//			pincode = "602106"

						//		};
						//		objinv.delivery_Information = delivery1;
						//	}
						//	else
						//	{
						//		//var delivery = new
						//		//{
						//		//    company_Name = "VSPL",
						//		//    city = "Tamilnadu",
						//		//    state = "33",
						//		//    address1 = "C/O DHL Logistics Pvt Ltd,J.Matadee FTZ Pvt Ltd, Survey no 434/A5,434/A6,435/2,437/A,",
						//		//    address2 = " Mannur Village,Sriperumbudur Taluk,Kanchipuram Dist",
						//		//    pincode = "602105"

						//		//};
						//		//objinv.delivery_Information = delivery;
						//	}
						//}
						//var payee = new
						//{
						//    payee_Name = "Pay name",
						//    payer_Financial_Account = "01123421401",
						//    modeofPayment = "Cash",
						//    financial_Institution_Branch = "SBIN0021882",
						//    payment_Terms = "PTO",
						//    payment_Instruction = "PIO",
						//    credit_Transfer = "Done",
						//    direct_Debit = "Direct",
						//    creditDays = 3,
						//    paid_amount = 1800,
						//    amount_due_for_payment = 400

						//};
						//objinv.payee_Information = payee;


						//var ewaybill = new
						//{
						//    ewb_transporter_id = "05AAACH1004N1Z0",
						//    ewb_transMode = "1",
						//    ewb_transDistance = 100,
						//    ewb_transporterName = "VSPL Trans",
						//    ewb_transDocNo = "TEST01",
						//    ewb_transDocDt = "14-08-2020",
						//    ewb_vehicleNo = "UP86C2345",
						//    ewb_subSupplyType = "1",
						//    ewb_vehicleType = "O"

						//};
						//objinv.ewaybill_information = ewaybill;
						//stateCessValue = Convert.ToInt16(dr["stateCessValue"].ToString()),
						// roundoff = 0.2,
						string HDISCOUNT = dr["Discount"].ToString();
						if (HDISCOUNT == "")
							HDISCOUNT = "0";
						var documentTotal = new
						{
							total_assVal = Math.Round(Convert.ToDecimal(dr["total_assVal"].ToString()), 2),
							total_Invoice_Value = Math.Round(Convert.ToDecimal(dr["total_Invoice_Value"].ToString()), 2),
							// total_Invoice_Value = dr["total_Invoice_Value"].ToString(),
							igstvalue = Math.Round(Convert.ToDouble(dr["igstvalue"].ToString()), 2),
							cgstvalue = Math.Round(Convert.ToDouble(dr["cgstvalue"].ToString()), 2),
							sgstvalue = Math.Round(Convert.ToDouble(dr["sgstvalue"].ToString()), 2),
							cessvalue = Math.Round(Convert.ToDouble(dr["cessvalue"].ToString()), 2),

							// val_for_cur = 0,
							Discount = Math.Round(Convert.ToDouble(HDISCOUNT), 2)
							// OthChrg = Math.Round(Convert.ToDouble(dr["OthChrg"].ToString()), 2)

						};
						objinv.document_Total = documentTotal;
						objinv.items = new List<items>();
						hsncode = HSNCODE1;
					}

					// item .......................................

					items objitem = new items();
					objitem.slno = Convert.ToInt16(dr["slno"].ToString());
					// objitem.item_Description = "Mobile";
					// if (hsncode == HSNCODE1)  //8471
					objitem.service = dr["service"].ToString().Trim();
					// else
					//objitem.service = "Y";
					if (hsncode != "EMPTY")
						objitem.hsn_code = hsncode;// dr["hsn_code"].ToString().Trim();
					else objitem.hsn_code = dr["hsn_code"].ToString().Trim();
					//var batchstr = new
					//{
					//    batchName = "PQR",
					//    batchExpiry_Date = "30-12-2019",
					//    warrantyDate = "20-11-2020"
					//};
					//objitem.batch = batchstr;
					//var attribDtlsSR = new
					//{
					//    attrib_name = "PQR",
					//    attrib_val = "12345"
					//};
					//objitem.attribDtls = attribDtlsSR;
					//objitem.barcode = "b123";
					objitem.quantity = Convert.ToInt32(dr["quantity"]);
					//objitem.freeQty = 0;
					objitem.uqc = dr["uqc"].ToString();
					objitem.rate = Math.Round(Convert.ToDecimal(dr["rate"].ToString()), 2);
					objitem.grossAmount = Math.Round(Convert.ToDecimal(dr["grossAmount"].ToString()), 2);
					objitem.discountAmount = Math.Round(Convert.ToDecimal(dr["discountAmount"].ToString()), 2);
					//objitem.preTaxAmount = Convert.ToInt16(dr["slno"].ToString());
					objitem.assesseebleValue = Math.Round(Convert.ToDecimal(dr["assesseebleValue"].ToString()), 2);
					objitem.igst_rt = Math.Round(Convert.ToDecimal(dr["igst_rt"].ToString()), 3);
					objitem.cgst_rt = Convert.ToDecimal(dr["cgst_rt"].ToString());
					objitem.sgst_rt = Convert.ToDecimal(dr["sgst_rt"].ToString());
					objitem.sgst_rt = Convert.ToDecimal(dr["sgst_rt"].ToString());
					objitem.iamt = Math.Round(Convert.ToDecimal(dr["iamt"].ToString()), 2);
					objitem.camt = Math.Round(Convert.ToDecimal(dr["camt"].ToString()), 2);
					objitem.samt = Math.Round(Convert.ToDecimal(dr["samt"].ToString()), 2);
					objitem.csamt = Math.Round(Convert.ToDecimal(dr["csamt"].ToString()), 2);
					//objitem.cessnonadval = 0;
					//objitem.state_cess = 0;
					//objitem.stateCessAmt = 0;
					//objitem.stateCesNonAdvlAmt = 0;
					objitem.otherCharges = Math.Round(Convert.ToDecimal(dr["otherCharges"].ToString()), 2);
					objitem.itemTotal = Math.Round(Convert.ToDecimal(dr["itemTotal"].ToString()), 2);
					//objitem.ordLineRef = "11";
					objitem.origin_Country = "IN";
					//objitem.prdSlNo = "";
					objinv.items.Add(objitem);
					hsncode = HSNCODE2;
				}
				objPt.inv = new[] { objinv };
				string json = JsonConvert.SerializeObject(objPt);
				// string strjson= json.Replace("extra_Information: null,", "");
				dynamic Response = "";
				if (invoice_type_code_B2C == "B2C")
				{
					//    Response = POSTData(objPt, "https://aspstaging.go4gst.com/GO4GST/rest/eInvWebService/eInvERP/VELOCEL?type=import");
				//	Response = POSTData(objPt, "https://velocis.go4gst.com/GO4GST_EINV/rest/eInvWebService/eInvERP/velocis_db?type=import");
				}
				else
				{
					Response = POSTData(objPt, APIURL + apitype);

				}
				//dynamic Response = POSTData(objPt, "https://velocis.go4gst.com/GO4GST_EINV/rest/eInvWebService/eInvERP/velocis_db?type=generate");

				//dynamic Response = POSTData(objPt, APIURL + apitype + "");
				dynamic deserialized = null;
				//if (Response.ToString() == "")
				//{
				//    listBox1.Items.Add("API Response Error!!");
				//    return;
				//}
				if (Response != null)
				{
					listBox1.Items.Add("API Response success!");
					deserialized = JsonConvert.DeserializeObject(Response.ToString());
				}
				else { listBox1.Items.Add("Waiting for API Response Failed....."); return; }
				webBrowser1.DocumentText = json + "Repsponse" + JsonConvert.DeserializeObject(Response.ToString());
				//dynamic deserialized = JsonConvert.DeserializeObject(Response.ToString());
				//webBrowser1.DocumentText = json+ "Response=>"+ deserialized.ToString();
				// webBrowser2.DocumentText = deserialized.ToString();
				
				
				
					string statusheader = deserialized.STATUS;
					string statuscode = "";
				if (invoice_type_code_B2C != "B2C")
				{
					statuscode = deserialized.STATUS_CODE;
				}
				var strresponse = deserialized.RESPONSE;
					if (statusheader == "SUCCESS_IMPORT")
					{
						listBox1.Items.Add(strresponse.ToString());
					}
					else if (statusheader == "ERROR_IN_GENERATE")
					{

						csQRCode objQr = new csQRCode();
						csCreateInvPdf objinvoce = new csCreateInvPdf();
						var ErrorDetails = strresponse["ErrorDetails"];
						dynamic des = JsonConvert.DeserializeObject(ErrorDetails.ToString());

						string ErrorCode = des[0].ErrorCode.ToString();
						//var InfoDtls = strresponse["InfoDtls"];
						if (ErrorCode == "2150")
						{
							try
							{
								var InfoDtls = strresponse["InfoDtls"];
								var Desc = InfoDtls[0].Desc;
								var irn = Desc["Irn"];
								var QrCode = Desc["SignedQRCode"];
								//var irn = Desc.Irn;
								string Errormsg = des[0].ErrorMessage.ToString();
								///string Irn = des[0].Irn.ToString();
								//Environment.NewLine

								var confirmResult = MessageBox.Show(Errormsg + ", If you want to creating again then click Yes!", "", MessageBoxButtons.YesNo);
								if (confirmResult == DialogResult.Yes)
								{
									listBox1.Items.Add("Please wait, Creating QR thumb in process..");
									Boolean djjj = objQr.createQrImage(irn.ToString(), cmbxInvoice.SelectedValue.ToString().Trim(), QrCode.ToString());
									//  Boolean jjj = objinvoce.createinv(cmbxInvoice.SelectedValue.ToString().Trim(), irn.ToString(), cmbreport.Text, cmbtype.Text);
									listBox1.Items.Add(" Created QR thumb!");
								}
								else
								{
									// If 'No', do something here.
								}
							}
							catch (Exception)
							{ throw; }
						}

						if (ErrorCode == "2176")
						{

							foreach (var i in ErrorDetails)
							{
								listBox1.Items.Add("Error Code:>" + i.ErrorCode.ToString() + " Error Message :>  " + i.ErrorMessage.ToString());
							}
						}
					}
					else if (statusheader == "ERROR_IN_IMPORT")         //import controll response
					{
						csQRCode objQr = new csQRCode();
						csCreateInvPdf objinvoce = new csCreateInvPdf();
						var ErrorDetails = strresponse["ErrorDetails"];
						dynamic des = JsonConvert.DeserializeObject(ErrorDetails.ToString());
						string ErrorCode = des[0].ErrorCode.ToString();
						foreach (var i in ErrorDetails)
						{
							listBox1.Items.Add("Error Code:>" + i.ErrorCode.ToString() + " Error Message :>  " + i.ErrorMessage.ToString());
						}
						if (ErrorCode == "ASP-4007")
						{
							try
							{
								var InfoDtls = strresponse["InfoDtls"];
								var Desc = InfoDtls[0].Desc;
								var irn = Desc["Irn"];
								var QrCode = Desc["SignedQRCode"];
								//var irn = Desc.Irn;
								string Errormsg = des[0].ErrorMessage.ToString();
								///string Irn = des[0].Irn.ToString();
								//Environment.NewLine
								var confirmResult = MessageBox.Show(Errormsg + ", If you want to creating again then click Yes!", "", MessageBoxButtons.YesNo);

								if (confirmResult == DialogResult.Yes)
								{
									listBox1.Items.Add("Please wait, Creating QR thumb in process..");
									Boolean djjj = objQr.createQrImage(irn.ToString(), cmbxInvoice.SelectedValue.ToString().Trim(), QrCode.ToString());
									listBox1.Items.Add(" Created QR thumb!");
									// Boolean jjj = objinvoce.createinv(cmbxInvoice.SelectedValue.ToString().Trim(), irn.ToString(), cmbreport.Text, cmbtype.Text);
								}
								else
								{
									// If 'No', do something here.
								}
							}
							catch (Exception)
							{ throw; }
						}
					}
					else if (statusheader == "ERROR")
					{
						if (invoice_type_code_B2C == "B2C")
						{
							listBox1.Items.Add(strresponse.UPI_QR_code);
							return;
						}
						try
						{
							var ErrorDetails = strresponse["ErrorDetails"];
							dynamic des = JsonConvert.DeserializeObject(ErrorDetails.ToString());
							string Errormsg = des[0].ErrorMessage.ToString();
							string ErrorCode = des[0].ErrorCode.ToString();
							string invNum = deserialized.invoiceNum;
							ErrorXmlList(invNum, ErrorCode, Errormsg);
							foreach (var i in ErrorDetails)
							{
								listBox1.Items.Add("Error Code:>" + i.ErrorCode.ToString() + " Error Message :>  " + i.ErrorMessage.ToString());
							}
							//listBox1.Items.Add("ErrorCode="+ErrorCode + "  Errormsg=" + Errormsg);
						}
						catch (Exception ex)
						{
							listBox1.Items.Add("statusheader == ERROR" + ex.Message.ToString());
						}

					}

					else if (statusheader == "SUCCESS")
					{
					if (invoice_type_code_B2C == "B2C")
					{
						try
						{
							//string statusheader = deserialized.STATUS;
							//string statuscode = deserialized.STATUS_CODE;
							//MessageBox.Show(strresponse.ToString());
							//var strresponse = deserialized.RESPONSE;
							
							//MessageBox.Show(strresponse["UPI_QR_Code"].ToString());
							var qrcd = strresponse["UPI_QR_Code"];
							string sirndt = DateTime.Now.ToString("dd/MM/yyyy");
							string sirn = "";
							string EwbNo = "";
							string EwbDt = "";
							string EwbValidTill = "";
							string sinv = deserialized.invoiceNum;
							listBox1.Items.Add("Please wait, QR Thumb creating in process.........");
							csCreateInvPdf objinvoce = new csCreateInvPdf();
							csQRCode objQr = new csQRCode();//UPI_QR_Code
							if (objinvoce.SaveIRNNOByInvNo(cmbxInvoice.SelectedValue.ToString(), comboBox1.Text, "B2C", sirndt, EwbNo, EwbDt, EwbValidTill, qrcd.ToString(), Datatype) == "TRUE")
							{
								listBox1.Items.Add("B2C Document Processed Successfully.");
								MessageBox.Show("B2C Document Processed Successfully.");
							}

							if (objQr.createQrImageBS64(sirn.ToString(), sinv.ToString(), qrcd.ToString()) == true)
							{
								listBox1.Items.Add("QR Thumb created!");
							}

						}
						catch (Exception )
						{ throw; }
					}
					else
						{
						try
						{
							csCreateInvPdf objinvoce = new csCreateInvPdf();
							csQRCode objQr = new csQRCode();
							string status = strresponse["Status"].ToString();
							string Irn = strresponse["Irn"].ToString();
							var rep = strresponse["SignedQRCode_Decrypted"];
							var dd = rep.data;
							var sirn = dd.Irn.ToString();
							var sinv = dd.DocNo.ToString();
							string qrcd = strresponse["SignedQRCode"].ToString();
							//var qrcd = dd.SignedQRCode.ToString();
							var sirndt = dd.IrnDt.ToString();
							string EwbNo = "";
							string EwbDt = "";
							string EwbValidTill = "";
							if (chkEwayBill.Checked == true)
							{
								EwbNo = ""; //gov_response.EwbNo;
								EwbDt = ""; //gov_response.EwbDt;
								EwbValidTill = "";// gov_response.EwbValidTill;
								listBox1.Items.Add("EwayBill NO:" + EwbNo);
								listBox1.Items.Add("EwbDt:" + EwbDt);
								txtEwaybillNo.Text = EwbNo;
								txtEWayDate.Text = EwbDt;
								txtEwatValTo.Text = EwbValidTill;
							}
							listBox1.Items.Add("Please wait, QR Thumb creating in process.........");

							if (objinvoce.SaveIRNNOByInvNo(cmbxInvoice.SelectedValue.ToString(), comboBox1.Text, sirn.ToString(), sirndt, EwbNo, EwbDt, EwbValidTill, qrcd.ToString(), Datatype) == "TRUE")
							{
								listBox1.Items.Add("IRN and Ewaybill Processed Successfully.");
								MessageBox.Show("IRN and Ewaybill Processed Successfully.");
							}

							if (objQr.createQrImage(sirn.ToString(), sinv.ToString(), qrcd.ToString()) == true)
							{
								listBox1.Items.Add("QR Thumb created!");
							}

						}
						catch (Exception)
						{ throw; }
						 }
					}
					else { return; }
				
			}
			else { MessageBox.Show("Data Empty!!!"); }
        }
        public void CredentialsXml()
        {
            if (!System.IO.File.Exists("YMCDetCRD.xml"))
            {
                XmlTextWriter writer = new XmlTextWriter(@"YMCDetCRD.xml", System.Text.Encoding.UTF8);
                writer.WriteStartDocument(false);
                writer.Formatting = System.Xml.Formatting.Indented;
                writer.Indentation = 2;
                writer.WriteStartElement("dbconfig");
                writer.WriteStartElement("SERVERNAME");
                writer.WriteString(".");
                writer.WriteEndElement();
                writer.WriteStartElement("USERNAME");
                writer.WriteString("ADMIN");
                writer.WriteEndElement();
                writer.WriteStartElement("PASSWORD");
                writer.WriteString("ADMIN");
                writer.WriteEndElement();
                writer.WriteStartElement("SAGEDB");
                writer.WriteString("GSTMAS");
                writer.WriteEndElement();
                writer.WriteStartElement("SAA");
                writer.WriteString("sa");
                writer.WriteEndElement();
                writer.WriteStartElement("SAPSS");
                writer.WriteString("Erp#12345");
                writer.WriteEndElement();
                writer.WriteStartElement("SGSTIN");
                writer.WriteString("EMPTY");
                writer.WriteEndElement();
                writer.WriteStartElement("BGSTIN");
                writer.WriteString("EMPTY");
                writer.WriteEndElement();
                writer.WriteStartElement("HSNCODE1");
                writer.WriteString("EMPTY");
                writer.WriteEndElement();
                writer.WriteStartElement("HSNCODE2");
                writer.WriteString("995411");
                writer.WriteEndElement();
                writer.WriteStartElement("STATE");
                writer.WriteString("EMPTY");
                writer.WriteEndElement();
                writer.WriteStartElement("PINCODE");
                writer.WriteString("EMPTY");
                writer.WriteEndElement();
                writer.WriteStartElement("INVNUM");
                writer.WriteString("EMPTY");
                writer.WriteEndElement();
                writer.WriteStartElement("APIURL");
                writer.WriteString("https://velocis.go4gst.com/rest/eInvWebService/eInvERP/velocis_db?type=");
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();

            }

            ReadWriteXML xml1 = new ReadWriteXML();
            bool conStatus = xml1.ReadXML();
            if (conStatus == true)
            {
                SERVERNAME = xml1.SERVERNAME;
                USERNAME = xml1.USERNAME;
                PASSWORD = xml1.PASSWORD;
                SAGEDB = xml1.SAGEDB;
                SAA = xml1.SAA;
                SAPSS = xml1.SAPSS;
                SGSTIN = xml1.SGSTIN;
                BGSTIN = xml1.BGSTIN;
                HSNCODE1 = xml1.HSNCODE1;
                HSNCODE2 = xml1.HSNCODE2;
                STATE = xml1.STATE;
                PINCODE = xml1.PINCODE;
                INVNUM = xml1.INVNUM;
                APIURL = xml1.APIURL;
            }
        }
        public void ErrorXmlList(string inv, string errcode, string errdetail)
        {
            if (!System.IO.File.Exists("ErrorList.xml"))
            {
                XElement element = new XElement("QRCODE");
                element.Save("ErrorList.xml");
            }
            XmlDocument xmlEmloyeeDoc = new XmlDocument();
            xmlEmloyeeDoc.Load("ErrorList.xml");
            XmlElement ParentElement = xmlEmloyeeDoc.CreateElement("QRCODE");
            XmlElement ID = xmlEmloyeeDoc.CreateElement("InvNumber");
            ID.InnerText = inv;
            XmlElement Name = xmlEmloyeeDoc.CreateElement("ErrorCode");
            Name.InnerText = errcode;
            XmlElement Designation = xmlEmloyeeDoc.CreateElement("ErrorMessage");
            Designation.InnerText = errdetail;
            ParentElement.AppendChild(ID);
            ParentElement.AppendChild(Name);
            ParentElement.AppendChild(Designation);
            xmlEmloyeeDoc.DocumentElement.AppendChild(ParentElement);
            xmlEmloyeeDoc.Save("ErrorList.xml");
        }
        public object POSTData(object json, string url)
        {
            object returnValue = null;
            try
            {
                using (var content = new StringContent(JsonConvert.SerializeObject(json), System.Text.Encoding.UTF8, "application/json"))
                {
                    using (var httpClientHandler = new HttpClientHandler { Credentials = new NetworkCredential("GO4GST", "GO4GST#1234") })

                    //using (var httpClientHandler = new HttpClientHandler { Credentials = new NetworkCredential("sch9650", "09102010") })
                    using (var _httpClient = new HttpClient(httpClientHandler))
                    {
                        _httpClient.BaseAddress = new Uri(url);
                        _httpClient.DefaultRequestHeaders.Add("Authorization", "Basic R080R1NUOkdPNEdTVCMxMjM0");
                        _httpClient.DefaultRequestHeaders.Add("Cookie", "JSESSIONID=940BDAB1D8EF9A30B8657CA25A610E18; AWSALB=rgrhwx17vz3cDbea785P7grZ9/4VBQ4YysHkxaqJKGqBzHphfapEOdproZcjZfSzMK59qj/l4YhSaY9niC1/cTKLWmhvHQZYkpkEZuu7t5drJ4oYpzSjrD5Del+b; AWSALBCORS=rgrhwx17vz3cDbea785P7grZ9/4VBQ4YysHkxaqJKGqBzHphfapEOdproZcjZfSzMK59qj/l4YhSaY9niC1/cTKLWmhvHQZYkpkEZuu7t5drJ4oYpzSjrD5Del+b");
                        _httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
                        _httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
                        _httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
                        HttpResponseMessage result = _httpClient.PostAsync(url, content).Result;
                        if (result.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            //return true;
                            returnValue = result.Content.ReadAsStringAsync().Result;
                            dynamic deserialized = JsonConvert.DeserializeObject(returnValue.ToString());
                        }
                        else { }
                    }
                }
            }
            catch (Exception)
            { throw; }
            return returnValue;
        }
        private void btnGenerate_Click(object sender, EventArgs e)
        {
            apitype = "Generate";
            if (comboBox1.SelectedItem.ToString() == "INV")
                CreateJsonString(dsGenEWB.Tables[0]);
            if (comboBox1.SelectedItem.ToString() == "CRN")
                CreateJsonStringCRD(dsGenEWB.Tables[0]);
            if (comboBox1.SelectedItem.ToString() == "DBN")
                CreateJsonStringCRD(dsGenEWB.Tables[0]);
        }
        private void btnImport_Click(object sender, EventArgs e)
        {
            apitype = "import";
            if (comboBox1.SelectedItem.ToString() == "INV")
                CreateJsonString(dsGenEWB.Tables[0]);
            if (comboBox1.SelectedItem.ToString() == "CRN")
                CreateJsonStringCRD(dsGenEWB.Tables[0]);
            if (comboBox1.SelectedItem.ToString() == "DBN")
                CreateJsonStringCRD(dsGenEWB.Tables[0]);
           
        }

        #region  CREDIT NODE

        public void CreateJsonStringCRD(DataTable crJsonTb)
        {

            if (crJsonTb.Rows.Count > 0)
            {
                int HeadeIndx = 0;
                csInvPost objPt = new csInvPost();
                invCrd objinv = new invCrd();
                string hsncode = HSNCODE2;
                foreach (DataRow dr in crJsonTb.Rows)
                {
                    if (HeadeIndx == 0)
                    {
                        string bvv = "";
                        if (SGSTIN == "EMPTY")
                            bvv = dr["supplier_GSTIN"].ToString().Trim();
                        else bvv = SGSTIN;
                        string sts = "";
                        if (STATE == "EMPTY")
                            sts = dr["supplier_State"].ToString().Trim();
                        else sts = STATE;

                        string pncd = "";
                        if (PINCODE == "EMPTY")
                            pncd = dr["supplier_Pincode"].ToString().Replace(" ", "");
                        else pncd = PINCODE;
                        objPt.supplier_GSTIN = bvv;// "29AAFPB7029M000";//dr["supplier_GSTIN"].ToString();
                        objPt.supplier_Legal_Name = dr["supplier_Legal_Name"].ToString().Trim();
                        // objPt.supplier_trading_name = "VSPL";
                        objPt.supplier_City = dr["supplier_City"].ToString().Trim();
                        objPt.supplier_Address1 = dr["supplier_Address1"].ToString().Trim();
                        objPt.supplier_Address2 = dr["supplier_Address2"].ToString().Trim();
                        objPt.supplier_State = sts; //"29";//dr["supplier_State"].ToString();
                        objPt.supplier_Pincode = pncd;// "560087";// dr["supplier_Pincode"].ToString();
                                                      // objPt.supplier_Phone = "9810885187";
                                                      //  objPt.supplier_Email = "supplier@gmail.com";
                        HeadeIndx++;
                       // string gg = "";
                        if (dr["invoice_type_code"].ToString().Trim() == "SEZWO" || dr["invoice_type_code"].ToString().Trim() == "EXPWO")
                        {
							invoice_type_code_B2C = dr["invoice_type_code"].ToString().Trim() + "P";
                        }
                        else
                        { invoice_type_code_B2C = dr["invoice_type_code"].ToString().Trim(); }


                        var transaction = new
                        {
							transactionMode = cmbTransMd.Text.ToString().Trim(),// dr["transactionMode"].ToString().Trim(),
                            invoice_type_code = invoice_type_code_B2C,
                            reversecharge = dr["reversecharge"].ToString().Trim()
                            //ecom_GSTIN = "",
                            // IgstOnIntra = "N"

                        };
                        objinv.transaction_details = transaction;

                        string binv = "";
                        if (INVNUM == "EMPTY")
                            binv = dr["invoiceNum"].ToString();
                        else binv = INVNUM;

                        var document = new
                        {
                            invoice_subtype_code = dr["invoice_subtype_code"].ToString().Trim(),
                            invoiceNum = binv,
                            invoiceDate = dr["invoiceDate"].ToString().Trim()
                            // transation_id = "1234",
                            // plant = "PLANT1",
                            // custom = "123"

                        };
                        objinv.document_details = document;

                        //var export = new
                        //{
                        //    shipping_bill_no = "1234567",
                        //    shipping_bill_date = "02-01-2018",
                        //    port_code = "",
                        //    invoice_currency_code = "INR",
                        //    cnt_code = "IN",
                        //    refClm = "N",
                        //    ExpDuty = 1
                        //};
                        //objinv.export_details = export;

                        var extra = new
                        {

                            //    invoice_Period_Start_Date = "10-03-2019",
                            //    invoice_Period_End_Date = "11-02-2019",
                            preceeding_Invoice_Number = dr["preceeding_Invoice_Number"].ToString(),//"INV002",
                            preceeding_Invoice_Date = dr["preceeding_Invoice_Date"].ToString(),//"20-09-2019"
                            //    invoice_Document_Reference = "KOL01",
                            //    receipt_Advice_ReferenceNo = "CREDIT30",
                            //    receipt_Advice_ReferenceDt = "20-09-2019",
                            //    tender_or_Lot_Reference = "AALPS",
                            //    contract_Reference = "CONT23072019",
                            //    external_Reference = "EXT23222",
                            //    project_Reference = "PJTCODE01",
                            //    refNum = "Vendor PO /1",
                            //    refDate = "30-01-2020",
                            //    remarks = "Invoice remarks",
                            //    Url = "http://www.xyz.com/abc",
                            //    Docs = "",
                            //    Info = "3333"

                        };
                        objinv.extra_Information = extra;

                        string bGst = "";
                        if (STATE == "EMPTY")
                            bGst = dr["billing_GSTIN"].ToString();
                        else bGst = BGSTIN;
                        string bstate = "";
                        if (STATE == "EMPTY")
                            bstate = dr["billing_State"].ToString();
                        else bstate = STATE;

                        string bpin = "";
                        if (PINCODE == "EMPTY")
                            bpin = dr["billing_Pincode"].ToString().Replace(" ", "");
                        else bpin = PINCODE;

                        var billing = new
                        {
                            billing_Name = dr["billing_Name"].ToString().Trim(),
                            // billing_trade_name = "ADA",
                            billing_GSTIN = bGst,//"29AABCS0858G1Z9",//dr["billing_GSTIN"].ToString(),
                            billing_POS = dr["billing_POS"].ToString().Trim(),
                            billing_City = dr["billing_City"].ToString().Trim(),
                            billing_State = bstate,//"29",//dr["billing_State"].ToString(),
                            billing_Address1 = dr["billing_Address1"].ToString().Trim(),
                            billing_Address2 = dr["billing_Address2"].ToString().Trim(),
                            billing_Pincode = bpin.Trim() //"560087",// dr["billing_Pincode"].ToString()//"560087"
                            // billing_Phone = "9999999999",
                            //billing_Email = "billing@go4gst.com"
                        };
                        objinv.billing_Information = billing;

                        if (cmbTransMd.Text.ToString() == "SHP" || cmbTransMd.Text.ToString() == "CMB")
                        {
                            // Querystring += " ,H.SHPNAME,H.SHIPTO,H.SHPADDR1,H.SHPADDR2,H.SHPCITY,H.SHPCOUNTRY,H.SHPSTATE,H.SHPZIP ";
                            var shipping = new
                            {
                                shippingTo_Name = dr["SHPNAME"].ToString().Trim(),
                                //shippingTo_trade_name = dr["SHIPTO"].ToString().Trim(),
                                // shippingTo_GSTIN = "",
                                shippingTo_Place = dr["SHPCITY"].ToString().Trim(),
                                shippingTo_State = bstate,
                                shippingTo_Address1 = dr["SHPADDR1"].ToString().Trim(),
                                shippingTo_Address2 = dr["SHPADDR2"].ToString().Trim(),
                                shippingTo_Pincode = dr["SHPZIP"].ToString().Trim(),

                            };
                            objinv.shipping_Information = shipping;
                        }

                        //delivery_Information/Dispatch from information
                        if (cmbTransMd.Text.ToString() != "REG")
                        {
                            //MessageBox.Show(dr["location_code"].ToString());
                            //  string gg = "";
                            if (dr["location_code"].ToString().Substring(3, 2) == "13")
                            {
                                var delivery = new
                                {
                                    company_Name = "VSPL",
                                    city = "Gurgaon",
                                    state = "06",
                                    address1 = "M22, Old DLF,Sector-14",
                                    address2 = "",
                                    pincode = "122002"
                                };
                                objinv.delivery_Information = delivery;
                            }
                            else if (dr["location_code"].ToString().Substring(3, 2) == "15")
                            {
                                var delivery = new
                                {
                                    company_Name = "VSPL",
                                    city = "Noida",
                                    state = "09",
                                    address1 = "C/O Pro-Connect Supply Chain Solutions Ltd,Khasra No.357",
                                    address2 = ",Village Chhajarsi,Sec.-63",
                                    pincode = "201301"
                                };
                                objinv.delivery_Information = delivery;
                            }
                            else if (dr["location_code"].ToString().Substring(3, 2) == "30")
                            {
                                var delivery = new
                                {
                                    company_Name = "VSPL",
                                    city = "Bangalore",
                                    state = "29",
                                    address1 = "No.46/2,Ramesh Reddy layout,Behind Nandi Toyota Showroom",
                                    address2 = ",Garvebhavipalya, Hosur Road",
                                    pincode = "560068"
                                };
                                objinv.delivery_Information = delivery;
                            }
                            else if (dr["location_code"].ToString().Substring(3, 2) == "40")
                            {
                                var delivery = new
                                {
                                    company_Name = "VSPL",
                                    city = "Mumbai",
                                    state = "27",
                                    address1 = "Gala No. 26,1st Floor, Sainath Industrial EstateVishweshwar Road,",
                                    address2 = " Goregaon (East)",
                                    pincode = "400063"
                                };
                                objinv.delivery_Information = delivery;
                            }
                            else if (dr["location_code"].ToString().Substring(3, 2) == "45")
                            {
                                var delivery = new
                                {
                                    company_Name = "VSPL",
                                    city = "Mumbai",
                                    state = "27",
                                    address1 = "Gala No. 26,1st Floor, Sainath Industrial EstateVishweshwar Road, Goregaon (East)",
                                    address2 = "",
                                    pincode = "400063"
                                };
                                objinv.delivery_Information = delivery;
                            }
                            else if (dr["location_code"].ToString().Substring(3, 2) == "61")
                            {
                                var delivery = new
                                {
                                    company_Name = "VSPL",
                                    city = "Tamilnadu",
                                    state = "33",
                                    address1 = "C/O DHL Logistics Pvt Ltd,J.Matadee FTZ Pvt Ltd, Survey no 434/A5,434/A6,435/2,437/A",
                                    address2 = ", Mannur Village,Sriperumbudur Taluk,Kanchipuram Dist",
                                    pincode = "602105"
                                };
                                objinv.delivery_Information = delivery;
                            }
                            else if (dr["location_code"].ToString().Substring(3, 2) == "65")
                            {
                                var delivery1 = new
                                {
                                    company_Name = "VSPL",
                                    city = "Tiruvallur",
                                    state = "33",
                                    address1 = "C/O Pro-Connect Supply Chain Solutions Ltd 79,Kurathanamedu,Panpakkam Village",
                                    address2 = ", Gummudipundi Taluk",
                                    pincode = "602106"
                                };
                                objinv.delivery_Information = delivery1;
                            }
                            else
                            {
                                //var delivery = new
                                //{
                                //    company_Name = "VSPL",
                                //    city = "Tamilnadu",
                                //    state = "33",
                                //    address1 = "C/O DHL Logistics Pvt Ltd,J.Matadee FTZ Pvt Ltd, Survey no 434/A5,434/A6,435/2,437/A,",
                                //    address2 = " Mannur Village,Sriperumbudur Taluk,Kanchipuram Dist",
                                //    pincode = "602105"

                                //};
                                //objinv.delivery_Information = delivery;
                            }

                        }



						//var payee = new
						//{
						//    payee_Name = "Pay name",
						//    payer_Financial_Account = "01123421401",
						//    modeofPayment = "Cash",
						//    financial_Institution_Branch = "SBIN0021882",
						//    payment_Terms = "PTO",
						//    payment_Instruction = "PIO",
						//    credit_Transfer = "Done",
						//    direct_Debit = "Direct",
						//    creditDays = 3,
						//    paid_amount = 1800,
						//    amount_due_for_payment = 400

						//};
						//objinv.payee_Information = payee;


						//var ewaybill = new
						//{
						//    ewb_transporter_id = "05AAACH1004N1Z0",
						//    ewb_transMode = "1",
						//    ewb_transDistance = 100,
						//    ewb_transporterName = "VSPL Trans",
						//    ewb_transDocNo = "TEST01",
						//    ewb_transDocDt = "14-08-2020",
						//    ewb_vehicleNo = "UP86C2345",
						//    ewb_subSupplyType = "1",
						//    ewb_vehicleType = "O"

						//};
						//objinv.ewaybill_information = ewaybill;
						//stateCessValue = Convert.ToInt16(dr["stateCessValue"].ToString()),
						// roundoff = 0.2,
						string HDISCOUNT = dr["Discount"].ToString();
						if (HDISCOUNT == "")
							HDISCOUNT = "0";
						var documentTotal = new
                        {
                            total_assVal = Math.Round(Convert.ToDecimal(dr["total_assVal"].ToString()), 2),
                            total_Invoice_Value = Math.Round(Convert.ToDecimal(dr["total_Invoice_Value"].ToString()), 2),
                            // total_Invoice_Value = dr["total_Invoice_Value"].ToString(),
                            igstvalue = Math.Round(Convert.ToDouble(dr["igstvalue"].ToString()), 2),
                            cgstvalue = Math.Round(Convert.ToDouble(dr["cgstvalue"].ToString()), 2),
                            sgstvalue = Math.Round(Convert.ToDouble(dr["sgstvalue"].ToString()), 2),
                            cessvalue = Math.Round(Convert.ToDouble(dr["cessvalue"].ToString()), 2),
                            // val_for_cur = 0,
                            Discount = Math.Round(Convert.ToDouble(HDISCOUNT), 2)
                            // OthChrg = 0
                        };
                        objinv.document_Total = documentTotal;
                        objinv.items = new List<items>();
                        hsncode = HSNCODE1;
                    }

                    // item .......................................

                    items objitem = new items();
                    objitem.slno = Convert.ToInt16(dr["slno"].ToString());
                    // objitem.item_Description = "Mobile";
                    // if (hsncode == HSNCODE1)  //8471
                    objitem.service = dr["service"].ToString().Trim();
                    // else
                    //objitem.service = "Y";
                    if (hsncode != "EMPTY")
                        objitem.hsn_code = hsncode;// dr["hsn_code"].ToString().Trim();
                    else objitem.hsn_code = dr["hsn_code"].ToString().Trim();
                    //var batchstr = new
                    //{
                    //    batchName = "PQR",
                    //    batchExpiry_Date = "30-12-2019",
                    //    warrantyDate = "20-11-2020"
                    //};
                    //objitem.batch = batchstr;
                    //var attribDtlsSR = new
                    //{
                    //    attrib_name = "PQR",
                    //    attrib_val = "12345"
                    //};
                    //objitem.attribDtls = attribDtlsSR;
                    //objitem.barcode = "b123";
                    objitem.quantity = Convert.ToInt32(dr["quantity"]);
                    //objitem.freeQty = 0;
                    objitem.uqc = dr["uqc"].ToString();
                    objitem.rate = Math.Round(Convert.ToDecimal(dr["rate"].ToString()), 2);
                    objitem.grossAmount = Math.Round(Convert.ToDecimal(dr["grossAmount"].ToString()), 2);
                    objitem.discountAmount = Math.Round(Convert.ToDecimal(dr["discountAmount"].ToString()), 2);
                    //objitem.preTaxAmount = Convert.ToInt16(dr["slno"].ToString());
                    objitem.assesseebleValue = Math.Round(Convert.ToDecimal(dr["assesseebleValue"].ToString()), 2);
                    objitem.igst_rt = Math.Round(Convert.ToDecimal(dr["igst_rt"].ToString()), 3); ///Convert.ToDecimal(dr["igst_rt"].ToString());
                    objitem.cgst_rt = Math.Round(Convert.ToDecimal(dr["cgst_rt"].ToString()), 3); // Convert.ToDecimal(dr["cgst_rt"].ToString());
                                                                                                  //objitem.sgst_rt = Math.Round(Convert.ToDecimal(dr["igst_rt"].ToString()), 3); // Convert.ToDecimal(dr["sgst_rt"].ToString());
                    objitem.sgst_rt = Math.Round(Convert.ToDecimal(dr["sgst_rt"].ToString()), 3); //Convert.ToDecimal(dr["sgst_rt"].ToString());
                    objitem.iamt = Math.Round(Convert.ToDecimal(dr["iamt"].ToString()), 2);
                    objitem.camt = Math.Round(Convert.ToDecimal(dr["camt"].ToString()), 2);
                    objitem.samt = Math.Round(Convert.ToDecimal(dr["samt"].ToString()), 2);
                    objitem.csamt = Math.Round(Convert.ToDecimal(dr["csamt"].ToString()), 2);
                    //objitem.cessnonadval = 0;
                    //objitem.state_cess = 0;
                    //objitem.stateCessAmt = 0;
                    //objitem.stateCesNonAdvlAmt = 0;
                    objitem.otherCharges = Math.Round(Convert.ToDecimal(dr["otherCharges"].ToString()), 2);
                    objitem.itemTotal = Math.Round(Convert.ToDecimal(dr["itemTotal"].ToString()), 2);
                    //objitem.ordLineRef = "11";
                    objitem.origin_Country = "IN";
                    //objitem.prdSlNo = "";
                    objinv.items.Add(objitem);
                    hsncode = HSNCODE2;
                }
                objPt.inv = new[] { objinv };
                string json = JsonConvert.SerializeObject(objPt);
				//webBrowserRequst.DocumentText = json;
				// dynamic Response = POSTData(objPt, "https://aspstaging.go4gst.com/GO4GST_EINV/rest/eInvWebService/eInvERP/DEMO?type=generate"); //test

				//dynamic Response = POSTData(objPt, "https://velocis.go4gst.com/GO4GST_EINV/rest/eInvWebService/eInvERP/velocis_db?type=generate");

				//  dynamic Response = POSTData(objPt, "https://velocis.go4gst.com/GO4GST_EINV/rest/eInvWebService/eInvERP/velocis_db?type=" + apitype + "");
				dynamic Response=null;
				if (invoice_type_code_B2C == "B2C")
				{
					//    Response = POSTData(objPt, "https://aspstaging.go4gst.com/GO4GST/rest/eInvWebService/eInvERP/VELOCEL?type=import");
					//Response = POSTData(objPt, "https://velocis.go4gst.com/GO4GST_EINV/rest/eInvWebService/eInvERP/velocis_db?type=import");
				}
				else { Response = POSTData(objPt, APIURL + apitype); }				
              
                dynamic deserialized = null;
                if (Response != null)
                {
                    listBox1.Items.Add("API Response success!");
                    deserialized = JsonConvert.DeserializeObject(Response.ToString());
                }
                else { listBox1.Items.Add("Waiting for API Response Failed....."); 
                webBrowser1.DocumentText = json + "Repsponse" + JsonConvert.DeserializeObject(Response.ToString()); return;
				}
				//webBrowserResonse.DocumentText = deserialized.ToString();
				string statusheader = deserialized.STATUS;
                string statuscode = deserialized.STATUS_CODE;

                var strresponse = deserialized.RESPONSE;
                if (statusheader == "ERROR_IN_GENERATE")
                {
                    csQRCode objQr = new csQRCode();
                    csCreateInvPdf objinvoce = new csCreateInvPdf();
                    var ErrorDetails = strresponse["ErrorDetails"];
                    dynamic des = JsonConvert.DeserializeObject(ErrorDetails.ToString());

                    string ErrorCode = des[0].ErrorCode.ToString();
                    var InfoDtls = strresponse["InfoDtls"];
                    if (ErrorCode == "2150")
                    {
                        try
                        {
                            var Desc = InfoDtls[0].Desc;
                            var irn = Desc["Irn"];
                            var qrcode = Desc["SignedQRCode"];
                            //var irn = Desc.Irn;
                            string Errormsg = des[0].ErrorMessage.ToString();
                            ///string Irn = des[0].Irn.ToString();
                            //Environment.NewLine
                            var confirmResult = MessageBox.Show(Errormsg + ", If you want to creating again then click Yes!", "", MessageBoxButtons.YesNo);
                            if (confirmResult == DialogResult.Yes)
                            {
                                if (objinvoce.SaveIRNNOByInvNo(cmbxInvoice.SelectedValue.ToString(), comboBox1.Text, irn.ToString(), DateTime.Now.ToString(), "", "", "", qrcode.ToString(), Datatype) == "TRUE")
                                {
                                    Boolean djjj = objQr.createQrImage(irn.ToString(), cmbxInvoice.SelectedValue.ToString().Trim(), qrcode.ToString());

                                    listBox1.Items.Add("IRN and Ewaybill Processed Successfully.");
                                    MessageBox.Show("IRN and Ewaybill Processed Successfully.");
                                }

                                //Boolean jjj = objinvoce.createinv(cmbxInvoice.SelectedValue.ToString().Trim(), irn.ToString(), cmbCRDRep.Text, CRDNOTEstr);
                            }
                            else
                            {
                                // If 'No', do something here.
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }

                else if (statusheader == "ERROR_IN_IMPORT")
                {
                    csQRCode objQr = new csQRCode();
                    csCreateInvPdf objinvoce = new csCreateInvPdf();
                    var ErrorDetails = strresponse["ErrorDetails"];
                    dynamic des = JsonConvert.DeserializeObject(ErrorDetails.ToString());

                    string ErrorCode = des[0].ErrorCode.ToString();
                    var InfoDtls = strresponse["InfoDtls"];
                    if (ErrorCode == "ASP-4007")
                    {
                        try
                        {
                            var Desc = InfoDtls[0].Desc;
                            var irn = Desc["Irn"];
                            var qrcode = Desc["SignedQRCode"];
                            //var irn = Desc.Irn;
                            string Errormsg = des[0].ErrorMessage.ToString();
                            ///string Irn = des[0].Irn.ToString();
                            //Environment.NewLine
                            var confirmResult = MessageBox.Show(Errormsg + ", If you want to creating again then click Yes!", "", MessageBoxButtons.YesNo);
                            if (confirmResult == DialogResult.Yes)
                            {
                                Boolean djjj = objQr.createQrImage(irn.ToString(), cmbxInvoice.SelectedValue.ToString().Trim(), qrcode.ToString());
                                //Boolean jjj = objinvoce.createinv(cmbCreditnote.SelectedValue.ToString().Trim(), irn.ToString(), cmbCRDRep.Text, CRDNOTEstr);
                            }
                            else
                            {
                                // If 'No', do something here.
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                else if (statusheader == "ERROR")
                {
					if (invoice_type_code_B2C == "B2C")
					{
						listBox1.Items.Add(strresponse.UPI_QR_code);
						return;
					}
					try
                    {
                        var ErrorDetails = strresponse["ErrorDetails"];
                        dynamic des = JsonConvert.DeserializeObject(ErrorDetails.ToString());
                        string Errormsg = des[0].ErrorMessage.ToString();
                        string ErrorCode = des[0].ErrorCode.ToString();
                        string invNum = deserialized.invoiceNum;
                        ErrorXmlList(invNum, ErrorCode, Errormsg);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                //else if (statusheader == "ERROR_IN_GENERATE")
                //{
                //    try
                //    {
                //        var InfoDtls = strresponse["InfoDtls"];
                //        var ErrorDetails = strresponse["ErrorDetails"];
                //        dynamic des = JsonConvert.DeserializeObject(ErrorDetails.ToString());
                //        string Errormsg = des[0].ErrorMessage.ToString();
                //        string ErrorCode = des[0].ErrorCode.ToString();
                //        string invNum = deserialized.invoiceNum;
                //        ErrorXmlList(invNum, ErrorCode, Errormsg);
                //    }
                //    catch (Exception)
                //    {
                //        throw;
                //    }
                //}
                else if (statusheader == "SUCCESS")
                {
					if (invoice_type_code_B2C == "B2C")
					{
						try
						{
							//string statusheader = deserialized.STATUS;
							//string statuscode = deserialized.STATUS_CODE;
							//MessageBox.Show(strresponse.ToString());
							//var strresponse = deserialized.RESPONSE;
							//MessageBox.Show(strresponse["UPI_QR_Code"].ToString());
							var qrcd = strresponse["UPI_QR_Code"];
							string sirndt = DateTime.Now.ToString("dd/MM/yyyy");
							string sirn = "";
							string EwbNo = "";
							string EwbDt = "";
							string EwbValidTill = "";
							string sinv = deserialized.invoiceNum;
							listBox1.Items.Add("Please wait, QR Thumb creating in process.........");
							csCreateInvPdf objinvoce = new csCreateInvPdf();
							csQRCode objQr = new csQRCode();//UPI_QR_Code
							if (objinvoce.SaveIRNNOByInvNo(cmbxInvoice.SelectedValue.ToString(), comboBox1.Text, "B2C", sirndt, EwbNo, EwbDt, EwbValidTill, qrcd.ToString(), Datatype) == "TRUE")
							{
								listBox1.Items.Add("B2C Document Processed Successfully.");
								MessageBox.Show("B2C Document Processed Successfully.");
							}

							if (objQr.createQrImageBS64(sirn.ToString(), sinv.ToString(), qrcd.ToString()) == true)
							{
								listBox1.Items.Add("QR Thumb created!");
							}
						}
						catch (Exception)
						{ throw; }
					}
					else
					{
						try
						{
							csCreateInvPdf objinvoce = new csCreateInvPdf();
							csQRCode objQr = new csQRCode();
							string status = strresponse["Status"].ToString();
							string Irn = strresponse["Irn"].ToString();
							var rep = strresponse["SignedQRCode_Decrypted"];
							var dd = rep.data;
							var sirn = dd.Irn.ToString();
							var sinv = dd.DocNo.ToString();
							string qrcd = strresponse["SignedQRCode"].ToString();
							//var qrcd = dd.SignedQRCode.ToString();
							var sirndt = dd.IrnDt.ToString();
							string EwbNo = "";
							string EwbDt = "";
							string EwbValidTill = "";
							if (chkEwayBill.Checked == true)
							{
								EwbNo = ""; //gov_response.EwbNo;
								EwbDt = ""; //gov_response.EwbDt;
								EwbValidTill = "";// gov_response.EwbValidTill;
								listBox1.Items.Add("EwayBill NO:" + EwbNo);
								listBox1.Items.Add("EwbDt:" + EwbDt);
								txtEwaybillNo.Text = EwbNo;
								txtEWayDate.Text = EwbDt;
								txtEwatValTo.Text = EwbValidTill;
							}
							listBox1.Items.Add("Please wait, QR Thumb creating in process.........");

							if (objinvoce.SaveIRNNOByInvNo(cmbxInvoice.SelectedValue.ToString(), comboBox1.Text, sirn.ToString(), sirndt, EwbNo, EwbDt, EwbValidTill, qrcd.ToString(), Datatype) == "TRUE")
							{
								listBox1.Items.Add("IRN and Ewaybill Processed Successfully.");
								MessageBox.Show("IRN and Ewaybill Processed Successfully.");
							}
							if (objQr.createQrImage(sirn.ToString(), sinv.ToString(), qrcd.ToString()) == true)
							{
								listBox1.Items.Add("QR Thumb created!");
							}
						}
						catch (Exception)
						{ throw; }
					}

					//try
					//{
					//    csCreateInvPdf objinvoce = new csCreateInvPdf();
					//    csQRCode objQr = new csQRCode();
					//    string status = strresponse["Status"].ToString();
					//    string Irn = strresponse["Irn"].ToString();
					//    var rep = strresponse["SignedQRCode"];
					//    var repDc = strresponse["SignedQRCode_Decrypted"];
					//    var dd = repDc.data;
					//    var sirn = dd.Irn.ToString();
					//   /// var qrcode = dd.SignedQRCode.ToString();
					//    var sinv = dd.DocNo.ToString();
					//    var sirndt = dd.IrnDt.ToString();
					//    listBox1.Items.Add("Please wait, QR Thumb creating in process.........");

					//    if (objinvoce.SaveIRNNOByInvNo(cmbxInvoice.SelectedValue.ToString(), comboBox1.Text, sirn.ToString(), sirndt, "", "", "", rep.ToString(), Datatype) == "TRUE")
					//    {
					//        listBox1.Items.Add("IRN and Ewaybill Processed Successfully.");
					//        MessageBox.Show("IRN and Ewaybill Processed Successfully.");
					//    }

					//    if (objQr.createQrImage(sirn.ToString(), sinv.ToString(), rep.ToString()) == true)
					//    {
					//        listBox1.Items.Add("QR Thumb created!");
					//    }
					//    //SuccessXmlList(sinv, Irn, sirndt);
					//}
					//catch (Exception)
					//{ throw; }

					//MessageBox.Show("Invoice Number="+ sinv + "   Irn Number== "+ sirn);
				}
                else { return; }
            }
            else { MessageBox.Show("Data Empty!!!"); }
        }
        public string checkGtnAvlCRDND(string invn)
        {
            DataSet dt;
            string connectionstring = "Data Source=" + SERVERNAME + "; Initial Catalog=" + SAGEDB + "; User ID=" + SAA + "; Password=" + SAPSS + ";";
            //MessageBox.Show(connectionstring);
            //constr = "Provider=SQLOLEDB;Data Source=ERP-DATABASE; Initial Catalog=TSTDAT;User ID=sa; Password=Vspl@4321"
            //string connectionstring = "Data Source=ERP-DATABASE; Initial Catalog=TSTDAT; User ID=sa; Password=Vspl@4321;";
            System.Data.SqlClient.SqlConnection conn;
            System.Data.SqlClient.SqlCommand cmd;
            conn = new System.Data.SqlClient.SqlConnection(connectionstring);
            conn.Open();           
            string Querystring = "select ar.value from arcuso ar	inner join OECRDH h on h.CUSTOMER=ar.IDCUST where ar.optfield='gstin' and h.CRDNUMBER='" + invn + "'";
            cmd = new System.Data.SqlClient.SqlCommand(Querystring, conn);
            cmd.CommandTimeout = 180;
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
            using (System.Data.SqlClient.SqlDataAdapter sda = new System.Data.SqlClient.SqlDataAdapter())
            {
                cmd.Connection = conn;
                sda.SelectCommand = cmd;
                using (dt = new DataSet())
                {
                    sda.Fill(dt);
                    String ss = dt.Tables[0].Rows[0]["value"].ToString().Trim();
                    if (string.IsNullOrEmpty(ss))
                        return "FALSE";
                    int lntg = ss.Length;
                    if (lntg >= 3 & lntg <= 15)
                    {
                        return "TRUE";
                    }
                    else
                        return "FALSE";
                }
            }
        }
        public void GetInvDetbyCRD(string strCRDNode)
        {
            //DataSet dsGenEWB;
            string connectionstring = "Data Source=" + SERVERNAME + "; Initial Catalog=" + SAGEDB + "; User ID=" + SAA + "; Password=" + SAPSS + ";";
            //MessageBox.Show(connectionstring);
            //constr = "Provider=SQLOLEDB;Data Source=ERP-DATABASE; Initial Catalog=TSTDAT;User ID=sa; Password=Vspl@4321"
            //string connectionstring = "Data Source=ERP-DATABASE; Initial Catalog=TSTDAT; User ID=sa; Password=Vspl@4321;";
            System.Data.SqlClient.SqlConnection conn;
            System.Data.SqlClient.SqlCommand cmd;
            conn = new System.Data.SqlClient.SqlConnection(connectionstring);
            conn.Open();
			///filter_0 = filter_0.Replace("d.QTYSHIPPED", "d.QTYRETURN");
			string QuerystringDTN = "select rtrim((select [value] from  CSOPTFD where OPTFIELD='GSTNOS' and left([value],2)=left(H.TAXGROUP,2))) supplier_GSTIN,  ";
            QuerystringDTN += " (select RTRIM(CONAME) from  CSCOM) supplier_Legal_Name,(Select RTRIM(CITY) from ICLOC a1 where a1.LOCATION=H.LOCATION) supplier_City, ";
            QuerystringDTN += "  (Select RTRIM(ADDRESS1)+' '+RTRIM(ADDRESS2) from ICLOC a1 where a1.LOCATION=H.LOCATION) supplier_Address1,  ";
            QuerystringDTN += "  (Select RTRIM(ADDRESS3)+' '+RTRIM(ADDRESS4) from ICLOC a1 where a1.LOCATION=H.LOCATION) supplier_Address2,  ";
            QuerystringDTN += "  (Select left(h.taxgroup,2) from ICLOC a1 where a1.LOCATION=H.LOCATION) supplier_State,  ";
            QuerystringDTN += "  (Select RTRIM(ZIP) from ICLOC a1 where a1.LOCATION=H.LOCATION)supplier_Pincode,  ";
			QuerystringDTN += "CASE WHEN H1.VALUE='R' THEN 'B2B' WHEN  H1.VALUE='SEWP' THEN 'SEZWP' WHEN  H1.VALUE='SEWOP' THEN 'SEZWOP' WHEN  H1.VALUE='EXWP' THEN 'EXPWP' WHEN  H1.VALUE='EXWOP' THEN 'EXPWOP' WHEN  H1.VALUE='DE' THEN 'DEXP' ELSE 'B2B' END invoice_type_code ,";
            //QuerystringDTN += "  ISNULL((select value from  OECRDHO where CRDUNIQ=h.CRDUNIQ and OPTFIELD='GINVTYPE'),'') invoice_type_code,  ";
            QuerystringDTN += "  CASE when SUBSTRING(h.TAXGROUP,5,1)='R' then 'Y' else 'N' end reversecharge,'DBN' invoice_subtype_code,  ";
            QuerystringDTN += "  RTRIM(h.CRDNUMBER) invoiceNum,SUBSTRING(CAST(h.CRDDATE AS CHAR),7,2)+'-'+SUBSTRING(CAST(h.CRDDATE AS CHAR),5,2)+'-'+SUBSTRING(CAST(h.CRDDATE AS CHAR),1,4) invoiceDate,  ";
            QuerystringDTN += "  h.INVNUMBER preceeding_Invoice_Number, SUBSTRING(CAST(h.CRDDATE AS CHAR),7,2)+'-'+SUBSTRING(CAST(h.CRDDATE AS CHAR),5,2)+'-'+SUBSTRING(CAST(h.CRDDATE AS CHAR),1,4) preceeding_Invoice_Date,  ";
            QuerystringDTN += "   RTRIM(h.BILNAME) billing_Name, rtrim(ISNULL((select value from  OECRDHO where CRDUNIQ=h.CRDUNIQ and OPTFIELD='GPOS'),'')) billing_POS, ar.VALUE billing_GSTIN, ";
            QuerystringDTN += "  h.BILCITY billing_City,rtrim(ISNULL((select s.VALUE from ARCUSO s where h.CUSTOMER=s.IDCUST and s.OPTFIELD='GSTCODE'),'')) billing_State,RTRIM(H.BILADDR1) billing_Address1,  ";
            QuerystringDTN += " RTRIM(H.BILADDR2) billing_Address2,RTRIM(H.BILZIP) billing_Pincode,H.TBASE1*(H.INRATE) total_assVal,  ";
            QuerystringDTN += " case when substring(H.TAUTH1 ,3,3)='CGN' then H.TEAMOUNT1 else 0 end*(H.INRATE) cgstvalue,  case when substring(H.TAUTH2 ,3,3)='SGN' then H.TEAMOUNT2 else 0 end*(H.INRATE) sgstvalue,   ";
            QuerystringDTN += " case when substring(H.TAUTH1 ,3,3)='IGN' then H.TEAMOUNT1 else 0 end*(H.INRATE) igstvalue,  case when substring(H.TAUTH2 ,3,3)='CEN' then H.TEAMOUNT2 else  ";
            QuerystringDTN += "  case when substring(H.TAUTH3 ,3,3)='CEN' then H.TEAMOUNT3  else 0 end end*(H.INRATE) cessvalue, 0 Discount,ISNULL((SELECT SUM(D1.EXTCRDMISC) EXTINVMISC FROM OECRDD D1 WHERE D.CRDUNIQ=D1.CRDUNIQ AND D1.MISCCHARGE='ROUND'),0)*-1*(H.INRATE) HDISCOUNT , case when substring(d.TAUTH2 ,3,3)='TCS' then d.TAMOUNT2 else case when substring(d.TAUTH3 ,3,3)='TCS' then d.TAMOUNT3  else 0 end end*(H.INRATE) othercharges,0 roundoff,  H.CRDNETWTX*(H.INRATE) total_Invoice_Value,  ";
            QuerystringDTN += " 0 val_for_cur,  row_number() over(partition by d.CRDUNIQ order by d.CRDUNIQ) SLno,  ";
			//QuerystringDTN += "  case  when ISNULL((select value from  ICITEMO where ITEMNO=i.ITEMNO and OPTFIELD='GITEMTYPE'),'N')='S' then 'Y'  ";
			QuerystringDTN += " CASE  WHEN D.LINETYPE=1 AND  ISNULL((SELECT VALUE FROM  ICITEMO WHERE ITEMNO=I.ITEMNO AND OPTFIELD='GITEMTYPE'),'N')='S' THEN 'Y'  WHEN D.LINETYPE=2 AND ISNULL((SELECT VALUE FROM  OEMISCO  WHERE MISCCHARGE=D.MISCCHARGE AND OPTFIELD='GITEMTYPE' AND CURRENCY=H.INSOURCURR),'N')='S' THEN 'Y' ELSE 'N' END service, ";

			QuerystringDTN += " ISNULL((select SUBSTRING(value,1,8) value from  OECRDDO where CRDUNIQ=d.CRDUNIQ and LINENUM=d.LINENUM and OPTFIELD='GHSNCODE'),'') hsn_code,  ";
            QuerystringDTN += "  d.QTYRETURN quantity,ISNULL((Select RTRIM(VDESC) from CSOPTFD where OPTFIELD='GUOM' and RTRIM([value])=d.INVUNIT),'OTH') uqc,  ";
            QuerystringDTN += "  d.unitprice*(H.INRATE) rate, d.EXTCRDMISC grossAmount, d.CRDDISC+d.HDRDISC*(H.INRATE) discountAmount, d.TBASE1*(H.INRATE) assesseebleValue,case when substring(d.TAUTH1 ,3,3)='IGN' then d.TRATE1 else 0  end igst_rt,  ";
            QuerystringDTN += "  case when substring(d.TAUTH1 ,3,3)='CGN' then d.TRATE1 else 0 end cgst_rt, case when substring(d.TAUTH2 ,3,3)='SGN' then d.TRATE2 else 0 end sgst_rt,   ";
            QuerystringDTN += "  case when substring(d.TAUTH2 ,3,3)='CEN' then d.TRATE2 else case when substring(d.TAUTH3 ,3,3)='CEN' then d.TRATE3  else 0 end end cess_rt,    ";
            QuerystringDTN += "  case when substring(d.TAUTH1 ,3,3)='IGN' then d.TAMOUNT1 else 0 end*(H.INRATE) iamt, case when substring(d.TAUTH1 ,3,3)='CGN' then d.TAMOUNT1 else 0 end*(H.INRATE) camt,   ";
            QuerystringDTN += "  case when substring(d.TAUTH2 ,3,3)='SGN' then d.TAMOUNT2 else 0 end*(H.INRATE) samt, case when substring(d.TAUTH2 ,3,3)='CEN' then d.TAMOUNT2 else  ";
            QuerystringDTN += "  case when substring(d.TAUTH3 ,3,3)='CEN' then d.TAMOUNT3  else 0 end end*(H.INRATE) csamt,(d.TBASE1+d.TAMOUNT1+d.TAMOUNT2+d.TAMOUNT3)*(H.INRATE) itemTotal , h.CRDETAXTOT*(H.INRATE) TOTGST,  ";
            QuerystringDTN += "  RTRIM(H.LOCATION) location_code,H.SHPNAME,H.SHIPTO,H.SHPADDR1,H.SHPADDR2,H.SHPCITY,H.SHPCOUNTRY,H.SHPSTATE,H.SHPZIP from  OECRDH h left outer join OECRDD d on h.CRDUNIQ=d.CRDUNIQ AND D.MISCCHARGE<>'ROUND'   ";//left outer join OEINVH invh on h.INVNUMBER=invh.INVNUMBER 
			QuerystringDTN += "  left outer join ICITEM i on d.item=i.FMTITEMNO  left outer join ICITEMo o on i.ITEMNO=o.ITEMNO and o.OPTFIELD='GHSNCODE'  ";
            QuerystringDTN += "  left outer join ARCUSO ar on h.customer=ar.IDCUST and ar.OPTFIELD='GSTIN' LEFT OUTER JOIN OECRDHO H1 ON H.CRDUNIQ=H1.CRDUNIQ AND RTRIM(H1.OPTFIELD)='GINVTYPE'    ";
            QuerystringDTN += "  where substring(h.TAXGROUP,3,3) In('IGN','CGN','IGX') AND (D.LINETYPE=2 OR D.QTYRETURN<>0) and h.ADJTYPE=2 and H.CRDNUMBER='" + cmbxInvoice.SelectedValue.ToString().Trim() + "' ";
            // Querystring += " ,H.SHPNAME,H.SHIPTO,H.SHPADDR1,H.SHPADDR2,H.SHPCITY,H.SHPCOUNTRY,H.SHPSTATE,H.SHPZIP ";
            string Querystring = "select rtrim((select [value] from  CSOPTFD where OPTFIELD='GSTNOS' and left([value],2)=left(H.TAXGROUP,2))) supplier_GSTIN, ";
            Querystring += " (select RTRIM(CONAME) from  CSCOM) supplier_Legal_Name,(Select RTRIM(CITY) from ICLOC a1 where a1.LOCATION=H.LOCATION) supplier_City, ";
            Querystring += " (Select RTRIM(ADDRESS1)+' '+RTRIM(ADDRESS2) from ICLOC a1 where a1.LOCATION=H.LOCATION) supplier_Address1, ";
            Querystring += " (Select RTRIM(ADDRESS3)+' '+RTRIM(ADDRESS4) from ICLOC a1 where a1.LOCATION=H.LOCATION) supplier_Address2, ";
            Querystring += " (Select left(h.taxgroup,2) from ICLOC a1 where a1.LOCATION=H.LOCATION) supplier_State,  ";
            Querystring += " (Select RTRIM(ZIP) from ICLOC a1 where a1.LOCATION=H.LOCATION)supplier_Pincode, ";
			//Querystring += " ISNULL((select value from  OECRDHO where CRDUNIQ=h.CRDUNIQ and OPTFIELD='GINVTYPE'),'') invoice_type_code, ";
			Querystring += "CASE WHEN H1.VALUE='R' THEN 'B2B' WHEN  H1.VALUE='SEWP' THEN 'SEZWP' WHEN  H1.VALUE='SEWOP' THEN 'SEZWOP' WHEN  H1.VALUE='EXWP' THEN 'EXPWP' WHEN  H1.VALUE='EXWOP' THEN 'EXPWOP' WHEN  H1.VALUE='DE' THEN 'DEXP' ELSE 'B2B' END invoice_type_code ,";

			Querystring += " CASE when SUBSTRING(h.TAXGROUP,5,1)='R' then 'Y' else 'N' end reversecharge,'CRN' invoice_subtype_code, ";
            Querystring += " RTRIM(h.CRDNUMBER) invoiceNum,SUBSTRING(CAST(h.CRDDATE AS CHAR),7,2)+'-'+SUBSTRING(CAST(h.CRDDATE AS CHAR),5,2)+'-'+SUBSTRING(CAST(h.CRDDATE AS CHAR),1,4) invoiceDate,  ";
            Querystring += " h.INVNUMBER preceeding_Invoice_Number, SUBSTRING(CAST(h.CRDDATE AS CHAR),7,2)+'-'+SUBSTRING(CAST(h.CRDDATE AS CHAR),5,2)+'-'+SUBSTRING(CAST(h.CRDDATE AS CHAR),1,4) preceeding_Invoice_Date, ";
            Querystring += "  RTRIM(h.BILNAME) billing_Name, rtrim(ISNULL((select value from  OECRDHO where CRDUNIQ=h.CRDUNIQ and OPTFIELD='GPOS'),'')) billing_POS, ar.VALUE billing_GSTIN, ";
            Querystring += " h.BILCITY billing_City,rtrim(ISNULL((select s.VALUE from ARCUSO s where h.CUSTOMER=s.IDCUST and s.OPTFIELD='GSTCODE'),'')) billing_State,RTRIM(H.BILADDR1) billing_Address1, ";
            Querystring += " RTRIM(H.BILADDR2) billing_Address2,RTRIM(H.BILZIP) billing_Pincode,H.TBASE1 total_assVal, ";
            Querystring += " case when substring(H.TAUTH1 ,3,3)='CGN' then H.TEAMOUNT1 else 0 end*(H.INRATE) cgstvalue,  case when substring(H.TAUTH2 ,3,3)='SGN' then H.TEAMOUNT2 else 0 end*(H.INRATE) sgstvalue,  ";
            Querystring += " case when substring(H.TAUTH1 ,3,3)='IGN' then H.TEAMOUNT1 else 0 end*(H.INRATE) igstvalue,  case when substring(H.TAUTH2 ,3,3)='CEN' then H.TEAMOUNT2 else ";
            Querystring += " case when substring(H.TAUTH3 ,3,3)='CEN' then H.TEAMOUNT3  else 0 end end*(H.INRATE) cessvalue, 0 Discount,ISNULL((SELECT SUM(D1.EXTCRDMISC) EXTINVMISC FROM OECRDD D1 WHERE D.CRDUNIQ=D1.CRDUNIQ AND D1.MISCCHARGE='ROUND'),0)*-1*(H.INRATE) HDISCOUNT , case when substring(d.TAUTH2 ,3,3)='TCS' then d.TAMOUNT2 else case when substring(d.TAUTH3 ,3,3)='TCS' then d.TAMOUNT3  else 	0 end end*(H.INRATE) othercharges,0 roundoff,  H.CRDNETWTX*(H.INRATE) total_Invoice_Value, ";
            Querystring += " 0 val_for_cur,  row_number() over(partition by d.CRDUNIQ order by d.CRDUNIQ) SLno, ";
			Querystring += " CASE  WHEN D.LINETYPE=1 AND  ISNULL((SELECT VALUE FROM  ICITEMO WHERE ITEMNO=I.ITEMNO AND OPTFIELD='GITEMTYPE'),'N')='S' THEN 'Y'  WHEN D.LINETYPE=2 AND ISNULL((SELECT VALUE FROM  OEMISCO  WHERE MISCCHARGE=D.MISCCHARGE AND OPTFIELD='GITEMTYPE' AND CURRENCY=H.INSOURCURR),'N')='S' THEN 'Y' ELSE 'N' END service, ";
			Querystring += " ISNULL((select SUBSTRING(value,1,8) value from  OECRDDO where CRDUNIQ=d.CRDUNIQ and LINENUM=d.LINENUM and OPTFIELD='GHSNCODE'),'') hsn_code, ";
            Querystring += " d.QTYRETURN quantity,ISNULL((Select RTRIM(VDESC) from CSOPTFD where OPTFIELD='GUOM' and RTRIM([value])=d.INVUNIT),'OTH') uqc, ";
            Querystring += " d.unitprice*(H.INRATE) rate, d.EXTCRDMISC*(H.INRATE) grossAmount, (d.CRDDISC+d.HDRDISC)*(H.INRATE) discountAmount, d.TBASE1*(H.INRATE) assesseebleValue,case when substring(d.TAUTH1 ,3,3)='IGN' then d.TRATE1 else 0  end igst_rt, ";
            Querystring += " case when substring(d.TAUTH1 ,3,3)='CGN' then d.TRATE1 else 0 end cgst_rt, case when substring(d.TAUTH2 ,3,3)='SGN' then d.TRATE2 else 0 end sgst_rt,  ";
            Querystring += " case when substring(d.TAUTH2 ,3,3)='CEN' then d.TRATE2 else case when substring(d.TAUTH3 ,3,3)='CEN' then d.TRATE3  else 0 end end cess_rt,   ";
            Querystring += " case when substring(d.TAUTH1 ,3,3)='IGN' then d.TAMOUNT1 else 0 end*(H.INRATE) iamt, case when substring(d.TAUTH1 ,3,3)='CGN' then d.TAMOUNT1 else 0 end*(H.INRATE) camt,  ";
            Querystring += " case when substring(d.TAUTH2 ,3,3)='SGN' then d.TAMOUNT2 else 0 end*(H.INRATE) samt, case when substring(d.TAUTH2 ,3,3)='CEN' then d.TAMOUNT2 else ";
            Querystring += " case when substring(d.TAUTH3 ,3,3)='CEN' then d.TAMOUNT3  else 0 end end*(H.INRATE) csamt,(d.TBASE1+d.TAMOUNT1+d.TAMOUNT2+d.TAMOUNT3)*(H.INRATE) itemTotal , h.CRDETAXTOT*(H.INRATE) TOTGST, ";
            Querystring += " RTRIM(H.LOCATION) location_code,H.SHPNAME,H.SHIPTO,H.SHPADDR1,H.SHPADDR2,H.SHPCITY,H.SHPCOUNTRY,H.SHPSTATE,H.SHPZIP from  OECRDH h left outer join OECRDD d on h.CRDUNIQ=d.CRDUNIQ AND D.MISCCHARGE<>'ROUND' ";  // left outer join OEINVH invh on h.INVNUMBER=invh.INVNUMBER
			Querystring += " left outer join ICITEM i on d.item=i.FMTITEMNO  left outer join ICITEMo o on i.ITEMNO=o.ITEMNO and o.OPTFIELD='GHSNCODE' ";
            Querystring += "  left outer join ARCUSO ar on h.customer=ar.IDCUST and ar.OPTFIELD='GSTIN'  LEFT OUTER JOIN OEINVHO H1 ON H.INVUNIQ = H1.INVUNIQ AND RTRIM(H1.OPTFIELD)= 'GINVTYPE'  ";
            Querystring += "  where substring(h.TAXGROUP,3,3) In('IGN','CGN','IGX') AND (D.LINETYPE=2 OR D.QTYRETURN<>0) and  h.CRDNUMBER='" + cmbxInvoice.SelectedValue.ToString().Trim() + "' ";
            string crdnote = "";
            if (comboBox1.SelectedItem.ToString() == "CRN")
            {
                crdnote = Querystring;
            }
            else if (comboBox1.SelectedItem.ToString() == "DBN")
            {
                crdnote = QuerystringDTN;
            }
            else
            {
                MessageBox.Show("Please Select Document Type!");
                return;
            }
            cmd = new System.Data.SqlClient.SqlCommand(crdnote, conn);
            cmd.CommandTimeout = 180;
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();

            using (System.Data.SqlClient.SqlDataAdapter sda = new System.Data.SqlClient.SqlDataAdapter())
            {
                var objerrList = (IDictionary<string, object>)person;
                cmd.Connection = conn;
                sda.SelectCommand = cmd;
                dsGenEWB = new DataSet();
               // using (dsGenEWB = new DataSet())
               // { 
                sda.Fill(dsGenEWB);
                    if (CheckValidation(dsGenEWB.Tables[0]) == true)
                    {
                        MessageBox.Show("Validation Passed, You can proceed Import/Generate IRN.");
                    }
                    else { MessageBox.Show("Validation Failed.."); }
                    txtSupplyType.Text = dsGenEWB.Tables[0].Rows[0]["invoice_type_code"].ToString();
                    SGst.Text = dsGenEWB.Tables[0].Rows[0]["supplier_GSTIN"].ToString();
                    string gstin = dsGenEWB.Tables[0].Rows[0]["supplier_GSTIN"].ToString();

                    sname.Text = dsGenEWB.Tables[0].Rows[0]["supplier_Legal_Name"].ToString();
                    sState.Text = dsGenEWB.Tables[0].Rows[0]["supplier_State"].ToString();
                    SrichTextBox1.Text = dsGenEWB.Tables[0].Rows[0]["supplier_Address1"].ToString() + " " + dsGenEWB.Tables[0].Rows[0]["supplier_Address2"].ToString();
                    SCity.Text = dsGenEWB.Tables[0].Rows[0]["supplier_City"].ToString();
                    //Buyer Detail
                    Bgst.Text = dsGenEWB.Tables[0].Rows[0]["billing_GSTIN"].ToString();
                    Bname.Text = dsGenEWB.Tables[0].Rows[0]["billing_Name"].ToString();
                    Bstate.Text = dsGenEWB.Tables[0].Rows[0]["billing_POS"].ToString();
                    BrichTextBox2.Text = dsGenEWB.Tables[0].Rows[0]["billing_Address1"].ToString() + " " + dsGenEWB.Tables[0].Rows[0]["billing_Address2"].ToString();
                    BCity.Text = dsGenEWB.Tables[0].Rows[0]["billing_City"].ToString();
                    //Shipment Detail
                    SHCity.Text = dsGenEWB.Tables[0].Rows[0]["SHPCITY"].ToString();
                    SHToPin.Text = dsGenEWB.Tables[0].Rows[0]["SHPZIP"].ToString();
                    SHState.Text = dsGenEWB.Tables[0].Rows[0]["SHPSTATE"].ToString();
                    SHrichTextBox4.Text = dsGenEWB.Tables[0].Rows[0]["SHPADDR1"].ToString() + " " + dsGenEWB.Tables[0].Rows[0]["SHPADDR2"].ToString();
                    SHPTO.Text = dsGenEWB.Tables[0].Rows[0]["SHPNAME"].ToString();
                    txtTotASSVal.Text = dsGenEWB.Tables[0].Rows[0]["total_assVal"].ToString();
                    txtTotGST.Text = dsGenEWB.Tables[0].Rows[0]["TOTGST"].ToString();
                    txtTotInvVal.Text = dsGenEWB.Tables[0].Rows[0]["total_Invoice_Value"].ToString();
                //}
            }
            conn.Close();
        }
		private void label27_DoubleClick(object sender, EventArgs e)
        {
            listBox1.Visible = false;
            webBrowser1.Visible = true;
        }
		private void label17_DoubleClick(object sender, EventArgs e)
        {
            listBox1.Visible = true;
            webBrowser1.Visible = false;
        }
        #endregion

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem.ToString() == "INV")
                InvList();
            if (comboBox1.SelectedItem.ToString() == "CRN")
                CRNList(1);
            if (comboBox1.SelectedItem.ToString() == "DBN")
                CRNList(2);
        }
        public void InvList()
        {
            DataTable dt;
            string connectionstring = "Data Source=" + SERVERNAME + "; Initial Catalog=" + SAGEDB + "; User ID=" + SAA + "; Password=" + SAPSS + ";";
            System.Data.SqlClient.SqlConnection conn;
            System.Data.SqlClient.SqlCommand cmd;
            conn = new System.Data.SqlClient.SqlConnection(connectionstring);
            try
            {
                conn.Open();
                string Querystring = "SELECT RTRIM(INVNUMBER) INVNUMBER FROM OEINVH WHERE INVDATE>=20201001 ORDER BY INVDATE DESC ";
                cmd = new System.Data.SqlClient.SqlCommand(Querystring, conn);
                cmd.CommandTimeout = 180;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                using (System.Data.SqlClient.SqlDataAdapter sda = new System.Data.SqlClient.SqlDataAdapter())
                {
                    DataRow dr;
                    cmd.Connection = conn;
                    sda.SelectCommand = cmd;
                    dt = new DataTable();
                    sda.Fill(dt);
                    dr = dt.NewRow();
                    dt.Rows.InsertAt(dr, 0);
                    cmbxInvoice.DataSource = dt;
                    cmbxInvoice.ValueMember = "INVNUMBER";
                    cmbxInvoice.DisplayMember = "INVNUMBER";
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                listBox1.Items.Add(ex.Message);
                conn.Close();
            }
        }
        private void CRNList(int ss)
        {
            DataTable dt;
            string connectionstring = "Data Source=" + SERVERNAME + "; Initial Catalog=" + SAGEDB + "; User ID=" + SAA + "; Password=" + SAPSS + ";";
            System.Data.SqlClient.SqlConnection conn;
            System.Data.SqlClient.SqlCommand cmd;

            conn = new System.Data.SqlClient.SqlConnection(connectionstring);
            try
            {

                conn.Open();
                string Querystring = "select RTRIM(CRDNUMBER) CRDNUMBER from OECRDH where CRDDATE>=20201001 AND ADJTYPE=" + ss + "  ORDER BY CRDDATE DESC ";
                cmd = new System.Data.SqlClient.SqlCommand(Querystring, conn);
                cmd.CommandTimeout = 180;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                using (System.Data.SqlClient.SqlDataAdapter sda = new System.Data.SqlClient.SqlDataAdapter())
                {
                    DataRow dr;
                    cmd.Connection = conn;
                    sda.SelectCommand = cmd;
                    dt = new DataTable();
                    sda.Fill(dt);
                    dr = dt.NewRow();
                    dt.Rows.InsertAt(dr, 0);
                    cmbxInvoice.DataSource = dt;
                    cmbxInvoice.ValueMember = "CRDNUMBER";
                    cmbxInvoice.DisplayMember = "CRDNUMBER";
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                listBox1.Items.Add("Error in cdnote/dbtnote" + ex.Message);
                conn.Close();
            }
        }
		private void txtDistance_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))//&& e.KeyChar != '.'
            {
                e.Handled = true;
                MessageBox.Show("Please Enter only Numeric value!");
            }
        }
		        
    }
}
