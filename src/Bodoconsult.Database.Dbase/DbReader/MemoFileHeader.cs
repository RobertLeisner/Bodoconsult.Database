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

using System.IO;

namespace Bodoconsult.Database.Dbase.DbReader
{
    /// <summary>
    /// Memo file metadata header
    /// </summary>
    public class MemoFileHeader
    {
        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="br">Content of the memo file</param>
        public MemoFileHeader(BinaryReader br)
        {

            br.BaseStream.Seek(6, SeekOrigin.Begin);

            var buffer = new byte[4];
            br.Read(buffer, 2, 2);

            BlockSize = DbfHelper.ToInteger(buffer);

        }

        /// <summary>
        /// Block size of the memo content
        /// </summary>
        public int BlockSize { get; set; }
    }
}