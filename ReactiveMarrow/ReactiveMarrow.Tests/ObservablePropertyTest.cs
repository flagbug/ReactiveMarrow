using System;
using Xunit;

namespace ReactiveMarrow.Tests
{
    public class ObservablePropertyTest
    {
        [Fact]
        public void FailingSetterContractThrowsException()
        {
            var prop = new ObservableProperty<int>(x => x != 1);

            Assert.Throws<Exception>(() => prop.Value = 1);
        }

        [Fact]
        public void ValueGetterGetsSpecifiedValue()
        {
            var prop = new ObservableProperty<int>(() => 1);

            Assert.Equal(1, prop.Value);
        }

        [Fact]
        public void ValueSetterInvokesTransformation()
        {
            var prop = new ObservableProperty<int>(i => ++i) { Value = 1 };

            Assert.Equal(2, prop.Value);
        }
    }
}