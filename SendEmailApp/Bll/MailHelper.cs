/*=============================================================
 * Send Mail Helper
 * Author ： Danny,Li
 * E-mail ： xing.dong.li@163.com
 * Edition： V-101014
 *=============================================================*/
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace SendEmailApp
{
    public class MailHelper
    {
        #region 从web.config中取得发送邮件参数
        //SMTP服务器
        public static string MailSmtpServer
        {
            get { return ConfigurationManager.AppSettings["mailSmtpServer"]; }
        }

        //系统发件地址
        public static string MailServerFrom
        {
            get { return ConfigurationManager.AppSettings["MailServerFrom"]; }
        }

        //系统调试时收件地址
        public static string DebugMail
        {
            get
            {
                if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["debugMail"]))
                    return string.Empty;
                else
                    return ConfigurationManager.AppSettings["debugMail"];
            }
        }
        #endregion

        /// <summary>
        /// 私有构造方法，不允许创建实例
        /// </summary>
        private MailHelper()
        {
            // TODO: Add constructor logic here
        }

        /// <summary>
        /// SendNetMail(须配置SMTP服务器地址)(多个收件人、抄送人、附件其参数用";"隔开,最后一个不能有";")
        /// </summary>
        /// <param name="mailFrom">发件人</param>
        /// <param name="mailTo">收件人(多个收件人用"；"隔开，最后一个不能有"；")</param>
        /// <param name="mailSubject">主题</param>
        /// <param name="mailBody">内容</param>
        /// <param name="mailAttch">附件（多个附件用"；"隔开，最后一个不能有"；"）</param>
        /// <param name="mailCode">密码（对加密过的）</param>
        /// <param name="mailPriority">优先级</param>
        /// <param name="mailCC">抄送(多个抄送人用"；"隔开，最后一个不能有"；")</param>
        /// <param name="resultMessage">输出信息</param>
        public static void SendNetMail(string mailFrom, string mailTo, string mailSubject, string mailBody, string mailAttch, string mailCode, string mailPriority, string mailCC, out string resultMessage)
        {
            //初始化输出参数
            resultMessage = "";
            //发件人和收件人不为空
            if (string.IsNullOrEmpty(mailFrom) || string.IsNullOrEmpty(mailTo))
            {
                resultMessage = "Please Fill Email Addresser Or Addressee . ";
                return;
            }

            System.Net.Mail.MailMessage email = new System.Net.Mail.MailMessage();
            System.Net.Mail.MailAddress emailFrom = new System.Net.Mail.MailAddress(mailFrom);
            //发件人
            email.From = emailFrom;
            //收件人
            if (string.IsNullOrEmpty(DebugMail))
            {
                string[] toUsers = mailTo.Split(';');
                foreach (string to in toUsers)
                    email.To.Add(to);
            }
            else
            {
                email.To.Add(DebugMail);
                mailSubject += "(MailTo " + mailTo + ")";
            }
            //抄送
            if (string.IsNullOrEmpty(DebugMail))
            {
                if (!string.IsNullOrEmpty(mailCC))
                {
                    string[] ccUsers = mailCC.Split(';');
                    foreach (string cc in ccUsers)
                        email.CC.Add(cc);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(mailCC))
                    mailSubject += "(MailCC " + mailCC + ")";
            }
            //主题
            email.Subject = mailSubject;
            //内容
            email.Body = mailBody;
            //附件
            if (!string.IsNullOrEmpty(mailAttch))
            {
                string[] attachments = mailAttch.Split(';');
                foreach (string file in attachments)
                {
                    System.Net.Mail.Attachment attach = new System.Net.Mail.Attachment(file, System.Net.Mime.MediaTypeNames.Application.Octet);
                    //为附件添加发送时间
                    System.Net.Mime.ContentDisposition disposition = attach.ContentDisposition;
                    disposition.CreationDate = System.IO.File.GetCreationTime(file);
                    disposition.ModificationDate = System.IO.File.GetLastWriteTime(file);
                    disposition.ReadDate = System.IO.File.GetLastAccessTime(file);
                    //添加附件
                    email.Attachments.Add(attach);
                }
            }
            //优先级
            email.Priority = (mailPriority == "High") ? System.Net.Mail.MailPriority.High : System.Net.Mail.MailPriority.Normal;
            //内容编码、格式
            email.BodyEncoding = System.Text.Encoding.UTF8;
            email.IsBodyHtml = true;
            //SMTP服务器
            System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient(MailSmtpServer);

            //验证（Credentials 凭证）
            client.Credentials = new System.Net.NetworkCredential(mailFrom, mailCode);

            //处理待发的电子邮件的方法 (Delivery 发送,传输)
            client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;

            try
            {
                //发送邮件
                client.Send(email);
                resultMessage = "Your E-mail Send Success .";
            }
            catch (Exception ex)
            {
                resultMessage = "Your E-mail Send Faile And Bring Error :" + ex.Message;
            }
        }

        /// <summary>
        /// SendNetMail（密码需配置）（多个收件人、抄送人、附件其参数用";"隔开,最后一个不能有";"）
        /// web.config配置如下(配置同<system.web>接点并列于同一级)：
        /// <system.net>
        ///     <mailSettings>
        ///            <smtp from="yangxiangwen789@163.com">
        ///                <network host="smtp.163.com" password="a1984c11d13" userName="yangxiangwen789" />
        ///         </smtp>
        ///     </mailSettings>
        /// </system.net>
        /// </summary>
        /// <param name="mailFrom">发件人</param>
        /// <param name="mailTo">收件人(多个收件人用"；"隔开，最后一个不能有"；")</param>
        /// <param name="mailSubject">主题</param>
        /// <param name="mailBody">内容</param>
        /// <param name="mailAttch">附件（多个附件用"；"隔开，最后一个不能有"；"）</param>
        /// <param name="mailPriority">优先级</param>
        /// <param name="mailCC">抄送(多个抄送人用"；"隔开，最后一个不能有"；")</param>
        /// <param name="resultMessage">输出信息</param>
        public static void SendNetMail(string mailFrom, string mailTo, string mailSubject, string mailBody, string mailAttch, string mailPriority, string mailCC, out string resultMessage)
        {
            //初始化输出参数
            resultMessage = "";
            //发件人和收件人不为空
            if (string.IsNullOrEmpty(mailFrom) || string.IsNullOrEmpty(mailTo))
            {
                resultMessage = "Please Fill Email Addresser Or Addressee . ";
                return;
            }

            System.Net.Mail.MailMessage email = new System.Net.Mail.MailMessage();

            //发件人
            email.From = new System.Net.Mail.MailAddress(mailFrom);
            //收件人
            if (string.IsNullOrEmpty(DebugMail))
            {
                string[] toUsers = mailTo.Split(';');
                foreach (string to in toUsers)
                    email.To.Add(to);
            }
            else
            {
                email.To.Add(DebugMail);
                mailSubject += "(MailTo " + mailTo + ")";
            }
            //抄送
            if (string.IsNullOrEmpty(DebugMail))
            {
                if (!string.IsNullOrEmpty(mailCC))
                {
                    string[] ccUsers = mailCC.Split(';');
                    foreach (string cc in ccUsers)
                        email.CC.Add(cc);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(mailCC))
                    mailSubject += "(MailCC " + mailCC + ")";
            }
            //主题
            email.Subject = mailSubject;
            //内容
            email.Body = mailBody;
            //优先级
            email.Priority = (mailPriority == "High") ? System.Net.Mail.MailPriority.High : System.Net.Mail.MailPriority.Normal;
            //附件
            if (!string.IsNullOrEmpty(mailAttch))
            {
                string[] attachments = mailAttch.Split(';');
                foreach (string file in attachments)
                {
                    System.Net.Mail.Attachment attach = new System.Net.Mail.Attachment(file, System.Net.Mime.MediaTypeNames.Application.Octet);
                    //为附件添加发送时间
                    System.Net.Mime.ContentDisposition disposition = attach.ContentDisposition;
                    disposition.CreationDate = System.IO.File.GetCreationTime(file);
                    disposition.ModificationDate = System.IO.File.GetLastWriteTime(file);
                    disposition.ReadDate = System.IO.File.GetLastAccessTime(file);
                    //添加附件
                    email.Attachments.Add(attach);
                }
            }
            //内容编码
            email.BodyEncoding = System.Text.Encoding.UTF8;
            email.IsBodyHtml = true;

            System.Net.Mail.SmtpClient sc = new System.Net.Mail.SmtpClient();
            try
            {
                //发送邮件
                sc.Send(email);
                resultMessage = "Your E-mail Send Success .";
            }
            catch (Exception ex)
            {
                resultMessage = "Your E-mail Send Faile And Bring Error :" + ex.Message;
            }
        }

    }
}
