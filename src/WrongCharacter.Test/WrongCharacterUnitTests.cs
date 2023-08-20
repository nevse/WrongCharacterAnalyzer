using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = WrongCharacter.Test.CSharpCodeFixVerifier<
    WrongCharacter.WrongCharacterAnalyzer,
    WrongCharacter.WrongCharacterCodeFixProvider>;

namespace WrongCharacter.Test
{
    [TestClass]
    public class WrongCharacterUnitTest
    {
        //No diagnostics expected to show up
        [TestMethod]
        public async Task TestMethod1()
        {
            var test = @"";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task WrongClassNameTest()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class {|#0:TypыName|}
        {   
        }
    }";

            var fixtest = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TypyName
        {   
        }
    }";

            var expected = VerifyCS.Diagnostic("DXMAUI0001").WithLocation(0).WithArguments("TypыName");
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }
        [TestMethod]
        public async Task WrongNamespaceNameTest()
        {
            var test = @"
    namespace {|#0:TypыName|}
    {
        class Simple
        {   
        }
    }";

            var fixtest = @"
    namespace TypyName
    {
        class Simple
        {   
        }
    }";

            var expected = VerifyCS.Diagnostic("DXMAUI0001").WithLocation(0).WithArguments("TypыName");
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }
        [TestMethod]
        public async Task WrongMethodNameTest()
        {
            var test = @"
    namespace Some
    {
        class Simple
        {
            public void {|#0:TypыName|}()
            {
            }
        }
    }";

            var fixtest = @"
    namespace Some
    {
        class Simple
        {
            public void TypyName()
            {
            }
        }
    }";

            var expected = VerifyCS.Diagnostic("DXMAUI0001").WithLocation(0).WithArguments("TypыName");
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }
        [TestMethod]
        public async Task WrongFieldNameTest()
        {
            var test = @"
    namespace Some
    {
        class Simple
        {
            string {|#0:TypыName|};
        }
    }";

            var fixtest = @"
    namespace Some
    {
        class Simple
        {
            string TypyName;
        }
    }";

            var expected = VerifyCS.Diagnostic("DXMAUI0001").WithLocation(0).WithArguments("TypыName");
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }
        [TestMethod]
        public async Task WrongLocalNameTest()
        {
            var test = @"
    namespace Some
    {
        class Simple
        {
            public void SomeMethod()
            {
                string {|#0:TypыName|};
            }
        }
    }";

            var fixtest = @"
    namespace Some
    {
        class Simple
        {
            public void SomeMethod()
            {
                string TypyName;
            }
        }
    }";

            var expected = VerifyCS.Diagnostic("DXMAUI0001").WithLocation(0).WithArguments("TypыName");
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }
        [TestMethod]
        public async Task WrongLocalNameWithUsageTest()
        {
            var test = @"
    namespace Some
    {
        class Simple
        {
            public void SomeMethod()
            {
                string {|#0:TypыName|} = ""asf"";
                System.Console.WriteLine(TypыName);
            }
        }
    }";

            var fixtest = @"
    namespace Some
    {
        class Simple
        {
            public void SomeMethod()
            {
                string TypyName = ""asf"";
                System.Console.WriteLine(TypyName);
            }
        }
    }";

            var expected = VerifyCS.Diagnostic("DXMAUI0001").WithLocation(0).WithArguments("TypыName");
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }
    }
}
