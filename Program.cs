var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

// Diğer klasik servisler ve Swagger ekleniyor
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger arayüz ayarları
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Image Converter API V1");
    c.RoutePrefix = "swagger";
});

// Eklenen CORS politikasını uygulamaya "kullan" talimatı veriyoruz
app.UseCors();

app.UseAuthorization();
app.MapControllers();

app.Run();