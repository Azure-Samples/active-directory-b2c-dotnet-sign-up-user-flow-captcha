#r "Newtonsoft.Json"

using System.Net;
using System.Environment;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System.Text;

public static async Task<object> Run(HttpRequest req, ILogger log)
{
    log.LogInformation("C# HTTP trigger function processed a request.");

    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
    log.LogInformation("Request body: " + requestBody);
    dynamic data = JsonConvert.DeserializeObject(requestBody);
    string extension_Captchatext = data?.extension_aaebe32dff39461b940c8245c5b7dc33_Captchatext; //extension app-id
    bool verified_captcha = !string.IsNullOrEmpty(extension_Captchatext);

    using(var client = new HttpClient())
    {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        dictionary.Add("secret", System.Environment.GetEnvironmentVariable("SECRET_KEY"));
        dictionary.Add("response", extension_Captchatext);
        var formContent = new FormUrlEncodedContent(dictionary);

        var result = await client.PostAsync("https://www.google.com/recaptcha/api/siteverify", formContent);
        string resultContent = await result.Content.ReadAsStringAsync();
        log.LogInformation("Response from captcha service: " + resultContent);
        dynamic data_captcha = JsonConvert.DeserializeObject(resultContent);
        verified_captcha = data_captcha.success;
    }

    if (verified_captcha)
    {
        var response = new HttpResponseMessage(HttpStatusCode.OK) {
                Content = new StringContent(JsonConvert.SerializeObject(new ResponseObjectNoMessage()
                {
                    action = "Continue",
                    extension_Captchatext = ""
                }), Encoding.UTF8, "application/json")
        };
        string responseBody = await response.Content.ReadAsStringAsync();
        log.LogInformation("Response: " + responseBody);
        return response;
    }
    else
    {
        var response = new HttpResponseMessage(HttpStatusCode.BadRequest) {
                Content = new StringContent(JsonConvert.SerializeObject(new ResponseObject() {
                action =  "ShowBlockPage",
                userMessage = "Invalid captcha or captcha expired",
                code = "B2C003",
                status = 400,
                version = "1.0.0"
            }), Encoding.UTF8, "application/json")
        };
        string responseBody = await response.Content.ReadAsStringAsync();
        log.LogInformation("Response: " + responseBody);
        return response;
    }
}

public class ResponseObject
{
    public string action { get; set; }
    public string userMessage { get; set; }
    public string code { get; set; }
    public string version { get; set; }
    public int status { get; set; }
}

private class ResponseObjectNoMessage
{
    public string action { get; set; }
    public string extension_Captchatext { get; set; }
}
