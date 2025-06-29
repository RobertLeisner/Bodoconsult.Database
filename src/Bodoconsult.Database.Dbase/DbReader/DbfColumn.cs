// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

// Social Explorer

// https://github.com/SocialExplorer/FastDBF

// Licence: BSD-2-Clause license

// Copyright (c) 2016, Social Explorer

// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
// 
// * Redistributions of source code must retain the above copyright notice, this
//  list of conditions and the following disclaimer.
// 
// * Redistributions in binary form must reproduce the above copyright notice,
//  this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 
// About
// A free and open source .net library for reading/writing DBF files. Fast and easy to use. Supports writing to forward-only streams which makes it easy to write dbf files in a web server environment.


using System;

namespace Bodoconsult.Database.Dbase.DbReader
{

    /// <summary>
    /// This class represents a DBF Column.
    /// </summary>
    /// 
    /// <remarks>
    /// Note that certain properties can not be modified after creation of the object. 
    /// This is because we are locking the header object after creation of a data row,
    /// and columns are part of the header so either we have to have a lock field for each column,
    /// or make it so that certain properties such as length can only be set during creation of a column.
    /// Otherwise a user of this object could modify a column that belongs to a locked header and thus corrupt the DBF file.
    /// </remarks>
    public class DbfColumn
    {

        /*
         (FoxPro/FoxBase) Double integer *NOT* a memo field
         G 	General 	(dBASE V: like Memo) OLE Objects in MS Windows versions 
         P 	Picture 	(FoxPro) Like Memo fields, but not for text processing. 
         Y 	Currency 	(FoxPro)
         T 	DateTime 	(FoxPro)
         I 	Integer 	Length: 4 byte little endian integer 	(FoxPro)
        */

        /// <summary>
        ///  Great information on DBF located here: 
        ///  http://www.clicketyclick.dk/databases/xbase/format/data_types.html
        ///  http://www.clicketyclick.dk/databases/xbase/format/dbf.html
        /// </summary>
        public enum DbfColumnType
        {

            /// <summary>
            /// Character  less than 254 length
            /// ASCII text less than 254 characters long in dBASE. 
            /// 
            /// Character fields can be up to 32 KB long (in Clipper and FoxPro) using decimal 
            /// count as high byte in field length. It's possible to use up to 64KB long fields 
            /// by reading length as unsigned.
            /// 
            /// </summary>
            Character = 0,

            /// <summary>
            /// Number 	Length: less than 18 
            ///   ASCII text up till 18 characters long (include sign and decimal point). 
            /// 
            /// Valid characters: 
            ///    "0" - "9" and "-". Number fields can be up to 20 characters long in FoxPro and Clipper. 
            /// </summary>
            /// <remarks>
            /// We are not enforcing this 18 char limit.
            /// </remarks>
            Number = 1,

            /// <summary>
            ///  L  Logical  Length: 1    Boolean/byte (8 bit) 
            ///  
            ///  Legal values: 
            ///   ? 	Not initialised (default)
            ///   Y,y 	Yes
            ///   N,n 	No
            ///   F,f 	False
            ///   T,t 	True
            ///   Logical fields are always displayed using T/F/?. Some sources claims 
            ///   that space (ASCII 20h) is valid for not initialised. Space may occur, but is not defined. 	 
            /// </summary>
            Boolean = 2,

            /// <summary>
            /// D 	Date 	Length: 8  Date in format YYYYMMDD. A date like 0000-00- 00 is *NOT* valid. 
            /// </summary>
            Date = 3,

            /// <summary>
            /// M 	Memo 	Length: 10 	Pointer to ASCII text field in memo file 10 digits representing a pointer to a DBT block (default is blanks). 
            /// </summary>
            Memo = 4,

            /// <summary>
            /// B 	Binary 	 	(dBASE V) Like Memo fields, but not for text processing.
            /// </summary>
            Binary = 5,

            /// <summary>
            /// I 	Integer 	Length: 4 byte little endian integer 	(FoxPro)
            /// </summary>
            Integer = 6,

        }


        /// <summary>
        /// Column (field) name
        /// </summary>
        private string _mName;


        /// <summary>
        /// Field Type (Char, number, boolean, date, memo, binary)
        /// </summary>
        private DbfColumnType _mType;


        /// <summary>
        /// Offset from the start of the record
        /// </summary>
        internal int MDataAddress;


        /// <summary>
        /// Length of the data in bytes; some rules apply which are in the spec (read more above).
        /// </summary>
        private int _mLength;


        /// <summary>
        /// Decimal precision count, or number of digits afer decimal point. This applies to Number types only.
        /// </summary>
        private int _mDecimalCount;



        /// <summary>
        /// Full spec constructor sets all relevant fields.
        /// </summary>
        /// <param name="sName"></param>
        /// <param name="type"></param>
        /// <param name="nLength"></param>
        /// <param name="nDecimals"></param>
        public DbfColumn(string sName, DbfColumnType type, int nLength, int nDecimals)
        {

            Name = sName;
            _mType = type;
            _mLength = nLength;

            _mDecimalCount = type == DbfColumnType.Number ? nDecimals : 0;



            //perform some simple integrity checks...
            //-------------------------------------------

            //decimal precision:
            //we could also fix the length property with a statement like this: mLength = mDecimalCount + 2;
            if (_mDecimalCount > 0 && _mLength - _mDecimalCount <= 1)
                throw new Exception("Decimal precision can not be larger than the length of the field.");

            if (_mType == DbfColumnType.Integer)
                _mLength = 4;

            if (_mType == DbfColumnType.Binary)
                _mLength = 1;

            if (_mType == DbfColumnType.Date)
                _mLength = 8;  //Dates are exactly yyyyMMdd

            if (_mType == DbfColumnType.Memo)
                _mLength = 10;  //Length: 10 Pointer to ASCII text field in memo file. pointer to a DBT block.

            if (_mType == DbfColumnType.Boolean)
                _mLength = 1;

            //field length:
            if (_mLength <= 0)
                throw new Exception("Invalid field length specified. Field length can not be zero or less than zero.");
            else if (type != DbfColumnType.Character && type != DbfColumnType.Binary && _mLength > 255)
                throw new Exception("Invalid field length specified. For numbers it should be within 20 digits, but we allow up to 255. For Char and binary types, length up to 65,535 is allowed. For maximum compatibility use up to 255.");
            else if ((type == DbfColumnType.Character || type == DbfColumnType.Binary) && _mLength > 65535)
                throw new Exception("Invalid field length specified. For Char and binary types, length up to 65535 is supported. For maximum compatibility use up to 255.");


        }


        /// <summary>
        /// Create a new column fully specifying all properties.
        /// </summary>
        /// <param name="sName">column name</param>
        /// <param name="type">type of field</param>
        /// <param name="nLength">field length including decimal places and decimal point if any</param>
        /// <param name="nDecimals">decimal places</param>
        /// <param name="nDataAddress">offset from start of record</param>
        internal DbfColumn(string sName, DbfColumnType type, int nLength, int nDecimals, int nDataAddress) : this(sName, type, nLength, nDecimals)
        {

            MDataAddress = nDataAddress;

        }


        public DbfColumn(string sName, DbfColumnType type) : this(sName, type, 0, 0)
        {
            if (type == DbfColumnType.Number || type == DbfColumnType.Character)
                throw new Exception("For number and character field types you must specify Length and Decimal Precision.");

        }


        /// <summary>
        /// Field Name.
        /// </summary>
        public string Name
        {
            get => _mName;

            set
            {
                //name:
                if (string.IsNullOrEmpty(value))
                    throw new Exception("Field names must be at least one char long and can not be null.");

                if (value.Length > 11)
                    throw new Exception("Field names can not be longer than 11 chars.");

                _mName = value;

            }

        }


        /// <summary>
        /// Field Type (C N L D or M).
        /// </summary>
        public DbfColumnType ColumnType => _mType;


        /// <summary>
        /// Returns column type as a char, (as written in the DBF column header)
        /// N=number, C=char, B=binary, L=boolean, D=date, I=integer, M=memo
        /// </summary>
        public char ColumnTypeChar
        {
            get
            {
                switch (_mType)
                {
                    case DbfColumnType.Number:
                        return 'N';

                    case DbfColumnType.Character:
                        return 'C';

                    case DbfColumnType.Binary:
                        return 'B';

                    case DbfColumnType.Boolean:
                        return 'L';

                    case DbfColumnType.Date:
                        return 'D';

                    case DbfColumnType.Integer:
                        return 'I';

                    case DbfColumnType.Memo:
                        return 'M';

                }

                throw new Exception("Unrecognized field type!");

            }
        }


        /// <summary>
        /// Field Data Address offset from the start of the record.
        /// </summary>
        public int DataAddress => MDataAddress;

        /// <summary>
        /// Length of the data in bytes.
        /// </summary>
        public int Length => _mLength;

        /// <summary>
        /// Field decimal count in Binary, indicating where the decimal is.
        /// </summary>
        public int DecimalCount => _mDecimalCount;


        /// <summary>
        /// Returns corresponding dbf field type given a .net Type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static DbfColumnType GetDbaseType(Type type)
        {

            if (type == typeof(string))
                return DbfColumnType.Character;
            if (type == typeof(double) || type == typeof(float))
                return DbfColumnType.Number;
            if (type == typeof(bool))
                return DbfColumnType.Boolean;
            if (type == typeof(DateTime))
                return DbfColumnType.Date;

            throw new NotSupportedException($"{type.Name} does not have a corresponding dbase type.");

        }

        public static DbfColumnType GetDbaseType(char c)
        {
            switch (c.ToString().ToUpper())
            {
                case "C": return DbfColumnType.Character;
                case "N": return DbfColumnType.Number;
                case "B": return DbfColumnType.Binary;
                case "L": return DbfColumnType.Boolean;
                case "D": return DbfColumnType.Date;
                case "I": return DbfColumnType.Integer;
                case "M": return DbfColumnType.Memo;
            }

            throw new NotSupportedException($"{c} does not have a corresponding dbase type.");

        }

        /// <summary>
        /// Returns shp file Shape Field.
        /// </summary>
        /// <returns></returns>
        public static DbfColumn ShapeField()
        {
            return new DbfColumn("Geometry", DbfColumnType.Binary);

        }


        /// <summary>
        /// Returns Shp file ID field.
        /// </summary>
        /// <returns></returns>
        public static DbfColumn IdField()
        {
            return new DbfColumn("Row", DbfColumnType.Integer);

        }


    }
}
