#region FileInfo
// 
// File: LSLRotationLiteralNode.cs
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
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;
using LibLSLCC.Parser;

#endregion

namespace LibLSLCC.CodeValidator.ValidatorNodes.ExpressionNodes
{
    public class LSLRotationLiteralNode : ILSLRotationLiteralNode, ILSLExprNode
    {
// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLRotationLiteralNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }

        internal LSLRotationLiteralNode(LSLParser.RotationLiteralContext context, ILSLExprNode x, ILSLExprNode y,
            ILSLExprNode z, ILSLExprNode s)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (x == null)
            {
                throw new ArgumentNullException("x");
            }
            x.Parent = this;

            if (y == null)
            {
                throw new ArgumentNullException("y");
            }
            y.Parent = this;

            if (z == null)
            {
                throw new ArgumentNullException("z");
            }
            z.Parent = this;

            if (s == null)
            {
                throw new ArgumentNullException("s");
            }
            s.Parent = this;

            ParserContext = context;
            XExpression = x;
            YExpression = y;
            ZExpression = z;
            SExpression = s;

            SourceCodeRange = new LSLSourceCodeRange(context);

            CommaOneSourceCodeRange = new LSLSourceCodeRange(context.comma_one);
            CommaTwoSourceCodeRange = new LSLSourceCodeRange(context.comma_two);
            CommaThreeSourceCodeRange = new LSLSourceCodeRange(context.comma_three);

            SourceCodeRangesAvailable = true;
        }



        internal LSLParser.RotationLiteralContext ParserContext { get; private set; }

        /// <summary>
        /// The expression node used to initialize the X (first) Component of the rotation literal.  
        /// This should never be null.
        /// </summary>
        public ILSLExprNode XExpression { get; private set; }

        /// <summary>
        /// The expression node used to initialize the Y (second) Component of the rotation literal.  
        /// This should never be null.
        /// </summary>
        public ILSLExprNode YExpression { get; private set; }

        /// <summary>
        /// The expression node used to initialize the Z (third) Component of the rotation literal.  
        /// This should never be null.
        /// </summary>
        public ILSLExprNode ZExpression { get; private set; }

        /// <summary>
        /// The expression node used to initialize the S (fourth) Component of the rotation literal.  
        /// This should never be null.
        /// </summary>
        public ILSLExprNode SExpression { get; private set; }


        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }


        ILSLReadOnlyExprNode ILSLRotationLiteralNode.XExpression
        {
            get { return XExpression; }
        }


        ILSLReadOnlyExprNode ILSLRotationLiteralNode.YExpression
        {
            get { return YExpression; }
        }



        ILSLReadOnlyExprNode ILSLRotationLiteralNode.ZExpression
        {
            get { return ZExpression; }
        }



        ILSLReadOnlyExprNode ILSLRotationLiteralNode.SExpression
        {
            get { return SExpression; }
        }


        public static
            LSLRotationLiteralNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLRotationLiteralNode(sourceRange, Err.Err);
        }

        #region Nested type: Err

        protected enum Err
        {
            Err
        }

        #endregion

        #region ILSLExprNode Members

        /// <summary>
        /// Deep clones the expression node.  It should clone the node and also clone all of its children.
        /// </summary>
        /// <returns>A deep clone of this expression node.</returns>
        public ILSLExprNode Clone()
        {
            if (HasErrors)
            {
                return GetError(SourceCodeRange);
            }

            return new LSLRotationLiteralNode(ParserContext, XExpression.Clone(), YExpression.Clone(),
                ZExpression.Clone(),
                SExpression.Clone())
            {
                HasErrors = HasErrors,
                Parent = Parent
            };
        }


        /// <summary>
        /// The parent node of this syntax tree node.
        /// </summary>
        public ILSLSyntaxTreeNode Parent { get; set; }


        /// <summary>
        /// True if this syntax tree node contains syntax errors.
        /// </summary>
        public bool HasErrors { get; set; }

        /// <summary>
        /// The source code range that this syntax tree node occupies.
        /// </summary>
        public LSLSourceCodeRange SourceCodeRange { get; private set; }



        /// <summary>
        /// Should return true if source code ranges are available/set to meaningful values for this node.
        /// </summary>
        public bool SourceCodeRangesAvailable { get; private set; }


        /// <summary>
        /// The source code range of the first component separator comma to appear in the rotation literal.
        /// </summary>
        public LSLSourceCodeRange CommaOneSourceCodeRange { get; private set; }

        /// <summary>
        /// The source code range of the second component separator comma to appear in the rotation literal.
        /// </summary>
        public LSLSourceCodeRange CommaTwoSourceCodeRange { get; private set; }


        /// <summary>
        /// The source code range of the third component separator comma to appear in the rotation literal.
        /// </summary>
        public LSLSourceCodeRange CommaThreeSourceCodeRange { get; private set; }


        /// <summary>
        /// Accept a visit from an implementor of <see cref="ILSLValidatorNodeVisitor{T}"/>
        /// </summary>
        /// <typeparam name="T">The visitors return type.</typeparam>
        /// <param name="visitor">The visitor instance.</param>
        /// <returns>The value returned from this method in the visitor used to visit this node.</returns>
        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitRotationLiteral(this);
        }



        /// <summary>
        /// The return type of the expression. see: <see cref="LSLType" />
        /// </summary>
        public LSLType Type
        {
            get { return LSLType.Rotation; }
        }



        /// <summary>
        /// The expression type/classification of the expression. see: <see cref="LSLExpressionType" />
        /// </summary>
        /// <value>
        /// The type of the expression.
        /// </value>
        public LSLExpressionType ExpressionType
        {
            get { return LSLExpressionType.Literal; }
        }


        /// <summary>
        /// True if the expression is constant and can be calculated at compile time.
        /// </summary>
        public bool IsConstant
        {
            get
            {
                var precondition =
                    XExpression != null &&
                    YExpression != null &&
                    ZExpression != null &&
                    SExpression != null;

                return precondition &&
                       XExpression.IsConstant &&
                       YExpression.IsConstant &&
                       ZExpression.IsConstant &&
                       SExpression.IsConstant;
            }
        }


        /// <summary>
        /// Should produce a user friendly description of the expressions return type.
        /// This is used in some syntax error messages, Ideally you should enclose your description in
        /// parenthesis or something that will make it stand out in a string.
        /// </summary>
        /// <returns></returns>
        public string DescribeType()
        {
            return "(" + Type + (this.IsLiteral() ? " Literal)" : ")");
        }


        ILSLReadOnlyExprNode ILSLReadOnlyExprNode.Clone()
        {
            return Clone();
        }

        #endregion
    }
}