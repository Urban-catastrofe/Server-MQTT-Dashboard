﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SimmeMqqt.EntityFramework;

#nullable disable

namespace SimmeMqqt.Migrations
{
    [DbContext(typeof(MachineData))]
    partial class MachineDataModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("SimmeMqqt.Model.MQTTMachineData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<bool>("Break")
                        .HasColumnType("bit");

                    b.Property<bool>("Failure")
                        .HasColumnType("bit");

                    b.Property<int>("IdealCyclus")
                        .HasColumnType("int");

                    b.Property<int>("MachineID")
                        .HasColumnType("int");

                    b.Property<string>("MachineName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.Property<int>("TotalGoodProduction")
                        .HasColumnType("int");

                    b.Property<int>("TotalProduction")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("MachineDatas");
                });
#pragma warning restore 612, 618
        }
    }
}
