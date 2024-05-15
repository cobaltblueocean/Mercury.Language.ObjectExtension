using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using Mercury.Language.ObjectExtension.Test.DummyObjects;
using Mercury.Language.ObjectExtension.Test.ProtoTypeObjects;
using System.Linq;

namespace Mercury.Language.ObjectExtension.Test
{
    [Parallelizable(ParallelScope.ContextMask)]
    public class ObjectExtensionTest
    {
        Random rnd = new Random();

        [Test]
        public void Test1()
        {
            var objectA = new ComplexObject();
            var objectB = new ComplexObject();
            var objectC = new ComplexObject();

            var data = new Dictionary<int, double?>();
            var values = new double?[] { 1, 0, 0.12345, 1.25 };
            for (int i = 0; i<5; i++)
            {
                data.Add(i, rnd.NextDouble());
            }

            var t1 = new Tuple<double?, double?>(3.2575342465753425, 112367173.16659279);
            var t2 = new Tuple<double?, double?>(3.2575342465753425, -141799015.05124027);

            objectA.Description = "Description of ObjectA";
            objectB.Description = "Description of ObjectB";
            objectC.Description = "Description of ObjectC";

            SampleReferenceObject ref1 = new SampleReferenceObject() { Name = "Sample" };
            TestReferenceObject ref2 = new TestReferenceObject() { Name = "Test" };
            TestReferenceObject ref3 = new TestReferenceObject() { Name = "Sample" };

            objectA.Id = 1;
            objectA.Name = "Dummy";
            objectA.Value = values;
            objectA.ReferenceObject = ref1;
            objectA.Function = new SimpleFunction();

            objectB.Id = 1;
            objectB.Name = "Dummy";
            objectB.Value = values;
            objectB.ReferenceObject = ref1;
            objectB.Function = new SimpleFunction();

            objectC.Id = 1;
            objectC.Name = "Dummy";
            objectC.Value = values;
            objectC.ReferenceObject = ref2;
            objectC.Function = new MultiplyFunction();

            foreach (var item in data)
            {
                objectA.AddChild(item.Key, item.Value);
                objectB.AddChild(item.Key, item.Value);
                objectC.AddChild(item.Key, item.Value);
            }

            objectA.AddItem(t1);
            objectA.AddItem(t2);

            objectB.AddItem(t1);
            objectB.AddItem(t2);

            objectC.AddItem(t1);
            objectC.AddItem(t2);

            ClassicAssert.IsTrue(objectA.HasValue());

            ClassicAssert.IsTrue(typeof(String).Equals(typeof(String)));
            ClassicAssert.IsFalse(typeof(String).Equals(typeof(Double)));

            ClassicAssert.IsTrue(objectA.AreObjectsEqual(objectB));
            ClassicAssert.IsFalse(objectA.AreObjectsEqual(objectC));

            objectB.ReferenceObject = ref3;
            ClassicAssert.IsFalse(objectB.AreObjectsEqual(objectC));

            objectB.ReferenceObject.Name = "Test";
            objectB.Function = new MultiplyFunction();

            ClassicAssert.IsTrue(objectB.Name.AreObjectsEqual(objectC.Name));
            ClassicAssert.IsTrue(objectB.Id.AreObjectsEqual(objectC.Id));

            ClassicAssert.IsTrue(objectB.AreObjectsEqual(objectC));
            ClassicAssert.IsFalse(objectA.AreObjectsEqual(objectC));

            double? dummy1 = null;
            double? dummy2 = 1234.5678;
            double dummy3 = 1234.5678;

            ClassicAssert.IsFalse(dummy1.HasValue());
            ClassicAssert.IsTrue(dummy2.HasValue());
            ClassicAssert.IsTrue(dummy3.HasValue());

            //Assert.Pass();
        }

        [Test]
        public void Test2()
        {
            var protoType = new ProtpTypeClass<ProtoTypeImplemebtedClass>();

            ClassicAssert.IsTrue(protoType.IsGenericParameterImplementType(typeof(ProtoTypeInterface1)));
            ClassicAssert.IsTrue(protoType.GetType().IsGenericParameterImplementType(typeof(ProtoTypeInterface1)));

            ClassicAssert.IsTrue(protoType.IsGenericParameterImplementBaseClass(typeof(ProtpTypeBaseClass)));
            ClassicAssert.IsTrue(protoType.GetType().IsGenericParameterImplementBaseClass(typeof(ProtpTypeBaseClass)));
        }

        [Test]
        public void Test3()
        {
            DateTime[] array1 = new DateTime[] { new DateTime(2012, 12, 15), new DateTime(2012, 10, 5), new DateTime(2011, 9, 1) };
            DateTime[] array2 = new DateTime[] { new DateTime(2012, 12, 15), new DateTime(2012, 10, 8), new DateTime(2011, 9, 1) };

            ClassicAssert.IsFalse(array1.AreObjectsEqual(array2));
        }

        [Test]
        public void Test4()
        {
            List<Type> listExcludeType = new List<Type>() { typeof(String), typeof(DateTime), typeof(DateTimeOffset) };
            var objectA = new ComplexObject();

            var data = new Dictionary<int, double?>();
            var values = new double?[] { 1, 0, 0.12345, 1.25 };
            for (int i = 0; i < 5; i++)
            {
                data.Add(i, rnd.NextDouble());
            }

            var t1 = new Tuple<double?, double?>(3.2575342465753425, 112367173.16659279);
            var t2 = new Tuple<double?, double?>(3.2575342465753425, -141799015.05124027);

            objectA.Description = "Description of ObjectA";

            SampleReferenceObject ref1 = new SampleReferenceObject() { Name = "Sample" };

            objectA.Id = 1;
            objectA.Name = "Dummy";
            objectA.Value = values;
            objectA.ReferenceObject = ref1;
            objectA.Function = new SimpleFunction();

            foreach (var item in data)
            {
                objectA.AddChild(item.Key, item.Value);
            }

            objectA.AddItem(t1);
            objectA.AddItem(t2);

            ClassicAssert.IsTrue(checkExists(objectA, listExcludeType));
            ClassicAssert.IsTrue(checkExists(10, listExcludeType));
            ClassicAssert.IsFalse(checkExists(objectA.Name, listExcludeType));
            ClassicAssert.IsFalse(checkExists(new DateTime(2012, 12, 15), listExcludeType));

            ClassicAssert.Pass();
        }

        private Boolean checkExists(Object objectA, List<Type> listExcludeType)
        {
            if (!objectA.GetType().IsPrimitive)
            {
                if (!listExcludeType.Any(x => x == objectA.GetType()))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
    }
}