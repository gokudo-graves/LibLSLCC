﻿#region FileInfo

// 
// File: CSharpInheritanceList.cs
// 
// 
// ============================================================
// ============================================================
// 
// 
// Copyright (c) 2015, Teriks
// 
// All rights reserved.
// 
// 
// This file is part of LibLSLCC.
// 
// LibLSLCC is distributed under the following BSD 3-Clause License
// 
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
// 1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// 
// 2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer
//     in the documentation and/or other materials provided with the distribution.
// 
// 3. Neither the name of the copyright holder nor the names of its contributors may be used to endorse or promote products derived
//     from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
// ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 
// 
// ============================================================
// ============================================================
// 
// 

#endregion

#region Imports

using System;
using System.Xml.Serialization;
using LibLSLCC.Collections;
using LibLSLCC.Settings;

#endregion

namespace LibLSLCC.CSharp
{
    /// <summary>
    ///     Abstraction that provides parsing and validation for CSharp inheritance list strings.
    /// </summary>
    public sealed class CSharpInheritanceList : SettingsBaseClass<CSharpInheritanceList>, IObservableHashSetItem
    {
        private readonly IReadOnlyHashedSet<string> _hashEqualityPropertyNames = new HashedSet<string>()
        {
            "FullSignature"
        };

        private string _fullSignature;
        private CSharpInheritanceListValidationResult _validatedSignature;


        /// <summary>
        ///     Parameterless constructor used by <see cref="SettingsBaseClass{CSharpNamespace}" />
        /// </summary>
        private CSharpInheritanceList()
        {
        }


        /// <summary>
        ///     Initializes a new instance of the <see cref="CSharpInheritanceList" /> class by parsing a class inheritance list
        ///     from <paramref name="fullSignature" />
        /// </summary>
        /// <param name="fullSignature">The full signature.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="fullSignature" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentException">
        ///     Thrown if <see cref="CSharpInheritanceListValidator.Validate" /> fails to
        ///     successfully parse <paramref name="fullSignature" />.
        /// </exception>
        public CSharpInheritanceList(string fullSignature)
        {
            if (fullSignature == null)
            {
                throw new ArgumentNullException("fullSignature", "Inheritance list signature string cannot be null.");
            }

            _validatedSignature = CSharpInheritanceListValidator.Validate(fullSignature);

            if (!_validatedSignature.Success)
            {
                throw new ArgumentException(_validatedSignature.ErrorDescription, "fullSignature");
            }

            _fullSignature = fullSignature;
        }


        /// <summary>
        ///     Gets or sets the full signature of the inheritance list.  Parsing and validation occurs when this property is set.
        /// </summary>
        /// <value>
        ///     The full inheritance list signature.
        /// </value>
        /// <exception cref="System.ArgumentNullException">Thrown if the value is set to <c>null</c>.</exception>
        /// <exception cref="System.ArgumentException">
        ///     Thrown if <see cref="CSharpInheritanceListValidator.Validate" /> fails to
        ///     successfully parse a value set to this property.
        /// </exception>
        public string FullSignature
        {
            get { return _fullSignature; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value", "Inheritance list signature string cannot be null.");
                }

                var vSig = CSharpInheritanceListValidator.Validate(value);

                if (!vSig.Success)
                {
                    throw new ArgumentException(_validatedSignature.ErrorDescription, "value");
                }


                SetField(ref _validatedSignature, vSig, "ValidatedSignature");
                SetField(ref _fullSignature, value, "FullSignature");
            }
        }

        /// <summary>
        ///     Gets the parsing/validation results of the class/type signature string.
        /// </summary>
        /// <value>
        ///     The parsing/validation results of the class/type signature string.
        /// </value>
        [XmlIgnore]
        public CSharpInheritanceListValidationResult ValidatedSignature
        {
            get { return _validatedSignature; }
            set { _validatedSignature = value; }
        }

        /// <summary>
        ///     Gets a formated version of the inheritance list with an added colon character at the front if one is necessary.
        /// </summary>
        /// <value>
        ///     The formated inheritance list with and added colon prefix if necessary.
        /// </value>
        [XmlIgnore]
        public string ListWithColonIfNecessary
        {
            get
            {
                if (FullSignature == null) return null;

                if (_validatedSignature.InheritedTypes.Count > 0)
                {
                    return ": " + FullSignature;
                }
                return FullSignature;
            }
        }

        /// <summary>
        ///     Is true if the inheritance list is completely empty, IE.. there are no inherited classes or type constraints.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the inheritance list is empty; otherwise, <c>false</c>.
        /// </value>
        public bool IsEmpty
        {
            get { return string.IsNullOrWhiteSpace(FullSignature); }
        }

        IReadOnlyHashedSet<string> IObservableHashSetItem.HashEqualityPropertyNames
        {
            get { return _hashEqualityPropertyNames; }
        }


        /// <summary>
        ///     Implicitly converts a string into a <see cref="CSharpInheritanceList" /> by parsing it.
        /// </summary>
        /// <param name="fullSignature">The string representing the full inheritance list.</param>
        /// <returns>The newly created <see cref="CSharpInheritanceList" /> from the string.</returns>
        public static implicit operator CSharpInheritanceList(string fullSignature)
        {
            return new CSharpInheritanceList(fullSignature);
        }


        /// <summary>
        ///     Returns <see cref="FullSignature" />.
        /// </summary>
        /// <returns>
        ///     <see cref="FullSignature" />.
        /// </returns>
        public override string ToString()
        {
            return FullSignature;
        }


        /// <summary>
        ///     Returns the hash code of <see cref="FullSignature" />.
        /// </summary>
        /// <returns>
        ///     Returns the hash code of <see cref="FullSignature" />.
        /// </returns>
        public override int GetHashCode()
        {
            if (FullSignature == null) return -1;
            return FullSignature.GetHashCode();
        }


        /// <summary>
        ///     Compares using <see cref="FullSignature" />.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="System.Object" /> is a <see cref="CSharpInheritanceList" /> with an equal
        ///     <see cref="FullSignature" /> value; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var ns = obj as CSharpInheritanceList;
            if (ns == null) return false;

            if (ns.FullSignature != null && FullSignature != null)
                return FullSignature.Equals(ns.FullSignature, StringComparison.Ordinal);

            return ns.FullSignature == FullSignature;
        }
    }
}