﻿#region FileInfo
// 
// File: LSLControlStatementNode.cs
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

#endregion

namespace LibLSLCC.CodeValidator.ValidatorNodes.StatementNodes
{
    public class LSLControlStatementNode : ILSLControlStatementNode, ILSLCodeStatement
    {
        private readonly List<LSLElseIfStatementNode> _elseIfStatements = new List<LSLElseIfStatementNode>();
        private LSLElseStatementNode _elseStatement;
        private LSLIfStatementNode _ifStatement;
// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLControlStatementNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }

        internal LSLControlStatementNode(LSLParser.ControlStructureContext context, bool isSingleBlockStatement)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            ParserContext = context;
            IsSingleBlockStatement = isSingleBlockStatement;
            ParserContext = context;

            SourceCodeRange = new LSLSourceCodeRange(context);
        }

        internal LSLControlStatementNode(LSLParser.ControlStructureContext context, LSLIfStatementNode ifStatement,
            IEnumerable<LSLElseIfStatementNode> elseIfStatements,
            LSLElseStatementNode elseStatement, bool isSingleBlockStatement) :
                this(context, ifStatement, elseIfStatements, isSingleBlockStatement)
        {
            if (elseStatement == null)
            {
                throw new ArgumentNullException("elseStatement");
            }

            ElseStatement = elseStatement;
            elseStatement.Parent = this;
        }

        internal LSLControlStatementNode(LSLParser.ControlStructureContext context, LSLIfStatementNode ifStatement,
            IEnumerable<LSLElseIfStatementNode> elseIfStatements, bool isSingleBlockStatement) :
                this(context, ifStatement, isSingleBlockStatement)
        {
            if (elseIfStatements == null)
            {
                throw new ArgumentNullException("elseIfStatements");
            }

            foreach (var lslElseIfStatementNode in elseIfStatements)
            {
                AddElseIfStatement(lslElseIfStatementNode);
            }
        }

        internal LSLControlStatementNode(LSLParser.ControlStructureContext context,
            LSLIfStatementNode ifStatement,
            bool isSingleBlockStatement) : this(context, isSingleBlockStatement)
        {
            if (ifStatement == null)
            {
                throw new ArgumentNullException("ifStatement");
            }

            IfStatement = ifStatement;
            IfStatement.Parent = this;
        }

        public LSLElseStatementNode ElseStatement
        {
            get { return _elseStatement; }
            set
            {
                if (value != null)
                {
                    value.Parent = this;
                }


                _elseStatement = value;
            }
        }

        public LSLIfStatementNode IfStatement
        {
            get { return _ifStatement; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                value.Parent = this;
                _ifStatement = value;
            }
        }

        public IEnumerable<LSLElseIfStatementNode> ElseIfStatements
        {
            get { return _elseIfStatements; }
        }

        internal LSLParser.ControlStructureContext ParserContext { get; private set; }
        public ILSLCodeStatement ReturnPath { get; set; }
        public LSLDeadCodeType DeadCodeType { get; set; }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        public bool HasElseStatement
        {
            get { return ElseStatement != null; }
        }

        public bool HasIfStatement
        {
            get { return IfStatement != null; }
        }

        public bool HasElseIfStatements
        {
            get { return ElseIfStatements.Any(); }
        }

        ILSLElseStatementNode ILSLControlStatementNode.ElseStatement
        {
            get { return ElseStatement; }
        }

        ILSLIfStatementNode ILSLControlStatementNode.IfStatement
        {
            get { return IfStatement; }
        }

        IEnumerable<ILSLElseIfStatementNode> ILSLControlStatementNode.ElseIfStatements
        {
            get { return _elseIfStatements; }
        }

        public ulong ScopeId { get; set; }

        public static
            LSLControlStatementNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLControlStatementNode(sourceRange, Err.Err);
        }

        public void AddElseIfStatement(LSLElseIfStatementNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }


            node.Parent = this;
            _elseIfStatements.Add(node);
        }

        private bool HaveReturnPath()
        {
            if (!ElseIfStatements.Any())
            {
                return IfStatement.HasReturnPath && (ElseStatement != null && ElseStatement.HasReturnPath);
            }

            return IfStatement.HasReturnPath && ElseIfStatements.All(x => x.HasReturnPath) &&
                   (ElseStatement != null && ElseStatement.HasReturnPath);
        }

        #region Nested type: Err

        protected enum Err
        {
            Err
        }

        #endregion

        #region ILSLCodeStatement Members

        public bool IsSingleBlockStatement { get; private set; }
        public ILSLSyntaxTreeNode Parent { get; set; }
        public int StatementIndex { get; set; }

        public bool IsLastStatementInScope { get; set; }

        public bool IsDeadCode { get; set; }

        ILSLReadOnlyCodeStatement ILSLReadOnlyCodeStatement.ReturnPath
        {
            get { return ReturnPath; }
        }


        public bool HasReturnPath
        {
            get { return HaveReturnPath(); }
        }


        public bool HasErrors { get; set; }

        public LSLSourceCodeRange SourceCodeRange { get; }


        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitControlStatement(this);
        }


        /// <summary>
        ///     Returns a constant jump from this control statement chain if there is one,
        ///     otherwise null.  A constant jump is a singular jump to the same label
        ///     from every possible branch of the control chain. meaning the jump
        ///     happens in a constant manner regardless of which branch is taken.
        ///     DeterminingJump will point to the LSLJumpStatementNode in the 'if' code scope.
        ///     EffectiveJumpStatement will point to the control statement, because it can effectively
        ///     be considered a jump statement since it will always cause a jump to a known label to occur.
        ///     The label that this control statement jumps to can be found with DeterminingJump.JumpTarget.
        ///     EffectiveJumpStatement is mostly useful for its statement index information
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public LSLConstantJumpDescription GetConstantJump()
        {
            if (!HasElseIfStatements && !HasElseStatement)
            {
                return null;
            }

            var cmp = new JumpCmp();
            var i = new HashSet<LSLConstantJumpDescription>(IfStatement.ConstantJumps, cmp);
            if (HasElseIfStatements)
            {
                foreach (var node in ElseIfStatements)
                {
                    var ie = new HashSet<LSLConstantJumpDescription>(node.ConstantJumps, cmp);
                    i.IntersectWith(ie);
                }
            }
            if (HasElseStatement)
            {
                var e = new HashSet<LSLConstantJumpDescription>(ElseStatement.ConstantJumps, cmp);
                i.IntersectWith(e);
            }

            var x = i.SingleOrDefault();

            if (x != null)
            {
                x = new LSLConstantJumpDescription(x, this);
            }

            return x;
        }


        private class JumpCmp :
            IEqualityComparer<LSLConstantJumpDescription>
        {
            public bool Equals(LSLConstantJumpDescription x, LSLConstantJumpDescription y)
            {
                return x.DeterminingJump.JumpTarget == y.DeterminingJump.JumpTarget &&
                       x.DeterminingJump.JumpTarget.ScopeId == y.DeterminingJump.JumpTarget.ScopeId;
            }

            public int GetHashCode(LSLConstantJumpDescription obj)
            {
                var hash = 17;
                hash = hash*31 + obj.DeterminingJump.JumpTarget.ScopeId.GetHashCode();
                hash = hash*31 + obj.DeterminingJump.LabelName.GetHashCode();

                return hash;
            }
        }

        #endregion
    }
}