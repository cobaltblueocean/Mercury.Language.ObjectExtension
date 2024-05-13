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
    /// ChildObject Description
    /// </summary>
    public class ChildObject
    {
        public ChildObject(int key, double? value)
        {
            Key = key;
            Value = value;
            CreatedDateTime = DateTime.Now;
        }
        public int Key { get; set; }
        public double? Value { get; set; }

        [IgnoreObjectCompare]
        public DateTime CreatedDateTime { get; private set; }
    }
}
