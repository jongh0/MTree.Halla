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
    /// <summary>
    /// Gmail을 통해 메일을 보내려면 "보안 수준이 낮은 앱" 사용 허가 필요함
    /// https://www.google.com/settings/security/lesssecureapps
    /// </summary>
    public class EmailUtility
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static void SendEmail(string subject, string body, string attachFilePath = null)
        {
            try
            {
                using (var client = new SmtpClient())
                {
                    client.Host = "smtp.gmail.com";
                    client.Port = 587;
                    client.UseDefaultCredentials = false; // 시스템에 설정된 인증 정보를 사용하지 않는다
                    client.EnableSsl = true;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network; // 이걸 하지 않으면 Gmail에 인증을 받지 못한다
                    client.Credentials = new System.Net.NetworkCredential(Config.Instance.Email.UserId, Config.Instance.Email.UserPw);

                    using (var message = new MailMessage())
                    {
                        message.From = new MailAddress(Config.Instance.Email.UserId, "MTree", Encoding.UTF8);

                        foreach (var to in Config.Instance.Email.MailTo)
                        {
                            message.To.Add(new MailAddress(to));
                        }

                        message.Subject = subject;
                        message.SubjectEncoding = Encoding.UTF8;
                        message.Body = body;
                        message.BodyEncoding = Encoding.UTF8;

                        if (string.IsNullOrEmpty(attachFilePath) == false &&
                            File.Exists(attachFilePath) == true)
                        {
                            var contentType = new ContentType();
                            contentType.Name = Path.GetFileName(attachFilePath);
                            contentType.MediaType = MimeMapping.GetMimeMapping(contentType.Name);

                            message.Attachments.Add(new Attachment(attachFilePath, contentType));
                        }

                        client.Send(message);
                        logger.Info($"Email sent, <{message.Subject}> {message.Body}");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}
