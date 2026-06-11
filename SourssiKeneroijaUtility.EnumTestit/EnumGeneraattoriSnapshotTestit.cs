
namespace SourssiKeneroijaUtility.EnumTestit
{
    
    public class EnumGeneratorSnapshotTests
    {
        [Fact]
        public Task GeneroiEnumLaajennuksetOikein()
        {
            // The source code to test
            var source = @"
                    using NetEscapades.EnumGenerators;

                    ";

            // Pass the source code to our helper and snapshot test the output
            return TestiApulainen.Verify(source);
        }
    }
}
