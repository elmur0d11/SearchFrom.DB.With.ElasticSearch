using DotnetELK.Models;
using Nest;
using System.Runtime.CompilerServices;

namespace DotnetELK.Extensions
{
    public static class ElasticSearchExtensions
    {
        #region About
        //Sinfning MAQSADI
        /*ElasticSearchExtensions sinfi quyidagilarni amalga oshiradi:
          1. Elasticsearch mijozini sozlash va xizmatlar konteyneriga qo'shish.
          2. Model uchun standart xaritalash qoidalarini belgilash.
          3. Elasticsearch indeksini avtomatik yaratish.*/

        //METHODLAR
        //Vazifasi
        /*Elasticsearch ulanish sozlamalarini o'qish.
         Elasticsearch mijozini (ElasticClient) yaratish.
        Xizmatlar konteyneriga (IServiceCollection) Elasticsearch mijozini qo'shish.
        Standart indeksni yaratish.*/

        //PEARAMETRLAR
        //IServiceCollection services: Xizmatlar konteyneri.
        //IConfiguration configuration: Konfiguratsiya.
        #endregion

        #region AddElasticSearch
        //QANADAY ISHLAYDI
        public static void AddElasticSearch(this IServiceCollection services, IConfiguration configuration)
        {
            //Elasticsearch ulanish sozlamalari.
            var url = configuration["ELKConfiguration:Uri"];
            var defaultIndex = configuration["ELKConfiguration:index"];
            //URI - Elasticsearch serverining URL manzili.
            //index - Standart indeks nomi.

            //Elasticsearch sozlamalarini yaratish.
            var settings = new ConnectionSettings(new Uri(url))
                .PrettyJson()
                .DefaultIndex(defaultIndex);
            //PrettyJson() - JSON so'zlamalarini chiroyli formatda qaytaradi.
            //DefaultIndex(defaultIndex) - Standart indeksni belgilaydi.

            //Standart xaritalarni qo'shish.
            AddDefaultMappings(settings);
            /*Bu metod Product modelining ayrim xususiyatlarini (masalan, Price va Quantity) 
             indekslashdan chiqarib tashlaydi.*/

            //Elasticsearch mijozini yaratish.
            var client = new ElasticClient(settings);
            services.AddSingleton<IElasticClient>(client);
            //ElasticClient → Elasticsearch bilan ishlash uchun mijoz yaratadi.
            /*services.AddSingleton() →
            Mijozni ilova davomida bir marta yaratib, butun ilovada qayta ishlatadi.*/

            //Standart indeksni yaratish.
            CreateIndex(client, defaultIndex);
            //Bu metod standart indeksni yaratadi.
        }
        #endregion

        #region AddDefaultMappings
        //Vazifasi
        //Modelning ayrim xususiyatlarini indeksdan chiqarib tashlaydi.
        private static void AddDefaultMappings(ConnectionSettings settings)
        {
            //DefaultMappingFor<Product> yordamida Product modeliga xaritalash qoidalari qo'llanadi.
            /*Ignore() → Ko'rsatilgan xususiyatlar (Price va Quantity) Elasticsearch indeksida saqlanmaydi.*/
            settings.DefaultMappingFor<Product>(p =>
            p.Ignore(x => x.Price)
            .Ignore(x => x.Quantity));
        }
        #endregion

        #region CreateIndex
        //Vazifasi
        //Elasticsearch'da indeks yaratish.
        private static void CreateIndex(IElasticClient client, string indexName)
        {
            client.Indices.Create(indexName, i => i.Map<Product>(x => x.AutoMap()));
            //client.Indices.Create() → Yangi indeksni yaratadi.
            /*i.Map<Product>(x => x.AutoMap()) → Product modelining
            barcha xususiyatlarini avtomatik xaritalashni amalga oshiradi (AutoMap).*/
        }
        #endregion
    }

}
