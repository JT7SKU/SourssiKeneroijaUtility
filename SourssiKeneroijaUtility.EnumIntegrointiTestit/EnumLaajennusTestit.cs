namespace SourssiKeneroijaUtility.EnumIntegrointiTestit
{
    public class EnumLaajennusTestit
    {

        [Theory]
        [InlineData(Vari.Punanen)]
        [InlineData(Vari.Vihrea)]
        [InlineData(Vari.Vihrea | Vari.Sininen)]
        [InlineData((Vari)15)]
        [InlineData((Vari)0)]
        public void FastToStringIsSameAsToString(Vari vari)
        {
            var expected = vari.ToString();
            var actual = vari.ToStringFast();

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(Suunta.Ylos)]
        [InlineData((Suunta)15)]
        [InlineData((Suunta)0)]
        public void CustomExtensionNameToStringFast(Suunta value)
        {
            var expected = value.ToString();
            var actual = value.ToStringFast();

            Assert.Equal(expected, actual);
        }
    }
}
