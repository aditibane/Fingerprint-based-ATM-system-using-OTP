using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Xml;

namespace ATM
{
    public static class ClickSendSMS
    {
        static string _UserName = "";
        /// <summary>
        /// Get or set username.
        /// </summary>
        public static string UserName
        {
            get { return _UserName; }
            set { _UserName = value; }
        }

        static string _Key = "";
        /// <summary>
        /// get or set user key.
        /// </summary>
        public static string Key
        {
            get { return _Key; }
            set { _Key = value; }
        }

        static string _SMSTo = "";
        /// <summary>
        /// get or set only Destination number(s).
        /// </summary>
        public static string SMSTo
        {
            get { return _SMSTo; }
            set { _SMSTo = value; }
        }

        static string _Message = "";
        /// <summary>
        /// get or set message text
        /// </summary>
        public static string Message
        {
            get { return _Message; }
            set { _Message = value; }
        }

        static string _SenderID = "";
        public static string SenderID
        {
            get { return ClickSendSMS._SenderID; }
            set { ClickSendSMS._SenderID = value; }
        }

        static string _Schedule = "";
        public static string Schedule
        {
            get { return ClickSendSMS._Schedule; }
            set { ClickSendSMS._Schedule = value; }
        }


        /// <summary>
        /// Send single sms.
        /// </summary>
        /// <param name="strUserName">UserName</param>
        /// <param name="strKey">Key</param>
        /// <param name="strSMS">Destination number(s)</param>
        /// <param name="strMessage">SMS text</param>
        /// <param name="strSenderID">Sender ID</param>
        /// <param name="strSchedule">Schedule</param>
        /// <returns>Responds text with appropriate message</returns>
        public static SMSRespondsData SendSms(string strUserName, string strKey, string strSMS, string strMessage, string strSenderID, string strSchedule)
        {
            _UserName = strUserName;
            _Key = strKey;
            _SMSTo = strSMS;
            _Message = strMessage;
            _SenderID = strSenderID;
            _Schedule = strSchedule;
            return SendSms();
        }

        /// <summary>
        /// Send single sms.
        /// </summary>
        /// <returns>Responds text with appropriate message</returns>
        public static SMSRespondsData SendSms()
        {
            try
            {
                if (_UserName == "")
                    throw new Exception("Username can not be empty");
                if (_Key == "")
                    throw new Exception("Key can not be empty");
                if (_Message == "")
                    throw new Exception("Message can not be empty");
                if (_SMSTo == "")
                    throw new Exception("Number(s) can not be empty");

                WebClient wc = new WebClient();
                string sRequestURL;


                sRequestURL = "https://api.clicksend.com/http/v2/send.php?method=http&username=" + _UserName + "&key=" + _Key + "&to=" + _SMSTo + "&message=" + _Message;
                if (_SenderID != "")
                    sRequestURL += "&senderid=" + _SenderID;

                if (_Schedule != "")
                    sRequestURL += "&schedule=" + _Schedule;

                byte[] response = wc.DownloadData(sRequestURL);
                string sResult = Encoding.ASCII.GetString(response);
                return SetSMSData(sResult);
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        static SMSRespondsData SetSMSData(string _XMLResponds)
        {
            XmlDocument _Doc = new XmlDocument();
            _Doc.LoadXml(_XMLResponds);
            XmlNode _Node = _Doc.SelectSingleNode("xml");
            XmlNode _resultNode = _Node.SelectSingleNode(".//" + "result");

            SMSRespondsData _SMSRespondsData = new SMSRespondsData();
            _SMSRespondsData.RespondsCode = System.Convert.ToInt32(_resultNode.InnerText);
            _SMSRespondsData.RespondsText = RespondsCode.RespondsCodeToString(_resultNode.InnerText);
            _SMSRespondsData.SMSTo = _SMSTo;
            _SMSRespondsData.Message = _Message;
            _SMSRespondsData.Key = _Message;
            _SMSRespondsData.UserName = _UserName;
            return _SMSRespondsData;
        }
    }
}
