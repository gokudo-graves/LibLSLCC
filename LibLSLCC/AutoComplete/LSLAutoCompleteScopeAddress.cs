#region FileInfo

// 
// File: LSLScopeAddress.cs
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

using System;

namespace LibLSLCC.AutoComplete
{
    /// <summary>
    ///     Represents a scope address within the auto complete parser.
    /// </summary>
    public struct LSLAutoCompleteScopeAddress : IEquatable<LSLAutoCompleteScopeAddress>
    {
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(LSLAutoCompleteScopeAddress other)
        {
            return CodeAreaId == other.CodeAreaId && ScopeLevel == other.ScopeLevel && ScopeId == other.ScopeId;
        }


        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        /// <param name="obj">Another object to compare to. </param>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is LSLAutoCompleteScopeAddress && Equals((LSLAutoCompleteScopeAddress) obj);
        }


        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = CodeAreaId;
                hashCode = (hashCode*397) ^ ScopeLevel;
                hashCode = (hashCode*397) ^ ScopeId;
                return hashCode;
            }
        }

        /// <summary>
        /// Test two <see cref="LSLAutoCompleteScopeAddress"/> for equality, uses: <see cref="CodeAreaId"/>, <see cref="ScopeId"/> and <see cref="ScopeLevel"/>.
        /// </summary>
        /// <param name="left">The left scope address.</param>
        /// <param name="right">The right scope address.</param>
        /// <returns><c>true</c> if both objects have equal properties; otherwise <c>false</c></returns>
        public static bool operator ==(LSLAutoCompleteScopeAddress left, LSLAutoCompleteScopeAddress right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Test two <see cref="LSLAutoCompleteScopeAddress"/> for inequality, uses: <see cref="CodeAreaId"/>, <see cref="ScopeId"/> and <see cref="ScopeLevel"/>.
        /// </summary>
        /// <param name="left">The left scope address.</param>
        /// <param name="right">The right scope address.</param>
        /// <returns><c>true</c> if both objects have inequal properties; otherwise <c>false</c></returns>
        public static bool operator !=(LSLAutoCompleteScopeAddress left, LSLAutoCompleteScopeAddress right)
        {
            return !left.Equals(right);
        }


        /// <summary>
        ///     Construct a new scope address given a code area ID, scope ID and scope level.
        /// </summary>
        /// <param name="codeAreaId">The code area ID of the address.</param>
        /// <param name="scopeId">The scope ID of the address.</param>
        /// <param name="scopeLevel">The cope level of the address.</param>
        public LSLAutoCompleteScopeAddress(int codeAreaId, int scopeId, int scopeLevel) : this()
        {
            CodeAreaId = codeAreaId;
            ScopeId = scopeId;
            ScopeLevel = scopeLevel;
        }


        /// <summary>
        ///     Effectively the index of the code able script area.
        ///     Each time a function declaration body or event handler body is encountered, this address value increases by one.
        /// </summary>
        public int CodeAreaId { get; private set; }

        /// <summary>
        ///     The overall scope level, every time an opening brace is encountered this increments, and when a closing brace is
        ///     encountered it decrements.
        /// </summary>
        public int ScopeLevel { get; private set; }

        /// <summary>
        ///     Gets the scope ID,  the top level of a function declaration scope or event handler scope is considered to be
        ///     ScopeId=1.
        ///     Subsequent scopes within the function declaration or event handler declaration cause the ID value to increment, it
        ///     is never decremented.
        ///     It resets to 1 at the top level of every function declaration or event handler.
        /// </summary>
        public int ScopeId { get; private set; }


        /// <summary>
        ///     Returns a string that represents the scope address.
        ///     The format is: "(CodeAreaID: 0, ScopeID: 0, ScopeLevel: 0)"
        /// </summary>
        /// <returns>A string in the format: "(CodeAreaID: 0, ScopeID: 0, ScopeLevel: 0)".</returns>
        public override string ToString()
        {
            return string.Format("(CodeAreaID: {0}, ScopeId: {1}, ScopeLevel: {2})", CodeAreaId, ScopeId, ScopeLevel);
        }
    }
}