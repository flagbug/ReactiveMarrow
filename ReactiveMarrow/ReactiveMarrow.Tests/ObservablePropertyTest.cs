using Xunit;

namespace ReactiveMarrow.Tests
{
    public class ObservablePropertyTest
    {
        [Fact]
        public void ValueSetterInvokesTransformation()
        {
            var prop = new ObservableProperty<int>(i => ++i) { Value = 1 };

            Assert.Equal(2, prop.Value);
        }
    }
}