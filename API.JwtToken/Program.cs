using API.JwtToken.Container;
using API.JwtToken.Helper;
using API.JwtToken.Modal;
using API.JwtToken.Repos;
using API.JwtToken.Repos.Models;
using API.JwtToken.Service;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<ICustomerService, CustomerService>();
builder.Services.AddTransient<IRefreshHandler, RefreshHandler>();
builder.Services.AddDbContext<dbContext>(db=>db.UseSqlServer(builder.Configuration.GetConnectionString("apicon")));

//Add Authentication Service here
//builder.Services.AddAuthentication("BasicAuthentication")
//    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

//Add JwtBearer Authentication service
var _authkey = builder.Configuration.GetValue<string>("JwtSettings:SecurityKey"); //get securityKey from appsettings.json

builder.Services.AddAuthentication(item =>
{
    item.DefaultAuthenticateScheme=JwtBearerDefaults.AuthenticationScheme;
    item.DefaultChallengeScheme=JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(item =>
{
    item.RequireHttpsMetadata=true;
    item.SaveToken = true;
    item.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authkey)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});


//declare the auto mapper
var autoMapper = new MapperConfiguration(item => item.AddProfile(new AutoMapperHandler()));
IMapper mapper = autoMapper.CreateMapper();
builder.Services.AddSingleton(mapper);

//Config CORS
builder.Services.AddCors(p => p.AddPolicy("corspolicy", build =>
{
    build.WithOrigins("*")
                .AllowAnyMethod().AllowAnyHeader();
}));

builder.Services.AddCors(p => p.AddPolicy("corspolicy1", build =>
{
    build.WithOrigins("https://domain3.com")
                .AllowAnyMethod().AllowAnyHeader();
}));

builder.Services.AddCors(p => p.AddDefaultPolicy(build =>
{
    build.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

//Define the Rate Limiting
builder.Services.AddRateLimiter(_=>_.AddFixedWindowLimiter(policyName: "fixedwindow", options =>
{
    options.Window = TimeSpan.FromSeconds(10);
    options.PermitLimit = 1;
    options.QueueLimit = 0;
    options.QueueProcessingOrder=System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
    /*
     * Đoạn code bạn cung cấp sử dụng để cấu hình một bộ giới hạn tốc độ (rate limiter) trong ứng dụng, cụ thể là sử dụng chiến lược "fixed window". Dưới đây là một số lý do tại sao nên sử dụng đoạn code này trong ứng dụng của bạn:

        Kiểm soát lưu lượng truy cập: Rate limiter giúp bạn kiểm soát số lượng yêu cầu mà một người dùng hoặc một hệ thống có thể thực hiện trong một khoảng thời gian nhất định. Điều này giúp ngăn chặn tình trạng quá tải và bảo vệ tài nguyên của ứng dụng.

        Bảo mật: Bằng cách giới hạn số lượng yêu cầu, bạn có thể giảm thiểu rủi ro từ các cuộc tấn công như DDoS (Distributed Denial of Service) hoặc các cuộc tấn công brute force.

        Cải thiện hiệu suất: Khi bạn giới hạn số lượng yêu cầu, bạn có thể cải thiện hiệu suất tổng thể của ứng dụng bằng cách đảm bảo rằng các tài nguyên không bị tiêu tốn quá mức vào các yêu cầu không cần thiết.

        Thiết lập chính sách rõ ràng: Đoạn code này cho phép bạn dễ dàng cấu hình các tùy chọn như khoảng thời gian (window), giới hạn số lượng yêu cầu (permit limit) và số lượng yêu cầu trong hàng đợi (queue limit), giúp bạn có thể tùy chỉnh theo nhu cầu cụ thể của ứng dụng.

        Sắp xếp xử lý yêu cầu: Tham số QueueProcessingOrder cho phép bạn xác định cách xử lý các yêu cầu trong hàng đợi, ở đây là xử lý theo thứ tự yêu cầu cũ nhất trước, điều này có thể hữu ích trong một số tình huống nhất định.
     */
}).RejectionStatusCode=401);

//Config Serilog here
string logPath = builder.Configuration.GetSection("Logging:Logpath").Value;
var _logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .MinimumLevel.Override("microsoft", Serilog.Events.LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                    .WriteTo.File(logPath)
                    .CreateLogger();
builder.Logging.AddSerilog(_logger);

//Register JwtSettings here 
var _jwtSetting = builder.Configuration.GetSection("JwtSettings");
//Regiter Jwt Sercive
builder.Services.Configure<JwtSettings>(_jwtSetting);

//after configuring this => need to go the CustomerService 

var app = builder.Build();

//Config Minimal API
app.MapGet("/minimalApi", () => "chuong");

app.MapGet("/getchannel", (string channelname) => "Wellcome to " + channelname).WithOpenApi(opt =>
{
    var parameter = opt.Parameters[0];
    parameter.Description = "Enter Channel Name:";
    return opt;
});

app.MapGet("/getCustomer", async (dbContext db) => {
    return await db.TblCustomers.ToListAsync();
});

app.MapGet("/getCustomer/{code}", async (dbContext db, string code) => {
    return await db.TblCustomers.FindAsync(code);
});

app.MapPost("/createCustomer", async (dbContext db, TblCustomer customer) => {
    await db.TblCustomers.AddAsync(customer);
    await db.SaveChangesAsync();
});

//Use Rate Limiting here
app.UseRateLimiter();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//enable MiddleWare

app.UseStaticFiles();//Enable this to read static files : Image,...
app.UseCors();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
