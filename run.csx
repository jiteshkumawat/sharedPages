#r "Newtonsoft.Json"

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;
using System.IO;

public static async Task<IActionResult> Run(HttpRequest req, ILogger log)
    {
        try
        {
        using(HttpClient httpClient = new HttpClient()) {
        string appKey = req.Query["appkey"];
        string redirect_uri = req.Query["redirect_uri"];
        string tokenurl = req.Query["tokenurl"];
        string code = req.Query["code"];
        string client_id = req.Query["client_id"];
        string client_secret = req.Query["client_secret"];
        string grant_type = req.Query["grant_type"];
        httpClient.DefaultRequestHeaders.Add("AppKey", appKey);
        
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
        var queryString = req.QueryString.Value;
        var bodyData = new[] {
            new KeyValuePair<string, string>("redirect_uri", redirect_uri),
            new KeyValuePair<string, string>("grant_type", grant_type),
            new KeyValuePair<string, string>("code", code),
            new KeyValuePair<string, string>("client_id", client_id),
            new KeyValuePair<string, string>("client_secret", client_secret)
        };
        var requestData = new FormUrlEncodedContent(bodyData);
        var url = tokenurl + queryString;
        var response  = await httpClient.PostAsync(url, requestData);
        var result = await response.Content.ReadAsStringAsync();
        if(response.IsSuccessStatusCode){
            //throw new Exception(result);
            return new JsonResult(JsonConvert.DeserializeObject(result));
        }
        else{
            return new BadRequestObjectResult(result);
        }
        }
        }
        catch(Exception ex){
            return new BadRequestObjectResult(ex.Message);
            throw ex;
        }

    return new BadRequestObjectResult("Authorization Request Failed");
}