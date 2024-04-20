using BaiTestPost.Data;
using BaiTestPost.Entities;
using BaiTestPost.Handler.Pagination;
using BaiTestPost.Payload.Converters.CommentPost;
using BaiTestPost.Payload.Converters.LikeComment;
using BaiTestPost.Payload.Converters.LikePost;
using BaiTestPost.Payload.Converters.PostCollectionConverter;
using BaiTestPost.Payload.Converters.PostConverter;
using BaiTestPost.Payload.Converters.Relationship;
using BaiTestPost.Payload.Converters.ReportTypeConverter;
using BaiTestPost.Payload.Converters.UserConverter;
using BaiTestPost.Payload.DataResponses.CommentPost;
using BaiTestPost.Payload.DataResponses.LikeComment;
using BaiTestPost.Payload.DataResponses.LikePost;
using BaiTestPost.Payload.DataResponses.Post;
using BaiTestPost.Payload.DataResponses.PostCollection;
using BaiTestPost.Payload.DataResponses.User;
using BaiTestPost.Payload.Responses;
using BaiTestPost.Services.Implement;
using BaiTestPost.Services.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel;
using System.Text;
using System.Text.Json.Serialization;
using CollectionConverter = BaiTestPost.Payload.Converters.PostCollectionConverter.CollectionConverter;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        builder.Services.AddControllers().AddJsonOptions(options =>

        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            options.JsonSerializerOptions.WriteIndented = true;
        });
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IPostService, PostService>();
        builder.Services.AddScoped<IPostCollectionService, PostCollectionService>();
        builder.Services.AddScoped<IUserLikePostService, LikePostService>();
        builder.Services.AddScoped<IUserCommentPost, CommentPostService>();
        builder.Services.AddScoped<IUserLikeCommentPostService, LikeCommentService>();
        builder.Services.AddScoped<IRelationshipService, RelationshipService>();


        builder.Services.AddScoped<AppDbContext>();
        builder.Services.AddScoped<UserConverter>();
        builder.Services.AddScoped<PostConverter>();
        builder.Services.AddScoped<CollectionConverter>();
        builder.Services.AddScoped <PostCollectionConverter >();
        builder.Services.AddScoped<LikePostConverter>();
        builder.Services.AddScoped<CommentPostConverter>();
        builder.Services.AddScoped<UpdateCommentPostConverter>();
        builder.Services.AddScoped<LikeCommentConverter>();
        builder.Services.AddScoped<RelationshipConverter>();
        builder.Services.AddScoped<ReportTypeConverter>();

        builder.Services.AddScoped<ResponseObject<DataResponse_Post>>();
        builder.Services.AddScoped<ResponseObject<DataResponse_User>>();
        builder.Services.AddScoped<ResponseObject<DataResponse_Token>>();
        builder.Services.AddScoped<ResponseObject<Data_Collection>>();
        builder.Services.AddScoped<ResponseObject<Data_PostCollection>>();
        builder.Services.AddScoped<ResponseObject<DataLikePost>>();
        builder.Services.AddScoped<ResponseObject<Data_CommentPostCreate>>();
        builder.Services.AddScoped<ResponseObject<Data_CommentPostUpdate>>();
        builder.Services.AddScoped<ResponseObject<Data_LikeComment>>();

       builder.Services.AddHttpContextAccessor();
        builder.Services.AddSession();
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddCors();
        builder.Services.AddHttpClient();
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = false;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateAudience = false,
                ValidateIssuer = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("AppSettings:SecretKey").Value!))
            };
        });
        builder.Services.AddDbContext<AppDbContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAllOrigins", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyHeader()
                       .AllowAnyMethod();
            });
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}