﻿#region FileInfo
// 
// File: LSLCompilationUnitNode.cs
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
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodes.StatementNodes;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;
using LibLSLCC.Parser;

#endregion

namespace LibLSLCC.CodeValidator.ValidatorNodes.ScopeNodes
{
    public class LSLCompilationUnitNode : ILSLCompilationUnitNode, ILSLSyntaxTreeNode
    {
        private readonly List<LSLFunctionDeclarationNode> _functionDeclarations = new List<LSLFunctionDeclarationNode>();

        private readonly List<LSLVariableDeclarationNode> _globalVariableDeclarations =
            new List<LSLVariableDeclarationNode>();

        private readonly List<LSLStateScopeNode> _stateDeclarations = new List<LSLStateScopeNode>();
        private int _addCounter;
        private LSLStateScopeNode _defaultState;
// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLCompilationUnitNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }

        public LSLCompilationUnitNode(LSLParser.CompilationUnitContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            ParserContext = context;

            SourceCodeRange = new LSLSourceCodeRange(context);
        }

        internal LSLParser.CompilationUnitContext ParserContext { get; private set; }

        public IReadOnlyList<LSLVariableDeclarationNode> GlobalVariableDeclarations
        {
            get { return _globalVariableDeclarations; }
        }

        public IReadOnlyList<LSLFunctionDeclarationNode> FunctionDeclarations
        {
            get { return _functionDeclarations; }
        }

        public IReadOnlyList<LSLStateScopeNode> StateDeclarations
        {
            get { return _stateDeclarations; }
        }

        public LSLStateScopeNode DefaultState
        {
            get { return _defaultState; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                if (_defaultState == null)
                {
                    if (_globalVariableDeclarations.Count > 0)
                    {
                        _globalVariableDeclarations.Last().IsLastStatementInScope = false;
                    }

                    _addCounter++;
                }

                _defaultState = value;
                _defaultState.Parent = this;
            }
        }


        /// <summary>
        /// A list of objects describing the comments found in the source code and their position/range.
        /// </summary>
        public IReadOnlyList<LSLComment> Comments { get; set; }


        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        IReadOnlyList<ILSLVariableDeclarationNode> ILSLCompilationUnitNode.GlobalVariableDeclarations
        {
            get { return _globalVariableDeclarations ?? new List<LSLVariableDeclarationNode>(); }
        }

        IReadOnlyList<ILSLFunctionDeclarationNode> ILSLCompilationUnitNode.FunctionDeclarations
        {
            get { return _functionDeclarations ?? new List<LSLFunctionDeclarationNode>(); }
        }

        IReadOnlyList<ILSLStateScopeNode> ILSLCompilationUnitNode.StateDeclarations
        {
            get { return _stateDeclarations ?? new List<LSLStateScopeNode>(); }
        }

        ILSLStateScopeNode ILSLCompilationUnitNode.DefaultState
        {
            get { return DefaultState; }
        }

        public static
            LSLCompilationUnitNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLCompilationUnitNode(sourceRange, Err.Err);
        }



        /// <summary>
        /// Add a global variable declaration node to this compilation unit node.
        /// </summary>
        /// <param name="declaration">The global variable declaration node to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if the 'declaration' parameter is null.</exception>
        public void AddVariableDeclaration(LSLVariableDeclarationNode declaration)
        {
            if (declaration == null)
            {
                throw new ArgumentNullException("declaration");
            }

            declaration.Parent = this;
            declaration.StatementIndex = _addCounter;
            declaration.IsLastStatementInScope = true;

            if (_globalVariableDeclarations.Count > 0)
            {
                _globalVariableDeclarations.Last().IsLastStatementInScope = false;
            }

            _addCounter++;
            _globalVariableDeclarations.Add(declaration);
        }


        /// <summary>
        /// Add a function declaration node to this compilation unit node.
        /// </summary>
        /// <param name="declaration">The function declaration node to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if the 'declaration' parameter is null.</exception>
        public void AddFunctionDeclaration(LSLFunctionDeclarationNode declaration)
        {
            if (declaration == null)
            {
                throw new ArgumentNullException("declaration");
            }

            declaration.Parent = this;

            if (_globalVariableDeclarations.Count > 0)
            {
                _globalVariableDeclarations.Last().IsLastStatementInScope = false;
            }
            _addCounter++;
            _functionDeclarations.Add(declaration);
        }


        /// <summary>
        /// Add a state declaration node to this compilation unit node.
        /// </summary>
        /// <param name="declaration">The state declaration node to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if the 'declaration' parameter is null.</exception>
        public void AddStateDeclaration(LSLStateScopeNode declaration)
        {
            if (declaration == null)
            {
                throw new ArgumentNullException("declaration");
            }

            declaration.Parent = this;

            if (_globalVariableDeclarations.Count > 0)
            {
                _globalVariableDeclarations.Last().IsLastStatementInScope = false;
            }

            _addCounter++;
            _stateDeclarations.Add(declaration);
        }

        #region Nested type: Err

        protected enum Err
        {
            Err
        }

        #endregion

        #region ILSLTreeNode Members

        /// <summary>
        /// True if this syntax tree node contains syntax errors.
        /// </summary>
        public bool HasErrors { get; set; }

        /// <summary>
        /// The source code range that this syntax tree node occupies.
        /// </summary>
        public LSLSourceCodeRange SourceCodeRange { get; private set; }


        /// <summary>
        /// Accept a visit from an implementor of ILSLValidatorNodeVisitor
        /// </summary>
        /// <typeparam name="T">The visitors return type.</typeparam>
        /// <param name="visitor">The visitor instance.</param>
        /// <returns>The value returned from this method in the visitor used to visit this node.</returns>
        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitCompilationUnit(this);
        }


        /// <summary>
        /// The parent node of this syntax tree node.
        /// </summary>
        public ILSLSyntaxTreeNode Parent { get; set; }

        #endregion
    }
}