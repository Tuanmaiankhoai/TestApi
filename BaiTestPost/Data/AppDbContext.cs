using BaiTestPost.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace BaiTestPost.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext (DbContextOptions<AppDbContext> option) : base(option) { }
        public AppDbContext() { }
        public DbSet<Collection> collections { get; set; }
        public DbSet<ConfirmEmail> confirmEmails { get; set; }
        public DbSet<PostCollection> postCollections { get; set; }
        public DbSet<Post> posts { get; set; }
        public DbSet<PostStatus> postStatuses { get; set; }
        public DbSet<RefreshToken> refreshTokens { get; set; }
        public DbSet<Relationship> relationships { get; set; }       
        public DbSet<Report> reports { get; set; }
        public DbSet<Role> roles { get; set; }
        public DbSet<UserCommentPost> userCommentPosts { get; set; }
        public DbSet<UserLikeCommentOfPost> userLikeCommentOfPosts { get; set; }
        public DbSet<UserLikePost> userLikePosts { get; set;}
        public DbSet<User> users { get; set; }
        public DbSet<UserStatus> userStatuses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.NoAction;
            }
        }
    }
}
