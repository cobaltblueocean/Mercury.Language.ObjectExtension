// Copyright (c) 2017 - presented by Kei Nakai
//
// Please see distribution for license.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
//     
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercury.Language.ObjectExtension.Test.DummyObjects
{
    /// <summary>
    /// ComplexObject Description
    /// </summary>
    public class ComplexObject : IComplexObject
    {
        private List<ChildObject> _children;
        private Function<Double, Double> _function;
        private List<Tuple<double?, double?>> _list;


        public int Id { get; set; }
        public string Name { get; set; }
        public double?[] Value { get; set; }

        public Object Reference { get ; set; }

        public ChildObject[] Children { get { return _children.ToArray(); } }

        public List<Tuple<double?, double?>> ItemList { get { return _list; } }

        public Object ObsolatedReference
        {
            get { throw new NotImplementedException(); }
        }

        public Type RefType { get { return typeof(String); } }
        public IRefObject ReferenceObject { get; set; }
        public Function<Double, Double> Function { get; set; }
        
        [IgnoreObjectCompare]
        public string Description { get; set; }

        public Double? this[int index]
        {
            get
            {
                return Value[index];
            }
        }

        public ComplexObject()
        {
            _children = new List<ChildObject>();
            _list = new List<Tuple<double?, double?>>();
        }

        public void AddChild(int key, double? value)
        {
            _children.Add(new ChildObject(key, value));
        }

        public void AddRange(double?[] values)
        {
            throw new NotSupportedException();
        }

        public void AddItem(Tuple<double?, double?> item)
        {
            _list.Add(item);
        }
    }
}
