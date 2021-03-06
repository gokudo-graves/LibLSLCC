﻿#region FileInfo

// 
// File: LSLFloatLiteralNode.cs
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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using LibLSLCC.AntlrParser;

#endregion

namespace LibLSLCC.CodeValidator
{
    /// <summary>
    ///     Default <see cref="ILSLFloatLiteralNode" /> implementation used by <see cref="LSLCodeValidator" />
    /// </summary>
    public sealed class LSLFloatLiteralNode : LSLConstantLiteralNode<LSLFloatLiteralNode>, ILSLFloatLiteralNode
    {
        // ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        private LSLFloatLiteralNode(LSLSourceCodeRange sourceRange, Err err)
            : base(sourceRange, Err.Err)
            // ReSharper restore UnusedParameter.Local
        {
        }


        /// <summary>
        ///     Create an <see cref="LSLFloatLiteralNode" /> by cloning from another.
        /// </summary>
        /// <param name="other">The other node to clone from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other" /> is <c>null</c>.</exception>
        private LSLFloatLiteralNode(LSLFloatLiteralNode other) : base(other)
        {
        }


        /// <summary>
        ///     Construct an <see cref="LSLFloatLiteralNode" /> with a positive floating point value.
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     if <paramref name="value" /> is not positive; add to a prefix negate node
        ///     instead.
        /// </exception>
        /// <exception cref="ArithmeticException"><paramref name="value" /> is equal to <see cref="F:System.Single.NaN" />. </exception>
        public LSLFloatLiteralNode(float value)
            : base(value.ToString(CultureInfo.InvariantCulture), LSLType.Float, null)
        {
            if (Math.Sign(value) < 0)
            {
                throw new ArgumentOutOfRangeException("value",
                    "value must positive, add to a prefix negate node instead.");
            }
        }


        internal LSLFloatLiteralNode(LSLParser.Expr_AtomContext context)
            : base(context.GetText(), LSLType.Float, new LSLSourceCodeRange(context))
        {
        }


        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }


        /// <summary>
        ///     Accept a visit from an implementor of <see cref="ILSLValidatorNodeVisitor{T}" />
        /// </summary>
        /// <typeparam name="T">The visitors return type.</typeparam>
        /// <param name="visitor">The visitor instance.</param>
        /// <returns>The value returned from this method in the visitor used to visit this node.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="visitor"/> is <c>null</c>.</exception>
        public override T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            if (visitor == null) throw new ArgumentNullException("visitor");

            return visitor.VisitFloatLiteral(this);
        }


        ILSLReadOnlyExprNode ILSLReadOnlyExprNode.Clone()
        {
            return Clone();
        }


        /// <summary>
        ///     Returns a version of this node type that represents its error state;  in case of a syntax error
        ///     in the node that prevents the node from being even partially built.
        /// </summary>
        /// <param name="sourceRange">The source code range of the error.</param>
        /// <returns>A version of this node type in its undefined/error state.</returns>
        public static LSLFloatLiteralNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLFloatLiteralNode(sourceRange, Err.Err);
        }


        /// <summary>
        ///     Deep clones the expression node.  It should clone the node and all of its children and cloneable properties, except
        ///     the parent.
        ///     When cloned, the parent node reference should still point to the same node.
        /// </summary>
        /// <returns>A deep clone of this expression node.</returns>
        public override LSLFloatLiteralNode Clone()
        {
            return HasErrors ? GetError(SourceRange) : new LSLFloatLiteralNode(this);
        }
    }
}