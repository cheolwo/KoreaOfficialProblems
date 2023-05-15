using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Quartz;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Xml;
using TestApp.Service.KoreaWarehouseInfo;

namespace TestApp.Service.KoreaPrice
{
    public class BusanUmgungAPIService
    {
        private readonly string baseUrl;
        private readonly string serviceKey;
        private readonly IMapper _mapper;
        private readonly BusanUmgungDbContext _busanUmgungDbContext;

        public BusanUmgungAPIService(IConfiguration configuration, IMapper mapper, BusanUmgungDbContext busanUmgungDbContext)
        {
            baseUrl = configuration.GetSection("BusanUmgungApiSettings:BaseUrl")?.Value ?? throw new ArgumentNullException(baseUrl);
            serviceKey = configuration.GetSection("BusanUmgungApiSettings:ServiceKey")?.Value ?? throw new ArgumentNullException(serviceKey);
            _mapper = mapper;
            _busanUmgungDbContext = busanUmgungDbContext;
        }
        static bool IsDuplicate(List<BusanUmgungInfo> itemList, BusanUmgungInfo newItem)
        {
            bool isDuplicate = itemList.Any(item =>
                item.midName == newItem.midName &&
                item.goodName == newItem.goodName &&
                item.danq == newItem.danq &&
                item.poj == newItem.poj &&
                item.sizeName == newItem.sizeName &&
                item.lv == newItem.lv &&
                item.saledate == newItem.saledate &&
                item.cmpName == newItem.cmpName &&
                item.largeName == newItem.largeName);;

            if (isDuplicate)
            {
                return true;
            }
            else return false;
        }
        public async Task CollectDailyCost(string pageNo, string numOfRows)
        {
            using HttpClient client = new();
            string apiUrl = BuildAPIUrlForInfo(pageNo, numOfRows);

            HttpResponseMessage response = await client.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                var Infos = DeserializeToNet(responseContent);
                var DBInfo = _busanUmgungDbContext.BusanUmgungInfos.ToList();
                foreach(var info in Infos)
                {                 
                    var model = _mapper.Map<BusanUmgungInfo>(info);
                    var isDuplicate = IsDuplicate(DBInfo, model);
                    if(isDuplicate) { continue; }
                    else
                    {
                        _busanUmgungDbContext.Add(model);
                        await _busanUmgungDbContext.SaveChangesAsync();
                    }
                }
            }
            else
            {
                throw new Exception("API 요청이 실패했습니다. 상태 코드: " + response.StatusCode);
            }
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
        public async Task<string> GetCostForInfo(string pageNo, string numOfRows)
        {
            using HttpClient client = new();
            string apiUrl = BuildAPIUrlForInfo(pageNo, numOfRows);

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
        private string BuildAPIUrlForInfo(string pageNo, string numofRows, string resultType="json")
        {
            var urlBuilder = new UriBuilder(baseUrl);
            urlBuilder.Query = $"ServiceKey={serviceKey}&pageNo={pageNo}&numOfRows={numofRows}&resultType={resultType}";
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
        public List<Item> DeserializeToNet(string json)
        {
            // JSON 데이터 매핑
            Root root = JsonSerializer.Deserialize<Root>(json) ?? throw new ArgumentNullException(json);

            return root.getDailyCost.body.items.item;
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
    public class CollectBusanUmgongInfoJob : IJob
    {
        private readonly BusanUmgungAPIService busanUmgungAPIService;
        public CollectBusanUmgongInfoJob(BusanUmgungAPIService busanUmgungAPIService)
        {
            this.busanUmgungAPIService = busanUmgungAPIService;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            await busanUmgungAPIService.CollectDailyCost("1", "300");
        }
    }
    public class BusanUmgungInfo
    {
        [Key]
        public string Id { get; set; }
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
    public class BusanUmgungProfile : Profile
    {
        public BusanUmgungProfile()
        {
            // Add as many of these lines as you need to map your objects
            CreateMap<Item, BusanUmgungInfo>();
            CreateMap<BusanUmgungInfo, Item>();
        }
    }
    public class BusanUmgungDbContext : DbContext
    {
        public BusanUmgungDbContext(DbContextOptions<BusanUmgungDbContext> options)
            : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BusanUmgungInfo>()
                .Property(e => e.Id)
                .ValueGeneratedOnAdd();
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    /*
        //     * 1. 법인명
        //     * 2. 일자
        //     * 3. 대분류
        //     * 4. 소분류,
        //     * 5. 중분류
        //     * 6. 등급,
        //     * 7. 사이즈
        //     * 8. 단위,
        //     * 9. 포장
        //     * 10. 수량
        //     */
        //    modelBuilder.Entity<BusanUmgungItem>()
        //        .HasKey(e => new { e.midName, e.goodName, e.sizeName, e.lv, e.poj, e.dan, e.danq, e.saledate, e.cmpName, e.largeName }); // 복합 키 설정

        public DbSet<BusanUmgungInfo> BusanUmgungInfos { get; set; }
    }
}

