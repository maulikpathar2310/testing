using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace TheProvidersNest.Infrastructure
{
    public class Common
    {
        public static void SendEmail(string htmlString, string ToUser, string Subject)
        {
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress("lathiyav2810@gmail.com");
                message.To.Add(new MailAddress(ToUser));
                message.Subject = Subject;
                message.IsBodyHtml = true; //to make message body as html  
                message.Body = htmlString;
                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com"; //for gmail host  
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = new NetworkCredential("lathiyav2810@gmail.com", "Vijay@343511");
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
            }
            catch (Exception ex) { }
        }

		public static string DataTableToJsonWithStringBuilder(DataTable table)
		{
			var jsonString = new StringBuilder();
			if (table.Rows.Count > 0)
			{
				jsonString.Append("[");
				for (int i = 0; i < table.Rows.Count; i++)
				{
					jsonString.Append("{");
					for (int j = 0; j < table.Columns.Count; j++)
					{
						if (j < table.Columns.Count - 1)
						{
							jsonString.Append("\"" + table.Columns[j].ColumnName.ToString()
											  + "\":" + "\""
											  + table.Rows[i][j].ToString() + "\",");
						}
						else if (j == table.Columns.Count - 1)
						{
							jsonString.Append("\"" + table.Columns[j].ColumnName.ToString()
											  + "\":" + "\""
											  + table.Rows[i][j].ToString() + "\"");
						}
					}
					if (i == table.Rows.Count - 1)
					{
						jsonString.Append("}");
					}
					else
					{
						jsonString.Append("},");
					}
				}
				jsonString.Append("]");
			}
			return jsonString.ToString();
		}
	}
}