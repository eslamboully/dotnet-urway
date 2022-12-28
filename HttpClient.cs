using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Newtonsoft.Json;
using NuGet.Packaging;

namespace DotnetURWay;

public class HttpClient : BaseService
{
    protected Dictionary<string, string> Attributes = new();
    private readonly IConfiguration _config;

    public HttpClient(IConfiguration config)
    {
        _config = config;
    }

    public HttpClient SetTrackId(string trackId)
    {
        Attributes["trackid"] = trackId;
        return this;
    }
    
    public HttpClient SetCustomerEmail(string email)
    {
        Attributes["customerEmail"] = email;
        return this;
    }
    
    public HttpClient SetCustomerIp(string ip)
    {
        Attributes["customerIp"] = ip;
        return this;
    }
    
    public HttpClient SetMerchantIp(string ip)
    {
        Attributes["merchantIp"] = ip;
        return this;
    }
    
    public HttpClient SetCurrency(string currency)
    {
        Attributes["currency"] = currency;
        return this;
    }
    
    public HttpClient SetCountry(string country)
    {
        Attributes["country"] = country;
        return this;
    }
    
    public HttpClient SetAmount(string amount)
    {
        Attributes["amount"] = amount;
        return this;
    }
    
    public HttpClient SetRedirectUrl(string url)
    {
        Attributes["udf2"] = url;
        return this;
    }
    
    public HttpClient SetAttributes(Dictionary<string, string> attributes)
    {
        Attributes = attributes;
        return this;
    }
    
    public HttpClient MergeAttributes(Dictionary<string, string> attributes)
    {
        Attributes.AddRange(attributes);
        return this;
    }
    
    public HttpClient SetAttribute(string key, string value)
    {
        Attributes[key] = value;
        return this;
    }
    
    public bool HasAttribute(string key)
    {
        return Attributes.ContainsKey(key);
    }
    
    public HttpClient RemoveAttribute(string key)
    {
        _ = Attributes.Remove(key);

        return this;
    }
    
    public async Task<Response> Pay()
    {
        // According to documentation we have to send the `terminal_id`, and `password` now.
        SetAuthAttributes();

        // We have to generate request
        GenerateRequestHash();
        
        using var client = new System.Net.Http.HttpClient();
        try
        {
            var json = JsonConvert.SerializeObject(Attributes);
            var attributes = new StringContent(json, Encoding.UTF8, "application/json");
            
            bool isProduction = Boolean.Parse(_config.GetSection("URWay")["isProduction"]);
            
            var response = await client.PostAsync(GetEndPointPath(isProduction), attributes);

            return new Response(await response.Content.ReadAsStringAsync());
        } catch (Exception ex) {
            throw new Exception(ex.Message);
        }
    }
    
    public async Task<Response> Find(string transactionId)
    {
        // According to documentation we have to send the `terminal_id`, and `password` now.
        SetAuthAttributes();

        // As requestHas for paying request is different from requestHash for find request.
        GenerateFindRequestHash();

        Attributes["transid"] = transactionId;

        using var client = new System.Net.Http.HttpClient();
        try
        {
            var json = JsonConvert.SerializeObject(Attributes);
            var attributes = new StringContent(json, Encoding.UTF8, "application/json");

            bool isProduction = Boolean.Parse(_config.GetSection("URWay")["isProduction"]);
            
            var response = await client.PostAsync(GetEndPointPath(isProduction), attributes);

            return new Response(await response.Content.ReadAsStringAsync());
        } catch (Exception ex) {
            throw new Exception(ex.Message);
        }
    }
    
    protected void GenerateRequestHash()
    {
        string requestHash = Attributes["trackid"] + '|' + _config.GetSection("URWay")["terminalId"] + '|' + 
                             _config.GetSection("URWay")["password"] + '|' + 
                             _config.GetSection("URWay")["merchantKey"] + '|' + 
                             Attributes["amount"] + '|' + Attributes["currency"];
        
        Attributes["requestHash"] = Sha256(requestHash);
        Attributes["action"] = "1";
    }
    
    protected void GenerateFindRequestHash()
    {
        string requestHash = Attributes["trackid"] + '|' + _config.GetSection("URWay")["terminalId"] + '|' + 
                             _config.GetSection("URWay")["password"] + '|' + 
                             _config.GetSection("URWay")["merchantKey"] + '|' + 
                             Attributes["amount"] + '|' + Attributes["currency"];
        
        Attributes["requestHash"] = Sha256(requestHash);
        Attributes["action"] = "10";
    }
    
    protected void SetAuthAttributes()
    {
        Attributes["terminalId"] = _config.GetSection("URWay")["terminalId"];
        Attributes["password"] = _config.GetSection("URWay")["password"];
    }
    
    static string Sha256(string randomString)
    {
        var crypt = new SHA256Managed();
        string hash = String.Empty;
        byte[] crypto = crypt.ComputeHash(Encoding.ASCII.GetBytes(randomString));
        foreach (byte theByte in crypto)
        {
            hash += theByte.ToString("x2");
        }
        return hash;
    }
}