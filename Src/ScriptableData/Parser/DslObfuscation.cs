using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptableData.Parser
{
    enum ScriptableDataCodeEnum : byte
    {
        CODE_PUSH_ID,
        CODE_PUSH_STR,
        CODE_PUSH_NUM,
        CODE_PUSH_TRUE,
        CODE_PUSH_FALSE,
        CODE_MARK_PERIOD_PARENTHESIS_PARAM,
        CODE_MARK_PERIOD_BRACKET_PARAM,
        CODE_MARK_PERIOD_BRACE_PARAM,
        CODE_SET_MEMBER_ID,
        CODE_MARK_PERIOD_PARAM,
        CODE_MARK_BRACKET_PARAM,
        CODE_BUILD_HIGHORDER_FUNCTION,
        CODE_MARK_PARENTHESIS_PARAM,
        CODE_MARK_HAVE_STATEMENT,
        CODE_MARK_HAVE_EXTERN_SCRIPT,
        CODE_SET_EXTERN_SCRIPT,
        CODE_SET_FUNCTION_ID,
        CODE_BEGIN_FUNCTION,
        CODE_END_FUNCTION,
        CODE_BEGIN_STATEMENT,
        CODE_END_STATEMENT,
        CODE_BUILD_OPERATOR,
        CODE_BUILD_FIRST_TERNARY_OPERATOR,
        CODE_BUILD_SECOND_TERNARY_OPERATOR,
        CODE_NUM
    };
    delegate string EncodeStringDelegation(string s);
    delegate string DecodeStringDelegation(string s);
    class TokenTempStore
    {
        internal DecodeStringDelegation onDecodeString
        {
            get { return mDecodeString; }
            set { mDecodeString = value; }
        }
        internal void setLastToken(string token)
        {
            mLastToken = decode(token);
        }
        internal string getLastToken()
        {
            return mLastToken;
        }

        private string decode(string s)
        {
            if (null != mDecodeString)
                return mDecodeString(s);
            else
                return s;
        }

        private DecodeStringDelegation mDecodeString;
        private string mLastToken = "";
    }
    class ObfuscationAction : DslAction
    {
#if FULL_VERSION
        internal GetLastTokenDelegation onGetLastToken
        {
            get { return mGetLastToken; }
            set { mGetLastToken = value; }
        }
        internal EncodeStringDelegation onEncodeString
        {
            get { return mEncodeString; }
            set { mEncodeString = value; }
        }
        internal string getObfuscatedCode()
        {
            return string.Format("{0}|{1}", mObfuscateCode.ToString(), string.Join("`", mIdentifiers.ToArray()));
        }
#endif
        internal override void predict(short production_number)
        {
#if FULL_VERSION
            //LogSystem.Info("{0}", DslString.GetProductionName(production_number));
#endif
        }
        internal override void execute(int number)
        {
#if FULL_VERSION
            switch (number)
            {
                case 1: endStatement(); break;
                case 2: pushId(); break;
                case 3: buildOperator(); break;
                case 4: buildFirstTernaryOperator(); break;
                case 5: buildSecondTernaryOperator(); break;
                case 6: beginStatement(); break;
                case 7: beginFunction(); break;
                case 8: endFunction(); break;
                case 9: setFunctionId(); break;
                case 10: markHaveStatement(); break;
                case 11: markHaveExternScript(); break;
                case 12: setExternScript(); break;
                case 13: markParenthesisParam(); break;
                case 14: buildHighOrderFunction(); break;
                case 15: markBracketParam(); break;
                case 16: markPeriodParam(); break;
                case 17: setMemberId(); break;
                case 18: markPeriodParenthesisParam(); break;
                case 19: markPeriodBracketParam(); break;
                case 20: markPeriodBraceParam(); break;
                case 21: pushStr(); break;
                case 22: pushNum(); break;
                case 23: pushTrue(); break;
                case 24: pushFalse(); break;
            }
#endif
        }
#if FULL_VERSION
        private void buildOperator()
        {
            genCode(ScriptableDataCodeEnum.CODE_BUILD_OPERATOR);
        }
        private void buildFirstTernaryOperator()
        {
            genCode(ScriptableDataCodeEnum.CODE_BUILD_FIRST_TERNARY_OPERATOR);
        }
        private void buildSecondTernaryOperator()
        {
            genCode(ScriptableDataCodeEnum.CODE_BUILD_SECOND_TERNARY_OPERATOR);
        }
        private void beginStatement()
        {
            genCode(ScriptableDataCodeEnum.CODE_BEGIN_STATEMENT);
        }
        private void endStatement()
        {
            genCode(ScriptableDataCodeEnum.CODE_END_STATEMENT);
        }
        private void beginFunction()
        {
            genCode(ScriptableDataCodeEnum.CODE_BEGIN_FUNCTION);
        }
        private void setFunctionId()
        {
            genCode(ScriptableDataCodeEnum.CODE_SET_FUNCTION_ID);
        }
        private void setMemberId()
        {
            genCode(ScriptableDataCodeEnum.CODE_SET_MEMBER_ID);
        }
        private void endFunction()
        {
            genCode(ScriptableDataCodeEnum.CODE_END_FUNCTION);
        }
        private void buildHighOrderFunction()
        {
            genCode(ScriptableDataCodeEnum.CODE_BUILD_HIGHORDER_FUNCTION);
        }
        private void markParenthesisParam()
        {
            genCode(ScriptableDataCodeEnum.CODE_MARK_PARENTHESIS_PARAM);
        }
        private void markBracketParam()
        {
            genCode(ScriptableDataCodeEnum.CODE_MARK_BRACKET_PARAM);
        }
        private void markPeriodParam()
        {
            genCode(ScriptableDataCodeEnum.CODE_MARK_PERIOD_PARAM);
        }
        private void markPeriodParenthesisParam()
        {
            genCode(ScriptableDataCodeEnum.CODE_MARK_PERIOD_PARENTHESIS_PARAM);
        }
        private void markPeriodBracketParam()
        {
            genCode(ScriptableDataCodeEnum.CODE_MARK_PERIOD_BRACKET_PARAM);
        }
        private void markPeriodBraceParam()
        {
            genCode(ScriptableDataCodeEnum.CODE_MARK_PERIOD_BRACE_PARAM);
        }
        private void markHaveStatement()
        {
            genCode(ScriptableDataCodeEnum.CODE_MARK_HAVE_STATEMENT);
        }
        private void markHaveExternScript()
        {
            genCode(ScriptableDataCodeEnum.CODE_MARK_HAVE_EXTERN_SCRIPT);
        }
        private void setExternScript()
        {
            genPush();
            genCode(ScriptableDataCodeEnum.CODE_SET_EXTERN_SCRIPT);
        }
        private void pushId()
        {
            genPush();
            genCode(ScriptableDataCodeEnum.CODE_PUSH_ID);
        }
        private void pushNum()
        {
            genPush();
            genCode(ScriptableDataCodeEnum.CODE_PUSH_NUM);
        }
        private void pushStr()
        {
            genPush();
            genCode(ScriptableDataCodeEnum.CODE_PUSH_STR);
        }
        private void pushTrue()
        {
            genCode(ScriptableDataCodeEnum.CODE_PUSH_TRUE);
        }
        private void pushFalse()
        {
            genCode(ScriptableDataCodeEnum.CODE_PUSH_FALSE);
        }

        private void genPush()
        {
            string token = getLastToken();
            token = encode(token);
            mIdentifiers.Add(token);
        }
        private void genCode(ScriptableDataCodeEnum code)
        {
            mObfuscateCode.Append(((byte)code).ToString("d2"));
        }
        private string getLastToken()
        {
            if (null != mGetLastToken)
                return mGetLastToken();
            else
                return "";
        }
        private string encode(string s)
        {
            if (null != onEncodeString)
                return onEncodeString(s);
            else
                return s;
        }

        private GetLastTokenDelegation mGetLastToken;
        private EncodeStringDelegation mEncodeString;
        private StringBuilder mObfuscateCode = new StringBuilder();
        private List<string> mIdentifiers = new List<string>();
#endif

        internal static void parse(string obfuscatedCode, RuntimeAction action, DecodeStringDelegation decode)
        {
            if (string.IsNullOrEmpty(obfuscatedCode))
                return;
            int split = obfuscatedCode.IndexOf('|');
            if (split <= 0)
                return;

            TokenTempStore tokenStore = new TokenTempStore();
            if (null != decode)
            {
                tokenStore.onDecodeString = decode;
            }
            action.onGetLastToken = tokenStore.getLastToken;

            string code = obfuscatedCode.Substring(0, split);
            string[] ids = obfuscatedCode.Substring(split + 1).Split('`');
            int idIndex = 0;
            for (int i = 0; i < code.Length; i += 2)
            {
                string t = code.Substring(i, 2);
                byte c = byte.Parse(t);
                ScriptableDataCodeEnum cc = (ScriptableDataCodeEnum)c;
                switch (cc)
                {
                    case ScriptableDataCodeEnum.CODE_PUSH_ID:
                        tokenStore.setLastToken(nextToken(ids, ref idIndex));
                        action.pushId();
                        break;
                    case ScriptableDataCodeEnum.CODE_PUSH_NUM:
                        tokenStore.setLastToken(nextToken(ids, ref idIndex));
                        action.pushNum();
                        break;
                    case ScriptableDataCodeEnum.CODE_PUSH_STR:
                        tokenStore.setLastToken(nextToken(ids, ref idIndex));
                        action.pushStr();
                        break;
                    case ScriptableDataCodeEnum.CODE_PUSH_TRUE:
                        action.pushTrue();
                        break;
                    case ScriptableDataCodeEnum.CODE_PUSH_FALSE:
                        action.pushFalse();
                        break;
                    case ScriptableDataCodeEnum.CODE_MARK_PERIOD_PARENTHESIS_PARAM:
                        action.markPeriodParenthesisParam();
                        break;
                    case ScriptableDataCodeEnum.CODE_MARK_PERIOD_BRACKET_PARAM:
                        action.markPeriodBracketParam();
                        break;
                    case ScriptableDataCodeEnum.CODE_MARK_PERIOD_BRACE_PARAM:
                        action.markPeriodBraceParam();
                        break;
                    case ScriptableDataCodeEnum.CODE_SET_MEMBER_ID:
                        action.setMemberId();
                        break;
                    case ScriptableDataCodeEnum.CODE_MARK_PERIOD_PARAM:
                        action.markPeriodParam();
                        break;
                    case ScriptableDataCodeEnum.CODE_MARK_BRACKET_PARAM:
                        action.markBracketParam();
                        break;
                    case ScriptableDataCodeEnum.CODE_BUILD_HIGHORDER_FUNCTION:
                        action.buildHighOrderFunction();
                        break;
                    case ScriptableDataCodeEnum.CODE_MARK_PARENTHESIS_PARAM:
                        action.markParenthesisParam();
                        break;
                    case ScriptableDataCodeEnum.CODE_MARK_HAVE_STATEMENT:
                        action.markHaveStatement();
                        break;
                    case ScriptableDataCodeEnum.CODE_MARK_HAVE_EXTERN_SCRIPT:
                        action.markHaveExternScript();
                        break;
                    case ScriptableDataCodeEnum.CODE_SET_EXTERN_SCRIPT:
                        tokenStore.setLastToken(nextToken(ids, ref idIndex));
                        action.setExternScript();
                        break;
                    case ScriptableDataCodeEnum.CODE_SET_FUNCTION_ID:
                        action.setFunctionId();
                        break;
                    case ScriptableDataCodeEnum.CODE_BEGIN_FUNCTION:
                        action.beginFunction();
                        break;
                    case ScriptableDataCodeEnum.CODE_END_FUNCTION:
                        action.endFunction();
                        break;
                    case ScriptableDataCodeEnum.CODE_BEGIN_STATEMENT:
                        action.beginStatement();
                        break;
                    case ScriptableDataCodeEnum.CODE_END_STATEMENT:
                        action.endStatement();
                        break;
                    case ScriptableDataCodeEnum.CODE_BUILD_OPERATOR:
                        action.buildOperator();
                        break;
                    case ScriptableDataCodeEnum.CODE_BUILD_FIRST_TERNARY_OPERATOR:
                        action.buildFirstTernaryOperator();
                        break;
                    case ScriptableDataCodeEnum.CODE_BUILD_SECOND_TERNARY_OPERATOR:
                        action.buildSecondTernaryOperator();
                        break;
                }
            }
        }

        private static string nextToken(string[] tokens, ref int index)
        {
            if (null != tokens && index < tokens.Length)
            {
                return tokens[index++];
            }
            else
            {
                return "";
            }
        }
    }
}
