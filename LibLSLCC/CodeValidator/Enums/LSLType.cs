#region FileInfo

// 
// File: LSLType.cs
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

#endregion

namespace LibLSLCC.CodeValidator
{
    /// <summary>
    ///     Represents the basic types in LSL
    /// </summary>
    public enum LSLType
    {
        /// <summary>
        ///     LSL type key
        /// </summary>
        Key = 7,

        /// <summary>
        ///     LSL type integer
        /// </summary>
        Integer = 6,

        /// <summary>
        ///     LSL type string
        /// </summary>
        String = 5,

        /// <summary>
        ///     LSL type float
        /// </summary>
        Float = 4,

        /// <summary>
        ///     LSL type list
        /// </summary>
        List = 3,

        /// <summary>
        ///     LSL type vector
        /// </summary>
        Vector = 2,

        /// <summary>
        ///     LSL type rotation
        /// </summary>
        Rotation = 1,

        /// <summary>
        ///     LSL type void (symbolic)
        /// </summary>
        Void = 0
    }


    /// <summary>
    ///     Extensions for converting <see cref="LSLType" />'s to strings and also parsing them.
    /// </summary>
    public static class LSLTypeTools
    {
        /// <summary>
        ///     Convert an LSL type name into an <see cref="LSLType" /> representation (case insensitive).  "void" is not
        ///     recognized.
        /// </summary>
        /// <param name="typeName">LSL Type name as string.</param>
        /// <returns>An <see cref="LSLType" /> representation of the name passed to <paramref name="typeName" />.</returns>
        /// <exception cref="ArgumentException">If <paramref name="typeName" /> was not recognized.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="typeName" /> was <c>null</c>.</exception>
        public static LSLType FromLSLTypeName(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                throw new ArgumentNullException("typeName");
            }


            switch (typeName.ToLower())
            {
                case "integer":
                    return LSLType.Integer;
                case "float":
                    return LSLType.Float;
                case "string":
                    return LSLType.String;
                case "key":
                    return LSLType.Key;
                case "vector":
                    return LSLType.Vector;
                case "quaternion":
                    return LSLType.Rotation;
                case "rotation":
                    return LSLType.Rotation;
                case "list":
                    return LSLType.List;
            }

            throw new ArgumentException("\"" + typeName + "\" is not a valid LSL type name", "typeName");
        }


        /// <summary>
        ///     Convert an <see cref="LSLType" /> to an LSL type name string.
        /// </summary>
        /// <param name="type"><see cref="LSLType" /> to convert.</param>
        /// <returns>LSL type string representation.</returns>
        /// <exception cref="ArgumentException">If <see cref="LSLType" /> is <see cref="LSLType.Void" />.</exception>
        public static string ToLSLTypeName(this LSLType type)
        {
            if (type == LSLType.Void)
            {
                throw new ArgumentException("Cannot convert LSLType.Void to a valid LSL type string", "type");
            }

            return type.ToString().ToLower();
        }
    }
}