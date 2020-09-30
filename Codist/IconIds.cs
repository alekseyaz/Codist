﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Imaging;

namespace Codist
{
	static class IconIds
	{
		public const int None = KnownImageIds.Blank;
		// sematic icons
		public const int AbstractMember = KnownImageIds.DialogTemplate;
		public const int AsyncMember = KnownImageIds.DynamicGroup;
		public const int Generic = KnownImageIds.MarkupXML;
		public const int ExtensionMethod = KnownImageIds.ExtensionMethod;
		public const int StaticMember = KnownImageIds.Link;
		public const int ReadonlyField = KnownImageIds.EncapsulateField;
		public const int ReadonlyProperty = KnownImageIds.MoveProperty;
		public const int VolatileField = KnownImageIds.SetField;
		public const int AbstractClass = KnownImageIds.AbstractClass;
		public const int SealedClass = KnownImageIds.ClassSealed;
		public const int SealedMethod = KnownImageIds.MethodSealed;
		public const int SealedEvent = KnownImageIds.EventSealed;
		public const int SealedProperty = KnownImageIds.PropertySealed;
		public const int Deconstructor = KnownImageIds.DeleteListItem;
		public const int PublicConstructor = KnownImageIds.TypePublic;
		public const int ProtectedConstructor = KnownImageIds.TypeProtected;
		public const int InternalConstructor = KnownImageIds.TypeInternal;
		public const int PrivateConstructor = KnownImageIds.TypePrivate;
		public const int PartialClass = KnownImageIds.ClassShortcut;
		public const int PartialInterface = KnownImageIds.InterfaceShortcut;
		public const int PartialStruct = KnownImageIds.StructureShortcut;
		public const int ConvertOperator = KnownImageIds.ConvertPartition;
		public const int LocalFunction = KnownImageIds.MethodSnippet;
		public const int LocalVariable = KnownImageIds.LocalVariable;
		public const int Namespace = KnownImageIds.Namespace;
		public const int Class = KnownImageIds.Class;
		public const int Structure = KnownImageIds.Structure;
		public const int Enum = KnownImageIds.Enumeration;
		public const int Interface = KnownImageIds.Interface;
		public const int Delegate = KnownImageIds.Delegate;
		public const int Field = KnownImageIds.Field;
		public const int Event = KnownImageIds.Event;
		public const int Method = KnownImageIds.Method;
		public const int Constructor = KnownImageIds.Type;
		public const int EnumField = KnownImageIds.EnumerationItemPublic;
		public const int GenericDefinition = KnownImageIds.Template;
		public const int Region = KnownImageIds.Numeric;
		public const int Unsafe = KnownImageIds.HotSpot;
		public const int UnavailableSymbol = KnownImageIds.StatusWarning;
		public const int LambdaExpression = KnownImageIds.PartitionFunction;
		public const int ParenthesizedExpression = KnownImageIds.NamedSet;
		public const int Argument = KnownImageIds.Parameter;
		public const int Attribute = KnownImageIds.FormPostBodyParameterNode;
		public const int Return = KnownImageIds.Return;
		public const int PublicSymbols = KnownImageIds.ModulePublic;
		public const int PrivateSymbols = KnownImageIds.ModulePrivate;
		public const int OverrideEvent = KnownImageIds.ModifyEvent;
		public const int OverrideProperty = KnownImageIds.ModifyProperty;
		public const int OverrideMethod = KnownImageIds.ModifyMethod;
		public const int Constant = KnownImageIds.Constant;

		// interface icons
		public const int Filter = KnownImageIds.Filter;
		public const int ClearFilter = KnownImageIds.StopFilter;
		public const int Search = KnownImageIds.SearchContract;
		public const int Project = KnownImageIds.CSProjectNode;
		public const int File = KnownImageIds.CSSourceFile;
		public const int FileEmpty = KnownImageIds.CSFile;
		public const int Module = KnownImageIds.Module;
		public const int Headings = KnownImageIds.PageHeader;
		public const int Heading1 = KnownImageIds.FlagDarkRed;
		public const int Heading2 = KnownImageIds.FlagDarkPurple;
		public const int Heading3 = KnownImageIds.FlagDarkBlue;
		public const int Heading4 = KnownImageIds.Flag;
		public const int Heading5 = KnownImageIds.FlagOutline;
		public const int ReferencedXmlDoc = KnownImageIds.GoToNextComment;
		public const int ExceptionXmlDoc = KnownImageIds.StatusInvalidOutline;
		public const int RemarksXmlDoc = KnownImageIds.CommentGroup;
		public const int ExampleXmlDoc = KnownImageIds.EnableCode;
		public const int SeeAlsoXmlDoc = KnownImageIds.Next;
		public const int ReadVariables = KnownImageIds.ExternalVariableValue;
		public const int TypeAndDelegate = KnownImageIds.EntityContainer;
		public const int ReturnValue = KnownImageIds.ReturnValue;
		public const int AnonymousType = KnownImageIds.UserDataType;
		public const int ParameterCandidate = KnownImageIds.ParameterWarning;
		public const int SymbolCandidate = KnownImageIds.CodeInformation;
		public const int InterfaceImplementation = KnownImageIds.ImplementInterface;
		public const int Disposable = KnownImageIds.PartWarning;
		public const int BaseTypes = KnownImageIds.ParentChild;
		public const int MethodOverloads = KnownImageIds.MethodSet;
		public const int XmlDocComment = KnownImageIds.Comment;
		public const int TypeParameters = KnownImageIds.TypeDefinition;
		public const int OpCodes = KnownImageIds.Binary;
		public const int Comment = KnownImageIds.CommentCode;
		public const int Uncomment = KnownImageIds.UncommentCode;
		public const int Rename = KnownImageIds.Rename;
		public const int ReorderParameters = KnownImageIds.ReorderParameters;
		public const int ExtractInterface = KnownImageIds.ExtractInterface;
		public const int ExtractMethod = KnownImageIds.ExtractMethod;
		public const int EncapsulateField = KnownImageIds.EncapsulateField;
		public const int ToggleValue = KnownImageIds.ToggleButton;
		public const int InvertOperator = KnownImageIds.Operator;
		public const int AddXmlDoc = KnownImageIds.AddComment;
		public const int TagCode = KnownImageIds.MarkupTag;
		public const int TagXmlDocSee = KnownImageIds.GoToNext;
		public const int TagXmlDocPara = KnownImageIds.ParagraphHardReturn;
		public const int TagBold = KnownImageIds.Bold;
		public const int TagItalic = KnownImageIds.Italic;
		public const int TagUnderline = KnownImageIds.Underline;
		public const int TagHyperLink = KnownImageIds.HyperLink;
		public const int TagStrikeThrough = KnownImageIds.StrikeThrough;
		public const int Marks = KnownImageIds.FlagGroup;
		public const int MarkSymbol = KnownImageIds.Flag;
		public const int UnmarkSymbol = KnownImageIds.FlagOutline;
		public const int ToggleBreakpoint = KnownImageIds.BreakpointEnabled;
		public const int ToggleBookmark = KnownImageIds.Bookmark;
		public const int Watch = KnownImageIds.Watch;
		public const int DeleteBreakpoint = KnownImageIds.DeleteBreakpoint;
		public const int FormatSelection = KnownImageIds.FormatSelection;
		public const int FormatDocument = KnownImageIds.FormatDocument;
		public const int PartialDocumentCount = KnownImageIds.OpenDocumentFromCollection;
		public const int DeclarationModifier = KnownImageIds.ControlAltDel;
		public const int LineOfCode = KnownImageIds.Code;
		public const int InstanceProducer = KnownImageIds.NewItem;
		public const int GoToDefinition = KnownImageIds.GoToDefinition;
		public const int SelectCode = KnownImageIds.BlockSelection;
		public const int SelectAll = KnownImageIds.SelectAll;
		public const int Open = KnownImageIds.Open;
		public const int OpenFolder = KnownImageIds.OpenFolder;
		public const int OpenWithVisualStudio = KnownImageIds.VisualStudio;
		public const int Load = KnownImageIds.FolderOpened;
		public const int SaveAs = KnownImageIds.SaveAs;
		public const int Copy = KnownImageIds.Copy;
		public const int Cut = KnownImageIds.Cut;
		public const int Paste = KnownImageIds.Paste;
		public const int Delete = KnownImageIds.Cancel;
		public const int Duplicate = KnownImageIds.CopyItem;
		public const int Find = KnownImageIds.QuickFind;
		public const int FindNext = KnownImageIds.FindNext;
		public const int Replace = KnownImageIds.QuickReplace;
		public const int FindInFile = KnownImageIds.FindInFile;
		public const int ReplaceInFolder = KnownImageIds.ReplaceInFolder;
		public const int SearchWebSite = KnownImageIds.OpenWebSite;
		public const int SurroundWith = KnownImageIds.AddSnippet;
		public const int ToggleParentheses = KnownImageIds.MaskedTextBox;
		public const int NewGuid = KnownImageIds.NewNamedSet;
		public const int IncrementNumber = KnownImageIds.Counter;
		public const int JoinLines = KnownImageIds.Join;
		public const int Unindent = KnownImageIds.DecreaseIndent;
		public const int Indent = KnownImageIds.IncreaseIndent;
		public const int ListMembers = KnownImageIds.ListMembers;
		public const int FindReference = KnownImageIds.ReferencedDimension;
		public const int FindReferencingSymbols = KnownImageIds.ShowBuiltIns;
		public const int FindReferrers = KnownImageIds.ShowCallerGraph;
		public const int FindTypeReferrers = KnownImageIds.ShowReferencedElements;
		public const int FindDerivedTypes = KnownImageIds.ParentChildAttribute;
		public const int FindImplementations = KnownImageIds.ImplementInterface;
		public const int FindSymbolsWithName = KnownImageIds.DisplayName;
		public const int FindOverloads = KnownImageIds.OverloadBehavior;
		public const int FindMethodsMatchingSignature = KnownImageIds.ClassMethodReference;
		public const int GoToReturnType = KnownImageIds.GoToDeclaration;
		public const int GoToType = KnownImageIds.Type;
		public const int GoToMember = KnownImageIds.GoToNext;
		public const int GoToSymbol = KnownImageIds.FindSymbol;
		public const int GoToDeclaration = KnownImageIds.GoToDeclaration;
		public const int Capitalize = KnownImageIds.Font;
		public const int Uppercase = KnownImageIds.ASerif;
		public const int EditMatches = KnownImageIds.EditAssociation;
		public const int ShowClassificationInfo = KnownImageIds.HighlightText;
		public const int TogglePinning = KnownImageIds.Unpin;
		public const int Pin = KnownImageIds.Pin;
		public const int Unpin = KnownImageIds.Unpin;
		public const int Close = KnownImageIds.Close;
		public const int SyntaxTheme = KnownImageIds.ColorPalette;
		public const int Opacity = KnownImageIds.FillOpacity;
		public const int PickColor = KnownImageIds.ColorDialog;
		public const int Reset = KnownImageIds.EmptyBucket;
		public const int ResetTheme = KnownImageIds.CleanData;

		// usage
		public const int UseToWrite = KnownImageIds.Writeable;
		public const int UseAsTypeParameter = KnownImageIds.CPPMarkupXML;
		public const int UseToCast = KnownImageIds.ReportingAction;
		public const int UseToCatch = KnownImageIds.TryCatch;
		public const int UseAsDelegate = KnownImageIds.DelegateShortcut;
		public const int AttachEvent = KnownImageIds.AddEvent;
		public const int DetachEvent = KnownImageIds.EventMissing;
	}
}
