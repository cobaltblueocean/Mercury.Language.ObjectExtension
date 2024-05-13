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
    public class LocalizedResources : INotifyPropertyChanged
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

        public String CannotCompareValues
        {
            get { return Properties.Resources.CannotCompareValues; }
        }

        public String CannotCompareProperty
        {
            get { return Properties.Resources.CannotCompareProperty; }
        }

        public String MismatchWithPropertyFound
        {
            get { return Properties.Resources.MismatchWithPropertyFound; }
        }

        public String ItemInPropertyCollectionDoesNotMatch
        {
            get { return Properties.Resources.ItemInPropertyCollectionDoesNotMatch; }
        }

        public String CollectionCountsForPropertyDoNotMatch
        {
            get { return Properties.Resources.CollectionCountsForPropertyDoNotMatch; }
        }
        public String CoreSourceDestinationObjectsAreNull
        {
            get { return Resources.CoreSourceDestinationObjectsAreNull; }
        }

        public String PropertyOrMethodNotImplemented
        {
            get { return Resources.PropertyOrMethodNotImplemented; }
        }

    }
}
