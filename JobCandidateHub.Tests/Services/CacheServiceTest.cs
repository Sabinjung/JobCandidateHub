using Microsoft.Extensions.Caching.Memory;
using FluentAssertions;
using JobCandidateHub.Services;

namespace JobCandidateHub.Tests.Services
{
    public class CacheServiceTests
    {
        private readonly IMemoryCache _memoryCache;
        private readonly CacheService _cacheService;

        public CacheServiceTests()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _cacheService = new CacheService(_memoryCache);
        }

        [Fact]
        public void GetFromCache_WhenKeyExists_ReturnsValue()
        {
            // Arrange
            var key = "testKey";
            var expectedValue = "testValue";
            _memoryCache.Set(key, expectedValue);

            // Act
            var result = _cacheService.GetFromCache<string>(key);

            // Assert
            result.Should().Be(expectedValue);
        }

        [Fact]
        public void GetFromCache_WhenKeyDoesNotExist_ReturnsDefault()
        {
            // Arrange
            var key = "nonExistentKey";

            // Act
            var result = _cacheService.GetFromCache<string>(key);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void SetInCache_WithoutExpiration_StoresValue()
        {
            // Arrange
            var key = "testKey";
            var value = "testValue";

            // Act
            _cacheService.SetInCache(key, value);
            var result = _memoryCache.Get<string>(key);

            // Assert
            result.Should().Be(value);
        }

        [Fact]
        public void SetInCache_WithExpiration_StoresValueTemporarily()
        {
            // Arrange
            var key = "expiringKey";
            var value = "expiringValue";
            var expiration = TimeSpan.FromMilliseconds(100);

            // Act
            _cacheService.SetInCache(key, value, expiration);
            var immediateResult = _memoryCache.Get<string>(key);

            Thread.Sleep(200); // Wait for expiration
            var afterExpirationResult = _memoryCache.Get<string>(key);

            // Assert
            immediateResult.Should().Be(value);
            afterExpirationResult.Should().BeNull();
        }

        [Fact]
        public void SetInCache_WhenUpdatingExistingKey_UpdatesValue()
        {
            // Arrange
            var key = "updateKey";
            var initialValue = "initialValue";
            var updatedValue = "updatedValue";

            // Act
            _cacheService.SetInCache(key, initialValue);
            _cacheService.SetInCache(key, updatedValue);
            var result = _memoryCache.Get<string>(key);

            // Assert
            result.Should().Be(updatedValue);
        }

        [Fact]
        public void RemoveFromCache_WhenKeyExists_RemovesValue()
        {
            // Arrange
            var key = "testKey";
            var value = "testValue";
            _memoryCache.Set(key, value);

            // Act
            _cacheService.RemoveFromCache(key);
            var result = _memoryCache.Get<string>(key);

            // Assert
            result.Should().BeNull();
        }

        private class TestObject
        {
            public int Id { get; set; }
            public string? Name { get; set; }
        }

    }
}