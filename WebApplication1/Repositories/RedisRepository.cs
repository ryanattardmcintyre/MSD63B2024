using StackExchange.Redis;
using System.Diagnostics.Tracing;

namespace WebApplication1.Repositories
{
    public class RedisRepository
    {


        private IDatabase db;
        public RedisRepository()
        {
            //to configure the connectionstring
            try
            {
                var connection = ConnectionMultiplexer.Connect(
                    "redis-17265.c284.us-east1-2.gce.redns.redis-cloud.com:17265,password=");
                db = connection.GetDatabase();
            }
            catch (Exception ex)
            {
                //atm its not working
            }
        
        }


        /*
         * for the assignment its being asked to load the menus from the cache and show them in the navbar
         * public List<Menu> GetMenusFromCache()
         * {
         *  
         *      string menus = ...get menus from cache
         *      List<Menu> myMenus = JsonConvert.DeserializeObject<List<Menu>>("menus");
         *      return menus;
         * }
         * 
         * note: this is not being asked in the assignment >>>
         * optional:
         * public void SetMenus(List<Menu> menus)
         * {
         * }
        */

        //Read from the cache
        public int GetCounterInfo()
        {
            try
            {
                string counter = db.StringGet("counter");
                if (string.IsNullOrEmpty(counter))
                {
                    return 0;
                }
                else
                {
                    return Convert.ToInt32(counter);
                }
            }
            catch (Exception ex)
            {
                //log the error
                return 0;
            }
        
        }

        //Write in the cache
        public void IncrementCounter() {
        
            var counter = GetCounterInfo();
            counter++;
            db.StringSet("counter", counter);
        
        }
    }
}
