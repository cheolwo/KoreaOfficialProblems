using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Quartz;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;

namespace TestApp.Service.KoreaWarehouseInfo
{
    public class WarehouseInfoDbContext : DbContext
    {
        public WarehouseInfoDbContext(DbContextOptions<WarehouseInfoDbContext> options)
            : base(options)
        {

        }

        public DbSet<WarehouseInfo> WarehouseInfoes { get; set; }
    }
    public class WarehouseInfo
    {
        public string PRESIDENT_NAME { get; set; }
        public string STORAGE_ITEM { get; set; }
        public string FROZEN_AREA { get; set; }
        public int RNUM { get; set; }
        public string COMPANY_NAME { get; set; }
        public string FROZEN_WING_COUNT { get; set; }
        public string GENERAL_WING_COUNT { get; set; }
        public string GENERAL_AREA { get; set; }
        public string COMPANY_TEL { get; set; }
        public string COMPANY_ADDRESS { get; set; }
        [Key]
        public string WARE_NO { get; set; }
        public string STORAGE_AREA { get; set; }
    }
    public class Root
    {
        public List<WarehouseInfo> items { get; set; }
    }
    public class WarehouseInfoAPIService
    {
        private readonly string baseUrl;
        private readonly string serviceKey;
        private readonly ILogger<WarehouseInfoAPIService> _logger;
        private readonly WarehouseInfoDbContext _warehouseInfoDbContext;

        public WarehouseInfoAPIService(IConfiguration configuration, ILogger<WarehouseInfoAPIService> logger, WarehouseInfoDbContext warehouseInfoDbContext)
        {
            baseUrl = configuration.GetSection("WarehouseApiSettings:BaseUrl")?.Value ?? throw new ArgumentNullException(baseUrl);
            serviceKey = configuration.GetSection("WarehouseApiSettings:ServiceKey")?.Value ?? throw new ArgumentNullException(serviceKey);
            _logger = logger;
            _warehouseInfoDbContext = warehouseInfoDbContext;
        }
        public async Task<string> GetWarehouseInfo(int pageNo, int numOfRows, string type, string startDate, string endDate)
        {
            string apiUrl = BuildApiUrl(baseUrl, serviceKey, pageNo, numOfRows, type, startDate, endDate);

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    // JSON 문자열을 JToken으로 파싱
                    JToken parsedJson = JToken.Parse(responseContent);
                    Console.WriteLine(parsedJson.ToString());   
                    return parsedJson.ToString();
                }
                else
                {
                    throw new Exception("API 요청이 실패했습니다. 상태 코드: " + response.StatusCode);
                }
            }
        }
        public async Task CollectWarehouseInfo(int pageNo, int numOfRows, string type, string startDate, string endDate)
        {
            string apiUrl = BuildApiUrl(baseUrl, serviceKey, pageNo, numOfRows, type, startDate, endDate);

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    var Infos = DeserializeToNet(responseContent);
                    foreach(var info in Infos)
                    {               
                        var value = _warehouseInfoDbContext.WarehouseInfoes.FirstOrDefault(e=>e.WARE_NO.Equals(info.WARE_NO));
                        if(value == null) 
                        {
                            // '(' 기준으로 문자열 자르기
                            int index = info.COMPANY_ADDRESS.IndexOf('(');
                            string result = index >= 0 ? info.COMPANY_ADDRESS.Substring(0, index) : info.COMPANY_ADDRESS;
                             info.COMPANY_ADDRESS = result;
                            _warehouseInfoDbContext.Add(info);
                            await _warehouseInfoDbContext.SaveChangesAsync(); 
                        }
                    }
                }
                else
                {
                    throw new Exception("API 요청이 실패했습니다. 상태 코드: " + response.StatusCode);
                }
            }
        }
        private string BuildApiUrl(string baseUrl, string serviceKey, int pageNo, int numOfRows, string type, string startDate, string endDate)
        {
            StringBuilder urlBuilder = new();
            urlBuilder.Append(baseUrl);
            urlBuilder.Append("?ServiceKey=").Append(serviceKey);
            urlBuilder.Append("&pageNo=").Append(pageNo);
            urlBuilder.Append("&numOfRows=").Append(numOfRows);
            urlBuilder.Append("&type=").Append(type);
            urlBuilder.Append("&sday=").Append(startDate);
            urlBuilder.Append("&eday=").Append(endDate);

            return urlBuilder.ToString();
        }
        public void Deserialize(string json)
        {
            // JSON 데이터 매핑
            Root root = JsonSerializer.Deserialize<Root>(json) ?? throw new ArgumentNullException(json);

            // 매핑된 데이터 출력
            foreach (var warehouseInfo in root.items)
            {
                Console.WriteLine("PRESIDENT_NAME: " + warehouseInfo.PRESIDENT_NAME);
                Console.WriteLine("STORAGE_ITEM: " + warehouseInfo.STORAGE_ITEM);
                Console.WriteLine("FROZEN_AREA: " + warehouseInfo.FROZEN_AREA);
                Console.WriteLine("RNUM: " + warehouseInfo.RNUM);
                Console.WriteLine("COMPANY_NAME: " + warehouseInfo.COMPANY_NAME);
                Console.WriteLine("FROZEN_WING_COUNT: " + warehouseInfo.FROZEN_WING_COUNT);
                Console.WriteLine("GENERAL_WING_COUNT: " + warehouseInfo.GENERAL_WING_COUNT);
                Console.WriteLine("GENERAL_AREA: " + warehouseInfo.GENERAL_AREA);
                Console.WriteLine("COMPANY_TEL: " + warehouseInfo.COMPANY_TEL);
                Console.WriteLine("COMPANY_ADDRESS: " + warehouseInfo.COMPANY_ADDRESS);
                Console.WriteLine("WARE_NO: " + warehouseInfo.WARE_NO);
                Console.WriteLine("STORAGE_AREA: " + warehouseInfo.STORAGE_AREA);
                Console.WriteLine();
            }
        }
        public List<WarehouseInfo> DeserializeToNet(string json)
        {
            // JSON 데이터 매핑
            Root root = JsonSerializer.Deserialize<Root>(json) ?? throw new ArgumentNullException(json);

            return root.items;
        }
    }
    public class CollectWarehouseInfoJob : IJob
    {
        private readonly WarehouseInfoAPIService WarehouseInfoAPIService;
        public CollectWarehouseInfoJob(WarehouseInfoAPIService warehouseInfoAPIService)
        {
            WarehouseInfoAPIService = warehouseInfoAPIService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
           await WarehouseInfoAPIService.CollectWarehouseInfo(1, 100, "json", "20200101", "20230101");
        }
    }
}

