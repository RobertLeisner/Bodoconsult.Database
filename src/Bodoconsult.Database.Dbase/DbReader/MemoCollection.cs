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
using System.Linq;

namespace Bodoconsult.Database.Dbase.DbReader
{
    public class MemoCollection : IEnumerable<MemoBlock>
    {
        private readonly Stream _stream;
        private int _blockSize;

        public MemoCollection(Stream memoStream)
        {
            _stream = memoStream;
        }

        #region IEnumerable<MemoBlock> Members

        public IEnumerator<MemoBlock> GetEnumerator()
        {
            return GetCollection();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private IEnumerator<MemoBlock> GetCollection()
        {
            _stream.Seek(0, SeekOrigin.Begin);
            GetBlockSize();
            while (_stream.Position < _stream.Length)
            {
                var memo = ReadMemo();
                PositionToNextBlock();
                yield return memo;
            }
        }

        private void PositionToNextBlock()
        {
            var unusedbytecount = (int)(_stream.Position % _blockSize);
            if (unusedbytecount > 0)
            {
                _stream.Seek(_blockSize - unusedbytecount, SeekOrigin.Current);
            }
        }

        private MemoBlock ReadMemo()
        {
            var header = new byte[8];
            _stream.Read(header, 0, 8);
            var type = (MemoDataType)ToInteger(header.Take(4).ToArray());
            var memoSize = ToInteger(header.Skip(4).Take(4).ToArray());
            var memoData = new byte[memoSize];
            _stream.Read(memoData, 0, memoData.Length);
            return new MemoBlock(type, memoData);

        }

        private void GetBlockSize()
        {
            var buffer = new byte[512];
            _stream.Read(buffer, 0, buffer.Length);
            _blockSize = ToInteger(new byte[] { 0, 0, buffer[6], buffer[7] });

        }

        private int ToInteger(byte[] data)
        {
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(data);
            }
            return BitConverter.ToInt32(data, 0);
        }

        #endregion
    }
}