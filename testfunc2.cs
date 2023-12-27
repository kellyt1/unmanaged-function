using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Http.Json;
using System.Net.Mail;


namespace azure.function
{
    public class testfunc2
    {
        private readonly ILogger _logger;

        public testfunc2(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<testfunc2>();
        }

        [Function("testfunc2")]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            // dynamic data = JsonSerializer.Deserialize(requestBody);
            var data = string.IsNullOrEmpty(requestBody) ? null : JsonSerializer.Deserialize<EmailItem>(requestBody);

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString("Welcome to Azure Functions!" + data.From);

            emailsender();

            return response;
        }

        public void emailsender()
        {
            _logger.LogInformation("sending email");
            using (var client = new SmtpClient())
            {
                client.Host = "smtp.gmail.com";
                client.Port = 587;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential("kellyuwm01@gmail.com", "mboj sgsv tlkv dxqa");
                using (var message = new MailMessage(
                    from: new MailAddress("kellyuwm01@gmail.com", "YOUR NAME"),
                    to: new MailAddress("tossy.kelly@gmail.com", "THEIR NAME")
                    ))
                {

                    message.Subject = "Hello from Azure!";
                    message.Body = "Loremn ipsum dolor sit amet ...";

                    client.Send(message);
                }
                _logger.LogInformation("email sent");
            }
        }
    }
    public class EmailItem
    {
        public string From { get; set; }

        public Boolean Has_Attachment { get; set; }

        //Add other fields which you want, please note add all of the fields which your request may contains in this inner class.
    }
}
