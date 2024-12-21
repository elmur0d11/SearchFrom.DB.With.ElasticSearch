using DotnetELK.Models;
using Microsoft.AspNetCore.Mvc;
using Nest;

namespace DotnetELK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        //MAQSADI   
        /*ProductsController sinfi quyidagilarni amalga oshiradi:
         1. GET so'rovi orqali mahsulotlarni qidirish.
         2. POST so'rovi orqali yangi mahsulot qo'shish.*/

        #region Constructor
        private readonly IElasticClient _elasticClient;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IElasticClient elasticClient, ILogger<ProductsController> logger)
        {
            _elasticClient = elasticClient;
            _logger = logger;
        }
        // _elasticClient Elasticsearch bilan bog'lanish uchun ishlatiladi.
        // _logger: Log yozuvlarini boshqarish uchun ishlatiladi.
        #endregion

        #region Get Products
        //Vazifasi:
        /*Kiritilgan kalit so'zga mos keladigan 
         mahsulotlarni Elasticsearch indeksidan qidiradi.*/
        [HttpGet(Name = "Get Products")]
        public async Task<IActionResult> Get(string keyword)
        {
            try
            {
                var results = await _elasticClient.SearchAsync<Product>(
                    s => s.Query(
                        q => q.QueryString(
                            d => d.Query('*' + keyword + '*')
                            )
                        ).Size(100)
                    );
                //SearchAsync: Elasticsearch'dagi Product indeksida qidiruvni amalga oshiradi.
                //s => s.Query(...): Qidiruv so'rovini aniqlaydi.
                //q => q.QueryString(...): Kalit so'z asosida matn qidiruvini amalga oshiradi.
                /*('*' + keyword + '*'): * belgisi kalit so'zdan oldin va keyin qo'shilib, 
                 to'liq mos kelmagan (partial match) qidiruvni amalga oshiradi.*/
                // .Size(1000): Maksimal 1000 ta natija qaytaradi.
                return Ok(results.Documents.ToList());
                //results.Documents.ToList(): Elasticsearch dan olingan mahsulotlarni ro'yxat sifatida qaytaradi.
                //Ok(...): HTTP 200 javob holati bilan natijani qaytaradi.
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        #endregion

        #region Add Product
        // Vazifasi:
        //Yangi mahsulotni Elasticsearch indeksiga qo'shadi.
        [HttpPost(Name = "AddProduct")]
        public async Task<IActionResult> Post(Product product)
        {
            try
            {
                await _elasticClient.IndexDocumentAsync(product);
                /*IndexDocumentAsync: Kiritilgan product obyektini Elasticsearch'ga 
                 indekslaydi (yangi yozuv qo'shadi).*/
                return Created("Product Created!", product);
                //Ok(): Agar mahsulot muvaffaqiyatli qo'shilsa, HTTP 200 holatini qaytaradi.
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
        #endregion
    }
}
