// See https://aka.ms/new-console-template for more information
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.Json;
BusanUmgungAPI BusanUmgungAPI = new();
string result = await BusanUmgungAPI.GetCostForInfo(1, 100);
Console.WriteLine(result);
public class BusanUmgungAPI
{
    private readonly string baseUrl;
    private readonly string serviceKey;

    public BusanUmgungAPI()
    {
        baseUrl = "http://apis.data.go.kr/6260000/EgMarketCost/getDailyCost";
        serviceKey = "D0wkCvWHdCeJsYuU8A14KWl7mzOJ%2FiKbyKR%2F5xvnALYMf5wi5rcbCp2CXsx6xCsBhvgl5PJ8u%2Fwilufv%2FjhMcg%3D%3D";
    }

    public async Task<string> GetDailyCost(string pageNo, string numOfRows, string cmpName, string goodName, string midName, string largeName, string resultType)
    {
        using HttpClient client = new();
        string apiUrl = BuildApiUrl(pageNo, numOfRows, cmpName, goodName, midName, largeName, resultType);

        HttpResponseMessage response = await client.GetAsync(apiUrl);

        if (response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync();
            // JSON 문자열을 JToken으로 파싱
            JToken parsedJson = JToken.Parse(responseContent);
            return parsedJson.ToString();
        }
        else
        {
            throw new Exception("API 요청이 실패했습니다. 상태 코드: " + response.StatusCode);
        }
    }
    public async Task<string> GetCostForInfo(int pageNo, int numOfRows)
    {
        using HttpClient client = new();
        string apiUrl = BuildAPIUrlForInfo(pageNo, numOfRows);

        HttpResponseMessage response = await client.GetAsync(apiUrl);

        if (response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync();
            JToken parsedJson = JToken.Parse(responseContent);
            return parsedJson.ToString();
        }
        else
        {
            throw new Exception("API 요청이 실패했습니다. 상태 코드: " + response.StatusCode);
        }
    }
    private string BuildAPIUrlForInfo(int pageNo, int numofRows)
    {
        var urlBuilder = new UriBuilder(baseUrl);
        urlBuilder.Query = $"ServiceKey={serviceKey}&pageNo={pageNo}&numOfRows={numofRows}&resultType=json";
        return urlBuilder.ToString();
    }
    private string BuildApiUrl(string pageNo, string numOfRows, string cmpName, string goodName, string midName, string largeName, string resultType)
    {
        var urlBuilder = new UriBuilder(baseUrl);
        urlBuilder.Query = $"ServiceKey={serviceKey}&pageNo={pageNo}&numOfRows={numOfRows}&cmpName={cmpName}&goodName={goodName}&midName={midName}&largeName={largeName}&resultType={resultType}";

        return urlBuilder.ToString();
    }
    public void Deserialize(string json)
    {
        // JSON 데이터 매핑
        Root root = JsonSerializer.Deserialize<Root>(json) ?? throw new ArgumentNullException(json);
        PrintItems(root.getDailyCost.body.items.item);

    }
    static void PrintItems(List<Item> items)
    {
        foreach (var item in items)
        {
            Console.WriteLine("midName: " + item.midName);
            Console.WriteLine("goodName: " + item.goodName);
            Console.WriteLine("danq: " + item.danq);
            Console.WriteLine("dan: " + item.dan);
            Console.WriteLine("poj: " + item.poj);
            Console.WriteLine("sizeName: " + item.sizeName);
            Console.WriteLine("lv: " + item.lv);
            Console.WriteLine("minCost: " + item.minCost);
            Console.WriteLine("maxCost: " + item.maxCost);
            Console.WriteLine("aveCost: " + item.aveCost);
            Console.WriteLine("saledate: " + item.saledate);
            Console.WriteLine("cmpName: " + item.cmpName);
            Console.WriteLine("largeName: " + item.largeName);
            Console.WriteLine();
        }
    }
}
public class Item
{
    public string midName { get; set; }
    public string goodName { get; set; }
    public string danq { get; set; }
    public string dan { get; set; }
    public string poj { get; set; }
    public string sizeName { get; set; }
    public string lv { get; set; }
    public string minCost { get; set; }
    public string maxCost { get; set; }
    public string aveCost { get; set; }
    public string saledate { get; set; }
    public string cmpName { get; set; }
    public string largeName { get; set; }
}

public class Items
{
    public List<Item> item { get; set; }
}

public class Body
{
    public Items items { get; set; }
}

public class Header
{
    public string resultCode { get; set; }
    public string resultMsg { get; set; }
}

public class GetDailyCost
{
    public Header header { get; set; }
    public Body body { get; set; }
}

public class Root
{
    public GetDailyCost getDailyCost { get; set; }
}