using System;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using System.Text;

namespace TaycanLogger
{
    internal enum Token
    {
      EOF,
      Add,
      Subtract,
      Multiply,
      Divide,
      Modulo,
      OpenParens,
      CloseParens,
      Index,
      Number,
    }

    internal class Tokenizer
    {
      private TextReader m_TextReader;
      private char m_Char;
      private Token m_Token;
      private double m_Value;

      internal Token Token { get => m_Token; }

      internal double Value { get => m_Value; }

      internal Tokenizer(TextReader p_TextReader)
      {
        m_TextReader = p_TextReader;
        NextChar();
        NextToken();
      }

      internal void NextToken()
      {
        while (char.IsWhiteSpace(m_Char))
          NextChar();

        m_Token = m_Char switch
        {
          '\0' => Token.EOF,
          '+' => Token.Add,
          '-' => Token.Subtract,
          '*' => Token.Multiply,
          '/' => Token.Divide,
          '%' => Token.Modulo,
          '(' => Token.OpenParens,
          ')' => Token.CloseParens,
          _ => Token.Index,
        };
        if (m_Token != Token.EOF && m_Token != Token.Index)
          NextChar();
        if (m_Token != Token.Index)
          return;

        if (char.IsDigit(m_Char) || m_Char == '.')
        {
          var v_StringBuilder = new StringBuilder();
          bool v_HasDecimalPoint = false;
          while (char.IsDigit(m_Char) || (!v_HasDecimalPoint && m_Char == '.'))
          {
            v_StringBuilder.Append(m_Char);
            v_HasDecimalPoint = m_Char == '.';
            NextChar();
          }
          m_Value = double.Parse(v_StringBuilder.ToString(), CultureInfo.InvariantCulture);
          m_Token = Token.Number;
          return;
        }

        if (m_Char == 'B')
        {
          var v_StringBuilder = new StringBuilder();
          NextChar();
          while (char.IsDigit(m_Char))
          {
            v_StringBuilder.Append(m_Char);
            NextChar();
          }
          m_Value = double.Parse(v_StringBuilder.ToString(), CultureInfo.InvariantCulture);
          m_Token = Token.Index;
          return;
        }
      }

      private void NextChar()
      {
        int v_Char = m_TextReader.Read();
        m_Char = v_Char < 0 ? '\0' : (char)v_Char;
      }
    }

    public class ConversionParser
  {
      private Tokenizer m_Tokenizer;

      public ParameterExpression ParameterArray { get; private init; }

      public ConversionParser(string p_Conversion)
      {
        m_Tokenizer = new Tokenizer(new StringReader(p_Conversion));
        ParameterArray = Expression.Parameter(typeof(byte[]));
      }

      public Expression ParseFormula()
      {
        var v_Expression = ParseAddSubtract();
        if (m_Tokenizer.Token != Token.EOF)
          throw new Exception("Unexpected characters at end of expression");
        return v_Expression;
      }

      private Expression ParseAddSubtract()
      {
        var v_ExpressionLeft = ParseMultiplyDivideModulus();
        while (true)
        {
          Token v_Operation = m_Tokenizer.Token;
          if (v_Operation != Token.Add && v_Operation != Token.Subtract)
            return v_ExpressionLeft;
          m_Tokenizer.NextToken();
          var v_ExpressionRight = ParseMultiplyDivideModulus();
          if (v_Operation == Token.Add)
            v_ExpressionLeft = Expression.Add(v_ExpressionLeft, v_ExpressionRight);
          else if (v_Operation == Token.Subtract)
            v_ExpressionLeft = Expression.Subtract(v_ExpressionLeft, v_ExpressionRight);
        }
      }

      private Expression ParseMultiplyDivideModulus()
      {
        var v_ExpressionLeft = ParseUnary();
        while (true)
        {
          Token v_Operation = m_Tokenizer.Token;
          if (v_Operation != Token.Multiply && v_Operation != Token.Divide && v_Operation != Token.Modulo)
            return v_ExpressionLeft;
          m_Tokenizer.NextToken();
          var v_ExpressionRight = ParseUnary();
          if (v_Operation == Token.Multiply)
            v_ExpressionLeft = Expression.Multiply(v_ExpressionLeft, v_ExpressionRight);
          else if (v_Operation == Token.Divide)
            v_ExpressionLeft = Expression.Divide(v_ExpressionLeft, v_ExpressionRight);
          else if (v_Operation == Token.Modulo)
            v_ExpressionLeft = Expression.Modulo(v_ExpressionLeft, v_ExpressionRight);
        }
      }

      private Expression ParseUnary()
      {
        while (true)
        {
          if (m_Tokenizer.Token == Token.Add)
            m_Tokenizer.NextToken();
          else
          {
            if (m_Tokenizer.Token == Token.Subtract)
            {
              m_Tokenizer.NextToken();
              return Expression.Negate(ParseUnary());
            }
            return ParseLeaf();
          }
        }
      }

      private Expression ParseLeaf()
      {
        if (m_Tokenizer.Token == Token.Number)
        {
          var v_Expression = Expression.Constant(m_Tokenizer.Value, typeof(double));
          m_Tokenizer.NextToken();
          return v_Expression;
        }
        if (m_Tokenizer.Token == Token.OpenParens)
        {
          m_Tokenizer.NextToken();
          var v_Expression = ParseAddSubtract();
          if (m_Tokenizer.Token != Token.CloseParens)
            throw new Exception("Missing close parenthesis");
          m_Tokenizer.NextToken();
          return v_Expression;
        }
        if (m_Tokenizer.Token == Token.Index)
        {
          var v_Expression = Expression.Constant((int)m_Tokenizer.Value, typeof(int));
          m_Tokenizer.NextToken();
          return Expression.Convert(Expression.ArrayIndex(ParameterArray, v_Expression), typeof(double));
        }
        throw new Exception($"Unexpect token: {m_Tokenizer.Token}");
      }
    }
  }
