using Xunit;
using Yarn.Compiler;

namespace YarnSpinner.Tests
{
    public class TagTests : TestBase
    {
        public TagTests() : base()
        {
        }

        [Fact]
        void TestLineBeforeOptionsTaggedLastLine()
        {
            var source = "title:Start\n---\nline before options #line:1\n-> option 1\n-> option 2\n===\n";

            var result = Compiler.Compile(CompilationJob.CreateFromString("input", source));
            Assert.Empty(result.Diagnostics);

            var info = result.StringTable["line:1"];

            Assert.Contains("lastline",info.metadata);
        }

        [Fact]
        void TestLineNotBeforeOptionsNotTaggedLastLine()
        {
            var source = "title:Start\n---\nline not before options #line:0\nline before options #line:1\n-> option 1\n-> option 2\n===\n";

            var result = Compiler.Compile(CompilationJob.CreateFromString("input", source));
            Assert.Empty(result.Diagnostics);

            var info = result.StringTable["line:0"];

            Assert.DoesNotContain("lastline", info.metadata);
        }

        [Fact]
        void TestLineAfterOptionsNotTaggedLastLine()
        {
            var source = "title:Start\n---\nline before options #line:1\n-> option 1\n-> option 2\nline after options #line:2\n===\n";

            var result = Compiler.Compile(CompilationJob.CreateFromString("input", source));
            Assert.Empty(result.Diagnostics);

            var info = result.StringTable["line:2"];

            Assert.DoesNotContain("lastline", info.metadata);
        }

        [Fact]
        void TestNestedOptionLinesTaggedLastLine()
        {
            var source = CreateTestNode(@"
line before options #line:1
-> option 1
    line 1a #line:1a
    line 1b #line:1b
    -> option 1a
    -> option 1b
-> option 2
-> option 3
            ");

            var result = Compiler.Compile(CompilationJob.CreateFromString("input", source));
            Assert.Empty(result.Diagnostics);
            var info = result.StringTable["line:1"];
            Assert.Contains("lastline", info.metadata);

            info = result.StringTable["line:1b"];
            Assert.Contains("lastline", info.metadata);
        }

        [Fact]
        void TestIfInteriorLinesTaggedLastLine()
        {
            var source = CreateTestNode(@"
<<if true>>
line before options #line:0
-> option 1
-> option 2
<<endif>>
            ");

            var result = Compiler.Compile(CompilationJob.CreateFromString("input", source));
            Assert.Empty(result.Diagnostics);
            var info = result.StringTable["line:0"];
            Assert.Contains("lastline", info.metadata);
        }
    }
}
