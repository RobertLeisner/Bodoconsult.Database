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
using System.Globalization;
using System.IO;
using System.Text;

namespace Bodoconsult.Database.Dbase.DbReader
{

    /// <summary>
    /// Use this class to create a record and write it to a dbf file. You can use one record object to write all records!!
    /// It was designed for this kind of use. You can do this by clearing the record of all data 
    /// (call Clear() method) or setting values to all fields again, then write to dbf file. 
    /// This eliminates creating and destroying objects and optimizes memory use.
    /// 
    /// Once you create a record the header can no longer be modified, since modifying the header would make a corrupt DBF file.
    /// </summary>
    public class DbfRecord
    {


        /// <summary>
        /// The current memo data source
        /// </summary>
        public DbfMemo MemoSource { get; set; }



        public byte[] RecordData => _mData;

        /// <summary>
        /// Dbf data are a mix of ASCII characters and binary, which neatly fit in a byte array.
        /// BinaryWriter would esentially perform the same conversion using the same Encoding class.
        /// </summary>
        private readonly byte[] _mData;

        /// <summary>
        /// Empty Record array reference used to clear fields quickly (or entire record).
        /// </summary>
        private readonly byte[] _mEmptyRecord;


        //array used to clear decimals, we can clear up to 40 decimals which is much more than is allowed under DBF spec anyway.
        //Note: 48 is ASCII code for 0.
        private static readonly byte[] MDecimalClear = {48,48,48,48,48,48,48,48,48,48,48,48,48,48,48,
                                                               48,48,48,48,48,48,48,48,48,48,48,48,48,48,48,
                                                               48,48,48,48,48,48,48,48,48,48,48,48,48,48,48};


        //Warning: do not make this one static because that would not be thread safe!! The reason I have 
        //placed this here is to skip small memory allocation/deallocation which fragments memory in .net.
        private readonly int[] _mTempIntVal = { 0 };


        //Ascii Encoder
        //private static readonly Encoding ASCIIEncoder = Encoding.GetEncoding(1250);
        private static readonly Encoding AsciiEncoder = Encoding.GetEncoding(_codePage);


        private static int _codePage;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oHeader">Dbf Header will be locked once a record is created 
        /// since the record size is fixed and if the header was modified it would corrupt the DBF file.</param>
        /// <param name="codepage"> </param>
        public DbfRecord(DbfHeader oHeader, int codepage = 850)
        {
            Header = oHeader;
            Header.Locked = true;
            _codePage = codepage;

            //create a buffer to hold all record data. We will reuse this buffer to write all data to the file.
            _mData = new byte[Header.RecordLength];
            _mEmptyRecord = Header.EmptyDataRecord;

        }



        /// <summary>
        /// Set string data to a column, if the string is longer than specified column length it will be truncated!
        /// If dbf column type is not a string, input will be treated as dbf column 
        /// type and if longer than length an exception will be thrown.
        /// </summary>
        /// <param name="nColIndex"></param>
        /// <returns></returns>
        public string this[int nColIndex]
        {

            set
            {

                var ocol = Header[nColIndex];
                var ocolType = ocol.ColumnType;


                //
                //if an empty value is passed, we just clear the data, and leave it blank.
                //note: test have shown that testing for null and checking length is faster than comparing to "" empty str :)
                //------------------------------------------------------------------------------------------------------------
                if (string.IsNullOrEmpty(value))
                {
                    //this is like NULL data, set it to empty. i looked at SAS DBF output when a null value exists 
                    //and empty data are output. we get the same result, so this looks good.
                    Buffer.BlockCopy(_mEmptyRecord, ocol.DataAddress, _mData, ocol.DataAddress, ocol.Length);

                }
                else
                {

                    //set values according to data type:
                    //-------------------------------------------------------------
                    if (ocolType == DbfColumn.DbfColumnType.Character)
                    {
                        if (!AllowStringTurncate && value.Length > ocol.Length)
                        {
                            throw new DbfDataTruncateException("Value not set. String truncation would occur and AllowStringTruncate flag is set to false. To supress this exception change AllowStringTruncate to true.");
                        }

                        //BlockCopy copies bytes.  First clear the previous value, then set the new one.
                        Buffer.BlockCopy(_mEmptyRecord, ocol.DataAddress, _mData, ocol.DataAddress, ocol.Length);
                        AsciiEncoder.GetBytes(value, 0, value.Length > ocol.Length ? ocol.Length : value.Length, _mData, ocol.DataAddress);

                    }
                    else if (ocolType == DbfColumn.DbfColumnType.Number)
                    {

                        if (ocol.DecimalCount == 0)
                        {

                            //integers
                            //----------------------------------

                            //throw an exception if integer overflow would occur
                            if (!AllowIntegerTruncate && value.Length > ocol.Length)
                            {
                                throw new DbfDataTruncateException("Value not set. Integer does not fit and would be truncated. AllowIntegerTruncate is set to false. To supress this exception set AllowIntegerTruncate to true, although that is not recomended.");
                            }


                            //clear all numbers, set to [space].
                            //-----------------------------------------------------
                            Buffer.BlockCopy(_mEmptyRecord, 0, _mData, ocol.DataAddress, ocol.Length);


                            //set integer part, CAREFUL not to overflow buffer! (truncate instead)
                            //-----------------------------------------------------------------------
                            var nNumLen = value.Length > ocol.Length ? ocol.Length : value.Length;
                            AsciiEncoder.GetBytes(value, 0, nNumLen, _mData, ocol.DataAddress + ocol.Length - nNumLen);

                        }
                        else
                        {

                            //TODO: we can improve perfomance here by not using temp char arrays cDec and cNum,
                            //simply direcly copy from source string using AsciiEncoder!


                            //break value down into integer and decimal portions
                            //--------------------------------------------------------------------------
                            var nidxDecimal = value.IndexOf('.'); //index where the decimal point occurs
                            char[] cDec = null; //decimal portion of the number
                            char[] cNum; //integer portion

                            if (nidxDecimal > -1)
                            {
                                cDec = value.Substring(nidxDecimal + 1).ToCharArray();
                                cNum = value.Substring(0, nidxDecimal).ToCharArray();

                                //throw an exception if decimal overflow would occur
                                if (!AllowDecimalTruncate && cDec.Length > ocol.DecimalCount)
                                {
                                    throw new DbfDataTruncateException("Value not set. Decimal does not fit and would be truncated. AllowDecimalTruncate is set to false. To supress this exception set AllowDecimalTruncate to true.");
                                }

                            }
                            else
                                cNum = value.ToCharArray();


                            //throw an exception if integer overflow would occur
                            if (!AllowIntegerTruncate && cNum.Length > ocol.Length - ocol.DecimalCount - 1)
                            {
                                throw new DbfDataTruncateException("Value not set. Integer does not fit and would be truncated. AllowIntegerTruncate is set to false. To supress this exception set AllowIntegerTruncate to true, although that is not recomended.");
                            }



                            //clear all decimals, set to 0.
                            //-----------------------------------------------------
                            Buffer.BlockCopy(MDecimalClear, 0, _mData, ocol.DataAddress + ocol.Length - ocol.DecimalCount, ocol.DecimalCount);

                            //clear all numbers, set to [space].
                            Buffer.BlockCopy(_mEmptyRecord, 0, _mData, ocol.DataAddress, ocol.Length - ocol.DecimalCount);



                            //set decimal numbers, CAREFUL not to overflow buffer! (truncate instead)
                            //-----------------------------------------------------------------------
                            if (nidxDecimal > -1)
                            {
                                var nLen = cDec.Length > ocol.DecimalCount ? ocol.DecimalCount : cDec.Length;
                                AsciiEncoder.GetBytes(cDec, 0, nLen, _mData, ocol.DataAddress + ocol.Length - ocol.DecimalCount);
                            }

                            //set integer part, CAREFUL not to overflow buffer! (truncate instead)
                            //-----------------------------------------------------------------------
                            var nNumLen = cNum.Length > ocol.Length - ocol.DecimalCount - 1 ? ocol.Length - ocol.DecimalCount - 1 : cNum.Length;
                            AsciiEncoder.GetBytes(cNum, 0, nNumLen, _mData, ocol.DataAddress + ocol.Length - ocol.DecimalCount - nNumLen - 1);


                            //set decimal point
                            //-----------------------------------------------------------------------
                            _mData[ocol.DataAddress + ocol.Length - ocol.DecimalCount - 1] = (byte)'.';


                        }


                    }
                    else if (ocolType == DbfColumn.DbfColumnType.Integer)
                    {
                        //note this is a binary Integer type!
                        //----------------------------------------------

                        //TODO: maybe there is a better way to copy 4 bytes from int to byte array. Some memory function or something.
                        _mTempIntVal[0] = Convert.ToInt32(value);
                        Buffer.BlockCopy(_mTempIntVal, 0, _mData, ocol.DataAddress, 4);

                    }
                    else if (ocolType == DbfColumn.DbfColumnType.Memo)
                    {
                        //copy 10 digits...
                        //TODO: implement MEMO

                        throw new Exception("Memo data type functionality not implemented yet!");

                    }
                    else if (ocolType == DbfColumn.DbfColumnType.Boolean)
                    {
                        if (string.Compare(value, "true", StringComparison.OrdinalIgnoreCase) == 0 ||
                            string.Compare(value, "1", StringComparison.OrdinalIgnoreCase) == 0 ||
                            string.Compare(value, "T", StringComparison.OrdinalIgnoreCase) == 0 ||
                            string.Compare(value, "yes", StringComparison.OrdinalIgnoreCase) == 0 ||
                            string.Compare(value, "Y", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            _mData[ocol.DataAddress] = (byte)'T';
                        }
                        else if (value == " " || value == "?")
                        {
                            _mData[ocol.DataAddress] = (byte)'?';
                        }
                        else
                        {
                            _mData[ocol.DataAddress] = (byte)'F';
                        }

                    }
                    else if (ocolType == DbfColumn.DbfColumnType.Date)
                    {
                        //try to parse out date value using Date.Parse() function, then set the value
                        if (DateTime.TryParse(value, out var dateval))
                        {
                            SetDateValue(nColIndex, dateval);
                        }
                        else
                        {
                            throw new InvalidOperationException("Date could not be parsed from source string! Please parse the Date and set the value (you can try using DateTime.Parse() or DateTime.TryParse() functions).");
                        }

                    }
                    else if (ocolType == DbfColumn.DbfColumnType.Binary)
                    {
                        throw new InvalidOperationException("Can not use string source to set binary data. Use SetBinaryValue() and GetBinaryValue() functions instead.");
                    }

                    else
                    {
                        throw new Exception($"Unrecognized data type: {ocolType}");
                    }

                }

            }

            get
            {
                var ocol = Header[nColIndex];

                var data = AsciiEncoder.GetChars(_mData, ocol.DataAddress, ocol.Length);

                var s = new string(data);

                var ocolType = ocol.ColumnType;

                if (MemoSource == null || ocolType != DbfColumn.DbfColumnType.Memo)
                {
                    return s;
                }

                var result = int.TryParse(s, 
                    NumberStyles.Integer, 
                    CultureInfo.InvariantCulture, 
                    out var pointer);

                if (result)
                {
                    s = MemoSource.GetContent(pointer);
                }

                return s;

            }
        }


        /// <summary>
        /// Get date value.
        /// </summary>
        /// <param name="nColIndex"></param>
        /// <returns></returns>
        public DateTime GetDateValue(int nColIndex)
        {
            var ocol = Header[nColIndex];

            if (ocol.ColumnType != DbfColumn.DbfColumnType.Date)
            {
                throw new Exception("Invalid data type. Column '" + ocol.Name + "' is not a date column.");
            }


            var sDateVal = AsciiEncoder.GetString(_mData, ocol.DataAddress, ocol.Length);
            return DateTime.ParseExact(sDateVal, "yyyyMMdd", CultureInfo.InvariantCulture);

        }


        /// <summary>
        /// Get date value.
        /// </summary>
        /// <param name="nColIndex"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public void SetDateValue(int nColIndex, DateTime value)
        {

            var ocol = Header[nColIndex];
            var ocolType = ocol.ColumnType;


            if (ocolType == DbfColumn.DbfColumnType.Date)
            {

                //Format date and set value, date format is like this: yyyyMMdd
                //-------------------------------------------------------------
                AsciiEncoder.GetBytes(value.ToString("yyyyMMdd"), 0, ocol.Length, _mData, ocol.DataAddress);

            }
            else
            {
                throw new Exception($"Invalid data type. Column is of '{ocol.ColumnType}' type, not date.");
            }
        }


        /// <summary>
        /// Clears all data in the record.
        /// </summary>
        public void Clear()
        {
            Buffer.BlockCopy(_mEmptyRecord, 0, _mData, 0, _mEmptyRecord.Length);
            RecordIndex = -1;

        }


        /// <summary>
        /// returns a string representation of this record.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return new string(AsciiEncoder.GetChars(_mData));
        }


        /// <summary>
        /// Gets/sets a zero based record index. This information is not directly stored in DBF. 
        /// It is the location of this record within the DBF. 
        /// </summary>
        /// <remarks>
        /// This property is managed from outside this object,
        /// CDbfFile object updates it when records are read. The reason we don't set it in the Read() 
        /// function within this object is that the stream can be forward-only so the Position property 
        /// is not available and there is no way to figure out what index the record was unless you 
        /// count how many records were read, and that's exactly what CDbfFile does.
        /// </remarks>
        public int RecordIndex { get; set; } = -1;


        /// <summary>
        /// Returns/sets flag indicating whether this record was tagged deleted. 
        /// </summary>
        /// <remarks>Use CDbf4File.Compress() function to rewrite dbf removing records flagged as deleted.</remarks>
        /// <seealso cref="CDbf4File.Compress() function"/>
        public bool IsDeleted
        {
            get => _mData[0] == '*';

            set
            {
                if (value)
                {
                    _mData[0] = (byte)'*';
                }
                else
                {
                    _mData[0] = (byte)' ';
                }

            }

        }


        /// <summary>
        /// Specifies whether strings can be truncated. If false and string is longer than can fit in the field, an exception is thrown.
        /// Default is True.
        /// </summary>
        public bool AllowStringTurncate { get; set; } = true;

        /// <summary>
        /// Specifies whether to allow the decimal portion of numbers to be truncated. 
        /// If false and decimal digits overflow the field, an exception is thrown. Default is false.
        /// </summary>
        public bool AllowDecimalTruncate { get; set; }


        /// <summary>
        /// Specifies whether integer portion of numbers can be truncated.
        /// If false and integer digits overflow the field, an exception is thrown. 
        /// Default is False.
        /// </summary>
        public bool AllowIntegerTruncate { get; set; }


        /// <summary>
        /// Returns header object associated with this record.
        /// </summary>
        public DbfHeader Header { get; }


        /// <summary>
        /// Get column by index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public DbfColumn Column(int index)
        {
            return Header[index];
        }

        /// <summary>
        /// Get column by name.
        /// </summary>
        /// <param name="sName"></param>
        /// <returns></returns>
        public DbfColumn Column(string sName)
        {
            return Header[sName];
        }

        /// <summary>
        /// Gets column count from header.
        /// </summary>
        public int ColumnCount => Header.ColumnCount;

        /// <summary>
        /// Finds a column index by searching sequentially through the list. Case is ignored. Returns -1 if not found.
        /// </summary>
        /// <param name="sName">Column name.</param>
        /// <returns>Column index (0 based) or -1 if not found.</returns>
        public int FindColumn(string sName)
        {
            return Header.FindColumn(sName);
        }

        /// <summary>
        /// Writes data to stream. Make sure stream is positioned correctly because we simply write out the data to it.
        /// </summary>
        /// <param name="osw"></param>
        protected internal void Write(Stream osw)
        {
            osw.Write(_mData, 0, _mData.Length);

        }


        /// <summary>
        /// Writes data to stream. Make sure stream is positioned correctly because we simply write out data to it, and clear the record.
        /// </summary>
        /// <param name="obw"></param>
        /// <param name="bClearRecordAfterWrite"></param>
        protected internal void Write(Stream obw, bool bClearRecordAfterWrite)
        {
            obw.Write(_mData, 0, _mData.Length);

            if (bClearRecordAfterWrite)
            {
                Clear();
            }

        }


        /// <summary>
        /// Read record from stream. Returns true if record read completely, otherwise returns false.
        /// </summary>
        /// <param name="obr"></param>
        /// <returns></returns>
        protected internal bool Read(Stream obr)
        {
            return obr.Read(_mData, 0, _mData.Length) >= _mData.Length;
        }
    }
}
