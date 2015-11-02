#region FileInfo
// 
// File: ILSLControlStatementNode.cs
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

using System.Collections.Generic;

#endregion

namespace LibLSLCC.CodeValidator.Nodes.Interfaces
{

    /// <summary>
    /// AST node interface for control statement chains.  This node's children consists of
    /// an if statement, optionally multiple else-if statements, and an optional else statement.
    /// </summary>
    public interface ILSLControlStatementNode : ILSLReadOnlyCodeStatement
    {
        /// <summary>
        /// True if the control statement node has an else statement child.
        /// </summary>
        bool HasElseStatement { get; }

        /// <summary>
        /// True if the control statement node has an if statement child.
        /// This can only really be false if the node contains errors.
        /// It should always be checked before using the IfStatement child property.
        /// </summary>
        bool HasIfStatement { get; }

        /// <summary>
        /// True if the control statement node has any if-else statement children.
        /// </summary>
        bool HasElseIfStatements { get; }

        /// <summary>
        /// The else statement child of this control statement node if one exists, otherwise null.
        /// </summary>
        ILSLElseStatementNode ElseStatement { get; }

        /// <summary>
        /// The if statement child of this control statement node if one exists, otherwise null.
        /// </summary>
        ILSLIfStatementNode IfStatement { get; }

        /// <summary>
        /// The else-if statement children of this control statement node if one exists, otherwise an empty enumerable.
        /// </summary>
        IEnumerable<ILSLElseIfStatementNode> ElseIfStatements { get; }
    }
}