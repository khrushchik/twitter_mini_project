﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Thread_.NET.DAL.Context;

namespace Thread_.NET.DAL.Migrations
{
    [DbContext(typeof(ThreadContext))]
    partial class ThreadContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.3-servicing-35854")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Thread_.NET.DAL.Entities.Comment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("AuthorId");

                    b.Property<string>("Body");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<DateTime>("ModifiedOn");

                    b.Property<int?>("PostId");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("PostId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("Thread_.NET.DAL.Entities.Image", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedOn");

                    b.Property<DateTime>("ModifiedOn");

                    b.Property<string>("URL");

                    b.HasKey("Id");

                    b.ToTable("Images");
                });

            modelBuilder.Entity("Thread_.NET.DAL.Entities.LikeableEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedOn");

                    b.Property<DateTime>("ModifiedOn");

                    b.Property<int>("ParentId");

                    b.Property<int?>("ReactionId");

                    b.Property<int>("ReactionTo");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.HasIndex("ReactionId");

                    b.ToTable("LikeableEntities");
                });

            modelBuilder.Entity("Thread_.NET.DAL.Entities.Post", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("AuthorId");

                    b.Property<string>("Body");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<DateTime>("ModifiedOn");

                    b.Property<int?>("PreviewId");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("PreviewId");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("Thread_.NET.DAL.Entities.Reaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedOn");

                    b.Property<bool>("IsLike");

                    b.Property<DateTime>("ModifiedOn");

                    b.Property<int?>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Reaction");
                });

            modelBuilder.Entity("Thread_.NET.DAL.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("AvatarId");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("Email");

                    b.Property<DateTime>("ModifiedOn");

                    b.Property<string>("Password");

                    b.Property<string>("UserName");

                    b.HasKey("Id");

                    b.HasIndex("AvatarId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Thread_.NET.DAL.Entities.Comment", b =>
                {
                    b.HasOne("Thread_.NET.DAL.Entities.User", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId");

                    b.HasOne("Thread_.NET.DAL.Entities.Post", "Post")
                        .WithMany("Comments")
                        .HasForeignKey("PostId");
                });

            modelBuilder.Entity("Thread_.NET.DAL.Entities.LikeableEntity", b =>
                {
                    b.HasOne("Thread_.NET.DAL.Entities.Comment", "Comment")
                        .WithMany("Reactions")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Thread_.NET.DAL.Entities.Post", "Post")
                        .WithMany("Reactions")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Thread_.NET.DAL.Entities.Reaction", "Reaction")
                        .WithMany()
                        .HasForeignKey("ReactionId");
                });

            modelBuilder.Entity("Thread_.NET.DAL.Entities.Post", b =>
                {
                    b.HasOne("Thread_.NET.DAL.Entities.User", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId");

                    b.HasOne("Thread_.NET.DAL.Entities.Image", "Preview")
                        .WithMany()
                        .HasForeignKey("PreviewId");
                });

            modelBuilder.Entity("Thread_.NET.DAL.Entities.Reaction", b =>
                {
                    b.HasOne("Thread_.NET.DAL.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Thread_.NET.DAL.Entities.User", b =>
                {
                    b.HasOne("Thread_.NET.DAL.Entities.Image", "Avatar")
                        .WithMany()
                        .HasForeignKey("AvatarId");
                });
#pragma warning restore 612, 618
        }
    }
}
