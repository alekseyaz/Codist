﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using AppHelpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.Text.Classification;

namespace Codist
{
	sealed class SymbolFormatter
	{
		public Brush Class { get; private set; }
		public Brush Const { get; private set; }
		public Brush Delegate { get; private set; }
		public Brush Enum { get; private set; }
		public Brush Field { get; private set; }
		public Brush Interface { get; private set; }
		public Brush Keyword { get; private set; }
		public Brush Method { get; private set; }
		public Brush Namespace { get; private set; }
		public Brush Number { get; private set; }
		public Brush Parameter { get; private set; }
		public Brush Property { get; private set; }
		public Brush Struct { get; private set; }
		public Brush Text { get; private set; }
		public Brush TypeParameter { get; private set; }

		internal void ToUIText(System.Windows.Documents.InlineCollection text, ISymbol symbol, string alias) {
			switch (symbol.Kind) {
				case SymbolKind.ArrayType:
					ToUIText(text, (symbol as IArrayTypeSymbol).ElementType, alias);
					if (alias == null) {
						text.Add("[]");
					}
					return;
				case SymbolKind.Event: text.Add(symbol.Render(alias, Delegate)); return;
				case SymbolKind.Field:
					text.Add(symbol.Render(alias, (symbol as IFieldSymbol).IsConst ? Const : Field));
					return;
				case SymbolKind.Method:
					text.Add(symbol.Render(alias, Method));
					var method = symbol as IMethodSymbol;
					if (method.IsGenericMethod) {
						var arguments = method.TypeParameters;
						AddTypeArguments(text, arguments);
					}
					return;
				case SymbolKind.NamedType:
					var type = symbol as INamedTypeSymbol;
					switch (type.TypeKind) {
						case TypeKind.Class:
							text.Add(symbol.Render(alias, Class)); break;
						case TypeKind.Delegate:
							text.Add(symbol.Render(alias, Delegate)); return;
						case TypeKind.Dynamic:
							text.Add(symbol.Render(alias ?? symbol.Name, Keyword)); return;
						case TypeKind.Enum:
							text.Add(symbol.Render(alias, Enum)); return;
						case TypeKind.Interface:
							text.Add(symbol.Render(alias, Interface)); break;
						case TypeKind.Struct:
							text.Add(symbol.Render(alias, Struct)); break;
						case TypeKind.TypeParameter:
							text.Add(symbol.Render(alias ?? symbol.Name, TypeParameter)); return;
						default:
							text.Add(symbol.MetadataName.Render(Class)); return;
					}
					if (type.IsGenericType) {
						var arguments = type.TypeParameters;
						AddTypeArguments(text, arguments);
					}
					return;
				case SymbolKind.Namespace: text.Add(symbol.Name.Render(Namespace)); return;
				case SymbolKind.Parameter: text.Add(symbol.Name.Render(Parameter)); return;
				case SymbolKind.Property: text.Add(symbol.Render(alias, Property)); return;
				case SymbolKind.TypeParameter: text.Add(symbol.Name.Render(TypeParameter)); return;
				default: text.Add(symbol.Name); return;
			}
		}

		internal TextBlock ToUIText(TextBlock block, ImmutableArray<SymbolDisplayPart> parts, int argIndex) {
			foreach (var part in parts) {
				switch (part.Kind) {
					case SymbolDisplayPartKind.AliasName:
						//todo resolve alias type
						goto default;
					case SymbolDisplayPartKind.ClassName:
						if ((part.Symbol as INamedTypeSymbol).IsAnonymousType) {
							block.AddText("?", Class);
						}
						else {
							block.AddSymbol(part.Symbol, argIndex == Int32.MinValue, Class);
						}
						break;
					case SymbolDisplayPartKind.EnumName:
						block.AddSymbol(part.Symbol, argIndex == Int32.MinValue, Enum);
						break;
					case SymbolDisplayPartKind.InterfaceName:
						block.AddSymbol(part.Symbol, argIndex == Int32.MinValue, Interface);
						break;
					case SymbolDisplayPartKind.MethodName:
						block.AddSymbol(part.Symbol, argIndex != Int32.MinValue, Method);
						break;
					case SymbolDisplayPartKind.ParameterName:
						var p = part.Symbol as IParameterSymbol;
						if (p.Ordinal == argIndex || p.IsParams && argIndex > p.Ordinal) {
							block.AddText(p.Name, true, true, Parameter);
						}
						else {
							block.AddText(p.Name, false, false, Parameter);
						}
						break;
					case SymbolDisplayPartKind.StructName:
						block.AddSymbol(part.Symbol, argIndex == Int32.MinValue, Struct);
						break;
					case SymbolDisplayPartKind.DelegateName:
						block.AddSymbol(part.Symbol, argIndex == Int32.MinValue, Delegate);
						break;
					case SymbolDisplayPartKind.StringLiteral:
						block.AddText(part.ToString(), false, false, Text);
						break;
					case SymbolDisplayPartKind.Keyword:
						block.AddText(part.ToString(), false, false, Keyword);
						break;
					case SymbolDisplayPartKind.NamespaceName:
						block.AddText(part.Symbol.Name, Namespace);
						break;
					case SymbolDisplayPartKind.TypeParameterName:
						block.AddText(part.Symbol.Name, argIndex == Int32.MinValue, false, TypeParameter);
						break;
					case SymbolDisplayPartKind.FieldName:
						block.AddText(part.Symbol.Name, Field);
						break;
					case SymbolDisplayPartKind.PropertyName:
						block.AddText(part.Symbol.Name, Property);
						break;
					case SymbolDisplayPartKind.EventName:
						block.AddText(part.Symbol.Name, Delegate);
						break;
					default:
						block.AddText(part.ToString());
						break;
				}
			}
			return block;
		}

		internal void ToUIText(TextBlock block, TypedConstant constant) {
			switch (constant.Kind) {
				case TypedConstantKind.Primitive:
					if (constant.Value is bool) {
						block.AddText((bool)constant.Value ? "true" : "false", Keyword);
					}
					else if (constant.Value is string) {
						block.AddText(constant.ToCSharpString(), Text);
					}
					else {
						block.AddText(constant.ToCSharpString(), Number);
					}
					break;
				case TypedConstantKind.Enum:
					var en = constant.ToCSharpString();
					if (en.IndexOf('|') != -1) {
						var items = constant.Type.GetMembers().Where(i => {
							var field = i as IFieldSymbol;
							return field != null
								&& field.HasConstantValue != false
								&& UnsafeArithmeticHelper.Equals(UnsafeArithmeticHelper.And(constant.Value, field.ConstantValue), field.ConstantValue)
								&& UnsafeArithmeticHelper.IsZero(field.ConstantValue) == false;
						});
						var flags = items.ToArray();
						for (int i = 0; i < flags.Length; i++) {
							if (i > 0) {
								block.AddText(" | ");
							}
							block.AddText(constant.Type.Name + "." + flags[i].Name, Enum);
						}
					}
					else {
						block.AddText(constant.Type.Name + en.Substring(en.LastIndexOf('.')), Enum);
					}
					break;
				case TypedConstantKind.Type:
					block.AddText("typeof", Keyword).AddText("(")
						.AddSymbol((constant.Value as ITypeSymbol), null, this)
						.AddText(")");
					break;
				case TypedConstantKind.Array:
					block.AddText("{");
					bool c = false;
					foreach (var item in constant.Values) {
						if (c == false) {
							c = true;
						}
						else {
							block.AddText(", ");
						}
						ToUIText(block, item);
					}
					block.AddText("}");
					break;
				default:
					block.AddText(constant.ToCSharpString());
					break;
			}
		}

		internal void UpdateSyntaxHighlights(IEditorFormatMap formatMap) {
			System.Diagnostics.Trace.Assert(formatMap != null, "format map is null");
			Interface = formatMap.GetBrush(Constants.CodeInterfaceName);
			Class = formatMap.GetBrush(Constants.CodeClassName);
			Text = formatMap.GetBrush(Constants.CodeString);
			Enum = formatMap.GetBrush(Constants.CodeEnumName);
			Delegate = formatMap.GetBrush(Constants.CodeDelegateName);
			Number = formatMap.GetBrush(Constants.CodeNumber);
			Struct = formatMap.GetBrush(Constants.CodeStructName);
			Keyword = formatMap.GetBrush(Constants.CodeKeyword);
			Namespace = formatMap.GetBrush(Constants.CSharpNamespaceName);
			Method = formatMap.GetBrush(Constants.CSharpMethodName);
			Parameter = formatMap.GetBrush(Constants.CSharpParameterName);
			TypeParameter = formatMap.GetBrush(Constants.CSharpTypeParameterName);
			Property = formatMap.GetBrush(Constants.CSharpPropertyName);
			Field = formatMap.GetBrush(Constants.CSharpFieldName);
			Const = formatMap.GetBrush(Constants.CSharpConstFieldName);
		}

		void AddTypeArguments(System.Windows.Documents.InlineCollection text, ImmutableArray<ITypeParameterSymbol> arguments) {
			text.Add("<");
			for (int i = 0; i < arguments.Length; i++) {
				if (i > 0) {
					text.Add(", ");
				}
				text.Add(arguments[i].Name.Render(TypeParameter));
			}
			text.Add(">");
		}
	}
}
