﻿#region FileInfo
// 
// File: LSLLibraryConstantSignature.cs
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
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using LibLSLCC.CodeValidator;
using LibLSLCC.CodeValidator.Components.Interfaces;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.Collections;
using LibLSLCC.Utility;

#endregion

namespace LibLSLCC.LibraryData
{
    /// <summary>
    /// Represents the signature of a constant provided from an <see cref="ILSLLibraryDataProvider"/> implementation
    /// </summary>
    [XmlRoot("LibraryConstant")]
    public sealed class LSLLibraryConstantSignature : IXmlSerializable, ILSLLibrarySignature
    {
        private Dictionary<string, string> _properties = new Dictionary<string, string>();
        private LSLLibraryDataSubsetCollection _subsets = new LSLLibraryDataSubsetCollection();



        private string _name;
        private string _valueString;
        private LSLType _type;

        private LSLLibraryConstantSignature()
        {
            DocumentationString = "";
        }

        /// <summary>
        /// Construct the LSLLibraryConstantSingature by cloning another one.
        /// </summary>
        /// <param name="other"></param>
        public LSLLibraryConstantSignature(LSLLibraryConstantSignature other)
        {
            
            DocumentationString = other.DocumentationString;
            _subsets = new LSLLibraryDataSubsetCollection(other._subsets);
            _properties = other._properties.ToDictionary(x => x.Key, y => y.Value);
            _type = other.Type;
            _valueString = other.ValueString;
        }


        /// <summary>
        /// Construct the LSLLibraryConstantSignature from a given <see cref="LSLType"/> and constant name. <see cref="ValueString"/> is given the default
        /// value for the given <see cref="LSLType"/> passed in <paramref name="type"/>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        public LSLLibraryConstantSignature(LSLType type, string name)
        {
            DocumentationString = "";
            Name = name;
            Type = type;

            //use _valueString to bypass validation, since its faster
            //and we know what is required by the class.
            switch (type)
            {
                case LSLType.Key:
                    _valueString = "00000000-0000-0000-0000-000000000000";
                    break;
                case LSLType.Integer:
                    _valueString = "0";
                    break;
                case LSLType.String:
                    _valueString = "";
                    break;
                case LSLType.Float:
                    _valueString = "0.0";
                    break;
                case LSLType.List:
                    _valueString = "";    //empty list
                    break;
                case LSLType.Vector:
                    _valueString = "0,0,0";
                    break;
                case LSLType.Rotation:
                    _valueString = "0,0,0,0";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }
        }


        /// <summary>
        /// Construct the LSLLibraryConstantSignature from a given <see cref="LSLType"/>, constant name, and Value string.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="valueString"></param>
        public LSLLibraryConstantSignature(LSLType type, string name, string valueString)
        {
            DocumentationString = "";
            Name = name;
            Type = type;
            ValueString = valueString;
        }

        /// <summary>
        /// The library subsets this signature belongs to/is shared among.
        /// </summary>
        public LSLLibraryDataSubsetCollection Subsets { get { return _subsets; } }


        /// <summary>
        /// Additional dynamic property values that can be attached to the constant signature and parsed from XML
        /// </summary>
        public IDictionary<string, string> Properties
        {
            get { return _properties; }
        }

        /// <summary>
        /// Returns a formated signature string for the constant, in the form:  NAME = ValueStringAsCodeLiteral
        /// Without a trailing semi-colon character.
        /// </summary>
        public string SignatureString
        {
            get { return Type.ToLSLTypeString() + " " + Name + " = " + ValueStringAsCodeLiteral; }
        }

        /// <summary>
        /// Gets or sets the documentation string attached to this signature.
        /// </summary>
        public string DocumentationString { get; set; }


        /// <summary>
        /// Combines the SignatureString and Documentation string.  The signature will
        /// have a trailing semi-colon, and if there is a documentation string a new-line will
        /// be inserted between the signature and documentation string.
        /// </summary>
        public string SignatureAndDocumentation
        {
            get
            {
                if (string.IsNullOrWhiteSpace(DocumentationString))
                {
                    return SignatureString + ";";
                }
                return SignatureString + ";" +
                       Environment.NewLine +
                       DocumentationString;
            }
        }

        /// <summary>
        /// The <see cref="LSLType"/> that the library constant is defined with.
        /// </summary>
        public LSLType Type
        {
            get { return _type; }
            set
            {
                if (value == LSLType.Void)
                {
                    throw new LSLLibraryDataInvalidConstantTypeException("Library Constant's Type may not be set to Void.");
                }
                _type = value;
            }
        }

        /// <summary>
        /// The name of the library constant, must abide by LSL symbol naming rules or an exception will be thrown.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new LSLInvalidSymbolNameException("LSLFunctionSignature: Function name was null or whitespace.");
                }

                if (!LSLTokenTools.IDRegexAnchored.IsMatch(value))
                {
                    throw new LSLInvalidSymbolNameException(string.Format("LSLConstantSignature: Constant name '{0}' contained invalid characters or formating.", value));
                }

                _name = value;
            }
        }

        private static string _floatRegexString = "([-+]?[0-9]*(?:\\.[0-9]*))";
        private static Regex _vectorValidationRegex = new Regex("^"+_floatRegexString+"\\s*,\\s*"+_floatRegexString+"\\s*,\\s*"+_floatRegexString+ "$");
        private static Regex _rotationValidationRegex = new Regex("^" + _floatRegexString + "\\s*,\\s*" + _floatRegexString + "\\s*,\\s*" + _floatRegexString+ "\\s*,\\s*" + _floatRegexString + "$");


        /// <summary>
        /// The value string of the library constant, you must set <see cref="Type" /> first to a value that is not <see cref="LSLType.Void" /> or an exception will be thrown.
        /// </summary>
        /// <value>
        /// The value string.
        /// </value>
        /// <exception cref="System.ArgumentNullException">If you attempt to set the value to <c>null</c>.</exception>
        /// <exception cref="LSLInvalidConstantValueStringException">
        /// If the Value is an invalid value for a float and <see cref="Type" /> is set to <see cref="LSLType.Float" />
        /// or
        /// If the Value is an invalid value for an integer and <see cref="Type" /> is set to <see cref="LSLType.Integer" />
        /// or
        /// If the Value is an invalid value for a vector and <see cref="Type" /> is set to <see cref="LSLType.Vector" />
        /// or
        /// If the Value is an invalid value for a rotation and <see cref="Type" /> is set to <see cref="LSLType.Rotation" /></exception>
        /// <exception cref="LSLLibraryDataInvalidConstantTypeException">If you try to set this value and <see cref="Type" /> is equal to <see cref="LSLType.Void" />.</exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <remarks>
        /// Only integral or hexadecimal values are allowed when <see cref="Type" /> is set to <see cref="LSLType.Integer" />
        /// Only floating point or hexadecimal values are allowed when <see cref="Type" /> is set to <see cref="LSLType.Float" />
        /// The enclosing less than and greater than symbols will be removed when <see cref="Type" /> is set to <see cref="LSLType.Vector" /> or <see cref="LSLType.Rotation" />.
        /// </remarks>
        public string ValueString
        {
            get { return _valueString; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value",GetType().Name+".ValueString cannot be set to null.");
                }

                switch (Type)
                {
                    case LSLType.Float:
                        SetFloatValueString(value);
                        return;
                    case LSLType.Integer:
                        SetIntegerValueString(value);
                        return;
                    case LSLType.List:
                        SetListValueString(value);
                        return;
                    case LSLType.Vector:
                        SetVectorValueString(value);
                        return;
                    case LSLType.Rotation:
                        SetRotationValueString(value);
                        return;
                    case LSLType.Void:
                        throw new LSLLibraryDataInvalidConstantTypeException("Could not set ValueString because the 'Type' Properties value is set to Void.");
                }

                _valueString = value;

            }
        }

        private void SetFloatValueString(string value)
        {
            string stripSpecifiers = value.TrimEnd('f', 'F', 'd', 'D');

            double f;
            if (!double.TryParse(stripSpecifiers, out f))
            {
                int i;
                if (!int.TryParse(value, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out i))
                {
                    throw new LSLInvalidConstantValueStringException(
                        string.Format("Float Constant ValueString:  Given string '{0}' is not a valid float value.",
                            value));
                }
                _valueString = value;
            }
            _valueString = stripSpecifiers;
        }


        private void SetIntegerValueString(string value)
        {
            bool b;
            if (!bool.TryParse(value, out b))
            {
                int i;
                if (!int.TryParse(value, out i) &&
                    !int.TryParse(value, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out i))
                {
                    throw new LSLInvalidConstantValueStringException(
                        string.Format("Integer Constant ValueString:  Given value '{0}' is not a valid integer, hexadecimal or boolean value.",
                            value));
                }
                _valueString = value;
            }
            else
            {
                //make it integral
                _valueString = b ? "1" : "0";
            }
            
        }

        private void SetListValueString(string value)
        {
            try
            {
                string s = value.Trim(' ');

                if (string.IsNullOrWhiteSpace(s))
                {
                    throw new LSLInvalidConstantValueStringException(
                        "List Constant ValueString Invalid: May not be null or whitespace.");
                }


                char firstChar = s[0];
                char lastChar = s[s.Length - 1];

                if ((firstChar == '[' || lastChar == ']') &&
                    (firstChar != '[' || lastChar != ']'))
                {
                    throw new LSLInvalidConstantValueStringException(
                        "List Constant ValueString '{0}' Invalid: If brackets are used for the List value string, both brackets must be present.");
                }
                if (firstChar == '[')
                {
                    s = s.Substring(1, s.Length - 2);
                }

                if (string.IsNullOrWhiteSpace(s)) _valueString = "";

                _valueString = LSLListParser.Format("[" + s + "]", false);
            }
            catch (LSLListParserSyntaxException e)
            {
                throw new LSLInvalidConstantValueStringException("List Constant ValueString Invalid: " + e.Message);
            }
        }

        private void SetRotationValueString(string value)
        {
            string s = value.Trim(' ');


            if (string.IsNullOrWhiteSpace(s))
            {
                throw new LSLInvalidConstantValueStringException(
                    "Rotation Constant ValueString Invalid: May not be null or whitespace.");
            }


            char firstChar = s[0];
            char lastChar = s[s.Length - 1];

            if ((firstChar == '<' || lastChar == '>') &&
                (firstChar != '<' || lastChar != '>'))
            {
                throw new LSLInvalidConstantValueStringException(
                    "Rotation Constant ValueString '{0}' Invalid: If rotation quotes are used for a Rotation value string, both '<' and '>' must be present.");
            }

            if (firstChar == '<')
            {
                s = s.Substring(1, s.Length - 2);
            }


            var match = _rotationValidationRegex.Match(s);
            if (!match.Success)
            {
                throw new LSLInvalidConstantValueStringException(
                    string.Format("Rotation Constant ValueString: '{0}' could not be parsed and formated.", value));
            }

            _valueString = match.Groups[1] + ", " + match.Groups[2] + ", " + match.Groups[3] + ", " + match.Groups[4];
        }


        private void SetVectorValueString(string value)
        {
            string s = value.Trim(' ');


            if (string.IsNullOrWhiteSpace(s))
            {
                throw new LSLInvalidConstantValueStringException(
                    "Vector Constant ValueString Invalid: May not be null or whitespace.");
            }

            char firstChar = s[0];
            char lastChar = s[s.Length - 1];

            if ((firstChar == '<' || lastChar == '>') &&
                (firstChar != '<' || lastChar != '>'))
            {
                throw new LSLInvalidConstantValueStringException(
                    "Vector Constant ValueString '{0}' Invalid: If vector quotes are used for a Vector value string, both '<' and '>' must be present.");
            }

            if (firstChar == '<')
            {
                s = s.Substring(1, s.Length - 2);
            }


            var match = _vectorValidationRegex.Match(s);
            if (!match.Success)
            {
                throw new LSLInvalidConstantValueStringException(
                    string.Format("Vector Constant ValueString: '{0}' could not be parsed and formated.", value));
            }

            _valueString = match.Groups[1] + ", " + match.Groups[2] + ", " + match.Groups[3];
        }




        /// <summary>
        /// Returns a string which represents what this Constant would look like
        /// if it were expanded into an LSL code literal.  This takes the Type and contents
        /// of ValueString into account.
        /// </summary>
        public string ValueStringAsCodeLiteral
        {
            get
            {
                if (Type == LSLType.Key || Type == LSLType.String)
                {
                    return "\"" + LSLFormatTools.ShowControlCodeEscapes(ValueString) + "\"";
                }
                if (Type == LSLType.Vector || Type == LSLType.Rotation)
                {
                    return "<" + ValueString + ">";
                }
                if (Type == LSLType.List)
                {
                    return "[" + ValueString + "]";
                }

                return ValueString;
            }
        }


        /// <summary>
        ///     This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return
        ///     null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply the
        ///     <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute" /> to the class.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:System.Xml.Schema.XmlSchema" /> that describes the XML representation of the object that is
        ///     produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)" /> method
        ///     and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)" />
        ///     method.
        /// </returns>
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Fills a constant signature object from an XML fragment.
        /// </summary>
        /// <param name="reader">The XML reader containing the fragment to read.</param>
        /// <exception cref="LSLInvalidSymbolNameException">Thrown if the constants name does not abide by LSL symbol naming conventions.</exception>
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            reader.MoveToContent();

            var hasSubsets = false;
            var hasType = false;
            var hasName = false;
            var hasValue = false;

            string valueString = null;

            var lineNumberInfo = (IXmlLineInfo) reader;

            while (reader.MoveToNextAttribute())
            {
                if (reader.Name == "Value")
                {
                    var val = reader.Value;
                    //The value is only truly missing if its entirely devoid of character data
                    //some LSL constants like EOF are nothing but whitespace characters
                    if (val.Length != 0)
                    {
                        hasValue = true;
                    }

                    //need to set the type first, defer this until later.
                    valueString = val;
                }
                else if (reader.Name == "Subsets")
                {
                    Subsets.SetSubsets(reader.Value);
                    hasSubsets = true;
                }
                else if (reader.Name == "Type")
                {
                    LSLType type;
                    if (Enum.TryParse(reader.Value, out type))
                    {
                        Type = type;
                        hasType = true;
                    }
                    else
                    {
                        throw new LSLLibraryDataXmlSyntaxException(lineNumberInfo.LineNumber,
                            string.Format("LibraryConstantSignature{0}: Type attribute invalid.", hasName ? (" '" + Name + "'") : ""));
                    }
                }
                else if (reader.Name == "Name")
                {
                    Name = reader.Value;
                    hasName = true;
                }
                else
                {
                    throw new LSLLibraryDataXmlSyntaxException(lineNumberInfo.LineNumber,
                        string.Format("LibraryConstantSignature{0}: Unknown attribute '{1}'.", 
                        hasName ? (" '" + Name + "'") : "",  reader.Name));
                }
            }

            if (!hasName)
            {
                throw new LSLLibraryDataXmlSyntaxException(lineNumberInfo.LineNumber,
                    "LibraryConstantSignature: Missing Name attribute.");
            }

            if (!hasType)
            {
                throw new LSLLibraryDataXmlSyntaxException(lineNumberInfo.LineNumber,
                    string.Format("LibraryConstantSignature '{0}': Missing Type attribute.", Name));
            }

            if (!hasValue)
            {
                throw new LSLLibraryDataXmlSyntaxException(lineNumberInfo.LineNumber,
                    string.Format("LibraryConstantSignature '{0}': Missing Value attribute.", Name));
            }

            if (!hasSubsets)
            {
                throw new LSLLibraryDataXmlSyntaxException(lineNumberInfo.LineNumber,
                    string.Format("LibraryConstantSignature '{0}': Missing Subsets attribute.", Name));
            }

            //Set the value string, this can possibly throw an LSLInvalidConstantValueStringException
            //The Type property needs to be set first above for validation to occur.
            ValueString = valueString;


            var canRead = reader.Read();
            while (canRead)
            {
                if ((reader.Name == "DocumentationString") && reader.IsStartElement())
                {
                    DocumentationString = reader.ReadElementContentAsString();
                    canRead = reader.Read();
                }
                else if ((reader.Name == "Property") && reader.IsStartElement())
                {
                    var pName = reader.GetAttribute("Name");

                    if (string.IsNullOrWhiteSpace(pName))
                    {
                        throw new LSLLibraryDataXmlSyntaxException(lineNumberInfo.LineNumber,
                            string.Format(
                                "LibraryConstantSignature '{0}': Property element's Name attribute cannot be empty.", Name));
                    }

                    var value = reader.GetAttribute("Value");

                    if (string.IsNullOrWhiteSpace(value))
                    {
                        throw new LSLLibraryDataXmlSyntaxException(lineNumberInfo.LineNumber,
                            string.Format(
                                "LibraryConstantSignature '{0}': Property element's Value attribute cannot be empty.",Name));
                    }

                    if (_properties.ContainsKey(pName))
                    {
                        throw new LSLLibraryDataXmlSyntaxException(lineNumberInfo.LineNumber,
                            string.Format(
                                "LibraryConstantSignature '{0}': Property name '{1}' has already been used.", Name, pName));
                    }

                    _properties.Add(pName, value);

                    canRead = reader.Read();
                }
                else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "LibraryConstant")
                {
                    break;
                }
                else
                {
                    canRead = reader.Read();
                }
            }
        }

        /// <summary>
        ///     Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is serialized. </param>
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Name", Name);
            writer.WriteAttributeString("Type", Type.ToString());
            writer.WriteAttributeString("Value", ValueString);
            writer.WriteAttributeString("Subsets", string.Join(",", _subsets));

            writer.WriteStartElement("DocumentationString");
            writer.WriteString(DocumentationString);
            writer.WriteEndElement();

            foreach (var prop in Properties)
            {
                writer.WriteStartElement("Property");
                writer.WriteAttributeString("Name", prop.Key);
                writer.WriteAttributeString("Value", prop.Value);
                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Delegates to the SignatureString Property.
        /// </summary>
        /// <returns>
        /// The SignatureString Property.
        /// </returns>
        public override string ToString()
        {
            return SignatureString;
        }


        /// <summary>
        /// Reads a constant signature object from an XML fragment.
        /// </summary>
        /// <param name="reader">The XML reader containing the fragment to read.</param>
        /// <exception cref="LSLInvalidSymbolNameException">Thrown if the constants name does not abide by LSL symbol naming conventions.</exception>
        /// <returns>The parsed LSLLibraryConstantSignature object.</returns>
        public static LSLLibraryConstantSignature FromXmlFragment(XmlReader reader)
        {
            var con = new LSLLibraryConstantSignature();
            IXmlSerializable x = con;
            x.ReadXml(reader);
            return con;
        }


        /// <summary>
        /// Returns the hash code of the LSLConstantSignature object.  The Type and Name properties are used to generate the hash code.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            var hash = 17;
            


            hash = hash*31 + Type.GetHashCode();
            hash = hash*31 + Name.GetHashCode();


            return hash;
        }


        /// <summary>
        /// Determines whether the Type and Name properties of another LSLLibraryConstantSignature equal the Type and Name properties of this object.
        /// If the passed object is not an LSLLibraryConstantSignature object then the result will always be false.
        /// </summary>
        /// <param name="obj">The object to compare this object with.</param>
        /// <returns>True if the object is an LSLLibraryConstantSignature object and the Name and Type properties of both objects are equal to each other.</returns>
        public override bool Equals(object obj)
        {
            var o = obj as LSLLibraryConstantSignature;
            if (o == null)
            {
                return false;
            }

            return o.Name == Name && o.Type == Type;
        }


        /// <summary>
        /// Whether or not this constant is marked as deprecated.
        /// </summary>
        public bool Deprecated
        {
            get
            {
                string deprecatedStatus;
                return (Properties.TryGetValue("Deprecated", out deprecatedStatus) &&
                        deprecatedStatus.ToLower() == "true");
            }
            set
            {
                if (value == false)
                {
                    if (Properties.ContainsKey("Deprecated"))
                    {
                        Properties.Remove("Deprecated");
                    }
                }
                else
                {
                    Properties["Deprecated"] = "true";
                }
            }
        }

        /// <summary>
        /// A hint to compilers, if Expand is true then the constant's value should be expanded and placed
        /// into the generated code, otherwise if Expand is false the constants value should be retrieved
        /// by referencing the constant by name in the current object or some other object where constants
        /// are stored.
        /// </summary>
        public bool Expand
        {
            get {
                string expand;
                return (Properties.TryGetValue("Expand", out expand) && expand.ToLower() == "true");
            }
            set
            {
                if (value == false)
                {
                    if (Properties.ContainsKey("Expand"))
                    {
                        Properties.Remove("Expand");
                    }
                }
                else
                {
                    Properties["Expand"] = "true";
                }
            }
        }
    }
}