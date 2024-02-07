﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Rinha2024.Data;

#nullable disable

namespace Rinha2024.Migrations
{
    [DbContext(typeof(AppDBContext))]
    [Migration("20240207025439_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Rinha2024.Model.Cliente", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("Limite")
                        .HasColumnType("integer");

                    b.Property<long>("Saldoinicial")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("Clientes");
                });

            modelBuilder.Entity("Rinha2024.Model.Transacao", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Descricao")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<int?>("IdCliente")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Realizada_em")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Tipo")
                        .IsRequired()
                        .HasMaxLength(1)
                        .HasColumnType("character varying(1)");

                    b.Property<long>("Valor")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("IdCliente");

                    b.ToTable("Transacoes");
                });

            modelBuilder.Entity("Rinha2024.Model.Transacao", b =>
                {
                    b.HasOne("Rinha2024.Model.Cliente", "Cliente")
                        .WithMany("Transacoes")
                        .HasForeignKey("IdCliente");

                    b.Navigation("Cliente");
                });

            modelBuilder.Entity("Rinha2024.Model.Cliente", b =>
                {
                    b.Navigation("Transacoes");
                });
#pragma warning restore 612, 618
        }
    }
}
