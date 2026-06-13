using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserStatus = table.Column<int>(type: "int", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserType = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DomainUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Credentials_Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Credentials_HashedPasswords = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Credentials_Salt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Credentials_MultiFactorTokens = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfileInfo_UserActualName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfileInfo_Bio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfileInfo_Avatar = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfileInfo_ThemePreference = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfileInfo_TopicsInterestedIn = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DomainUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserInteractions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInteractions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_UserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_UserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DomainUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subscriptions_DomainUsers_DomainUserId",
                        column: x => x.DomainUserId,
                        principalTable: "DomainUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WatchHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WatchHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WatchHistories_DomainUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "DomainUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleClaims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Channels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Profile_SubscribersCount = table.Column<int>(type: "int", nullable: false),
                    Profile_ChannelsDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Profile_Links = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Profile_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Profile_Avatar = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Profile_GreaterImg = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubscriptionsId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Channels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Channels_DomainUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "DomainUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Channels_Subscriptions_SubscriptionsId",
                        column: x => x.SubscriptionsId,
                        principalTable: "Subscriptions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Playlists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThumbnailUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Accessibility = table.Column<int>(type: "int", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: false),
                    ChannelId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChannelId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CustomPlaylist_OwnerId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OwnerId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DomainUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Playlists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Playlists_Channels_ChannelId1",
                        column: x => x.ChannelId1,
                        principalTable: "Channels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Playlists_DomainUsers_DomainUserId",
                        column: x => x.DomainUserId,
                        principalTable: "DomainUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Playlists_DomainUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "DomainUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Post",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChannelId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Accessibility = table.Column<int>(type: "int", nullable: false),
                    ChannelId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Post_Channels_ChannelId1",
                        column: x => x.ChannelId1,
                        principalTable: "Channels",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Videos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChannelId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Basics_VideoId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Basics_ThumbnailUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Basics_VideoUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Basics_PrivacyStatus = table.Column<int>(type: "int", nullable: false),
                    Descriptive_Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descriptive_Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descriptive_Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descriptive_Tags = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TechnicalDetails_Duration = table.Column<int>(type: "int", nullable: false),
                    TechnicalDetails_Resolution = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TechnicalDetails_FileSize = table.Column<long>(type: "bigint", nullable: false),
                    TechnicalDetails_ContainerFormat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TechnicalDetails_VideoCodec = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TechnicalDetails_AudioCodec = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TechnicalDetails_FrameRate = table.Column<float>(type: "real", nullable: false),
                    TechnicalDetails_BitRate = table.Column<int>(type: "int", nullable: false),
                    TemporalMetadata_UploadDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TemporalMetadata_UpateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TemporalMetadata_UploadStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Stats_WatchCount = table.Column<int>(type: "int", nullable: false),
                    Stats_LikesCount = table.Column<int>(type: "int", nullable: false),
                    Stats_DislikesCount = table.Column<int>(type: "int", nullable: false),
                    ChannelId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PlaylistId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WatchHistoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Videos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Videos_Channels_ChannelId1",
                        column: x => x.ChannelId1,
                        principalTable: "Channels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Videos_Playlists_PlaylistId",
                        column: x => x.PlaylistId,
                        principalTable: "Playlists",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Videos_WatchHistories_WatchHistoryId",
                        column: x => x.WatchHistoryId,
                        principalTable: "WatchHistories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommentId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuthorId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VideoId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParentCommentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    VideoId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Comments_ParentCommentId",
                        column: x => x.ParentCommentId,
                        principalTable: "Comments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Comments_Videos_VideoId1",
                        column: x => x.VideoId1,
                        principalTable: "Videos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Channels_OwnerId",
                table: "Channels",
                column: "OwnerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Channels_SubscriptionsId",
                table: "Channels",
                column: "SubscriptionsId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ParentCommentId",
                table: "Comments",
                column: "ParentCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_VideoId1",
                table: "Comments",
                column: "VideoId1");

            migrationBuilder.CreateIndex(
                name: "IX_Playlists_ChannelId1",
                table: "Playlists",
                column: "ChannelId1");

            migrationBuilder.CreateIndex(
                name: "IX_Playlists_DomainUserId",
                table: "Playlists",
                column: "DomainUserId",
                unique: true,
                filter: "[DomainUserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Playlists_UserId",
                table: "Playlists",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_ChannelId1",
                table: "Post",
                column: "ChannelId1");

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaims_RoleId",
                table: "RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Roles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_DomainUserId",
                table: "Subscriptions",
                column: "DomainUserId",
                unique: true,
                filter: "[DomainUserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId",
                table: "UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_UserId",
                table: "UserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_ChannelId1",
                table: "Videos",
                column: "ChannelId1");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_PlaylistId",
                table: "Videos",
                column: "PlaylistId");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_WatchHistoryId",
                table: "Videos",
                column: "WatchHistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_WatchHistories_OwnerId",
                table: "WatchHistories",
                column: "OwnerId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "Post");

            migrationBuilder.DropTable(
                name: "RoleClaims");

            migrationBuilder.DropTable(
                name: "UserClaims");

            migrationBuilder.DropTable(
                name: "UserInteractions");

            migrationBuilder.DropTable(
                name: "UserLogins");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "UserTokens");

            migrationBuilder.DropTable(
                name: "Videos");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Playlists");

            migrationBuilder.DropTable(
                name: "WatchHistories");

            migrationBuilder.DropTable(
                name: "Channels");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "DomainUsers");
        }
    }
}
