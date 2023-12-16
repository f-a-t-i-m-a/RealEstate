﻿using System.Data.Entity.ModelConfiguration;
using JahanJooy.RealEstate.Domain.Audit;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations.Audit
{
    public class LoginNameQueryConfiguration : EntityTypeConfiguration<LoginNameQuery>
    {
        public LoginNameQueryConfiguration()
        {
            HasRequired(l => l.TargetUser)
                .WithMany()
                .HasForeignKey(l => l.TargetUserID)
                .WillCascadeOnDelete(true);

			HasRequired(l => l.Session)
				.WithMany()
				.HasForeignKey(l => l.SessionID)
				.WillCascadeOnDelete(false);

			HasRequired(l => l.ContactMethod)
				.WithMany()
				.HasForeignKey(l => l.ContactMethodID)
				.WillCascadeOnDelete(false);
        }
    }
}