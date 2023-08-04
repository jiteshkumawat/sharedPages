#r "Newtonsoft.Json"

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Web;
using System.Net.Http.Headers;
using System.Collections.Generic;
using Newtonsoft.Json;

public static async Task<IActionResult> Run(HttpRequest req, ILogger log)
    {
        try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");

                Dictionary<string, string> bodyData = new Dictionary<string, string>();
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                if (!string.IsNullOrEmpty(requestBody))
                {
                    string[] requestBodyParams = requestBody.Trim('\"').Split('&');
                    string appKey = string.Empty;
                    string stringurl = string.Empty;

                    foreach (string requestBodyParam in requestBodyParams)
                    {
                        string[] properties = requestBodyParam.Split('=');
                        if (!string.IsNullOrEmpty(properties[0]) && !string.IsNullOrEmpty(properties[1]))
                        {
                            bodyData.Add(HttpUtility.UrlDecode(properties[0]), HttpUtility.UrlDecode(properties[1]));
                            if (properties[0].Trim().ToLowerInvariant() == "appkey")
                            {
                                appKey = HttpUtility.UrlDecode(properties[1]);
                            }
                            else if (properties[0].Trim().ToLowerInvariant() == "tokenurl")
                            {
                                stringurl = HttpUtility.UrlDecode(properties[1]);
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(stringurl) && !string.IsNullOrEmpty(appKey))
                    {
                        using (HttpClient httpClient = new HttpClient())
                        {
                            httpClient.DefaultRequestHeaders.Add("AppKey", appKey);
                            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                            var requestData = new FormUrlEncodedContent(bodyData);
                            var response = await httpClient.PostAsync(stringurl, requestData);
                            var result = await response.Content.ReadAsStringAsync();
                            if (response.IsSuccessStatusCode)
                            {
                                return new JsonResult(JsonConvert.DeserializeObject(result));
                            }
                            else
                            {
                                return new BadRequestObjectResult(result);
                            }
                        }
                    }
                    return new BadRequestObjectResult("Token Url / App Key not present.");
                }

                return new BadRequestObjectResult("Request Body is not present.");
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
}
