// <copyright file="PasswordService.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using System.Security.Cryptography;
using Beddin.Application.Common.Interfaces;

namespace Beddin.Infrastructure.Services
{
    /// <summary>
    /// Provides functionality for password management, including generating temporary passwords, hashing passwords, and verifying passwords.
    /// </summary>
    public class PasswordService : IPasswordService
    {
        private const string Lower = "abcdefghjkmnpqrstuvwxyz";
        private const string Upper = "ABCDEFGHJKMNPQRSTUVWXYZ";
        private const string Digits = "23456789";
        private const string Special = "!@#$%^&*";

        /// <inheritdoc/>
        public string GenerateTemporaryPassword(int length = 12)
        {
            var all = Lower + Upper + Digits + Special;
            var result = new char[length];
            result[0] = Pick(Lower);
            result[1] = Pick(Upper);
            result[2] = Pick(Digits);
            result[3] = Pick(Special);
            for (int i = 4; i < length; i++)
            {
                result[i] = Pick(all);
            }

            for (int i = length - 1; i > 0; i--)
            {
                int j = RandomNumberGenerator.GetInt32(i + 1);
                (result[i], result[j]) = (result[j], result[i]);
            }

            return new string(result);
        }

        /// <inheritdoc/>
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        /// <inheritdoc/>
        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        private static char Pick(string s) => s[RandomNumberGenerator.GetInt32(s.Length)];
    }
}
