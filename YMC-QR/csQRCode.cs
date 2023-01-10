using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VEL_E_INV
{
    public class csQRCode
    {
        frm_ymc form1 = new frm_ymc();
        public bool createQrImage(string irn, string invnumber, string Qrcode12)
        {
            Boolean ss = true;
            if (Qrcode12 != "")
            {
                try
                {
                    // invnumber = "IN0000000051";
                    string strinvnumber = invnumber.Replace("/", "");
                    #region

                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(Qrcode12, QRCodeGenerator.ECCLevel.M);
                    
                    QRCode qrCode = new QRCode(qrCodeData);
                    Bitmap qrCodeImage = qrCode.GetGraphic(1);
                     var resultImage = new Bitmap(qrCodeImage.Width, qrCodeImage.Height); // 20 is bottom padding, adjust to your text
                     using (var graphics = Graphics.FromImage(resultImage))
                    using (var brush = new SolidBrush(Color.Black))
                     {
                        //graphics.Clear(Color.White);
                        graphics.DrawImage(qrCodeImage, new PointF());
                        // graphics.DrawString(code, font, brush, resultImage.Width / 2, resultImage.Height, format);
                    }
                    // Set the size of the PictureBox control.
                    form1.pictureBox1.Size = new System.Drawing.Size(145, 145);
                    //Set the SizeMode to center the image.
                    form1.pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
                    form1.pictureBox1.Image = resultImage;
					#endregion
                     //form1.pictureBox1.Image.Save(@"C:/E-Invoice/QR/" + strinvnumber + ".png", System.Drawing.Imaging.ImageFormat.Png);
                    form1.pictureBox1.Image.Save(@"QR/" + strinvnumber + ".png", System.Drawing.Imaging.ImageFormat.Png);
                    ss = true;
                }
                catch (Exception ex)
                {
                    string dd = ex.ToString();
                    ss= false;
                }
            }
            return ss;
        }
        public bool createQrImageBS64(string irn, string invnumber, string Qrcode12)
        {
            try
			{				
				string Qrcode123 = Qrcode12.ToString().Replace("data:image/png;base64,", "");
				// invnumber = "IN0000000051";
				//MessageBox.Show(invnumber + Qrcode123);
				string strinvnumber = invnumber.Replace("/", "");
                #region
                byte[] bytes = Convert.FromBase64String(Qrcode123.ToString());

                Image image;
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    image = Image.FromStream(ms);
                }
                #endregion                
                image.Save(@"QR/" + strinvnumber + ".png", System.Drawing.Imaging.ImageFormat.Png);
                //image.Save(@"C:/E-Invoice/QR/" + strinvnumber + ".png", System.Drawing.Imaging.ImageFormat.Png);
                //form1.pictureBox1.Image.Save(@"C:/E-Invoice/QR/" + strinvnumber + ".png", System.Drawing.Imaging.ImageFormat.Png);
                //form1.pictureBox1.Image.Save(@"QR/" + strinvnumber + ".png", System.Drawing.Imaging.ImageFormat.Png);
				return true;
            }
            catch (Exception ex)
            {
                string dd = ex.ToString();
				//MessageBox.Show(dd);
				return false;
            }
        }
    }
}
