using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using SecuGen.FDxSDKPro.Windows;
using System.IO;

namespace ATM
{
    public partial class Finger : Form
    {
        SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename='F:\Finger + OTP\ATM\Database1.mdf';Integrated Security=True");
        
        private SGFingerPrintManager m_FPM;
        private bool m_LedOn = false;
        private Int32 m_ImageWidth;
        private Int32 m_ImageHeight;
        private Byte[] m_RegMin1;
        private Byte[] m_RegMin2;
        private Byte[] m_VrfMin;
        private SGFPMDeviceList[] m_DevList; // Used for EnumerateDevice
        string Name1 = "", Bal = "", BankAC = "", BankName = "", Pin = "";
        public Finger()
        {
            InitializeComponent();
        }

        
        private void button2_Click(object sender, EventArgs e)
        {
            SqlDataAdapter da = new SqlDataAdapter("Select Name,Bal,Temp,BankAC,BankName,Pin from Reg where BankName = '"+comboBox1.Text+"' AND Pin = '"+textBox6.Text+"'", con);
            DataSet ds = new DataSet();
            da.Fill(ds);
            if (ds.Tables[0].Rows.Count <= 0)
            {
                con.Close();
                MessageBox.Show("No Data Present", "Error !!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                
                Int32 iError;
                Byte[] fp_image;
                Int32 img_qlty;
                
                fp_image = new Byte[m_ImageWidth * m_ImageHeight];
                img_qlty = 0;

                iError = m_FPM.GetImage(fp_image);

                m_FPM.GetImageQuality(m_ImageWidth, m_ImageHeight, fp_image, ref img_qlty);


                if (iError == (Int32)SGFPMError.ERROR_NONE)
                {
                    DrawImage(fp_image, pictureBox1);
                    iError = m_FPM.CreateTemplate(fp_image, m_RegMin1);

                    if (iError == (Int32)SGFPMError.ERROR_NONE)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {

                            fp_image = (byte[])(ds.Tables[0].Rows[i][2]);

                            iError = m_FPM.CreateTemplate(fp_image, m_VrfMin);

                            MemoryStream ms = new MemoryStream();
                            bool matched1 = false;
                            SGFPMSecurityLevel secu_level;

                            secu_level = (SGFPMSecurityLevel)5;

                            iError = m_FPM.MatchTemplate(m_RegMin1, m_VrfMin, secu_level, ref matched1);

                            if (iError == (Int32)SGFPMError.ERROR_NONE)
                            {
                                if (matched1)
                                {
                                    Name1 = ds.Tables[0].Rows[i][0].ToString();
                                    Bal = ds.Tables[0].Rows[i][1].ToString();
                                    BankAC = ds.Tables[0].Rows[i][3].ToString();
                                    BankName = ds.Tables[0].Rows[i][4].ToString();
                                    Pin = ds.Tables[0].Rows[i][5].ToString();
                                }
                            }
                            else
                            {
                                DisplayError("MatchTemplate()", iError);
                            }
                        }
                    }
                }
                if (Name1 != "")
                {
                    comboBox1.Enabled = false;
                    textBox6.Enabled = false;
                    button2.Enabled = false;
                    panel1.Visible = true;
                }
                else
                {
                    comboBox1.Enabled = true;
                    textBox6.Enabled = true;
                    button2.Enabled = true;
                    panel1.Visible = false;
                    MessageBox.Show("Sorry No Match Found Please Re-Scan the Finger", "Error !!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Finger_Load(object sender, EventArgs e)
        {
            m_LedOn = false;

            m_RegMin1 = new Byte[400];
            m_RegMin2 = new Byte[400];
            m_VrfMin = new Byte[400];
            m_FPM = new SGFingerPrintManager();
            EnumerateBtn_Click(sender, e);
        }

        private void EnumerateBtn_Click(object sender, System.EventArgs e)
        {
            Int32 iError;
            string enum_device;

            // Enumerate Device
            iError = m_FPM.EnumerateDevice();

            // Get enumeration info into SGFPMDeviceList
            m_DevList = new SGFPMDeviceList[m_FPM.NumberOfDevice];

            if (m_FPM.NumberOfDevice == 0)
                return;

            //DisplayError("OpenDevice()", iError);

            m_DevList[0] = new SGFPMDeviceList();
            m_FPM.GetEnumDeviceInfo(0, m_DevList[0]);
            enum_device = m_DevList[0].DevName.ToString() + " : " + m_DevList[0].DevID;

            SGFPMDeviceName device_name;
            Int32 device_id;


            device_name = m_DevList[0].DevName;
            device_id = m_DevList[0].DevID;

            iError = m_FPM.Init(device_name);
            iError = m_FPM.OpenDevice(device_id);

            if (iError == (Int32)SGFPMError.ERROR_NONE)
            {
                GetBtn_Click(sender, e);
                //panel1.Visible = false;
            }
            else
            {

            }
        }

        private void GetBtn_Click(object sender, System.EventArgs e)
        {
            SGFPMDeviceInfoParam pInfo = new SGFPMDeviceInfoParam();
            Int32 iError = m_FPM.GetDeviceInfo(pInfo);

            if (iError == (Int32)SGFPMError.ERROR_NONE)
            {
                m_ImageWidth = pInfo.ImageWidth;
                m_ImageHeight = pInfo.ImageHeight;
                ASCIIEncoding encoding = new ASCIIEncoding();
            }
        }

        private void DrawImage(Byte[] imgData, PictureBox picBox)
        {
            int colorval;
            Bitmap bmp = new Bitmap(m_ImageWidth, m_ImageHeight);
            picBox.Image = (Image)bmp;

            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    colorval = (int)imgData[(j * m_ImageWidth) + i];
                    bmp.SetPixel(i, j, Color.FromArgb(colorval, colorval, colorval));
                }
            }
            picBox.Refresh();
        }


        void DisplayError(string funcName, int iError)
        {
            string text = "";

            switch (iError)
            {
                case 0:                             //SGFDX_ERROR_NONE				= 0,
                    text = "Error none";
                    break;

                case 1:                             //SGFDX_ERROR_CREATION_FAILED	= 1,
                    text = "Can not create object";
                    break;

                case 2:                             //   SGFDX_ERROR_FUNCTION_FAILED	= 2,
                    text = "Function Failed";
                    break;

                case 3:                             //   SGFDX_ERROR_INVALID_PARAM	= 3,
                    text = "Invalid Parameter";
                    break;

                case 4:                          //   SGFDX_ERROR_NOT_USED			= 4,
                    text = "Not used function";
                    break;

                case 5:                                //SGFDX_ERROR_DLLLOAD_FAILED	= 5,
                    text = "Can not create object";
                    break;

                case 6:                                //SGFDX_ERROR_DLLLOAD_FAILED_DRV	= 6,
                    text = "Can not load device driver";
                    break;
                case 7:                                //SGFDX_ERROR_DLLLOAD_FAILED_ALGO = 7,
                    text = "Can not load sgfpamx.dll";
                    break;

                case 51:                //SGFDX_ERROR_SYSLOAD_FAILED	   = 51,	// system file load fail
                    text = "Can not load driver kernel file";
                    break;

                case 52:                //SGFDX_ERROR_INITIALIZE_FAILED  = 52,   // chip initialize fail
                    text = "Failed to initialize the device";
                    break;

                case 53:                //SGFDX_ERROR_LINE_DROPPED		   = 53,   // image data drop
                    text = "Data transmission is not good";
                    break;

                case 54:                //SGFDX_ERROR_TIME_OUT			   = 54,   // getliveimage timeout error
                    text = "Time out";
                    break;

                case 55:                //SGFDX_ERROR_DEVICE_NOT_FOUND	= 55,   // device not found
                    text = "Device not found";
                    break;

                case 56:                //SGFDX_ERROR_DRVLOAD_FAILED	   = 56,   // dll file load fail
                    text = "Can not load driver file";
                    break;

                case 57:                //SGFDX_ERROR_WRONG_IMAGE		   = 57,   // wrong image
                    text = "Wrong Image";
                    break;

                case 58:                //SGFDX_ERROR_LACK_OF_BANDWIDTH  = 58,   // USB Bandwith Lack Error
                    text = "Lack of USB Bandwith";
                    break;

                case 59:                //SGFDX_ERROR_DEV_ALREADY_OPEN	= 59,   // Device Exclusive access Error
                    text = "Device is already opened";
                    break;

                case 60:                //SGFDX_ERROR_GETSN_FAILED		   = 60,   // Fail to get Device Serial Number
                    text = "Device serial number error";
                    break;

                case 61:                //SGFDX_ERROR_UNSUPPORTED_DEV		   = 61,   // Unsupported device
                    text = "Unsupported device";
                    break;

                // Extract & Verification error
                case 101:                //SGFDX_ERROR_FEAT_NUMBER		= 101, // utoo small number of minutiae
                    text = "The number of minutiae is too small";
                    break;

                case 102:                //SGFDX_ERROR_INVALID_TEMPLATE_TYPE		= 102, // wrong template type
                    text = "Template is invalid";
                    break;

                case 103:                //SGFDX_ERROR_INVALID_TEMPLATE1		= 103, // wrong template type
                    text = "1st template is invalid";
                    break;

                case 104:                //SGFDX_ERROR_INVALID_TEMPLATE2		= 104, // vwrong template type
                    text = "2nd template is invalid";
                    break;

                case 105:                //SGFDX_ERROR_EXTRACT_FAIL		= 105, // extraction fail
                    text = "Minutiae extraction failed";
                    break;

                case 106:                //SGFDX_ERROR_MATCH_FAIL		= 106, // matching  fail
                    text = "Matching failed";
                    break;

            }

            text = funcName + " Error # " + iError + " :" + text;
            MessageBox.Show(text, "Error !!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            deposit d = new deposit(Name1, Bal, BankAC, BankName, Pin);
            d.MdiParent = this.MdiParent;
            this.Close();
            d.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Withdraw w = new Withdraw(Name1, Bal, BankAC, BankName, Pin);
            w.MdiParent = this.MdiParent;
            this.Close();
            w.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            mini m = new mini(Name1, Bal, BankAC, BankName, Pin);
            m.MdiParent = this.MdiParent;
            this.Close();
            m.Show();
        }
    }
}
