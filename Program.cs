using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Northwind.WebApi.AutoMaper;
using Northwind.WebApi.Config;
using Northwind.WebApi.Converter;
using Northwind.WebApi.Model.Data;
using Northwind.WebApi.ModelsExt.Errors;
using Northwind.WebApi.ModelsExt.ModelAuthentic;
using Northwind.WebApi.ServiceExt;
using System.Configuration;
using System.Net.Mime;



var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 5715200;//file size for download 5.71MB
});
builder.Services.AddControllers(el =>
    {
        el.Filters.Add(new ApiExceptionFilter());
        el.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;

    }).AddJsonOptions(el =>
    {
        el.JsonSerializerOptions.DefaultIgnoreCondition =
            System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        el.JsonSerializerOptions.Converters.Add(new DateTimeConverter());

    });
builder.Services.AddMapProfilers();

var configure = builder.Configuration;
builder.Services.AddDbContext<NorthwindContext>(el => el.UseSqlServer(configure.GetConnectionString("Northwind")));
builder.Services.Configure<JWTSettings>(configure.GetSection("JWTSettings"));
builder.Services.Configure<MailConfiguration>(configure.GetSection("MailConfiguration"));
builder.Services.Configure<ComfirmEmail>(configure.GetSection("ComfirmEmail"));
builder.Services.AddResponseCompression(el =>
{
    el.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
    {
        MediaTypeNames.Application.Octet,
        MediaTypeNames.Application.Json,
        MediaTypeNames.Application.Xml,
        MediaTypeNames.Text.Html
    });
});
//to validate the token which has been sent by clients
builder.Services.AddJwtAuthentication(configure.GetSection("JWTSettings").Get<JWTSettings>());
builder.Services.RegisterSwagger();
builder.Services.AddCors(el =>
{
    el.AddDefaultPolicy(polDef =>
    {
        polDef.AllowAnyOrigin().AllowAnyHeader().AllowAnyHeader();
    });

});
//builder.Services.AddCors(opt =>
//{
//    opt.AddPolicy("IISPolicy", el =>
//    {
//        el.WithOrigins("http://localhost:552", "http://localhost:5025");
//    });
//});

var app = builder.Build();


if (app.Environment.IsDevelopment())
    app.UseExceptionHandler("/Error");

// Configure the HTTP request pipeline.
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(ui =>
{
    ui.SwaggerEndpoint("../swagger/v1.0/swagger.json", "Northwind API Endpoint");
});


//app.UseBlazorFrameworkFiles();
//app.UseStaticFiles();
app.UseCors();

//app.UseCors("IISPolicy");

app.MapControllers();
//app.MapFallbackToFile("index.html");

app.Run();
