using System;
using System.Diagnostics;
using System.Globalization;

namespace System.Reflection.Adds
{
	internal struct Token
	{
		public readonly static Token Nil;

		private int @value;

		public int Index
		{
			get
			{
				return this.@value & 16777215;
			}
		}

		public bool IsNil
		{
			get
			{
				return this.Index == 0;
			}
		}

		public System.Reflection.Adds.TokenType TokenType
		{
			get
			{
				return (System.Reflection.Adds.TokenType)((int)((long)this.@value & (ulong)-16777216));
			}
		}

		public int Value
		{
			get
			{
				return this.@value;
			}
		}

		static Token()
		{
			Token.Nil = new Token(0);
		}

		[DebuggerStepThrough]
		public Token(int value)
		{
			this.@value = value;
		}

		public Token(System.Reflection.Adds.TokenType type, int rid)
		{
			this.@value = (int)type + rid;
		}

		[DebuggerStepThrough]
		public Token(uint value)
		{
			this.@value = (int)value;
		}

		public override bool Equals(object obj)
		{
			if (obj is Token)
			{
				Token token = (Token)obj;
				return this.@value == token.@value;
			}
			if (!(obj is int))
			{
				return false;
			}
			return this.@value == (int)obj;
		}

		public override int GetHashCode()
		{
			return this.@value.GetHashCode();
		}

		public static bool IsType(int token, params System.Reflection.Adds.TokenType[] types)
		{
			for (int i = 0; i < (int)types.Length; i++)
			{
				if ((int)((long)token & (ulong)-16777216) == (int)types[i])
				{
					return true;
				}
			}
			return false;
		}

		public bool IsType(System.Reflection.Adds.TokenType type)
		{
			return this.TokenType == type;
		}

		public static bool operator ==(Token token1, Token token2)
		{
			return token1.@value == token2.@value;
		}

		public static bool operator ==(Token token1, int token2)
		{
			return token1.@value == token2;
		}

		public static bool operator ==(int token1, Token token2)
		{
			return token1 == token2.@value;
		}

		public static implicit operator Int32(Token token)
		{
			return token.@value;
		}

		public static bool operator !=(Token token1, Token token2)
		{
			return !(token1 == token2);
		}

		public static bool operator !=(Token token1, int token2)
		{
			return !(token1 == token2);
		}

		public static bool operator !=(int token1, Token token2)
		{
			return !(token1 == token2);
		}

		public override string ToString()
		{
			CultureInfo invariantCulture = CultureInfo.InvariantCulture;
			object[] tokenType = new object[] { this.TokenType, this.Index };
			return string.Format(invariantCulture, "{0}(0x{1:x})", tokenType);
		}
	}
}