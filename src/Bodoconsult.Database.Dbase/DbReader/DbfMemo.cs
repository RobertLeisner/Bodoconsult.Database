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
using System.IO;
using System.Text;

namespace Bodoconsult.Database.Dbase.DbReader
{
    /// <summary>
    /// Represents the data of a memo file with file extension .fpt
    /// </summary>
    public class DbfMemo: IDisposable
    {
        private readonly BinaryReader _br;


        /// <summary>
        /// File header data of the FPT memo file
        /// </summary>
        public MemoFileHeader FileHeader { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="fileName"></param>
        public DbfMemo(string fileName)
        {

            try
            {
                _br = new BinaryReader(File.OpenRead(fileName));
            }
            catch (Exception e)
            {

                throw new FileLoadException($"Memo file {fileName} could not be opened.", e);
            }
        }

        /// <summary>
        /// Load the meta data from the memo file
        /// </summary>
        public void LoadMetaData()
        {
            FileHeader = new MemoFileHeader(_br);
        }

        /// <summary>
        /// Read the string content from the byte buffer
        /// </summary>
        /// <param name="header">Current memo block header</param>
        /// <returns>String content of the memo field</returns>
        private string GetRawContent(MemoBlockHeader header)
        {
            var buffer = new byte[header.ContentSize];
            _br.BaseStream.Seek(header.StartLocation, SeekOrigin.Begin);
            _br.Read(buffer, 0, header.ContentSize);

            return Encoding.UTF8.GetString(buffer);
        }

        /// <summary>
        /// Read the strong content for a certain pointer
        /// </summary>
        /// <param name="pointer">Pointer</param>
        /// <returns>Content string of the memo field</returns>
        public string GetContent(int pointer)
        {

            var header = new MemoBlockHeader(pointer, _br, FileHeader.BlockSize);

            var s = GetRawContent(header);

            return s;
        }

        /// <summary>Disposes open ressources</summary>
        public void Dispose()
        {
            _br?.Dispose();
        }
    }
}
