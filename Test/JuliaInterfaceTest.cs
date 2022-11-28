using JULIAdotNET;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TestJuliaInterface
{
    public class JuliadotNET{

        [OneTimeSetUp]
        public void Setup(){
            Julia.Init();
            Julia.Eval(@"@netusing System
                         @netusing TestJuliaInterface");
        }

        [OneTimeTearDown] public void Destroy() => Julia.Exit(0);

        [Test]
        public void JuliaTypes() => Assert.AreNotEqual(IntPtr.Zero, (IntPtr)JLType.JLBool, "Julia Type Import Failure");

        [Test]
        public void JuliaFuns() => Assert.AreNotEqual(IntPtr.Zero, (IntPtr)JLFun.LengthF, "Julia Function Import Failure");

        [Test]
        public void JuliaEval() => Assert.AreEqual(4.0, (double)Julia.Eval("2.0 * 2.0"), "Julia Evaluation Failure");


        [Test]
        public void FunctionParamTest()
        {
            JLFun fun = Julia.Eval("t(x::Int) = Int32(x)");
            Assert.Multiple(() => {
                Assert.AreEqual((IntPtr)JLType.JLInt32, (IntPtr)fun.ReturnType, "Julia Function Return Type Failure");
                Assert.AreEqual((IntPtr)JLType.JLInt64, (IntPtr)fun.ParameterTypes[1], "Julia Function Parameter Type Failure");
            });
        }

        [Test]
        public void JuliaPinGC()
        {
            JLArray fun = Julia.Eval("[2, 3, 4]");
            var pinHandle = fun.Pin();
            pinHandle.Free();
        }


        [Test]
        public void Array()
        {
            JLArray inta = Julia.Eval("[2, 3, 4]"), doba = Julia.Eval("[2.0, 3.0, 4.0]");
            JLVal oa = new object[] { 2 }, ma = new object[,] { { 2, 3 }, { 3, 4 } };
            Assert.Multiple(() => {
                Assert.IsTrue(Enumerable.SequenceEqual(inta.UnboxArray<long>(), new long[] { 2, 3, 4 }), "Int Array copy failed");
                Assert.IsTrue(Enumerable.SequenceEqual(doba.UnboxArray<double>(), new double[] { 2, 3, 4 }), "Double Array copy failed");
                Assert.IsTrue(oa == Julia.Eval("[2]"), "Julia Array Box Conversion Failure");
                Assert.IsTrue(ma == Julia.Eval("[2 3; 3 4]"), "Julia Multiarray failure");
            });
        }

        [Test]
        public void SharpType() => Assert.IsFalse(((JLVal)JLType.SharpType).IsNull, "Julia Type Import Failure");

        [Test]
        public void Construction()
        {
            Assert.Multiple(() => {
                Assert.IsFalse(Julia.Eval(@"
                        item = T""ReflectionTestClass"".new[](3)
                        return item
                        ").IsNull, "Object instantiation failure");

                Assert.IsFalse(Julia.Eval(@"
                        itemG = T""ReflectionGenericTestClass`1"".new[T""System.Int64""](3)
                        return itemG
                        ").IsNull, "Generic Object instantiation failure");
            });
        }

        [Test]
        public void GetField()
        {
            if (!Julia.Eval("@isdefined itemG").UnboxBool())
                Construction();
            Assert.Multiple(() => {
                Assert.AreEqual(3, Julia.Eval("itemG.g[]").Value, "Failed to Get Field");
                Assert.AreEqual(5, Julia.Eval(@"T""ReflectionTestClass"".TestStaticField[]").Value);
            });
        }

        [Test]
        public void Method()
        {
            Assert.Multiple(() => {
                Assert.AreEqual(5, Julia.Eval(@"T""ReflectionTestClass"".StaticMethod[]()").Value);
                //Assert.AreEqual(3, Julia.Eval(@"T""ReflectionTestClass"".StaticGenericMethod[T""System.Int64""]()").Value);
            });
        }

        [Test]
        public void BoxingTest()
        {
            Assert.Multiple(() => {
                Assert.IsTrue((ulong)Julia.Eval("sharpbox(5)").Value == 5, "Boxing Failed");
            });
            //Assert.AreEqual(Julia.Eval("sharpunbox(T""ReflectionTestClass"".TestStaticField)"), 5, "Unboxing Failed")
        }

        [Test]
        public void UsingTest()
        {
            Assert.Multiple(() => {
                Julia.Eval(@"T""Int64""");
            });
        }

        [Test]
        public void TypeMacros() {
            Julia.Eval(@"P""System.Int64""");
            Assert.AreEqual(1, Julia.Eval("length(Reflection.TypeMap)").UnboxInt64(), "Did not push to type map");
            Julia.Eval(@"G""System.Int64""");
            Julia.Eval(@"R""System.Int64""");
            Assert.AreEqual(0, Julia.Eval("length(Reflection.TypeMap)").UnboxInt64(), "Did not push to type map");
        }

        [Test]
        public void SharpGC()
        {
            if (!Julia.Eval("@isdefined itemG").UnboxBool())
                Construction();
            Julia.Eval("handle = pin(itemG)");
            Julia.Eval("free(handle)");
        }

        [Test]
        public void SharpStreams(){
            //var i = new SharpInputStream();
            //var o = new SharpOutputStream();

            //i.Write("Test!");
        }
    }

    public class ReflectionTestClass{
        public long g;
        public static int TestStaticField = 5;
        public ReflectionTestClass(long g) { this.g = g; }
        public long InstanceMethod() => 5;
        public static long StaticMethod() => 5;
        public static long StaticGenericMethod<T>() => 3;
    }

    public class ReflectionGenericTestClass<T>
    {
        public T g;
        public ReflectionGenericTestClass(T g) { this.g = g; }
    }
}