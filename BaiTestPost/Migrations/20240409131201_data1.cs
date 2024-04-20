using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaiTestPost.Migrations
{
    /// <inheritdoc />
    public partial class data1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "postStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_postStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "userStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Avatar = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false),
                    UserStatusId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_users_roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "roles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_users_userStatuses_UserStatusId",
                        column: x => x.UserStatusId,
                        principalTable: "userStatuses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "collections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CollectionTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CollectionName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_collections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_collections_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "confirmEmails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ExpiredTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConfirmCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Confirmed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_confirmEmails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_confirmEmails_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "posts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    NumberOfLikes = table.Column<int>(type: "int", nullable: false),
                    NumberOfComments = table.Column<int>(type: "int", nullable: false),
                    PostStatusId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RemoveAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_posts_postStatuses_PostStatusId",
                        column: x => x.PostStatusId,
                        principalTable: "postStatuses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_posts_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "refreshTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiredTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_refreshTokens_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "relationships",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FollowerId = table.Column<int>(type: "int", nullable: false),
                    FollowingId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_relationships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_relationships_users_FollowerId",
                        column: x => x.FollowerId,
                        principalTable: "users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_relationships_users_FollowingId",
                        column: x => x.FollowingId,
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "postCollections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PostId = table.Column<int>(type: "int", nullable: false),
                    CollectionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_postCollections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_postCollections_collections_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "collections",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_postCollections_posts_PostId",
                        column: x => x.PostId,
                        principalTable: "posts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "reports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PostId = table.Column<int>(type: "int", nullable: false),
                    UserReportId = table.Column<int>(type: "int", nullable: false),
                    UserReportedId = table.Column<int>(type: "int", nullable: false),
                    ReportType = table.Column<int>(type: "int", nullable: false),
                    ReportingReason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_reports_posts_PostId",
                        column: x => x.PostId,
                        principalTable: "posts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_reports_users_UserReportId",
                        column: x => x.UserReportId,
                        principalTable: "users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_reports_users_UserReportedId",
                        column: x => x.UserReportedId,
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "userCommentPosts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PostId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumberOfLikes = table.Column<int>(type: "int", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RemoveAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userCommentPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_userCommentPosts_posts_PostId",
                        column: x => x.PostId,
                        principalTable: "posts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_userCommentPosts_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "userLikePosts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PostId = table.Column<int>(type: "int", nullable: false),
                    LikeTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Unlike = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userLikePosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_userLikePosts_posts_PostId",
                        column: x => x.PostId,
                        principalTable: "posts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_userLikePosts_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "userLikeCommentOfPosts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    UserCommentPostId = table.Column<int>(type: "int", nullable: false),
                    LikeTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Unlike = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userLikeCommentOfPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_userLikeCommentOfPosts_userCommentPosts_UserCommentPostId",
                        column: x => x.UserCommentPostId,
                        principalTable: "userCommentPosts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_userLikeCommentOfPosts_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_collections_UserId",
                table: "collections",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_confirmEmails_UserId",
                table: "confirmEmails",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_postCollections_CollectionId",
                table: "postCollections",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_postCollections_PostId",
                table: "postCollections",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_posts_PostStatusId",
                table: "posts",
                column: "PostStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_posts_UserId",
                table: "posts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_refreshTokens_UserId",
                table: "refreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_relationships_FollowerId",
                table: "relationships",
                column: "FollowerId");

            migrationBuilder.CreateIndex(
                name: "IX_relationships_FollowingId",
                table: "relationships",
                column: "FollowingId");

            migrationBuilder.CreateIndex(
                name: "IX_reports_PostId",
                table: "reports",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_reports_UserReportedId",
                table: "reports",
                column: "UserReportedId");

            migrationBuilder.CreateIndex(
                name: "IX_reports_UserReportId",
                table: "reports",
                column: "UserReportId");

            migrationBuilder.CreateIndex(
                name: "IX_userCommentPosts_PostId",
                table: "userCommentPosts",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_userCommentPosts_UserId",
                table: "userCommentPosts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_userLikeCommentOfPosts_UserCommentPostId",
                table: "userLikeCommentOfPosts",
                column: "UserCommentPostId");

            migrationBuilder.CreateIndex(
                name: "IX_userLikeCommentOfPosts_UserId",
                table: "userLikeCommentOfPosts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_userLikePosts_PostId",
                table: "userLikePosts",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_userLikePosts_UserId",
                table: "userLikePosts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_users_RoleId",
                table: "users",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_users_UserStatusId",
                table: "users",
                column: "UserStatusId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "confirmEmails");

            migrationBuilder.DropTable(
                name: "postCollections");

            migrationBuilder.DropTable(
                name: "refreshTokens");

            migrationBuilder.DropTable(
                name: "relationships");

            migrationBuilder.DropTable(
                name: "reports");

            migrationBuilder.DropTable(
                name: "userLikeCommentOfPosts");

            migrationBuilder.DropTable(
                name: "userLikePosts");

            migrationBuilder.DropTable(
                name: "collections");

            migrationBuilder.DropTable(
                name: "userCommentPosts");

            migrationBuilder.DropTable(
                name: "posts");

            migrationBuilder.DropTable(
                name: "postStatuses");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "userStatuses");
        }
    }
}
