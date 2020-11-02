#r "Newtonsoft.Json"

using System.Net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

public static async Task<object> Run(HttpRequest req, ILogger log)
{
    log.LogInformation("C# HTTP trigger function processed a request.");

    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
    log.LogInformation("Request body: " + requestBody);
    
    // Check HTTP basic authorization
    if (!Authorize(req, log))
    {
        log.LogWarning("HTTP basic authentication validation failed.");
        var response = new HttpResponseMessage(HttpStatusCode.BadRequest) {
                Content = new StringContent(JsonConvert.SerializeObject(new ResponseObject() {
                action =  "ShowBlockPage",
                userMessage = "Captcha verification failed due to invalid auth. Please contact the administrator to fix this issue.",
                code = "B2C004",
                status = 400,
                version = "1.0.0"
            }), Encoding.UTF8, "application/json")
        };
        string responseBody = await response.Content.ReadAsStringAsync();
        log.LogInformation("Response: " + responseBody);
        return response;
    }
    
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

private static bool Authorize(HttpRequest req, ILogger log)
{   
    // Get the environment's credentials 
    string username = System.Environment.GetEnvironmentVariable("BASIC_AUTH_USERNAME", EnvironmentVariableTarget.Process);
    string password = System.Environment.GetEnvironmentVariable("BASIC_AUTH_PASSWORD", EnvironmentVariableTarget.Process);

    // Returns authorized if the username is empty or not exists.
    if (string.IsNullOrEmpty(username))
    {
        log.LogInformation("HTTP basic authentication is not set.");
        return true;
    }

    // Check if the HTTP Authorization header exist
    if (!req.Headers.ContainsKey("Authorization"))
    {
        log.LogWarning("Missing HTTP basic authentication header.");
        return false;  
    }

    // Read the authorization header
    var auth = req.Headers["Authorization"].ToString();

    // Ensure the type of the authorization header id `Basic`
    if (!auth.StartsWith("Basic "))
    {
        log.LogWarning("HTTP basic authentication header must start with 'Basic '.");
        return false;  
    }

    // Get the the HTTP basic authorization credentials
    var cred = System.Text.UTF8Encoding.UTF8.GetString(Convert.FromBase64String(auth.Substring(6))).Split(':');

    // Evaluate the credentials and return the result
    return (cred[0] == username && cred[1] == password) ;
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
