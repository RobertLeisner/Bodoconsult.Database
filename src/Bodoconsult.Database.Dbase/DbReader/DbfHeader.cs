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
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bodoconsult.Database.Dbase.DbReader
{

    /// <summary>
    /// This class represents a DBF IV file header.
    /// </summary>
    /// 
    /// <remarks>
    /// DBF files are really wasteful on space but this legacy format lives on because it's really really simple. 
    /// It lacks much in features though.
    /// 
    /// 
    /// Thanks to Erik Bachmann for providing the DBF file structure information!!
    /// http://www.clicketyclick.dk/databases/xbase/format/dbf.html
    /// 
    ///           _______________________  _______
    /// 00h /   0| Version number      *1|  ^
    ///          |-----------------------|  |
    /// 01h /   1| Date of last update   |  |
    /// 02h /   2|      YYMMDD        *21|  |
    /// 03h /   3|                    *14|  |
    ///          |-----------------------|  |
    /// 04h /   4| Number of records     | Record
    /// 05h /   5| in data file          | header
    /// 06h /   6| ( 32 bits )        *14|  |
    /// 07h /   7|                       |  |
    ///          |-----------------------|  |
    /// 08h /   8| Length of header   *14|  |
    /// 09h /   9| structure ( 16 bits ) |  |
    ///          |-----------------------|  |
    /// 0Ah /  10| Length of each record |  |
    /// 0Bh /  11| ( 16 bits )     *2 *14|  |
    ///          |-----------------------|  |
    /// 0Ch /  12| ( Reserved )        *3|  |
    /// 0Dh /  13|                       |  |
    ///          |-----------------------|  |
    /// 0Eh /  14| Incomplete transac.*12|  |
    ///          |-----------------------|  |
    /// 0Fh /  15| Encryption flag    *13|  |
    ///          |-----------------------|  |
    /// 10h /  16| Free record thread    |  |
    /// 11h /  17| (reserved for LAN     |  |
    /// 12h /  18|  only )               |  |
    /// 13h /  19|                       |  |
    ///          |-----------------------|  |
    /// 14h /  20| ( Reserved for        |  |            _        |=======================| ______
    ///          |   multi-user dBASE )  |  |           / 00h /  0| Field name in ASCII   |  ^
    ///          : ( dBASE III+ - )      :  |          /          : (terminated by 00h)   :  |
    ///          :                       :  |         |           |                       |  |
    /// 1Bh /  27|                       |  |         |   0Ah / 10|                       |  |
    ///          |-----------------------|  |         |           |-----------------------| For
    /// 1Ch /  28| MDX flag (dBASE IV)*14|  |         |   0Bh / 11| Field type (ASCII) *20| each
    ///          |-----------------------|  |         |           |-----------------------| field
    /// 1Dh /  29| Language driver     *5|  |        /    0Ch / 12| Field data address    |  |
    ///          |-----------------------|  |       /             |                     *6|  |
    /// 1Eh /  30| ( Reserved )          |  |      /              | (in memory !!!)       |  |
    /// 1Fh /  31|                     *3|  |     /       0Fh / 15| (dBASE III+)          |  |
    ///          |=======================|__|____/                |-----------------------|  |  -
    /// 20h /  32|                       |  |  ^          10h / 16| Field length       *22|  |   |
    ///          |- - - - - - - - - - - -|  |  |                  |-----------------------|  |   | *7
    ///          |                    *19|  |  |          11h / 17| Decimal count      *23|  |   |
    ///          |- - - - - - - - - - - -|  |  Field              |-----------------------|  |  -
    ///          |                       |  | Descriptor  12h / 18| ( Reserved for        |  |
    ///          :. . . . . . . . . . . .:  |  |array     13h / 19|   multi-user dBASE)*18|  |
    ///          :                       :  |  |                  |-----------------------|  |
    ///       n  |                       |__|__v_         14h / 20| Work area ID       *16|  |
    ///          |-----------------------|  |    \                |-----------------------|  |
    ///       n+1| Terminator (0Dh)      |  |     \       15h / 21| ( Reserved for        |  |
    ///          |=======================|  |      \      16h / 22|   multi-user dBASE )  |  |
    ///       m  | Database Container    |  |       \             |-----------------------|  |
    ///          :                    *15:  |        \    17h / 23| Flag for SET FIELDS   |  |
    ///          :                       :  |         |           |-----------------------|  |
    ///     / m+263                      |  |         |   18h / 24| ( Reserved )          |  |
    ///          |=======================|__v_ ___    |           :                       :  |
    ///          :                       :    ^       |           :                       :  |
    ///          :                       :    |       |           :                       :  |
    ///          :                       :    |       |   1Eh / 30|                       |  |
    ///          | Record structure      |    |       |           |-----------------------|  |
    ///          |                       |    |        \  1Fh / 31| Index field flag    *8|  |
    ///          |                       |    |         \_        |=======================| _v_____
    ///          |                       | Records
    ///          |-----------------------|    |
    ///          |                       |    |          _        |=======================| _______
    ///          |                       |    |         / 00h /  0| Record deleted flag *9|  ^
    ///          |                       |    |        /          |-----------------------|  |
    ///          |                       |    |       /           | Data               *10|  One
    ///          |                       |    |      /            : (ASCII)            *17: record
    ///          |                       |____|_____/             |                       |  |
    ///          :                       :    |                   |                       | _v_____
    ///          :                       :____|_____              |=======================|
    ///          :                       :    |
    ///          |                       |    |
    ///          |                       |    |
    ///          |                       |    |
    ///          |                       |    |
    ///          |                       |    |
    ///          |=======================|    |
    ///          |__End_of_File__________| ___v____  End of file ( 1Ah )  *11
    /// 
    /// </remarks>
    public class DbfHeader
    {

        /// <summary>
        /// Header file descriptor size is 33 bytes (32 bytes + 1 terminator byte), followed by column metadata which is 32 bytes each.
        /// </summary>
        public const int FileDescriptorSize = 33;


        /// <summary>
        /// Field or DBF Column descriptor is 32 bytes long.
        /// </summary>
        public const int ColumnDescriptorSize = 32;


        //type of the file, must be 03h
        private const int MFileType = 0x03;

        //Date the file was last updated.
        private DateTime _mUpdateDate;

        //Number of records in the datafile, 32bit little-endian, unsigned 
        private uint _mNumRecords;

        //Length of the header structure

        //Length of the records, ushort - unsigned 16 bit integer

        //DBF fields/columns
        internal List<DbfColumn> MFields;


        //indicates whether header columns can be modified!


        /// <summary>
        /// mEmptyRecord is an array used to clear record data in CDbf4Record.
        /// This is shared by all record objects, used to speed up clearing fields or entire record.
        /// <seealso cref="EmptyDataRecord"/>
        /// </summary>
        private byte[] _mEmptyRecord;



        public DbfHeader()
        {
            //create a list of fields of default size
            MFields = new List<DbfColumn>();

        }


        /// <summary>
        /// Specify initial column capacity.
        /// </summary>
        /// <param name="nFieldCapacity"></param>
        public DbfHeader(int nFieldCapacity)
        {
            MFields = new List<DbfColumn>(nFieldCapacity);

        }


        /// <summary>
        /// Gets header length.
        /// </summary>
        public ushort HeaderLength { get; private set; } = FileDescriptorSize;


        /// <summary>
        /// Add a new column to the DBF header.
        /// </summary>
        /// <param name="oNewCol"></param>
        public void AddColumn(DbfColumn oNewCol)
        {

            //throw exception if the header is locked
            if (Locked)
            {
                throw new Exception("This header is locked and can not be modified. Modifying the header would result in a corrupt DBF file. You can unlock the header by calling UnLock() method.");
            }

            //since we are breaking the spec rules about max number of fields, we should at least 
            //check that the record length stays within a number that can be recorded in the header!
            //we have 2 unsigned bytes for record length for a maximum of 65535.
            if (RecordLength + oNewCol.Length > 65535)
            {
                throw new Exception("Unable to add new column. Adding this column puts the record length over the maximum (which is 65535 bytes).");
            }


            //add the column
            MFields.Add(oNewCol);

            //update offset bits, record and header lengths
            oNewCol.MDataAddress = RecordLength;
            RecordLength += oNewCol.Length;
            HeaderLength += ColumnDescriptorSize;

            //clear empty record
            _mEmptyRecord = null;

            //set dirty bit
            IsDirty = true;

        }


        /// <summary>
        /// Create and add a new column with specified name and type.
        /// </summary>
        /// <param name="sName"></param>
        /// <param name="type"></param>
        public void AddColumn(string sName, DbfColumn.DbfColumnType type)
        {
            AddColumn(new DbfColumn(sName, type));
        }


        /// <summary>
        /// Create and add a new column with specified name, type, length, and decimal precision.
        /// </summary>
        /// <param name="sName">Field name. Uniqueness is not enforced.</param>
        /// <param name="type"></param>
        /// <param name="nLength">Length of the field including decimal point and decimal numbers</param>
        /// <param name="nDecimals">Number of decimal places to keep.</param>
        public void AddColumn(string sName, DbfColumn.DbfColumnType type, int nLength, int nDecimals)
        {
            AddColumn(new DbfColumn(sName, type, nLength, nDecimals));
        }


        /// <summary>
        /// Remove column from header definition.
        /// </summary>
        /// <param name="nIndex"></param>
        public void RemoveColumn(int nIndex)
        {
            //throw exception if the header is locked
            if (Locked)
            {
                throw new Exception("This header is locked and can not be modified. Modifying the header would result in a corrupt DBF file. You can unlock the header by calling UnLock() method.");
            }


            var oColRemove = MFields[nIndex];
            MFields.RemoveAt(nIndex);


            oColRemove.MDataAddress = 0;
            RecordLength -= oColRemove.Length;
            HeaderLength -= ColumnDescriptorSize;

            //if you remove a column offset shift for each of the columns 
            //following the one removed, we need to update those offsets.
            var nRemovedColLen = oColRemove.Length;
            for (var i = nIndex; i < MFields.Count; i++)
                MFields[i].MDataAddress -= nRemovedColLen;

            //clear the empty record
            _mEmptyRecord = null;

            //set dirty bit
            IsDirty = true;

        }


        /// <summary>
        /// Look up a column index by name. Note that this is case insensitive and this implementation does not do any optmizations on the lookup.
        /// We simply look for the column by checking every one from first to last until found or -1 on not found. 
        /// String.Compare() function is used for comparisons.
        /// </summary>
        /// <param name="sName"></param>
        /// <returns>Index or -1 if not found.</returns>
        public DbfColumn this[string sName]
        {
            get
            {
                for (var i = 0; i < MFields.Count; i++)
                {
                    if (string.Compare(MFields[i].Name, sName, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        return MFields[i];
                    }
                }

                return null;
            }
        }


        /// <summary>
        /// Returns column at specified index. Index is 0 based.
        /// </summary>
        /// <param name="nIndex">Zero based index.</param>
        /// <returns></returns>
        public DbfColumn this[int nIndex] => MFields[nIndex];


        /// <summary>
        /// Finds a column index by searching sequentially through the list. Case is ignored. Returns -1 if not found.
        /// </summary>
        /// <param name="sName">Column name</param>
        /// <returns>column index (0 based) or -1 if not found.</returns>
        public int FindColumn(string sName)
        {
            for (var i = 0; i < MFields.Count; i++)
            {
                if (string.Compare(MFields[i].Name, sName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return i;
                }
            }

            return -1;

        }


        /// <summary>
        /// Returns an empty data record. This is used to clear columns 
        /// </summary>
        /// <remarks>
        /// The reason we put this in the header class is because it allows us to use the CDbf4Record class in two ways.
        /// 1. we can create one instance of the record and reuse it to write many records quickly clearing the data array by bitblting to it.
        /// 2. we can create many instances of the record (a collection of records) and have only one copy of this empty dataset for all of them.
        ///    If we had put it in the Record class then we would be taking up twice as much space unnecessarily. The empty record also fits the model
        ///    and everything is neatly encapsulated and safe.
        /// 
        /// </remarks>
        protected internal byte[] EmptyDataRecord =>
            _mEmptyRecord ?? (_mEmptyRecord =
                Encoding.ASCII.GetBytes("".PadLeft(RecordLength, ' ').ToCharArray()));


        /// <summary>
        /// Returns Number of columns in this dbf header.
        /// </summary>
        public int ColumnCount => MFields.Count;


        /// <summary>
        /// Size of one record in bytes. All fields + 1 byte delete flag.
        /// </summary>
        public int RecordLength { get; private set; } = 1;


        /// <summary>
        /// Get/Set number of records in the DBF.
        /// </summary>
        /// <remarks>
        /// The reason we allow client to set RecordCount is beause in certain streams 
        /// like internet streams we can not update record count as we write out records, we have to set it in advance,
        /// so client has to be able to modify this property.
        /// </remarks>
        public uint RecordCount
        {
            get => _mNumRecords;

            set
            {
                _mNumRecords = value;

                //set the dirty bit
                IsDirty = true;

            }
        }


        /// <summary>
        /// Get/set whether this header is read only or can be modified. When you create a CDbfRecord 
        /// object and pass a header to it, CDbfRecord locks the header so that it can not be modified any longer.
        /// in order to preserve DBF integrity.
        /// </summary>
        internal bool Locked { get; set; }


        /// <summary>
        /// Use this method with caution. Headers are locked for a reason, to prevent DBF from becoming corrupt.
        /// </summary>
        public void Unlock()
        {
            Locked = false;
        }


        /// <summary>
        /// Returns true when this object is modified after read or write.
        /// </summary>
        public bool IsDirty { get; set; }


        /// <summary>
        /// Encoding must be ASCII for this binary writer.
        /// </summary>
        /// <param name="writer"></param>
        /// <remarks>
        /// See class remarks for DBF file structure.
        /// </remarks>
        public void Write(BinaryWriter writer)
        {

            //write the header
            // write the output file type.
            writer.Write((byte)MFileType);

            //Update date format is YYMMDD, which is different from the column Date type (YYYYDDMM)
            writer.Write((byte)(_mUpdateDate.Year - 1900));
            writer.Write((byte)_mUpdateDate.Month);
            writer.Write((byte)_mUpdateDate.Day);

            // write the number of records in the datafile. (32 bit number, little-endian unsigned)
            writer.Write(_mNumRecords);

            // write the length of the header structure.
            writer.Write(HeaderLength);

            // write the length of a record
            writer.Write((ushort)RecordLength);

            // write the reserved bytes in the header
            for (var i = 0; i < 20; i++)
                writer.Write((byte)0);

            // write all of the header records
            var byteReserved = new byte[14];  //these are initialized to 0 by default.
            for (var i = 0; i < MFields.Count; i++)
            {
                var cname = MFields[i].Name.PadRight(11, (char)0).ToCharArray();
                writer.Write(cname);

                // write the field type
                writer.Write(MFields[i].ColumnTypeChar);

                // write the field data address, offset from the start of the record.
                writer.Write(MFields[i].DataAddress);


                // write the length of the field.
                // if char field is longer than 255 bytes, then we use the decimal field as part of the field length.
                if (MFields[i].ColumnType == DbfColumn.DbfColumnType.Character && MFields[i].Length > 255)
                {
                    //treat decimal count as high byte of field length, this extends char field max to 65535
                    writer.Write((ushort)MFields[i].Length);

                }
                else
                {
                    // write the length of the field.
                    writer.Write((byte)MFields[i].Length);

                    // write the decimal count.
                    writer.Write((byte)MFields[i].DecimalCount);
                }

                // write the reserved bytes.
                writer.Write(byteReserved);

            }

            // write the end of the field definitions marker
            writer.Write((byte)0x0D);
            writer.Flush();

            //clear dirty bit
            IsDirty = false;


            //lock the header so it can not be modified any longer, 
            //we could actually postpond this until first record is written!
            Locked = true;


        }


        /// <summary>
        /// Read header data, make sure the stream is positioned at the start of the file to read the header otherwise you will get an exception.
        /// When this function is done the position will be the first record.
        /// </summary>
        /// <param name="reader"></param>
        public void Read(BinaryReader reader)
        {

            // type of reader.
            int nFileType = reader.ReadByte();

            if (nFileType != 0x03 && nFileType != 245)
            {
                throw new NotSupportedException("Unsupported DBF reader Type " + nFileType);
            }

            // parse the update date information.
            var year = (int)reader.ReadByte();
            var month = (int)reader.ReadByte();
            var day = (int)reader.ReadByte();
            _mUpdateDate = new DateTime(year + 1900, month, day);

            // read the number of records.
            _mNumRecords = reader.ReadUInt32();

            // read the length of the header structure.
            HeaderLength = reader.ReadUInt16();

            // read the length of a record
            RecordLength = reader.ReadInt16();

            // skip the reserved bytes in the header.
            reader.ReadBytes(20);

            // calculate the number of Fields in the header
            var nNumFields = (HeaderLength - FileDescriptorSize) / ColumnDescriptorSize;

            //offset from start of record, start at 1 because that's the delete flag.
            var nDataOffset = 1;

            // read all of the header records
            MFields = new List<DbfColumn>(nNumFields);
            for (var i = 0; i < nNumFields; i++)
            {

                // read the field name				
                var buffer = reader.ReadChars(11);
                var sFieldName = new string(buffer);
                var nullPoint = sFieldName.IndexOf((char)0);
                if (nullPoint != -1)
                    sFieldName = sFieldName.Substring(0, nullPoint);


                //read the field type
                var cDbaseType = (char)reader.ReadByte();

                // read the field data address, offset from the start of the record.
                var nFieldDataAddress = reader.ReadInt32();


                //read the field length in bytes
                //if field type is char, then read FieldLength and Decimal count as one number to allow char fields to be
                //longer than 256 bytes (ASCII char). This is the way Clipper and FoxPro do it, and there is really no downside
                //since for char fields decimal count should be zero for other versions that do not support this extended functionality.
                //-----------------------------------------------------------------------------------------------------------------------
                var nFieldLength = 0;
                var nDecimals = 0;
                if (cDbaseType == 'C' || cDbaseType == 'c')
                {
                    //treat decimal count as high byte
                    nFieldLength = reader.ReadUInt16();
                }
                else
                {
                    //read field length as an unsigned byte.
                    nFieldLength = reader.ReadByte();

                    //read decimal count as one byte
                    nDecimals = reader.ReadByte();

                }


                //read the reserved bytes.
                reader.ReadBytes(14);

                //Create and add field to collection
                MFields.Add(new DbfColumn(sFieldName, DbfColumn.GetDbaseType(cDbaseType), nFieldLength, nDecimals, nDataOffset));

                // add up address information, you can not trust the address recorded in the DBF file...
                nDataOffset += nFieldLength;

            }

            // Last byte is a marker for the end of the field definitions.
            reader.ReadBytes(1);


            //read any extra header bytes...move to first record
            //equivalent to reader.BaseStream.Seek(mHeaderLength, SeekOrigin.Begin) except that we are not using the seek function since
            //we need to support streams that can not seek like web connections.
            var nExtraReadBytes = HeaderLength - (FileDescriptorSize + ColumnDescriptorSize * MFields.Count);
            if (nExtraReadBytes > 0)
            {
                reader.ReadBytes(nExtraReadBytes);
            }



            //if the stream is not forward-only, calculate number of records using file size, 
            //sometimes the header does not contain the correct record count
            //if we are reading the file from the web, we have to use ReadNext() functions anyway so
            //Number of records is not so important and we can trust the DBF to have it stored correctly.
            if (reader.BaseStream.CanSeek && _mNumRecords == 0)
            {
                //notice here that we subtract file end byte which is supposed to be 0x1A,
                //but some DBF files are incorrectly written without this byte, so we round off to nearest integer.
                //that gives a correct result with or without ending byte.
                if (RecordLength > 0)
                {
                    _mNumRecords = (uint)Math.Round((double)(reader.BaseStream.Length - HeaderLength - 1) / RecordLength);
                }

            }


            //lock header since it was read from a file. we don't want it modified because that would corrupt the file.
            //user can override this lock if really necessary by calling UnLock() method.
            Locked = true;

            //clear dirty bit
            IsDirty = false;

        }

        public static string GetMemoField(int blockIndex, string dbtFile)
        {
            using (var fs = new FileStream(dbtFile, FileMode.Open, FileAccess.Read))
            {
                var m = new MemoCollection(fs);
                foreach (var v in m)
                {

                }
            }

            //throw new InvalidOperationException();

            return string.Empty;
        }
    }
}
