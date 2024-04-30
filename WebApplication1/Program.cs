using Google.Cloud.SecretManager.V1;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Newtonsoft.Json.Linq;
using PdfSharp.Fonts;
using WebApplication1.Controllers;
using WebApplication1.Repositories;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            string keyFilePath = builder.Environment.ContentRootPath + "msd63b2024-e89abcf33d84.json";
            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", keyFilePath);
            
            
            string project = builder.Configuration["project"].ToString();
            var mySecrets = GetMySecrets(project); //mySecrets will be returned in json-format

            // Parse the JSON string into a JObject
            JObject jsonObject = JObject.Parse(mySecrets);

            // Access values by key
            string clientId = (string)jsonObject["Authentication:Google:ClientId"];
            string clientSecret = (string)jsonObject["Authentication:Google:ClientSecret"];


            builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
                })
                .AddCookie()
                .AddGoogle(options =>
                {
                    options.ClientId = clientId;//builder.Configuration["Authentication:Google:ClientId"];
                    options.ClientSecret = clientSecret; //builder.Configuration["Authentication:Google:ClientSecret"];
                });


            builder.Services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
                options.OnAppendCookie = cookieContext =>
                    SameSite.CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
                options.OnDeleteCookie = cookieContext =>
                     SameSite.CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
            });




            builder.Services.AddRazorPages();


       
            string bucket = builder.Configuration["bucket"].ToString();

            //this is where we inform the runtime, that there is a service called BlogsRepository/PostsRepository that needs to
            //be instantiated and so therefore we are registering them with a services collection here, meaning
            //that the injector class will know about their existence. Thus whenever they will be requested that instance can
            //be provided to the consumer class

            builder.Services.AddScoped(x => new BlogsRepository(project));
            builder.Services.AddScoped(x => new PostsRepository(project));
            builder.Services.AddScoped(x => new BucketsRepository(project, bucket));
            builder.Services.AddScoped(x => new PubSubRepository(project, "msd63b2024_ra"));
            builder.Services.AddScoped<IFontResolver, FileFontResolver>();
            builder.Services.AddScoped<RedisRepository>();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }


        public static string GetMySecrets(string projectId)
        {

            string secretId = "mysecrets";
            string secretVersionId = "1";
             
                // Create the client.
            SecretManagerServiceClient client = SecretManagerServiceClient.Create();

            // Build the resource name.
            SecretVersionName secretVersionName = new SecretVersionName(projectId, secretId, secretVersionId);

            // Call the API.
            AccessSecretVersionResponse result = client.AccessSecretVersion(secretVersionName);

            // Convert the payload to a string. Payloads are bytes by default.
            String payload = result.Payload.Data.ToStringUtf8();
            return payload; //json string
        }
    
    }
}