// Copyright (c) 2017 - presented by Kei Nakai
//
// Original project is developed and published by OpenGamma Inc.
//
// Copyright (C) 2012 - present by OpenGamma Inc. and the OpenGamma group of companies
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
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Runtime.CompilerServices;
using Mercury.Language.Properties;

namespace Mercury.Language
{
    /// <summary>
    /// Resource Manager for Culture dependent message context
    /// </summary>
    internal class LocalizedResources : INotifyPropertyChanged
    {
        #region Singleton Class Implementation
        private readonly Resources resources = new Resources();

        private static LocalizedResources instance;

        /// <summary>
        /// Constructor implement as Singleton pattern
        /// </summary>
        private LocalizedResources()
        {
        }

        /// <summary>
        /// Return singleton instance
        /// </summary>
        /// <returns>Return current instance</returns>
        public static LocalizedResources Instance()
        {
            if (instance == null)
                instance = new LocalizedResources();

            return instance;
        }

        /// <summary>
        /// Hangling culture changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Change resource culture change
        /// </summary>
        /// <param name="name"></param>
        public void ChangeCulture(string name)
        {
            Resources.Culture = CultureInfo.GetCultureInfo(name);
            RaisePropertyChanged("Resources");
        }

        /// <summary>
        /// Get resource
        /// </summary>
        internal Resources Resources
        {
            get { return resources; }
        }

        #endregion

        public String CANNOT_COMPARE_VALUES
        {
            get { return Properties.Resources.CANNOT_COMPARE_VALUES; }
        }

        public String CANNOT_COMPARE_PROPERTY
        {
            get { return Properties.Resources.CANNOT_COMPARE_PROPERTY; }
        }

        public String MISMATCH_WITH_PROPERTY_FOUND
        {
            get { return Properties.Resources.MISMATCH_WITH_PROPERTY_FOUND; }
        }

        public String ITEM_IN_PROPERTY_COLLECTION_DOES_NOT_MATCH
        {
            get { return Properties.Resources.ITEM_IN_PROPERTY_COLLECTION_DOES_NOT_MATCH; }
        }

        public String COLLECTION_COUNTS_FOR_PROPERTY_DO_NOT_MATCH
        {
            get { return Properties.Resources.COLLECTION_COUNTS_FOR_PROPERTY_DO_NOT_MATCH; }
        }
        public String CORE_SOURCE_DESTINATION_OBJECTS_ARE_NULL
        {
            get { return Resources.CORE_SOURCE_DESTINATION_OBJECTS_ARE_NULL; }
        }

        public String PROPERTY_OR_METHOD_NOT_IMPLEMENTED
        {
            get { return Resources.PROPERTY_OR_METHOD_NOT_IMPLEMENTED; }
        }

    }
}
