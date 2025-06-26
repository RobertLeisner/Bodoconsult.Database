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

namespace Bodoconsult.Database.Dbase.DbReader
{
    public class FastDbf : IDisposable
    {
        #region Private Fields

        private readonly int _codepage;
        private DbfFile _odbf;
        private readonly DbfRecord _orec;

        private readonly DbfMemo _ofpt;

        #endregion Private Fields

        #region Public Constructors

        public FastDbf(string fname, int codepage = 1252)
        {
            _codepage = codepage;
            if (!File.Exists(fname))
            {
                throw new FileNotFoundException(fname);
            }

            _odbf = new DbfFile();
            _odbf.Open(fname, FileMode.Open);

            var fi = new FileInfo(fname);

            var memoFile = fi.FullName.Replace(fi.Extension, ".fpt");

            if (File.Exists(memoFile))
            {

                _ofpt = new DbfMemo(memoFile);
                _ofpt.LoadMetaData();
            }


            _orec = new DbfRecord(_odbf.Header, codepage)
                {
                    MemoSource = _ofpt
                };
        }

        #endregion Public Constructors

        #region Public Properties

        public int ColumnCount => _odbf.Header.ColumnCount;
        public uint RecordCount => _odbf.Header.RecordCount;

        #endregion Public Properties

        #region Public Methods

        public static DbfResult Read(string fname, int codepage = 1252)
        {
            if (!File.Exists(fname))
            {
                throw new FileNotFoundException(fname);
            }

            var odbf = new DbfFile();
            odbf.Open(fname, FileMode.Open);

            DbfMemo ofpt = null;

            var fi = new FileInfo(fname);

            var memoFile = fi.FullName.Replace(fi.Extension, ".fpt");

            if (File.Exists(memoFile))
            {
                ofpt = new DbfMemo(memoFile);
                ofpt.LoadMetaData();
            }

            var orec = new DbfRecord(odbf.Header, codepage)
            {
                MemoSource = ofpt
            };

            var retval = new DbfResult { DbfHeader = odbf.Header };

            try
            {
                for (var i = 0; i < odbf.Header.RecordCount; i++)
                {
                    if (!odbf.Read(i, orec))
                    {
                        break;
                    }

                    if (orec.IsDeleted)
                    {
                        continue;
                    }

                    var record = GetRecord(odbf.Header, orec);

                    retval.DbfRecords.Add(record);
                }
            }
            finally
            {
                odbf.Close();
            }

            return retval;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public int? FindIndex(Func<List<string>, bool> conditionFunc)
        {
            try
            {
                for (var i = 0; i < RecordCount; i++)
                {
                    if (!_odbf.Read(i, _orec))
                    {
                        break;
                    }

                    if (_orec.IsDeleted)
                    {
                        continue;
                    }

                    var record = GetRecord(_odbf.Header, _orec);
                    if (conditionFunc(record))
                    {
                        return i;
                    }
                }
            }
            catch
            {
                _odbf.Close();
                _odbf = null;
                throw;
            }

            return null;
        }

        public DbfResult Read(int index, int count = 1, Func<List<string>, bool> filter = null, List<int> skipIndexes = null)
        {
            if (filter == null)
            {
                filter = x => true;
            }

            var retval = new DbfResult
            {
                DbfHeader = _odbf.Header
            };

            try
            {
                for (var i = 0; i < count && i + index < RecordCount; i++)
                {
                    if (skipIndexes != null && skipIndexes.Contains(index + i))
                    {
                        continue;
                    }

                    if (!_odbf.Read(index + i, _orec))
                    {
                        break;
                    }

                    if (_orec.IsDeleted)
                    {
                        continue;
                    }

                    var record = GetRecord(_odbf.Header, _orec);
                    if (filter(record))
                    {
                        retval.DbfRecords.Add(record);
                    }
                }
            }
            catch
            {
                _odbf.Close();
                _odbf = null;
            }

            return retval;
        }

        #endregion Public Methods

        #region Protected Methods

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }
            _odbf?.Close();
            _odbf = null;
        }

        #endregion Protected Methods

        #region Private Methods

        private static List<string> GetRecord(DbfHeader dbfHeader, DbfRecord orec)
        {
            var record = new List<string>();

            for (var j = 0; j < dbfHeader.ColumnCount; j++)
            {
                //var header = odbf.Header[j];
                //if (header.ColumnType == DbfColumn.DbfColumnType.Character)
                //{
                //    var data = orec.RecordData.ToList().Skip(header.DataAddress).Take(header.Length);
                //    foreach (var encodingInfo in Encoding.GetEncodings())
                //    {
                //        Debug.Write(encodingInfo.GetEncoding().GetString(data.ToArray()) + "\t");
                //        Debug.WriteLine(string.Format("DisplayName:{0} CodePage:{1} Name:{2} ", encodingInfo.DisplayName, encodingInfo.CodePage, encodingInfo.Name));
                //    }
                //    var encoding = Encoding.GetEncoding(852);

                //}
                //else
                //{
                    record.Add(orec[j]);
                //}
            }
            return record;
        }

        #endregion Private Methods
    }
}