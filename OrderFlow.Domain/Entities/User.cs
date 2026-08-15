using System;
using System.Collections.Generic;
using OrderFlow.Domain.Enums;
using OrderFlow.Domain.Exceptions;

namespace OrderFlow.Domain.Entities
{
    public class User : BaseEntity<Guid>
    {
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public string? DisplayName { get; private set; }

        public _UserRole RoleId { get; set; } = _UserRole.Client;
        public UserRole UserRole { get; set; }
        public ICollection<Order> Orders { get; set; } = [];

        public static User Create(
            string email,
            string passwordHash,
            string? displayName = null,
            Guid? createdBy = null)
        {
            ValidateEmail(email);
            ValidatePasswordHash(passwordHash);
            
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email.Trim(),
                PasswordHash = passwordHash,
                DisplayName = string.IsNullOrWhiteSpace(displayName) ? null : displayName.Trim()
            };
            user.CreateRecord(createdBy);

            return user;
        }
        
        // Partial update - updates only provided values (null means no change)
        public void UpdateDisplayName(string? displayName = null, Guid? modifiedBy = null)
        {
            if (displayName is not null)
            {
                var newDisplayName = string.IsNullOrWhiteSpace(displayName) ? null : displayName.Trim();
                if (!string.Equals(DisplayName, newDisplayName, StringComparison.Ordinal))
                {
                    DisplayName = newDisplayName;
                    TouchRecord(modifiedBy);
                }
            }
        }

        public void PromoteToAdmin(Guid userId, Guid? modifiedBy = null)
        {
            if (userId == Guid.Empty) throw new DomainValidationException("Invalid user id.");
            if (RoleId == _UserRole.Admin) throw new DomainValidationException("User is already an admin.");
            RoleId = _UserRole.Admin;
            TouchRecord(modifiedBy);
        }

        // Validation helpers
        private static void ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) throw new DomainValidationException("Email must not be empty.");
            var trimmed = email.Trim();
            if (!trimmed.Contains("@")) throw new DomainValidationException("Email is not valid.");
            if (trimmed.Length > 320) throw new DomainValidationException("Email is too long.");
        }

        private static void ValidatePasswordHash(string hash)
        {
            if (string.IsNullOrWhiteSpace(hash)) throw new DomainValidationException("Password hash must not be empty.");
            if (hash.Length > 1000) throw new DomainValidationException("Password hash is too long.");
        }
    }
}
