using MTree.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MTree.Utility
{
    public class EmailUtility
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static void SendEmail(string subject, string body, string attachFilePath = null)
        {
            SmtpClient client = null;
            MailMessage message = null;

            try
            {
                var userId = Config.Instance.Email.UserId;
                var userPw = Config.Instance.Email.UserPw;
                var mailTo = Config.Instance.Email.MailTo;

                if (string.IsNullOrEmpty(userId) == true || 
                    string.IsNullOrEmpty(userPw) == true ||
                    string.IsNullOrEmpty(mailTo) == true)
                {
                    logger.Error("Check Email configuration");
                    return;
                }

                client = new SmtpClient();
                client.Host = "smtp.gmail.com";
                client.Port = 587;
                client.UseDefaultCredentials = false; // 시스템에 설정된 인증 정보를 사용하지 않는다
                client.EnableSsl = true;
                client.DeliveryMethod = SmtpDeliveryMethod.Network; // 이걸 하지 않으면 Gmail에 인증을 받지 못한다
                client.Credentials = new System.Net.NetworkCredential(Config.Instance.Email.UserId, Config.Instance.Email.UserPw);

                var from = new MailAddress(userId, "MTree", Encoding.UTF8);
                var to = new MailAddress("jacking@dyon.co.kr");

                message = new MailMessage();
                message.From = new MailAddress(userId, "MTree", Encoding.UTF8);
                message.To.Add(new MailAddress(Config.Instance.Email.MailTo));
                message.Subject = subject;
                message.SubjectEncoding = Encoding.UTF8;
                message.Body = body;
                message.BodyEncoding = Encoding.UTF8;

                logger.Info($"<{subject}> {body}");

                if (string.IsNullOrEmpty(attachFilePath) == false &&
                    File.Exists(attachFilePath) == true)
                {
                    var contentType = new ContentType();
                    contentType.Name = Path.GetFileName(attachFilePath);
                    contentType.MediaType = MimeMapping.GetMimeMapping(contentType.Name);

                    message.Attachments.Add(new Attachment(attachFilePath, contentType));
                }

                client.SendAsync(message, "MTree");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                message?.Dispose();
                client?.Dispose();
            }
        }
    }
}
