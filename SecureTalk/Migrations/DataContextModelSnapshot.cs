﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SecureTalk;

namespace SecureTalk.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("SecureTalk.Models.Contact", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("ExchangeFingerprint")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("UserId");

                    b.ToTable("Contacts");
                });

            modelBuilder.Entity("SecureTalk.Models.Message", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("DecryptedRecipientId")
                        .HasColumnType("TEXT");

                    b.Property<string>("DecryptedSenderId")
                        .HasColumnType("TEXT");

                    b.Property<string>("KeyFingerprint")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("MessageText")
                        .IsRequired()
                        .HasColumnType("BLOB");

                    b.Property<byte[]>("RecipientId")
                        .IsRequired()
                        .HasColumnType("BLOB");

                    b.Property<string>("SenderFingerprint")
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("SenderId")
                        .IsRequired()
                        .HasColumnType("BLOB");

                    b.Property<byte[]>("SenderText")
                        .HasColumnType("BLOB");

                    b.Property<DateTime>("Sent")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("DecryptedSenderId");

                    b.HasIndex("SenderId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("SecureTalk.Models.MessageKey", b =>
                {
                    b.Property<string>("Fingerprint")
                        .HasColumnType("TEXT");

                    b.Property<string>("AssignedContactId")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsExchange")
                        .HasColumnType("INTEGER");

                    b.Property<string>("PrivatePem")
                        .HasColumnType("TEXT");

                    b.Property<string>("PublicPem")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Fingerprint");

                    b.HasIndex("AssignedContactId");

                    b.ToTable("MessageKeys");
                });

            modelBuilder.Entity("SecureTalk.Models.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Active")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ExchangeFingerprint")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ExchangePem")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("PEMPath")
                        .HasColumnType("TEXT");

                    b.Property<string>("PreferredName")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("SecureTalk.Models.MessageKey", b =>
                {
                    b.HasOne("SecureTalk.Models.Contact", "Contact")
                        .WithMany()
                        .HasForeignKey("AssignedContactId");

                    b.Navigation("Contact");
                });
#pragma warning restore 612, 618
        }
    }
}
