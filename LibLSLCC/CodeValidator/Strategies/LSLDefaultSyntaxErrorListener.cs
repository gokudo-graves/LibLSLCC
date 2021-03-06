#region FileInfo

// 
// File: LSLDefaultSyntaxErrorListener.cs
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
using LibLSLCC.Collections;

#endregion

namespace LibLSLCC.CodeValidator
{
    /// <summary>
    ///     The default implementation of <see cref="ILSLSyntaxErrorListener" /> for the library.
    ///     It writes error information to standard out or an arbitrary stream.
    /// </summary>
    public class LSLDefaultSyntaxErrorListener : ILSLSyntaxErrorListener
    {
        /// <summary>
        ///     A parsing error at the grammar level has occurred somewhere in the source code.
        /// </summary>
        /// <param name="line">The line on which the error occurred.</param>
        /// <param name="column">The character column at which the error occurred.</param>
        /// <param name="offendingTokenText">The text representing the offending token.</param>
        /// <param name="message">The parsing error messaged passed along from the parsing back end.</param>
        /// <param name="offendingTokenRange">The source code range of the offending symbol.</param>
        public virtual void GrammarLevelParserSyntaxError(int line, int column, LSLSourceCodeRange offendingTokenRange,
            string offendingTokenText, string message)
        {
            OnError(offendingTokenRange, message);
        }


        /// <summary>
        ///     A reference to an undefined variable was encountered.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="name">Name of the variable attempting to be referenced.</param>
        public virtual void UndefinedVariableReference(LSLSourceCodeRange location, string name)
        {
            OnError(location, string.Format("Variable \"{0}\" is undefined.", name));
        }


        /// <summary>
        ///     A parameter name for a function or event handler was used more than once.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="parameterListType">The type of parameter list the duplicate parameter name was found in.</param>
        /// <param name="type">The type of the new parameter who's name was duplicate.</param>
        /// <param name="name">The name of the new parameter, which was duplicate.</param>
        public virtual void ParameterNameRedefined(LSLSourceCodeRange location, LSLParameterListType parameterListType,
            LSLType type, string name)
        {
            OnError(location,
                (parameterListType == LSLParameterListType.EventParameters ? "Event" : "Function") +
                string.Format(" parameter name \"{0}\" is used more than once.", name));
        }


        /// <summary>
        ///     A binary operation was encountered that had expressions with incorrect return types on either or both sides.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="left">The left expression.</param>
        /// <param name="operation">The binary operation that was attempted on the two expressions.</param>
        /// <param name="right">The right expression.</param>
        public virtual void InvalidBinaryOperation(LSLSourceCodeRange location, ILSLReadOnlyExprNode left, string operation,
            ILSLReadOnlyExprNode right)
        {
            OnError(location, string.Format(
                "{0} {1} {2} is not a valid operation, operator cannot handle these types. (missing a cast?)",
                left.DescribeType(), operation, right.DescribeType()));
        }


        /// <summary>
        ///     A prefix operation was attempted on expression with an invalid return type.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="operation">The prefix operation that was attempted on the expression.</param>
        /// <param name="right">The expression the prefix operation was used on.</param>
        public virtual void InvalidPrefixOperation(LSLSourceCodeRange location, string operation, ILSLReadOnlyExprNode right)
        {
            OnError(location, string.Format(
                "{0}{1} is not a valid operation, operator cannot handle this type. (missing a cast?)", operation,
                right.DescribeType()));
        }


        /// <summary>
        ///     A postfix operation was attempted on expression with an invalid return type.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="operation">The postfix operation that was attempted on the expression.</param>
        /// <param name="left">The expression the postfix operation was used on.</param>
        public virtual void InvalidPostfixOperation(LSLSourceCodeRange location, ILSLReadOnlyExprNode left, string operation)
        {
            OnError(location, string.Format(
                "{0}{1} is not a valid operation, operator cannot handle this type. (missing a cast?)",
                left.DescribeType(),
                operation));
        }


        /// <summary>
        ///     An invalid cast was preformed on an expression.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="castTo">The type that the cast operation attempted to cast the expression to.</param>
        /// <param name="fromExpression">The expression that the cast was attempted on.</param>
        public virtual void InvalidCastOperation(LSLSourceCodeRange location, LSLType castTo,
            ILSLReadOnlyExprNode fromExpression)
        {
            OnError(location, string.Format(
                "Cannot cast to {0} from {1}.", castTo, fromExpression.DescribeType()));
        }


        /// <summary>
        ///     A variable declaration was initialized with an invalid type on the right.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="variableType">The actual type of the variable attempting to be initialized.</param>
        /// <param name="assignedExpression">The invalid expression that was assigned in the variable declaration.</param>
        public virtual void TypeMismatchInVariableDeclaration(LSLSourceCodeRange location, LSLType variableType,
            ILSLReadOnlyExprNode assignedExpression)
        {
            OnError(location, string.Format(
                "Type mismatch in variable declaration, {0} cannot be assigned to a {1} variable.",
                assignedExpression.DescribeType(),
                "(" + variableType + ")"));
        }


        /// <summary>
        ///     A variable was redefined.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="variableType">The type of the variable that was considered a re-definition.</param>
        /// <param name="variableName">The name of the variable that was considered a re-definition.</param>
        public virtual void VariableRedefined(LSLSourceCodeRange location, LSLType variableType, string variableName)
        {
            OnError(location, string.Format(
                "Variable name conflict, \"{0}\" is already defined and accessible from this scope.", variableName));
        }


        /// <summary>
        ///     A vector literal contained an invalid expression in its initializer list.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="component">The vector component of the initializer that contained the invalid expression.</param>
        /// <param name="invalidExpressionContent">The expression that was considered to be invalid vector initializer content.</param>
        public virtual void InvalidVectorContent(LSLSourceCodeRange location, LSLVectorComponent component,
            ILSLReadOnlyExprNode invalidExpressionContent)
        {
            OnError(location,
                string.Format("Vectors '{0}' component is not a float or integer, erroneous type is {1}.",
                    component.ToComponentName(),
                    invalidExpressionContent.DescribeType()));
        }


        /// <summary>
        ///     A list literal contained an invalid expression in its initializer list.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="index">The index in the initializer list that contained the invalid expression.</param>
        /// <param name="invalidExpressionContent">The expression that was considered to be invalid list initializer content.</param>
        public virtual void InvalidListContent(LSLSourceCodeRange location, int index,
            ILSLReadOnlyExprNode invalidExpressionContent)
        {
            OnError(location,
                string.Format("Lists cannot contain the type '{0}', encountered invalid type at list index {1}.",
                    invalidExpressionContent.DescribeType(), index));
        }


        /// <summary>
        ///     A rotation literal contained an invalid expression in its initializer list.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="component">The rotation component of the initializer that contained the invalid expression.</param>
        /// <param name="invalidExpressionContent">The expression that was considered to be invalid rotation initializer content.</param>
        public virtual void InvalidRotationContent(LSLSourceCodeRange location, LSLRotationComponent component,
            ILSLReadOnlyExprNode invalidExpressionContent)
        {
            OnError(location,
                string.Format("Rotations '{0}' component is not a float or integer, erroneous type is {1}.",
                    component.ToComponentName(),
                    invalidExpressionContent.DescribeType()));
        }


        /// <summary>
        ///     Attempted to return a value from a function with no return type. (A void function)
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="functionSignature">The signature of the function the return was attempted from.</param>
        /// <param name="attemptedReturnExpression">The expression that was attempted to be returned.</param>
        public virtual void ReturnedValueFromVoidFunction(LSLSourceCodeRange location,
            ILSLFunctionSignature functionSignature,
            ILSLReadOnlyExprNode attemptedReturnExpression)
        {
            OnError(location, string.Format(
                "Cannot return {0} value from function \"{1}\" because it does not specify a return type.",
                attemptedReturnExpression.DescribeType(), functionSignature.Name));
        }


        /// <summary>
        ///     Type mismatch between the expression returned from a function and the functions actual return type.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="functionSignature">The signature of the function the return was attempted from.</param>
        /// <param name="attemptedReturnExpression">The expression that was attempted to be returned.</param>
        public virtual void TypeMismatchInReturnValue(LSLSourceCodeRange location,
            ILSLFunctionSignature functionSignature,
            ILSLReadOnlyExprNode attemptedReturnExpression)
        {
            OnError(location, string.Format(
                "Type mismatch in return type for function \"{0}\", expected {1} and got {2}.",
                functionSignature.Name,
                functionSignature.ReturnType,
                attemptedReturnExpression.DescribeType()));
        }


        /// <summary>
        ///     An empty return statement was encountered in a non void function. <para/>
        ///     In other words, the function required an expression to be returned but none was provided.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="functionSignature">The signature of the function the return was attempted from.</param>
        public virtual void ReturnedVoidFromNonVoidFunction(LSLSourceCodeRange location,
            ILSLFunctionSignature functionSignature)
        {
            OnError(location, string.Format("function \"{0}\" must return value, expected {1} but got Void.",
                functionSignature.Name,
                functionSignature.ReturnType));
        }


        /// <summary>
        ///     A jump statement to an undefined label name was encountered.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="labelName">The name of the label that was given to the jump statement.</param>
        public virtual void JumpToUndefinedLabel(LSLSourceCodeRange location, string labelName)
        {
            OnError(location, string.Format("Label \"{0}\" is not defined.",
                labelName));
        }


        /// <summary>
        ///     A call to an undefined function was encountered.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="functionName">The name of the function used in the call.</param>
        public virtual void CallToUndefinedFunction(LSLSourceCodeRange location, string functionName)
        {
            OnError(location, string.Format("Function \"{0}\" is not defined.",
                functionName));
        }


        /// <summary>
        ///     A user defined or library function was called with the wrong number of parameters.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="functionSignature">The function signature of the defined function attempting to be called.</param>
        /// <param name="parameterExpressionsGiven">The expressions given to the function call.</param>
        public virtual void ImproperParameterCountInFunctionCall(LSLSourceCodeRange location,
            ILSLFunctionSignature functionSignature, ILSLReadOnlyExprNode[] parameterExpressionsGiven)
        {
            var length = parameterExpressionsGiven.Length == 0
                ? (object) "no"
                : parameterExpressionsGiven.Length;

            if (!functionSignature.HasVariadicParameter)
            {
                if (functionSignature.ParameterCount == 0)
                {
                    OnError(location, string.Format(
                        "Function \"{0}\" does not take any parameters, but {1} parameters were passed.",
                        functionSignature.Name,
                        parameterExpressionsGiven.Length));
                }
                else
                {
                    OnError(location, string.Format(
                        "Function \"{0}\" takes {1} parameter(s), but {2} parameters were passed.",
                        functionSignature.Name,
                        functionSignature.ParameterCount,
                        length));
                }
            }
            else
            {
                OnError(location, string.Format(
                    "Variadic Function \"{0}\" takes {1} concrete parameter(s), but {2} parameters were passed.",
                    functionSignature.Name,
                    functionSignature.ConcreteParameterCount,
                    length));
            }
        }


        /// <summary>
        ///     A user defined function was re-defined.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="previouslyDefinedSignature">
        ///     The signature of the previously defined function that is considered a
        ///     duplicated to the new definition.
        /// </param>
        public virtual void RedefinedFunction(LSLSourceCodeRange location,
            ILSLFunctionSignature previouslyDefinedSignature)
        {
            OnError(location,
                string.Format("Function \"{0}\" has already been defined.", previouslyDefinedSignature.Name));
        }


        /// <summary>
        ///     A code label was considered a redefinition, given the scope of the new definition.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="labelName">The name of the label being redefined.</param>
        public virtual void RedefinedLabel(LSLSourceCodeRange location, string labelName)
        {
            OnError(location, string.Format("Label \"{0}\" has already defined.", labelName));
        }


        /// <summary>
        ///     Dead code after a return path was detected in a function with a non-void return type.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="inFunction">The signature of the function the dead code was detected in.</param>
        /// <param name="deadSegment">An object describing the location an span of the dead code segment.</param>
        public virtual void DeadCodeAfterReturnPath(LSLSourceCodeRange location, ILSLFunctionSignature inFunction,
            ILSLDeadCodeSegment deadSegment)
        {
            if (deadSegment.SourceRange.IsSingleLine)
            {
                OnError(location, "Unreachable code detected after return path in function \"" + inFunction.Name + "\".");
            }
            else
            {
                OnError(location,
                    string.Format(
                        "Unreachable code detected after return path in function \"" + inFunction.Name +
                        "\" between lines {0} and {1}.",
                        MapLineNumber(deadSegment.SourceRange.LineStart),
                        MapLineNumber(deadSegment.SourceRange.LineEnd)));
            }
        }


        /// <summary>
        ///     A function with a non-void return type lacks a necessary return statement. <para/>
        ///     There is a path that leads to the end of the function without it returning a value.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="inFunction">The signature of the function in question.</param>
        public virtual void NotAllCodePathsReturnAValue(LSLSourceCodeRange location, ILSLFunctionSignature inFunction)
        {
            OnError(location, "Not all code paths return a value in function \"" + inFunction.Name + "\".");
        }


        /// <summary>
        ///     A code state does not declare any event handlers at all; this is not allowed.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="stateName">The name of the state in which this error occurred.</param>
        public virtual void StateHasNoEventHandlers(LSLSourceCodeRange location, string stateName)
        {
            OnError(location,
                "State \"" + stateName +
                "\" has no event handlers defined, state's must have at least one event handler.");
        }


        /// <summary>
        ///     A conditional expression is missing from an IF, ELSE IF, or WHILE/DO-WHILE statement.  <para/>
        ///     FOR loops can have a missing condition expression, but other control statements cannot.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="statementType">The type of branch/loop statement the condition was missing from.</param>
        public virtual void MissingConditionalExpression(LSLSourceCodeRange location,
            LSLConditionalStatementType statementType)
        {
            OnError(location, "Conditional expression was required but none was given.");
        }


        /// <summary>
        ///     Attempted to define a variable inside of a braceless scope. <para/>
        ///     For example, inside of an IF statement which does not use braces in its code area. (a single statement is used) <para/>
        ///     This applies to other control/loop statements that can use braceless scopes as well.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        public virtual void DefinedVariableInBracelessScope(LSLSourceCodeRange location)
        {
            OnError(location, "Declaration requires a new scope, use { and }.");
        }


        /// <summary>
        ///     An illegal character was found inside of a string literal according to the current
        ///     <see cref="ILSLStringPreProcessor" /> instance.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="err">The generated character error object from the <see cref="ILSLStringPreProcessor" /> instance.</param>
        public virtual void IllegalStringCharacter(LSLSourceCodeRange location, LSLStringCharacterError err)
        {
            OnError(location,
                string.Format("Illegal character '{0}' found in string at index [{1}].", err.CausingCharacter,
                    err.StringIndex));
        }


        /// <summary>
        ///     An invalid escape sequence was found inside of a string literal according to the current
        ///     <see cref="ILSLStringPreProcessor" /> instance.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="err">The generated character error object from the <see cref="ILSLStringPreProcessor" /> instance.</param>
        public virtual void InvalidStringEscapeCode(LSLSourceCodeRange location, LSLStringCharacterError err)
        {
            OnError(location,
                string.Format("Unknown escape sequence '\\{0}' found in string at index [{1}].", err.CausingCharacter,
                    err.StringIndex));
        }


        /// <summary>
        ///     A library defined event handler was used more than once in the same state.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="eventHandlerName">The name of the event handler which was used more than once.</param>
        /// <param name="stateName">The name of the code state in which the error occured.</param>
        public virtual void RedefinedEventHandler(LSLSourceCodeRange location, string eventHandlerName, string stateName)
        {
            OnError(location,
                string.Format("Event handler '{0}' was defined more than once in state '{1}'.", eventHandlerName,
                    stateName));
        }


        /// <summary>
        ///     The default code state was missing from the program.
        /// </summary>
        public virtual void MissingDefaultState()
        {
            OnError(new LSLSourceCodeRange(), "Default state is missing.");
        }


        /// <summary>
        ///     An overload could not be resolved for an attempted call to an overloaded library function.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="functionName">The name of the overloaded library function that the user attempted to call.</param>
        /// <param name="givenParameterExpressions">
        ///     The parameter expressions the user attempted to pass to the overloaded library
        ///     function.
        /// </param>
        public virtual void NoSuitableLibraryFunctionOverloadFound(LSLSourceCodeRange location, string functionName,
            IReadOnlyGenericArray<ILSLReadOnlyExprNode> givenParameterExpressions)
        {
            OnError(location,
                string.Format("Overloads of \"{0}\" exist, but no overloads match the given parameters expressions.",
                    functionName));
        }


        /// <summary>
        ///     The arguments passed to an overloaded library function match up with more than one defined overload.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="functionName">The name of the overloaded library function that the user attempted to call.</param>
        /// <param name="ambiguousMatches">All of the function overloads the call to the library function matched up with.</param>
        /// <param name="givenParameterExpressions">
        ///     The parameter expressions the user attempted to pass to the overloaded library
        ///     function.
        /// </param>
        public virtual void CallToOverloadedLibraryFunctionIsAmbiguous(LSLSourceCodeRange location, string functionName,
            IReadOnlyGenericArray<ILSLFunctionSignature> ambiguousMatches,
            IReadOnlyGenericArray<ILSLReadOnlyExprNode> givenParameterExpressions)
        {
            OnError(location,
                string.Format(
                    "Overloads of \"{0}\" exist, but the given parameter expressions match more than one overload (desired function is ambiguous).",
                    functionName));
        }


        /// <summary>
        ///     The dot operator used to access the components of vectors and rotations was used on a library constant.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="libraryConstantReferenceNode">The variable reference node on the left side of the dot operator.</param>
        /// <param name="libraryConstantSignature">
        ///     The library constant signature that was referenced, retrieved from the library
        ///     data provider.
        /// </param>
        /// <param name="accessedMember">The member the user attempted to access.</param>
        public virtual void TupleComponentAccessOnLibraryConstant(LSLSourceCodeRange location,
            ILSLVariableNode libraryConstantReferenceNode,
            ILSLConstantSignature libraryConstantSignature, string accessedMember)
        {
            OnError(location,
                "The member access operator is not allowed on library constants, even if they are vector or rotation constants.");
        }


        /// <summary>
        ///     A call to function was attempted in a static context. (in a global variable declaration expression) <para/>
        ///     This will occur even for undeclared functions.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="functionName">The name of the function.</param>
        public virtual void CallToFunctionInStaticContext(LSLSourceCodeRange location, string functionName)
        {
            OnError(location, "Functions cannot be called in a static context.");
        }


        /// <summary>
        ///     A binary operator was used in a static context.  <para/>
        ///     (in a global variable declaration expression)
        /// </summary>
        /// <param name="location">The location of the error.</param>
        public virtual void BinaryOperatorInStaticContext(LSLSourceCodeRange location)
        {
            OnError(location, "Binary operators cannot be used in a static context.");
        }


        /// <summary>
        ///     A parenthesized expression was used in a static context.  <para/>
        ///     (in a global variable declaration expression)
        /// </summary>
        /// <param name="location">The location of the error.</param>
        public virtual void ParenthesizedExpressionInStaticContext(LSLSourceCodeRange location)
        {
            OnError(location, "Parenthesized expressions cannot be used in a static context.");
        }


        /// <summary>
        ///     A postfix expression was used in a static context.  <para/>
        ///     (in a global variable declaration expression)
        /// </summary>
        /// <param name="location">The location of the error.</param>
        public virtual void PostfixOperationInStaticContext(LSLSourceCodeRange location)
        {
            OnError(location, "Postfix expressions cannot be used in a static context.");
        }


        /// <summary>
        ///     An invalid prefix expression was used in a static context (a global variable declaration expression) <para/>
        ///     Negate is the only prefix operator allowed in a static context, and only on literals and not variables.
        /// </summary>
        /// <param name="location">The location of the error.</param>
        /// <param name="type">The operation type.</param>
        public virtual void InvalidPrefixOperationUsedInStaticContext(LSLSourceCodeRange location,
            LSLPrefixOperationType type)
        {
            OnError(location,
                string.Format("The Prefix operator '{0}' cannot be used in a static context.", type.ToOperatorString()));
        }


        /// <summary>
        ///     A prefix operator was used on a global variable reference in a static context.  <para/>
        ///     (in a global variable declaration expression)
        /// </summary>
        /// <param name="location">The location of the error.</param>
        /// <param name="type">The operation type.</param>
        public virtual void PrefixOperationOnGlobalVariableInStaticContext(LSLSourceCodeRange location,
            LSLPrefixOperationType type)
        {
            OnError(location,
                string.Format("The Prefix operator '{0}' cannot be used in a static context with a variable operand.",
                    type.ToOperatorString()));
        }


        /// <summary>
        ///     A postfix operation was applied to an expression that was not a variable.
        /// </summary>
        /// <param name="location">The location of the error.</param>
        /// <param name="type">The operation type.</param>
        public virtual void PostfixOperationOnNonVariable(LSLSourceCodeRange location, LSLPostfixOperationType type)
        {
            OnError(location,
                string.Format("The Postfix operator '{0}' cannot be used with a non-variable operand.",
                    type.ToOperatorString()));
        }


        /// <summary>
        ///     A '.' member access operation was attempted on an invalid variable type, or the variable type did not contain the given member.  <para/>
        ///     Valid member names for vectors are:  "x", "y" and "z" <para/>
        ///     Valid member names for rotations are:  "x", "y", "z" and "s" <para/>
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="exprLvalue">The variable expression on the left side of the dot operator.</param>
        /// <param name="memberAccessed">The member/component name on the right side of the dot operator.</param>
        public virtual void InvalidTupleComponentAccessOperation(LSLSourceCodeRange location, ILSLReadOnlyExprNode exprLvalue,
            string memberAccessed)
        {
            OnError(location,
                string.Format("\".{0}\" member access operator is not valid on {1}'s.", memberAccessed, exprLvalue.Type));
        }


        /// <summary>
        ///     The return type of an expression used in an if statement condition is not a valid type.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="attemptedConditionExpression">The invalid expression in the condition area of the if statement.</param>
        public virtual void IfConditionNotValidType(LSLSourceCodeRange location,
            ILSLReadOnlyExprNode attemptedConditionExpression)
        {
            OnError(location, string.Format(
                "If condition must evaluate to an Integer, Key, Vector, Rotation or List" +
                " but given expression evaluates to {0}.",
                attemptedConditionExpression.DescribeType()));
        }


        /// <summary>
        ///     The return type of an expression used in an else-if statement condition is not a valid type.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="attemptedConditionExpression">The invalid expression in the condition area of the else-if statement.</param>
        public virtual void ElseIfConditionNotValidType(LSLSourceCodeRange location,
            ILSLReadOnlyExprNode attemptedConditionExpression)
        {
            OnError(location, string.Format(
                "Else If condition must evaluate to an Integer, Key, Vector, Rotation or List" +
                " but given expression evaluates to {0}.",
                attemptedConditionExpression.DescribeType()));
        }


        /// <summary>
        ///     The return type of an expression used in a do-loops condition is not a valid type.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="attemptedConditionExpression">The invalid expression in the condition area of the do-loop.</param>
        public virtual void DoLoopConditionNotValidType(LSLSourceCodeRange location,
            ILSLReadOnlyExprNode attemptedConditionExpression)
        {
            OnError(location, string.Format(
                "Do loop condition must evaluate to an Integer, Key, Vector, Rotation or List" +
                " but given expression evaluates to {0}.", attemptedConditionExpression.DescribeType()));
        }


        /// <summary>
        ///     The return type of an expression used in a while-loops condition is not a valid type.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="attemptedConditionExpression">The invalid expression in the condition area of the while-loop.</param>
        public virtual void WhileLoopConditionNotValidType(LSLSourceCodeRange location,
            ILSLReadOnlyExprNode attemptedConditionExpression)
        {
            OnError(location, string.Format(
                "While loop condition must evaluate to an Integer, Key, Vector, Rotation or List" +
                " but given expression evaluates to {0}.",
                attemptedConditionExpression.DescribeType()));
        }


        /// <summary>
        ///     The return type of an expression used in a for-loops condition is not a valid type.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="attemptedConditionExpression">The invalid expression in the condition area of the for-loop.</param>
        public virtual void ForLoopConditionNotValidType(LSLSourceCodeRange location,
            ILSLReadOnlyExprNode attemptedConditionExpression)
        {
            OnError(location, string.Format(
                "For loop condition must evaluate to an Integer, Key, Vector, Rotation or List" +
                " but given expression evaluates to {0}.",
                attemptedConditionExpression.DescribeType()));
        }


        /// <summary>
        ///     A parameter type mismatch occured when trying to call a user defined or library function.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="parameterIndexWithError">The index of the parameter with the type mismatch. (Zero based)</param>
        /// <param name="calledFunction">The defined/library function that was attempting to be called.</param>
        /// <param name="parameterExpressionsGiven">The parameter expressions given for the function call.</param>
        public virtual void ParameterTypeMismatchInFunctionCall(LSLSourceCodeRange location,
            int parameterIndexWithError,
            ILSLFunctionSignature calledFunction, ILSLReadOnlyExprNode[] parameterExpressionsGiven)
        {
            var message = calledFunction.HasVariadicParameter
                ? "Type Mismatch in call to Variadic Function \"{0}\" at Parameter #{1} ({2}), expected {3} and got {4}."
                : "Type Mismatch in call to Function \"{0}\" at Parameter #{1} ({2}), expected {3} and got {4}.";

            OnError(location, string.Format(
                message,
                calledFunction.Name,
                parameterIndexWithError+1,
                calledFunction.Parameters[parameterIndexWithError].Name,
                calledFunction.Parameters[parameterIndexWithError].Type,
                parameterExpressionsGiven[parameterIndexWithError].DescribeType()));
        }


        /// <summary>
        ///     A state name was re-defined.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="stateName">The name of the state being redefined.</param>
        public virtual void RedefinedStateName(LSLSourceCodeRange location, string stateName)
        {
            OnError(location, "State \"" + stateName + "\" has already been defined.");
        }


        /// <summary>
        ///     An event handler was declared that was not defined in the library data provider.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="givenEventHandlerSignature">The signature of the event handler attempting to be used.</param>
        public virtual void UnknownEventHandlerDeclared(LSLSourceCodeRange location,
            ILSLEventSignature givenEventHandlerSignature)
        {
            OnError(location,
                "Event handler \"" + givenEventHandlerSignature.Name + "\" is not a valid LSL event handler.");
        }


        /// <summary>
        ///     An event handler was declared in the program with a call signature differing from its definition in the library data provider.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="givenEventHandlerSignature">The invalid signature used for the event handler in the source code.</param>
        /// <param name="correctEventHandlerSignature">
        ///     The correct signature for for the event handler; the one that was expected.
        /// </param>
        public virtual void IncorrectEventHandlerSignature(LSLSourceCodeRange location,
            ILSLEventSignature givenEventHandlerSignature,
            ILSLEventSignature correctEventHandlerSignature)
        {
            OnError(location,
                "Event handler \"" + correctEventHandlerSignature.Name + "\" has incorrect parameter definitions.");
        }


        /// <summary>
        ///     A standard library constant defined in the library data provider was redefined in source code as a global or local
        ///     variable.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="redefinitionType">The type used in the re-definition.</param>
        /// <param name="originalSignature">The original signature of the constant taken from the library data provider.</param>
        public virtual void RedefinedStandardLibraryConstant(LSLSourceCodeRange location, LSLType redefinitionType,
            ILSLConstantSignature originalSignature)
        {
            OnError(location,
                "Cannot define variable with name \"" + originalSignature.Name +
                "\" as it's the name of an existing default library constant.");
        }


        /// <summary>
        ///     A library function that exist in the library data provider was redefined by the user.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="functionName">The name of the library function the user attempted to redefine.</param>
        /// <param name="libraryFunctionSignatureOverloads">
        ///     All of the overloads for the library function, there may only be one if
        ///     no overloads actually exist.
        /// </param>
        public virtual void RedefinedStandardLibraryFunction(LSLSourceCodeRange location, string functionName,
            IReadOnlyGenericArray<ILSLFunctionSignature> libraryFunctionSignatureOverloads)
        {
            OnError(location,
                "Cannot define function with name \"" + functionName +
                "\" as it's the name of an existing default library function.");
        }


        /// <summary>
        ///     A state change statement was encountered that referenced an undefined state name.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="stateName">The undefined state name referenced.</param>
        public virtual void ChangeToUndefinedState(LSLSourceCodeRange location, string stateName)
        {
            OnError(location,
                "Cannot change to state \"" + stateName +
                "\" as a state with that name does not exist.");
        }


        /// <summary>
        ///     An attempt to modify a library constant defined in the library data provider was made.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="constantName">The name of the constant the user attempted to modified.</param>
        public virtual void ModifiedLibraryConstant(LSLSourceCodeRange location, string constantName)
        {
            OnError(location,
                "Cannot modify library constant \"" + constantName + "\"");
        }


        /// <summary>
        ///     The user attempted to use 'default' for a user defined state name.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        public virtual void RedefinedDefaultState(LSLSourceCodeRange location)
        {
            OnError(location,
                "Cannot defined a new state with the name \"default\" as that is the name of LSL's default state.");
        }


        /// <summary>
        ///     A modifying prefix operation was applied to an expression that was not a variable.
        /// </summary>
        /// <param name="location">The location of the error.</param>
        /// <param name="type">The operation type.</param>
        public virtual void ModifyingPrefixOperationOnNonVariable(LSLSourceCodeRange location,
            LSLPrefixOperationType type)
        {
            OnError(location,
                string.Format("Prefix operator '{0}' cannot be used with a non-variable operand.",
                    type.ToOperatorString()));
        }


        /// <summary>
        ///     The negate prefix operator was used on a vector literal in a static context. <para/>
        ///      (in a global variable declaration expression)
        /// </summary>
        /// <param name="location">The location of the error.</param>
        public virtual void NegateOperationOnVectorLiteralInStaticContext(LSLSourceCodeRange location)
        {
            OnError(location, "The negate operator cannot be used on vector literals in a static context.");
        }


        /// <summary>
        ///     The negate prefix operator was used on a rotation literal in a static context. <para/>
        ///     (in a global variable declaration expression)
        /// </summary>
        /// <param name="location">The location of the error.</param>
        public virtual void NegateOperationOnRotationLiteralInStaticContext(LSLSourceCodeRange location)
        {
            OnError(location, "The negate operator cannot be used on rotation literals in a static context.");
        }


        /// <summary>
        ///     A cast expression was used inside of a static context. <para/>
        ///     (in a global variable declaration expression)
        /// </summary>
        /// <param name="location">The location of the error.</param>
        public virtual void CastExpressionInStaticContext(LSLSourceCodeRange location)
        {
            OnError(location, "Cast expressions cannot be used in a static context.");
        }


        /// <summary>
        ///     Occurs with an expression that is left of an assignment type operator is not assignable.
        ///     This includes compound assignment operators such as: += ... <para/>
        ///     This error occurs only for left expressions that are not library constants.
        ///     There is a separate error for library constants, see <see cref="ILSLSyntaxErrorListener.ModifiedLibraryConstant" />.
        /// </summary>
        /// <param name="location">The source code range of the assignment operator used.</param>
        /// <param name="assignmentOperatorUsed">The assignment operator used.</param>
        public void AssignmentToNonassignableExpression(LSLSourceCodeRange location, string assignmentOperatorUsed)
        {
            OnError(location,
                "Expression left of assignment operator '" + assignmentOperatorUsed + "' is not assignable.");
        }


        /// <summary>
        ///     A cast expression was used directly on another cast expression without parenthesizing the expression on the right. <para/>
        ///     LibLSLCC's parser can handle this, but Secondlife's LSL parser cannot; it's ambiguous to it and causes a syntax error.
        /// </summary>
        /// <param name="location">The source code range of the offending cast expression.</param>
        /// <param name="castedExpression">The casted expression, which will be another typecast expression.</param>
        public void CastOnCastExpression(LSLSourceCodeRange location, ILSLReadOnlyExprNode castedExpression)
        {
            OnError(location,
                "Cannot use cast operator directly on another cast expression; use parentheses around the second cast.");
        }


        /// <summary>
        ///     A hook for intercepting error messages produced by the implementations of all other functions in the
        ///     LSLDefaultSyntaxErrorListener object.
        ///     The default behavior is to write error messages to the Console.
        /// </summary>
        /// <param name="location">Location in source code for the error.</param>
        /// <param name="message">The error message.</param>
        protected virtual void OnError(LSLSourceCodeRange location, string message)
        {
            Console.WriteLine("({0},{1}) ERROR: {2}", MapLineNumber(location.LineStart), location.ColumnStart,
                message + Environment.NewLine);
        }


        /// <summary>
        ///     A hook to allow the modification of line numbers used in either the header of an error or the body.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         You should pass line numbers you wish to put into customized error messages
        ///         through this function so that the derived class can easily offset them.
        ///         Line numbers reported in syntax errors default to using a 'one' based index where
        ///         line #1 is the first line of source code.  To modify all line numbers reported in syntax
        ///         errors you could overload this function and return the passed in value with some offset
        ///         added/subtracted.
        ///         For example, if you want all line number references in errors sent to OnError to have a 0 based index.
        ///         then you should return (oneBasedLine-1) from this function.
        ///     </para>
        /// </remarks>
        /// <param name="oneBasedLine">
        ///     The 'one' based line number that we might need to modify, a common modification would be to
        ///     subtract 1 from it.
        /// </param>
        /// <returns>The possibly modified line number to use in the error message.</returns>
        protected virtual int MapLineNumber(int oneBasedLine)
        {
            return oneBasedLine;
        }
    }
}