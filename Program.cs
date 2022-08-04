using System.Text;
using System.Text.Json;
using System.Security.Cryptography;

var url = "https://sandbox.ipaymu.com/api/v2/payment"; //sandbox url (for development mode)
// var url = "https://my.ipaymu.com/api/v2/payment"; //production url
var va = "1179000899"; // your iPaymu VA
var apiKey = "QbGcoO0Qds9sQFDmY0MWg1Tq.xtuh1"; // your iPaymu API Key

var values = new Dictionary<string, string>();
values.Add("product", "Baju");
values.Add("qty", "1");
values.Add("price", "50000");
values.Add("returnUrl", "https://your-website/thank-you-page");
values.Add("notifyUrl", "https://your-website/callback-url");
values.Add("cancelUrl", "https://your-website/cancel-page");
values.Add("referenceId", "1234");

string[] productVal = new string[] { "T-Shirt", "Jacket", "Shoes" };
string[] qtyVal = new string[] { "1", "1", "2" };
string[] priceVal = new string[] { "100000", "250000", "350000" };

var model = new {
    product = productVal,
    qty = qtyVal,
    price = priceVal,
    notifyUrl = "https://your-website/callback-url",
    returnUrl = "https://your-website/thank-you-page",
    cancelUrl = "https://your-website/cancel-page",
    referenceId = "1234", //your reference id or transaction id
    buyerName = "Buyer Name", //optional
    buyerEmail = "buyer@mail.com", //optional
    buyerPhone = "08123123", //optional
};

var json = JsonSerializer.Serialize(model);
var RequestBody = ComputeSha256Hash(json);
var stringToSign = "POST:" + va + ":" + RequestBody + ":" + apiKey;

String signature = calcHmac(stringToSign, apiKey);

var data = new StringContent(json, Encoding.UTF8, "application/json");
using var client = new HttpClient();

client.DefaultRequestHeaders.Add("VA", va);
client.DefaultRequestHeaders.Add("signature", signature);

var response = await client.PostAsync(url, data);

string result = response.Content.ReadAsStringAsync().Result;
Console.WriteLine(result);


static string ComputeSha256Hash(string rawData)  
{  
    // Create a SHA256   
    using (SHA256 sha256Hash = SHA256.Create())  
    {  
        // ComputeHash - returns byte array  
        byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));  

        // Convert byte array to a string   
        StringBuilder builder = new StringBuilder();  
        for (int i = 0; i < bytes.Length; i++)  
        {  
            builder.Append(bytes[i].ToString("x2"));  
        }  
        return builder.ToString();  
    }  
}  

static string calcHmac( string data, string apiKey)
{
    byte[] key = Encoding.ASCII.GetBytes(apiKey);
    HMACSHA256 myhmacsha256 = new HMACSHA256(key);
    byte[] byteArray = Encoding.ASCII.GetBytes(data);
    MemoryStream stream = new MemoryStream(byteArray);
    string result = myhmacsha256.ComputeHash(stream).Aggregate("", (s, e) => s + String.Format("{0:x2}",e), s => s );
    return result;
}