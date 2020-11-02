using OpenChat.Model;
using System;
using Xunit;

namespace OpenChat.Tests
{
    public class CredentialTests
    {
        [Fact]
        public void CanCreateCredential()
        {
            var credential = Credential.Create("Password");

            Assert.NotNull(credential);
        }

        [Fact]
        public void SucceedsWithSamePassword()
        {
            var password = "P4ssw0rd!";
            var credential = Credential.Create(password);

            Assert.True(credential.WithPassword(password));
        }

        [Fact]
        public void FailsWithDifferentPassword()
        {
            var password = "P4ssw0rd!";
            var credential = Credential.Create(password);

            Assert.False(credential.WithPassword(password + "-"));
        }

        [Fact]
        public void CantCreateCredentialWithEmptyPassword()
        {
            var exception = Assert.Throws<InvalidOperationException>(
                () => Credential.Create("")
            );

            Assert.Equal(Credential.MSG_CANT_CREATE_CREDENTIAL_WITH_EMPTY_PASSWORD, exception.Message);
        }
    }
}
