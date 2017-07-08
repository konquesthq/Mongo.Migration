﻿using System;
using FluentAssertions;
using Mongo.Migration.Services.Interceptors;
using Mongo.Migration.Test.TestDoubles;
using NUnit.Framework;

namespace Mongo.Migration.Test.Services.Interceptors
{
    [TestFixture]
    internal class MigrationInterceptorProvider_when_get_serializer : IntegrationTest
    {
        [Test]
        public void When_entity_is_document_Then_provide_serializer()
        {
            // Arrange 
            var provider = _components.Get<MigrationInterceptorProvider>();

            // Act
            var serializer = provider.GetSerializer(typeof(TestDocument));

            // Assert
            serializer.ValueType.Should().Be(typeof(TestDocument));
        }

        [Test]
        public void When_entity_is_not_document_Then_provide_null()
        {
            // Arrange 
            var provider = _components.Get<MigrationInterceptorProvider>();

            // Act
            var serializer = provider.GetSerializer(typeof(TestClass));

            // Assert
            serializer.Should().BeNull();
        }
    }
}