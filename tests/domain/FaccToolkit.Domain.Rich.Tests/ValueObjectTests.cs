namespace FaccToolkit.Domain.Rich.Tests
{
    public class ValueObjectTests
    {
        public class TestValueObject : ValueObject
        {
            public string Property1 { get; }
            public int Property2 { get; }

            public TestValueObject(string property1, int property2)
            {
                Property1 = property1;
                Property2 = property2;
            }

            protected override IEnumerable<object> GetEqualityComponents()
            {
                yield return Property1;
                yield return Property2;
            }
        }

        [Fact]
        public void Equals_SameValues_ReturnsTrue()
        {
            // Arrange
            var valueObject1 = new TestValueObject("Test", 1);
            var valueObject2 = new TestValueObject("Test", 1);

            // Act
            bool result = valueObject1.Equals(valueObject2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_DifferentValues_ReturnsFalse()
        {
            // Arrange
            var valueObject1 = new TestValueObject("Test", 1);
            var valueObject2 = new TestValueObject("Test", 2);

            // Act
            bool result = valueObject1.Equals(valueObject2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void GetHashCode_SameValues_ReturnsSameHashCode()
        {
            // Arrange
            var valueObject1 = new TestValueObject("Test", 1);
            var valueObject2 = new TestValueObject("Test", 1);

            // Act
            int hashCode1 = valueObject1.GetHashCode();
            int hashCode2 = valueObject2.GetHashCode();

            // Assert
            Assert.Equal(hashCode1, hashCode2);
        }

        [Fact]
        public void GetHashCode_DifferentValues_ReturnsDifferentHashCode()
        {
            // Arrange
            var valueObject1 = new TestValueObject("Test", 1);
            var valueObject2 = new TestValueObject("Test", 2);

            // Act
            int hashCode1 = valueObject1.GetHashCode();
            int hashCode2 = valueObject2.GetHashCode();

            // Assert
            Assert.NotEqual(hashCode1, hashCode2);
        }

        [Fact]
        public void OperatorEquals_SameValues_ReturnsTrue()
        {
            // Arrange
            var valueObject1 = new TestValueObject("Test", 1);
            var valueObject2 = new TestValueObject("Test", 1);

            // Act
            bool result = valueObject1 == valueObject2;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void OperatorNotEquals_DifferentValues_ReturnsTrue()
        {
            // Arrange
            var valueObject1 = new TestValueObject("Test", 1);
            var valueObject2 = new TestValueObject("Test", 2);

            // Act
            bool result = valueObject1 != valueObject2;

            // Assert
            Assert.True(result);
        }
    }
}
