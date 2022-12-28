using System.Text.Json;
using Newtonsoft.Json;

namespace DotnetURWay;

public class Response
{
    protected RequestAttributes Data;
    
    public Response(string response)
    {
        Data = JsonConvert.DeserializeObject<RequestAttributes>(response);
    }
    
    public string GetPaymentUrl()
    {
        if (!string.IsNullOrEmpty(Data.payid) && ! string.IsNullOrEmpty(Data.targetUrl)) {
            return Data.targetUrl + "?paymentid=" + Data.payid;
        }
    
        return "false";
    }
    
    public bool IsSuccess()
    {
        return Data.result == "Successful" && Data.responseCode == "000";
    }

    public bool IsFailure()
    {
        return !IsSuccess();
    }
}