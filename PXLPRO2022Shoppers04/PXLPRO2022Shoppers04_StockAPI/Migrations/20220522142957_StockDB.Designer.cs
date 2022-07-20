﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PXLPRO2022Shoppers04_StockAPI.Data;

namespace PXLPRO2022Shoppers04_StockAPI.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20220522142957_StockDB")]
    partial class StockDB
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.19")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("PXLPRO2022Shoppers04_StockAPI.Models.Stock", b =>
                {
                    b.Property<Guid>("SSN")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Amount")
                        .HasColumnType("int");

                    b.HasKey("SSN");

                    b.ToTable("Stock");
                });

            modelBuilder.Entity("PXLPRO2022Shoppers04_StockAPI.Models.StockLog", b =>
                {
                    b.Property<int>("SLID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Message")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("SSN")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("SLID");

                    b.ToTable("StockLog");
                });
#pragma warning restore 612, 618
        }
    }
}
