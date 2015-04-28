﻿using LibLSLCC.CodeValidator.Primitives;

namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLForLoopNode : ILSLReadOnlyCodeStatement, ILSLLoopNode
    {

        LSLSourceCodeRange ForKeywordSourceCodeRange { get; }

        LSLSourceCodeRange OpenParenthSourceCodeRange { get; }

        ILSLExpressionListNode InitExpressionsList { get; }

        LSLSourceCodeRange FirstSemiColonSourceCodeRange { get; }

        ILSLReadOnlyExprNode ConditionExpression { get; }

        LSLSourceCodeRange SecondSemiColonSourceCodeRange { get; }

        LSLSourceCodeRange CloseParenthSourceCodeRange { get; }


        ILSLExpressionListNode AfterthoughExpressions { get; }
        ILSLCodeScopeNode Code { get; }
        bool HasInitExpression { get; }
        bool HasConditionExpression { get; }
        bool HasAfterthoughtExpressions { get; }
    }
}