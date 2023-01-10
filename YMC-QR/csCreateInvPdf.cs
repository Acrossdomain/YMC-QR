//using ACCPAC.Advantage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VELOCIS_EINV
{
    public class csCreateInvPdf
    {
        protected internal string USERNAME = null;
        protected internal string PASSWORD = null;
        protected internal string SAGEDB = null;
        protected internal string SERVERNAME = null;
        protected internal string SAA = null;
        protected internal string SAPSS = null;

        ReadWriteXML xml1 = new ReadWriteXML();
        public string SaveIRNNOByInvNo(string invn, String invtype, string irnno, String irnDate, string ewbno, string ewbdt, string ewbVdTo, string qrcode,string datatype)
        {
            String strreturn = "False";
            try
            {
                bool conStatus = xml1.ReadXML();
                if (conStatus == true)
                {
                    SERVERNAME = xml1.SERVERNAME;
                    SAGEDB = xml1.SAGEDB;
                    SAA = xml1.SAA;
                    SAPSS = xml1.SAPSS;
                }
                string connectionstring = "Data Source=" + SERVERNAME + "; Initial Catalog=" + SAGEDB + "; User ID=" + SAA + "; Password=" + SAPSS + ";";
                System.Data.SqlClient.SqlConnection conn;
                System.Data.SqlClient.SqlCommand cmd;
                conn = new System.Data.SqlClient.SqlConnection(connectionstring);
                conn.Open();
			
                string Querystring = "INSERT INTO OEIRNO VALUES ('" + invn + "','" + invtype.Substring(0, 1) + "','" + irnno + "','" + irnDate + "','" + ewbno + "','" + ewbdt + "','" + ewbVdTo + "','" + qrcode + "')"; //,'"+datatype+"'
				cmd = new System.Data.SqlClient.SqlCommand(Querystring, conn);
                cmd.CommandTimeout = 180;
                cmd.CommandType = CommandType.Text;
                int i = cmd.ExecuteNonQuery();
                strreturn = "TRUE";
            }
            catch (Exception ex)
            {
                string ss = ex.Message;
                strreturn = "FALSE";
            }

            return strreturn;
        }

        /*
        private Session session;
        private DBLink mDBLinkCmpRW;
        ReadWriteXML xml1 = new ReadWriteXML();
        public string SaveIRNNOByInvNo(string invn,String irnno,string invtype,string ewbno,string ewbdt,string ewbVdTo)
        {
            bool conStatus = xml1.ReadXML();
            if (conStatus == true)
            {
                SERVERNAME = xml1.SERVERNAME;
                SAGEDB = xml1.SAGEDB;
                SAA = xml1.SAA;
                SAPSS = xml1.SAPSS;
            }
            DataSet dt;
            string connectionstring = "Data Source=" + SERVERNAME + "; Initial Catalog=" + SAGEDB + "; User ID=" + SAA + "; Password=" + SAPSS + ";";
            System.Data.SqlClient.SqlConnection conn;
            System.Data.SqlClient.SqlCommand cmd;
            conn = new System.Data.SqlClient.SqlConnection(connectionstring);
            conn.Open();
            string Querystring = "INSERT INTO OEIRNO VALUES ('" + invn + "','" + invn + "','" + invn + "','" + invn + "','" + invn + "')";
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

 */
        public bool createinv(string invnumber, string irnNo, string report, string type)
        {
            /*
              string rptType="4";
              if (type != "CRDNOTE")
              {
                  if (type == "Original")
                      rptType = "1";
                  else if (type == "Duplicate")
                      rptType = "2";
                  else if (type == "Triplicate")
                      rptType = "3";
                  else
                      rptType = "4";
              }
              else rptType = "1";
              bool conStatus = xml1.ReadXML();
              if (conStatus == true)
              {
                  SAGEDB = xml1.SAGEDB;
                  USERNAME = xml1.USERNAME;
                  PASSWORD = xml1.PASSWORD;
              }
                // invnumber = "IN0000000051";
              try
              {
                  session = new Session();
                  session.Init("", "XY", "XY1000", "61A");
                  session.Open(USERNAME,PASSWORD, SAGEDB, DateTime.Today, 0);
                  mDBLinkCmpRW = session.OpenDBLink(DBLinkType.Company, DBLinkFlags.ReadWrite);
                  Report rpt;
                  try
                  {
                      if (report == "oecrn01-VEL-GSTQ")
                      {
                          rpt = session.ReportSelect("OECRN01[" + report + ".RPT]", "", " ");
                          rpt.SetParam("PRINTED", "1");             //' Report parameter: 4  
                          rpt.SetParam("QTYDEC", "4");//           ' Report parameter: 5 - O/E Sales History:Detail,Sort by Item Number
                          rpt.SetParam("SORTFROM", invnumber);//     ' Report parameter: 2 - limit by invoice
                          rpt.SetParam("SORTTO", invnumber);//        ' Report parameter: 3 - limit by invoice
                          rpt.SetParam("SWDELMETHOD", "3");//          ' Report parameter: 10
                          rpt.SetParam("ADJTYPE", type);//           ' Report parameter: 6
                          rpt.SetParam("SERIALLOTNUMBERS", "0");//    ' Report parameter: 14
                          rpt.SetParam("PRINTKIT", "0");//           ' Report parameter: 11
                          rpt.SetParam("PRINTBOM", "0");//         ' Report parameter: 12
                          rpt.SetParam("RETAINAGE", "0");//          ' Report parameter: 13
                         // rpt.SetParam("TEXT", rptType);
                          rpt.SetParam("IRNo", irnNo);//          ' Report parameter: 14
                         }
                      else
                      {
                          rpt = session.ReportSelect("OEINV01[" + report + ".RPT]", "", " ");
                          rpt.SetParam("PRINTED", "1");             //' Report parameter: 4
                          rpt.SetParam("DELMETHOD", "1");//           ' Report parameter: 6
                          rpt.SetParam("ECENABLED", "0");//         ' Report parameter: 7
                          rpt.SetParam("DIRECTEC", "0");//         ' Report parameter: 8
                          rpt.SetParam("QTYDEC", "4");//           ' Report parameter: 5 - O/E Sales History:Detail,Sort by Item Number
                          rpt.SetParam("BOITEM", "1");//          ' Report parameter: 9
                          rpt.SetParam("SORTFROM", invnumber);//     ' Report parameter: 2 - limit by invoice
                          rpt.SetParam("SORTTO", invnumber);//        ' Report parameter: 3 - limit by invoice
                          rpt.SetParam("SWDELMETHOD", "3");//          ' Report parameter: 10
                                                           //rpt.SetParam("CMPNAME", "valocis");//    ' Report parameter: 14
                          rpt.SetParam("SERIALLOTNUMBERS", "0");//    ' Report parameter: 14
                          rpt.SetParam("PRINTKIT", "0");//           ' Report parameter: 11
                          rpt.SetParam("PRINTBOM", "0");//         ' Report parameter: 12
                          rpt.SetParam("RETAINAGE", "0");//          ' Report parameter: 13
                          rpt.SetParam("TEXT", rptType);
                          rpt.SetParam("IRNo", irnNo);//          ' Report parameter: 14

                          rpt.SetParam("@SELECTION_CRITERIA", "(({OEINVH.INVNUMBER} >= " + invnumber + ") AND ({OEINVH.INVNUMBER} <= " + invnumber + ")) AND ({OEINVH.INVPRINTED} = 0)");  //' Report parameter: 0
                      }
                      rpt.NumberOfCopies = 1;
                      rpt.Destination = PrintDestination.File;
                      rpt.Format = PrintFormat.PDF;
                      rpt.Destination = PrintDestination.Preview;
                      MessageBox.Show("Successfully created, selected Invoice with QR. , Please wait while show preveiw of invoice !! ");

                      rpt.Print();


                       return true;
                  }
                  catch (Exception ex ){ String DDD=ex.Message;  return false;}
              }
              catch (Exception) { return false;}           
          }

        */
            return true;
        }
    }
}
